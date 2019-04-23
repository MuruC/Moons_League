using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalState {
    public static int eState_Nomal = 0;
    public static int eState_Move = 1;
    public static int eState_Attack = 2;
    public static int eState_Sell = 0;
}

public static class GlobalHeroIndex {
    static int eEntityType_GoblinTechies = 0;
    static int eEntityType_Paladin = 1;
    static int eEntityType_WitchDoctor = 2;
    static int eEntityType_Bard = 3;
    static int eEntityType_Snaker = 4;
    static int eEntityType_Snow = 5;
    static int eEntityType_SandShaper = 6;
    static int eEntityType_Mole = 7;
    static int eEntityType_Monk = 8;
    static int eEntityType_Flame = 9;
    static int eEntityType_Lich = 10;
    static int eEntityType_Silencer = 11;
    static int eEntityType_ElfArcher = 12;
    static int eEntityType_Berserker = 13;
    static int eEntityType_Cleric = 14;
    static int eEntityType_Purifier = 15;
    static int eEntityType_LandGuardian = 16;
    static int eEntityType_Rogue = 17;
    static int eEntityType_DeathAlchemist = 18;
    static int eEntityType_LordOfTime = 19;
    static int eEntityType_MasterOfCircus = 20;
}

public static class ThisHero {
   // private int currentHP;

}



public class HeroEntity : MonoBehaviour
{
    public static HeroEntity Instance;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
