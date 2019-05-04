using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCard : MonoBehaviour
{
    public int nType;
    GameObject unitSpawner;
    // Start is called before the first frame update
    void Start()
    {
        unitSpawner = GameObject.Find("UnitSpawner");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUnactive() {
        /*
        Color tmp = GetComponent<Image>().color;
        tmp.a = alpha;
        GetComponent<Image>().color = tmp;
        */

        gameObject.SetActive(false);
    }

    public void spawnHero() {
        unitSpawner.GetComponent<UnitSpawner>().spawnHero(nType);
    }
}
