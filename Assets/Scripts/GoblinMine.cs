using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinMine : MonoBehaviour
{
    public int x;
    public int y;
    int attack;
    public int ally;
    // Start is called before the first frame update
    void Start()
    {
        attack = HeroManager.Instance.getHeroDataDic(GlobalHeroIndex.eEntityType_GoblinTechies).m_nDamage;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            Destroy(gameObject);
        }
    }

}
