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
    void setCamera(int x, int y) {
        //int x = GameManager.Instance.currPlayer.getOriginalIndex().x;
        //int y = GameManager.Instance.currPlayer.getOriginalIndex().y;
        Vector3 startPos = GameManager.Instance.getTileObjectByIndex("xIndex_" + x.ToString() + "yIndex_" + y.ToString()).transform.position;

        GameObject.Find("myCamera").transform.position = startPos;
    }
    void spawnKing1() {
        GameObject pKingUnit1 = Instantiate(heroUnit);
        pKingUnit1.GetComponent<HeroEntity>().setEntity(GlobalHeroIndex.eEntityType_King, 0);
        pKingUnit1.name = "king1";
        //pKingUnit1.GetComponent<MovableUnit>().indexX = 8;
        //pKingUnit1.GetComponent<MovableUnit>().indexY = 2;
        int x;
        int y;
        do
        {
            x = (int)Random.Range(8,GameManager.Instance.width);
            y = (int)Random.Range(0,5);
        } while (checkIfTileIsOnLand(x, y) == false);
        pKingUnit1.GetComponent<MovableUnit>().indexX = x;
        pKingUnit1.GetComponent<MovableUnit>().indexY = y;
        string tileName = "xIndex_" + pKingUnit1.GetComponent<MovableUnit>().indexX.ToString() + "yIndex_" + pKingUnit1.GetComponent<MovableUnit>().indexY.ToString();
        pKingUnit1.transform.position = GameManager.Instance.getTileObjectByIndex(tileName).transform.position;
        GameManager.Instance.m_player1.setOriginalIndex(new Vector2Int(x,y));
        setCamera(x,y);
        GameObject.Find("TutorialController").GetComponent<TutorialScript>().spawnPointAtKing(pKingUnit1.transform.position.x,pKingUnit1.transform.position.y);
        GameManager.Instance.removeFogOfWar(x,y,0);
    }
    bool checkIfTileIsOnLand(int x, int y) {
        string tileName = "xIndex_" + x.ToString() + "yIndex_" + y.ToString();
        GameObject tile = GameManager.Instance.getTileObjectByIndex(tileName);
        int type = tile.GetComponent<TileScript>().terrainType;
        if (type == GlobTileType.eTile_nSea || type == GlobTileType.eTile_nObstacle) {
            return false;
        }
        return true;
    }
    void spawnKing2()
    {
        GameObject pKingUnit2 = Instantiate(heroUnit);
        pKingUnit2.GetComponent<HeroEntity>().setEntity(GlobalHeroIndex.eEntityType_King, 1);
        pKingUnit2.name = "king2";
        //pKingUnit2.GetComponent<MovableUnit>().indexX = 8;
        // pKingUnit2.GetComponent<MovableUnit>().indexY = 10;

        int x;
        int y;
        do
        {
            x = (int)Random.Range(0, 8);
            y = (int)Random.Range(8, GameManager.Instance.height);
        } while (checkIfTileIsOnLand(x, y) == false);
        pKingUnit2.GetComponent<MovableUnit>().indexX = x;
        pKingUnit2.GetComponent<MovableUnit>().indexY = y;
        string tileName = "xIndex_" + pKingUnit2.GetComponent<MovableUnit>().indexX.ToString() + "yIndex_" + pKingUnit2.GetComponent<MovableUnit>().indexY.ToString();
        pKingUnit2.transform.position = GameManager.Instance.getTileObjectByIndex(tileName).transform.position;
        GameManager.Instance.m_player2.setOriginalIndex(new Vector2Int(x,y));
        GameManager.Instance.removeFogOfWar(x, y, 1);
        GameManager.Instance.m_player2.setFogOfWarActive(false);

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
        pHeroUnit.name = name;
        HeroManager.Instance.addHeroByName(name,pHeroUnit);
        if (nAlign == 0)
        {
            pHeroUnit.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = UIManager.Instance.heroChips[0];
        }
        else
        {
            pHeroUnit.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = UIManager.Instance.heroChips[1];
        }
        PlaceHero thisHeroPlacingScript = pHeroUnit.GetComponent<PlaceHero>();
        thisHeroPlacingScript.bMoveWithMouse = true;
        if (nType == GlobalHeroIndex.eEntityType_Miner) {
            int a = currPlayer.getWorkshopIndex().x;
            int b = currPlayer.getWorkshopIndex().y;
            index_GridCanPlaceHero(thisHeroPlacingScript, a, b);
        }
        else {
            int a = currPlayer.getTavernIndex().x;
            int b = currPlayer.getTavernIndex().y;
            index_GridCanPlaceHero(thisHeroPlacingScript, a, b);
        }
        /*
        string tileName = "xIndex_" + pHeroUnit.GetComponent<MovableUnit>().indexX.ToString() + "yIndex_" + pHeroUnit.GetComponent<MovableUnit>().indexY.ToString();
        pHeroUnit.transform.position = GameManager.Instance.getTileObjectByIndex(tileName).transform.position;

        GameManager.Instance.currPlayer.addHero(pHeroUnit);
        Vector2Int posIndex = new Vector2Int(pHeroUnit.GetComponent<MovableUnit>().indexX, pHeroUnit.GetComponent<MovableUnit>().indexY);
        GameManager.Instance.setTileHasUnit(posIndex,GameManager.Instance.getTileObjectByIndex(tileName));
        GameManager.Instance.setUnitInTile(posIndex,pHeroUnit);
        */
        //pHeroUnit.GetComponent<HeroEntity>().m_pHero.addStatus(name, HeroManager.Instance.pDamageBuff);
        totalHeroNum++;
    }

    void index_GridCanPlaceHero(PlaceHero thisHeroPlacingScript,int a, int b) {
        if (b % 2 == 1)
        {
            thisHeroPlacingScript.showPlaceableGrid(a + 1, b);
            thisHeroPlacingScript.showPlaceableGrid(a, b + 1);
            thisHeroPlacingScript.showPlaceableGrid(a + 1, b + 1);
            thisHeroPlacingScript.showPlaceableGrid(a + 1, b - 1);
            thisHeroPlacingScript.showPlaceableGrid(a, b - 1);
            thisHeroPlacingScript.showPlaceableGrid(a - 1, b);
            
        }   
        else
        {   
            thisHeroPlacingScript.showPlaceableGrid(a, b + 1);
            thisHeroPlacingScript.showPlaceableGrid(a + 1, b);
            thisHeroPlacingScript.showPlaceableGrid(a - 1, b + 1);
            thisHeroPlacingScript.showPlaceableGrid(a - 1, b);
            thisHeroPlacingScript.showPlaceableGrid(a - 1, b - 1);
            thisHeroPlacingScript.showPlaceableGrid(a, b - 1);
        }
    }


    
}
