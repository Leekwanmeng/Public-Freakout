using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// attached to each player in the lobby (not actual players), 
// need to assign the player game object itself, the image of the playerwheel, 
// the colour (alpha set to 150 for yellow, 120 for the rest), and 
// also need to assign the key (assumed to be A for all controllers)

public class PlayerJoin : MonoBehaviour
{
    public Image playerwheel;
    public Color wheelcolour;
    public string key;
    public bool joined;
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
                joined = true;
            }
            else
            {
                playerwheel.color = defaultcolour;
                joined = false;
            }
        }
    }
}
