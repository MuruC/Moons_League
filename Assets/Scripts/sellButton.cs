using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class sellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject textPanel;
    public GameObject mouseControllerObj;
    MouseController mouseControllScript;
    public void OnPointerEnter(PointerEventData eventData)
    {
        textPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textPanel.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        mouseControllScript = mouseControllerObj.GetComponent<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sellTheCharacter() {
        if (mouseControllScript.selectedUnitObj == null)
        {
            return;
        }
            HeroEntity hero = mouseControllScript.selectedUnitObj.GetComponent<HeroEntity>();
        SoundEffectManager.Instance.playAudio(2);
        if (hero.nEntityType == GlobalHeroIndex.eEntityType_King)
        {
            return;
        }
            MovableUnit heroMoving = mouseControllScript.selectedUnitObj.GetComponent<MovableUnit>();
            HeroManager.Instance.removeHero(hero.nAlign, mouseControllScript.selectedUnitObj);

            GameManager.Instance.currPlayer.removeHero(mouseControllScript.selectedUnitObj);
            GameManager.Instance.currPlayer.modifyCurrentHeroNum(-1);
            GameManager.Instance.currPlayer.modifyMoney((int)(HeroManager.Instance.getHeroDataDic(hero.nEntityType).m_nMoneyTobuy/2));

        Vector2Int pos = new Vector2Int(heroMoving.indexX, heroMoving.indexY);
        GameManager.Instance.modifyTileHasUnit(pos);
        GameManager.Instance.modifyUnitInTile(pos);
        HeroManager.Instance.addHeroCountInStock(hero.nEntityType, 1);

        mouseControllScript.playerState = GlobPlayerAction.ePlayerState_Normal;
        mouseControllScript.resetSelectedHeroObj();
        Destroy(mouseControllScript.selectedUnitObj);

    }
}
