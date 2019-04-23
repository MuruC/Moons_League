using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        return int.Parse(GetString(strKeyName));
    }

    public string GetString(string strKeyName)
    {
        int nKeyIndex = m_dicKeyIndex[strKeyName];
        return m_pCsvData[m_nCurReadLine][nKeyIndex];
    }
}

struct HeroData
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
}

class Program
{

    static void Main()
    {
        Dictionary<int, HeroData> dicHeroData = new Dictionary<int, HeroData>();

        CsvReader pReader = new CsvReader(@"W:\intro to game development\final\HeroData.csv");
        //CsvReader pReader = new CsvReader(); unity
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

            dicHeroData[pData.m_nID] = pData;

            pReader.ReadNextLine();
        }
        pReader.Release();
    }
}


public class csvReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
