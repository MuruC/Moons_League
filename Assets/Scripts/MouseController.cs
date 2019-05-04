using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    public GameObject UnitSpawner;
    public GameObject tavernPrefab;
    public GameObject farmPrefab;
    public GameObject workshopPrefab;
    public GameObject kingdomIcon;
    public int playerState = 0;
    public GameObject selectedUnitObj;
    HeroEntity thisHeroScript;
    MovableUnit thisHeroMovingScript;
    PlayerAlly.Players currPlayer;
    // Start is called before the first frame update
    void Start()
    {
        playerState = GlobPlayerAction.ePlayerState_Normal;
    }


    // Update is called once per frame
    void Update()
    {
        currPlayer = GameManager.Instance.currPlayer;
        clickMouseLeftButton();
        clickMouseRightButton();
    }

    private bool isMouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void clickMouseRightButton() {
        if (Input.GetMouseButtonUp(1)) {
            playerState = GlobPlayerAction.ePlayerState_Normal;
            HeroSkill.Instance.resetHeroSkillState();
        }
    }

    void clickMouseLeftButton()
    {
        if (Input.GetMouseButtonUp(0)) {
            if (isMouseOverUI())
            {
                if (playerState == GlobPlayerAction.ePlayerState_Normal) {
                    clickOnUI();
                }
            }
            else
            {
                RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit != null)
                {
                    for (int i = 0; i < hit.Length; i++)
                    {
                        GameObject ourHitObject = hit[i].collider.transform.gameObject;

                        if (playerState != GlobPlayerAction.ePlayerState_Building) {
                            if (ourHitObject.tag == "Tavern")
                            {
                                if (playerState == GlobPlayerAction.ePlayerState_Normal) {
                                    if (ourHitObject.GetComponent<Structure>().nPlayer != GameManager.Instance.getTurn()) {
                                        return;
                                    }
                                    UIManager.Instance.openTavernCanvas();
                                    playerState = GlobPlayerAction.ePlayerState_Normal;
                                }
                            }
                            else if (ourHitObject.tag == "kingdomIcon")
                            {
                                if (ourHitObject.GetComponent<Structure>().nPlayer != GameManager.Instance.getTurn())
                                {
                                    return;
                                }
                                if (playerState == GlobPlayerAction.ePlayerState_Normal) {
                                    UIManager.Instance.openKingdomCanvas();
                                }
                            }
                            else if (ourHitObject.GetComponent<MovableUnit>() != null)
                            {
                                selectHero(ourHitObject);

                            }
                            else if (ourHitObject.GetComponent<TileScript>() != null) {
                                clickHex(ourHitObject);
                            }
                        }
                    }
                }
            }
        }
    }

    void selectHero(GameObject pSelectedHero) {
        if (playerState == GlobPlayerAction.ePlayerState_Normal) {
            if (pSelectedHero.GetComponent<HeroEntity>().nAlign != GameManager.Instance.getTurn()) {
                return;
            }
            UIManager.Instance.setHeroInformation(pSelectedHero);
            string unitName = pSelectedHero.name;
            if (selectedUnitObj != null && selectedUnitObj.name != unitName) {
                selectedUnitObj.GetComponent<MovableUnit>().clearWalkableTileList();
                selectedUnitObj.GetComponent<MovableUnit>().thisUnitHasBeenClicked = false;
                selectedUnitObj.GetComponent<HeroEntity>().setAnime(false);
            }
            thisHeroScript = pSelectedHero.GetComponent<HeroEntity>();
            thisHeroMovingScript = pSelectedHero.GetComponent<MovableUnit>();
            thisHeroScript.setAnime(true);
            thisHeroMovingScript.thisUnitHasBeenClicked = true;
            selectedUnitObj = pSelectedHero;

            walkableTileIndex();
        } else if (playerState == GlobPlayerAction.ePlayerState_DoAction) {
            HeroEntity targetHeroScript = pSelectedHero.GetComponent<HeroEntity>();
            int nType = thisHeroScript.nEntityType;
            if (HeroSkill.Instance.aimedTargetList.Count >= 2) {
                return;
            }

            if (nType == GlobalHeroIndex.eEntityType_LordOfTime || nType == GlobalHeroIndex.eEntityType_Mole 
                || nType == GlobalHeroIndex.eEntityType_SandShaper || nType == GlobalHeroIndex.eEntityType_Paladin) {
                return;
            }

            HeroSkill.Instance.aimedTargetList.Add(pSelectedHero);
            HeroSkill.Instance.currChooseTargetNum++;
        }
    }

    void clickHex(GameObject pTile) {
        if (selectedUnitObj == null){ 
            return;
        }

        if (playerState == GlobPlayerAction.ePlayerState_Normal){
            HeroEntity.Heroes m_pHero = thisHeroScript.m_pHero;
            TileScript thisTileScript = pTile.GetComponent<TileScript>();
            if (!CheckIfHasSoldierInGrid() && m_pHero.getCurrentMoveStep() > 0) {
                if (thisHeroMovingScript.walkableTiles.Contains(pTile)) {
                    Vector2Int preTileIndex = new Vector2Int(thisHeroMovingScript.indexX, thisHeroMovingScript.indexY);
                    GameManager.Instance.modifyTileHasUnit(preTileIndex);
                    thisHeroMovingScript.destination = pTile.transform.position;
                    thisHeroMovingScript.destinationXIndex = thisTileScript.x;
                    thisHeroMovingScript.destinationYIndex = thisTileScript.y;
                    thisHeroMovingScript.indexX = thisTileScript.x;
                    thisHeroMovingScript.indexY = thisTileScript.y;
                    Vector2Int tileIndex = new Vector2Int(thisTileScript.x, thisTileScript.y);
                    GameManager.Instance.setTileHasUnit(tileIndex, pTile);
                }
            }
        }

        if (playerState == GlobPlayerAction.ePlayerState_DoAction) {
            int nType = thisHeroScript.nEntityType;
            if (nType == GlobalHeroIndex.eEntityType_LordOfTime || nType == GlobalHeroIndex.eEntityType_Mole
                || nType == GlobalHeroIndex.eEntityType_SandShaper || nType == GlobalHeroIndex.eEntityType_Paladin) {
                HeroSkill.Instance.aimedTargetList.Add(pTile);
                HeroSkill.Instance.currChooseTargetNum++;
            }
        }
    }

    public void resetSelectedHeroObj() {
        selectedUnitObj = null;
        for (int i = 0; i < thisHeroMovingScript.walkableTiles.Count; i++)
        {
            thisHeroMovingScript.walkableTiles[i].GetComponent<TileScript>().setColorAnime(GlobTileColorAnimeIndex.eTileColor_null);
        }
        thisHeroMovingScript.walkableTiles.Clear();
        thisHeroMovingScript = null;
        thisHeroScript = null;
    }

    void clickOnUI() {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<HeroCard>() != null)
            {
                pickCard(raycastResultList[i]);
            }
            if (raycastResultList[i].gameObject.GetComponent<StructureCard>() != null) {
                pickStructure(raycastResultList[i]);
            }
            if (raycastResultList[i].gameObject.tag == "actionButton") {
                clickActionButton(raycastResultList[i]);
            }
        }
    }

    public void clickActionButton(RaycastResult thisRaycastResult) {
        if (thisHeroScript == null) {
            return;
        }
        HeroSkill.Instance.doAction();
        playerState = GlobPlayerAction.ePlayerState_DoAction;
        thisHeroMovingScript.clearWalkableTileList();
    }

    void pickCard(RaycastResult thisRaycastResult) {
        if (currPlayer.getCurrHeroNum() >= currPlayer.getMaxHeroNum()) {
            return;
        }
        GameObject thisHeroCard = thisRaycastResult.gameObject;
        HeroCard thisHeroCardScript = thisHeroCard.GetComponent<HeroCard>();
        thisHeroCardScript.setUnactive();
        thisHeroCardScript.spawnHero();
    }

    void enterKingdom() {
        UIManager.Instance.openKingdomCanvas();
        playerState = GlobPlayerAction.ePlayerState_Building;
    }

    void pickStructure(RaycastResult thisRaycastResult) {
        if (currPlayer.getMaxStructureNum() <= currPlayer.getCurrStructureNum()) {
            return;
        }
        GameObject thisBuildingCard = thisRaycastResult.gameObject;
        StructureCard thisStructureCardScript = thisBuildingCard.GetComponent<StructureCard>();
        playerState = GlobPlayerAction.ePlayerState_Building;
        Vector3 mousePixelCoords = Input.mousePosition;
        Vector3 mouseWorldCoords = Camera.main.ScreenToWorldPoint(mousePixelCoords);
        GameObject newBuilding = new GameObject();
        if (thisStructureCardScript.nType == GlobStructureType.eStructure_nTavern) {
            newBuilding = Instantiate(tavernPrefab, mouseWorldCoords,Quaternion.identity);
        }
        if (thisStructureCardScript.nType == GlobStructureType.eStructure_nWorkshop) {
            newBuilding = Instantiate(workshopPrefab,mouseWorldCoords,Quaternion.identity);
        }
        if (thisStructureCardScript.nType == GlobStructureType.eStructure_nFarm)
        {
            newBuilding = Instantiate(farmPrefab,mouseWorldCoords,Quaternion.identity);
        }
        PlaceStructure placeStructureScript = newBuilding.GetComponent<PlaceStructure>();
        placeStructureScript.bMoveWithMouse = true;
        index_GridCanBuildStructure(placeStructureScript);
        UIManager.Instance.exitKingdomCanvas();

        currPlayer.modifyCurrStrutureNum(1);
    }

    void index_GridCanBuildStructure(PlaceStructure thisStructureScript) {
        GameObject kingdomObj = kingdomIcon;
        Structure kingdomScript = kingdomObj.GetComponent<Structure>();
        int a = kingdomScript.x;
        int b = kingdomScript.y;
        if (b % 2 == 1)
        {
            thisStructureScript.showPlaceableGrid(a + 1, b);
            thisStructureScript.showPlaceableGrid(a, b + 1);
            thisStructureScript.showPlaceableGrid(a + 1, b + 1);
            thisStructureScript.showPlaceableGrid(a + 1, b - 1);
            thisStructureScript.showPlaceableGrid(a, b - 1);
            thisStructureScript.showPlaceableGrid(a - 1, b);

        }
        else
        {
            thisStructureScript.showPlaceableGrid(a, b + 1);
            thisStructureScript.showPlaceableGrid(a + 1, b);
            thisStructureScript.showPlaceableGrid(a - 1, b + 1);
            thisStructureScript.showPlaceableGrid(a - 1, b);
            thisStructureScript.showPlaceableGrid(a - 1, b - 1);
            thisStructureScript.showPlaceableGrid(a, b - 1);
        }
    }

    void walkableTileIndex() {
        int a = selectedUnitObj.GetComponent<MovableUnit>().indexX;
        int b = selectedUnitObj.GetComponent<MovableUnit>().indexY;
        if (b % 2 == 1)
        {
            showWalkableTile(a + 1, b);
            showWalkableTile(a, b + 1);
            showWalkableTile(a + 1, b + 1);
            showWalkableTile(a + 1, b - 1);
            showWalkableTile(a, b - 1);
            showWalkableTile(a - 1, b);

        }
        else
        {
            showWalkableTile(a, b + 1);
            showWalkableTile(a + 1, b);
            showWalkableTile(a - 1, b + 1);
            showWalkableTile(a - 1, b);
            showWalkableTile(a - 1, b - 1);
            showWalkableTile(a, b - 1);
        }
    }

    void showWalkableTile(int x, int y) {
        if (x < 0 || x >= GameManager.Instance.width || y < 0 || y >= GameManager.Instance.height) {
            return;
        }
        GameObject terrain = GameManager.Instance.getTileObjectByIndex("xIndex_" + x.ToString() + "yIndex_" + y.ToString());
        if (terrain.GetComponent<TileScript>().terrainType != GlobTileType.eTile_nObstacle 
            && terrain.GetComponent<TileScript>().terrainType != GlobTileType.eTile_nSea) {
            thisHeroMovingScript.walkableTiles.Add(terrain);
            terrain.GetComponent<TileScript>().setColorAnime(GlobTileColorAnimeIndex.eTileColor_walkable);
        }
    }



    public bool CheckIfHasSoldierInGrid()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit != null)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider != null)
                {
                    GameObject pObj = hit[i].collider.transform.gameObject;
                    if (pObj.GetComponent<HeroEntity>() != null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
