using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileScript : MonoBehaviour
{
    public int x;
    public int y;
    public int terrainType;
    public int myColorAnime;
    Animator tileAnimator;

    public GameObject tileExplanationPrefab;
    // Start is called before the first frame update
    void Start()
    {
        tileAnimator = GetComponent<Animator>();
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

    public void setColorAnime(int colorAnimeIndex) {
        tileAnimator.SetInteger("gridAnimationState", colorAnimeIndex);
    }

    private void OnMouseOver()
    {
        if (terrainType == GlobTileType.eTile_nGoldMine)
        {
            //pHeroUnit.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = UIManager.Instance.heroChips[0];
            GameObject tileExplanation = Instantiate(tileExplanationPrefab,transform.position,Quaternion.identity);
            tileExplanation.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Gold Mine. Need a miner. Get 20 C.";
        }

        if (terrainType == GlobTileType.eTile_nLake)
        {
            GameObject tileExplanation = Instantiate(tileExplanationPrefab, transform.position, Quaternion.identity);
            tileExplanation.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Text>().text = "HolyLoch: remote all debuffs.";
        }
    }
}
