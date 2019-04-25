using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public List<Sprite> tileSprites;
    public GameObject kingdomCanvas;
    [Header("Tavern")]
    public GameObject TavernPanel;
    public List<GameObject> heroCardList;
    public List<Text> heroNameList;
    public List<Text> heroSkillDescription;
    [Header("hero information on map")]
    public Text heroName;
    public Text currHP;
    public Text currMove;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null) {
            return;
        }
        resetUIManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openKingdomCanvas() {
        kingdomCanvas.SetActive(true);
    }

    public void exitTavernPanel() {
        TavernPanel.SetActive(false);
    }

    public void setHeroName(int heroCardIndex,string name) {
        heroNameList[heroCardIndex].text = name;
    }

    public void setHeroSkillDescriptionInTavern(int heroCardIndex,string description) {
        heroSkillDescription[heroCardIndex].text = description;
    }

    public void setCardActiveEveryTurn() {
        for (int i = 0; i < heroCardList.Count; i++)
        {
            heroCardList[i].SetActive(true);
        }
    }

    void resetUIManager() {
        Instance = this;
    }
}
