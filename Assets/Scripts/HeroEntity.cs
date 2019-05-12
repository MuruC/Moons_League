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

        m_dicCacheData = new Dictionary<string, string>();

        DoStart();
    }
    public void DoStart()
    {
        ChangeAttr(true);

        if (m_pStatusInfo.strStatusName == "LordOfTimeSkill")
        {
            HeroEntity.Heroes pHero = HeroManager.Instance.getHeroByName(m_strDstPlayerName);
            if (pHero != null)
            {
                // 记录下位置信息
                SetCacheData("SrcPosX",pHero.getPosIndex().x);
                SetCacheData("SrcPosY",pHero.getPosIndex().y);
            }
        }
    }

    public StatusInfo getStatusInfo() {
        return m_pStatusInfo;
    }

    public void DoFinish()
    {
        if (bRemoved)
        {
            return;
        }
        bRemoved = true;
        /*
        for (int i = m_lstOverLapCount.Count - 1; i >= 0; --i)
        {
            m_lstOverLapCount.Remove(i);
            ChangeAttr(false);
        }
        */
        HeroEntity.Heroes pHero = HeroManager.Instance.getHeroByName(m_strDstPlayerName);
        if (pHero == null)
        {
            return;
        }
        if (m_pStatusInfo.strStatusName == "deathSacrifice")
        {
            //献祭：加给对方的

                HeroSkill.Instance.aoeAttack(3, GameObject.Find(m_strDstPlayerName));

            pHero.setHP(0);

        }
        else if (m_pStatusInfo.strStatusName == "LordOfTimeSkill")
        {
            //回溯：加给自己的的
      
                //设置位置回去
                GameObject dstHero = GameObject.Find(m_strDstPlayerName);
                MovableUnit heroMovingScript = dstHero.GetComponent<MovableUnit>();
                int x = int.Parse(GetCacheData("SrcPosX"));
                int y = int.Parse(GetCacheData("SrcPosY"));

            if (GameManager.Instance.bTileHasUnit(new Vector2Int(x,y)))
            {
                bool bFindEmptyPos = false;
                for (int ox = -1; ox !=0 && ox< 1; ox++)
                {
                    for (int oy = -1; oy != 0 && oy<= 1; oy++)
                    {
                        int new_x = x + ox;
                        int new_y = y + oy;

                        int nType = GameManager.Instance.getTileObjectByIndex("xIndex_" + new_x.ToString() + "yIndex_" + new_y.ToString()).GetComponent<TileScript>().terrainType;
                        if (!GameManager.Instance.bTileHasUnit(new Vector2Int(new_x,new_y)) && nType != GlobTileType.eTile_nObstacle && nType != GlobTileType.eTile_nSea)
                        {
                            bFindEmptyPos = true;
                            x = new_x;
                            y = new_y;
                            break;
                        }
                    }

                    if (bFindEmptyPos)
                    {
                        break;
                    }
                }
            }

                heroMovingScript.indexX = x;
                heroMovingScript.indexY = y;
                heroMovingScript.destinationXIndex = x;
                heroMovingScript.destinationYIndex = y;
                GameObject tile = GameManager.Instance.getTileObjectByIndex("xIndex_" + x.ToString() + "yIndex_" +y.ToString());
                dstHero.transform.position = tile.transform.position;
                heroMovingScript.destination = dstHero.transform.position;
            HeroSkill.Instance.spawnClock(tile.transform.position);
            //GameObject.Find("Main Camera").GetComponent<CameraShake>().shake(0.2f,0.3f);
            HeroSkill.Instance.aoeAttack(2, GameObject.Find(m_strDstPlayerName));
            SoundEffectManager.Instance.playAudio(9);
        }

        for (int i = m_lstOverLapCount.Count - 1; i >= 0; --i)
        {
            m_lstOverLapCount.Remove(i);
            ChangeAttr(false);
        }


       
            pHero.removeStatusByName(m_pStatusInfo.strStatusName);
        
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

        if (strStatusName == "giveEnemyPoisonDamage") {
            if (m_dicChangedAttr.ContainsKey("hp")) {
                string[] arrValues = m_dicChangedAttr["hp"].Split('|');
                //变化的百分比和固定值
                int nPercent = int.Parse(arrValues[0]);
                int nValue = int.Parse(arrValues[1])*m_nCurOverLapCount;

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
                m_lstOverLapCount.RemoveAt(i);
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
            } else if (kvp.Key == "backToPos") {
                //Debug.Log("backToPos!");
            }
            else if (kvp.Key == "deathSacrifice")
            {

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

    public void SetCacheData(string strKey, string strValue) {
        m_dicCacheData[strKey] = strValue;
    }

    public void SetCacheData(string strKey, int strValue) {
        m_dicCacheData[strKey] = strValue.ToString();
    }

    public void SetCacheData(string strKey, float strValue)
    {
        m_dicCacheData[strKey] = strValue.ToString();
    }
    public void SetCacheData(string strKey, double strValue)
    {
        m_dicCacheData[strKey] = strValue.ToString();
    }
    public string GetCacheData(string strKey) {
        if (!m_dicCacheData.ContainsKey(strKey))
        {
            return "0";
        }
        return m_dicCacheData[strKey];
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

    private bool bRemoved = false;

    private Dictionary<string, string> m_dicCacheData; //
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
        private Vector2Int posIndex;
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

        public void setPos(Vector2Int pos) {
            posIndex = pos;
        }

        public Vector2Int getPosIndex() {
            return posIndex;
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
    float deathTime = 0;
    bool setDeathTime = false;
    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = UIManager.Instance.heroIconOnMapSrpites[nEntityType];
        heroUnitAnimator = GetComponent<Animator>();
        goblinMineList = new List<GameObject>();
        if (nAlign == 1 && nEntityType == GlobalHeroIndex.eEntityType_King)
        {
            GetComponent<SpriteRenderer>().sprite = UIManager.Instance.heroIconOnMapSrpites[23];
        }
        rend = GetComponent<Renderer>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if(m_pHero == null)
        {
            return;
        }
        if (m_pHero.getCurrHP() <= 0 && setDeathTime == false)
        {
            setDeathTime = true;
            deathTime = Time.time;
            for (int i = 0; i < 3; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<awakeDissolve>().setDissolveTime();
            }
            SoundEffectManager.Instance.playAudio(7);
        }

        
        if (setDeathTime)
        {
            rend.material.shader = Shader.Find("Custom/2D/Dissolve");
            float threshold = Time.time - deathTime;
            rend.material.SetFloat("_Threshold", threshold);

            if (threshold > 1)
            {
                destroyThisCharacter();
            }

        }
        /*
        foreach(KeyValuePair<string, Status> kvp in m_pHero.getStatusList())
        {
            kvp.Value.Update(GameManager.Instance.getCurrentTurn());
        }
        */
        Dictionary<string, Status> pStatusList = m_pHero.getStatusList();
        List<string> lstStatusNames = new List<string>(pStatusList.Keys);
        for (int i = lstStatusNames.Count - 1; i >= 0; --i)
        {
            string strKey = lstStatusNames[i];
            pStatusList[strKey].Update(GameManager.Instance.getCurrentTurn());
        }

        if (m_pHero.getStunnedStatus())
        {
            gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = UIManager.Instance.stunnedMask;
        }
        else
        {
            gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
        }

        if (m_pHero.getSilenceStatus())
        {
            gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = UIManager.Instance.silenceMask;
        }
        else
        {
            gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
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
            bool hasTwoMine = true;
            if (goblinMineList[0] == null)
            {
                goblinMineList[0] = goblinMine;
                hasTwoMine = false;
            }
            else if (goblinMineList[1] == null)
            {
                goblinMineList[1] = goblinMine;
                hasTwoMine = false;
            }

            if (hasTwoMine)
            {
                GameObject goblinMine2 = goblinMineList[1];

                goblinMineList[0].GetComponent<GoblinMine>().destroyThisObject();
                goblinMineList[0] = goblinMine2;
                goblinMineList[1] = goblinMine;
            }
            
        }
    }

    public void resetHeroInfoEveryTurn() {
        m_pHero.setCurrentMoveStep(m_pHero.getMaxMoveStep());
        setAnime(false);
    }

    void destroyThisCharacter() {
        HeroManager.Instance.removeHero(nAlign, gameObject);
        if (nAlign == 0)
        {
            GameManager.Instance.m_player1.removeHero(gameObject);
            GameManager.Instance.m_player1.modifyCurrentHeroNum(-1);
            GameManager.Instance.m_player2.modifyMoney(HeroManager.Instance.getHeroDataDic(nEntityType).m_nGetMoneyByKillingThisHero);
        }
        else
        {
            GameManager.Instance.m_player2.removeHero(gameObject);
            GameManager.Instance.m_player2.modifyCurrentHeroNum(-1);
            GameManager.Instance.m_player1.modifyMoney(HeroManager.Instance.getHeroDataDic(nEntityType).m_nGetMoneyByKillingThisHero);
        }
        Vector2Int pos = new Vector2Int(gameObject.GetComponent<MovableUnit>().indexX, gameObject.GetComponent<MovableUnit>().indexY);
        GameManager.Instance.modifyTileHasUnit(pos);
        GameManager.Instance.modifyUnitInTile(pos);
        HeroManager.Instance.addHeroCountInStock(nEntityType, 1);
        Destroy(gameObject);
    }
}
