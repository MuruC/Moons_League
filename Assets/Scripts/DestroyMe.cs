﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    public float aliveTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,aliveTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void destroyThisObject() {
        Destroy(gameObject);
    }
}
