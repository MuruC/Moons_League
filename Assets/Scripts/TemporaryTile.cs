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
    private void Awake()
    {
        initTurnNum = GameManager.Instance.getCurrentTurn();
        tileOfThisIndex = GameManager.Instance.getTileObjectByIndex("xIndex_"+x.ToString()+"yIndex_"+y.ToString());
        nOldType = tileOfThisIndex.GetComponent<TileScript>().terrainType;
        tileOfThisIndex.GetComponent<TileScript>().terrainType = nNewType;
    }

    // Update is called once per frame
    void Update()
    {
        int currentTurnNum = GameManager.Instance.getCurrentTurn();
        if (currentTurnNum - initTurnNum < 3) {
            return;
        }
        else {
            tileOfThisIndex.GetComponent<TileScript>().terrainType = nOldType;
            Destroy(gameObject);
        }
    }
}
