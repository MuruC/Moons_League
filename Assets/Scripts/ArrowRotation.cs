using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotation : MonoBehaviour
{
    public float rotateSpeed = 20f;
    Vector2 temp;
    float size;
    GameObject myMouseController;
    // Start is called before the first frame update
    void Start()
    {
        size = GetComponent<SpriteRenderer>().bounds.size.x;
        myMouseController = GameObject.Find("mouseController");
        GameObject selectedUnitObject = myMouseController.GetComponent<MouseController>().selectedUnitObj;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
        GameObject selectedUnitObject = myMouseController.GetComponent<MouseController>().selectedUnitObj;
        if (selectedUnitObject == null)
        {
            return;
        }
    }
}
