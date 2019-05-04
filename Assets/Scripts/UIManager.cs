using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject mouseController;
    public List<Sprite> tileSprites;
    public List<Sprite> heroIconOnMapSrpites;
    [Header("Kingdom")]
    public GameObject kingdomCanvas;
    [Header("Tavern")]
    public GameObject TavernCanvas;
    public GameObject TavernPanel;
    public List<GameObject> heroCardList;
    public List<Text> heroNameList;
    public List<Text> heroSkillDescription;
    [Header("hero information on map")]
    public Text heroNameText;
    public Text currHPText;
    public Text currMoveText;
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
        if (Input.GetKeyDown(KeyCode.Space)) {
            exitKingdomCanvas();
        }
    }

    public void openKingdomCanvas() {
        kingdomCanvas.SetActive(true);
    }

    public void exitKingdomCanvas() {
        kingdomCanvas.SetActive(false);
        //mouseController.GetComponent<MouseController>().playerState = GlobPlayerAction.ePlayerState_Normal;
    }

    public void openTavernCanvas() {
        TavernCanvas.SetActive(true);
    }

    public void exitTavernCanvas() {
        TavernCanvas.SetActive(false);
        mouseController.GetComponent<MouseController>().playerState = GlobPlayerAction.ePlayerState_Normal;
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

    public void setHeroInformation(GameObject pSelectedHero) {
        int nEntityType = pSelectedHero.GetComponent<HeroEntity>().nEntityType;
        HeroEntity.Heroes m_pHero = pSelectedHero.GetComponent<HeroEntity>().m_pHero;
        HeroData thisHeroData = HeroManager.Instance.getHeroDataDic(nEntityType);
        int currHP = m_pHero.getCurrHP();
        int currMoveStep = m_pHero.getCurrentMoveStep();

        heroNameText.text = thisHeroData.m_strName;
        currHPText.text = currHP.ToString();
        currMoveText.text = currMoveStep.ToString();
    }


    void resetUIManager() {
        Instance = this;
    }
}
