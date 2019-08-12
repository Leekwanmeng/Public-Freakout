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
    
    public Color m_JoinedColour;
    public int m_PlayerNumber;
    public string key;
    public bool joined;
    public bool m_LockedJoin;
    public GameObject m_CurrentAnimation;
    private Image playerwheel;
    private Color defaultcolour;
    private string m_AButtonName;
    private string m_BButtonName;
    private PlayerAnimationDatabase m_DB;
    
    void Awake()
    {
        m_DB = GetComponent<PlayerAnimationDatabase>();
    }
    void Start()
    {
        m_AButtonName = "AButton" + m_PlayerNumber;
        m_BButtonName = "BButton" + m_PlayerNumber;
        playerwheel = gameObject.GetComponentInChildren<Image>();
        defaultcolour = playerwheel.color;
        m_LockedJoin = false;
    }

    void Update()
    {
        if (!m_LockedJoin) {
            if (Input.GetButtonDown(m_AButtonName)) {
                if (playerwheel.color == defaultcolour) {
                    playerwheel.color = m_JoinedColour;
                    JoinAnimation();
                    joined = true;
                }
            }
            if (Input.GetButtonDown(m_BButtonName)) {
                playerwheel.color = defaultcolour;
                DefaultAnimation();
                joined = false;
            }
        }
    }

    void JoinAnimation() {
        if (m_CurrentAnimation != null) Destroy(m_CurrentAnimation);
        m_CurrentAnimation = m_DB.GetAnimationById(m_PlayerNumber);
        m_CurrentAnimation = Instantiate (m_CurrentAnimation, transform.position, transform.rotation) as GameObject;
        m_CurrentAnimation.transform.parent = transform;
    }

    void DefaultAnimation() {
        if (m_CurrentAnimation != null) Destroy(m_CurrentAnimation);
        m_CurrentAnimation = m_DB.GetAnimationById(0);
        m_CurrentAnimation = Instantiate (m_CurrentAnimation, transform.position, transform.rotation) as GameObject;
        m_CurrentAnimation.transform.parent = transform;
    }
}
