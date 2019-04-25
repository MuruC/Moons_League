using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    public GameObject UnitSpawner;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        clickMouseLeftButton();
    }

    private bool isMouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void clickMouseLeftButton()
    {
        if (Input.GetMouseButtonUp(0)) {
            if (isMouseOverUI())
            {
                pickCard();
            }
            else
            {
                RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit != null)
                {
                    for (int i = 0; i < hit.Length; i++)
                    {
                        GameObject ourHitObject = hit[i].collider.transform.gameObject;

                        if (ourHitObject.GetComponent<MovableUnit>() != null) {

                        }
                    }
                }
            }
        }
    }

    void selectHero() {

    }

    void pickCard() {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData,raycastResultList);

        if (Input.GetMouseButtonUp(0)) {
            for (int i = 0; i < raycastResultList.Count; i++)
            {
                if (raycastResultList[i].gameObject.GetComponent<HeroCard>() != null) {
                    GameObject thisHeroCard = raycastResultList[i].gameObject;
                    HeroCard thisHeroCardScript = thisHeroCard.GetComponent<HeroCard>();
                    thisHeroCardScript.setAlpha(0);
                    thisHeroCardScript.spawnHero();
                }
            }
        }
    }

    public bool CheckIfHasSoldierInGrid()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit != null && Input.GetMouseButton(0))
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider != null)
                {
                    GameObject pObj = hit[i].collider.transform.gameObject;
                    if (pObj.tag == "kingdomIcon") {

                    }
                    if (pObj.GetComponent<HeroEntity>() != null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
