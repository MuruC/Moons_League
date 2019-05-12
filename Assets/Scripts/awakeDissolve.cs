using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class awakeDissolve : MonoBehaviour
{
    float deathTime = 0;
    bool setDeathTime = false;
    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (setDeathTime)
        {
            float threshold = Time.time - deathTime;
            rend.material.SetFloat("_Threshold", threshold);

        }
    }

    public void setDissolveTime() {
        rend.material.shader = Shader.Find("Custom/2D/Dissolve");
        setDeathTime = true;
        deathTime = Time.time;
    }
}
