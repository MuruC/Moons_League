using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TutorialScript : MonoBehaviour
{
    public GameObject tutorialCanvas;
    public GameObject actionTutorial;
    public GameObject pointAtKingPrefab;
    public GameObject pointAtTavernPrefab;
    public GameObject pointAtWorkshopPrefab;
    public GameObject pointAtKingdomPrefab;

    int pressKing1Num;
    int pressKing2Num;
    public bool king1_buildTavern;
    public bool king1_buildWorkshop;
    public bool King2_buildTavern;
    public bool king2_buildWorkshop;

    public bool king1_spawnHero;
    public bool king2_spawnHero;

    bool king1_spawnKingdomTutorial;
    bool king2_spawnKingdomTutorial;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.getTurn() == 0)
        {
            if (pressKing1Num == 0)
            {
                actionTutorial.SetActive(false);
            }

            if (pressKing1Num >= 1)
            {
                actionTutorial.SetActive(true);
                if (GameObject.Find("pointAtKingPrefab" + GameManager.Instance.getTurn().ToString()) != null)
                {
                    GameObject pointAtKing1Prefab = GameObject.Find("pointAtKingPrefab" + GameManager.Instance.getTurn().ToString());
                    float aliveTime = 2.0f;
                    Destroy(pointAtKing1Prefab, aliveTime);
                }

            }

            if (GameManager.Instance.currPlayer.getBHasKingdom())
            {
                actionTutorial.SetActive(false);

              
            }
        }
        if (GameManager.Instance.getTurn() == 1)
        {
            if (pressKing2Num == 0)
            {
                actionTutorial.SetActive(false);
            }

            if (pressKing2Num >= 1)
            {
                actionTutorial.SetActive(true);

                if (GameObject.Find("pointAtKingPrefab" + GameManager.Instance.getTurn().ToString()) != null)
                {
                    GameObject pointAtKing2Prefab = GameObject.Find("pointAtKingPrefab" + GameManager.Instance.getTurn().ToString());
                    float aliveTime = 2.0f;
                    Destroy(pointAtKing2Prefab, aliveTime);
                }

            }
            if (GameManager.Instance.currPlayer.getBHasKingdom())
            {
                actionTutorial.SetActive(false);
             
            }
        }
       
    }

    public void modifyPressKing1(int value) {
        pressKing1Num += value;
    }

    public void modifyPressKing2(int value) {
        pressKing2Num += value;
    }

    public void spawnPointAtKing(float x, float y) {
        GameObject pointAtKingPrefab1 = Instantiate(pointAtKingPrefab);
        pointAtKingPrefab1.name = "pointAtKingPrefab" + GameManager.Instance.getTurn().ToString();
        pointAtKingPrefab1.transform.position = new Vector2(x, y + 1);
    }
}
