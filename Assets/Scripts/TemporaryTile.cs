using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryTile : MonoBehaviour
{
    public int x;
    public int y;
    int initTurnNum;
    GameObject tileOfThisIndex;
    int nOldType;
    public int nNewType;

    // Update is called once per frame
    void Update()
    {
        int currentTurnNum = GameManager.Instance.getCurrentTurn();
        if (currentTurnNum - initTurnNum < 6) {
            return;
        }
        else {
            tileOfThisIndex.GetComponent<TileScript>().terrainType = nOldType;
            Destroy(gameObject);
        }
    }

    public void thisTileAwake(int x_, int y_, int newType_) {
        x = x_;
        y = y_;
        nNewType = newType_;
        initTurnNum = GameManager.Instance.getCurrentTurn();
        tileOfThisIndex = GameManager.Instance.getTileObjectByIndex("xIndex_" + x.ToString() + "yIndex_" + y.ToString());
        nOldType = tileOfThisIndex.GetComponent<TileScript>().terrainType;
        tileOfThisIndex.GetComponent<TileScript>().terrainType = nNewType;
    }
}
