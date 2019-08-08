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
    public float m_ExtinguisherForce = 0.8f;

    public float m_ChargeShovePressure;
    private string m_AButtonName;
    private string m_BButtonName;
    private PlayerState m_PlayerState;
    private ItemDatabase m_ItemDatabase;
    private Rigidbody m_RigidBody;
    void Awake()
    {
        m_PlayerState = GetComponent<PlayerState>();
        m_ItemDatabase = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemDatabase>();
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        m_AButtonName = "AButton" + m_PlayerState.m_PlayerNumber;
        m_BButtonName = "BButton" + m_PlayerState.m_PlayerNumber;
        m_ChargeShovePressure = 0f;
        m_PlayerState.m_MinChargeShovePressure = m_MinChargeShovePressure;
        m_PlayerState.m_MaxChargeShovePressure = m_MaxChargeShovePressure;
    }

    void FixedUpdate()
    {
        CheckBButton();
        CheckAButton();
    }

    void CheckAButton() {
        if (Input.GetButton(m_AButtonName)) {
            if (m_PlayerState.m_HoldItemId < 0) {
                ChargeShove();
            }
            else {
                UseItem();
            }
        }
        else {
            if (m_PlayerState.m_HoldItemId < 0) {
                Shove();
                StartCoroutine(WaitToWalk());
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
        m_PlayerState.m_CanWalk = false;
        // TODO: find a way to stop movement
        if (m_ChargeShovePressure == 0f) {
            m_RigidBody.AddForce(m_PlayerState.m_PreviousMovement - m_PlayerState.m_CurrentMovement, ForceMode.VelocityChange);
            m_PlayerState.m_CurrentMovement = Vector3.zero;
        }
        
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
            m_ChargeShovePressure += m_MinChargeShovePressure;
            Vector3 force = transform.forward * m_ChargeShovePressure;
            m_RigidBody.AddForce(force, ForceMode.VelocityChange);
            m_ChargeShovePressure = 0f;
            m_PlayerState.m_ChargeShovePressure = m_ChargeShovePressure;
        }
    }

    IEnumerator WaitToWalk() {
        yield return new WaitForSeconds(1);
        m_PlayerState.m_CanWalk = true;
    }


    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
            PlayerState otherPlayer = other.gameObject.GetComponent<PlayerState>();
            Vector3 pushDirection = other.transform.position - transform.position;
            pushDirection = pushDirection.normalized;
            other.gameObject.GetComponent<Rigidbody>().AddForce(
                pushDirection * m_ShoveKnockForce, ForceMode.VelocityChange
            );
        }
    }

    void UseItem() {
        // Check holding item id, do appropriate action
        switch(m_PlayerState.m_HoldItemId) {
            // bb bat
            case 0:
                break;

            // AED
            case 1:
                StartCoroutine(UseAED());
                break;
            
            // extinguisher
            case 2:
                UseExtinguisher();
                break;
            
            // jackhammer
            case 3:
                break;
            
            default:
                break;
        
        }
    }

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
                if (angle < maxAngle && !otherPlayer.m_IsKnocked) {
                    pushDirection = pushDirection.normalized;
                    col.gameObject.GetComponent<Rigidbody>().AddForce(
                        pushDirection * m_AEDForce, ForceMode.VelocityChange
                    );
                    otherPlayer.EnterKnockedState();
                }
            }
        }
    }

    void UseExtinguisher() {        
        float radius = 6.0f;
        float maxAngle = 15f;

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
        col.gameObject.GetComponent<Rigidbody>().AddForce(
            pushDirection * scaledPush * m_ExtinguisherForce, ForceMode.VelocityChange
        );
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
