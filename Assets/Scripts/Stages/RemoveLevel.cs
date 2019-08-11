using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveLevel : MonoBehaviour
{
    public bool start;
    private Vector3 startPosition;
    private float vibrateFreq;
    private float vibrateAmp;
    private float distanceTravelled;
    private float speed;
    private float count;
    private float timer;
    private bool reduceCollider = true;

    // Start is called before the first frame update
    void Start()
    {
        start = false;
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        vibrateAmp = 0.05f;
        vibrateFreq = 50.0f;
        distanceTravelled = 0f;
        // speed = -2.00f;
        speed = -0.01f;
        count = 1f;
        timer = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (start) {
            
            
            if (Mathf.Abs(distanceTravelled) < 20.0f) {
                //Distance travelled before reducing collider 
                if (Mathf.Abs(distanceTravelled) > 0.2f && reduceCollider){
                    reduceCollider = false;
                    transform.parent.GetComponent<StageManager2>().reduceStageCollider();
                }
                
                //Vibrate about x axis
                float x = Mathf.Sin(Time.time * vibrateFreq) * vibrateAmp;

                //Vibrate about y axis
                float z = Mathf.Sin(Time.time * vibrateFreq) * vibrateAmp;

                if (timer < 0){
                    
                    distanceTravelled = startPosition.y - transform.position.y;
                    //Move stage
                    transform.Translate(x, speed * Time.deltaTime, z);
                    speed = speed * Mathf.Pow(1.01f, count);
                    count = count + 0.01f;
                } else {
                    
                    transform.Translate(x, 0, z);
                    timer -= Time.deltaTime;
                }
            } else {
                ResetRemove();
            }

        }
    }  

    public void ResetRemove(){
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
            start = false;
            distanceTravelled = 0f;
            speed = -0.01f;
            count = 1f;
            timer = 5;
            transform.position = new Vector3(0,0,0);
        }
    }
}
