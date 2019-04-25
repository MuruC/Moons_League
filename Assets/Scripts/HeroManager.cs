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
}

public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance;
    private Dictionary<int, HeroData>heroDataDic;
    private Dictionary<int, int> maxHeroNumDic;
    private List<GameObject> player0HeroObjList;
    private List<GameObject> player1HeroObjList;

    private List<HeroData> randomHeroEveryTurn;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            return;
        }
        resetHeroEntity();

        // 读取数据
        CsvReader pReader = new CsvReader(@"W:\Github\gameDev_final\HeroData.csv");
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

            Instance.setHeroDataDic(pData.m_nID, pData);

            pReader.ReadNextLine();
        }
        pReader.Release();
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

    public void generateHeroEveryTurn()
    {
        randomHeroEveryTurn.Clear();
        for (int i = 0; i < 3; i++)
        {
            int heroIndex = (int)Random.Range(0,21);
            HeroData newRandomHeroData = getHeroDataDic(heroIndex);
            randomHeroEveryTurn.Add(newRandomHeroData);
            UIManager.Instance.heroCardList[i].GetComponent<HeroCard>().nType = heroIndex;
            UIManager.Instance.setHeroName(i,newRandomHeroData.m_strName);
            UIManager.Instance.setHeroSkillDescriptionInTavern(i,newRandomHeroData.m_strSkillDescription);
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