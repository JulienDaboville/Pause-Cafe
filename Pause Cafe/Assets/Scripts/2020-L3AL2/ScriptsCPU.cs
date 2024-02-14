using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using CharactersCPU;
using static UtilCPU.UtilCPU;
using static MainGame;
using AI_Class;
using AI_Util;
using Hexas;
using Misc;

namespace ScriptsCPU
{
    public class ScriptsCPU
    {
        /// <summary>
        /// Makes all the characters wait on their turn.
        /// </summary>
        /// <param name="currentTeam">The current team in play</param>
        /// <param name="hexas">The game board</param>
        /// <returns>A list of ActionAIPos turn skips</returns>
        public static List<(Character, ActionAIPos)> prone(int currentTeam, HexaGrid hexas)
        {
            Debug.Log("Script called : PRONE");

            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();

            foreach (CharacterCPU c in teamList)
                actions.Add(c.wait());

            return actions;
        }

        /// <summary>
        /// Makes the CPU team gang up on one character.
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns>A (Character, ActionAIPos) list describing the actions to take</returns>
        public static List<(Character, ActionAIPos)> offense(int currentTeam, HexaGrid hexas, List<CharacterCPU> enemyList)
        {
            Debug.Log("Script called : OFFENSE");

            List<CharacterCPU> teamList = order(getTeamList(currentTeam, hexas),
                CharClass.SOIGNEUR, CharClass.ENVOUTEUR);

            List<(Character chr, ActionAIPos act)> actions = new List<(Character, ActionAIPos)>();
            CharacterCPU victim = null;

            foreach (CharacterCPU c in teamList)
                for (int countPA = 0; countPA < c.Character.PA; countPA++)
                {
                    // When all enemies are defeated
                    if (enemyList.Count < 1)
                        actions.Add(c.wait());

                    else
                    {
                        if (victim == null)
                            victim = enemyList[0];

                        switch (c.Character.charClass)
                        {
                            case CharClass.SOIGNEUR:
                                // Heals the ally with the least HP
                                HealerCPU h = (HealerCPU)c;
                                actions.Add(h.findHealingPriority(teamList));
                                break;
                            case CharClass.ENVOUTEUR:
                                // Targets allies in this order : archers > thieves > mages > warriors
                                SorcererCPU s = (SorcererCPU)c;
                                actions.Add(s.findBuffingPriority(teamList, actions));
                                break;
                            default:
                                // Plainly targets the current appointed unit to focus on
                                actions.Add(c.target(victim));

                                if (victim.HP <= 0)
                                {
                                    enemyList.Remove(victim);
                                    victim = null;
                                }

                                break;
                        }
                    }

                    // If the current character skipped this turn, close this for loop 
                    if (actions[actions.Count - 1].act.action == ActionType.SKIP)
                        countPA = c.Character.PA;
                }

            return actions;
        }

        /// <summary>
        /// Gets the unit with the least HP to move towards the healer, then gets them healed.
        /// </summary>
        /// <param name="currentTeam">The team currently in control</param>
        /// <param name="hexas">The game board</param>
        /// <returns>A (Character, ActionAIPos) list describing the actions to take</returns>
        public static List<(Character, ActionAIPos)> patchUpUnit(int currentTeam, HexaGrid hexas, List<CharacterCPU> enemyList)
        {
            Debug.Log("Script called : PATCHUPUNIT");

            List<CharacterCPU> teamList = getHealingFocusedList(currentTeam, hexas);

            List<(Character chr, ActionAIPos act)> actions = new List<(Character, ActionAIPos)>();
            CharacterCPU victim = null;

            foreach (CharacterCPU c in teamList)
                for (int countPA = 0; countPA < c.Character.PA; countPA++)
                {
                    // When all enemies are defeated
                    if (enemyList.Count < 1)
                        actions.Add(c.wait());

                    else
                    {
                        if (victim == null)
                            victim = enemyList[0];

                        // Team member to focus healing on
                        if (teamList.IndexOf(c) == 0 && gotHealerInTeam(currentTeam))
                        {
                            // Can this unit get an attack in before needing to walk to the healer?
                            if (hexas.getWalkingDistance(c.X, c.Y, teamList[1].X, teamList[1].Y) <
                                (c.Character.PA - countPA - 1) * c.Character.getClassData().basePM
                                + teamList[1].Character.getClassData().basicAttack.range &&
                                (c.SkillAvailable ? isInRangeToUseSkill(c, victim) : isInRangeToAttack(c, victim)))
                            {
                                actions.Add(c.target(victim));

                                if (victim.HP <= 0)
                                {
                                    enemyList.Remove(victim);
                                    victim = null;
                                }
                            }
                            else
                                actions.Add(c.moveTowards(teamList[1]));
                        }
                        else switch (c.Character.charClass)
                            {
                                case CharClass.SOIGNEUR:
                                    // Heals the ally in the first slot of teamList
                                    actions.Add(c.target(teamList[0]));
                                    break;
                                case CharClass.ENVOUTEUR:
                                    // Targets allies in this order : archers > thieves > mages > warriors
                                    SorcererCPU s = (SorcererCPU)c;
                                    actions.Add(s.findBuffingPriority(teamList, actions));
                                    break;
                                default:
                                    // Plainly targets the current appointed unit to focus on
                                    actions.Add(c.target(victim));

                                    if (victim.HP <= 0)
                                    {
                                        enemyList.Remove(victim);
                                        victim = null;
                                    }

                                    break;
                            }
                    }

                    // If the current character skipped this turn, close this for loop 
                    if (actions[actions.Count - 1].act.action == ActionType.SKIP)
                        countPA = c.Character.PA;
                }

            return actions;
        }

        /// <summary>
        /// The team called moves as one towards the enemy.
        /// </summary>
        /// <param name="currentTeam">The team currently in control</param>
        /// <param name="hexas">The game board</param>
        /// <returns>A (Character, ActionAIPos) list describing the actions to take</returns>
        public static List<(Character, ActionAIPos)> formation(int currentTeam, HexaGrid hexas)
        {
            Debug.Log("Script called : FORMATION");

            List<CharacterCPU> teamList = order(getTeamList(currentTeam, hexas),
                CharClass.GUERRIER, CharClass.VALKYRIE, CharClass.VOLEUR, CharClass.MAGE, CharClass.ARCHER);
            List<(Character chr, ActionAIPos act)> actions = new List<(Character, ActionAIPos)>();

            CharacterCPU target = getTeamList(currentTeam == 0 ? 1 : 0, hexas)[0];
            CharacterCPU leader = teamList[0];

            foreach (CharacterCPU c in teamList)
                for (int countPA = 0; countPA < c.Character.PA; countPA++)
                {
                    // Leader call
                    if (c == leader)
                    {
                        // Limiting leader movement to 2 PA so leading thieves don't lose the other members
                        if (countPA >= 2)
                            actions.Add(c.wait());
                        else
                            actions.Add(c.moveTowards(target));
                    }
                    // Follower call
                    else
                        actions.Add(c.moveTowards(getTacticalFormation(target, leader, c.Character.getCharClass(), hexas)));

                    // If the current character skipped this turn, close this for loop 
                    if (actions[actions.Count - 1].act.action == ActionType.SKIP)
                        countPA = c.Character.PA;
                }


            return actions;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        //*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//*//
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Heal the closest ally wich health point aren't full
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> heal(CharacterCPU healer, int currentTeam, HexaGrid hexas)
        {
            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            CharacterCPU targetAlly;

            //Debug.Log("HEALER PLAYING");

            Character allyInPrio = identifyAllyClassPriority(currentTeam, hexas);

            if (allyInNeed(currentTeam, hexas))
            {
                // If an ally is in need
                Debug.Log("On est ici ");

                if (healer.Character.skillAvailable && canAOEHeal(currentTeam, hexas, healer.Character))
                {
                    Debug.Log(", on est là");
                    // If spell is ready, and find a good way to use it
                    Point p = bestPointForAOEHeal(currentTeam, hexas, healer.Character);
                    ActionAIPos ciblage = new ActionAIPos(ActionType.ATK2, new Point(p.x, p.y));
                    actions.Add((healer.Character, ciblage));
                }

                
                // Choosing target to heal/follow
                targetAlly = choosingAlly(allyRankedByMissingHP(currentTeam, hexas), currentTeam, hexas, healer.Character);
                Debug.Log("heal =>" + targetAlly.Character.charClass);
                actions.Add(healer.target(targetAlly));
            }

            else if (allTeamFullLife(currentTeam, hexas) && !atLeastOnInRange(currentTeam, hexas))
            {
                // If no one is in need, and no one in range, get closer to an ally
                Debug.Log("first elif");
                actions.Add(healer.moveTowards(allyInPrio));
                return actions;
            }
            else if (!allyInNeed(currentTeam, hexas) && moveTowardsWithoutRisk(currentTeam, hexas, allyInPrio) && AIUtil.hexaGrid.getWalkingDistance(healer.Character.x, healer.Character.y, allyInPrio.x, allyInPrio.y) > 2)
            {
                Debug.Log("second elif");
                actions.Add(healer.moveTowards(allyInPrio));
                return actions;
            }
            
            else actions.Add(healer.wait());
            Debug.Log(" else");
            return actions;
        }


        /// <summary>
        /// Boost the closest ally 
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> envouteur(CharacterCPU envouteur, int currentTeam, HexaGrid hexas)
        {
            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            //Character toBoost = null;

            if (envouteur.Character.skillAvailable && atLeastOnInRange(currentTeam, hexas))
            {
                // If the spell is ready, find the best way to use it
                Debug.Log("AOE BOOST ");
                Point p = bestPointForAOEBoost(currentTeam, hexas, envouteur.Character);
                ActionAIPos ciblage = new ActionAIPos(ActionType.ATK2, new Point(p.x, p.y));
                actions.Add((envouteur.Character, ciblage));
                return actions;
            }

            if (envouteur.Character.skillAvailable2)
            {
                List<Character> enemyTeam = getEnemyCharacterTeamList(currentTeam, hexas);
                Character lessImportantCharacterToSpelled = null;
                foreach (Character cEnemy in enemyTeam)
                {
                    if(cEnemy.getName() != "Soigneur" && cEnemy.getName() != "Envouteur")
                    {
                        if (isInRangeToUseSkill2(envouteur.Character, cEnemy))
                        {
                            lessImportantCharacterToSpelled = cEnemy;
                            if (cEnemy.getName() == "Guerrier" || cEnemy.getName() == "Valkyrie")
                            {
                                ActionAIPos to_spell = new ActionAIPos(ActionType.ATK3, new Point(cEnemy.getX(), cEnemy.getY()));
                                actions.Add((envouteur.Character, to_spell));
                                Debug.Log("Cpu --> Utilisation compétence 2 envoûteur");
                                return actions;
                            }
                        }
                    }
                    
                }
                if(lessImportantCharacterToSpelled != null)
                {
                    ActionAIPos to_spell = new ActionAIPos(ActionType.ATK3, new Point(lessImportantCharacterToSpelled.getX(), lessImportantCharacterToSpelled.getY()));
                    actions.Add((envouteur.Character, to_spell));
                    Debug.Log("Cpu --> Utilisation compétence 2 envoûteur");
                    return actions;
                }
                
            }
            if (atLeastOnInRange(currentTeam, hexas))
            {
                // If an ally is in range, boost the best ally possible
                actions.Add(envouteur.target(allyToBoost(currentTeam, hexas)));
                return actions;
            }
            if (!atLeastOnInRange(currentTeam, hexas))
            {
                // If no ally is in range, get closer to an one
                actions.Add(envouteur.moveTowards(identifyAllyClassPriority(currentTeam, hexas)));
                return actions;
            }

            return actions;
        }

        /// <summary>
        /// The currentChar will flee the enemy, and play around his healer
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> fleeWithHealer(CharacterCPU currentChar, HexaGrid hexas)
        {
            // If the currentChar is a healer AND someone is in need, the healer won't flee
            if (currentChar.Character.charClass == CharClass.SOIGNEUR && allyInNeed(currentChar.Character.team, hexas))
                return heal(currentChar, currentChar.Character.team, hexas);

            // If the currentChar is a healer AND no one is in need, the healer will flee
            else if (currentChar.Character.charClass == CharClass.SOIGNEUR && !allyInNeed(currentChar.Character.team, hexas))
                return flee(currentChar, hexas);

            // If an ally is already fleing, the currentChar won't flee
            if (alreadyAtRisk(currentChar.Character.team, hexas, currentChar.Character))
                return attack(currentChar.Character.team, hexas);

            List<Character> teamList = getCharacterTeamList(currentChar.Character.team, hexas);
            List<ActionAIPos> mouvement = new List<ActionAIPos>();
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            Hexa goTo;
            Point healerPos = null;

            // Find healer position
            foreach (Character c in teamList)
                if (c.charClass == CharClass.SOIGNEUR)
                    healerPos = new Point(c.x, c.y);

            // For every point around the healer, we check if this point, is not on enemy range
            foreach (Point p in pointsAroundHexa(healerPos, 3))
                if (!isPointInEnemyTeamRangeAttack(p, currentChar.Character.team))
                {
                    goTo = hexas.getHexa(p.x, p.y);
                    if (goTo != null && (goTo.type == HexaType.GROUND || goTo.type == HexaType.BONUS) && goTo.charOn == null)
                    {
                        mouvement.AddRange(AIUtil.findSequencePathToHexa(currentChar.Character, goTo.x, goTo.y));
                        actions.Add((currentChar.Character, mouvement[0]));
                        return actions;
                    }
                }

            // The currentChar couldn't find a good way to flee, he will attack
            Debug.Log("Can't flee");
            return attack(currentChar.Character.team, hexas);
        }

        /// <summary>
        /// The currentChar will flee the enemy, 
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> flee(CharacterCPU currentChar, HexaGrid hexas)
        {

            if (currentChar.Character.charClass == CharClass.SOIGNEUR)
                return heal(currentChar, currentChar.Character.team, hexas);

            List<ActionAIPos> mouvement = new List<ActionAIPos>();
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            Hexa goTo;

            // For every point around the currentChar, we check if this point, is not on enemy range
            foreach (Point p in hexas.findAllPaths(currentChar.Character.x, currentChar.Character.y, 3))
                if (!isPointInEnemyTeamRangeAttack(p, currentChar.Character.team))
                {
                    Debug.Log("hehe ici on flee 1");
                    goTo = hexas.getHexa(p.x, p.y);
                    if ((goTo != null && goTo.type == HexaType.GROUND && goTo.charOn == null) || (goTo != null && goTo.type == HexaType.BONUS && goTo.charOn == null))
                    {
                        mouvement.AddRange(AIUtil.findSequencePathToHexa(currentChar.Character, goTo.x, goTo.y));
                        actions.Add((currentChar.Character, mouvement[0]));
                        Debug.Log("hehe ici on flee 2");
                        return actions;
                    }
                }

            // The currentChar couldn't find a good way to flee, he will attack
            Debug.Log("Can't flee");
            return attack(currentChar.Character.team, hexas);
        }



        //edited by GOUVEIA Klaus, group: L3Q1
        //Author : ?
        /// <summary>
        /// Makes the CPU team gang up on one character.
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> attack(int currentTeam, HexaGrid hexas)
        {
            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            Character victim = null;

            foreach (CharacterCPU c in teamList)
                for (int countPA = 0; countPA < c.Character.PA; countPA++)
                {
                    //victim = enemyInMostAllyRange(currentTeam, c.Character, hexas);
                    victim = easierTargetToKill(currentTeam, c.Character, hexas);

                    // If the character canno't attack this turn, he won't put himself at risk
                    if (c.Character.getPA() == 1 && getEnemyTargetsAvailable(c.Character).Count == 0 && !moveWithoutRisk(currentTeam, hexas, c.Character))
                        actions.Add(c.wait());
                    else
                        actions.Add(c.target(victim));

                }


            return actions;
        }


        /// <summary>
        /// Guerrier decision
        /// Edited Socrate Louis Deriza
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> guerrier(CharacterCPU guerrier, int currentTeam, HexaGrid hexas)
        {
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            Character victim = null;
            int theoretical_hp;
            theoretical_hp = -1;
            for (int countPA = 0; countPA < guerrier.Character.PA; countPA++)
            {
                //victim = enemyInMostAllyRange(currentTeam, c.Character, hexas);
                victim = easierTargetToKill(currentTeam, guerrier.Character, hexas);
                if (countPA == 0)
                {
                    theoretical_hp = victim.getHP();
                }
                // If the character canno't attack this turn, he won't put himself at risk
                if (guerrier.Character.getPA() == 1 && getEnemyTargetsAvailable(guerrier.Character).Count == 0 && !moveWithoutRisk(currentTeam, hexas, guerrier.Character))
                {
                    actions.Add(guerrier.wait());
                }
                else
                {
                    if (theoretical_hp <= 0)
                    {
                        actions.Add(guerrier.wait());
                    }
                    else
                    {
                        actions.Add(guerrier.target(victim));
                    }
                }
                if(actions[actions.Count - 1].Item2.action == ActionType.ATK1)
                {
                    theoretical_hp -= guerrier.Character.getDamage();
                }
                if (actions[actions.Count - 1].Item2.action == ActionType.ATK2)
                {
                    theoretical_hp -= guerrier.Character.getSkillDamage() ;
                }
                if (actions[actions.Count - 1].Item2.action == ActionType.ATK3)
                {
                    theoretical_hp -= 2 + ((guerrier.Character.dmgbuff == true) ? 1 : 0);
                }



            }
            return actions;
        }

        /// <summary>
        /// Druide decision
        /// Added by Socrate Louis Deriza
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> druide(CharacterCPU druide, int currentTeam, HexaGrid hexas)
        {
            List<Character> enemyTeam = getEnemyCharacterTeamList(currentTeam, hexas);
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();

            for (int countPA = 0; countPA < druide.Character.PA; countPA++)
            {

                bool cantAttack = false;
                foreach (Character cEnemy in enemyTeam)
                {

                    Debug.Log("---" + cEnemy.getName() + "----");
                    if (druide.Character.skillAvailable == true)
                    {
                        Debug.Log("available");
                        if (cantAttack == false)
                        {
                            cantAttack = true;
                        }
                        if (isInRangeToUseSkill(druide.Character, cEnemy))
                        {
                            actions.Add((druide.Character, new ActionAIPos(ActionType.ATK2, new Point(cEnemy.x, cEnemy.y))));

                            Debug.Log("Skill1");
                            //return actions;
                            break;



                        }
                    }
                    if (druide.Character.skillAvailable2)
                    {
                        Debug.Log("available2");
                        if (cantAttack == false)
                        {
                            cantAttack = true;
                        }
                        if (isInRangeToUseSkill2(druide.Character, cEnemy))
                        {
                            actions.Add((druide.Character, new ActionAIPos(ActionType.ATK3, new Point(cEnemy.x, cEnemy.y))));
                            Debug.Log("Skill2");
                            //return actions;
                            break;
                        }
                        else
                        {
                            if (druide.Character.skillAvailable == false)
                            {
                                Debug.Log("moove Druide/skillAvaible == false");
                                actions.Add(druide.target(cEnemy));
                                break;
                            }
                        }
                    }
                    if (isInRangeToAttack(druide.Character, cEnemy))
                    {
                        if (cantAttack == false)
                        {
                            cantAttack = true;
                        }
                        actions.Add((druide.Character, new ActionAIPos(ActionType.ATK1, new Point(cEnemy.x, cEnemy.y))));
                        Debug.Log("attack1");
                        //return actions;
                        break;
                    }
                    if (cantAttack == true)
                    {
                        Debug.Log("moove Druide");
                        actions.Add(druide.target(cEnemy));

                        //return actions;
                        break;
                    }
                    Debug.Log("fin itération first for");


                }
            }
            
            
            
            return actions;

                /*
                List<Character> enemyTeam = getEnemyCharacterTeamList(currentTeam, hexas);

                foreach (cEnemy in enemyTeam)
                {
                    if (druide.Character.skillAvailable)
                    {
                        if (isInRangeToUseSkill())
                        {
                            return
                        }
                    }
                    if (druide.Character.skillAvailable)
                    {

                    }
                }
                */
        }
            /// <summary>
            /// Mage decision
            /// Edited by Socrate Louis Deriza
            /// </summary>
            /// <param name="currentChar">The character currently in control</param>
            /// <param name="charList">The list of all the characters on the board</param>
            /// <returns></returns>
            public static List<(Character, ActionAIPos)> mage(CharacterCPU mage, int currentTeam, HexaGrid hexas)
        {
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            Character victim = null;

            //Debug.Log("MAGE PLAYING");
            // If the mage spell is ready, he check if can find  agood way to use it, if yes he'll use it
            if (mage.Character.skillAvailable && canAOEDamage(currentTeam, hexas, mage.Character.x, mage.Character.y, 5, 2) /*&& !betterAOEDamage(currentTeam, hexas, mage.Character.x, mage.Character.y, 3)*/)
            {

                //Debug.Log("AOE DAMAGE ");
                Point p = bestPointForAOEDamage(currentTeam, hexas, mage.Character.x, mage.Character.y, 5, 2);
                ActionAIPos ciblage = new ActionAIPos(ActionType.ATK2, new Point(p.x, p.y));
                actions.Add((mage.Character, ciblage));
                return actions;
            }

            //Debug.Log("MAGE PLAYING");
            // If the mage spell is ready, he check if can find  agood way to use it, if yes he'll use it
            if (mage.Character.skillAvailable2 && canAOEDamage(currentTeam, hexas, mage.Character.x, mage.Character.y, 5, 2) /*&& !betterAOEDamage(currentTeam, hexas, mage.Character.x, mage.Character.y, 3)*/)
            {

                //Debug.Log("Poison DAMAGE ");
                Point p = bestPointForAOEDamage(currentTeam, hexas, mage.Character.x, mage.Character.y, 5, 2);
                ActionAIPos ciblage = new ActionAIPos(ActionType.ATK3, new Point(p.x, p.y));
                actions.Add((mage.Character, ciblage));
                return actions;
            }

            for (int countPA = 0; countPA < mage.Character.PA; countPA++)
                {
                    //victim = enemyInMostAllyRange(currentTeam, c.Character, hexas);
                    victim = easierTargetToKill(currentTeam, mage.Character, hexas);

                    // If the character canno't attack this turn, he won't put himself at risk
                    if (mage.Character.getPA() == 1 && getEnemyTargetsAvailable(mage.Character).Count == 0 && !moveWithoutRisk(currentTeam, hexas, mage.Character))
                        actions.Add(mage.wait());
                    else
                        actions.Add(mage.target(victim));

                }
            return actions;
        }



        //edited by GOUVEIA Klaus, group: L3Q1
        //Author : ?
        /// <summary>
        /// Rogue decision
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> voleur(CharacterCPU voleur, int currentTeam, HexaGrid hexas, Hexa caseBonus)
        {
            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            Character victim = null;
            List<Character> enemysList = getEnemyTargetsAvailable(voleur.Character);
            Debug.Log("Enemy Tragets Available :" + enemysList.Count);
            List<Character> currentCharacter = new List<Character>();
            currentCharacter.Add(voleur.Character);

            Point centreBonus = new Point(caseBonus.x, caseBonus.y);
            
            for (int countPA = 0; countPA < voleur.Character.PA; countPA++)
            {
                victim = null;
                
                foreach(Character c in enemysList){
                    if(c.getCharClass() ==  CharClass.SOIGNEUR){
                        if(isInTotalRange(voleur.Character, c)){
                            victim = c;
                        }
                    }
                }
                if(victim != null) {
                    Debug.Log("Voleur Target Soigneur");
                    actions.Add(voleur.target(victim));
                }


                else if(enemysList.Count != 0 && countPA == 0 && moveWithoutRisk(currentTeam, hexas, voleur.Character) && actions.Count == 0){
                    Debug.Log("Voleur PA = 0");
                    if(canGroupKill(currentCharacter, enemysList[0])){
                        Debug.Log("Voleur Kill Ennemy");
                        actions.Add(voleur.target(enemysList[0]));
                        actions.Add(voleur.target(enemysList[0]));
                        actions.Add(voleur.target(enemysList[0]));
                        return actions;
                    }
                    else if(isInTotalRange(voleur.Character, enemysList[0])){
                        
                        actions.Add(voleur.target(enemysList[0]));
                        actions.Add(voleur.target(enemysList[0]));
                        Debug.Log("Voleur Hit and Flee 1");
                        actions.Add(flee(voleur, hexas)[0]);
                        Debug.Log("Fin Voleur Hit and Flee 1");
                        return actions;                    
                    }
                    else{
                        
                        actions.Add(voleur.target(enemysList[0]));
                        Debug.Log("Voleur Hit and Flee 2 f");
                        actions.Add(flee(voleur, hexas)[0]);
                        Debug.Log("Fin Voleur Hit and Flee 2--> first  flee");
                        Debug.Log("Voleur Hit and Flee 2 s");
                        actions.Add(flee(voleur, hexas)[0]);
                        Debug.Log("Fin Voleur Hit and Flee 2--> second flee");
                        return actions; 
                    }
                }

                else if(enemysList.Count != 0 && countPA == 1 && moveWithoutRisk(currentTeam, hexas, voleur.Character)  && actions.Count == 1){
                    Debug.Log("Voleur PA = 1");
                    if(canGroupKill(currentCharacter, enemysList[0])){
                        Debug.Log("Voleur Kill Ennemy");
                        actions.Add(voleur.target(enemysList[0]));
                        actions.Add(voleur.target(enemysList[0]));
                        return actions;
                    }
                    else{
                        Debug.Log("Voleur Hit and Flee3");
                        actions.Add(voleur.target(enemysList[0]));
                        actions.Add(flee(voleur, hexas)[0]);
                        Debug.Log("Fin Voleur Hit and Flee3");
                        return actions;                    
                    }
                }

                else if(getBonusTeam() != currentTeam && hexas.getHexa(voleur.X, voleur.Y).type == HexaType.BONUS){
                    Debug.Log("Voleur Attack Zone");
                    victim = closestEnemy(currentTeam, voleur.Character, hexas);
                    actions.Add(voleur.target(victim)); 
                }

                else if(getBonusTeam() != currentTeam){
                    Debug.Log("Voleur Move Towards Zone");
                    Point bonus = new Point(caseBonus.x, caseBonus.y);
                    actions.Add(voleur.moveTowards(bonus)); 
                }
                

                else {
                    //if (notToFarAway(currentTeam, hexas, voleur.Character) && moveWithoutRisk(currentTeam, hexas, voleur.Character)){
                    victim = easierTargetToKill(currentTeam, voleur.Character, hexas);
                    if(getEnemyTargetsAvailable(voleur.Character).Count == 0 && !moveWithoutRisk(currentTeam, hexas, voleur.Character)){
                        actions.Add(voleur.wait());
                        Debug.Log("Voleur Wait");
                    }
                    else {
                        Debug.Log("Voleur Target Enemy");
                        actions.Add(voleur.target(victim));
                    }
                }
                //if (notToFarAway(currentTeam, hexas, voleur.Character) && moveWithoutRisk(currentTeam, hexas, voleur.Character))
                //{
                //    //victim = enemyInMostAllyRange(currentTeam, voleur.Character, hexas);
                //    victim = easierTargetToKill(currentTeam, voleur.Character, hexas);
                //    actions.Add(voleur.target(victim));
                //}
                //else
                //    actions.Add(voleur.wait());
                //***
            }
            
            
            
            return actions;
        }


        //edited by GOUVEIA Klaus, group: L3Q1
        //Author : ?
        /// <summary>
        /// Archer behaviour
        /// </summary>
        /// <param name="currentChar">The character currently in control</param>
        /// <param name="charList">The list of all the characters on the board</param>
        /// <returns></returns>
        public static List<(Character, ActionAIPos)> archer(CharacterCPU archer, int currentTeam, HexaGrid hexas)
        {
            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            Character victim = null;
            List<Character> enemysList = getEnemyTargetsAvailable(archer.Character);
            Debug.Log("Enemy Tragets Available :" + enemysList.Count);

            for (int countPA = 0; countPA < archer.Character.PA; countPA++)
            {

                victim = easierTargetToKill(currentTeam, archer.Character, hexas);
                actions.Add(archer.target(victim));
                Debug.Log("Archer Attack2");
                
            }

            Debug.Log("Archer Actions => " + actions.Count);
            return actions;
        }


        //edited by GOUVEIA Klaus, group: L3Q1
        //Edited by Soocrate Louis Deriza L3C1
        //Author : Klaus GOUVEIA
        public static List<(Character, ActionAIPos)> valkyrie(CharacterCPU valkyrie, int currentTeam, HexaGrid hexas, Hexa caseBonus)
        {
            List<(Character, ActionAIPos)> actions = new List<(Character, ActionAIPos)>();
            List<CharacterCPU> teamList = getTeamList(currentTeam, hexas);
            Character victim = null;
            int theoretical_hp;
            theoretical_hp = -1;
            for (int countPA = 0; countPA < valkyrie.Character.PA; countPA++)
            {
                //victim = enemyInMostAllyRange(currentTeam, c.Character, hexas);
                victim = easierTargetToKill(currentTeam, valkyrie.Character, hexas);
                if (countPA == 0)
                {
                    theoretical_hp = victim.getHP();
                }
                // If the character canno't attack this turn, he won't put himself at risk
                if (valkyrie.Character.getPA() == 1 && getEnemyTargetsAvailable(valkyrie.Character).Count == 0 && !moveWithoutRisk(currentTeam, hexas, valkyrie.Character))
                {
                    actions.Add(valkyrie.wait());
                }
                else
                {
                    if (theoretical_hp <= 0)
                    {
                        actions.Add(valkyrie.wait());
                    }
                    else
                    {
                        actions.Add(valkyrie.target(victim));
                    }
                }
                if (actions[actions.Count - 1].Item2.action == ActionType.ATK1)
                {
                    theoretical_hp -= valkyrie.Character.getDamage();
                }
                if (actions[actions.Count - 1].Item2.action == ActionType.ATK2)
                {
                    theoretical_hp -= valkyrie.Character.getSkillDamage();
                }
                if (actions[actions.Count - 1].Item2.action == ActionType.ATK3)
                {
                    theoretical_hp -= 2 + ((valkyrie.Character.dmgbuff == true) ? 1 : 0);
                }



            }
            return actions;



        }



    }
}

