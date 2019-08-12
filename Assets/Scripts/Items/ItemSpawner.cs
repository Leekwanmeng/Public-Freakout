using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject m_BaseStage;
    public float m_HeightToSpawn = 10;
    public int m_ItemsToSpawn = 10;
    public float m_SpawnProbability = 0.2f;

    private ItemDatabase m_ItemDb;
    private float m_y;
    private float m_xMin;
    private float m_xMax;
    private float m_zMin;
    private float m_zMax;
    private float m_Time = 0.0f;
    private float lastItemId;
    
    void Awake()
    {
        m_ItemDb = GetComponent<ItemDatabase>();
        lastItemId = -1;
    }
    // void Start()
    // {
    //     Collider collider = m_BaseStage.GetComponent<Collider>();
    //     Vector3 min = collider.bounds.min;
    //     Vector3 max = collider.bounds.max;
    //     m_xMin = min.x;
    //     m_zMin = min.z;
    //     m_xMax = max.x;
    //     m_zMax = max.z;
    //     m_y = m_HeightToSpawn;
    // }

    void Update()
    {
        Collider collider = m_BaseStage.GetComponent<Collider>();
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;
        m_xMin = min.x + 1;
        m_zMin = min.z + 1;
        m_xMax = max.x - 1;
        m_zMax = max.z - 1;
        m_y = m_HeightToSpawn;

        m_Time += Time.deltaTime;
        if (m_Time >= 1f){
            m_Time = 0.0f;
            if (m_ItemsToSpawn != 0){
                if (Random.value < m_SpawnProbability){
                    Vector3 spawnPoint = new Vector3(Random.Range(m_xMin, m_xMax), m_y, Random.Range(m_zMin, m_zMax));

                    Vector3 force = new Vector3(Random.Range(-5f,5f), 0, Random.Range(-5f,5f));
                    SpawnRandomItem(spawnPoint, force);
                    m_ItemsToSpawn--;
                }
            }
        }
        
    }

    void SpawnRandomItem(Vector3 spawnPoint, Vector3 force)
    {
        int randomItemId = Mathf.RoundToInt(Random.Range(0, m_ItemDb.m_ItemList.Count));

        if (randomItemId == lastItemId){
            randomItemId = Mathf.RoundToInt(Random.Range(0, m_ItemDb.m_ItemList.Count));
        }
        GameObject randomItem = m_ItemDb.GetItemById(randomItemId);
        randomItem = Instantiate(randomItem, spawnPoint, Quaternion.identity);
        randomItem.transform.parent = gameObject.transform;
        Rigidbody item_rb = randomItem.GetComponent<Rigidbody>();
        // Debug.Log("Add force" + force);
        // item_rb.velocity = force;
        item_rb.angularVelocity = force;
    }

    public void ResetItems() {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        for (int i=0; i<items.Length; i++) {
            Destroy(items[i]);
        }
        m_ItemsToSpawn = 10;
    }
}
