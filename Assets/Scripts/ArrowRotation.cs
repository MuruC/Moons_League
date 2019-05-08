using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotation : MonoBehaviour
{
    public float rotateSpeed = 20f;
    Vector2 temp;
    float size;
    GameObject myMouseController;
    MouseController myMouseControllerScript;
    // Start is called before the first frame update
    void Start()
    {
        size = GetComponent<SpriteRenderer>().bounds.size.x;
        myMouseController = GameObject.Find("mouseController");
        GameObject selectedUnitObject = myMouseController.GetComponent<MouseController>().selectedUnitObj;
        myMouseControllerScript = myMouseController.GetComponent<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (myMouseControllerScript.selectedUnitObj == null)
        {
            Destroy(gameObject);
        }

        if (myMouseControllerScript.playerState != GlobPlayerAction.ePlayerState_DoAction)
        {
            Destroy(gameObject);
        }
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
        GameObject selectedUnitObject = myMouseController.GetComponent<MouseController>().selectedUnitObj;
        int heroIndex = selectedUnitObject.GetComponent<HeroEntity>().nEntityType;
        int actionDist = HeroManager.Instance.getHeroDataDic(heroIndex).m_nAttackDistance;
        float distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
        temp = transform.localScale;
        temp.x = (int)(distance/1.4f);
        temp.x = Mathf.Clamp(temp.x, 1, actionDist);
        transform.localScale = temp;
    }
}
