using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }

        public void setHP(int value) {
            currentHP = value;
        }
        public void modifyHP(int value) {
            currentHP += value;
        }
        public int getHP() {
            return currentHP;
        }

        public void setCurrentMoveStep(int value) {
            currentMoveStep = value;
        }
        public void modifyCurrentMoveStep(int value) {
            currentMoveStep += value;
        }
        public int getCurrentMoveStep()
        {
            return currentMoveStep;
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
    }


    public int nEntityType;
    public int nAlign;
    public HeroEntity.Heroes m_pHero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setEntity(int nType,int nAlign_) {
        nEntityType = nType;
        nAlign = nAlign_;
        m_pHero = new HeroEntity.Heroes(nEntityType,nAlign);
    }
}
