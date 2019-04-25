using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject heroUnit;
    public GameObject minerUnit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnHero(int nType) {
        int nAlign = GameManager.Instance.getTurn();
        GameObject pHeroUnit = Instantiate(heroUnit);
        pHeroUnit.GetComponent<HeroEntity>().setEntity(nType,nAlign);
        HeroManager.Instance.addHero(nAlign,pHeroUnit);
    }
}
