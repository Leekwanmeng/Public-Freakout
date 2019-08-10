using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            other.gameObject.SetActive(false);
        }

        if (other.tag == "Item") {
            other.GetComponent<Item>().ExpireItem();
        }
    }
}
