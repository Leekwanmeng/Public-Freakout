using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationDatabase : MonoBehaviour
{
    public List<GameObject> m_PlayerAnimationList;

    public GameObject GetAnimationById(int id) {
        return m_PlayerAnimationList[id];
    }
    
}
