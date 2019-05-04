using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            if (currChooseTargetNum < nNeedTargetNum)
            {
                return;
            }

            releaseHeroSkill(heroIndex);

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
        bDoneSkill = true;
    }

    void skill_Paladin() {
        GameObject selectedPaladin = mouseController.GetComponent<MouseController>().selectedUnitObj;
        GameObject pTile = aimedTargetList[0];

        int x = selectedPaladin.GetComponent<MovableUnit>().indexX;
        int y = selectedPaladin.GetComponent<MovableUnit>().indexY;

        if (y % 2 == 1) {
            Vector2 dir1 = new Vector2(1,0).normalized;
            Vector2 dir2 = new Vector2(0, -1).normalized;
            Vector2 dir3 = new Vector2(-1,-1).normalized;
            Vector2 dir4 = new Vector2(-1,0).normalized;
            Vector2 dir5 = new Vector2(-1,1).normalized;
            Vector2 dir6 = new Vector2(0, 1).normalized;
            //List<Vector2>dirList = new List<Vector2> { dir1,dir2,dir3,dir4,dir5,dir6};
            Vector2 getDir = new Vector2(pTile.GetComponent<TileScript>().x - selectedPaladin.GetComponent<MovableUnit>().indexX,
                    pTile.GetComponent<TileScript>().y - selectedPaladin.GetComponent<MovableUnit>().indexY);
            int getStepNum = selectedPaladin.GetComponent<HeroEntity>().m_pHero.getCurrentMoveStep();
            List<GameObject> passTile = new List<GameObject>();
            bool bCrash = false;

            if (getDir == dir1) {
                int passTileNum = 0;
                for (int i = 0; i < getStepNum; i++)
                {
                    if (bCrash == false) {
                        int a = x + 1;
                        int b = y;
                        GameObject thisTile = GameManager.Instance.getTileObjectByIndex("xIndex_" + a.ToString()+"yIndex_"+b.ToString());
                        if (thisTile.GetComponent<TileScript>().terrainType == GlobTileType.eTile_nSea || thisTile.GetComponent<TileScript>().terrainType == GlobTileType.eTile_nObstacle)
                        {
                            bCrash = true;
                        }
                        else {
                            passTile.Add(thisTile);
                            passTileNum += 1;
                        }
                    }
                }
                
            }
        }
        bDoneSkill = true;
    }                                             

    void skill_WitchDoctor() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign != selectedHero.GetComponent<HeroEntity>().nAlign)
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
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pDamageBuff);
        bDoneSkill = true;
    }

    void skill_Snaker() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
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
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pStunnedDebuff);
        bDoneSkill = true;
    }

    void skill_SandShaper() {
        TileScript aimedTileScipt = aimedTargetList[0].GetComponent<TileScript>();
        int x = aimedTileScipt.x;
        int y = aimedTileScipt.y;
        GameObject newTile = Instantiate(temporaryTilePrefab, aimedTargetList[0].transform.position, Quaternion.identity);
        newTile.GetComponent<SpriteRenderer>().color = Color.red;
        TemporaryTile newTileScript = newTile.GetComponent<TemporaryTile>();
        newTileScript.x = x;
        newTileScript.y = y;
        newTileScript.nNewType = GlobTileType.eTile_nObstacle;
        bDoneSkill = true;
    }

    void skill_Mole() {
        TileScript aimedTileScipt = aimedTargetList[0].GetComponent<TileScript>();
        int x = aimedTileScipt.x;
        int y = aimedTileScipt.y;
        GameObject newTile = Instantiate(temporaryTilePrefab, aimedTargetList[0].transform.position, Quaternion.identity);
        newTile.GetComponent<SpriteRenderer>().color = Color.yellow;
        TemporaryTile newTileScript = newTile.GetComponent<TemporaryTile>();
        newTileScript.x = x;
        newTileScript.y = y;
        newTileScript.nNewType = GlobTileType.eTile_nWalkable;
        bDoneSkill = true;
    }

    void skill_Monk() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
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
        //AOE还没写！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
        //attack(selectedHero, aimedHero);
        //aimedHero.GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pFlameDebuff);
    }

    void skill_Lich() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
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
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pSilenceDebuff);
        bDoneSkill = true;
    }

    void skill_Berserker() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == selectedHero.GetComponent<HeroEntity>().nAlign)
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
        attack(selectedHero, aimedTargetList[0]);
        bDoneSkill = true;
    }

    void skill_Cleric() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign != selectedHero.GetComponent<HeroEntity>().nAlign)
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
        aimedTargetList[0].GetComponent<HeroEntity>().m_pHero.addStatus(selectedHero.name,HeroManager.Instance.pPurifier);
        bDoneSkill = true;
    }

    void skill_LandGuardian() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign != selectedHero.GetComponent<HeroEntity>().nAlign)
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
        if (aimedTarget1Script.nAlign == selectedHeroScript.nAlign) {
            aimedTarget2Script.m_pHero.stealStatus(selectedHero.name);
        } else if (aimedTarget2Script.nAlign == selectedHeroScript.nAlign) {
            aimedTarget1Script.m_pHero.stealStatus(selectedHero.name);
        }
        
    }

    void skill_DeathAlchemist() {


    }

    void skill_LordOfTime() {


    }

    void skill_MasterOfCircus() {
        if (aimedTargetList[0].GetComponent<HeroEntity>().nAlign == aimedTargetList[1].GetComponent<HeroEntity>().nAlign)
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

        aimedHero1MovingScript.indexX = x2;
        aimedHero2MovingScript.indexX = x1;
        aimedHero1MovingScript.indexY = y2;
        aimedHero2MovingScript.indexY = y1;

        GameManager.Instance.setUnitInTile(aimedHero1Index, aimedTargetList[1]);
        GameManager.Instance.setUnitInTile(aimedHero2Index, aimedTargetList[0]);

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
        newKingdom.GetComponent<Structure>().x = indexX;
        newKingdom.GetComponent<Structure>().y = indexY;
        mouseController.GetComponent<MouseController>().resetSelectedHeroObj();
        mouseController.GetComponent<MouseController>().kingdomIcon = newKingdom;
        bDoneSkill = true;
    }



    void resetHeroSkill() {
        Instance = this;
    }

    void attack(GameObject myHero, GameObject enemyHero) {
        HeroEntity.Heroes my_pHero = myHero.GetComponent<HeroEntity>().m_pHero;
        HeroEntity.Heroes enemy_pHero = enemyHero.GetComponent<HeroEntity>().m_pHero;

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

}
