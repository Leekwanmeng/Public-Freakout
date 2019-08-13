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
    private float m_Offset = 0.1f;

    public bool reset = false;

    void Awake()
    {
        buildingCollapse = GetComponent<AudioSource>();
        stageCollider = gameObject.AddComponent<BoxCollider>();
        stageCollider.isTrigger = true;
        stageCollider.center = new Vector3(0,0,0);
        b_Min = new Vector3( -buildingLength/2f, 0, 0 -buildingLength/2f);
        b_Max = new Vector3( buildingLength/2f, 0, buildingLength/2f);
    }


    public void StartScript()
    {
        Reset();
        //Build flooring for levels
        for (float i = 0; i < levels; i++){
            Vector3 position = new Vector3(0, -i * heightOfLevel - 0.1818f/2, 0);
            GameObject cube = Instantiate(flooring, position, Quaternion.identity);
            cube.transform.localScale = new Vector3(Mathf.Abs(b_Min.x - b_Max.x), 0.182f, Mathf.Abs(b_Max.z - b_Min.z));
        }
    }

    // void Start(){
    //     StartScript();
    // }

    

    private IEnumerator RemoveLevel(GameObject layer, float waitTime){
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Removing layer :" + Time.time);
        layer.GetComponent<RemoveLevel>().start = true;
        // script.start = true;
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

    public void Reset(){
        StopAllCoroutines();
        foreach (Transform child in transform) {
           
            if (child.childCount == 0){
                child.GetComponent<CreateLevel>().StartScript();
            }
            if (child.GetComponent<RemoveLevel>().start == true){
                buildingCollapse.Stop();
                child.GetComponent<RemoveLevel>().ResetRemove();       
                child.GetComponent<CreateLevel>().StartScript();
            }
        }

         
        for (int i = 0; i < layersToRemove.Length; i++){
            float waitTime = (i + 1) * durationBetweenRounds;
            IEnumerator coroutine = RemoveLevel(layersToRemove[i], waitTime);
            StartCoroutine(coroutine);
        }

        int numOfLayers = transform.childCount;
        stageCollider.size = new Vector3(buildingLength + numOfLayers * 2 + m_Offset, 5, buildingLength + numOfLayers * 2 + m_Offset);
    }

}
