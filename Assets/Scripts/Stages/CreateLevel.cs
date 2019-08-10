using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLevel : MonoBehaviour
{
    public float layer;
    public GameObject sidePrefab;
    public GameObject cornerPrefab;

    Vector3 m_Min, m_Max;
    // float heightOfLevel;

    void Start()
    {
        //Fetch the Collider from the GameObject
        // Collider m_Collider = parent.GetComponent<Collider>();
        //Fetch the minimum and maximum bounds of the Collider volume
        StageManager2 parent = transform.parent.GetComponent<StageManager2>();
        m_Min = parent.b_Min;
        m_Max = parent.b_Max;
        int levels = parent.levels;
        float heightOfLevel = parent.heightOfLevel;
        // float y_min = m_Min.y;
        // float y_max = m_Max.y;

        for (float i = 1; i <= levels; i++){
            createFloor(-i * heightOfLevel);
        }
        // createFloor(7.3f);
        // float start_x = m_Min.z;
        // float start_z = m_Max.x + 1;

        // for (float i = start_x; i < m_Max.x; i++){
        //     Vector3 position = new Vector3(start_x, m_Min.y, start_z);
        //     Debug.Log(position);
        //     GameObject cube = Instantiate(sidePrefab, position, Quaternion.identity);
        //     cube.transform.parent = gameObject.transform;
        // }
        //Output this data into the console

    }

    private void createFloor(float height){
        float x_min = m_Min.x - layer + 1f;
        float x_max = m_Max.x + layer - 1f;
        float z_min = m_Min.z - layer + 1f;
        float z_max = m_Max.z + layer - 1f;
        
        for (float i = x_min; i < x_max; i++){
            Vector3 position = new Vector3(i, height, z_min);
            GameObject cube = Instantiate(sidePrefab, position, Quaternion.identity);
            cube.transform.parent = gameObject.transform;
        }

        for (float i = x_min; i < x_max; i++){
            Vector3 position = new Vector3(i+1, height, z_max);
            GameObject cube = Instantiate(sidePrefab, position, Quaternion.identity);
            cube.transform.parent = gameObject.transform;
            cube.transform.Rotate(0, 180, 0);
        }

        for (float j = z_min; j < z_max; j++){
            Vector3 position = new Vector3(x_min, height, j+1);
            GameObject cube = Instantiate(sidePrefab, position, Quaternion.identity);
            cube.transform.parent = gameObject.transform;
            cube.transform.Rotate(0, 90, 0);
        }

        for (float j = z_min; j < z_max; j++){
            Vector3 position = new Vector3(x_max, height, j);
            GameObject cube = Instantiate(sidePrefab, position, Quaternion.identity);
            cube.transform.parent = gameObject.transform;
            cube.transform.Rotate(0, 270, 0);
        }

        GameObject corner1 = Instantiate(cornerPrefab, new Vector3(x_min-1, height, z_min), Quaternion.identity);
        corner1.transform.parent = gameObject.transform;

        GameObject corner2 = Instantiate(cornerPrefab, new Vector3(x_max, height, z_min-1), Quaternion.identity);
        corner2.transform.Rotate(0f, -90f, 0f);
        corner2.transform.parent = gameObject.transform;

        GameObject corner3 = Instantiate(cornerPrefab, new Vector3(x_min, height, z_max+1), Quaternion.identity);
        corner3.transform.Rotate(0f, 90f, 0f);
        corner3.transform.parent = gameObject.transform;

        GameObject corner4 = Instantiate(cornerPrefab, new Vector3(x_max+1, height, z_max), Quaternion.identity);
        corner4.transform.Rotate(0f, 180f, 0f);
        corner4.transform.parent = gameObject.transform;
     
    }

}
