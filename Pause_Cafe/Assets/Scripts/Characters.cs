using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexas;


// ##################################################################################################################################################
// Characters
// Author : ?
// Edited by L3Q1, VALAT Thibault and GOUVEIA Klaus
// Commented by L3Q1, VALAT Thibault
// ##################################################################################################################################################

namespace Characters
{

    //Edited by L3C1 CROUZET Oriane
    public enum CharClass : byte { GUERRIER, VOLEUR, ARCHER, MAGE, SOIGNEUR, ENVOUTEUR, VALKYRIE, DRUIDE };

    //New enum RoleCharacter
    //Author : L3C1 LOUIS DERIZA Socrate
    //Edited by L3C1 CROUZET Oriane
    public enum RoleCharacter : byte { TANKS, SUPPORTS, ASSASSINS, COMBATTANTS, MAGICIENS};
    //Author : ?
    //Edited by L3Q1, VALAT Thibault
    public class CharsDB
    {
        ///Edited by L3C1 CROUZET Oriane, MEDILEH Youcef
        // new AttackEffect DOOM, SHIELD, HEALTH_STEALING, MASSIVE_HEAL, RANDOM_DAMAGE, FREEZE
        public enum AttackEffect : byte
        {
            DAMAGE, RANDOM_DAMAGE, HEAL, MASSIVE_HEAL, PA_BUFF, DMG_BUFF, STUN, DOOM, SHIELD, HEALTH_STEALING, FREEZE, DAMAGE_OVER_TIME,
            LIGHNING
        }
        // Attack 
        public class Attack
        {
            public int range;
            public int rangeAoE;
            public bool targetsEnemies;
            public bool targetsAllies;
            public bool targetsSelf;
            public AttackEffect attackEffect;
            public int effectValue;

            //Initialize an attack
            public Attack(int range, int rangeAoE, bool targetsEnemies, bool targetsAllies, bool targetsSelf, AttackEffect attackEffect, int effectValue)
            {
                this.range = range;
                this.rangeAoE = rangeAoE;
                this.targetsEnemies = targetsEnemies;
                this.targetsAllies = targetsAllies;
                this.targetsSelf = targetsSelf;
                this.attackEffect = attackEffect;
                this.effectValue = effectValue;
            }
        }
        // Char base stats per class and attacks
        public class CharacterDB
        {
            public int maxHP;
            public int basePA;
            public int basePM;
            public int basePriority;
            public Attack basicAttack;
            public Attack skill_1;
            //2 skills per class
            ///Edited by L3C1 MEDILEH Youcef
            public Attack skill_2;

            // base stats list
            public CharacterDB(int maxHP, int basePA, int basePM, int priority, Attack basicAttack, Attack skill_1, Attack skill_2)
            {
                this.maxHP = maxHP;
                this.basePA = basePA;
                this.basePM = basePM;
                this.basePriority = priority;
                this.basicAttack = basicAttack;
                this.skill_1 = skill_1;
                //2 skills per class
                ///Edited by L3C1 MEDILEH Youcef
                this.skill_2 = skill_2;
            }
        }
        public static List<CharacterDB> list;

        //Initialize the characters for the game
        //edited by L3Q1, GOUVEIA Klaus and VALAT Thibault
        ///Edited by L3C1 CROUZET Oriane, MEDILEH Youcef
        //Author : ?
        public static void initCharsDB()
        {
            list = new List<CharacterDB>();
            //int range, int rangeAoE, bool targetsEnemies, bool targetsAllies, bool targetsSelf, AttackEffect attackEffect, int effectValue
            list.Add(new CharacterDB(17, 2, 3, 7, new Attack(1, 0, true, false, false, AttackEffect.DAMAGE, 3), new Attack(1, 0, true, false, false, AttackEffect.DAMAGE, 5), new Attack(1, 1, true, false, false, AttackEffect.RANDOM_DAMAGE, 3))); // GUERRIER
            list.Add(new CharacterDB(11, 3, 3, 4, new Attack(1, 0, true, false, false, AttackEffect.DAMAGE, 1), new Attack(2, 0, true, false, false, AttackEffect.DAMAGE, 3), new Attack(1, 0, true, false, false, AttackEffect.HEALTH_STEALING, 2))); // VOLEUR
            list.Add(new CharacterDB(13, 2, 3, 5, new Attack(6, 0, true, false, false, AttackEffect.DAMAGE, 2), new Attack(8, 0, true, false, false, AttackEffect.DAMAGE, 4), new Attack(15, 7, true, false, false, AttackEffect.DAMAGE, 1))); // ARCHER
            list.Add(new CharacterDB(10, 2, 3, 5, new Attack(3, 1, true, false, false, AttackEffect.DAMAGE, 2), new Attack(5, 2, true, false, false, AttackEffect.DAMAGE, 3), new Attack(5, 2, true, false, false, AttackEffect.DAMAGE_OVER_TIME, 1))); // MAGE
            list.Add(new CharacterDB(13, 2, 3, 6, new Attack(4, 0, false, true, false, AttackEffect.HEAL, 3)  , new Attack(5, 1, false, true, false, AttackEffect.HEAL, 3), new Attack(1, 0, false, true, true, AttackEffect.MASSIVE_HEAL, 1))); // SOIGNEUR
            list.Add(new CharacterDB(11, 2, 3, 6, new Attack(4, 0, false, true, false, AttackEffect.DMG_BUFF,1),new Attack(4, 2, false, true, false, AttackEffect.PA_BUFF, 1), new Attack(1, 1, true, false, false, AttackEffect.DOOM, 2))); // ENVOUTEUR
            list.Add(new CharacterDB(14, 2, 4, 4, new Attack(1, 1, true, false, false, AttackEffect.DAMAGE, 2), new Attack(2, 1, true, false, false, AttackEffect.DAMAGE, 3), new Attack(3, 1, true, false, false, AttackEffect.LIGHNING, 4))); // VALKYRIE		
            list.Add(new CharacterDB(11, 2, 3, 4, new Attack(2, 0, true, false, false, AttackEffect.DAMAGE, 1), new Attack(3, 0, true, false, false, AttackEffect.DAMAGE, 4), new Attack(1, 0, true, false, false, AttackEffect.FREEZE, 0))); // DRUIDE
        }
    }

    //Class character, to instatiate characters
    //Author : ?
    //Edited by L3Q1, Valat Thibault and Klaus Gouveia
    public class Character : System.IEquatable<Character>, System.IComparable<Character>
    {
        public static GameObject characterTemplate;
        public static List<GameObject> characterTemplateModels;
        public static GameObject charactersFolder;
        public static Color TEAM_1_COLOR = new Color(0.125f, 0.125f, 1);
        public static Color TEAM_2_COLOR = new Color(1, 0.125f, 0);
        public int totalDamage;
        public CharClass charClass;
        //new roleCharacter
        public RoleCharacter roleCharacter;
        public int team;
        public int HPmax;
        public int HP;
        public int characterPA;
        public int PA;
        public int PM;
        public int priority;
        public int basePriority;
        public int x;
        public int y;
        public bool skillAvailable;
        //new roleCharacter
        ///Edited by L3C1 CROUZET Oriane
        public bool skillAvailable2;
        public bool doomed;
        public bool freezed;
        public bool dmgbuff;
        public HexaDirection directionFacing;
        public GameObject go;

        //Constructor
        //Author : ?
        //Edited by L3Q1, Klaus GOUVEIA and VALAT Thibault
        public Character(CharClass charClass, RoleCharacter roleCharacter, int x, int y, int team)
        {
            this.charClass = charClass;
            this.roleCharacter = roleCharacter;
            CharsDB.CharacterDB myCharClass = CharsDB.list[(int)charClass];
            HPmax = myCharClass.maxHP; HP = HPmax;
            PA = myCharClass.basePA;
            characterPA = myCharClass.basePA;
            PM = myCharClass.basePM;
            priority = myCharClass.basePriority;
            basePriority = myCharClass.basePriority;
            totalDamage = 0;
            this.x = x;
            this.y = y;
            this.team = team;
            this.skillAvailable = true;
            this.skillAvailable2 = true;
            //Edited by CROUZET Oriane, 15/03/2023
            this.doomed = false;
            this.freezed = false;
            this.dmgbuff = false;

            this.go = GameObject.Instantiate(characterTemplate, charactersFolder.transform);
            this.go.SetActive(true);
            this.go.transform.position = Hexa.hexaPosToReal(x, y, 0);
            this.go.GetComponent<CharacterGO>().character = this;
            this.setColorByTeam();
            this.setDirection(HexaDirection.DOWN);
        }

        // No GameObject (console mode)
        ///Edited by L3C1 CROUZET Oriane
        public Character(CharClass charClass, RoleCharacter roleCharacter, int x, int y, int team, bool a)
        {
            this.charClass = charClass;
            this.roleCharacter = roleCharacter;
            CharsDB.CharacterDB myCharClass = CharsDB.list[(int)charClass];
            HPmax = myCharClass.maxHP; HP = HPmax;
            PA = myCharClass.basePA;

            PM = myCharClass.basePM;
            priority = myCharClass.basePriority;
            basePriority = myCharClass.basePriority;

            this.x = x;
            this.y = y;
            this.team = team;
            this.skillAvailable = true;
            this.skillAvailable2 = true;
            //Edited by L3C1 CROUZET Oriane
            this.doomed = false;
            this.freezed = false;

            this.go = null;
        }

        //Override of the equals method
        // /!\ Needed to sort characters 
        //Author : VALAT Thibault
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Character objAsCharacter = obj as Character;
            if (objAsCharacter == null) return false;
            else return Equals(objAsCharacter);
        }

        //Decide how to sort characters : according to the priority
        // /!\ Needed to sort characters
        //Author : VALAT Thibault
        public int CompareTo(Character compareChar)
        {
            if (compareChar == null)
                return 1;
            else
                return this.priority.CompareTo(compareChar.priority);
        }

        //Return the attribut to sort the characters
        // /!\ Needed to sort characters
        //Author : VALAT Thibault
        public override int GetHashCode()
        {
            return priority;
        }

        //To compare two characters
        // /!\ Needed to sort characters
        //Author : VALAT Thibault
        public bool Equals(Character other)
        {
            if (other == null) return false;
            return (this.priority.Equals(other.priority));
        }

        //Reset the priority to its basic value 
        //Author : VALAT Thibault
        // Edited by Youcef MEDILEH
        // - debut reglage de la priorité des personnages
        public void resetPriority()
        {
            // Debug.Log("#### resetPriority " + this.getName() + " ####");
            // Debug.Log("# basePriority : " + basePriority + "#");
            // Debug.Log("# priority : " + priority + "#");
            // Debug.Log("###############################");
            if (basePriority == 4)
            {
                this.priority = (basePriority + UnityEngine.Random.Range(0, 2));
            }
            else if (basePriority >= 6)
            {
                this.priority = (basePriority - UnityEngine.Random.Range(0, 2));
            }
            else
            {
                this.priority = basePriority;
            }
        }

        //Update the position of the character on thr grid
        //Author : ?
        public void updatePos(int newX, int newY, HexaGrid hexaGrid)
        {
            hexaGrid.getHexa(x, y).charOn = null;
            x = newX;
            y = newY;
            hexaGrid.getHexa(x, y).charOn = this;
            this.go.transform.position = Hexa.hexaPosToReal(x, y, 0);
        }

        // Console mode
        public void updatePos2(int newX, int newY, HexaGrid hexaGrid)
        {
            hexaGrid.getHexa(x, y).charOn = null;
            x = newX;
            y = newY;
            hexaGrid.getHexa(x, y).charOn = this;
        }

        //Clear the position of the character on thr grid
        //Author : ?
        public void clearPos(HexaGrid hexaGrid)
        {
            hexaGrid.getHexa(x, y).charOn = null;
        }

        //x coordinate getter
        //Author : ?
        public int getX()
        {
            return x;
        }

        //y coordinate getter
        //Author : ?
        public int getY()
        {
            return y;
        }

        //total damage getter
        //Author : ?
        public int getTotalDamage()
        {
            return totalDamage;
        }

        //total damage setter
        //Author : ?
        public void setTotalDamage(int totalDamage)
        {
            this.totalDamage = totalDamage;
        }

        //Pa getter
        //Author : ?
        public int getPA()
        {
            return PA;
        }


        //Pa setter
        //Author : ?
        public void setPA(int PA)
        {
            this.PA = PA;
        }

        //Hp getter
        //Author : ?
        public int getHP()
        {
            return HP;
        }

        public int getHPmax()
        {
            return HPmax;
        }

        //Pa setter
        //Author : ?
        public void setHP(int HP)
        {
            this.HP = HP;
        }

        //Char class getter
        //Author : ?
        public CharClass getCharClass()
        {
            return charClass;
        }
        //RoleCharacter getter
        //Author : ?
        public RoleCharacter getRoleCharacter()
        {
            return roleCharacter;
        }
        //DamageBuff getter
        //Author : ?
        public bool getDamageBuff()
        {
            return dmgbuff;
        }

        //DamageBuff setter
        //Author : ?
        public void setDamageBuff(bool dmgbuff)
        {
            this.dmgbuff = dmgbuff;
        }

        //Damage getter
        //Author : ?
        public int getDamage()
        {
            if (dmgbuff == true) return CharsDB.list[(int)charClass].basicAttack.effectValue + 1;
            else return CharsDB.list[(int)charClass].basicAttack.effectValue;
        }

        //Range getter
        //Author : ?
        public int getRange()
        {
            return this.getClassData().basicAttack.range;
        }

        //Skill damage getter
        //Author : ?
        public int getSkillDamage()
        {
            if (dmgbuff == true) return CharsDB.list[(int)charClass].skill_1.effectValue + 1;
            else return CharsDB.list[(int)charClass].skill_1.effectValue;
        }

        ///Author : L3C1 MEDILEH Youcef
        public int getSkill2Damage()
        {
            if (dmgbuff == true) return CharsDB.list[(int)charClass].skill_2.effectValue + 1;
            else return CharsDB.list[(int)charClass].skill_2.effectValue;
        }


        //Skill range getter
        //Author : ?
        public int getSkillRange()
        {
            return this.getClassData().skill_1.range;
        }

        // AUTHOR : MEDILEH Youcef, group : L3C1
        public int getSkill2Range()
        {
            return this.getClassData().skill_2.range;
        }

        //Returns if the skill is ready or not
        //Author : ?
        public bool isSkill1Up()
        {
            return skillAvailable;
        }

        // AUTHOR : MEDILEH Youcef, group : L3C1
        public bool isSkill2Up()
        {
            return skillAvailable2;
        }

        // Author: CROUZET Oriane, group : L3C1
        // Date : 15/03/2023
        //Returns if the character is doomed or not
        public bool isDoomed()
        {
            return doomed;
        }
        // Author: CROUZET Oriane, group : L3C1
        // Date : 15/03/2023
        //Set the state of doomed
        public void setDoomed(bool doomed)
        {
            this.doomed = doomed;
            Debug.Log("Le perso est " + this.doomed);
        }
        // Author: CROUZET Oriane, group : L3C1
        // Date : 15/03/2023
        //Returns if the character is freezed or not
        public bool isFreezed()
        {
            return freezed;
        }
        // Author: CROUZET Oriane, group : L3C1
        // Date : 15/03/2023
        //Set the state of freezed
        public void setFreezed(bool freezed)
        {
            this.freezed = freezed;
        }

        //Set the color of the characters depending on the team
        //Author : ?
        public void setColorByTeam()
        {

            switch (team)
            {
                case 0: this.go.transform.GetChild(0).GetComponent<Renderer>().material.color = TEAM_1_COLOR; break;
                case 1: this.go.transform.GetChild(0).GetComponent<Renderer>().material.color = TEAM_2_COLOR; break;
                default: break;
            }
            GameObject.Instantiate(characterTemplateModels[(int)this.charClass], go.transform);
        }

        //Set the direction of the character
        //Author : ?
        public void setDirection(HexaDirection newDirection)
        {
            this.directionFacing = newDirection;
            Transform charModel = this.go.transform.GetChild(1);
            if (charModel) charModel.eulerAngles = new Vector3(0, (int)newDirection * 60, 0);
        }

        //Returns the name of the class of the character
        //Author : ?
        public string getName()
        {
            switch (this.charClass)
            {
                case CharClass.GUERRIER: return "Guerrier";
                case CharClass.VOLEUR: return "Voleur";
                case CharClass.ARCHER: return "Archer";
                case CharClass.MAGE: return "Mage";
                case CharClass.SOIGNEUR: return "Soigneur";
                case CharClass.ENVOUTEUR: return "Envouteur";
                case CharClass.VALKYRIE: return "Valkyrie";
                case CharClass.DRUIDE: return "Druide";
                default: return "None";
            }
        }
        //Returns the role of the class of the character
        ///Edited by L3C1 CROUZET Oriane, 19/04/2023
        public string getRole()
        {
            switch (this.roleCharacter)
            {
                case RoleCharacter.ASSASSINS: return "Assassin";
                case RoleCharacter.MAGICIENS: return "Magicien";
                case RoleCharacter.TANKS: return "Tank";
                case RoleCharacter.COMBATTANTS: return "Combattant";
                case RoleCharacter.SUPPORTS: return "Support";
                default: return "None";
            }
        }

        //Returns the data of the class of the character
        //Author : ?
        public CharsDB.CharacterDB getClassData()
        {
            return CharsDB.list[(int)charClass];
        }
    }

}
