﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GlobTileType {
    public const int eTile_nWalkable = 0;
    public const int eTile_nObstacle = 1;
    public const int eTile_nGoldMine = 2;
    public const int eTile_nLake = 3;
    public const int eTile_nSea = 4;
}

public static class GlobStructureType {
    public const int eStructure_nKingdom = 0;
    public const int eStructure_nTavern = 1;
    public const int eStructure_nWorkshop = 2;
    public const int eStructure_nFarm = 3;
}

public static class GlobTileColorAnimeIndex {
    public const int eTileColor_null = 0;
    public const int eTileColor_build = 1;
    public const int eTileColor_walkable = 2;
}

public static class GlobPlayerAction {
    public const int ePlayerState_Normal = 0;
    public const int ePlayerState_Building = 1;
    public const int ePlayerState_DoAction = 2;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int nTurn;

    public int width;
    public int height;
    public float xOffset;
    public float yOffset;
    public int xBeginPos;
    public int yBeginPos;
    GameObject gridHolder;
    private Dictionary<string, GameObject> tileObjectByIndex;
    private Dictionary<Vector2Int, GameObject> tileHasUnit;
    private Dictionary<Vector2Int, GameObject> unitInTile;
    private int currentTurnNum;

    public PlayerAlly.Players m_player1;
    public PlayerAlly.Players m_player2;
    public PlayerAlly.Players currPlayer;

    public GameObject mouseController;
    public GameObject tutorialController;
    TutorialScript tutorialControllerScript;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            return;
        }

        resetGameManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setTerrainPos(GameObject pTile, float xPos, float yPos)
    {
        Vector2 terrainPos = new Vector2(xPos, yPos);
        pTile.transform.parent = gridHolder.transform;
        pTile.transform.position = terrainPos;
    }

    public void setTileObjectByIndex(string indexToString, GameObject ptile)
    {
        tileObjectByIndex[indexToString] = ptile;
    }

    public GameObject getTileObjectByIndex(string indexToString)
    {
        return tileObjectByIndex[indexToString];
    }

    public Dictionary<string, GameObject> getTileObjByIndexDic() {
        return tileObjectByIndex;
    }

    public bool bContainThisTile(string name) {
        if (tileObjectByIndex.ContainsKey(name)) {
            return true;
        }
        else {
            return false;
        }
    }

    public void setTurn() {
        HeroSkill.Instance.resetHeroSkillState();
        if (nTurn == 0) {
            nTurn = 1;
            currPlayer = m_player2;
            currPlayer.resetEveryHeroInfoOnTurn();
            setCameraEveryTurn();
            addCurrentTurnNum();
            if (getCurrentTurn() == 0)
            {
                tutorialControllerScript.spawnPointAtKing(GameObject.Find("king2").transform.position.x, GameObject.Find("king2").transform.position.y);
            }
            m_player1.setFogOfWarActive(false);
            m_player2.setFogOfWarActive(true);
        }
        else if(nTurn == 1){
            nTurn = 0;
            currPlayer = m_player1;
            currPlayer.resetEveryHeroInfoOnTurn();
            setCameraEveryTurn();
            addCurrentTurnNum();

            m_player1.modifyMoney(2);
            m_player2.modifyMoney(2);

            m_player1.setFogOfWarActive(true);
            m_player2.setFogOfWarActive(false);
        }
        HeroManager.Instance.generateHeroEveryTurn();
        if (GameObject.Find("king1") != null)
        {
            GameObject.Find("king1").GetComponent<HeroEntity>().resetHeroInfoEveryTurn();
        }

        if (GameObject.Find("king2") != null)
        {
            GameObject.Find("king2").GetComponent<HeroEntity>().resetHeroInfoEveryTurn();
        }
        UIManager.Instance.setCardActiveEveryTurn();
        mouseController.GetComponent<MouseController>().resetSelectedHeroObj();
    }

    public int getTurn() { return nTurn; }

    public void addCurrentTurnNum() {
        currentTurnNum += 1;
    }
    public int getCurrentTurn() {
        return currentTurnNum;
    }

    public void setTileHasUnit(Vector2Int tileIndex,GameObject pTileHasGrid) {
        tileHasUnit[tileIndex] = pTileHasGrid;
    }

    public void modifyTileHasUnit(Vector2Int tileIndex) {
        tileHasUnit.Remove(tileIndex);
    }

    public Dictionary<Vector2Int, GameObject> getTileHasUnitDic() {
        return tileHasUnit;
    }

    public bool bTileHasUnit(Vector2Int tileIndex) {
        if (tileHasUnit.ContainsKey(tileIndex))
        {
            return true;
        }
        else {
            return false;
        }
    }

    public void setUnitInTile(Vector2Int tileIndex, GameObject unit) {
        unitInTile[tileIndex] = unit;
    }
    public void modifyUnitInTile(Vector2Int tileIndex) {
        unitInTile.Remove(tileIndex);
    }
    public GameObject getUnitInTile(Vector2Int tileIndex) {
        return unitInTile[tileIndex];
    }
    public bool bUnitInTile(Vector2Int tileIndex) {
        if (unitInTile.ContainsKey(tileIndex))
        {
            return true;
        }
        else {
            return false;
        }
    }

    public Dictionary<Vector2Int, GameObject> getUnitInTileDic() {
        return unitInTile;
    }

    public void setCameraEveryTurn() {
        int x;
        int y;
        if (currPlayer.getBHasKingdom() == false)
        {
            x = currPlayer.getOriginalIndex().x;
            y = currPlayer.getOriginalIndex().y;
        }
        else {
            x = currPlayer.getKingdomIndex().x;
            y = currPlayer.getKingdomIndex().y;
        }
        Vector3 startPos = getTileObjectByIndex("xIndex_" + x.ToString() + "yIndex_" + y.ToString()).transform.position;
        GameObject.Find("myCamera").transform.position = startPos;
    }

    public void removeFogOfWar(int a, int b, int i)
    {
        if (i == 0)
        {
            if (b % 2 == 1)
            {
                m_player1.removeFogOfWar(a + 1, b);
                m_player1.removeFogOfWar(a, b + 1);
                m_player1.removeFogOfWar(a + 1, b + 1);
                m_player1.removeFogOfWar(a + 1, b - 1);
                m_player1.removeFogOfWar(a, b - 1);
                m_player1.removeFogOfWar(a - 1, b);
                m_player1.removeFogOfWar(a, b);

            }
            else
            {
                m_player1.removeFogOfWar(a, b + 1);
                m_player1.removeFogOfWar(a + 1, b);
                m_player1.removeFogOfWar(a - 1, b + 1);
                m_player1.removeFogOfWar(a - 1, b);
                m_player1.removeFogOfWar(a - 1, b - 1);
                m_player1.removeFogOfWar(a, b - 1);
                m_player1.removeFogOfWar(a, b);
            }
        }
        else if (i == 1)
        {
            if (b % 2 == 1)
            {
                m_player2.removeFogOfWar(a + 1, b);
                m_player2.removeFogOfWar(a, b + 1);
                m_player2.removeFogOfWar(a + 1, b + 1);
                m_player2.removeFogOfWar(a + 1, b - 1);
                m_player2.removeFogOfWar(a, b - 1);
                m_player2.removeFogOfWar(a - 1, b);
                m_player2.removeFogOfWar(a, b);

            }
            else
            {
                m_player2.removeFogOfWar(a, b + 1);
                m_player2.removeFogOfWar(a + 1, b);
                m_player2.removeFogOfWar(a - 1, b + 1);
                m_player2.removeFogOfWar(a - 1, b);
                m_player2.removeFogOfWar(a - 1, b - 1);
                m_player2.removeFogOfWar(a, b - 1);
                m_player2.removeFogOfWar(a, b);
            }
        }
    }

        void resetGameManager() {
        Instance = this;
        nTurn = 0;
        tileObjectByIndex = new Dictionary<string, GameObject>();
        gridHolder = new GameObject();
        gridHolder.transform.position = new Vector3(-1f, 0.5f, 0);
        HeroManager.Instance.generateHeroEveryTurn();
        tileHasUnit = new Dictionary<Vector2Int, GameObject>();
        unitInTile = new Dictionary<Vector2Int, GameObject>();
        currPlayer = m_player1;

        tutorialControllerScript = tutorialController.GetComponent<TutorialScript>();
    }
}
