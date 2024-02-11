using System;
using System.Collections.Generic;
using UnityEngine;
using Misc;
using Characters;
using Hexas;
using AI_Util;
using AI_Class;
using static UtilCPU.UtilCPU;
using static MainGame;
using static ScriptsCPU.ScriptsCPU;

namespace CharactersCPU
{
    public abstract class CharacterCPU
    {
        // List of charCPUs created
        public static List<CharacterCPU> charCPUList;
        // Variable where the character and its relevant info is stored
        protected RelevantCharacterInfoWrapper infoWrapper;
        // The game board
        protected HexaGrid hexas;

        public CharacterCPU(Character character, HexaGrid hexas)
        {
            infoWrapper = new RelevantCharacterInfoWrapper(character);

            this.hexas = hexas;

            charCPUList.Add(this);
        }

        

        public Character Character { get => infoWrapper.Character; }

        public int HP { get => infoWrapper.HP; set => infoWrapper.HP = value; }
        public int TotalDamage { get => infoWrapper.TotalDamage; set => infoWrapper.TotalDamage = value; }
        public int X { get => infoWrapper.X; set => infoWrapper.X = value; }
        public int Y { get => infoWrapper.Y; set => infoWrapper.Y = value; }

        public bool SkillAvailable { get => infoWrapper.SkillAvailable; set => infoWrapper.SkillAvailable = value; }

        public bool SkillAvailable2 { get => infoWrapper.SkillAvailable2; set => infoWrapper.SkillAvailable2 = value; }

        public bool Freezed { get => infoWrapper.Freezed; set => infoWrapper.Freezed = value; }

        public bool Spelled { get => infoWrapper.Spelled; set => infoWrapper.Spelled = value; }

        public bool Dmgbuff { get => infoWrapper.Dmgbuff; set => infoWrapper.Dmgbuff = value; }

        public CharsDB.CharacterDB ClassData { get => infoWrapper.Character.getClassData(); }

        public Character TempChar { get => new Character(Character.charClass, Character.roleCharacter ,X, Y, Character.team, true); }

        /// <summary>
        /// This character will move up to <paramref name="target"/> and attack them once they are at range. 
        /// Uses skills or not depending on the parameter <paramref name="allowedToUseSkill"/>.
        /// </summary>
        /// <remarks>
        /// For coming up on other characters without attacking them, use <see cref="moveTowards(CharacterCPU)"/>.
        /// </remarks>
        /// <param name="target">The target character</param>
        /// <param name="allowedToUseSkill">If true, this character will use skills whenever they can, 
        /// and if false, they will not use any skills</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public virtual (Character, ActionAIPos) target(CharacterCPU target, bool allowedToUseSkill)
        {
            if (allowedToUseSkill && SkillAvailable && target.HP > ClassData.basicAttack.effectValue + (Dmgbuff ? 1 : 0))
            // Skill use allowed, available & the target can't go down with just a basic attack
            {
                Debug.Log("allowedToUseSkill && SkillAvailable && target.HP > ClassData.basicAttack.effectValue  == true");
                if (isInRangeToUseSkill(this, target))
                {
                    /*
                    SkillAvailable = false;
                    TotalDamage -= 10;
                    
                    target.HP -= Character.getSkillDamage() + (Dmgbuff ? 1 : 0);
                    TotalDamage += Character.getSkillDamage() + (Dmgbuff ? 1 : 0);

                    return (Character, new ActionAIPos(ActionType.ATK2, new Point(target.X, target.Y)));
                    //*/
                    //Added by Socrate Louis Deriza
                    if (SkillAvailable2 == true)
                    {
                        Debug.Log("Skill 2 avaible");
                        if (isInRangeToUseSkill2(this, target))
                        {
                            if (Character.getName() == "Soigneur")
                            {
                                if ((target.HP < target.Character.getHPmax() && (target.Spelled == true || target.Freezed == true)))
                                {
                                    Debug.Log("Un soigneur de l'équipe " + Character.team + " considère qu'un allié à portée de sa compétence 2 est en mauvaise posture...");
                                    Debug.Log("Utilisation de la compétence 2");
                                    return (Character, new ActionAIPos(ActionType.ATK3, new Point(this.X, this.Y)));
                                }
                                else
                                {

                                    if (target.HP < (0.5 * target.Character.getHPmax()))
                                    {
                                        Debug.Log("Un soigneur de l'équipe " + Character.team + " considère qu'un allié à portée de sa compétence 2 est en mauvaise posture...");
                                        Debug.Log("Utilisation de la compétence 2");
                                        return (Character, new ActionAIPos(ActionType.ATK3, new Point(this.X, this.Y)));
                                    }

                                    if (this.Character.HP < (0.75 * this.Character.getHPmax()))
                                    {
                                        Debug.Log("Un soigneur de l'équipe " + Character.team + " utilise sa 2nd compétence afin de regagner de la vie");
                                        return (Character, new ActionAIPos(ActionType.ATK3, new Point(this.X, this.Y)));
                                    }


                                    if (target.HP <= (target.Character.getHPmax() - 3))
                                    {
                                        Debug.Log("Un soigneur de l'équipe " + Character.team + " considère qu'un allié à portée de sa compétence 1 à perdu au 3 points de vie...");
                                        Debug.Log("Utilisation de la compétence 1");
                                        return (Character, new ActionAIPos(ActionType.ATK2, planForAttack(target, ActionType.ATK2)));
                                    }
                                }
                            }
                            else
                            {
                                return (Character, new ActionAIPos(ActionType.ATK3, planForAttack(target, ActionType.ATK3)));
                            }
                        }
                        else
                        {
                            if (Character.getName() == "Soigneur")
                            {
                                if ((target.HP < target.Character.getHPmax() && target.Spelled == true) || (target.HP < target.Character.getHPmax() &&  target.Freezed == true) || (target.HP < (0.5 * target.Character.getHPmax())))
                                {
                                    Debug.Log("Un soigneur de l'équipe " + Character.team + " considère qu'un allié est en très mauvaise posture...");
                                    Debug.Log("Le soigneur s'approche de lui afin d'utiliser sa compétence 2 par la suite");
                                    return moveTowardsSkill_2(target.Character);
                                }
                                if(this.Character.HP < (0.75 * this.Character.getHPmax()))
                                {
                                    Debug.Log("Un soigneur de l'équipe " + Character.team + " utilise sa 2nd compétence afin de regagner de la vie");
                                    return (Character, new ActionAIPos(ActionType.ATK3, new Point(this.X, this.Y)));
                                }
                                Debug.Log("out of  range skill_2");

                            }
                        }


                        if (Character.getName() != "Soigneur")
                        {
                            Debug.Log("Use skill_1");
                            return (Character, new ActionAIPos(ActionType.ATK2, planForAttack(target, ActionType.ATK2)));
                        }
                    }

                }
            }
            if (isInRangeToAttack(this, target))
            {
                Debug.Log("Here!");
                /*
                target.HP -= Character.getDamage() + (Dmgbuff ? 1 : 0);
                TotalDamage += Character.getDamage() + (Dmgbuff ? 1 : 0);

                return (Character, new ActionAIPos(ActionType.ATK1, new Point(target.X, target.Y)));
                //*/
                return (Character, new ActionAIPos(ActionType.ATK1, planForAttack(target, ActionType.ATK1)));
            }

            return moveTowards(target);
        }

        /// <summary>
        /// Returns a point indicating where to attack to hit the highest amount of enemy units.
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="action">The attack (basic attack or skill)</param>
        /// <returns>A position</returns>
        public Point planForAttack(CharacterCPU target, ActionType action)
        {
            CharsDB.Attack attack = null;

            // Basic attack or skill
            switch (action)
            {
                case ActionType.ATK1:
                    attack = this.Character.getClassData().basicAttack;
                    break;
                case ActionType.ATK2:
                    SkillAvailable = false;
                    TotalDamage -= 10;

                    attack = this.Character.getClassData().skill_1;
                    break;
                case ActionType.ATK3:
                    SkillAvailable2 = false;
                    TotalDamage -= 10;

                    attack = this.Character.getClassData().skill_2;
                    break;
                default:
                    Debug.Log("Default case called in planForAOE");

                    return new Point(target.X, target.Y);
            }

            // Has AOE
            if (attack.rangeAoE > 0)
            {
                Hexa toAttack;
                toAttack = takeAOEIntoAccount(this.TempChar, target.TempChar, attack, action);
                if (toAttack == null)
                {
                    Debug.Log("PlanForAttack: no enemy targets in range");
                    //return null;
                }
                Point toReturn = new Point(toAttack.x, toAttack.y);
                //Edited By Socrate Louis Deriza
                for (int i = 0; i < charCPUList.Count; i++)
                    if (hexas.getDistance(toReturn.x, toReturn.y, charCPUList[i].X, charCPUList[i].Y) <= attack.rangeAoE)
                        switch (attack.attackEffect)
                        {
                            case CharsDB.AttackEffect.DAMAGE:
                                target.HP -= attack.effectValue + (Dmgbuff ? 1 : 0);
                                TotalDamage += attack.effectValue + (Dmgbuff ? 1 : 0);
                                break;
                            case CharsDB.AttackEffect.HEAL:
                                target.HP += Math.Min(attack.effectValue, target.ClassData.maxHP);
                                TotalDamage += attack.effectValue + (Dmgbuff ? 1 : 0);
                                break;
                            case CharsDB.AttackEffect.DMG_BUFF:
                                target.Dmgbuff = true;
                                Debug.Log("CharacterCpu.planForAttack attack -->getAoE Dmg_Buff");
                                break;
                            case CharsDB.AttackEffect.FREEZE:
                                target.Freezed = true;
                                Debug.Log("CharacterCpu.planForAttack attack -->getAoE case Freeze");
                                break;
                            case CharsDB.AttackEffect.RANDOM_DAMAGE:
                                System.Random rnd = new();
                                int randomNumberDamage = rnd.Next(1, 5);
                                target.HP -= randomNumberDamage + (Dmgbuff ? 1 : 0);
                                TotalDamage += randomNumberDamage + (Dmgbuff ? 1 : 0);
                                break;
                            case CharsDB.AttackEffect.HEALTH_STEALING:
                                target.HP -= attack.effectValue ;
                                if (HP + attack.effectValue > Character.getHPmax())
                                {
                                    HP = Character.getHPmax();
                                }
                                else
                                {
                                    HP += attack.effectValue;
                                }
                                break;
                            case CharsDB.AttackEffect.PA_BUFF:
                                Debug.Log("CharacterCpu.planForAttack attack -->getAoE case PA_Buff");
                                break;
                            case CharsDB.AttackEffect.SHIELD:
                                Debug.Log("CharacterCpu.planForAttack attack -->getAoE case Shield");
                                break;
                            case CharsDB.AttackEffect.DOOM:
                                target.Spelled = true;
                                Debug.Log("CharacterCpu.planForAttack attack -->getAoE case Spell");
                                break;
                            case CharsDB.AttackEffect.MASSIVE_HEAL:
                                target.HP += Math.Min(attack.effectValue, target.ClassData.maxHP);
                                TotalDamage += attack.effectValue + (Dmgbuff ? 1 : 0);
                                Debug.Log("CharacterCpu.planForAttack attack -->getAoE case Massive heal");
                                break;
                            default:
                                Debug.Log("Default case called in effectValue calculations");
                                break;
                        }

                if (TotalDamage >= 10)
                {
                    TotalDamage -= 10;
                    SkillAvailable = true;
                    SkillAvailable = true;
                }

                return toReturn;
            }
            // No AOE
            //Edited by Socrate Louis Deriza
            else
            {
                Debug.Log("PlanForAttack: noAOE");
                switch (attack.attackEffect)
                {
                    case CharsDB.AttackEffect.DAMAGE:
                        target.HP -= attack.effectValue + (Dmgbuff ? 1 : 0);
                        TotalDamage += attack.effectValue + (Dmgbuff ? 1 : 0);
                        break;
                    case CharsDB.AttackEffect.HEAL:
                        target.HP += Math.Min(attack.effectValue, target.ClassData.maxHP);
                        TotalDamage += attack.effectValue + (Dmgbuff ? 1 : 0);
                        break;
                    case CharsDB.AttackEffect.DMG_BUFF:
                        target.Dmgbuff = true;
                        break;

                    case CharsDB.AttackEffect.FREEZE:
                        target.Freezed = true;
                        Debug.Log("CharacterCpu.planForAttack attack -->NoAoE case Freeze");
                        break;
                    case CharsDB.AttackEffect.RANDOM_DAMAGE:
                        System.Random rnd = new();
                        int randomNumberDamage = rnd.Next(1, 5);
                        target.HP -= randomNumberDamage + (Dmgbuff ? 1 : 0);
                        TotalDamage += randomNumberDamage + (Dmgbuff ? 1 : 0);
                        break;
                    case CharsDB.AttackEffect.HEALTH_STEALING:
                        target.HP -= attack.effectValue;
                        if (HP + attack.effectValue > Character.getHPmax())
                        {
                            HP = Character.getHPmax();
                        }
                        else
                        {
                            HP += attack.effectValue;
                        }
                        break;
                    case CharsDB.AttackEffect.PA_BUFF:
                        Debug.Log("CharacterCpu.planForAttack attack -->NoAoE case PA_Buff");
                        break;
                    case CharsDB.AttackEffect.SHIELD:
                        Debug.Log("CharacterCpu.planForAttack attack -->NoAoE case Shield");
                        break;
                    case CharsDB.AttackEffect.DOOM:
                        target.Spelled = true;
                        Debug.Log("CharacterCpu.planForAttack attack -->NoAoE case Spell");
                        break;
                    case CharsDB.AttackEffect.MASSIVE_HEAL:
                        target.HP += Math.Min(attack.effectValue, target.ClassData.maxHP);
                        TotalDamage += attack.effectValue + (Dmgbuff ? 1 : 0);
                        Debug.Log("CharacterCpu.planForAttack attack -->NoAoE case Massive heal");
                        break;

                    default:
                        Debug.Log("Default case called in effectValue calculations");
                        break;
                }

                if (TotalDamage >= 10)
                {
                    TotalDamage -= 10;
                    SkillAvailable = true;
                    SkillAvailable2 = true;
                }

                return new Point(target.X, target.Y);
            }
        }

        /// <summary>
        /// This character will move up to <paramref name="target"/> and attack them once they are at range.
        /// </summary>
        /// <remarks>
        /// For coming up on other characters without attacking them, use <see cref="moveTowards(CharacterCPU)"/>.
        /// </remarks>
        /// <param name="target">The target character</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public virtual (Character, ActionAIPos) target(CharacterCPU target)
        {
            return this.target(target, true);
        }

        /// <summary>
        /// The character defined in this class will move towards the point passed in the parameter.
        /// </summary>
        /// <remarks>
        /// If the hexa space located at point is freed up, the character will just go to it.
        /// Otherwise, they will attempt to position themselves at a nearby hexa.
        /// </remarks>
        /// <param name="point">The location this unit will move towards</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public (Character, ActionAIPos) moveTowards(Point point)
        {
            // Indicated point is the character's position
            if (X == point.x && Y == point.y)
                return (Character, new ActionAIPos(ActionType.SKIP, null));

            List<ActionAIPos> actions = new List<ActionAIPos>();
            Hexa pointHexa = hexas.getHexa(point.x, point.y);
            bool willBeOccupied = false;

            // Check to see if another CharacterCPU is planning to move on that point
            foreach (CharacterCPU cpu in charCPUList)
                willBeOccupied = willBeOccupied || (cpu.X == point.x && cpu.Y == point.y);

            // Can just walk on the target hexa
            if (pointHexa != null && hexas.canWalk(pointHexa) && pointHexa.charOn == null && !willBeOccupied)
                actions.AddRange(findSequencePathToHexaCPU(TempChar, point.x, point.y));
            // Target hexa is occupied or a wall
            else
            {
                List<Point> targetArea = pointsFromClosestToFarthest(point, this);
                int i = targetArea.Count - 1; // Going through targetArea in reverse (inner rings first)

                while (actions.Count == 0 && i >= 0)
                {
                    Point p = targetArea[i];
                    Hexa goTo = hexas.getHexa(p.x, p.y);
                    willBeOccupied = false;

                    foreach (CharacterCPU cpu in charCPUList)
                        willBeOccupied = willBeOccupied || (cpu.X == p.x && cpu.Y == p.y);

                    if (goTo != null && hexas.canWalk(goTo) && goTo.charOn == null && !willBeOccupied)
                        actions.AddRange(findSequencePathToHexaCPU(TempChar, goTo.x, goTo.y));

                    i--;
                }
            }

            if (actions.Count == 0)
                return (Character, new ActionAIPos(ActionType.SKIP, null));

            X = actions[0].pos.x;
            Y = actions[0].pos.y;
            return (Character, actions[0]);
        }
        //Author VALAT Thibault L3Q1


        /// <summary>
        /// The character defined in this class will move towards the character passed in the parameter.
        /// </summary>
        /// <remarks>
        /// This method will attempt to position the character in such a way that
        /// the target's distance to them is equal to their attack range.
        /// </remarks>
        /// <param name="target">The character this unit will move towards</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public virtual (Character, ActionAIPos) moveTowards(CharacterCPU target)
        {
            List<ActionAIPos> actions = new List<ActionAIPos>();
            List<Point> targetArea = pointsFromClosestToFarthest(target, this,
                SkillAvailable ?
                Character.getSkillRange() :
                Character.getRange());
            bool willBeOccupied;

            int j = 0;

            while (actions.Count == 0 && j < targetArea.Count)
            {
                Point p = targetArea[j];
                Hexa goTo = hexas.getHexa(p.x, p.y);
                willBeOccupied = false;

                // Check to see if another CharacterCPU is planning to move on that point
                foreach (CharacterCPU cpu in charCPUList)
                    willBeOccupied = willBeOccupied || (cpu.X == p.x && cpu.Y == p.y);

                //Debug.Log("To : " + p.x + " " + p.y + ", " + goTo.type + ", " + charToString(goTo.charOn));

                if (goTo != null && hexas.canWalk(goTo) && goTo.charOn == null && !willBeOccupied)
                    actions.AddRange(findSequencePathToHexaCPU(TempChar, goTo.x, goTo.y));

                j++;
            }

            if (actions.Count == 0)
                return (Character, new ActionAIPos(ActionType.SKIP, null));

            X = actions[0].pos.x;
            Y = actions[0].pos.y;
            return (Character, actions[0]);
        }

        /// <summary>
        /// The character defined in this class will move towards the character passed in the parameter.
        /// </summary>
        /// <remarks>
        /// This method will attempt to position the character in such a way that
        /// the target's distance to them is equal to their attack range.
        /// </remarks>
        /// <param name="target">The character this unit will move towards</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public virtual (Character, ActionAIPos) moveTowards(Character target)
        {
            List<ActionAIPos> actions = new List<ActionAIPos>();
            List<Point> targetArea = pointsFromClosestToFarthest(target, this.TempChar,
                Character.isSkill1Up() ?
                Character.getSkillRange() :
                Character.getRange());

            int j = 0;

            while (actions.Count == 0 && j < targetArea.Count)
            {
                Point p = targetArea[j];
                Hexa goTo = hexas.getHexa(p.x, p.y);

                //Debug.Log("To : " + p.x + " " + p.y + ", " + goTo.type + ", " + charToString(goTo.charOn));

                if (goTo != null && hexas.canWalk(goTo) && goTo.charOn == null)
                    actions.AddRange(findSequencePathToHexaCPU(Character, goTo.x, goTo.y));

                j++;
            }

            if (actions.Count == 0)
                return (Character, new ActionAIPos(ActionType.SKIP, null));
            return (Character, actions[0]);
        }

        //Added by Socrate Louis Deriza, L3C1
        public virtual (Character, ActionAIPos) moveTowardsSkill_2(Character target)
        {
            List<ActionAIPos> actions = new List<ActionAIPos>();
            List<Point> targetArea = pointsFromClosestToFarthest(target, this.TempChar, Character.getSkill2Range());

            int j = 0;

            while (actions.Count == 0 && j < targetArea.Count)
            {
                Point p = targetArea[j];
                Hexa goTo = hexas.getHexa(p.x, p.y);

                //Debug.Log("To : " + p.x + " " + p.y + ", " + goTo.type + ", " + charToString(goTo.charOn));

                if (goTo != null && hexas.canWalk(goTo) && goTo.charOn == null)
                    actions.AddRange(findSequencePathToHexaCPU(Character, goTo.x, goTo.y));

                j++;
            }

            if (actions.Count == 0)
                return (Character, new ActionAIPos(ActionType.SKIP, null));
            return (Character, actions[0]);
        }

        /// <summary>
        /// Targets a character given as a parameter.
        /// Edited Socrate Louis Deriza
        /// </summary>
        /// <param name="target">The target character</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public virtual (Character, ActionAIPos) target(Character target)
        {

            if (this.Character.getName() == "Mage")
            {
                return moveTowards(target);
            }
            
            /*if (this.Character.getName() == "Mage")
            {
                if (this.SkillAvailable)
                {
                    if (isInRangeToUseSkill(this.Character, target))
                    {
                        this.SkillAvailable = false;
                        return (Character, new ActionAIPos(ActionType.ATK2, new Point(target.x, target.y)));
                    }
                }
                if (this.SkillAvailable2)
                {
                    if (isInRangeToUseSkill2(this.Character, target))
                    {
                        this.SkillAvailable2 = false;
                        return (Character, new ActionAIPos(ActionType.ATK3, new Point(target.x, target.y)));
                    }
                }
                if (isInRangeToAttack(Character, target))
                    return (Character, new ActionAIPos(ActionType.ATK1, new Point(target.x, target.y)));
                return moveTowards(target);
            }
            */
            if (this.SkillAvailable && target.HP > this.ClassData.basicAttack.effectValue)
            // Skill possible et cible avec plus de PV que les dégâts d'une attaque simple
            {  // AIUtil.isCharWithinRangeSkill(hexas.charList.IndexOf(character), hexas.charList.IndexOf(target))
                if (isInRangeToUseSkill(Character, target)){
                    this.SkillAvailable = false;
                    return (Character, new ActionAIPos(ActionType.ATK2, new Point(target.x, target.y)));
                }
            }


            if (this.SkillAvailable2)
            {
                // Skill2 possible et cible avec plus de PV que le minimum de dégâts possible d'une attaque aléatoire
                if (target.HP > 2)
                {
                    if (isInRangeToUseSkill2(Character, target))
                    {
                        if (this.Character.getName() == "Guerrier" && this.SkillAvailable == false)
                        {
                            this.SkillAvailable2 = false;
                            Debug.Log("Cpu --> Utilisation compétence 2 guerrier");
                            return (Character, new ActionAIPos(ActionType.ATK3, new Point(target.x, target.y)));
                        }
                        
                        if (this.Character.getName() == "Voleur")
                        {
                            if ((Character.getHP() < 0.75 * Character.getHPmax()) && (target.getHP() < target.getHPmax()))
                            {
                                this.SkillAvailable2 = false;
                                Debug.Log("Le voleur, très prudent, se considère mal en point (a perdu au moins 25% de de sa vie) et considère sa cible \'" + target.getName() + "\' de l'équipe adverse comme étant attaquable (a perdu au moins 2 points de vie)");
                                Debug.Log("Cpu --> Utilisation compétence 2 voleur");
                                return (Character, new ActionAIPos(ActionType.ATK3, new Point(target.x, target.y)));
                            }
                        }
                    }
                }

                if (this.Character.getName() == "Valkyrie" )
                {
                    if (target.HP > 4)
                    {
                        if (target.HP >= this.Character.getHP())
                        {
                            if (isInRangeToUseSkill2(this.Character, target))
                            {
                                this.SkillAvailable2 = false;
                                Debug.Log("La valkyrie,  constate que sa cible \'" + target.getName() + "\' de l'équipe adverse possède plus (ou =) de point de qu'elle...");
                                Debug.Log("Cpu --> Utilisation compétence 2 Valkyrie");
                                return (Character, new ActionAIPos(ActionType.ATK3, new Point(target.x, target.y)));
                            }
                            
                        }
                        
                    }
                        
                }

                if (this.Character.getName() == "Archer")
                {
                    //If half or more of the opposing Character do not have their maximum Healf point use skill_2
                    int numberOfEnnemies = 0;
                    int countNotHpMaxCharacter = 0;

                    foreach (Character charact in AIUtil.hexaGrid.charList)
                    {
                        if (charact.team != this.Character.team)
                        {
                            numberOfEnnemies++;
                            if (charact.getHP() < charact.getHPmax())
                            {
                                countNotHpMaxCharacter++;
                            }
                        }
                    }
                    int b = 2;
                    float res = numberOfEnnemies / b;
                    int half = Mathf.FloorToInt(res);

                    if (half <= countNotHpMaxCharacter)
                    {
                        this.SkillAvailable2 = false;
                        Debug.Log("Cpu --> Utilisation compétence 2 Archer");
                        return (Character, new ActionAIPos(ActionType.ATK3, new Point(Character.getX(), Character.getY())));
                    }
                }

            }              
            // AIUtil.isCharWithinRangeAttack(hexas.charList.IndexOf(character), hexas.charList.IndexOf(target))
            if (isInRangeToAttack(Character, target))
                return (Character, new ActionAIPos(ActionType.ATK1, new Point(target.x, target.y)));

            return moveTowards(target);
        }

        /// <summary>
        /// Returns a turn skip.
        /// </summary>
        /// <returns>A turn skip</returns>
        public (Character, ActionAIPos) wait()
        {
            return (Character, new ActionAIPos(ActionType.SKIP, null));
        }
    }

    public class WarriorCPU : CharacterCPU
    {
        public WarriorCPU(Character character, HexaGrid hexas)
            : base(character, hexas) { }
    }

    public class ThiefCPU : CharacterCPU
    {
        public ThiefCPU(Character character, HexaGrid hexas)
            : base(character, hexas) { }
    }

    public class MageCPU : CharacterCPU
    {
        public MageCPU(Character character, HexaGrid hexas)
            : base(character, hexas) { }
    }

    public class ValkyrieCPU : CharacterCPU
    {
        public ValkyrieCPU(Character character, HexaGrid hexas)
            : base(character, hexas) { }
    }

    public class DruideCPU : CharacterCPU
    {
        public DruideCPU(Character character, HexaGrid hexas)
            : base(character, hexas) { }
    }

    //Addedd by L3C1 Louis Deriza Socrate

    public class ArcherCPU : CharacterCPU
    {
        public ArcherCPU(Character character, HexaGrid hexas)
            : base(character, hexas) { }

        /// <summary>
        /// The character defined in this class will move towards the character passed in the parameter.
        /// </summary>
        /// <remarks>
        /// This method will attempt to position the character in such a way that
        /// the target's distance to them is equal to their attack range.
        /// </remarks>
        /// <param name="target">The character this unit will move towards</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public override (Character, ActionAIPos) moveTowards(CharacterCPU target)
        {
            List<ActionAIPos> actions = new List<ActionAIPos>();
            List<Point> targetArea = pointsFromClosestToFarthest(target, this,
                SkillAvailable ?
                Character.getSkillRange() :
                Character.getRange());
            bool willBeOccupied;

            int j = 0;

            while (actions.Count == 0 && j < targetArea.Count)
            {
                Point p = targetArea[j];
                Hexa goTo = hexas.getHexa(p.x, p.y);
                willBeOccupied = false;

                // Check to see if another CharacterCPU is planning to move on that point
                foreach (CharacterCPU cpu in charCPUList)
                    willBeOccupied = willBeOccupied || (cpu.X == p.x && cpu.Y == p.y);

                //Debug.Log("To : " + p.x + " " + p.y + ", " + goTo.type + ", " + charToString(goTo.charOn));

                if (goTo != null && hexas.canWalk(goTo) && goTo.charOn == null && !willBeOccupied)
                    actions.AddRange(findSequencePathToHexaCPU(TempChar, goTo.x, goTo.y));

                j++;
            }

            if (actions.Count == 0)
                return (Character, new ActionAIPos(ActionType.SKIP, null));

            X = actions[0].pos.x;
            Y = actions[0].pos.y;
            return (Character, actions[0]);
        }
    }

    public class HealerCPU : CharacterCPU
    {
        public HealerCPU(Character character, HexaGrid hexas)
            : base(character, hexas) { }

        /*/// <summary>
        /// This character will move up to <paramref name="target"/> and heal them once they are at range. 
        /// Uses skills or not depending on the parameter <paramref name="allowedToUseSkill"/>.
        /// </summary>
        /// <remarks>
        /// For coming up on other characters without attacking them, use <see cref="moveTowards(CharacterCPU)"/>.
        /// </remarks>
        /// <param name="target">The target character</param>
        /// <param name="allowedToUseSkill">If true, this character will use skills whenever they can, 
        /// and if false, they will not use any skills</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public override (Character, ActionAIPos) target(CharacterCPU target, bool allowedToUseSkill)
        {
            if (allowedToUseSkill && SkillAvailable && target.HP > ClassData.basicAttack.effectValue + (Dmgbuff ? 1 : 0))
            // Skill use allowed, available & the target can't go down with just a basic attack
            {
                if (isInRangeToUseSkill(this, target))
                {
                    SkillAvailable = false;
                    TotalDamage -= 10;

                    target.HP += Character.getSkillDamage();
                    TotalDamage += Character.getSkillDamage();

                    return (Character, new ActionAIPos(ActionType.ATK2, new Point(target.X, target.Y)));
                }

            }
            else
            if (isInRangeToAttack(this, target))
            {
                target.HP += Character.getDamage();
                TotalDamage += Character.getDamage();

                return (Character, new ActionAIPos(ActionType.ATK1, new Point(target.X, target.Y)));
            }

            return moveTowards(target);
        }*/

        /// <summary>
        /// Finds which team member to heal.
        /// </summary>
        /// <param name="teamList">The list of all the team members</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        /// Edited by Socrate Louis Deriza L3C1
        public (Character, ActionAIPos) findHealingPriority(List<CharacterCPU> teamList)
        {
            CharacterCPU toHeal = null;
            int toHealScore = 0;

            foreach (CharacterCPU healerTarget in teamList)
                if (healerTarget.Character != this.Character)
                {
                    if (toHeal == null)
                    {
                        toHeal = healerTarget;
                        toHealScore = toHeal.Character.getClassData().maxHP - toHeal.HP;
                    }
                    else
                    {
                        int healerTargetScore = healerTarget.Character.getClassData().maxHP - healerTarget.HP;

                        if (healerTargetScore > toHealScore && isInTotalRange(this, toHeal))
                        {
                            toHeal = healerTarget;
                            toHealScore = healerTargetScore;
                        }
                    }
                }

            // toHeal has health to heal
            if (toHealScore > 0)
                return this.target(toHeal);

            else return this.wait();
        }
    }


    public class SorcererCPU : CharacterCPU
    {
        public SorcererCPU(Character character, HexaGrid hexas)
            : base(character, hexas) { }

        /*/// <summary>
        /// This character will move up to <paramref name="target"/> and heal them once they are at range. 
        /// Uses skills or not depending on the parameter <paramref name="allowedToUseSkill"/>.
        /// </summary>
        /// <remarks>
        /// For coming up on other characters without attacking them, use <see cref="moveTowards(CharacterCPU)"/>.
        /// </remarks>
        /// <param name="target">The target character</param>
        /// <param name="allowedToUseSkill">If true, this character will use skills whenever they can, 
        /// and if false, they will not use any skills</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public override (Character, ActionAIPos) target(CharacterCPU target, bool allowedToUseSkill)
        {
            if (allowedToUseSkill && SkillAvailable && target.HP > ClassData.basicAttack.effectValue + (Dmgbuff ? 1 : 0))
            // Skill use allowed, available & the target can't go down with just a basic attack
            {
                if (isInRangeToUseSkill(this, target))
                {
                    SkillAvailable = false;
                    TotalDamage -= 10;

                    return (Character, new ActionAIPos(ActionType.ATK2, new Point(target.X, target.Y)));
                }

            }
            else
            if (isInRangeToAttack(this, target))
            {
                target.Dmgbuff = true;

                return (Character, new ActionAIPos(ActionType.ATK1, new Point(target.X, target.Y)));
            }

            return moveTowards(target);
        }*/

        /// <summary>
        /// Finds which team member to buff. Waits if there's no need for it.
        /// </summary>
        /// <param name="teamList">The list of all the team members</param>
        /// <returns>A (character to use, action to do) tuple</returns>
        public (Character, ActionAIPos) findBuffingPriority(List<CharacterCPU> teamList, List<(Character, ActionAIPos)> actions)
        {
            List<CharacterCPU> sorcererFocusOrder = order(remaining(teamList, actions),
                                    CharClass.ARCHER, CharClass.VOLEUR, CharClass.MAGE, CharClass.GUERRIER);

            foreach (CharacterCPU toBuff in sorcererFocusOrder)
                if (toBuff != this && !toBuff.Dmgbuff)
                    return this.target(toBuff, false);

            return this.wait();
        }
    }


    //Edited by Socrate Louis Deriza L3C1
    public class RelevantCharacterInfoWrapper
    {
        private Character character;
        private int hp,
            totalDamage,
            x,
            y;
        private bool skillAvailable,
            dmgbuff,
            skillAvailable2,
            spelled,
            freezed;

        //Edited by Socrate Louis Deriza, L3C1
        public RelevantCharacterInfoWrapper(Character character)
        {
            this.character = character;

            this.hp = character.HP;
            this.totalDamage = character.totalDamage;
            this.x = character.x;
            this.y = character.y;
            this.hp = character.HP;
            this.skillAvailable = character.skillAvailable;

            this.skillAvailable2 = character.skillAvailable2;
            this.spelled = character.doomed;
            this.freezed = character.freezed;
            this.dmgbuff = character.dmgbuff;
        }

        public Character Character { get => character; }

        public int HP { get => hp; set => hp = value; }
        public int TotalDamage { get => totalDamage; set => totalDamage = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public bool SkillAvailable { get => skillAvailable; set => skillAvailable = value; }

        public bool SkillAvailable2 { get => skillAvailable2; set => skillAvailable2 = value; }

        public bool Spelled { get => spelled; set => spelled = value; }

        public bool Freezed { get => freezed; set => freezed = value; }
        public bool Dmgbuff { get => dmgbuff; set => dmgbuff = value; }
    }
}


