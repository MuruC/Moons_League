using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Camera mainCam;
    float shakeAmount = 0;

    private void Awake()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void shake(float amt, float length)
    {
        shakeAmount = amt;
        InvokeRepeating("beginShake", 0, 0.01f);
        Invoke("stopShake", length);
    }

    void beginShake()
    {
        if (shakeAmount > 0)
        {
            Vector3 camPos = mainCam.transform.position;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            camPos.x += offsetX;
            camPos.y += offsetY;

            mainCam.transform.position = camPos;
        }
    }

    void stopShake()
    {
        CancelInvoke("beginShake");
        mainCam.transform.localPosition = new Vector3(0, 0, -10);
    }
}
