using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public int x;
    public int y;
    public int terrainType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSprite(int index) {
        GetComponent<SpriteRenderer>().sprite = UIManager.Instance.tileSprites[index];

    }

    public void SetAlpha(int alpha) {
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = alpha;
        GetComponent<SpriteRenderer>().color = tmp;
    }
}
