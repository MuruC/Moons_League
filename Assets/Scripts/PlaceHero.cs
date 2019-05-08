using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHero : MonoBehaviour
{
    public int nType;
    public bool bMoveWithMouse;
    public List<GameObject> placeableGrid;
    GameObject mouseControllerObj;
    MouseController mouseControllerScript;
    // Start is called before the first frame update
    void Start()
    {
        mouseControllerObj = GameObject.Find("mouseController");
        mouseControllerScript = mouseControllerObj.GetComponent<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bMoveWithMouse)
        {
            moveWithMouse();

            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit != null)
                {
                    for (int i = 0; i < hit.Length; i++)
                    {
                        GameObject ourHitObj = hit[i].collider.transform.gameObject;

                        if (ourHitObj.GetComponent<TileScript>() != null)
                        {
                            if (placeableGrid.Contains(ourHitObj) == false)
                            {
                                return;
                            }
                            buildHero(ourHitObj);
                        }
                    }
                }
            }
        }
    }


    void moveWithMouse()
    {
        Vector3 mousePixelCoords = Input.mousePosition;
        Vector3 mouseWorldCoords = Camera.main.ScreenToWorldPoint(mousePixelCoords);
        transform.position = new Vector3(mouseWorldCoords.x, mouseWorldCoords.y, 0);
    }

    public void showPlaceableGrid(int x, int y) {
        string name = "xIndex_" + x.ToString() + "yIndex_" + y.ToString();
        Vector2Int tileIndex = new Vector2Int(x,y);
        if (GameManager.Instance.bContainThisTile(name) == false)
        {
            return;
        }

        if (GameManager.Instance.bTileHasUnit(tileIndex))
        {
            return;
        }

        GameObject terrain = GameManager.Instance.getTileObjectByIndex(name);
        TileScript thisTileScript = terrain.GetComponent<TileScript>();
        if (thisTileScript.terrainType == GlobTileType.eTile_nWalkable)
        {
            thisTileScript.setColorAnime(GlobTileColorAnimeIndex.eTileColor_build);
            placeableGrid.Add(terrain);
        }
    }

    void buildHero(GameObject tile) {
        TileScript tileScript = tile.GetComponent<TileScript>();
        int x = tileScript.x;
        int y = tileScript.y;
        Vector2Int posIndex = new Vector2Int(x,y);

        gameObject.GetComponent<MovableUnit>().indexX = x;
        gameObject.GetComponent<MovableUnit>().indexY = y;

        Vector2 pos = tile.transform.position;
        transform.position = pos;
        gameObject.GetComponent<MovableUnit>().destination = pos;

        GameManager.Instance.currPlayer.addHero(gameObject);
        GameManager.Instance.setTileHasUnit(posIndex,tile);
        GameManager.Instance.setUnitInTile(posIndex,tile);

        resetPlaceHeroState();
    }

    void resetPlaceHeroState() {
        bMoveWithMouse = false;
        mouseControllerScript.playerState = GlobPlayerAction.ePlayerState_Normal;

        for (int i = 0; i < placeableGrid.Count; i++)
        {
            placeableGrid[i].GetComponent<TileScript>().setColorAnime(GlobTileColorAnimeIndex.eTileColor_null);
        }

        placeableGrid.Clear();
    }

    bool placeableLstContainHitTile(GameObject hitTile) {
        bool contain = false;
        Vector2 hitTilePos = transform.position;

        for (int i = 0; i < placeableGrid.Count; i++)
        {
            Vector2 tilePos = placeableGrid[i].transform.position;
            if (hitTilePos == tilePos)
            {
                contain = true;
            }
        }
        return contain;
    }

}
