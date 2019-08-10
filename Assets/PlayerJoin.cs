using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerJoin : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI textPressToJoin;
    public GameObject model;
    public string key;
    // Start is called before the first frame update
    void Start()
    {
        panel = gameObject;
        textPressToJoin = panel.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        model = panel.gameObject.GetComponentInChildren<GameObject>();
        model.gameObject.SetActive(false);

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (textPressToJoin.IsActive())
            {
                textPressToJoin.gameObject.SetActive(false);
                model.gameObject.SetActive(true);

            }
            else
            {
                textPressToJoin.gameObject.SetActive(true);
                model.gameObject.SetActive(false);
            }
        }
    }
}
