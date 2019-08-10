using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager2 : MonoBehaviour
{
    public GameObject[] layersToRemove;
    public int levels;
    public float buildingLength;
    public float durationBetweenRounds;
    public GameObject flooring;
    public float heightOfLevel = 2.181781f;
    public Vector3 b_Min, b_Max;

    private AudioSource buildingCollapse;
    private BoxCollider stageCollider;


    void Awake()
    {
        buildingCollapse = GetComponent<AudioSource>();

        //Start script to remove layers periodically
        for (int i = 0; i < layersToRemove.Length; i++){
            float waitTime = (i + 1) * durationBetweenRounds;
            IEnumerator coroutine = RemoveLevel(layersToRemove[i], waitTime);
            StartCoroutine(coroutine);
        }

        b_Min = new Vector3( -buildingLength/2f, 0, 0 -buildingLength/2f);
        b_Max = new Vector3( buildingLength/2f, 0, buildingLength/2f);

        //Build flooring for levels
        for (float i = 0; i < levels; i++){
            Vector3 position = new Vector3(0, -i * heightOfLevel - 0.1818f/2, 0);
            GameObject cube = Instantiate(flooring, position, Quaternion.identity);
            cube.transform.localScale = new Vector3(Mathf.Abs(b_Min.x - b_Max.x), 0.182f, Mathf.Abs(b_Max.z - b_Min.z));
        }

        //Create bounding box for stage

        int numOfLayers = transform.childCount;

        stageCollider = gameObject.AddComponent<BoxCollider>();
        stageCollider.isTrigger = true;
        stageCollider.center = new Vector3(0,0,0);
        stageCollider.size = new Vector3(buildingLength + numOfLayers * 2, 5, buildingLength + numOfLayers * 2);

    }

    private IEnumerator RemoveLevel(GameObject layer, float waitTime){
        yield return new WaitForSeconds(waitTime);
        RemoveLevel script = layer.GetComponent<RemoveLevel>();
        script.start = true;
        buildingCollapse.Play(0);
    }

    public void reduceStageCollider(){
        stageCollider.size -= new Vector3(2, 0, 2);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player"){
            //Disable friction script, disable movement
            other.gameObject.GetComponent<PlayerState>().m_CanWalk = false;
            other.gameObject.GetComponent<PlayerState>().m_CanRotate = false;
            other.gameObject.GetComponent<PlayerFriction>().enabled = false;
            other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}
