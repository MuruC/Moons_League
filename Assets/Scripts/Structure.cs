using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public int nPlayer;
    public int x;
    public int y;
    public int nType;
    // Start is called before the first frame update
    private void Awake()
    {
        nPlayer = GameManager.Instance.getTurn();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<HeroEntity>() == null)
        {
            return;
        }

        if (collision.gameObject.GetComponent<HeroEntity>().nAlign == nPlayer)
        {
            return;
        }

        if (nType == GlobStructureType.eStructure_nKingdom)
        {
            Destroy(gameObject);
        }
    }
    
}
