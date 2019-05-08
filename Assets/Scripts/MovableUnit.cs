using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableUnit : MonoBehaviour
{
    public bool thisUnitHasBeenClicked = false;
    public Vector2 destination;
    public int destinationXIndex;
    public int destinationYIndex;
    public float speed;
    public bool bMoving = false;
    public List<GameObject> walkableTiles;
    HeroEntity.Heroes m_pHero;
    public int indexX;
    public int indexY;
    // Start is called before the first frame update
    void Start()
    {
        destination = transform.position;
        walkableTiles = new List<GameObject>();
        m_pHero = gameObject.GetComponent<HeroEntity>().m_pHero;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x == destination.x && transform.position.y == destination.y)
        {
            if (bMoving)
            {
                bMoving = false;
                whenUnitFirstArriveDestination();
            }
        }
        else
        {
            bMoving = true;
        }

        thisUnitHasBeenClicked = false;

        Vector2 dir = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        Vector2 velocity = dir.normalized * speed * Time.deltaTime;

        velocity = Vector2.ClampMagnitude(velocity, dir.magnitude);

        transform.Translate(velocity);

        m_pHero.setPos(new Vector2Int(indexX,indexY));
    }

    void whenUnitFirstArriveDestination() {
        if (thisUnitHasBeenClicked == false) {
            clearWalkableTileList();
        }

        int type = gameObject.GetComponent<HeroEntity>().nEntityType;
        if (type == GlobalHeroIndex.eEntityType_Miner)
        {
            if (GameManager.Instance.getTileObjectByIndex("xIndex_"+ indexX.ToString() + "yIndex_"+indexY.ToString()).GetComponent<TileScript>().terrainType == GlobTileType.eTile_nGoldMine)
            {
                GameManager.Instance.currPlayer.modifyMoney(20);
            }
        }

        if (GameManager.Instance.getTileObjectByIndex("xIndex_" + indexX.ToString() + "yIndex_" + indexY.ToString()).GetComponent<TileScript>().terrainType == GlobTileType.eTile_nLake)
        {
            gameObject.GetComponent<HeroEntity>().m_pHero.eliminateAllDebuffByName();
        }
    }

    public void clearWalkableTileList() {
        for (int i = 0; i < walkableTiles.Count; i++)
        {
            walkableTiles[i].GetComponent<TileScript>().setColorAnime(GlobTileColorAnimeIndex.eTileColor_null);
        }
        walkableTiles.Clear();
    }
}
