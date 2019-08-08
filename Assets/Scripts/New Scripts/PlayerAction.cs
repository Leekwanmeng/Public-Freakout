using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public float m_PickupRadius = 1f;
    public float m_WalkDropForce = 50f;
    public float m_MinDropForce = 120f;
    public float m_ItemSpawnFromPlayerHeight = 0.7f;
    public float m_ChargeShoveIncrement;
    public float m_MinChargeShovePressure;
    public float m_MaxChargeShovePressure;
    public float m_ShoveKnockForce = 5f;
    public float m_AEDForce = 20f;
    public float m_ExtinguisherForce = 1.5f;

    public float m_BatPlayerForce = 10f;
    public float m_BatItemHForce = 7f;
    public float m_BatItemVForce = 5f;
    private float m_JackhammerForce = 2f;

    public float m_ChargeShovePressure;
    private string m_AButtonName;
    private string m_BButtonName;
    private PlayerState m_PlayerState;
    private ItemDatabase m_ItemDatabase;
    private Rigidbody m_RigidBody;
    private float m_Cooldown;
    private Vector3 m_ExtinguisherPushbackCurrent;
    private Vector3 m_ExtinguisherPushbackPrevious;

    void Awake()
    {
        m_PlayerState = GetComponent<PlayerState>();
        m_ItemDatabase = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemDatabase>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_Cooldown = Time.time;
    }

    void Start()
    {
        m_AButtonName = "AButton" + m_PlayerState.m_PlayerNumber;
        m_BButtonName = "BButton" + m_PlayerState.m_PlayerNumber;
        m_ChargeShovePressure = 0f;
        m_ExtinguisherPushbackCurrent = Vector3.zero;
        m_ExtinguisherPushbackPrevious = Vector3.zero;
        m_PlayerState.m_MinChargeShovePressure = m_MinChargeShovePressure;
        m_PlayerState.m_MaxChargeShovePressure = m_MaxChargeShovePressure;
    }

    void FixedUpdate()
    {
        CheckAButton();
        CheckBButton();
    }

    void CheckAButton() {
        //Holdable actions
        if (Input.GetButton(m_AButtonName)) {
            switch(m_PlayerState.m_HoldItemId) {
          
            case -1:
                m_PlayerState.m_CanWalk = false;
                ChargeShove();
                break;

            case 2:
                m_PlayerState.m_IsUsingStationaryItem = true;
                UseExtinguisher();
                break;
            
            case 3:
                m_PlayerState.m_IsUsingStationaryItem = true;
                UseJackhammer();
                break;

            default:
                break;
            }
        }
        else {
            m_PlayerState.m_IsUsingStationaryItem = false;
            if (m_PlayerState.m_HoldItemId < 0) {
                Shove();
            }
        }

        if (m_Cooldown <= Time.time){       
            //Single click actions
            if (Input.GetButtonDown(m_AButtonName)) {
                switch(m_PlayerState.m_HoldItemId) {
                // bb bat
                case 0:
                    Debug.Log("Swing bat");
                    UseBat();
                    Set_Cooldown(1f);
                    break;

                // AED
                case 1:
                    StartCoroutine(UseAED());
                    Set_Cooldown(1f);
                    break;

                default:
                    break;
                }
            
            }
        }
    }

    void CheckBButton() {
        if (Input.GetButtonDown(m_BButtonName)) {
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
        if (m_ChargeShovePressure < m_MaxChargeShovePressure) {
            m_ChargeShovePressure += m_ChargeShoveIncrement * Time.deltaTime;
        }
        else {
            m_ChargeShovePressure = m_MaxChargeShovePressure;
        }
        m_PlayerState.m_ChargeShovePressure = m_ChargeShovePressure;
    }

    void Shove() {
        if (m_ChargeShovePressure > 0f) {
            m_PlayerState.m_CanRotate = false;
            m_ChargeShovePressure += m_MinChargeShovePressure;
            Vector3 force = transform.forward * m_ChargeShovePressure;

            StartCoroutine(WaitToWalk(m_ChargeShovePressure));

            m_RigidBody.AddForce(force, ForceMode.VelocityChange);
            m_ChargeShovePressure = 0f;
            m_PlayerState.m_ChargeShovePressure = m_ChargeShovePressure;
        }
    }

    void Set_Cooldown(float duration){
        m_Cooldown = Time.time + duration;
    }

    IEnumerator WaitToWalk(float force) {
        yield return new WaitForSeconds( (force/m_PlayerState.m_FrictionMagnitude - 5) * Time.deltaTime);
        m_PlayerState.m_CanWalk = true;
        m_PlayerState.m_CanRotate = true;
        m_PlayerState.m_IsCharging = false;
    }


    // void OnCollisionEnter(Collision other) {
    //     if (other.gameObject.tag == "Player") {
    //         PlayerState otherPlayer = other.gameObject.GetComponent<PlayerState>();
    //         Vector3 pushDirection = other.transform.position - transform.position;
    //         pushDirection = pushDirection.normalized;
    //         // other.gameObject.GetComponent<Rigidbody>().AddForce(
    //         //     pushDirection * m_ShoveKnockForce, ForceMode.VelocityChange
    //         // );

    //         other.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_ShoveKnockForce);

            
    //     }
    // }

    IEnumerator UseAED() {
        m_PlayerState.m_CanWalk = false;
        m_PlayerState.m_CanRotate = false;
        // TODO: find a way to stop movement
        yield return new WaitForSeconds(1);
        m_PlayerState.m_CanWalk = true;
        m_PlayerState.m_CanRotate = true;

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
                    // col.gameObject.GetComponent<Rigidbody>().AddForce(
                    //     pushDirection * m_AEDForce, ForceMode.VelocityChange
                    // );

                    col.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_AEDForce);

                }
            }
        }
    }

    void UseBat() {
        float radius = 1.0f;
        float maxAngle = 20f;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, m_PlayerState.m_PlayerMask);
        foreach (Collider col in colliders) {
            PlayerState otherPlayer = col.gameObject.GetComponent<PlayerState>();
            if (otherPlayer.m_PlayerNumber != m_PlayerState.m_PlayerNumber) {
                Vector3 pushDirection = col.transform.position - transform.position;
                float angle = Vector3.Angle(pushDirection, transform.forward);
                if (angle < maxAngle) {
                    pushDirection = pushDirection.normalized;
                    // col.gameObject.GetComponent<Rigidbody>().AddForce(
                    //     pushDirection * m_BatPlayerForce, ForceMode.VelocityChange
                    // );
                    col.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_BatPlayerForce);
                }
            }
        }

        colliders = Physics.OverlapSphere(transform.position, radius, m_PlayerState.m_ItemMask);
        foreach (Collider col in colliders) {
            
            Vector3 pushDifference = col.transform.position - transform.position;
            float angle = Vector3.Angle(pushDifference, transform.forward);
            if (angle < maxAngle) {
                Debug.Log("Hit object");
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
        Debug.Log(scaledPush * m_ExtinguisherForce);

        // ps.DoForce(pushDirection * scaledPush * m_ExtinguisherForce);
        col.gameObject.GetComponent<Rigidbody>().AddForce(
            pushDirection * scaledPush * m_ExtinguisherForce, ForceMode.VelocityChange
        );
    }

    void UseJackhammer() {     
        float radius = 4.0f;
        float randomRotate = 15f;
        float randomPlayerCap = 5f;
        float randomItemCap = 2f;

        if (m_RigidBody.velocity.magnitude < 1f) {
            Vector3 randomForce = new Vector3(Random.Range(-randomPlayerCap, randomPlayerCap), 0, Random.Range(-randomPlayerCap, randomPlayerCap));
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

    void DropItem() {
        GameObject itemPrefabToInstantiate = m_ItemDatabase.GetItemById(m_PlayerState.m_HoldItemId);
        Vector3 objPosition = transform.position + transform.forward + new Vector3(0f, m_ItemSpawnFromPlayerHeight, 0f);
        GameObject obj = Instantiate(itemPrefabToInstantiate, objPosition, Quaternion.identity);
        Vector3 force = transform.forward * (m_MinDropForce + m_PlayerState.m_MovementMagnitude * m_WalkDropForce);
        // Debug.Log(force.magnitude);
        obj.GetComponent<Rigidbody>().AddForce(force);
        obj.GetComponent<Item>().m_Thrown = true;
        // Set to empty
        m_PlayerState.m_HoldItemId = -1;
    }

}
