﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private float m_PickupRadius = 1f;
    private float m_WalkDropForce = 50f;
    private float m_MinDropForce = 120f;
    private float m_ItemSpawnFromPlayerHeight = 0.7f;
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
    private float m_UseShoveCooldown = 0.5f;
    private float m_UseAEDCooldown = 1f;
    

    private float m_ChargeShovePressure;
    private string m_AButtonName;
    private string m_BButtonName;
    private PlayerState m_PlayerState;
    private ItemDatabase m_ItemDatabase;
    private Rigidbody m_RigidBody;

    private float m_Cooldown;

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
        m_PlayerState.m_MinChargeShovePressure = m_MinChargeShovePressure;
        m_PlayerState.m_MaxChargeShovePressure = m_MaxChargeShovePressure;
    }

    void FixedUpdate()
    {
        CheckAButton();
        CheckBButton();
        //REMOVE THIS
        if (Input.GetKeyDown(KeyCode.K)){
            m_PlayerState.DoForce(new Vector3(0,0,15f));
        }

        if (Input.GetKeyDown(KeyCode.J)){
            m_PlayerState.DoForce(new Vector3(4f,0,0));
        }
    }

    void CheckAButton() {
        //Holdable actions
        if (m_Cooldown <= Time.time){ 
        if (Input.GetButton(m_AButtonName) && !m_PlayerState.m_IsKnocked) {
            switch(m_PlayerState.m_HoldItemId) {
          
            case -1:
                Debug.Log("Shoving");
                m_PlayerState.m_CanWalk = false;
                ChargeShove();
                break;

            case 2:
                UseExtinguisher();
                break;
            
            // jackhammer
            case 3:
                break;

            default:
                break;
            }
        } else {
            if (m_PlayerState.m_HoldItemId < 0) {
                Shove();
                // StartCoroutine(WaitToWalk());
            }
        }

              
            //Single click actions
            if (Input.GetButtonDown(m_AButtonName)  && !m_PlayerState.m_IsKnocked) {
                switch(m_PlayerState.m_HoldItemId) {
                // bb bat
                case 0:
                    Debug.Log("Swing bat");
                    UseBat();
                    Set_Cooldown(m_UseBatCooldown);
                    break;

                // AED
                case 1:
                    StartCoroutine(UseAED());
                    Set_Cooldown(m_UseAEDCooldown);
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
        // TODO: find a way to stop movement
        // if (m_ChargeShovePressure == 0f) {
        //     m_RigidBody.AddForce(-m_PlayerState.m_CurrentMovement, ForceMode.VelocityChange);
        //     m_PlayerState.m_CurrentMovement = Vector3.zero;
        // }
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
            m_PlayerState.m_IsCharging = false;
            m_PlayerState.m_IsShoving = true;
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
        Set_Cooldown(m_UseShoveCooldown);
        yield return new WaitForSeconds( (force/m_PlayerState.m_FrictionMagnitude - 5) * Time.deltaTime);
        m_PlayerState.m_CanWalk = true;
        m_PlayerState.m_CanRotate = true;
        m_PlayerState.m_IsShoving = false;
    }


    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player" && m_PlayerState.m_IsShoving) {
            PlayerState otherPlayer = other.gameObject.GetComponent<PlayerState>();
            Vector3 pushDirection = other.transform.position - transform.position;
            pushDirection = pushDirection.normalized;
            // other.gameObject.GetComponent<Rigidbody>().AddForce(
            //     pushDirection * m_ShoveKnockForce, ForceMode.VelocityChange
            // );

            other.gameObject.GetComponent<PlayerState>().DoForce(pushDirection * m_ShoveKnockForce);

            
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
        }
    }

}
