using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemInteract : MonoBehaviour
{
    public int m_ItemMask = 1 << 10; // layer 10: Items
    public int m_HoldingItemId = -1; // -1 means not holding
    public float m_PickupRadius = 1f;
    private PlayerController m_PlayerController;
    private ItemDatabase m_ItemDatabase;
    void Awake()
    {
        m_PlayerController = GetComponent<PlayerController>();
        m_ItemDatabase = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemDatabase>();
    }

    void Update()
    {
        CheckItem();
    }

    void CheckItem() {
        if (Input.GetButtonDown(m_PlayerController.m_BButtonName)) {
            if (m_HoldingItemId < 0) {
                m_HoldingItemId = ItemPickup();
            }
            else {
                DropItem();
            }
        }
    }

    int ItemPickup() {
        int itemId = -1;
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_PickupRadius, m_ItemMask);
        int nearestIndex = ScanNearest(colliders);
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

    void DropItem() {
        GameObject itemPrefabToInstantiate = m_ItemDatabase.GetItemById(m_HoldingItemId);
        Instantiate(itemPrefabToInstantiate, transform.position + new Vector3(0f, 0.3f, 0f), Quaternion.identity);
        m_HoldingItemId = -1;
    }

    int ScanNearest(Collider[] colliders) {
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

    public void SetHoldingItemId(int itemId) {
        m_HoldingItemId = itemId;
    }
}
