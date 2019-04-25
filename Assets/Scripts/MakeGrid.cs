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

    }

    void setTerrain(int x,int y, int spriteIndex,int terrainType) {
        GameObject pTile = GameManager.Instance.getTileObjectByIndex("xIndex_" + x.ToString() + "yIndex_" + y.ToString());
        pTile.GetComponent<TileScript>().terrainType = terrainType;
        pTile.GetComponent<TileScript>().SetSprite(spriteIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
