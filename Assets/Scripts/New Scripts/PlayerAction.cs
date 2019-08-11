using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private float m_PickupRadius = 1f;
    private float m_WalkDropForce = 50f;
    private float m_MinDropForce = 120f;
    private float m_ItemSpawnFromPlayerHeight = 1.0f;
    private float m_ChargeShoveIncrement = 12;
    private float m_MinChargeShovePressure = 3;
    private float m_MaxChargeShovePressure = 10;
    private float m_ShoveKnockForce = 5f;
    private float m_AEDForce = 20f;
    private float m_ExtinguisherForce = 1.5f;

    private float m_BatPlayerForce = 10f;
    private float m_BatItemHForce = 7f;
    private float m_BatItemVForce = 5f;
    
    private float m_UseBatCooldown = 1f;
    private float m_UseShoveCooldown = 1f;
    private float m_UseAEDCooldown = 1f;
    
    private bool m_CanUseShove = false;
    private bool m_CanUseFireEx = false;
    private bool m_CanUseJackhammer = false;

    // private float m_ChargeShovePressure;
    private string m_AButtonName;
    private string m_BButtonName;
    private PlayerState m_PlayerState;
    private ItemDatabase m_ItemDatabase;
    private Rigidbody m_RigidBody;
    private float m_Cooldown;
    private Vector3 m_ExtinguisherPushbackCurrent;
    private Vector3 m_ExtinguisherPushbackPrevious;

    public AudioClip sfx_PlayerCollision;
    public AudioClip sfx_AED;
    public AudioClip sfx_BatHit;
    public AudioClip sfx_BatMiss;
    public AudioClip sfx_Jackhammer;
    public AudioClip sfx_FireEx;
    private AudioSource audio;

    private float audioTimer;
    private float sfx_OffsetJH = 0.049f;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        m_PlayerState = GetComponent<PlayerState>();
        m_ItemDatabase = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemDatabase>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_Cooldown = Time.time;
    }

    void Start()
    {
        m_AButtonName = "AButton" + m_PlayerState.m_PlayerNumber;
        m_BButtonName = "BButton" + m_PlayerState.m_PlayerNumber;

        // m_ChargeShovePressure = 0f;
        m_ExtinguisherPushbackCurrent = Vector3.zero;
        m_ExtinguisherPushbackPrevious = Vector3.zero;
        m_PlayerState.m_MinChargeShovePressure = m_MinChargeShovePressure;
        m_PlayerState.m_MaxChargeShovePressure = m_MaxChargeShovePressure;
    }

    void Update()
    {
        CheckAButton();
        CheckBButton();

        //REMOVE THIS
        if (Input.GetKeyDown(KeyCode.K)){
            m_PlayerState.DoForce(new Vector3(0,0,15f));
        }

        if (Input.GetKeyDown(KeyCode.J)){
            m_PlayerState.DoForce(new Vector3(15f,0,0));
        }
    }

    void CheckAButton() {
        //Single click actions
        if (m_PlayerState.m_Cooldown <= 0f) {
            if (Input.GetButtonDown(m_AButtonName)  && !m_PlayerState.m_IsKnocked) {
                switch(m_PlayerState.m_HoldItemId) {
                // bb bat
                case -1:
                    m_CanUseShove = true;
                    break;

                case 0:
                    StartCoroutine(UseBat());
                    Set_Cooldown(m_UseBatCooldown);
                    break;

                // AED
                case 1:
                    StartCoroutine(UseAED());
                    Set_Cooldown(m_UseAEDCooldown);
                    break;

                //Fire extinguisher
                case 2:
                    audio.loop = true;
                    audio.clip = sfx_FireEx;
                    audio.Play();
                    m_CanUseFireEx = true;
                    break;

                //Jackhammer
                case 3:
                    audio.loop = true;
                    audio.clip = sfx_Jackhammer;
                    audio.Play();
                    // audioTimer = sfx_Jackhammer.length - sfx_OffsetJH;
                    m_CanUseJackhammer = true;
                    break;


                default:
                    break;
                }
            
            }
        
            //Holdable actions
            if (Input.GetButton(m_AButtonName) && !m_PlayerState.m_IsKnocked) {
                switch(m_PlayerState.m_HoldItemId) {
            
                case -1:
                    if (m_CanUseShove){
                        m_PlayerState.m_CanWalk = false;
                        ChargeShove();
                    }
                    break;

                case 2:
                    if (m_CanUseFireEx){
                        m_PlayerState.m_IsUsingStationaryItem = true;
                        UseExtinguisher();
                    }
                    break;
                
                case 3:
                    if (m_CanUseJackhammer){
                        m_PlayerState.m_IsUsingStationaryItem = true;
                        UseJackhammer();
                    }
                    break;

                default:
                    break;
                }
            }
            else {
                m_PlayerState.m_IsUsingStationaryItem = false;
                m_CanUseFireEx = false;
                m_CanUseShove = false;
                m_CanUseJackhammer = false;
                if (m_PlayerState.m_HoldItemId < 0) {
                    Shove();
                }
                
                // audio.loop = false;
                // audio.Stop();
                // m_UsingJackhammer = false;
                // m_UsingFireEx = false;
                // Debug.Log("AUDIO LOOP: " + audio.loop);
                // m_UsingJackhammer = false;
            }
        }
    }

    void CheckBButton() {
        if (Input.GetButtonDown(m_BButtonName) && !m_PlayerState.m_IsCharging) {
            if (m_PlayerState.m_HoldItemId < 0) {
                m_PlayerState.m_HoldItemId = ItemPickup();
            }
            else {
                DropItem();
            }
        }
    }

    private void ChargeShove() {
        m_PlayerState.m_IsCharging = true;
        if (m_PlayerState.m_ChargeShovePressure < m_MaxChargeShovePressure) {
            m_PlayerState.m_ChargeShovePressure += m_ChargeShoveIncrement * Time.deltaTime;
        }
        else {
            m_PlayerState.m_ChargeShovePressure = m_MaxChargeShovePressure;
        }
        
    }

    void Shove() {
        if (m_PlayerState.m_ChargeShovePressure > 0f && !m_PlayerState.m_IsKnocked) {
            m_PlayerState.m_IsCharging = false;
            m_PlayerState.m_IsShoving = true;
            m_PlayerState.m_CanWalk = false;
            m_PlayerState.m_CanRotate = false;
            m_PlayerState.m_ChargeShovePressure += m_MinChargeShovePressure;
            Vector3 force = transform.forward * m_PlayerState.m_ChargeShovePressure;
            Debug.Log(m_PlayerState.m_CanWalk);
            StartCoroutine(WaitToWalk(m_PlayerState.m_ChargeShovePressure));

            m_RigidBody.AddForce(force, ForceMode.VelocityChange);
            m_PlayerState.m_ChargeShovePressure = 0f;
        }
    }

    void Set_Cooldown(float duration){
        m_PlayerState.m_Cooldown = duration;
    }

    IEnumerator WaitToWalk(float force) {
        Set_Cooldown(m_UseShoveCooldown);
        yield return new WaitForSeconds( (force/m_PlayerState.m_FrictionMagnitude - 5) * Time.deltaTime);
        if (!m_PlayerState.m_IsKnocked){
            m_PlayerState.m_CanWalk = true;
            m_PlayerState.m_CanRotate = true;
            m_PlayerState.m_IsShoving = false;    
        }
        
    }


    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player" && m_PlayerState.m_IsShoving) {
            //Play collision audio
            audio.PlayOneShot(sfx_PlayerCollision);

            PlayerState otherPlayer = other.gameObject.GetComponent<PlayerState>();
            Vector3 pushDirection = other.transform.position - transform.position;
            pushDirection = pushDirection.normalized;
            other.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_ShoveKnockForce);
        } else if (other.gameObject.tag == "Item"){
            
            if (m_PlayerState.m_IsShoving){
                other.gameObject.GetComponent<Rigidbody>().AddForce( transform.forward * 3f + new Vector3(0,1f,0), ForceMode.VelocityChange);
            }
            // if (other.gameObject.GetComponent<Item>().m_Thrown == true && other.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 3f){
            //     audio.PlayOneShot(sfx_PlayerCollision, 0.1f);
            // }

        }

    }

    IEnumerator UseAED() {
        m_PlayerState.m_CanWalk = false;
        m_PlayerState.m_CanRotate = false;
        m_PlayerState.m_IsSingleUseItem = true;
        
        yield return new WaitForSeconds(1);
        if (m_PlayerState.m_HoldItemId == 1){
            audio.PlayOneShot(sfx_AED, 0.1f);

            float radius = 2.0f;
            float maxAngle = 15f;

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, m_PlayerState.m_PlayerMask);
            foreach (Collider col in colliders) {
                PlayerState otherPlayer = col.gameObject.GetComponent<PlayerState>();
                if (otherPlayer.m_PlayerNumber != m_PlayerState.m_PlayerNumber) {
                    Vector3 pushDirection = col.transform.position - transform.position;
                    float angle = Vector3.Angle(pushDirection, transform.forward);
                    if (angle < maxAngle) {
                        pushDirection = pushDirection.normalized;
                        col.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_AEDForce);

                    }
                }
            }
            m_PlayerState.m_IsSingleUseItem = false;
            m_PlayerState.m_CanWalk = true;
            m_PlayerState.m_CanRotate = true;
        
            }
        }

    IEnumerator UseBat() {
        m_PlayerState.m_IsSingleUseItem = true;

        float radius = 1.5f;
        float maxAngle = 25f;
        int hitCount = 0;

        Collider[] playerColliders = Physics.OverlapSphere(transform.position, radius, m_PlayerState.m_PlayerMask);
        foreach (Collider col in playerColliders) {
            PlayerState otherPlayer = col.gameObject.GetComponent<PlayerState>();
            if (otherPlayer.m_PlayerNumber != m_PlayerState.m_PlayerNumber) {
                Vector3 pushDirection = col.transform.position - transform.position;
                float angle = Vector3.Angle(pushDirection, transform.forward);
                if (angle < maxAngle) {
                    hitCount++;
                    pushDirection = pushDirection.normalized;
                    // col.gameObject.GetComponent<Rigidbody>().AddForce(
                    //     pushDirection * m_BatPlayerForce, ForceMode.VelocityChange
                    // );
                    col.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_BatPlayerForce);
                }
            }
        }

        // Collider[] playerColliders = Physics.OverlapSphere(transform.position, radius, m_PlayerState.m_PlayerMask);
        // foreach (Collider col in playerColliders) {
        //     PlayerState otherPlayer = col.gameObject.GetComponent<PlayerState>();
        //     if (otherPlayer.m_PlayerNumber != m_PlayerState.m_PlayerNumber) {
        //         Vector3 pushDirection = col.transform.position - transform.position;
        //         float angle = Vector3.Angle(pushDirection, transform.forward);

        //         float half_width = 0.5f;
        //         Vector3 other_position = col.transform.position;
        //         Vector2 my_position = new Vector2(transform.position.x, transform.position.z);
        //         Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
        //         Vector2 normal = Vector2.Perpendicular(forward);
        //         Vector2 min_thresh = forward - normal * half_width + my_position;
        //         Vector2 max_thresh = forward + normal * half_width + my_position;



        //         // bool IsCBetweenAB ( Vector3 A , Vector3 B , Vector3 C ) {
        //         //     return Vector3.Dot( (B-A).normalized , (C-B).normalized )<0f && Vector3.Dot( (A-B).normalized , (C-A).normalized )<0f;
        //         // }

        //         if (angle < maxAngle) {
        //             hitCount++;
        //             pushDirection = pushDirection.normalized;
        //             // col.gameObject.GetComponent<Rigidbody>().AddForce(
        //             //     pushDirection * m_BatPlayerForce, ForceMode.VelocityChange
        //             // );
        //             col.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_BatPlayerForce);
        //         }
        //     }
        // }

        Collider[] itemColliders = Physics.OverlapSphere(transform.position, radius, m_PlayerState.m_ItemMask);
        foreach (Collider col in itemColliders) {
            
            Vector3 pushDifference = col.transform.position - transform.position;
            float angle = Vector3.Angle(pushDifference, transform.forward);
            if (angle < maxAngle) {
                hitCount++;
                //Force to applied to object
                Vector3 force = transform.forward.normalized * m_BatItemHForce;
                //Vertical force
                force.y = m_BatItemVForce;
                col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                col.gameObject.GetComponent<Rigidbody>().AddForce(
                    force, ForceMode.VelocityChange
                );
            }
        }

        if (hitCount > 0){
            audio.PlayOneShot(sfx_BatHit);
        } else {
            //Play woosh
            audio.PlayOneShot(sfx_BatMiss);
        }
        yield return new WaitForSeconds(0.2f);
        m_PlayerState.m_IsSingleUseItem = false;
    }

    

    void UseExtinguisher() {        
        float radius = 6.0f;
        float maxAngle = 15f;

        // Push self back
        if (m_RigidBody.velocity.magnitude < 1f) {
            m_RigidBody.AddForce(-transform.forward * m_ExtinguisherForce, ForceMode.VelocityChange);
        }

        // Pushing others
        int playerAndItemMask = m_PlayerState.m_PlayerMask | m_PlayerState.m_ItemMask;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, playerAndItemMask);
        foreach (Collider col in colliders) {
            Vector3 pushDifference = col.transform.position - transform.position;
            float angle = Vector3.Angle(pushDifference, transform.forward);
            if (angle < maxAngle) {
                if (col.gameObject.tag == "Item") {
                    ExtinguisherPush(col, pushDifference);
                }
                else {
                    PlayerState otherPlayer = col.gameObject.GetComponent<PlayerState>();
                    if (otherPlayer.m_PlayerNumber != m_PlayerState.m_PlayerNumber && !otherPlayer.m_IsKnocked) {
                        ExtinguisherPush(col, pushDifference);
                    }
                }
            }
        }
    }

    void ExtinguisherPush(Collider col, Vector3 pushDifference) {
        float scaledPush = 1f / pushDifference.magnitude;
        Vector3 pushDirection = pushDifference.normalized;
        // ps.DoForce(pushDirection * scaledPush * m_ExtinguisherForce);
        col.gameObject.GetComponent<Rigidbody>().AddForce(
            pushDirection * scaledPush * m_ExtinguisherForce, ForceMode.VelocityChange
        );
    }

    void UseJackhammer() {     
        float radius = 4.0f;
        float randomRotate = 15f;
        float randomPlayerCap = 4f;
        float randomItemCap = 2f;
        float forwardScale = 2f;

        // if (audioTimer <= 0){
        //     audio.PlayOneShot(sfx_Jackhammer, 0.1f);
        //     audioTimer = sfx_Jackhammer.length - sfx_OffsetJH;
        // } else {
        //     audioTimer -= Time.deltaTime;
        // }
        if (m_RigidBody.velocity.magnitude < 1f) {
            Vector3 randomForce = new Vector3(Random.Range(-randomPlayerCap, randomPlayerCap), 0, Random.Range(-randomPlayerCap, randomPlayerCap));
            randomForce += forwardScale * transform.forward;
            m_RigidBody.AddForce(randomForce, ForceMode.VelocityChange);
            Quaternion newRotation = Quaternion.LookRotation(randomForce, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, randomRotate * Time.deltaTime);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, m_PlayerState.m_PlayerMask);
        foreach (Collider col in colliders) {
            PlayerState otherPlayer = col.gameObject.GetComponent<PlayerState>();
            Vector3 pushDirection = (col.transform.position - transform.position).normalized;
            if (otherPlayer.m_PlayerNumber != m_PlayerState.m_PlayerNumber) {
                float randomBonusInDirection = Random.Range(0f, 1f);
                col.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * randomBonusInDirection);
            }
        }

        colliders = Physics.OverlapSphere(transform.position, radius, m_PlayerState.m_ItemMask);
        foreach (Collider col in colliders) {
            //Force to applied to object
            float rand = Random.Range(-randomItemCap, randomItemCap);
            Vector3 randomForce = new Vector3(rand, Random.Range(0f, 1f), rand);
            Rigidbody otherRBD = col.gameObject.GetComponent<Rigidbody>();
            Item item = col.gameObject.GetComponent<Item>();
            if (otherRBD.velocity.magnitude < 1f && !item.m_Thrown) {
                otherRBD.AddForce(
                    randomForce, ForceMode.VelocityChange
                );
            }
        }
    }

    int ItemPickup() {
        int itemId = -1;
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_PickupRadius, m_PlayerState.m_ItemMask);
        int nearestIndex = ScanNearestItem(colliders);
        // If nearest item found
        if (nearestIndex >= 0) {
            GameObject nearestItem = colliders[nearestIndex].gameObject;
            if (nearestItem.tag == "Item") {
                Item otherPlayer = nearestItem.GetComponent<Item>();
                itemId = otherPlayer.m_ItemId;
                Destroy(nearestItem);
            }
        }
        return itemId;
    }

    int ScanNearestItem(Collider[] colliders) {
        int nearestIndex = -1;
        float nearestDistance = float.MaxValue;
        for (int i=0; i<colliders.Length; i++) {
            float distance = Vector3.Distance(colliders[i].transform.position, transform.position);
            if (distance < nearestDistance) {
                nearestIndex = i;
                nearestDistance = distance;
            }
        }
        return nearestIndex;
    }

    public void DropItem() {
        if (m_PlayerState.m_HoldItemId != -1){
            GameObject itemPrefabToInstantiate = m_ItemDatabase.GetItemById(m_PlayerState.m_HoldItemId);
            Vector3 objPosition = transform.position + transform.forward + new Vector3(0f, m_ItemSpawnFromPlayerHeight, 0f);
            GameObject obj = Instantiate(itemPrefabToInstantiate, objPosition, Quaternion.identity);
            Vector3 force = transform.forward * (m_MinDropForce + m_PlayerState.m_MovementMagnitude * m_WalkDropForce);
            // Debug.Log(force.magnitude);
            obj.GetComponent<Rigidbody>().AddForce(force);
            obj.GetComponent<Item>().m_Thrown = true;
            // Set to empty
            m_PlayerState.m_HoldItemId = -1;

            if (audio.loop == true){
                audio.Stop();
                audio.loop = false;
            }

            m_PlayerState.m_CanWalk = true;
            m_PlayerState.m_CanRotate = true;
            m_PlayerState.m_IsSingleUseItem = false;
            m_PlayerState.m_IsUsingStationaryItem = false;
            m_PlayerState.m_TurnSpeed = 10f;
        }
    }

    public void KnockDropItem(Vector3 knockForce){
        if (m_PlayerState.m_HoldItemId != -1){
            GameObject itemPrefabToInstantiate = m_ItemDatabase.GetItemById(m_PlayerState.m_HoldItemId);
            Vector3 objPosition = transform.position + knockForce.normalized + new Vector3(0f, 2.0f, 0f);
            GameObject obj = Instantiate(itemPrefabToInstantiate, objPosition, Quaternion.identity);
            

            Vector3 dir = Quaternion.AngleAxis( Random.Range(-30f, 30f), Vector3.up) * knockForce.normalized;
            Vector3 force = dir * (m_MinDropForce * 1.5f) + new Vector3(0, 7f, 0);
            Debug.Log(force.magnitude);
            obj.GetComponent<Rigidbody>().AddForce(force);
            obj.GetComponent<Rigidbody>().angularVelocity = force;
            obj.GetComponent<Item>().m_Thrown = false;
            // Set to empty
            m_PlayerState.m_HoldItemId = -1;

            if (audio.loop == true){
                audio.Stop();
                audio.loop = false;
            }
        }
    }

}
