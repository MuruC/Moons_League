using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeGrid : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject fogTilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance == null) {
            return;
        }
        float xOffset = GameManager.Instance.xOffset;
        float yOffset = GameManager.Instance.yOffset;
        for (int x = 0; x < GameManager.Instance.width; x++)
        {
            for (int y = 0; y < GameManager.Instance.height; y++)
            {
                float arrayXPos = x * xOffset;
                float arrayYpos = 0;
                GameObject newTile = Instantiate(tilePrefab);

                if (y % 2 == 1) {
                    arrayXPos += xOffset / 2;
                }

                arrayYpos = y * yOffset;

                newTile.name = "Terrain_" + x + "_" + y;
                newTile.GetComponent<TileScript>().x = x;
                newTile.GetComponent<TileScript>().y = y;

                GameManager.Instance.setTerrainPos(newTile,arrayXPos - GameManager.Instance.xBeginPos,arrayYpos-GameManager.Instance.yBeginPos);
                GameManager.Instance.setTileObjectByIndex("xIndex_" + x.ToString() + "yIndex_" + y.ToString(), newTile);

                
            }
        }
        setTerrainTypeIndex();
    }

    void setTerrainTypeIndex() {
        for (int x = 0; x < GameManager.Instance.width; x++)
        {
            setTerrain(x, 0, GlobTileType.eTile_nSea);
        }
        setTerrain(5, 0, GlobTileType.eTile_nWalkable);
        setTerrain(9, 0, GlobTileType.eTile_nWalkable);
        setTerrain(10, 0, GlobTileType.eTile_nWalkable);
        setTerrain(13, 0, GlobTileType.eTile_nWalkable);
        setTerrain(14, 0, GlobTileType.eTile_nWalkable);
        setTerrain(15, 0, GlobTileType.eTile_nWalkable);

        for (int x = 0; x < GameManager.Instance.width; x++)
        {
            setTerrain(x, 1, GlobTileType.eTile_nWalkable);
        }

        setTerrain(0, 1, GlobTileType.eTile_nSea);
        setTerrain(15, 1, GlobTileType.eTile_nSea);
        setTerrain(3, 1, GlobTileType.eTile_nSea);
        setTerrain(11, 1, GlobTileType.eTile_nSea);
        setTerrain(9, 1, GlobTileType.eTile_nGoldMine);

        for (int x = 0; x < GameManager.Instance.width; x++)
        {
            setTerrain(x, 2, GlobTileType.eTile_nWalkable);
        }
        setTerrain(0, 2, GlobTileType.eTile_nSea);
        setTerrain(15, 2, GlobTileType.eTile_nSea);
        setTerrain(3, 2, GlobTileType.eTile_nGoldMine);
        setTerrain(5, 2, GlobTileType.eTile_nGoldMine);
        setTerrain(13, 2, GlobTileType.eTile_nGoldMine);
        setTerrain(6, 2, GlobTileType.eTile_nObstacle);
        setTerrain(7, 2, GlobTileType.eTile_nObstacle);
        setTerrain(10, 2, GlobTileType.eTile_nObstacle);
        setTerrain(11, 2, GlobTileType.eTile_nObstacle);

        for (int x = 0; x < GameManager.Instance.width; x++)
        {
            setTerrain(x, 3, GlobTileType.eTile_nWalkable);
        }
        setTerrain(1, 3, GlobTileType.eTile_nObstacle);
        setTerrain(5, 3, GlobTileType.eTile_nObstacle);
        setTerrain(6, 3, GlobTileType.eTile_nObstacle);
        setTerrain(15, 3, GlobTileType.eTile_nSea);

        for (int x = 0; x < GameManager.Instance.width; x++)
        {
            for (int y = 4; y < GameManager.Instance.height - 1; y ++) {
                setTerrain(x, y, GlobTileType.eTile_nWalkable);
            }
        }
        setTerrain(1, 4, GlobTileType.eTile_nObstacle);
        setTerrain(2, 4, GlobTileType.eTile_nGoldMine);
        setTerrain(13, 4, GlobTileType.eTile_nObstacle);
        setTerrain(12, 4, GlobTileType.eTile_nGoldMine);
        setTerrain(9, 4, GlobTileType.eTile_nGoldMine);
        setTerrain(15, 4, GlobTileType.eTile_nSea);
        setTerrain(3, 5, GlobTileType.eTile_nObstacle);
        setTerrain(12, 5, GlobTileType.eTile_nObstacle);
        setTerrain(13, 5, GlobTileType.eTile_nObstacle);
        setTerrain(15, 5, GlobTileType.eTile_nSea);
        setTerrain(3, 6, GlobTileType.eTile_nObstacle);
        setTerrain(4, 6, GlobTileType.eTile_nObstacle);
        setTerrain(5, 6, GlobTileType.eTile_nGoldMine);
        setTerrain(7, 6, GlobTileType.eTile_nLake);
        setTerrain(8, 6, GlobTileType.eTile_nLake);
        setTerrain(9, 6, GlobTileType.eTile_nObstacle);
        setTerrain(12, 6, GlobTileType.eTile_nGoldMine);
        setTerrain(13, 6, GlobTileType.eTile_nObstacle);
        setTerrain(15, 6, GlobTileType.eTile_nSea);
        setTerrain(3, 7, GlobTileType.eTile_nObstacle);
        setTerrain(4, 7, GlobTileType.eTile_nGoldMine);
        setTerrain(7, 7, GlobTileType.eTile_nLake);
        setTerrain(12, 7, GlobTileType.eTile_nGoldMine);
        setTerrain(14, 7, GlobTileType.eTile_nSea);
        setTerrain(15, 7, GlobTileType.eTile_nSea);
        setTerrain(0, 8, GlobTileType.eTile_nSea);
        setTerrain(4, 8, GlobTileType.eTile_nObstacle);
        setTerrain(10, 8, GlobTileType.eTile_nGoldMine);
        setTerrain(15, 8, GlobTileType.eTile_nSea);
        setTerrain(5, 9, GlobTileType.eTile_nGoldMine);
        setTerrain(12, 9, GlobTileType.eTile_nObstacle);
        setTerrain(15, 9, GlobTileType.eTile_nSea);
        setTerrain(3, 10, GlobTileType.eTile_nGoldMine);
        setTerrain(4, 10, GlobTileType.eTile_nObstacle);
        setTerrain(7, 10, GlobTileType.eTile_nGoldMine);
        setTerrain(8, 10, GlobTileType.eTile_nGoldMine);
        setTerrain(11, 10, GlobTileType.eTile_nObstacle);
        setTerrain(12, 10, GlobTileType.eTile_nObstacle);
        setTerrain(2, 11, GlobTileType.eTile_nSea);
        setTerrain(3, 11, GlobTileType.eTile_nObstacle);
        setTerrain(11, 11, GlobTileType.eTile_nGoldMine);
        setTerrain(12, 11, GlobTileType.eTile_nObstacle);
        setTerrain(13, 11, GlobTileType.eTile_nSea);
        setTerrain(14, 11, GlobTileType.eTile_nSea);
        setTerrain(15, 11, GlobTileType.eTile_nSea);

        for (int x = 0; x < GameManager.Instance.width; x++)
        {
            setTerrain(x, 12, GlobTileType.eTile_nSea);
        }
        setTerrain(4, 12, GlobTileType.eTile_nWalkable);
        setTerrain(5, 12, GlobTileType.eTile_nWalkable);
        setTerrain(11, 12, GlobTileType.eTile_nWalkable);
        setTerrain(12, 12, GlobTileType.eTile_nWalkable);
    }

    void setTerrain(int x,int y, int terrainType) {
        GameObject pTile = GameManager.Instance.getTileObjectByIndex("xIndex_" + x.ToString() + "yIndex_" + y.ToString());
        pTile.GetComponent<TileScript>().terrainType = terrainType;
        pTile.GetComponentInChildren<TextMesh>().text = terrainType.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
