using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MoverOverActionButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler

{
    public GameObject skillTextPanel;
    public Text skillText;
    public GameObject mouseController;
    MouseController mouseControllerScript;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mouseControllerScript.selectedUnitObj == null) {
            return;
        }
        skillTextPanel.SetActive(true);
        HeroData thisHeroData = HeroManager.Instance.getHeroDataDic(mouseControllerScript.selectedUnitObj.GetComponent<HeroEntity>().nEntityType);
        skillText.text = thisHeroData.m_strSkillDescription;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillTextPanel.SetActive(false);
    }

    void Start()
    {
        mouseControllerScript = mouseController.GetComponent<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    
}
