using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceStructure : MonoBehaviour
{
    public int nType;
    public bool bMoveWithMouse;
    public List<GameObject> placeableGrid;
    GameObject mouseControllerObj;
    MouseController mouseControllerScript;
    public GameObject tavernTutorialPrefab;
    public GameObject workshopTutorialPrefab;
    // Start is called before the first frame update
    void Start()
    {
        mouseControllerObj = GameObject.Find("mouseController");
        mouseControllerScript = mouseControllerObj.GetComponent<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bMoveWithMouse) {
            moveWithMouse();

            if (Input.GetMouseButtonUp(0)) {
                RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit != null)
                {
                    for (int i = 0; i < hit.Length; i++)
                    {
                        GameObject ourHitObject = hit[i].collider.transform.gameObject;
                        if (ourHitObject.GetComponent<TileScript>() != null) {
                            if (placeableGrid.Contains(ourHitObject) == false)
                            {
                                return;
                            }
                            buildStructure(ourHitObject);
                        }
                    }
                }
            }
        }
    }

    void moveWithMouse() {
        Vector3 mousePixelCoords = Input.mousePosition;
        Vector3 mouseWorldCoords = Camera.main.ScreenToWorldPoint(mousePixelCoords);
        transform.position = new Vector3(mouseWorldCoords.x,mouseWorldCoords.y,0);
    }

    public void showPlaceableGrid(int x, int y) {
        string name = "xIndex_" + x.ToString() + "yIndex_" + y.ToString();
        Vector2Int tileIndex = new Vector2Int(x,y); 
        if (GameManager.Instance.bContainThisTile(name) == false) {
            return;
        }
        if (GameManager.Instance.bTileHasUnit(tileIndex)) {
            return;
        }
        GameObject terrain = GameManager.Instance.getTileObjectByIndex(name);
        TileScript thisTileScript = terrain.GetComponent<TileScript>();
        if (thisTileScript.terrainType == GlobTileType.eTile_nWalkable) {
            thisTileScript.setColorAnime(GlobTileColorAnimeIndex.eTileColor_build);
            placeableGrid.Add(terrain);
        }
        
    }

    void buildStructure(GameObject tile) {
        Structure thisStructureScipt = gameObject.GetComponent<Structure>();
        thisStructureScipt.x = tile.GetComponent<TileScript>().x;
        thisStructureScipt.y = tile.GetComponent<TileScript>().y;
        Vector3 pos = tile.transform.position;
        gameObject.transform.position = pos;
        bMoveWithMouse = false;
        mouseControllerScript.playerState = GlobPlayerAction.ePlayerState_Normal;
        for (int i = 0; i < placeableGrid.Count; i++)
        {
            placeableGrid[i].GetComponent<TileScript>().setColorAnime(GlobTileColorAnimeIndex.eTileColor_null);
        }
        placeableGrid.Clear();

        if (thisStructureScipt.nType == GlobStructureType.eStructure_nTavern) {
            GameManager.Instance.currPlayer.setTavernIndex(new Vector2Int(gameObject.GetComponent<Structure>().x, gameObject.GetComponent<Structure>().y));
            GameObject tavernTutorial = Instantiate(tavernTutorialPrefab);
            tavernTutorial.transform.position = new Vector2(pos.x, pos.y + 1);

        } else if (thisStructureScipt.nType == GlobStructureType.eStructure_nWorkshop) {
            GameManager.Instance.currPlayer.setWorkShopIndex(new Vector2Int(thisStructureScipt.x,thisStructureScipt.y));
            GameObject workshopTutorial = Instantiate(workshopTutorialPrefab);
            workshopTutorial.transform.position = new Vector2(pos.x, pos.y + 1);
            if (GameManager.Instance.getTurn() == 0)
            {
                GameObject.Find("TutorialController").GetComponent<TutorialScript>().king1_buildWorkshop = true;
            }
            else
            {
                GameObject.Find("TutorialController").GetComponent<TutorialScript>().king2_buildWorkshop = true;
            }
        }
    }
}
