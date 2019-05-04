using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject heroUnit;
    public GameObject minerUnit;
    int totalHeroNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        spawnKing1();
        spawnKing2();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void spawnKing1() {
        GameObject pKingUnit1 = Instantiate(heroUnit);
        pKingUnit1.GetComponent<HeroEntity>().setEntity(GlobalHeroIndex.eEntityType_King, 0);
        pKingUnit1.GetComponent<MovableUnit>().indexX = 8;
        pKingUnit1.GetComponent<MovableUnit>().indexY = 2;
        string tileName = "xIndex_" + pKingUnit1.GetComponent<MovableUnit>().indexX.ToString() + "yIndex_" + pKingUnit1.GetComponent<MovableUnit>().indexY.ToString();
        pKingUnit1.transform.position = GameManager.Instance.getTileObjectByIndex(tileName).transform.position;

    }

    void spawnKing2()
    {
        GameObject pKingUnit2 = Instantiate(heroUnit);
        pKingUnit2.GetComponent<HeroEntity>().setEntity(GlobalHeroIndex.eEntityType_King, 1);
        pKingUnit2.GetComponent<MovableUnit>().indexX = 8;
        pKingUnit2.GetComponent<MovableUnit>().indexY = 10;
        string tileName = "xIndex_" + pKingUnit2.GetComponent<MovableUnit>().indexX.ToString() + "yIndex_" + pKingUnit2.GetComponent<MovableUnit>().indexY.ToString();
        pKingUnit2.transform.position = GameManager.Instance.getTileObjectByIndex(tileName).transform.position;

    }

    public void spawnHero(int nType) {
        PlayerAlly.Players currPlayer = GameManager.Instance.currPlayer;

        int playerMaxHeroNum = currPlayer.getMaxHeroNum();
        int playerCurrHeroNum = currPlayer.getCurrHeroNum();
        if (playerCurrHeroNum >= playerMaxHeroNum) {
            return;
        }

        int nAlign = GameManager.Instance.getTurn();
        GameObject pHeroUnit = Instantiate(heroUnit);
        pHeroUnit.GetComponent<HeroEntity>().setEntity(nType,nAlign);
        HeroManager.Instance.addHero(nAlign,pHeroUnit);
        string name = "player_" + nAlign.ToString() + "entity_" + nType.ToString() + "num_" + totalHeroNum.ToString();
        pHeroUnit.GetComponent<HeroEntity>().m_pHero.setName(name);
        HeroManager.Instance.addHeroByName(name,pHeroUnit);

        pHeroUnit.GetComponent<MovableUnit>().indexX = currPlayer.getTavernIndex().x;
        pHeroUnit.GetComponent<MovableUnit>().indexY = currPlayer.getTavernIndex().y;
        string tileName = "xIndex_" + pHeroUnit.GetComponent<MovableUnit>().indexX.ToString() + "yIndex_" + pHeroUnit.GetComponent<MovableUnit>().indexY.ToString();
        pHeroUnit.transform.position = GameManager.Instance.getTileObjectByIndex(tileName).transform.position;

        GameManager.Instance.currPlayer.addHero(pHeroUnit);
        Vector2Int posIndex = new Vector2Int(pHeroUnit.GetComponent<MovableUnit>().indexX, pHeroUnit.GetComponent<MovableUnit>().indexY);
        GameManager.Instance.setTileHasUnit(posIndex,GameManager.Instance.getTileObjectByIndex(tileName));
        GameManager.Instance.setUnitInTile(posIndex,pHeroUnit);
        //pHeroUnit.GetComponent<HeroEntity>().m_pHero.addStatus(name, HeroManager.Instance.pDamageBuff);
        totalHeroNum++;
    }
}
