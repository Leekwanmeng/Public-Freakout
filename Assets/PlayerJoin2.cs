using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoin2 : MonoBehaviour
{
    public Image playerwheel;
    public Color wheelcolour;
    public string key;
    private Color defaultcolour;
    // Start is called before the first frame update
    void Start()
    {
        playerwheel = gameObject.GetComponentInChildren<Image>();
        //wheelcolour = new Color(255, 0, 0, 80/250);
        defaultcolour = playerwheel.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (playerwheel.color == defaultcolour)
            {
                playerwheel.color = wheelcolour;
            }
            else
            {
                playerwheel.color = defaultcolour;
            }
        }
    }
}
