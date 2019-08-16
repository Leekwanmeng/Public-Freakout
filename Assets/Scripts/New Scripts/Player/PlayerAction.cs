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
    private float m_MinChargeShovePressure = 5;
    private float m_MaxChargeShovePressure = 11;
    private float m_ChargeShovePressureForce;
    private float m_UseShoveCooldown = 1f;

    private float m_BatPlayerForce = 10f;
    private float m_BatItemHForce = 7f;
    private float m_BatItemVForce = 5f;
    private float m_UseBatCooldown = 1f;
    private float m_BatHitBoxWidth = 0.25f;
    private float m_BatHitBoxHeight = 0.5f;
    private float m_BatHitBoxLength = 0.5f;

    private float m_AEDForce = 17f;
    private float m_AEDHitBoxWidth = 0.3f;
    private float m_AEDHitBoxHeight = 0.5f;
    private float m_AEDHitBoxLength = 0.6f;

    private float m_ExtinguisherForce = 1.8f;
    
    private bool m_CanUseShove = false;
    private bool m_CanUseFireEx = false;
    private bool m_CanUseJackhammer = false;
    
    private string m_AButtonName;
    private string m_BButtonName;
    private PlayerState m_PlayerState;
    private ItemDatabase m_ItemDatabase;
    private Rigidbody m_RigidBody;
    private Vector3 m_ExtinguisherPushbackCurrent;
    private Vector3 m_ExtinguisherPushbackPrevious;

    public AudioClip sfx_PlayerCollision;
    public AudioClip sfx_AED;
    public AudioClip sfx_BatHit;
    public AudioClip sfx_BatMiss;
    public AudioClip sfx_Jackhammer;
    public AudioClip sfx_FireEx;
    private AudioSource audio;
    private ShockLauncher m_AEDVFX;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        m_PlayerState = GetComponent<PlayerState>();
        m_ItemDatabase = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemDatabase>();
        m_AEDVFX = GetComponentInChildren<ShockLauncher>();
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        m_AButtonName = "AButton" + m_PlayerState.m_PlayerNumber;
        m_BButtonName = "BButton" + m_PlayerState.m_PlayerNumber;

        m_ExtinguisherPushbackCurrent = Vector3.zero;
        m_ExtinguisherPushbackPrevious = Vector3.zero;
        m_PlayerState.m_MinChargeShovePressure = m_MinChargeShovePressure;
        m_PlayerState.m_MaxChargeShovePressure = m_MaxChargeShovePressure;
    }

    void Update()
    {
        CheckAButton();
        CheckBButton();
    }

    void CheckAButton() {
        //Single click actions
        if (m_PlayerState.m_Cooldown <= 0f) {
            if (Input.GetButtonDown(m_AButtonName)  && !m_PlayerState.m_IsKnocked) {
                m_PlayerState.m_UseCoolDownUI = false;
                switch(m_PlayerState.m_HoldItemId) {
                    case -1:
                        m_CanUseShove = true;
                        break;

                    // bb bat
                    case 0:
                        StartCoroutine(UseBat());
                        m_PlayerState.m_UseCoolDownUI = true;
                        break;

                    // AED
                    case 1:
                        StartCoroutine(UseAED());
                        Set_Cooldown(1.0f);
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
                        if (m_PlayerState.m_Ammo > 0){
                            if (m_CanUseFireEx){
                            m_PlayerState.m_IsUsingStationaryItem = true;
                            UseExtinguisher();
                            }
                        } else {
                            m_PlayerState.m_IsUsingStationaryItem = false;
                            m_CanUseFireEx = false;
                        }
                        break;
                    
                    case 3:
                        if (m_PlayerState.m_Ammo > 0){
                            if (m_CanUseJackhammer){
                            m_PlayerState.m_IsUsingStationaryItem = true;
                            UseJackhammer();
                            }
                        } else {
                            m_PlayerState.m_IsUsingStationaryItem = false;
                            m_CanUseFireEx = false;
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
        if (m_PlayerState.m_ChargeShovePressure < m_MaxChargeShovePressure - m_MinChargeShovePressure) {
            m_PlayerState.m_ChargeShovePressure += m_ChargeShoveIncrement * Time.deltaTime;
        }
        else {
            m_PlayerState.m_ChargeShovePressure = (m_MaxChargeShovePressure - m_MinChargeShovePressure);
        }
        
    }

    void Shove() {
        if (m_PlayerState.m_ChargeShovePressure > 0f && !m_PlayerState.m_IsKnocked) {
            m_PlayerState.m_IsCharging = false;
            m_PlayerState.m_IsShoving = true;
            m_PlayerState.m_CanWalk = false;
            m_PlayerState.m_CanRotate = false;
            m_PlayerState.m_ChargeShovePressure += m_MinChargeShovePressure;
            m_ChargeShovePressureForce = m_PlayerState.m_ChargeShovePressure;
            Vector3 force = transform.forward * m_PlayerState.m_ChargeShovePressure;
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
        m_PlayerState.m_UseCoolDownUI = true;
        yield return new WaitForSeconds( (force/m_PlayerState.m_FrictionMagnitude - 5) * Time.deltaTime);
        if (!m_PlayerState.m_IsKnocked){
            m_PlayerState.m_CanWalk = true;
            m_PlayerState.m_CanRotate = true;
            m_PlayerState.m_IsShoving = false;    
            m_PlayerState.m_ShoveHitLog = new List<int>();
        }
    }


    void OnCollisionStay(Collision other) {
        if (!m_PlayerState.m_ShoveHitLog.Contains(other.gameObject.GetInstanceID())) {            
            if (other.gameObject.tag == "Player" && m_PlayerState.m_IsShoving) {
                Debug.Log("Collision");
                m_PlayerState.m_ShoveHitLog.Add(other.gameObject.GetInstanceID());
                //Play collision audio
                audio.PlayOneShot(sfx_PlayerCollision);

                PlayerState otherPlayer = other.gameObject.GetComponent<PlayerState>();
                Vector3 pushDirection = other.transform.position - transform.position;
                pushDirection = pushDirection.normalized;
                other.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * (m_ChargeShovePressureForce / 1.5f));

            } else if (other.gameObject.tag == "Item"){
                Debug.Log("Collision");
                m_PlayerState.m_ShoveHitLog.Add(other.gameObject.GetInstanceID());
                if (m_PlayerState.m_IsShoving){
                    other.gameObject.GetComponent<Rigidbody>().AddForce( transform.forward * 3f + new Vector3(0,1f,0), ForceMode.VelocityChange);
                }
    
            }
        }
    }

    IEnumerator UseAED() {
        m_PlayerState.m_CanWalk = false;
        m_PlayerState.m_CanRotate = false;
        m_PlayerState.m_IsSingleUseItem = true;
        m_PlayerState.m_AEDChargingTime = Time.time;
        m_PlayerState.m_AEDIsCharging = true;
        
        yield return new WaitForSeconds(m_PlayerState.m_AEDCastDuration);
        if (m_PlayerState.m_HoldItemId == 1){
            m_PlayerState.m_AEDIsCharging = false;
            audio.PlayOneShot(sfx_AED, 0.1f);
            m_AEDVFX.Zap();

            Collider[] colliders = Physics.OverlapBox(transform.position + transform.forward * 0.6f, new Vector3(0.25f, 0.5f, 0.6f), transform.rotation, m_PlayerState.m_PlayerMask);
            foreach (Collider col in colliders) {
                PlayerState otherPlayer = col.gameObject.GetComponent<PlayerState>();
                if (otherPlayer.m_PlayerNumber != m_PlayerState.m_PlayerNumber) {
                    Vector3 pushDirection = col.transform.position - transform.position;
                        pushDirection = pushDirection.normalized;
                        col.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_AEDForce);

                }
            }
            m_PlayerState.m_IsSingleUseItem = false;
            m_PlayerState.m_CanWalk = true;
            m_PlayerState.m_CanRotate = true;
        
            }
        }

    IEnumerator UseBat() {
        m_PlayerState.m_IsSingleUseItem = true;

        int hitCount = 0;

        Collider[] playerColliders = Physics.OverlapBox(
            transform.position + transform.forward * m_BatHitBoxLength,
            new Vector3(m_BatHitBoxWidth, m_BatHitBoxHeight, m_BatHitBoxLength),
            transform.rotation,
            m_PlayerState.m_PlayerMask
        );
        
        foreach (Collider col in playerColliders) {
            PlayerState otherPlayer = col.gameObject.GetComponent<PlayerState>();
            if (otherPlayer.m_PlayerNumber != m_PlayerState.m_PlayerNumber) {
                Vector3 pushDirection = col.transform.position - transform.position;
                hitCount++;
                pushDirection = pushDirection.normalized;
                col.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_BatPlayerForce);
            
            }
        }

        Collider[] itemColliders = Physics.OverlapBox(
            transform.position + transform.forward * m_BatHitBoxLength,
            new Vector3(m_BatHitBoxWidth, m_BatHitBoxHeight, m_BatHitBoxLength),
            transform.rotation,
            m_PlayerState.m_ItemMask
        );
        
        foreach (Collider col in itemColliders) {
            Vector3 pushDifference = col.transform.position - transform.position;
            float angle = Vector3.Angle(pushDifference, transform.forward);
            
            hitCount++;
            //Force to applied to object
            Vector3 force = transform.forward.normalized * m_BatItemHForce;
            //Vertical force
            force.y = m_BatItemVForce;
            col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            col.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);    
            
        }

        if (hitCount > 0){
            audio.PlayOneShot(sfx_BatHit);
            Set_Cooldown(m_UseBatCooldown);
        } else {
            //Play woosh
            audio.PlayOneShot(sfx_BatMiss);
            Set_Cooldown(1.3f);
        }
        yield return new WaitForSeconds(0.2f);
        m_PlayerState.m_IsSingleUseItem = false;
    }

    

    void UseExtinguisher() {        
        float radius = 4.0f;
        float maxAngle = 15f;

        //Use up ammo
        m_PlayerState.m_Ammo -= 0.3f;

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
        float radius = 2.5f;
        float randomRotate = 15f;
        float randomPlayerCap = 3f;
        float randomItemCap = 2f;
        float forwardScale = 2f;

        m_PlayerState.m_Ammo -= 0.3f;

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
                float randomBonusInDirection = Random.Range(0.3f, 0.85f);
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

                m_PlayerState.m_Ammo = otherPlayer.m_Ammo;

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
            obj.GetComponent<Rigidbody>().AddForce(force);
            obj.GetComponent<Item>().m_Thrown = true;
            obj.GetComponent<Item>().m_Ammo = m_PlayerState.m_Ammo;
            m_PlayerState.m_Ammo = 0f;
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
            m_PlayerState.m_AEDIsCharging = false;
        }
    }

    public void KnockDropItem(Vector3 knockForce){
        if (m_PlayerState.m_HoldItemId != -1){
            GameObject itemPrefabToInstantiate = m_ItemDatabase.GetItemById(m_PlayerState.m_HoldItemId);
            Vector3 objPosition = transform.position + knockForce.normalized + new Vector3(0f, 2.0f, 0f);
            GameObject obj = Instantiate(itemPrefabToInstantiate, objPosition, Quaternion.identity);
            

            Vector3 dir = Quaternion.AngleAxis( Random.Range(-30f, 30f), Vector3.up) * knockForce.normalized;
            Vector3 force = dir * (m_MinDropForce * 1.5f) + new Vector3(0, 7f, 0);
            obj.GetComponent<Rigidbody>().AddForce(force);
            obj.GetComponent<Rigidbody>().angularVelocity = force;
            obj.GetComponent<Item>().m_Thrown = false;
            obj.GetComponent<Item>().m_Ammo = m_PlayerState.m_Ammo;
            m_PlayerState.m_Ammo = 0f;
            // Set to empty
            m_PlayerState.m_HoldItemId = -1;

            if (audio.loop == true){
                audio.Stop();
                audio.loop = false;
            }
        }
    }

}
