using System.Collections;
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

    public void setTurn() {
        if (nTurn == 0) {
            nTurn = 1;
        }else if(nTurn == 1){
            nTurn = 0;
        }
        HeroManager.Instance.generateHeroEveryTurn();
        UIManager.Instance.setCardActiveEveryTurn();
    }

    public int getTurn() { return nTurn; }


    void resetGameManager() {
        Instance = this;
        nTurn = 0;
        tileObjectByIndex = new Dictionary<string, GameObject>();
        gridHolder = new GameObject();
        gridHolder.transform.position = new Vector3(-1f, 0.5f, 0);
        HeroManager.Instance.generateHeroEveryTurn();

    }
}
