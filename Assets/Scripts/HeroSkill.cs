using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class HeroSkill : MonoBehaviour
{
    public static HeroSkill Instance;
    public GameObject kingdomIconPrefab;
    public GameObject mouseController;

    public GameObject selectedHero;
    //public GameObject aimedHero;
    //public GameObject aimedHero2;
    //public GameObject aimedTile;

    public List<GameObject> aimedTargetList;

    public GameObject goblinMinePrefab;
    public GameObject temporaryTilePrefab;
    public GameObject paladinEffectPrefab;
    public GameObject arrowPrefab;
    public GameObject pointAtKingdomPrefab;

    //int targetNum;
    public int currChooseTargetNum = 0;
    public bool bNeedChooseTarget = false;

    private bool bDoneSkill = false;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            return;
        }

        resetHeroSkill();
    }

    // Update is called once per frame
    void Update()
    {
        selectedHero = mouseController.GetComponent<MouseController>().selectedUnitObj;
        if (bNeedChooseTarget) {
            int heroIndex = selectedHero.GetComponent<HeroEntity>().nEntityType;
            int nNeedTargetNum = HeroManager.Instance.getHeroDataDic(heroIndex).m_nTargetNum;
            int nSkillNeedStep = HeroManager.Instance.getHeroDataDic(heroIndex).m_nSkillCostMove;
            int currMoveStep = selectedHero.GetComponent<HeroEntity>().m_pHero.getCurrentMoveStep();
            if (nSkillNeedStep > currMoveStep) {
                return;
            }

            
            if (selectedHero.GetComponent<HeroEntity>().nEntityType == GlobalHeroIndex.eEntityType_Paladin)
            {
                if (GameObject.Find(selectedHero.name + "paladinEffect") == null) {
                    GameObject paladinEffect = Instantiate(paladinEffectPrefab, selectedHero.transform.position, Quaternion.identity);
                    paladinEffect.GetComponent<Animator>().SetInteger("currentMove", currMoveStep);
                    paladinEffect.name = selectedHero.name + "paladinEffect";

                }
            }

            if (GameObject.Find(selectedHero.name + "arrow") == null) {
                if (HeroManager.Instance.getHeroDataDic(heroIndex).m_bNeedArrow == 0)
                {
                    GameObject actionArrow = Instantiate(arrowPrefab,selectedHero.transform.position,Quaternion.identity);
                    actionArrow.name = selectedHero.name + "arrow";
                    Color32 arrowColor = new Color32(UIManager.Instance.arrowColorLst[HeroManager.Instance.getHeroDataDic(heroIndex).m_nArrowColorIndex].r, 
                        UIManager.Instance.arrowColorLst[HeroManager.Instance.getHeroDataDic(heroIndex).m_nArrowColorIndex].g,
                        UIManager.Instance.arrowColorLst[HeroManager.Instance.getHeroDataDic(heroIndex).m_nArrowColorIndex].b,255);
                    //actionArrow.GetComponent<SpriteRenderer>().color = UIManager.Instance.arrowColorLst[HeroManager.Instance.getHeroDataDic(heroIndex).m_nArrowColorIndex];
                    actionArrow.GetComponent<SpriteRenderer>().color = arrowColor;
                }
            }

            if (currChooseTargetNum < nNeedTargetNum)
            {
                return;
            }

            releaseHeroSkill(heroIndex);

            if (GameObject.Find(selectedHero.name + "arrow") != null)
            {
                Destroy(GameObject.Find(selectedHero.name + "arrow"));
            }

            
            if (bDoneSkill) {
                selectedHero.GetComponent<HeroEntity>().m_pHero.modifyCurrentMoveStep(-nSkillNeedStep);
            }
            resetHeroSkillState();
        }
    }

    public void resetHeroSkillState() {
        bNeedChooseTarget = false;
        currChooseTargetNum = 0;
        mouseController.GetComponent<MouseController>().playerState = GlobPlayerAction.ePlayerState_Normal;
        aimedTargetList.Clear();
        bDoneSkill = false;
    }

    public void doAction() {
        bNeedChooseTarget = true;
        selectedHero = mouseController.GetComponent<MouseController>().selectedUnitObj;
    }

    public void releaseHeroSkill(int heroIndex) {
        switch (heroIndex) {
            case GlobalHeroIndex.eEntityType_GoblinTechies:
                skill_GoblinTechies();
                break;
            case GlobalHeroIndex.eEntityType_Paladin:
                skill_Paladin();
                break;
            case GlobalHeroIndex.eEntityType_WitchDoctor:
                skill_WitchDoctor();
                break;
            case GlobalHeroIndex.eEntityType_Bard:
                skill_Bard();
                break;
            case GlobalHeroIndex.eEntityType_Snaker:
                skill_Snaker();
                break;
            case GlobalHeroIndex.eEntityType_Snow:
                skill_Snow();
                break;
            case GlobalHeroIndex.eEntityType_SandShaper:
                skill_SandShaper();
                break;
            case GlobalHeroIndex.eEntityType_Mole:
                skill_Mole();
                break;
            case GlobalHeroIndex.eEntityType_Monk:
                skill_Monk();
                break;
            case GlobalHeroIndex.eEntityType_Flame:
                skill_Flame();
                break;
            case GlobalHeroIndex.eEntityType_Lich:
                skill_Lich();
                break;
            case GlobalHeroIndex.eEntityType_Silencer:
                skill_Silencer();
                break;
            case GlobalHeroIndex.eEntityType_Berserker:
                skill_Berserker();
                break;
            case GlobalHeroIndex.eEntityType_ElfArcher:
                skill_ElfArcher();
                break;
            case GlobalHeroIndex.eEntityType_Purifier:
                skill_Purifier();
                break;
            case GlobalHeroIndex.eEntityType_Cleric:
                skill_Cleric();
                break;
            case GlobalHeroIndex.eEntityType_LandGuardian:
                skill_LandGuardian();
                break;
            case GlobalHeroIndex.eEntityType_Rogue:
                skill_Rogue();
                break;
            case GlobalHeroIndex.eEntityType_DeathAlchemist:
                skill_DeathAlchemist();
                break;
            case GlobalHeroIndex.eEntityType_LordOfTime:
                skill_LordOfTime();
                break;
            case GlobalHeroIndex.eEntityType_MasterOfCircus:
                skill_MasterOfCircus();
                break;
            case GlobalHeroIndex.eEntityType_King:
                skill_King();
                break;
        }
    }

    void skill_GoblinTechies() {
        HeroEntity selectedHeroEntityScript = selectedHero.GetComponent<HeroEntity>();
        MovableUnit selectedHeroMovingScript = selectedHero.GetComponent<MovableUnit>();
        Vector2 pos = selectedHero.transform.position;
        GameObject newGoblinMine = Instantiate(goblinMinePrefab,pos,Quaternion.identity);
        GoblinMine thisNewGoblinMineScript = newGoblinMine.GetComponent<GoblinMine>();
        thisNewGoblinMineScript.x = selectedHeroMovingScript.indexX;
        thisNewGoblinMineScript.y = selectedHeroMovingScript.indexY;
        thisNewGoblinMineScript.ally = selectedHeroEntityScript.nAlign;
        selectedHeroEntityScript.addGoblinMineToList(newGoblinMine);
        thisNewGoblinMineScript.whenAwake();
        bDoneSkill = true;
    }

    void skill_Paladin() {
        GameObject selectedPaladin = mouseController.GetComponent<MouseController>().selectedUnitObj;
        GameObject pTile = aimedTargetList[0];
        MovableUnit paladinMovingScript = selectedPaladin.GetComponent<MovableUnit>();

        int x = paladinMovingScript.indexX;
        int y = paladinMovingScript.indexY;

        int tileX = pTile.GetComponent<TileScript>().x;
        int tileY = pTile.GetComponent<TileScript>().y;
        Dictionary<string, GameObject> tileByIndexDic = GameManager.Instance.getTileObjByIndexDic();
        GameObject heroTile = tileByIndexDic["xIndex_" + x.ToString() + "yIndex_"+y.ToString()];
        Dictionary<Vector2Int, GameObject> unitInTileDic = GameManager.Instance.getUnitInTileDic();

        if (GameManager.Instance.bTileHasUnit(new Vector2Int(tileX,tileY)))
        {
            bDoneSkill = false;
            return;
        }

        Vector2 dir1;
        Vector2 dir2;
        Vector2 dir3;
        Vector2 dir4;
        Vector2 dir5;
        Vector2 dir6;
        if (y % 2 == 1)
        {

           dir1 = getDirection(selectedPaladin,1,0);
           dir2 = getDirection(selectedPaladin,1, -1);
           dir3 = getDirection(selectedPaladin,0, -1);
           dir4 = getDirection(selectedPaladin,-1, 0);
           dir5 = getDirection(selectedPaladin,0, 1);
           dir6 = getDirection(selectedPaladin, 1, 1);
;
        }
        else
        {
           
            dir1 = getDirection(selectedPaladin,1, 0);
            dir2 = getDirection(selectedPaladin,0, -1);
            dir3 = getDirection(selectedPaladin,-1, -1);
            dir4 = getDirection(selectedPaladin,-1, 0);
            dir5 = getDirection(selectedPaladin,-1, 1);
            dir6 = getDirection(selectedPaladin, 0, 1);
        }
        List<Vector2>dirList = new List<Vector2> { dir1,dir2,dir3,dir4,dir5,dir6};
        Vector2 myHeroTilePos = heroTile.transform.position;
        Vector2 getDir = new Vector2(pTile.transform.position.x - heroTile.transform.position.x,
                pTile.transform.position.y - heroTile.transform.position.y).normalized;
        float fDist = Vector2.Distance(pTile.transform.position,heroTile.transform.position);
        int getStepNum = selectedPaladin.GetComponent<HeroEntity>().m_pHero.getCurrentMoveStep();
        float fOffset = 0.05f;
        float fScale = 1.375f;
        bool bCrash = false;
        bool bMoving = false;
        for (int i = 0; i < dirList.Count; i++)
        {
            if (dirList[i] == null)
            {
                continue;
            }
            if (getDir != dirList[i])
            {
                continue;
            }
            if (getDir == dirList[i])
            {
                if (fDist <= getStepNum * fScale + fOffset) {
                    bMoving = true;
                    for (int a = 0; a < tileByIndexDic.Count; a++)
                    {
                        GameObject thisTile = tileByIndexDic.ElementAt(a).Value;
                        Vector2 tilePos = thisTile.transform.position;
                        Vector2 dir_ = new Vector2(tilePos.x - myHeroTilePos.x, tilePos.y - myHeroTilePos.y).normalized;
                        float fDist_ = Vector2.Distance(myHeroTilePos, tilePos);
                        if (dir_ == getDir)
                        {
                            float fOffset_ = 0.1f;
                            if (fDist_ < fScale * getStepNum - fOffset_)
                            {
                                int thisTileX = thisTile.GetComponent<TileScript>().x;
                                int thisTileY = thisTile.GetComponent<TileScript>().y;
                                Vector2Int thisTileIndex = new Vector2Int(thisTileX,thisTileY);
                                for (int b = 0; b < unitInTileDic.Count; b++)
                                {
                                    if (unitInTileDic.ElementAt(b).Key == thisTileIndex && Vector2.Distance(myHeroTilePos, tilePos) < fDist)
                                    {
                                        HeroEntity passUnit = unitInTileDic[thisTileIndex].GetComponent<HeroEntity>();
                                        if (passUnit.nAlign != selectedPaladin.GetComponent<HeroEntity>().nAlign)
                                        {
                                            attack(selectedPaladin, unitInTileDic[thisTileIndex]);
                                        }
                                    }
                                }
                                if ((thisTile.GetComponent<TileScript>().terrainType == GlobTileType.eTile_nObstacle
                                    || thisTile.GetComponent<TileScript>().terrainType == GlobTileType.eTile_nSea) && Vector2.Distance(myHeroTilePos, tilePos) < fDist)
                                {
                                    bCrash = true;
                                    Debug.Log("you can't aim the hero behind obstacle!");
                                    Debug.Log("obstacle: x_ " + thisTile.GetComponent<TileScript>().x + " y_" + thisTile.GetComponent<TileScript>().y);
                                    Debug.Log("fDist_"+fDist + " HeroTilePos" + myHeroTilePos + " thisTilePos_"+tilePos);
                                    //bMoving = false;
                                }
                            }
                        }
                    }

                }
            }
        }
        if (bCrash)
        {
            Debug.Log("crash to obstacle!");
            return;
        }

        if (bMoving == false)
        {
            Debug.Log("bMoving = false!");
            return;
        }

        GameManager.Instance.modifyTileHasUnit(new Vector2Int(x,y));
        GameManager.Instance.modifyUnitInTile(new Vector2Int(x, y));

        GameManager.Instance.setTileHasUnit(new Vector2Int(tileX,tileY),pTile);
        GameManager.Instance.setUnitInTile(new Vector2Int(tileX, tileY),selectedPaladin);
        paladinMovingScript.destination = pTile.transform.position;
        paladinMovingScript.destinationXIndex = tileX;
        paladinMovingScript.destinationYIndex = tileY;
        paladinMovingScript.indexX = tileX;
        paladinMovingScript.indexY = tileY;

        if (GameObject.Find(selectedHero.name + "paladinEffect") != null)
        {
            Destroy(GameObject.Find(selectedHero.name + "paladinEffect"));
        }

        bDoneSkill = true;
    }

    Vector2 getDirection(GameObject selectedHero, int offsetX, int offsetY) {
        Vector2 heroPos = selectedHero.transform.position;
        int x = selectedHero.GetComponent<MovableUnit>().indexX;
        int y = selectedHero.GetComponent<MovableUnit>().indexY;
        int newX = x + offsetX;
        int newY = y + offsetY;
        Vector2 dir;
        if (newX < 0 || newX >= GameManager.Instance.width || newY < 0 || newY >= GameManager.Instance.height)
        {
            dir = new Vector2(999, 998);
        }
        else
        {
            Vector2 tilePos = GameManager.Instance.getTileObjectByIndex("xIndex_" + newX.ToString() + "yIndex_" + newY.ToString()).transform.position;
            dir = new Vector2(tilePos.x - heroPos.x, tilePos.y - heroPos.y).normalized;
        }
        return dir;
    }

    void skill_WitchDoctor() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign != selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (aimedObjWithinRange(selectedHero, aimedTargetList[0], 0) == false)
        {
            bDoneSkill = false;
            return;
        }
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pDoubleAllBuff);
        bDoneSkill = true;
    }

    void skill_Bard() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign != selectedHero.GetComponent<HeroEntity>().nAlign) {
            bDoneSkill = false;
            return;
        }
        if (aimedObjWithinRange(selectedHero, aimedTargetList[0], 0) == false)
        {
            bDoneSkill = false;
            return;
        }
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pDamageBuff);
        bDoneSkill = true;
    }

    void skill_Snaker() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (aimedObjWithinRange(selectedHero, aimedTargetList[0], 0) == false)
        {
            bDoneSkill = false;
            return;
        }
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pPoisonDebuff);
        bDoneSkill = true;
    }

    void skill_Snow() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pStunnedDebuff);
        bDoneSkill = true;
    }

    void skill_SandShaper() {
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 1))
        {
            bDoneSkill = false;
            return;
        }
        TileScript aimedTileScipt = aimedTargetList[0].GetComponent<TileScript>();
        int x = aimedTileScipt.x;
        int y = aimedTileScipt.y;
        if (GameManager.Instance.bTileHasUnit(new Vector2Int(x, y))) {
            bDoneSkill = false;
            return;
        }
        GameObject newTile = Instantiate(temporaryTilePrefab, aimedTargetList[0].transform.position, Quaternion.identity);
        newTile.GetComponent<SpriteRenderer>().color = Color.red;
        TemporaryTile newTileScript = newTile.GetComponent<TemporaryTile>();
        newTileScript.thisTileAwake(x,y, GlobTileType.eTile_nObstacle);
        bDoneSkill = true;
    }

    void skill_Mole() {
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 1))
        {
            bDoneSkill = false;
            return;
        }
        TileScript aimedTileScipt = aimedTargetList[0].GetComponent<TileScript>();
        int x = aimedTileScipt.x;
        int y = aimedTileScipt.y;
        GameObject newTile = Instantiate(temporaryTilePrefab, aimedTargetList[0].transform.position, Quaternion.identity);
        newTile.GetComponent<SpriteRenderer>().color = Color.yellow;
        TemporaryTile newTileScript = newTile.GetComponent<TemporaryTile>();
        newTileScript.thisTileAwake(x, y, GlobTileType.eTile_nWalkable);
        bDoneSkill = true;
    }

    void skill_Monk() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        HeroEntity.Heroes enemyHeroScript = aimedTargetList[0].GetComponent<HeroEntity>().m_pHero;
        enemyHeroScript.addStatus(selectedHero.name,HeroManager.Instance.pVulnerabilityByMonk);
        enemyHeroScript.addStatus(selectedHero.name,HeroManager.Instance.pWeaknessDebuff);
        bDoneSkill = true;
    }

    void skill_Flame() {
        aoeAttack(1, selectedHero);
        bDoneSkill = true;
    }

    void skill_Lich() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        HeroEntity.Heroes enemyHeroScript = aimedTargetList[0].GetComponent<HeroEntity>().m_pHero;
        enemyHeroScript.addStatus(selectedHero.name,HeroManager.Instance.pDoubleDebuff);
        bDoneSkill = true;
    }

    void skill_Silencer() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pSilenceDebuff);
        bDoneSkill = true;
    }

    void skill_Berserker() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        attack(selectedHero, aimedTargetList[0]);
        if (aimedTargetList[0] == null) {
            return;
        }
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pVulnerabilityByBerserker);
        bDoneSkill = true;
    }

    void skill_ElfArcher() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        attack(selectedHero, aimedTargetList[0]);
        bDoneSkill = true;
    }

    void skill_Cleric() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign != selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        HeroEntity selectedHeroScript = selectedHero.GetComponent<HeroEntity>();
        HeroEntity aimedHeroScript = aimedTargetList[0].GetComponent<HeroEntity>();
        if (selectedHeroScript.nAlign == aimedHeroScript.nAlign) {
            int healingValue = selectedHeroScript.m_pHero.getBasicAttack();
            aimedHeroScript.m_pHero.modifyHPByHealing(healingValue);
            bDoneSkill = true;
        }
    }

    void skill_Purifier() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign != selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pPurifier);
        bDoneSkill = true;
    }

    void skill_LandGuardian() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign != selectedHero.GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pShield);
        bDoneSkill = true;

    }

    void skill_Rogue() {
        HeroEntity aimedTarget1Script = aimedTargetList[0].GetComponent<HeroEntity>();
        HeroEntity aimedTarget2Script = aimedTargetList[1].GetComponent<HeroEntity>();
        HeroEntity selectedHeroScript = selectedHero.GetComponent<HeroEntity>();
        if (aimedTarget1Script.nAlign == aimedTarget2Script.nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[1], 0))
        {
            bDoneSkill = false;
            return;
        }
        if (aimedTarget1Script.nAlign == selectedHeroScript.nAlign) {
            aimedTarget2Script.m_pHero.stealStatus(selectedHero.name);
        } else if (aimedTarget2Script.nAlign == selectedHeroScript.nAlign) {
            aimedTarget1Script.m_pHero.stealStatus(selectedHero.name);
        }
        
    }

    void skill_DeathAlchemist() {
        HeroEntity aimedTarget1Script = aimedTargetList[0].GetComponent<HeroEntity>();
        HeroEntity selectedHeroScript = selectedHero.GetComponent<HeroEntity>();

        if (aimedTarget1Script.nAlign != selectedHeroScript.nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        aimedTarget1Script.m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pSacrifice);
        bDoneSkill = true;
    }

    void skill_LordOfTime() {
        HeroEntity selectedHeroScript = selectedHero.GetComponent<HeroEntity>();
        selectedHeroScript.m_pHero.addStatus(selectedHero.name, HeroManager.Instance.pBackToPos);
        bDoneSkill = true;
    }

    void skill_MasterOfCircus() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == aimedTargetList[1].GetComponent<HeroEntity>().nAlign)
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[0], 0))
        {
            bDoneSkill = false;
            return;
        }
        if (!aimedObjWithinRange(selectedHero, aimedTargetList[1], 0))
        {
            bDoneSkill = false;
            return;
        }
        MovableUnit aimedHero1MovingScript = aimedTargetList[0].GetComponent<MovableUnit>();
        MovableUnit aimedHero2MovingScript = aimedTargetList[1].GetComponent<MovableUnit>();

        Vector2 aimedHero1Pos = aimedTargetList[0].transform.position;
        Vector2 aimedHero2Pos = aimedTargetList[1].transform.position;

        int x1 = aimedHero1MovingScript.indexX;
        int y1 = aimedHero1MovingScript.indexY;

        int x2 = aimedHero2MovingScript.indexX;
        int y2 = aimedHero2MovingScript.indexY;

        Vector2Int aimedHero1Index = new Vector2Int(x1,y1);
        Vector2Int aimedHero2Index = new Vector2Int(x2,y2);

        aimedTargetList[0].transform.position = aimedHero2Pos;
        aimedTargetList[1].transform.position = aimedHero1Pos;

        aimedHero1MovingScript.destination = aimedHero2Pos;
        aimedHero2MovingScript.destination = aimedHero1Pos;

        aimedHero1MovingScript.indexX = x2;
        aimedHero2MovingScript.indexX = x1;
        aimedHero1MovingScript.indexY = y2;
        aimedHero2MovingScript.indexY = y1;

        aimedHero1MovingScript.destinationXIndex = x2;
        aimedHero2MovingScript.destinationXIndex = x1;

        aimedHero1MovingScript.destinationYIndex = y2;
        aimedHero2MovingScript.destinationYIndex = y1;

        GameManager.Instance.setUnitInTile(aimedHero1Index, aimedTargetList[1]);
        GameManager.Instance.setUnitInTile(aimedHero2Index, aimedTargetList[0]);
        
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.setPos(new Vector2Int(x2, y2));
        aimedTargetList[1].GetComponent<HeroEntity>().m_pHero.setPos(new Vector2Int(x1, y1));

        bDoneSkill = true;
    }

    void skill_King() {
        GameObject selectedKing = mouseController.GetComponent<MouseController>().selectedUnitObj;
        int indexX = selectedKing.GetComponent<MovableUnit>().indexX;
        int indexY = selectedKing.GetComponent<MovableUnit>().indexY;
        Vector2 pos = selectedKing.transform.position;
        MovableUnit kingMovingScript = selectedKing.GetComponent<MovableUnit>();
        for (int i = 0; i < kingMovingScript.walkableTiles.Count; i++) {
            kingMovingScript.walkableTiles[i].GetComponent<TileScript>().setColorAnime(GlobTileColorAnimeIndex.eTileColor_null);
        }
        Destroy(selectedKing);
        GameObject newKingdom = Instantiate(kingdomIconPrefab,pos,Quaternion.identity);
        newKingdom.name = "kingdomIcon" + GameManager.Instance.getTurn().ToString();
        GameObject pointAtKingdom = Instantiate(pointAtKingdomPrefab);
        pointAtKingdom.transform.position = new Vector2(pos.x, pos.y + 1);
        newKingdom.GetComponent<Structure>().x = indexX;
        newKingdom.GetComponent<Structure>().y = indexY;
        mouseController.GetComponent<MouseController>().resetSelectedHeroObj();
        mouseController.GetComponent<MouseController>().kingdomIcon = newKingdom;

        GameManager.Instance.currPlayer.setHasKingdom(true);
        GameManager.Instance.currPlayer.setKingdomIndex(new Vector2Int(indexX,indexY));
        bDoneSkill = true;
    }



    void resetHeroSkill() {
        Instance = this;
    }

    public void attack(GameObject myHero, GameObject enemyHero) {
        HeroEntity.Heroes my_pHero = myHero.GetComponent<HeroEntity>().m_pHero;
        HeroEntity.Heroes enemy_pHero = enemyHero.GetComponent<HeroEntity>().m_pHero;
/*
        if (myHero.GetComponent<HeroEntity>().nAlign == enemyHero.GetComponent<HeroEntity>().nAlign)
        {
            return;
        }
*/
        int attackValue = my_pHero.getAttack();
        int defenseValue = enemy_pHero.getDefense();

        if (defenseValue > 0) {
            if (attackValue > defenseValue)
            {
                attackValue -= defenseValue;
                enemy_pHero.setDefense(0);
            }
            else {
                attackValue = 0;
                enemy_pHero.modifyDefense(-attackValue);
            }
        }
        enemy_pHero.modifyHP(-attackValue);
    }

    bool aimedObjWithinRange(GameObject myHero,GameObject aimedObj, int aimObjType) {
        int aimObjType_hero = 0;
        int aimObjType_grid = 1;

        bool withinRange = false;
        int range = HeroManager.Instance.getHeroDataDic(myHero.GetComponent<HeroEntity>().nEntityType).m_nAttackDistance;
        float fScale = 1.375f;
        float fOffset = 0.08f;
        int x1 = myHero.GetComponent<MovableUnit>().indexX;
        int y1 = myHero.GetComponent<MovableUnit>().indexY;

        int x2;
        int y2;
        Vector2 aimedHeroTilePos;
        if (aimObjType == aimObjType_hero) {
            x2 = aimedObj.GetComponent<MovableUnit>().indexX;
            y2 = aimedObj.GetComponent<MovableUnit>().indexY;
            aimedHeroTilePos = GameManager.Instance.getTileObjectByIndex("xIndex_" + x2.ToString() + "yIndex_" + y2.ToString()).transform.position;
        }
        else
        {
            aimedHeroTilePos = aimedObj.transform.position;
        }
        Vector2 myHeroTilePos = GameManager.Instance.getTileObjectByIndex("xIndex_"+x1.ToString()+"yIndex_"+y1.ToString()).transform.position;

        Dictionary<string, GameObject> tileByIndexDic = GameManager.Instance.getTileObjByIndexDic();

        float fDist = Vector2.Distance(myHeroTilePos,aimedHeroTilePos);
        Vector2 dir = new Vector2(aimedHeroTilePos.x - myHeroTilePos.x,aimedHeroTilePos.y - myHeroTilePos.y).normalized;

        if (fDist <= fScale * range + fOffset)
        {
            withinRange = true;
            for (int i = 0; i < tileByIndexDic.Count; i++)
            {
                GameObject thisTile = tileByIndexDic.ElementAt(i).Value;
                Vector2 tilePos = thisTile.transform.position;
                Vector2 dir_ = new Vector2(tilePos.x-myHeroTilePos.x,tilePos.y-myHeroTilePos.y).normalized;
                float fDist_ = Vector2.Distance(myHeroTilePos,tilePos);
                if (dir_ == dir)
                {
                    float fOffset_ = 0.05f;
                    if (fDist_ < fScale * range - fOffset_) {
                        if (myHero.GetComponent<HeroEntity>().nEntityType != GlobalHeroIndex.eEntityType_ElfArcher) {
                            if (thisTile.GetComponent<TileScript>().terrainType == GlobTileType.eTile_nObstacle
                                || thisTile.GetComponent<TileScript>().terrainType == GlobTileType.eTile_nSea)
                            {
                                withinRange = false;
                                Debug.Log("you can't aim the hero behind obstacle!");
                            }
                        }
                    }
                }
            }

        }

        return withinRange;
    }


    public void aoeAttack(int range, GameObject myHero) {
        int x = myHero.GetComponent<MovableUnit>().indexX;
        int y = myHero.GetComponent<MovableUnit>().indexY;
        Vector2 myHeroPos = myHero.transform.position;
        Dictionary<Vector2Int, GameObject> unitByIndex = GameManager.Instance.getUnitInTileDic();
        for (int i = 0; i < unitByIndex.Count; i++)
        {
            Vector2Int posIndex = unitByIndex.ElementAt(i).Key;
            if (unitByIndex[posIndex] == null)
            {
                continue;
            }
            GameObject thisUnit = unitByIndex[posIndex];
            if (thisUnit.name.Substring(0,6) != "player")
            {
                continue;
            }
            Vector2 aimedUnitPos = thisUnit.transform.position;
            float fDist = Vector2.Distance(myHeroPos,aimedUnitPos);
            int a = posIndex.x;
            int b = posIndex.y;
            if (range == 1)
            {
                if (thisUnit.GetComponent<HeroEntity>().nAlign == myHero.GetComponent<HeroEntity>().nAlign) {
                    continue;
                }

                if (fDist <= 1.38f)
                {
                    if (aimedObjWithinRange(myHero,thisUnit,0)) {
                        attack(myHero, thisUnit);
                        if (myHero.GetComponent<HeroEntity>().nEntityType == GlobalHeroIndex.eEntityType_Flame) {
                            HeroEntity.Heroes enemyHeroScript = thisUnit.GetComponent<HeroEntity>().m_pHero;
                            enemyHeroScript.addStatus(selectedHero.name, HeroManager.Instance.pFlameDebuff);
                        }
                    }
                }
            }
            else if (range == 2)
            {
                if (thisUnit.GetComponent<HeroEntity>().nAlign == myHero.GetComponent<HeroEntity>().nAlign)
                {
                    continue;
                }

                if (fDist <= 2.75f)
                {
                    if (aimedObjWithinRange(myHero, thisUnit, 0))
                    {
                        attack(myHero, thisUnit);
                    }
                }
            }
            else if(range == 3)
            {
                if (fDist <= 4.13f)
                {
                    if (aimedObjWithinRange(myHero, thisUnit, 0))
                    {
                        HeroEntity.Heroes my_pHero = myHero.GetComponent<HeroEntity>().m_pHero;
                        HeroEntity.Heroes aimed_pHero = thisUnit.GetComponent<HeroEntity>().m_pHero;

                        int attackValue = my_pHero.getCurrHP();
                        int defenseValue = aimed_pHero.getDefense();

                        if (defenseValue > 0)
                        {
                            if (attackValue > defenseValue)
                            {
                                attackValue -= defenseValue;
                                aimed_pHero.setDefense(0);
                            }
                            else
                            {
                                attackValue = 0;
                                aimed_pHero.modifyDefense(-attackValue);
                            }
                        }
                        aimed_pHero.modifyHP(-attackValue);
                    }
                }
            }
        }
    }

}
