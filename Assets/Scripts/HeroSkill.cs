using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSkill : MonoBehaviour
{
    public static HeroSkill Instance;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            return;
        }

        resetHeroSkill();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void releaseHeroSkill(int heroIndex) {
        switch (heroIndex) {
            case GlobalHeroIndex.eEntityType_GoblinTechies:
                skill_GoblinTechies();
                break;
            case GlobalHeroIndex.eEntityType_Paladin:
                skill_Paladin();
                break;
            case GlobalHeroIndex.eEntityType_WitchDoctor:
                skill_WitchDoctor();
                break;
            case GlobalHeroIndex.eEntityType_Bard:
                skill_Bard();
                break;
            case GlobalHeroIndex.eEntityType_Snaker:
                skill_Snaker();
                break;
            case GlobalHeroIndex.eEntityType_Snow:
                skill_Snow();
                break;
            case GlobalHeroIndex.eEntityType_SandShaper:
                skill_SandShaper();
                break;
            case GlobalHeroIndex.eEntityType_Mole:
                skill_Mole();
                break;
            case GlobalHeroIndex.eEntityType_Monk:
                skill_Monk();
                break;
            case GlobalHeroIndex.eEntityType_Flame:
                skill_Flame();
                break;
            case GlobalHeroIndex.eEntityType_Lich:
                skill_Lich();
                break;
            case GlobalHeroIndex.eEntityType_Silencer:
                skill_Silencer();
                break;
            case GlobalHeroIndex.eEntityType_Berserker:
                skill_Berserker();
                break;
            case GlobalHeroIndex.eEntityType_ElfArcher:
                skill_ElfArcher();
                break;
            case GlobalHeroIndex.eEntityType_Purifier:
                skill_Purifier();
                break;
            case GlobalHeroIndex.eEntityType_Cleric:
                skill_Cleric();
                break;
            case GlobalHeroIndex.eEntityType_LandGuardian:
                skill_LandGuardian();
                break;
            case GlobalHeroIndex.eEntityType_Rogue:
                skill_Rogue();
                break;
            case GlobalHeroIndex.eEntityType_DeathAlchemist:
                skill_DeathAlchemist();
                break;
            case GlobalHeroIndex.eEntityType_LordOfTime:
                skill_LordOfTime();
                break;
            case GlobalHeroIndex.eEntityType_MasterOfCircus:
                skill_MasterOfCircus();
                break;
        }
    }

    void skill_GoblinTechies() {


    }

    void skill_Paladin() {

    }

    void skill_WitchDoctor() {

    }

    void skill_Bard() {

    }

    void skill_Snaker() {


    }

    void skill_Snow() {

    }

    void skill_SandShaper() {

    }

    void skill_Mole() {

    }

    void skill_Monk() {


    }

    void skill_Flame() {


    }

    void skill_Lich() {

    }

    void skill_Silencer() {

    }

    void skill_Berserker() {


    }

    void skill_ElfArcher() {


    }

    void skill_Cleric() {


    }

    void skill_Purifier() {


    }

    void skill_LandGuardian() {


    }

    void skill_Rogue() {


    }

    void skill_DeathAlchemist() {


    }

    void skill_LordOfTime() {


    }

    void skill_MasterOfCircus() {


    }

    void resetHeroSkill() {
        Instance = this;
    }
}
