using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//
// Status : Buff/Debuff
//

// Status的配置信息
public struct StatusInfo
{
    public string strStatusName;
    public int nTotalTurn;
    public int nMaxOverLapCount;
    public bool bDebuff; // 增益/减益
    public Dictionary<string, string> dicChangedAttr; // "damage" -> "10|5" 含义是伤害属性+10%，+5
}

// 每个Hero会有一份他身上所有Status的Dictonary
public class Status
{
    public Status(string strSrcName, string strDstName, StatusInfo pInfo)
    {
        //
        m_strSrcPlayerName = strSrcName;
        m_strDstPlayerName = strDstName;

        //
        m_nUpdateTurn = pInfo.nTotalTurn;
        m_lstOverLapCount = new List<int>();
        m_lstOverLapCount.Add(GameManager.Instance.getCurrentTurn() + m_nUpdateTurn);

        m_nMaxOverLapCount = pInfo.nMaxOverLapCount;
        m_nCurOverLapCount = 1;
        m_nLastUpdateTurn = -1;

        //
        bIsDebuff = pInfo.bDebuff;
        m_fAttrScale = 1.0f;
        m_dicChangedAttr = pInfo.dicChangedAttr;

        //
        DoStart();
        strStatusName = pInfo.strStatusName;

        m_pStatusInfo = pInfo;
    }
    public void DoStart()
    {
        ChangeAttr(true);
    }

    public StatusInfo getStatusInfo() {
        return m_pStatusInfo;
    }

    public void DoFinish()
    {
        for (int i = m_lstOverLapCount.Count - 1; i >= 0; --i)
        {
            m_lstOverLapCount.Remove(i);
            ChangeAttr(false);
        }
    }

    public bool getIsDebuff() {
        return bIsDebuff;
    }

    
    public string getStatusName() {
        return strStatusName;
    }


    public void DoEachTurn() {
        HeroEntity.Heroes pHero = HeroManager.Instance.getHeroByName(m_strDstPlayerName);
        if (pHero == null) {
            return;
        }

        if (strStatusName == "GiveEnemyPoisonDamage") {
            if (m_dicChangedAttr.ContainsKey("hp")) {
                string[] arrValues = m_dicChangedAttr["hp"].Split('|');
                //变化的百分比和固定值
                int nPercent = int.Parse(arrValues[0]);
                int nValue = int.Parse(arrValues[1]);

                pHero.modifyHP(nValue);
            }
        }

        if (strStatusName == "giveEnemyBuringDamage") {
            if (m_dicChangedAttr.ContainsKey("hp")) {
                string[] arrValues = m_dicChangedAttr["hp"].Split('|');
                //变化的百分比和固定值
                int nPercent = int.Parse(arrValues[0]);
                int nValue = int.Parse(arrValues[1]);

                pHero.modifyHP(nValue);
            }
        }
    }

    // 需要在Hero做Update的时候一起Update
    public void Update(int nCurTurn)
    {
        int nCurrentTurn = GameManager.Instance.getCurrentTurn();
        for(int i = m_lstOverLapCount.Count - 1; i >= 0; --i)
        {
            int nEndTurn = m_lstOverLapCount[i];
            int nRemainTurn = nCurrentTurn - nEndTurn;
            if (nRemainTurn >= 0)
            {
                m_lstOverLapCount.Remove(i);
                ChangeAttr(false);
            }
            else {
                if (m_nLastUpdateTurn != nCurTurn) {
                    m_nLastUpdateTurn = nCurTurn;
                    DoEachTurn();
                }
            }
        }

        if(m_lstOverLapCount.Count <= 0)
        {
            DoFinish();
        }
    }

    public void AddOverLapCount(int nCount)
    {
        int nValidCount = m_nMaxOverLapCount - m_nCurOverLapCount;
        if(nValidCount <= 0)
        {
            return;
        }

        int nAddCount = nCount;
        if(nCount > nValidCount)
        {
            nAddCount = nValidCount;
        }
        m_nCurOverLapCount += nAddCount;

        for(int i = 0; i < nAddCount; ++i)
        {
            ChangeAttr(true);
        }
    }

    // 添加/移除属性
    public void ChangeAttr(bool bAdd)
    {
        HeroEntity.Heroes pHero = HeroManager.Instance.getHeroByName(m_strDstPlayerName);
        if(pHero == null)
        {
            return;
        }

        // 拥有者属性变化
        foreach (KeyValuePair<string, string> kvp in m_dicChangedAttr)
        {
            // 以竖线分割的两个参数，约定第一个是百分比，第二个是固定值
            string[] arrValues = kvp.Value.Split('|');
            if (arrValues.Length != 2)
            {
                continue;
            }

            // 变化倍率：m_fAttrScale

            // 变化的百分比和固定值
            int nPercent = int.Parse(arrValues[0]);
            int nValue = int.Parse(arrValues[1]);

            // Hero属性相应变化
            if (kvp.Key == "damage")
            {
                // 移除恢复百分比会有bug
                int nOldAttack = pHero.getAttack();
                int nOffset = (int)(nOldAttack * nPercent * 0.01) + nValue;
                if (bAdd)
                {
                    pHero.modifyAttack(nOffset);
                }
                else
                {
                    pHero.modifyAttack(-nOffset);
                }
            }
            else if (kvp.Key == "defend")
            {
                int nOldDefense = pHero.getDefense();
                int nOffset = (int)(nOldDefense * nPercent * 0.01) + nValue;
                if (bAdd) {
                    pHero.modifyDefense(nOffset);
                }
                else {
                    pHero.modifyDefense(-nOffset);
                }
            }
            else if (kvp.Key == "hp")
            {
               // pHero.modifyHP(nValue);
            }
            else if (kvp.Key == "slience")
            {
                if (bAdd)
                {
                    pHero.setSilence(true);
                }
                else {
                    pHero.setSilence(false);
                }
            }
            else if (kvp.Key == "purifier") {
                if (pHero.getDebuffNum() <= 0) {
                    return;
                }
                pHero.eliminateAllDebuffByName();
                if (bAdd)
                {
                    pHero.setStunned(true);
                }
                else {
                    pHero.setStunned(false);
                }
            }
            else if (kvp.Key == "vulnerability") {
                int nOldVulnerability = pHero.getVulnerability();
                int nOffset = (int)(nOldVulnerability * nPercent * 0.01) + nValue;
                if (bAdd) {
                    pHero.modifyVulnerability(nOffset);
                }
                else {
                    pHero.modifyVulnerability(-nOffset);
                }
            }
            else if (kvp.Key == "DoubleDebuff") {
                pHero.doubleDebuff();
            }
            else if (kvp.Key == "DoubleDamageBuff") {
                pHero.doubleDamageBuff();
            }
            else if (kvp.Key == "DoubleAllBuff") {
                pHero.doubleAllBuff();
            }
            else if (kvp.Key == "Stunned") {
                pHero.eliminateAllDebuffByName();
                if (bAdd)
                {
                    pHero.setStunned(true);
                }
                else
                {
                    pHero.setStunned(false);
                }
            }
        }
    }

    public void SetAttrScale(float fAttrScale)
    {
        // 先移除旧添加的属性
        ChangeAttr(false);
        // 变化倍率
        m_fAttrScale = fAttrScale;
        // 重新添加上去
        ChangeAttr(true);
    }

    private string m_strSrcPlayerName;  // Status来源方
    private string m_strDstPlayerName;  // Status拥有者

    private int m_nLastUpdateTurn;      //上一次DoEachTurn的回合
    private int m_nStartTurn;           // 开始回合数
    private int m_nUpdateTurn;          // 持续回合数
    private List<int> m_lstOverLapCount;

    private int m_nMaxOverLapCount;     // 最大可叠加层数
    private int m_nCurOverLapCount;

    private bool bIsDebuff;             // 增益/减益    
    private float m_fAttrScale;         // 属性变化倍率
    private Dictionary<string, string> m_dicChangedAttr; // 属性变化列表

    private string strStatusName; //属性name
    private StatusInfo m_pStatusInfo;
}


public class HeroEntity : MonoBehaviour
{
    public class Heroes {
        private int nEntityType;
        private int nAlignType;
        private int currentHP;
        private int currentMoveStep;
        private int maxMoveStep;
        private int maxHP;
        private int maxAttack;
        private int currentAttack;
        private int currentState;
        private int defense;
        private string strName;
        private Dictionary<string, Status> dicStatusList;
        private bool bStun;
        private bool bSilence;
        private int vulnerability;
        public Heroes(int nType, int nAlign) {
            nEntityType = nType;
            nAlignType = nAlign;
            currentHP = HeroManager.Instance.getHeroDataDic(nType).m_nHP;
            maxHP = HeroManager.Instance.getHeroDataDic(nType).m_nHP;
            maxMoveStep = HeroManager.Instance.getHeroDataDic(nType).m_nMaxMove;
            currentMoveStep = HeroManager.Instance.getHeroDataDic(nType).m_nMaxMove;
            maxAttack = HeroManager.Instance.getHeroDataDic(nType).m_nDamage;
            currentAttack = HeroManager.Instance.getHeroDataDic(nType).m_nDamage;
            defense = 0;
            strName = "";
            bStun = false;
            bSilence = false;
            dicStatusList = new Dictionary<string, Status>();
            vulnerability = 1;
        }

        public void setHP(int value) {
            currentHP = value;
        }
        public void modifyHP(int value) {
            currentHP += value * vulnerability;
            currentHP = Mathf.Clamp(currentHP,0,maxHP);
        }
        public int getCurrHP() {
            return currentHP;
        }

        public void modifyHPByHealing(int value) {
            currentHP += value;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        }
        public int getAttack() {
            return currentAttack;
        }

        public void modifyAttack(int nOffset)
        {
            currentAttack += nOffset;
            if(currentAttack < 0)
            {
                currentAttack = 0;
            }
        }

        public void setAttack(int nValue)
        {
            currentAttack = nValue;
        }
        public int getBasicAttack() {
            return maxAttack;
        }
        public void setCurrentMoveStep(int value) {
            currentMoveStep = value;
        }
        public void modifyCurrentMoveStep(int value) {
            currentMoveStep += value;
            currentMoveStep = Mathf.Clamp(currentMoveStep,0,maxMoveStep);
        }
        public int getCurrentMoveStep()
        {
            return currentMoveStep;
        }
        public int getMaxMoveStep() {
            return maxMoveStep;
        }
        public void setVulnerability(int value) {
            vulnerability = value;
        }
        public void modifyVulnerability(int value) {
            vulnerability += value;
        }
        public int getVulnerability() {
            return vulnerability;
        }

        public void setDefense(int value) {
            defense = value;
        }
        public void modifyDefense(int value) {
            defense += value;
        }
        public int getDefense() {
            return defense;
        }
        public void addStatus(string strSrcHeroName, StatusInfo pInfo)
        {
            if(!dicStatusList.ContainsKey(pInfo.strStatusName))
            {
                Status pStatus = new Status(strSrcHeroName, strName, pInfo);
                dicStatusList[pInfo.strStatusName] = pStatus;
                return;
            }

            dicStatusList[pInfo.strStatusName].AddOverLapCount(1);
        }
        public void removeStatusByName(string strStatusName)
        {
            if(!dicStatusList.ContainsKey(strStatusName))
            {
                return;
            }

            dicStatusList[strStatusName].DoFinish();
            dicStatusList.Remove(strStatusName);
        }

        public void stealStatus(string strSrcHeroName)
        {
            HeroEntity.Heroes pHero = HeroManager.Instance.getHeroByName(strSrcHeroName);
            if (pHero == null)
            {
                return;
            }
            Dictionary<string, Status> dic = pHero.getStatusList();

            for (int i = 0; i < dic.Count; i++)
            {
                string strStatusName = dic.ElementAt(i).Key;
                StatusInfo pInfo_ = dic.ElementAt(i).Value.getStatusInfo();
                pHero.removeStatusByName(strStatusName);
                addStatus(strSrcHeroName, pInfo_);
            }

        }

        public void eliminateAllDebuffByName() {
            if (dicStatusList.Count <= 0) {
                return;
            }
            for (int i = 0; i < dicStatusList.Count; i++)
            {
                string name = dicStatusList.ElementAt(i).Key;
                if (dicStatusList.ElementAt(i).Value.getIsDebuff() == true) {
                    dicStatusList[name].DoFinish();
                    dicStatusList.Remove(name);
                }

            }
        }

        public int getDebuffNum() {
            int debuffNum = 0;
            for (int i = 0; i < dicStatusList.Count; i++)
            {
                if (dicStatusList.ElementAt(i).Value.getIsDebuff() == true) {
                    debuffNum++;
                }
            }
            return debuffNum;
        }

        public void doubleDebuff() {
            for (int i = 0; i < dicStatusList.Count; i++)
            {
                string name = dicStatusList.ElementAt(i).Key;
                if (dicStatusList.ElementAt(i).Value.getIsDebuff() == true)
                {
                    dicStatusList[name].SetAttrScale(2);
                }
            }
        }

        public void doubleDamageBuff() {
            for (int i = 0; i < dicStatusList.Count; i++)
            {
                string name = dicStatusList.ElementAt(i).Key;
                if (dicStatusList.ElementAt(i).Value.getStatusName() == "ezhu_paoxiao")
                {
                    dicStatusList[name].SetAttrScale(2);
                }
            }
        }

        public void doubleAllBuff() {
            for (int i = 0; i < dicStatusList.Count; i++)
            {
                string name = dicStatusList.ElementAt(i).Key;
                if (dicStatusList.ElementAt(i).Value.getIsDebuff() == false) {
                    dicStatusList[name].SetAttrScale(2);
                }
            }
        }
        public Dictionary<string, Status> getStatusList()
        {
            return dicStatusList;
        }
        public void setName(string thisHeroName) {
            strName = thisHeroName;
        }

        public string getName() {
            return strName;
        }

        public void setSilence(bool value) {
            bSilence = value;
        }

        public bool getSilenceStatus() {
            return bSilence;
        }

        public void setStunned(bool value) {
            bStun = value;
        }

        public bool getStunnedStatus() {
            return bStun;
        }
    }


    public int nEntityType;
    public int nAlign;
    public HeroEntity.Heroes m_pHero;
    Animator heroUnitAnimator;
    public int actionMode = 0;
    private List<GameObject> goblinMineList;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = UIManager.Instance.heroIconOnMapSrpites[nEntityType];
        heroUnitAnimator = GetComponent<Animator>();
        goblinMineList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_pHero == null)
        {
            return;
        }
        if (m_pHero.getCurrHP() <= 0)
        {
            HeroManager.Instance.removeHero(nAlign, gameObject);

            Destroy(gameObject);
        }

        foreach(KeyValuePair<string, Status> kvp in m_pHero.getStatusList())
        {
            kvp.Value.Update(GameManager.Instance.getCurrentTurn());
        }



        heroUnitAnimator.SetInteger("heroAlliance", nAlign);
    }

    public void setEntity(int nType,int nAlign_) {
        nEntityType = nType;
        nAlign = nAlign_;
        m_pHero = new HeroEntity.Heroes(nEntityType,nAlign);
    }

    public void setAnime(bool bSelected) {
        heroUnitAnimator.SetBool("beSelected",bSelected);
    }

    public void addGoblinMineToList(GameObject goblinMine) {
        if (goblinMineList.Count < 2)
        {
            goblinMineList.Add(goblinMine);
        }
        else {
            GameObject goblinMine2 = goblinMineList[1];

            Destroy(goblinMineList[0]);
            goblinMineList[0] = goblinMine2;
            goblinMineList[1] = goblinMine;

        }
    }

    public void resetHeroInfoEveryTurn() {
        m_pHero.setCurrentMoveStep(m_pHero.getMaxMoveStep());
    }
}
