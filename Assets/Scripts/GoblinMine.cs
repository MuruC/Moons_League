using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinMine : MonoBehaviour
{
    public int x;
    public int y;
    int attack;
    public int ally;
    public GameObject warningTilePreb;
    private List<GameObject> warningTileLst;
    private bool hasSpawnWarningTile = false;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (hasSpawnWarningTile == false) {
            return;
        }
        setMineVisible();
    }

    private void Awake()
    {
        warningTileLst = new List<GameObject>();
        attack = HeroManager.Instance.getHeroDataDic(GlobalHeroIndex.eEntityType_GoblinTechies).m_nDamage;
    }

    public void whenAwake() {
        List<int> offset = new List<int> { -1, 0, 1 };
        int offsetX = offset[(int)Random.Range(0, 3)];
        int offsetY = 0;
        if (offsetX != 0)
        {
            offsetY = 0;
        }
        else
        {
            offsetY = offset[(int)Random.Range(0, 3)];
        }
        warningTileIndex(offsetX, offsetY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.GetComponent<HeroEntity>())
        {
            return;
        }

        HeroEntity collisionScript = collision.gameObject.GetComponent<HeroEntity>();
        if (collisionScript.nAlign != ally)
        {
            HeroEntity.Heroes enemy_pHero = collisionScript.m_pHero;
            int defenseValue = enemy_pHero.getDefense();
            if (defenseValue > 0)
            {
                if (attack > defenseValue)
                {
                    attack -= defenseValue;
                    enemy_pHero.setDefense(0);
                }
                else
                {
                    attack = 0;
                    enemy_pHero.modifyDefense(-attack);
                }
            }
            enemy_pHero.modifyHP(-attack);
            destroyThisObject();
        }
    }

    public void destroyThisObject() {
        for (int i = 0; i < warningTileLst.Count; i++)
        {
            Destroy(warningTileLst[i]);
        }
        warningTileLst.Clear();
        Destroy(gameObject);
    }

    void warningTileIndex(int offsetX, int offsetY)
    {
        int a = x+offsetX;
        int b = y+offsetY;
        if (b % 2 == 1)
        {
            spawnWarningTile(a + 1, b);
            spawnWarningTile(a, b + 1);
            spawnWarningTile(a + 1, b + 1);
            spawnWarningTile(a + 1, b - 1);
            spawnWarningTile(a, b - 1);
            spawnWarningTile(a - 1, b);
            spawnWarningTile(a, b);
        }
        else
        {
            spawnWarningTile(a, b + 1);
            spawnWarningTile(a + 1, b);
            spawnWarningTile(a - 1, b + 1);
            spawnWarningTile(a - 1, b);
            spawnWarningTile(a - 1, b - 1);
            spawnWarningTile(a, b - 1);
            spawnWarningTile(a, b);
        }
        hasSpawnWarningTile = true;
    }

    void spawnWarningTile(int x, int y) {
        string name = "xIndex_" + x.ToString() + "yIndex_" + y.ToString();
        Vector2 pos = GameManager.Instance.getTileObjectByIndex(name).transform.position;
        GameObject newTile = Instantiate(warningTilePreb, pos,Quaternion.identity);
        newTile.SetActive(false);
        warningTileLst.Add(newTile);
    }

    void setMineVisible() {
        if (ally == GameManager.Instance.getTurn()) {
            for (int i = 0; i < warningTileLst.Count; i++)
            {
                warningTileLst[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < warningTileLst.Count; i++)
            {
                warningTileLst[i].SetActive(true);
            }
        }
    }
}
