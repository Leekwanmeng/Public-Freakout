using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    // Index is same as id
    public List<GameObject> m_ItemList;

    public GameObject GetItemById(int id) {
        return m_ItemList[id];
    }
}
