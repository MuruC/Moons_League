using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class GlobalState {
    public static int eState_Nomal = 0;
    public static int eState_Move = 1;
    public static int eState_Attack = 2;
    public static int eState_Sell = 0;
}

public static class GlobalHeroStatus
{
    public static int eStatus_Vulnerable = 0;
    public static int eStatus_PowerUp = 1;
    public static int eStatus_Weakness = 2;
    public static int eStatus_Poison = 3;
    public static int eStatus_Burning = 4;
    public static int eStatus_Silence = 5;
    public static int eStatus_Stun = 6;
}

public static class GlobalHeroIndex {  
    public const int eEntityType_GoblinTechies = 0;
    public const int eEntityType_Paladin = 1;
    public const int eEntityType_WitchDoctor = 2;
    public const int eEntityType_Bard = 3;
    public const int eEntityType_Snaker = 4;
    public const int eEntityType_Snow = 5;
    public const int eEntityType_SandShaper = 6;
    public const int eEntityType_Mole = 7;
    public const int eEntityType_Monk = 8;
    public const int eEntityType_Flame = 9;
    public const int eEntityType_Lich = 10;
    public const int eEntityType_Silencer = 11;
    public const int eEntityType_ElfArcher = 12;
    public const int eEntityType_Berserker = 13;
    public const int eEntityType_Cleric = 14;
    public const int eEntityType_Purifier = 15;
    public const int eEntityType_LandGuardian = 16;
    public const int eEntityType_Rogue = 17;
    public const int eEntityType_DeathAlchemist = 18;
    public const int eEntityType_LordOfTime = 19;
    public const int eEntityType_MasterOfCircus = 20;
    public const int eEntityType_Miner = 21;
    public const int eEntityType_King = 22;
}

public struct HeroData
{
    public int m_nID;
    public int m_nHP;
    public int m_nDamage;
    public int m_nMaxMove;
    public string m_strName;
    public int m_nAttackDistance;
    public int m_nSkillCostMove;
    public int m_nMoneyTobuy;
    public int m_nView;
    public int m_nGetMoneyByKillingThisHero;
    public int m_nTotalAmountInGame;
    public string m_strSkillDescription;
    public int m_nTargetNum;
    public string m_strSkillValue;
    public string m_strActionName;
    public int m_bNeedArrow;
    public int m_nArrowColorIndex;

}

public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance;
    private Dictionary<int, HeroData> heroDataDic;
    private Dictionary<int, int> maxHeroNumDic;
    private List<GameObject> player0HeroObjList;
    private List<GameObject> player1HeroObjList;

    private List<HeroData> randomHeroEveryTurn;
    private Dictionary<string, GameObject> heroByName;

    public StatusInfo pDamageBuff;
    public StatusInfo pWeaknessDebuff;
    public StatusInfo pFlameDebuff;
    public StatusInfo pPoisonDebuff;
    public StatusInfo pSilenceDebuff;
    public StatusInfo pStunnedDebuff;
    public StatusInfo pVulnerabilityByMonk;
    public StatusInfo pVulnerabilityByBerserker;
    public StatusInfo pPurifier;
    public StatusInfo pShield;
    public StatusInfo pDoubleDebuff;
    public StatusInfo pDoubleDamageBuff;
    public StatusInfo pDoubleAllBuff;
    public StatusInfo pBackToPos;
    public StatusInfo pSacrifice;

    private List<int> m_lstRandomHeroIndex;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            return;
        }
        //Get the path of the Game data folder
        string m_Path = Application.dataPath;

        //Output the Game data path to the console
        Debug.Log("Path : " + m_Path);

        resetHeroEntity();

        m_lstRandomHeroIndex = new List<int>();

        // 读取数据
        //CsvReader pReader = new CsvReader(@"W:\Github\gameDev_final\HeroData.csv");
        CsvReader pReader = new CsvReader(m_Path + "/StreamingAssets/HeroData.csv");
        while (!pReader.IsEnd())
        {
            HeroData pData = new HeroData();

            pData.m_nID = pReader.GetInt("ID");
            pData.m_strName = pReader.GetString("Name");
            pData.m_nHP = pReader.GetInt("MaxHP");
            pData.m_nAttackDistance = pReader.GetInt("SkillDistance");
            pData.m_nGetMoneyByKillingThisHero = pReader.GetInt("MoneyByKillingEnemy");
            pData.m_nDamage = pReader.GetInt("SkillValue");
            pData.m_nMaxMove = pReader.GetInt("MaxMoveStep");
            pData.m_nMoneyTobuy = pReader.GetInt("MoneyToBuy");
            pData.m_nView = pReader.GetInt("View");
            pData.m_nSkillCostMove = pReader.GetInt("SkillCostMove");
            pData.m_nTotalAmountInGame = pReader.GetInt("TotalAmount");
            pData.m_strSkillDescription = pReader.GetString("SkillDescription");
            pData.m_nTargetNum = pReader.GetInt("TargetNum");
            pData.m_strSkillValue = pReader.GetString("SkillValueString");
            pData.m_strActionName = pReader.GetString("ActionName");
            pData.m_bNeedArrow = pReader.GetInt("NeedArrow");
            pData.m_nArrowColorIndex = pReader.GetInt("ArrowColor");
            //初始化随即列表
            int nTotalCount = pData.m_nTotalAmountInGame;
            for (int i = 0; i < nTotalCount; i++)
            {
                m_lstRandomHeroIndex.Add(pData.m_nID);
            }


            Instance.setHeroDataDic(pData.m_nID, pData);

            pReader.ReadNextLine();
        }
        pReader.Release();


        //
        InitStatusInfo();
    }

    void InitStatusInfo()
    {
        InitDamageBuff();
        InitWeaknessDebuff();
        InitFlameDebuff();
        InitPoisonDebuff();
        InitSlienceDebuff();
        InitStunnedDebuff();
        InitPurifierBuff();
        InitVulnerabilityByMonk();
        InitVulnerabilityByBerserker();
        InitShieldBuff();
        InitDoubleDebuff();
        InitDoubleDamageBuff();
        InitDoubleAllBuff();
        InitBackToPos();
        InitSacrifice();
    }

    void InitDamageBuff() {
        pDamageBuff = new StatusInfo();
        pDamageBuff.strStatusName = "ezhu_paoxiao";
        pDamageBuff.nTotalTurn = 2;
        pDamageBuff.nMaxOverLapCount = 999;
        pDamageBuff.bDebuff = false;
        pDamageBuff.dicChangedAttr = new Dictionary<string, string>();
        pDamageBuff.dicChangedAttr["damage"] = "50|0";
    }

    void InitWeaknessDebuff() {
        pWeaknessDebuff = new StatusInfo();
        pWeaknessDebuff.strStatusName = "ezhu_isHungry";
        pWeaknessDebuff.nTotalTurn = 2;
        pWeaknessDebuff.nMaxOverLapCount = 999;
        pWeaknessDebuff.bDebuff = true;
        pWeaknessDebuff.dicChangedAttr = new Dictionary<string, string>();
        pWeaknessDebuff.dicChangedAttr["damage"] = "-30|-1";
    }

    void InitFlameDebuff() {
        pFlameDebuff = new StatusInfo();
        pFlameDebuff.strStatusName = "giveEnemyBuringDamage";
        pFlameDebuff.nTotalTurn = 3;
        pFlameDebuff.nMaxOverLapCount = 999;
        pFlameDebuff.bDebuff = true;
        pFlameDebuff.dicChangedAttr = new Dictionary<string, string>();
        pFlameDebuff.dicChangedAttr["hp"] = "0|-10";
    }

    void InitPoisonDebuff() {
        pPoisonDebuff = new StatusInfo();
        pPoisonDebuff.strStatusName = "giveEnemyPoisonDamage";
        pPoisonDebuff.nTotalTurn = 10;
        pPoisonDebuff.nMaxOverLapCount = 999;
        pPoisonDebuff.bDebuff = true;
        pPoisonDebuff.dicChangedAttr = new Dictionary<string, string>();
        pPoisonDebuff.dicChangedAttr["hp"] = "0|-5";
    }

    void InitSlienceDebuff() {
        pSilenceDebuff = new StatusInfo();
        pSilenceDebuff.strStatusName = "enemyCantDoSkill";
        pSilenceDebuff.nTotalTurn = 2;
        pSilenceDebuff.nMaxOverLapCount = 1;
        pSilenceDebuff.bDebuff = true;
        pSilenceDebuff.dicChangedAttr = new Dictionary<string, string>();
        pSilenceDebuff.dicChangedAttr["slience"] = "0|0";
    }

    void InitPurifierBuff() {
        pPurifier = new StatusInfo();
        pPurifier.strStatusName = "eliminateAllDebuff";
        pPurifier.nTotalTurn = 1;
        pPurifier.nMaxOverLapCount = 1;
        pPurifier.bDebuff = false;
        pPurifier.dicChangedAttr = new Dictionary<string, string>();
        pPurifier.dicChangedAttr["purifier"] = "0|0";
    }

    void InitVulnerabilityByMonk() {
        pVulnerabilityByMonk = new StatusInfo();
        pVulnerabilityByMonk.strStatusName = "MonkGiveVulnerability";
        pVulnerabilityByMonk.nTotalTurn = 2;
        pVulnerabilityByMonk.nMaxOverLapCount = 999;
        pVulnerabilityByMonk.bDebuff = true;
        pVulnerabilityByMonk.dicChangedAttr = new Dictionary<string, string>();
        pVulnerabilityByMonk.dicChangedAttr["vulnerability"] = "50|0";
    }

    void InitVulnerabilityByBerserker() {
        pVulnerabilityByBerserker = new StatusInfo();
        pVulnerabilityByBerserker.strStatusName = "BerserkerGiveVulnerability";
        pVulnerabilityByBerserker.nTotalTurn = 2;
        pVulnerabilityByBerserker.nMaxOverLapCount = 999;
        pVulnerabilityByBerserker.bDebuff = true;
        pVulnerabilityByBerserker.dicChangedAttr = new Dictionary<string, string>();
        pVulnerabilityByBerserker.dicChangedAttr["vulnerability"] = "30|0";
    }

    void InitShieldBuff() {
        pShield = new StatusInfo();
        pShield.strStatusName = "landGuardianGiveTeammateBuff";
        pShield.nTotalTurn = 2;
        pShield.nMaxOverLapCount = 999;
        pShield.bDebuff = false;
        pShield.dicChangedAttr = new Dictionary<string, string>();
        pShield.dicChangedAttr["defend"] = "0|15";
    }

    void InitDoubleDebuff() {
        pDoubleDebuff = new StatusInfo();
        pDoubleDebuff.strStatusName = "DoubleDebuff";
        pDoubleDebuff.nTotalTurn = 2;
        pDoubleDebuff.nMaxOverLapCount = 999;
        pDoubleDebuff.bDebuff = true;
        pDoubleDebuff.dicChangedAttr = new Dictionary<string, string>();
        pDoubleDebuff.dicChangedAttr["DoubleDebuff"] = "0|0";
    }

    void InitDoubleDamageBuff() {
        pDoubleDamageBuff = new StatusInfo();
        pDoubleDamageBuff.strStatusName = "DoubleDamageBuff";
        pDoubleDamageBuff.nTotalTurn = 2;
        pDoubleDamageBuff.nMaxOverLapCount = 999;
        pDoubleDamageBuff.bDebuff = false;
        pDoubleDamageBuff.dicChangedAttr = new Dictionary<string, string>();
        pDoubleDamageBuff.dicChangedAttr["DoubleDamageBuff"] = "0|0";
    }

    void InitDoubleAllBuff() {
        pDoubleAllBuff = new StatusInfo();
        pDoubleAllBuff.strStatusName = "DoubleAllBuff";
        pDoubleAllBuff.nTotalTurn = 2;
        pDoubleAllBuff.nMaxOverLapCount = 999;
        pDoubleAllBuff.bDebuff = false;
        pDoubleAllBuff.dicChangedAttr = new Dictionary<string, string>();
        pDoubleAllBuff.dicChangedAttr["DoubleAllBuff"] = "0|0";
    }

    void InitStunnedDebuff() {
        pStunnedDebuff = new StatusInfo();
        pStunnedDebuff.strStatusName = "stunnedDebuff";
        pStunnedDebuff.nTotalTurn = 2;
        pStunnedDebuff.nMaxOverLapCount = 999;
        pStunnedDebuff.bDebuff = true;
        pStunnedDebuff.dicChangedAttr = new Dictionary<string, string>();
        pStunnedDebuff.dicChangedAttr["Stunned"] = "0|0";
    }

    void InitBackToPos() {
        pBackToPos = new StatusInfo();
        pBackToPos.strStatusName = "LordOfTimeSkill";
        pBackToPos.nTotalTurn = 6;
        pBackToPos.nMaxOverLapCount = 1;
        pBackToPos.bDebuff = false;
        pBackToPos.dicChangedAttr = new Dictionary<string, string>();
        pBackToPos.dicChangedAttr["backToPos"] = "0|0";
    }

    void InitSacrifice() {
        pSacrifice = new StatusInfo();
        pSacrifice.strStatusName = "deathSacrifice";
        pSacrifice.nTotalTurn = 6;
        pSacrifice.nMaxOverLapCount = 1;
        pSacrifice.bDebuff = false;
        pSacrifice.dicChangedAttr = new Dictionary<string, string>();
        pSacrifice.dicChangedAttr["deathSacrifice"] = "0|0";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setHeroDataDic(int heroIndex, HeroData heroes) {
        heroDataDic[heroIndex] = heroes;
    }

    public HeroData getHeroDataDic(int heroIndex) {
        return heroDataDic[heroIndex];
    }

    public void modifyMaxHeroNumDic(int nPlayer, int nValue)
    {
        maxHeroNumDic[nPlayer] = maxHeroNumDic[nPlayer] + nValue;
    }

    public int getMaxHeroNum(int nPlayer)
    {
        return maxHeroNumDic[nPlayer];
    }

    public void addHero(int nPlayer, GameObject unit) {
        if (nPlayer == 0) {
            player0HeroObjList.Add(unit);
        } else if (nPlayer == 1) {
            player1HeroObjList.Add(unit);
        }
    }

    public void removeHero(int nPlayer, GameObject unit) {
        if (nPlayer == 0) {
            player0HeroObjList.Remove(unit);
        } else if (nPlayer == 1) {
            player1HeroObjList.Remove(unit);
        }
    }

    public void addHeroByName(string name, GameObject hero) {
        heroByName[name] = hero;
    }

    public HeroEntity.Heroes getHeroByName(string name) {
        return heroByName[name].GetComponent<HeroEntity>().m_pHero;
    }

    public int getCurrHeroNum(int nPlayer)
    {
        if (nPlayer == 0)
        {
            return player0HeroObjList.Count;
        }
        else
        {
            return player1HeroObjList.Count;
        }
    }

    public void removeHeroFromStock(int nIndex) {
        m_lstRandomHeroIndex.Remove(nIndex);
    }

    public void addHeroCountInStock(int nHeroInex, int nCount) {
        for (int i = 0; i < nCount; i++)
        {
            m_lstRandomHeroIndex.Add(nHeroInex);
        }
    }

    public void generateHeroEveryTurn()
    {
        randomHeroEveryTurn.Clear();
        for (int i = 0; i < 3; i++)
        {
            List<int> lstRandomHero = new List<int>();
            lstRandomHero = m_lstRandomHeroIndex;
            if (lstRandomHero.Count <= 0)
            {
                return;
            }
            //int heroIndex = (int)Random.Range(5,12);
            //int heroIndex = GlobalHeroIndex.eEntityType_LordOfTime;
            int nIndex = (int)Random.Range(0, lstRandomHero.Count);
            int heroIndex = lstRandomHero[nIndex];
            lstRandomHero.Remove(nIndex);
            HeroData newRandomHeroData = getHeroDataDic(heroIndex);
            randomHeroEveryTurn.Add(newRandomHeroData);
            UIManager.Instance.heroCardList[i].GetComponent<HeroCard>().nType = heroIndex;
            UIManager.Instance.setHeroName(i,newRandomHeroData.m_strName);
            UIManager.Instance.setHeroSkillDescriptionInTavern(i,newRandomHeroData.m_strSkillDescription);
            UIManager.Instance.setHeroMoney(i,newRandomHeroData.m_nMoneyTobuy);
            UIManager.Instance.setHeroCardSprite(i,heroIndex);
        }
    }

    void resetHeroEntity() {
        Instance = this;
        heroDataDic = new Dictionary<int, HeroData>();
        maxHeroNumDic = new Dictionary<int, int>();
        maxHeroNumDic[0] = 5;
        maxHeroNumDic[1] = 5;
        player0HeroObjList = new List<GameObject>();
        player1HeroObjList = new List<GameObject>();
        randomHeroEveryTurn = new List<HeroData>();
        heroByName = new Dictionary<string, GameObject>();
    }
}



//***************************************************************
class CsvReader
{
    private int m_nCurReadLine = 0;
    private int m_nLineCount = 0;
    private Dictionary<string, int> m_dicKeyIndex;
    private string[][] m_pCsvData;

    public CsvReader(string strFilePath)
    {
        // read
        string[] pFileData = File.ReadAllLines(strFilePath);

        // line 0 is key
        m_nCurReadLine = 0;
        m_nLineCount = pFileData.Length - 1;

        // key -> index
        m_dicKeyIndex = new Dictionary<string, int>();
        string[] pKeyData = pFileData[0].Split(',');
        for (int i = 0; i < pKeyData.Length; ++i)
        {
            m_dicKeyIndex[pKeyData[i]] = i;
        }

        // cache data
        m_pCsvData = new string[m_nLineCount][];
        for (int i = 1; i < pFileData.Length; ++i)
        {
            string[] pData = pFileData[i].Split(',');
            m_pCsvData[i - 1] = pData;
        }
    }

    public bool IsEnd()
    {
        if (m_nCurReadLine >= m_nLineCount)
        {
            return true;
        }

        return false;
    }

    public void ReadNextLine()
    {
        ++m_nCurReadLine;
    }

    public void Release()
    {
        m_dicKeyIndex.Clear();
        m_pCsvData = null;
    }

    public int GetInt(string strKeyName)
    {
        int nValue = 0;
        if(!int.TryParse(GetString(strKeyName), out nValue))
        {
            Debug.Log("CsvReader : Read " + strKeyName + " failed, default as 0.");
        }
        
        return nValue;
    }

    public string GetString(string strKeyName)
    {
        int nKeyIndex = m_dicKeyIndex[strKeyName];
        return m_pCsvData[m_nCurReadLine][nKeyIndex];
    }
}