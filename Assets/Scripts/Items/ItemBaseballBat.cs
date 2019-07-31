using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBaseballBat : Item
{
    protected override void Start() {
        m_MaxTime = 15f;
        base.Start();
    }
}
