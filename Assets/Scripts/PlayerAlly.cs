using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAlly : MonoBehaviour
{
    static int player1Index = 0;
    static int player2Index = 1;

    public class Players {
        private int playerIndex;
        private int currentMoney = 250;
        private int maxHeroNum = 5;
        private int currHeroNum = 0;
        private int maxStructureNum = 2;
        private int currStructureNum = 0;
        private List<GameObject> heroList = new List<GameObject>();
        private int tavernIndexX;
        private int tavernIndexY;
        private Vector2Int workshopIndex;
        private Vector2Int originalIndex;
        private Vector2Int kingdomIndex;
        private bool bHasKindom;
        public Players(int nIndex) {
            playerIndex = nIndex;
            bHasKindom = false;
        }
        public int getPlayerIndex() {
            return playerIndex;
        }
        public void setMoney(int value) {
            currentMoney = value;
        }

        public int getMoney() {
            return currentMoney;
        }

        public void modifyMoney(int value) {
            currentMoney += value;
        }

        public void modifyMaxHeroNum(int value) {
            maxHeroNum += value;
        }

        public int getMaxHeroNum() {
            return maxHeroNum;
        }

        public void modifyCurrentHeroNum(int value) {
            currHeroNum += value;
        }
        public int getCurrHeroNum() {
            return currHeroNum;
        }

        public void modifyCurrStrutureNum(int value) {
            currStructureNum += value;
        }

        public int getCurrStructureNum() {
            return currStructureNum;
        }

        public int getMaxStructureNum() {
            return maxStructureNum;
        }

        public void addHero(GameObject newHero) {
            if (heroList.Count <= maxHeroNum) {
                heroList.Add(newHero);
                currHeroNum += 1;
            }
        }

        public void removeHero(GameObject heroObj)
        {
            heroList.Remove(heroObj);
            currHeroNum -= 1;
        }

        public void setTavernIndex(Vector2Int index) {
            tavernIndexX = index.x;
            tavernIndexY = index.y;
        }

        public Vector2Int getTavernIndex() {
            Vector2Int index = new Vector2Int(tavernIndexX,tavernIndexY);
            return index;
        }

        public void setWorkShopIndex(Vector2Int index) {
            workshopIndex = index;
        }

        public Vector2Int getWorkshopIndex() {
            return workshopIndex;
        }


        public void resetEveryHeroInfoOnTurn() {
            for (int i = 0; i < heroList.Count; i++)
            {
                heroList[i].GetComponent<HeroEntity>().resetHeroInfoEveryTurn();
            }
        }

        public void setOriginalIndex(Vector2Int value) {
            originalIndex = value;
        }

        public Vector2Int getOriginalIndex() {
            return originalIndex;
        }

        public void setKingdomIndex(Vector2Int value) {
            kingdomIndex = value;
        }

        public Vector2Int getKingdomIndex() {
            return kingdomIndex;
        }

        public void setHasKingdom(bool value) {
            bHasKindom = value;
        }

        public bool getBHasKingdom() {
            return bHasKindom;
        }
    }

    public PlayerAlly.Players m_player1;
    public PlayerAlly.Players m_player2;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance == null) {
            return;
        }
        setPlayer1();
        setPlayer2();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPlayer1() {
        m_player1 = new PlayerAlly.Players(0);
        GameManager.Instance.m_player1 = m_player1;
        GameManager.Instance.currPlayer = m_player1;
    }

    public void setPlayer2() {
        m_player2 = new PlayerAlly.Players(1);
        GameManager.Instance.m_player2 = m_player2;
    }
}
