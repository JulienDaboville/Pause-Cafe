using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Misc;
using Hexas;
using Characters;
using AI_Util;
using static UtilCPU.UtilCPU;
using AI_Class;
using Stats;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.EventSystems;
using System.Collections;

// ##################################################################################################################################################
// MAIN
// Author : ?
// Edited by L3Q1, VALAT Thibault and GOUVEIA Klaus
// Commented by L3Q1, VALAT Thibault
// ##################################################################################################################################################

public class MainGame : MonoBehaviour
{
    // Init in character selection menu
    public static StartGameData startGameData;

    
    // Add Socrate Louis Deriza, L3C1
    public static EndGameDataCharacter[] endGameDataCharacter;

    // Declaraion of variables
    public Mesh hexaFilledMesh;
    public Mesh hexaHollowMesh;
    public Mesh hexaWallMesh;
    public GameObject ruinsMap;
    public GameObject libreMap;//L3C1, Y.H
    public GameObject arrow;
    public GameObject hexaHighlight;
    public GameObject hexasFolder;
    public GameObject hexaTemplate;
    public GameObject charactersFolder;
    public GameObject characterTemplate;
    public List<GameObject> characterTemplateModels;
    public GameObject particleExplosion;
    public GameObject particleHeal;
    public GameObject damageValueDisplay;
    public GameObject camera_;
    public Transform cameraPos;
    public Vector3 cameraPosGoal;
    public Boolean cameraMoved;
    public GameObject UICurrentChar;
    public GameObject UIEnemyChar;
    public GameObject Initiative;
    public GameObject BonusCard;
    public List<Texture> charSquares;
    public GameObject UICharTurns;
    public GameObject UICharTurnTemplate;
    public GameObject UIAction;
    public GameObject UIPauseMenu;

    //Author : L3C1 CROUZET Oriane, 08/04/2023
    public GameObject UIHelpMenu;
    public Text currentCharacterText;    
    public GameObject UITooltip;
    public Text tooltipText;
    public int doomDamage;

    //public Character.CharClass classCharacter;
    public GameObject UIVictoryScreen;
    public List<Texture> charCards;
    public int tileW;
    public int tileH;
    public bool lockedCamera;
    public Toggle toggleLockedCamera;
    public bool debugMode;
    public bool pauseMenu;
    public bool helpMenu;

    public int frame;
    public bool updateUI;
    public bool updateMouseHover;
    public HexaGrid hexaGrid;
    public Vector3 mousePosOld;
    public Vector3 mousePos;
    public Hexa hexaHoveredOld;
    public Hexa hexaHovered;
    public static Character currentCharControlled;
    public int currentCharControlledID;

    public List<GameObject> UICharTurnsList;
    public List<GameObject> pathFinderDisplay;
    public List<GameObject> lineOfSightDisplay;
    public List<GameObject> dangerDisplay;
    public List<Point> pathWalk;
    public Character charHovered;
    public int attackUsed;
    public Point attackUsedTargetHexa;
    public CharsDB.Attack attackUsedAttack;
    public int pathWalkpos;
    public int newTurn;
    public int AIwaitingTime;
    public int winner;
    public static int bonusTeam;
    public StatsGame statsGame;

    public Color pathDisplayColor;
    public Color allPathsDisplayColor;
    public Color lineOfSightColor;
    public Color blockedSightColor;
    public Color AoEColor;
    public Color neutralBonusDisplayColor;
    public Color redBonusDisplayColor;

    public Color blueBonusDisplayColor;

    // turn counter for the damage over time
    // Added by Youcef MEDILEH, L3C1
    //Edited by Julien D'aboville L3L1
    public int turnCounterDamageOverTimeTEAM1;

    public int turnCounterDamageOverTimeTEAM2;

    public enum ActionType { MOVE, ATK1, ATK2, ATK3,ATK4, SKIP, WAIT };
    public ActionType actionType;
    public List<ActionAIPos> decisionSequence; // AI decisions
    public List<(Character chr, ActionAIPos act)> decisionSequenceCPU; // CPU decisions
    public Slider slider;
    public int countTurn;
    public Boolean charge = false;
    public Material pathDisplayMat;
    public Material bonusDisplayMat;

    public Material allPathsDisplayMat;
    public Material lineOfSightMat;
    public Material blockedSightMat;
    public Material aoeMat;
    List<Hexa> bonusPoints;
    List<Hexa> poisonnedHexas;
    Hexa caseBonus;

    //Création des nouveaux args par L3C1, Yuting HUANG
    //Date : 01/04/2023
    //Ajout un temps limité pour chaque tour
    public Slider TimeRoleTureSlider;
    public float MaxTimeNum = 30;
    private float TimeNum;
    public float tour = 0;


    //Awake is called before Start
    void Awake()
    {
        Application.targetFrameRate = 75;
        QualitySettings.vSyncCount = 0;
    }

    // Start is called before the first frame update
    // Initialisation of variables and game settings
    //Author : ??
    //Edited by L3Q1 VALAT Thibault
    void Start()
    {
        //Définition de variable pour limiter le temps
        TimeNum = MaxTimeNum;

        //Initialisation of textures and colors
        pathDisplayMat = new Material(Shader.Find("Standard"));
        bonusDisplayMat = new Material(Shader.Find("Standard"));
        allPathsDisplayMat = new Material(Shader.Find("Standard"));
        lineOfSightMat = new Material(Shader.Find("Standard"));
        blockedSightMat = new Material(Shader.Find("Standard"));
        aoeMat = new Material(Shader.Find("Standard"));

        pathDisplayMat.color = pathDisplayColor;
        allPathsDisplayMat.color = allPathsDisplayColor;
        lineOfSightMat.color = lineOfSightColor;
        blockedSightMat.color = blockedSightColor;
        aoeMat.color = AoEColor;

        // Initialisation of Hexas and Characters global variables
        Hexa.hexasFolder = hexasFolder;
        Hexa.hexaFilledMesh = hexaFilledMesh;
        Hexa.hexaHollowMesh = hexaHollowMesh;
        Hexa.hexaWallMesh = hexaWallMesh;
        Hexa.hexaTemplate = hexaTemplate;
        Character.characterTemplate = characterTemplate;
        Character.characterTemplateModels = characterTemplateModels;
        Character.charactersFolder = charactersFolder;
        CharsDB.initCharsDB();
        bonusPoints = new List<Hexa>();
        poisonnedHexas = new List<Hexa>();
        Hexa.offsetX = -((tileW - 1) * 0.75f) / 2;
        Hexa.offsetY = -((tileH - 1) * -0.86f + ((tileW - 1) % 2) * 0.43f) / 2;
        List<Point> casesBonus = new List<Point>();
        hexaHighlight.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.25f);
        frame = 0;
        updateUI = true;
        updateMouseHover = true;
        cameraMoved = false;
        bonusTeam = -1;

        //Added by L3C1 CROUZET Oriane, 08/04/2023
        pauseMenu = false;
        helpMenu = false;
        doomDamage = CharsDB.list[(int)CharClass.ENVOUTEUR].skill_2.effectValue;

        // Added by Youcef MEDILEH, L3C1
        turnCounterDamageOverTimeTEAM1 = 0;
        turnCounterDamageOverTimeTEAM2 = 0;

        // Initialisation of game data if it's not (it should be in the main menu)
        if (startGameData == null)
        {
            Debug.Log("startGameData == null");
            startGameData = new StartGameData();
            startGameData.loadSave = false;
            startGameData.charsTeam1 = new List<CharClass>();
            startGameData.charsTeam1.Add(CharClass.MAGE);
            startGameData.charsTeam1.Add(CharClass.BUCHERON);
            startGameData.charsTeam2 = new List<CharClass>();
            startGameData.charsTeam2.Add(CharClass.SOIGNEUR);
            startGameData.charsTeam2.Add(CharClass.ARCHER);
            startGameData.player1Type = PlayerType.AI_HARD;
            startGameData.player2Type = PlayerType.AI_EASY;
            startGameData.mapChosen = 1;
        }

        if (startGameData.loadSave)
        {
            Debug.Log("startGameData.loadSave");
            loadGame();
        }
        else
        {
            // Initialisation of the map (hexa grid)
            slider.value = startGameData.slider;
            hexaGrid = new HexaGrid();
            if (startGameData.mapChosen == 0)
            {
                Debug.Log("We are !here!!createGridFromFile!!!!");
                hexaGrid.createGridFromFile(Application.streamingAssetsPath + "/Data/Map/ruins");
                tileW = hexaGrid.w; tileH = hexaGrid.h;
                ruinsMap.SetActive(true);
                foreach (Hexa hexa in hexaGrid.hexaList)
                {
                    if (hexa.type != HexaType.GROUND && hexa.type != HexaType.BONUS)
                    {
                        hexa.go.GetComponent<Renderer>().enabled = false;
                    }
                }
            }
            else if (startGameData.mapChosen == 2)
            {
                Debug.Log("We are !here!!!!!!");
                hexaGrid.createGridFromFile(Application.streamingAssetsPath + "/Data/Map/libre");
                tileW = hexaGrid.w; tileH = hexaGrid.h;
                libreMap.SetActive(true);
                foreach (Hexa hexa in hexaGrid.hexaList)
                {
                    if (hexa.type != HexaType.GROUND && hexa.type != HexaType.BONUS)
                    {
                        hexa.go.GetComponent<Renderer>().enabled = false;
                    }
                }
            }
            
            else if (startGameData.mapChosen == 1)
            {
                Debug.Log("We are !here!!createRandomRectGrid!!!!");
                hexaGrid.createRandomRectGrid(tileW, tileH);
            }

            caseBonus = initBonus();
            Initiative.SetActive(true);

            int nb = 0;
            // Put characters on the grid
            for (int i = 0; i < 5; i++)
            {
                if (i < startGameData.charsTeam1.Count)
                {
                    hexaGrid.addChar(startGameData.charsTeam1[i], tileW / 2 - 4 + 2 + i, tileH - 2, 0);
                    nb++;
                }
                if (i < startGameData.charsTeam2.Count)
                {
                    hexaGrid.addChar(startGameData.charsTeam2[i], tileW / 2 - 4 + 2 + i, 2, 1);
                    nb++;
                }
            }

            endGameDataCharacter = createEndDataGameList(nb);

            hexaGrid.charList.Sort();


            foreach (Character c in hexaGrid.charList)
            {
                hexaGrid.getHexa(c.x, c.y).changeType(HexaType.GROUND);
            }

            for (int i = hexaGrid.charList.Count; i <= 10; i++)
                Initiative.transform.GetChild(i).transform.position = new Vector3(10000, 10000, 0);

            // Initialisation of the current character cursor
            currentCharControlledID = 0;
            currentCharControlled = hexaGrid.charList[currentCharControlledID];

        }
        // Initialisation of the AI 
        decisionSequence = new List<ActionAIPos>();
        decisionSequenceCPU = new List<(Character chr, ActionAIPos act)>();
        AI.hexaGrid = hexaGrid;

        AIHard.learn = false;
        AIUtil.hexaGrid = hexaGrid;

        mousePos = Input.mousePosition;
        hexaHovered = null;
        hexaHoveredOld = null;
        charHovered = null;

        countTurn = 0;

        pathFinderDisplay = new List<GameObject>();
        lineOfSightDisplay = new List<GameObject>();
        pathWalk = null;
        attackUsed = 0;
        pathWalkpos = 0;
        newTurn = 0;
        winner = -1;
        statsGame = new StatsGame();
        actionType = ActionType.MOVE;
        UICharTurns.SetActive(true);
        UIAction.SetActive(true);

        //Author : L3C1 CROUZET Oriane
        UITooltip.SetActive(false);

        { // Init character turn list UI
            int i = 0;
            foreach (Character c in hexaGrid.charList)
            {
                GameObject go = GameObject.Instantiate(UICharTurnTemplate, UICharTurns.transform);
                go.SetActive(false);
                go.transform.localPosition = new Vector3(200 + i, 0, 0);
                go.transform.GetChild(0).GetComponent<Image>().color = (c.team == 0) ? Character.TEAM_1_COLOR : Character.TEAM_2_COLOR;
                go.transform.GetChild(1).GetComponent<RawImage>().texture = charCards[(int)c.charClass];
                UICharTurnsList.Add(go);
                i += 80;
            }
        }
        displayInitiative();
        // Initialisation of the Camera
        cameraPosGoal = cameraPos.position;
        lockedCamera = (startGameData.player1Type != PlayerType.HUMAN && startGameData.player2Type != PlayerType.HUMAN);
        toggleLockedCamera.isOn = lockedCamera;
        AIwaitingTime = lockedCamera ? 0 : 20;
        Debug.Log("Debut de la partie");
        
    }

    // Update is called once per frame
    //Author : ??
    //Edited by L3Q1 VALAT Thibault
    // Edited by L3C1 Youcef MEDILEH :
    // - ajout de la gestion de la zone de poison
    // - ajout de la capacité 2 des personnages
    // Edited by L3C1 Socrate Louis Deriza, ajout du bouton compétence 2
    //Edited by L3C1 Yuting HUANG, le 01/04/2023, ajout d'un temps limité
    //Edited by L3C1 CROUZET Oriane, 08/04/2023, ajout du bouton help
    void Update()
    {
        if (!pauseMenu && !helpMenu)
        {
            //Contrôler le temps
            TimeLimite();
        }

        //Change controlled character
        getHoveredHexa();
        if (Input.GetMouseButton(1) && charHovered != null && charHovered.PA > 0 && charHovered.team == currentCharControlled.team)
        {
            currentCharControlled = charHovered;
        }
        frame++;

        if (currentCharControlled.isFreezed())
        {
            currentCharControlled.PA = 0;
            currentCharControlled.setFreezed(false);
            nextTurn();
        }
        
        // mark poisoned hexas
        // Added by Youcef MEDILEH, L3C1, le 23/03/2023
        foreach (Hexa hexa in poisonnedHexas)
        {
            hexa.go.GetComponent<Renderer>().material.color = Color.green;
        }


        // Update mouse position
        mousePosOld = mousePos;
        mousePos = Input.mousePosition;

        hexaGrid.getHexa(16, 8).changeColor(new Color(0, 255, 0));

        // PAUSE MENU
        if (pauseMenu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                int menuPosX = Screen.width / 2 - 80;
                int menuPosY = Screen.height / 2 + 100 - 100;
                // Save the current game
                if (mousePos.x >= menuPosX && mousePos.x < menuPosX + 160 && mousePos.y >= menuPosY - 15 && mousePos.y < menuPosY - 15 + 20)
                {
                    saveGame();
                }
                // To restart the game:
                else if (mousePos.x >= menuPosX && mousePos.x < menuPosX + 160 && mousePos.y >= menuPosY - 47 && mousePos.y < menuPosY - 47 + 20)
                {
                    Debug.Log("Restarting game");
                    SceneManager.LoadScene(1);


                }
                // to quit the game:
                else if (mousePos.x >= menuPosX && mousePos.x < menuPosX + 160 && mousePos.y >= menuPosY - 79 && mousePos.y < menuPosY - 79 + 20)
                {
                    MainMenu.startGameData = new StartGameData();
                    MainMenu.startGameData.slider = slider.value;
                    SceneManager.LoadScene(0);
                }
                // To go back:
                else if (mousePos.x >= Screen.width / 2 - 160 && mousePos.x < Screen.width / 2 && mousePos.y >= Screen.height - 90 && mousePos.y < Screen.height)
                {
                    pauseMenu = false;
                    UIPauseMenu.SetActive(false);
                }
                lockedCamera = toggleLockedCamera.isOn;
            }
        }

        //Author : L3C1 CROUZET Oriane, 08/04/2023 --> 12/04/2023
        //HELP MENU
        else if (helpMenu)
        {
            switch (currentCharControlled.charClass)
            {
                case CharClass.GUERRIER:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par le Guerrier: \n" + 
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +  
                                                    "\n 2) Attaque de base : Vous pouvez porter " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString() + ") \n" +
                                                    "\n 3) Compétence 1 : Votre attaque passe à " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString() + ") \n" +
                                                    "\n 4) Compétence 2 : RANDOM DAMAGE --> permet d'obtenir entre 1 et 4 points bonus d'attaque (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.range.ToString() + ") \n" +
                                                    "\n 5) Passer votre tour";
                        break;
                    }

                case CharClass.VOLEUR:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par le Voleur: \n" +
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +
                                                    "\n 2) Attaque de base : Vous pouvez porter " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString() + ") \n" +
                                                    "\n 3) Compétence 1 : Votre attaque passe à " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString() + ") \n" +
                                                    "\n 4) Compétence 2 : SNEAK LIFEBOND --> permet de voler " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + " point.s de vie à un adversaire cible (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.range.ToString() + ") \n" +
                                                    "\n 5) Passer votre tour"; break;
                    }
                case CharClass.ARCHER:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par l'Archer: \n" +
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +
                                                    "\n 2) Attaque de base : Vous pouvez porter " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString() + ") \n" +
                                                    "\n 3) Compétence 1 : Votre attaque passe à " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString() + ") \n" +
                                                    "\n 4) Compétence 2 : RAIN OF ARROWS --> permet de lancer une pluie de flèches infligeant " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + " point.s de dégâts aux adversaires (portée : toute la map) \n" +
                                                    "\n 5) Passer votre tour"; break;
                    }
                case CharClass.MAGE:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par la Mage: \n" +
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +
                                                    "\n 2) Attaque de base : Vous pouvez porter " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + " points de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString() + ") \n" +
                                                    "\n 3) Compétence 1 : Votre attaque passe à " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " points de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString() + ") \n" +
                                                    "\n 4) Compétence 2 : SNAKE BITE --> permet de piéger des cases avec du poison pendant 4 tours, affectant ennemis et alliés de " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.range.ToString() + ") \n" +
                                                    "\n 5) Passer votre tour"; break;
                    }
                case CharClass.SOIGNEUR:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par le Soigneur: \n" +
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +
                                                    "\n 2) Soin de base : Vous pouvez restaurer " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + " point.s de vie à un allié (portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString() + ") \n" +
                                                    "\n 3) Compétence 1 : Votre soin passe à " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " point.s de vie restauré.s (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString() + ") \n" +
                                                    "\n 4) Compétence 2 : MASSIVE HEAL --> permet de soigner toute l'équipe, y compris soi-même, de " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + " point.s de vie, ou 0 si la vie est pleine (portée : en appuyant sur soi-même ou un allié) \n" +
                                                    "\n 5) Passer votre tour"; break;
                    }
                case CharClass.DRUIDE:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par le Druide: \n" +
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +
                                                    "\n 2) Attaque de base : Vous pouvez porter " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString() + ") \n" +
                                                    "\n 3) Compétence 1 : Votre attaque passe à " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString() + ") \n" +
                                                    "\n 4) Compétence 2 : FREEZING CURSE --> permet de geler un ennmi pendant un tour, l'empêchant de faire quelconque action (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.range.ToString() + ") \n" +
                                                    "\n 5) Passer votre tour"; break;
                    }
                case CharClass.VALKYRIE:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par la Valkyrie: \n" +
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +
                                                    "\n 2) Attaque de base : Vous pouvez porter " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString() + ") \n" +
                                                    "\n 3) Compétence 1 : Votre attaque passe à " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " point.s de dégâts (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString() + ") \n" +
                                                    "\n 4) Compétence 2 : THUNDER STRIKE --> permet de lancer un éclair causant " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + " point.s de dégâts sur 4 cases, et de bloquer les actions d'un ennemi pendant un tour (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.range.ToString() + ") \n" +
                                                    "\n 5) Passer votre tour"; break;
                    }
                case CharClass.ENVOUTEUR:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par l'Envoûteur: \n" +
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +
                                                    "\n 2) Damage buff : + " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + " point.s de dégâts (portée : sur soi-même) \n" +
                                                    "\n 3) PA buff : + " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " point.s d'action (portée : sur soi-même) \n" +
                                                    "\n 4) Compétence 2 : EVIL AURA --> permet de maudire un adversaire en lui ôtant " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + " points de dégâts lors de sa prochaine attaque (portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.range.ToString() + ") \n" +
                                                    "\n 5) Passer votre tour"; break;
                    }

                case CharClass.BUCHERON:
                    {
                        currentCharacterText.text = "Voici les actions réalisables par le Bucheron: \n" +
                                                    "\n 1) Déplacement : Vous pouvez vous déplacer de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases \n" +
                                                    "\n 2) Attaque de base :  " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.ToString() + " points de dégâts lors de sa prochaine attaque (portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString() + ") \n" +

                                                    "\n 3) Cri de guerre : augmente ses PV et ses dégats de " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + " \n" +
                                                    "\n 4) Attaque lourde a la hache de : + " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + " dégats \n" +
                                                    "\n 5) Déplacement de : + " + CharsDB.list[(int)currentCharControlled.charClass].skill_3.effectValue.ToString() + " cases \n" +

                                                    "\n 6) Passer votre tour"; break;
                    }
            }

        if (Input.GetMouseButtonDown(0))
            {
                //Close help menu
                if (mousePos.x >= Screen.width / 2 && mousePos.x < Screen.width / 2 + 160 && mousePos.y >= Screen.height - 90 && mousePos.y < Screen.height)
                {
                    helpMenu = false;
                    UIHelpMenu.SetActive(false);
                }
                lockedCamera = toggleLockedCamera.isOn;
            }
        }

        //When there is still no winner
        else if (winner == -1)
        {
            float forceCamX = 0.0f;
            float forceCamY = 0.0f;
            float forceCamZ = 0.0f;
            float scrollWheelStrengh = 7.0f;

            // ZOOM (MOUSEWHEEL)
            if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
            {
                forceCamY -= Input.GetAxis("Mouse ScrollWheel") * scrollWheelStrengh;
            }

            //Camera
            if (!lockedCamera)
            {
                // MOVE CAMERA (MIDDLE CLICK OR ZQSD (AZERTY keyboard))
                if (Input.GetMouseButton(0))
                {
                    forceCamX += (mousePosOld.x - mousePos.x) * 0.003f * cameraPosGoal.y;
                    forceCamZ += (mousePosOld.y - mousePos.y) * 0.003f * cameraPosGoal.y;
                }
                if (Input.GetKey(KeyCode.Z))
                {
                    forceCamZ += 1;
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    forceCamX -= 1;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    forceCamZ -= 1;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    forceCamX += 1;
                }
            }

            //Move the camera depending on the directions
            Vector3 newCameraPosGoal = new Vector3(cameraPosGoal.x + forceCamX, cameraPosGoal.y + forceCamY, cameraPosGoal.z + forceCamZ);
            cameraMoved = false;
            if (newCameraPosGoal.x > -21 && newCameraPosGoal.x < 23)
            {
                cameraMoved = true;
                cameraPosGoal.x = newCameraPosGoal.x;
            }

            if (newCameraPosGoal.y > 8.0f && newCameraPosGoal.y < 20.0f)
            {
                cameraMoved = true;
                cameraPosGoal.y = newCameraPosGoal.y;
                newCameraPosGoal.z += Input.GetAxis("Mouse ScrollWheel") * scrollWheelStrengh;
            }

            if (newCameraPosGoal.z > -27 && newCameraPosGoal.z < 10)
            {
                cameraMoved = true;
                cameraPosGoal.z = newCameraPosGoal.z;
            }
            if (forceCamX == 0.0f && forceCamY == 0.0f && forceCamZ == 0.0f)
            {
                cameraMoved = false;
            }

            // OPEN PAUSE MENU
            if (Input.GetMouseButtonDown(0))
            {
                if (mousePos.x >= Screen.width / 2 - 160 && mousePos.x < Screen.width / 2 && mousePos.y >= Screen.height - 90 && mousePos.y < Screen.height)
                {
                    pauseMenu = true;
                    UIPauseMenu.SetActive(true);
                }
            }

            //Author :  L3C1 CROUZET Oriane, 08/04/2023
            //OPEN HELP MENU
            if (Input.GetMouseButtonDown(0))
            {
                if (mousePos.x >= Screen.width / 2 && mousePos.x < Screen.width / 2 + 160 && mousePos.y >= Screen.height - 90 && mousePos.y < Screen.height)
                {
                    helpMenu = true;
                    UIHelpMenu.SetActive(true);
                }
            }
        }

        // SMOOTH CAMERA
        cameraPos.position = new Vector3(cameraPos.position.x * 0.85f + cameraPosGoal.x * 0.15f, cameraPos.position.y * 0.85f + cameraPosGoal.y * 0.15f, cameraPos.position.z * 0.85f + cameraPosGoal.z * 0.15f);

        //Edited by Socrate Louis Deriza L3C1
        //Edited by Julien L3L1
        // MAIN GAME LOOP
        if (winner == -1)
        {
           
        if (pathWalk != null && charge)
            {
                print("CHARGE");
                walkingAnimation(); //charge
            }
            else if (pathWalk != null)
            {
                // Walking animation when going from an hexa to another
                walkingAnimation();
            }
            else if (attackUsed > 0)
            {
                // Attack animation
                if (attackUsed == 1)
                {
                    useAttack();
                }
                attackUsed--;
                // Interaction with the game
            }
            else if (!pauseMenu)
            {
               
                PlayerType currentPlayerType = whoControlsThisChar(currentCharControlled);
                // ACTIONS FOR HUMAN PLAYERS
                if (currentPlayerType == PlayerType.HUMAN)
                {
                    // HOVER DETECTION : hovered hexa is stored in hexaHovered
                    getHoveredHexa();



                    // W Key : Move
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                       
                        actionType = ActionType.MOVE;
                        updateMouseHover = true; updateUI = true;
                    }

                    // X Key : Attack
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                      
                        actionType = ActionType.ATK1;
                        updateMouseHover = true; updateUI = true;
                    }

                    // C Key : Skill
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                      
                        if (currentCharControlled.skillAvailable)
                            actionType = ActionType.ATK2;

                        updateMouseHover = true; updateUI = true;
                    }

                    // V Key : Skill2
                    // Added by Youcef MEDILEH, L3C1
                    //Edited by L3C1 CROUZET Oriane
                    if (Input.GetKeyDown(KeyCode.V))
                    {
                        
                        if (currentCharControlled.skillAvailable2)
                            actionType = ActionType.ATK3;

                        updateMouseHover = true; updateUI = true;
                    }

                    // F Key : Skill3
                    // Author :Julien D'aboville L3L1
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                       
                        if (currentCharControlled.skillAvailable3)
                            actionType = ActionType.ATK4;

                        updateMouseHover = true; updateUI = true;
                    }

                    // B Key : Skip
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                       
                        currentCharControlled.PA = 0;
                        nextTurn();
                    }


                    // Left Click : action (Move / Attack) or change action Type (UI) or skip turn
                    // Click on the left top buttons, under characters card
                    //Edited by Socrate Louis Deriza L3C1
                    // Edited :Julien D'aboville L3L1

                    if (!cameraMoved && Input.GetMouseButtonUp(0) && (cameraPos.position - cameraPosGoal).magnitude <= 0.2f)
                    {
                        // Deplacement
                        if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 835 && mousePos.y < Screen.height - 835 + 24 * 1.8)
                        {
                            actionType = ActionType.MOVE;
                        }
                        // Attack
                        else if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 890 && mousePos.y < Screen.height - 890 + 24 * 1.8)
                        {
                            actionType = ActionType.ATK1;
                        }

                        // Special attack 1
                        else if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 945 && mousePos.y < Screen.height - 945 + 24 * 1.8)
                        {
                            if (currentCharControlled.skillAvailable) actionType = ActionType.ATK2;
                        }

                        // Special attack 2
                        else if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 1000 && mousePos.y < Screen.height - 1000 + 24 * 1.8)
                        {
                            if (currentCharControlled.skillAvailable2) actionType = ActionType.ATK3;
                        }

                        // Next turn
                        else if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 1055 && mousePos.y < Screen.height - 1055 + 24 * 1.8)
                        {

                            currentCharControlled.PA = 0;
                            nextTurn();
                        }


                        else
                        {
                            switch (actionType)
                            {
                                case ActionType.MOVE:

                                    //Added by Socrate Louis Deriza, L3C1
                                    //   getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.MOVE);
                                    int nbCase = actionMove(hexaHovered);
                              //      new Hexa(HexaType.GROUND, 10, 10)
                               //     getEndGameDataCharacterFromTheList(currentCharControlled).addMovement(nbCase);                                    
                                    break;
                                case ActionType.ATK1:
                                    int dmg = actionUseAttack(actionType, hexaHovered);
                                    //Added by Socrate Louis Deriza, L3C1
                                    getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK1);
                                    saveDataBasicAttack(currentCharControlled, hexaHovered, dmg);
                                    /*
                                    if (currentCharControlled.getName() == "Soigneur")
                                    {
                                        if (hexaHovered.charOn != null)
                                        {
                                            getEndGameDataCharacterFromTheList(hexaHovered.charOn).addAnHealingAction(dmg);
                                        }                         
                                    }
                                    else if (currentCharControlled.getName() == "Valkyrie" || currentCharControlled.getName() == "Mage")
                                    {
                                        List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled, 1);
                                        for (int i = 0; i < listCharacter.Count; i++)
                                        {
                                            getEndGameDataCharacterFromTheList(currentCharControlled).addDamage(dmg);
                                        }
                                    }
                                    else
                                    {
                                        if(currentCharControlled.getName() != "Envouteur")
                                        {
                                            
                                            getEndGameDataCharacterFromTheList(currentCharControlled).addDamage(dmg);
                                            
                                        }
                                    }*/
                                    break;
                                case ActionType.ATK2:
                                    //Added by Socrate Louis Deriza, L3C1
                                    int dmg2 = actionUseAttack(actionType, hexaHovered);
                                    getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK2);
                                    saveDataSkill1(currentCharControlled, hexaHovered, dmg2);
                                    /*
                                    if (currentCharControlled.getName() == "Soigneur")
                                    {
                                        List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled, 2);
                                        for (int i = 0; i < listCharacter.Count; i++)
                                        {
                                            getEndGameDataCharacterFromTheList(listCharacter[i]).addAnHealingAction(dmg2);
                                        }
                                    }
                                    else if (currentCharControlled.getName() == "Valkyrie" || currentCharControlled.getName() == "Mage")
                                    {
                                        List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled, 2);
                                        for (int i = 0; i < listCharacter.Count; i++)
                                        {
                                            getEndGameDataCharacterFromTheList(listCharacter[i]).addDamage(dmg2);
                                        }
                                    }
                                    else
                                    {
                                        if (currentCharControlled.getName() != "Envouteur")
                                        {
                                            if (hexaHovered.charOn != null)
                                            {
                                                getEndGameDataCharacterFromTheList(hexaHovered.charOn).addDamage(dmg2);
                                            }
                                        }
                                    }*/
                                    break;
                                
                                case ActionType.ATK4:
                                    int dmg4 = actionUseAttack(actionType, hexaHovered);
                                    getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK4);
                                    saveDataSkill3(currentCharControlled, hexaHovered, dmg4);
                                    break;
                                case ActionType.ATK3:
                                    int dmg3 = actionUseAttack(actionType, hexaHovered);
                                    getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK3);
                                    saveDataSkill2(currentCharControlled, hexaHovered, dmg3);
                                    break;
                                /*
                                if (currentCharControlled.getName() == "Soigneur")
                                {
                                    List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled, 3);
                                    for (int i = 0; i < listCharacter.Count; i++)
                                    {
                                        getEndGameDataCharacterFromTheList(listCharacter[i]).addAnHealingAction(dmg3);
                                    }
                                }
                                else if (currentCharControlled.getName() == "Voleur")
                                {

                                    if (hexaHovered.charOn != null)
                                    {
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAnHealingAction(dmg3);
                                        getEndGameDataCharacterFromTheList(hexaHovered.charOn).addDamage(dmg3);
                                    }
                                }
                                else if (currentCharControlled.getName() == "Valkyrie" || currentCharControlled.getName() == "Mage" || currentCharControlled.getName() == "Archer" || currentCharControlled.getName() == "Guerrier")
                                {
                                    List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled, 3);
                                    for (int i = 0; i < listCharacter.Count; i++)
                                    {
                                        getEndGameDataCharacterFromTheList(listCharacter[i]).addDamage(dmg3);
                                    }
                                }
                                else
                                {
                                    if (currentCharControlled.getName() != "Envouteur")
                                    {
                                        if (hexaHovered.charOn != null)
                                        {
                                            getEndGameDataCharacterFromTheList(hexaHovered.charOn).addDamage(dmg3);
                                        }
                                    }
                                }*/

                                // Added ATK3 by Youcef MEDILEH, L3C1
                                case ActionType.SKIP:
                                    //Added by Socrate Louis Deriza, L3C1
                                    currentCharControlled.PA = 0;
                                    nextTurn();
                                    getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.SKIP);
                                    break;
                            }
                        }
                    }
                }
                //If the player isnot human
                else
                {
                    if (newTurn > AIwaitingTime)
                    {
                        if (decisionSequence.Count == 0 && decisionSequenceCPU.Count == 0)
                        {
                            switch (currentPlayerType)
                            {
                                case PlayerType.HUMAN: // failsafe
                                case PlayerType.AI_CPU_Defense: decisionSequenceCPU = CPU.decideDefense(currentCharControlled.team, hexaGrid); break;
                                case PlayerType.AI_CPU_Offense: decisionSequenceCPU = CPU.decideOffense(currentCharControlled.team, hexaGrid, caseBonus); break;
                                //case PlayerType.AI_CPU: decisionSequenceCPU = CPU.chooseStrategy(currentCharControlled.team, hexaGrid);  break;


                                /*case PlayerType.AI_CPU: if(CPUStrategy == 0) decisionSequenceCPU = CPU.decideDefense(currentCharControlled.team, hexaGrid);  break;
								 *					      else if(CPUStrategy == 1) decisionSequenceCPU = CPU.decideOffense(currentCharControlled.team, hexaGrid);  break;
								 *					      else if(CPUStrategy == 2) decisionSequenceCPU = CPU.chooseStrategy(currentCharControlled.team, hexaGrid);  break;	
								*/

                                case PlayerType.AI_EASY: decisionSequence = AIEasy.decide(currentCharControlled); break;
                                case PlayerType.AI_MEDIUM: decisionSequence = AIMedium.decide(currentCharControlled, statsGame); break; //change for the AI medium afterwards
                                case PlayerType.AI_HARD: decisionSequence = AIHard.decide(currentCharControlled, statsGame); break;
                            }
                        }
                        else
                        {
                            if (decisionSequence.Count > 0)
                            {
                                ActionAIPos actionAIPos = decisionSequence[0];
                                decisionSequence.RemoveAt(0);
                                //Debug.Log(currentPlayerType + " " + charToString(currentCharControlled) + " : " + actionAIPos.action + ((actionAIPos.pos != null) ? (" " + actionAIPos.pos.x + " " + actionAIPos.pos.y) : ""));
                                //Debug.Log("Targeted hexa : " + charToString(hexaGrid.getHexa(actionAIPos.pos.x, actionAIPos.pos.y).charOn));

                                switch (actionAIPos.action)
                                {

                                    case ActionType.MOVE: 
                                        int nbCase = actionMove(hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addMovement(nbCase);
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.MOVE);
                                        break;
                                    case ActionType.ATK1:
                                        int dmg = actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK1);
                                        saveDataBasicAttack(currentCharControlled, hexaHovered, dmg);
                                        break;
                                    case ActionType.ATK2:
                                        int dmg2 = actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK2);
                                        saveDataSkill1(currentCharControlled, hexaHovered, dmg2);
                                        break;
                                    case ActionType.ATK3:
                                        int dmg3 = actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK3);
                                        saveDataSkill2(currentCharControlled, hexaHovered, dmg3);
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK3);
                                        break;
                                    case ActionType.ATK4:
                                        int dmg4 = actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK4);
                                        saveDataSkill3(currentCharControlled, hexaHovered, dmg4);
                                        break;
                                    case ActionType.SKIP:
                                        currentCharControlled.PA = 0;
                                        nextTurn();
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.SKIP);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else // decisionSequenceCPU.Count > 0
                            {
                                setControlledCharacter(decisionSequenceCPU[0].chr);
                                ActionAIPos actionAIPos = decisionSequenceCPU[0].act;
                                decisionSequenceCPU.RemoveAt(0);


                                /*//Debug.Log(currentPlayerType + " " + charToString(currentCharControlled) + " : " + actionAIPos.action + ((actionAIPos.pos != null) ? (" " + actionAIPos.pos.x + " " + actionAIPos.pos.y) : ""));
                                if (actionAIPos.pos != null)
                                    Debug.Log(actionAIPos.action + " to hexa " + (actionAIPos.pos != null ? (actionAIPos.pos.x + " " + actionAIPos.pos.y) : "null")
                                        + " : " + charToString(hexaGrid.getHexa(actionAIPos.pos.x, actionAIPos.pos.y).charOn));*/

                                switch (actionAIPos.action)
                                {
                                    case ActionType.MOVE:
                                        int nbCase = actionMove(hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addMovement(nbCase);
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.MOVE);
                                        break;
                                    case ActionType.ATK1:
                                        int dmg = actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK1);
                                        saveDataBasicAttack(currentCharControlled, hexaHovered, dmg);
                                        break;
                                    case ActionType.ATK2:
                                        int dmg2 = actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK2);
                                        saveDataSkill1(currentCharControlled, hexaHovered, dmg2);
                                        break;
                                    case ActionType.ATK3:
                                        int dmg3 = actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK3);
                                        saveDataSkill2(currentCharControlled, hexaHovered, dmg3);
                                        break;
                                    case ActionType.ATK4:
                                        int dmg4 = actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos));
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.ATK4);
                                        saveDataSkill3(currentCharControlled, hexaHovered, dmg4);
                                        break;
                                    case ActionType.SKIP:
                                        currentCharControlled.PA = 0;
                                        getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.SKIP);
                                        nextTurn();
                                        break;

                                    default: break;
                                }
                            }
                        }
                    }
                }
            }
        }
        // DISPLAY WINNER if there is a winner
        else
        {

            UIPauseMenu.SetActive(false);
            //Edited by L3C1 CROUZET Oriane, 08/04/2023
            UICharTurns.SetActive(false);
            UITooltip.SetActive(false);
            UIAction.SetActive(false);
            UIVictoryScreen.SetActive(true);
            Debug.Log("Fin de la partie");
            //for (int i = 0; i < 5; i++)
            //{
            //if (i < startGameData.charsTeam1.Count) Debug.Log(startGameData.charsTeam1[i].ToString()); 
            //if (i < startGameData.charsTeam2.Count) hexaGrid.addChar(startGameData.charsTeam2[i], tileW / 2 - 4 + 2 + i, 2, 1);
            //}
            displayAllEndData(endGameDataCharacter);
            UIVictoryScreen.transform.GetChild(1).GetComponent<Text>().text = "VICTOIRE DE L'EQUIPE " + ((winner == 0) ? "BLEUE" : "ROUGE");
            UIVictoryScreen.transform.GetChild(1).GetComponent<Text>().color = ((winner == 0) ? Character.TEAM_1_COLOR : Character.TEAM_2_COLOR);


            /*
            for(int i = 0; i< endGameDataCharacter.Length; i++)
            {
                Debug.Log("Nom: "+endGameDataCharacter[i].typeCharacter.ToString());
                Debug.Log("Team : " + endGameDataCharacter[i].teamCharacter);
                Debug.Log("Dommage de son équipe: "+ endGameDataCharacter[i].getDamagePercentageOfTHisTeam());
                Debug.Log("Pourcentage de point de vie restaurés: " + endGameDataCharacter[i].getPercentageOfHpRestored());
                Debug.Log("Pourcentage d'activité du personnage: "+endGameDataCharacter[i].getPercentageOfActivity());
                Debug.Log("Nombre de cases parcourrus: "+ endGameDataCharacter[i].getnumberOfSlotCrossedByTheCharacter());
                Debug.Log("nombre d'ennemie tués: "+ endGameDataCharacter[i].numberOfOpponentsEliminated);
            }*/



            // Back to menu
            if (Input.GetMouseButtonDown(0))
            {
                if (mousePos.x >= Screen.width / 2 - 205 && mousePos.x < Screen.width / 2  && mousePos.y >= Screen.height / 2 - 90 && mousePos.y < Screen.height / 2 - 90 + 40)
                {
                    // EVALUATE AI HARD
                    // to delete

                    statsGame.endGame(winner, hexaGrid);
                    if (AIHard.learn) statsGame.evaluateGame();
                    statsGame.evaluateGame();
                    MainMenu.startGameData = new StartGameData();
                    MainMenu.startGameData.slider = slider.value;
                    endGameDataCharacter = null;
                    SceneManager.LoadScene(0);
                }

                //Author : L3C1 CROUZET Oriane, 07/05/2023
                else if (mousePos.x >= Screen.width / 2  && mousePos.x < Screen.width / 2 + 205 && mousePos.y >= Screen.height / 2 - 90 && mousePos.y < Screen.height / 2 - 90 + 40)
                {
                    SceneManager.LoadScene(1);
                }
            }
        } //mousePos.x >= Screen.width / 2 - 100 && mousePos.x < Screen.width / 2 + 100 && mousePos.y >= Screen.height / 2 - 90 && mousePos.y < Screen.height / 2 - 90 + 40

        // - DISPLAYS -------------------------------------------------------------------------------


        // CENTER CHARACTER MODEL
        foreach (Character c in hexaGrid.charList)
        {

            if (c.go.transform.GetChild(1)) c.go.transform.GetChild(1).transform.position = c.go.transform.position;

        }
        if (winner == -1)
        {

            // Display arrow above the current character controlled
            {
                float currentHeight = (((newTurn % 60) < 30) ? (newTurn % 60) : (60 - (newTurn % 60))) / 60.0f * 0.2f;
                arrow.transform.position = new Vector3(currentCharControlled.go.transform.position.x, currentHeight + 1.5f, currentCharControlled.go.transform.position.z);
                if (newTurn == 0)
                {
                    hexaHighlight.GetComponent<MeshFilter>().mesh = hexaHollowMesh;
                }
                else if (newTurn < 10)
                {
                    hexaHighlight.transform.localScale = new Vector3(1 + (10 - newTurn) * 0.3f, 1 + (10 - newTurn) * 0.3f, 1);
                }
                else if (newTurn == 10)
                {
                    hexaHighlight.transform.localScale = new Vector3(1, 1, 1);
                    hexaHighlight.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
                }
                hexaHighlight.transform.position = new Vector3(currentCharControlled.go.transform.position.x, -0.013f, currentCharControlled.go.transform.position.z);
                newTurn++;
            }

            if (updateMouseHover)
            {
                // Clear previous hexas displayed
                foreach (GameObject go in pathFinderDisplay) GameObject.Destroy(go);
                pathFinderDisplay = new List<GameObject>();
                foreach (GameObject go in lineOfSightDisplay) GameObject.Destroy(go);
                lineOfSightDisplay = new List<GameObject>();
                // Display hovered hexa
                displayHoveredHexa();
            }

            // Display path in green / line of sight in blue / AoE in red
            if (pathWalk == null && updateMouseHover && whoControlsThisChar(currentCharControlled) == PlayerType.HUMAN)
            {
                switch (actionType)
                {
                    case ActionType.MOVE: displayPossiblePaths(); displaySortestPath(); if (Input.GetKey(KeyCode.LeftControl)) displayLineOfSight(); break; //now displays the attack range of the controlled character if we press "Left Control". 
                    case ActionType.ATK1: case ActionType.ATK2: case ActionType.ATK3: case ActionType.ATK4: displayLineOfSight(); break;
                    case ActionType.SKIP: break;
                }
            }

            // Display UI
            if (updateUI)
            {
                displayNewCharTurnList();
                displayActionButtons();
            }
        }


        updateMouseHover = false;
        updateUI = false;
    }

    // ##################################################################################################################################################
    // Functions used in main
    // ##################################################################################################################################################


    //Author : Yuting HUANG, L3C1
    //ajout d'un temps limité
    void TimeLimite()
    {

        if (TimeNum > 0)
        {
            TimeNum -= Time.deltaTime;
            TimeRoleTureSlider.value = TimeNum;
        }
        else
        {
            TimeNum = MaxTimeNum;
            nextTurn();
        }
    }

    //Author : VALAT Thibault L3Q1
    //Initialize a random bonus between : center-left of the map, center, or center right
    Hexa initBonus()
    {

        int bonusPlace = UnityEngine.Random.Range(1, 4);
        int x = 0;
        int y = 0;

        switch (bonusPlace)
        {
            case 1:
                displayBonus(hexaGrid.findAllPaths(5, 16, 2));
                x = 5;
                y = 16;
                break;
            case 2:
                displayBonus(hexaGrid.findAllPaths(17, 15, 2));
                x = 17;
                y = 15;
                break;
            case 3:
                displayBonus(hexaGrid.findAllPaths(29, 15, 2));
                x = 29;
                y = 15;
                break;
        }
        return hexaGrid.getHexa(x, y);
    }
    
    //Added by Socrate Louis Deriza, L3C1
    void saveDataBasicAttack(Character currentCharControlled1, Hexa hexaHovered1, int dmg1)
    {
        if (currentCharControlled1.getName() == "Soigneur")
        {
            if (hexaHovered1.charOn != null)
            {
                getEndGameDataCharacterFromTheList(hexaHovered1.charOn).addAnHealingAction(dmg1);
            }
        }
        else if (currentCharControlled1.getName() == "Valkyrie" || currentCharControlled1.getName() == "Mage")
        {
            List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled1, 1);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                getEndGameDataCharacterFromTheList(currentCharControlled1).addDamage(dmg1);
            }
        }
        else
        {
            if (currentCharControlled1.getName() != "Envouteur")
            {
                getEndGameDataCharacterFromTheList(currentCharControlled1).addDamage(dmg1);
            }
        }
    }

    // Added by Soccrate Louis Deriza, L3C1

    void displayAllEndData(EndGameDataCharacter[] endGameDataCharacterToDisplay)
    {
        EndGameDataCharacter[] data1 = getEndData1(endGameDataCharacterToDisplay);
        EndGameDataCharacter[] data2 = getEndData2(endGameDataCharacterToDisplay);
        EndGameDataCharacter[] data3 = getEndData3(endGameDataCharacterToDisplay);
        EndGameDataCharacter[] data4 = getEndData4(endGameDataCharacterToDisplay);
        EndGameDataCharacter[] data5 = getEndData5(endGameDataCharacterToDisplay);

        for(int i = 0; i<5; i++)
        {

            if (i == 0)
            {
                GameObject gData10 = GameObject.Find("statData1Team0");
                Text textGData10 = gData10.GetComponent<Text>();
                textGData10.text = "plus gros dêgâts de son équipe: " + (data1[0].typeCharacter).ToString() + "(" + (data1[0]).getDamagePercentageOfTHisTeam()+ " pourcents)";
                textGData10.color = new Color(0.125f, 0.125f, 1);
                GameObject gData11 = GameObject.Find("statData1Team1");
                Text textGData11 = gData11.GetComponent<Text>();
                textGData11.text = "plus gros dêgâts de son équipe: " + (data1[1].typeCharacter).ToString() + "(" + (data1[1]).getDamagePercentageOfTHisTeam() + " pourcents)";
                textGData11.color = new Color(1, 0.125f, 0);

            }
            else if (i == 1)
            {
                GameObject gData20 = GameObject.Find("statData2Team0");
                Text textGData20 = gData20.GetComponent<Text>();
                textGData20.text = "plus gros tueur de son équipe: "+ (data2[0].typeCharacter).ToString()+"("+ (data2[0]).numberOfOpponentsEliminated+ " kills)";
                textGData20.color = new Color(0.125f, 0.125f, 1);
                GameObject gData21 = GameObject.Find("statData2Team1");
                Text textGData21 = gData21.GetComponent<Text>();
                textGData21.text = "plus gros tueur de son équipe: " + (data2[1].typeCharacter).ToString() + "(" + (data2[1]).numberOfOpponentsEliminated+ " kills)";
                textGData21.color = new Color(1, 0.125f, 0);

            }
            else if (i == 2)
            {

                GameObject gData30 = GameObject.Find("statData3Team0");
                Text textGData30 = gData30.GetComponent<Text>();
                textGData30.text = "plus grosse activité de son équipe: " + (data3[0].typeCharacter).ToString() + "(" + (data3[0]).getPercentageOfActivity()+" pourcents d'utilisation d'attaque  ou de compétences)";
                textGData30.color = new Color(0.125f, 0.125f, 1);
                GameObject gData31 = GameObject.Find("statData3Team1");
                Text textGData31 = gData31.GetComponent<Text>();
                textGData31.text = "plus grosse activité de son équipe: " + (data3[1].typeCharacter).ToString() + "(" + (data3[1]).getPercentageOfActivity()+" pourcents d'utilisation d'attaque  ou de compétences)";
                textGData31.color = new Color(1, 0.125f, 0);


            }
            else if (i == 3)
            {
                GameObject gData40 = GameObject.Find("statData4Team0");
                Text textGData40 = gData40.GetComponent<Text>();
                textGData40.text = "plus grosse regéneration de son équipe: " + (data4[0].typeCharacter).ToString() + "(" + (data4[0]).getPercentageOfHpRestored()+ " pourcents de ses points de vie retrouvés)";
                textGData40.color = new Color(0.125f, 0.125f, 1);
                GameObject gData41 = GameObject.Find("statData4Team1");
                Text textGData41 = gData41.GetComponent<Text>();
                textGData41.text = "plus grosse regéneration de son équipe: " + (data4[1].typeCharacter).ToString() + "(" + (data4[1]).getPercentageOfHpRestored() + " pourcents de ses points de vie retrouvés)";
                textGData41.color = new Color(1, 0.125f, 0);
            }
            else
            {
                GameObject gData50 = GameObject.Find("statData5Team0");
                Text textGData50 = gData50.GetComponent<Text>();
                textGData50.text = "plus gros déplacements de son équipe: " + (data5[0].typeCharacter).ToString() + "(" + (data5[0]).getnumberOfSlotCrossedByTheCharacter()+ " cases)";
                textGData50.color = new Color(0.125f, 0.125f, 1);
                GameObject gData51 = GameObject.Find("statData5Team1");
                Text textGData51 = gData51.GetComponent<Text>();
                textGData51.text = "plus gros déplacements de son équipe: " + (data5[1].typeCharacter).ToString() + "(" + (data5[1]).getnumberOfSlotCrossedByTheCharacter()+ " cases)";
                textGData51.color = new Color(1, 0.125f, 0);

            }

        }
    }

    //Added by Socrate Louis Deriza, L3C1
    EndGameDataCharacter[] getEndData1(EndGameDataCharacter[] endGameDataCharacterToDisplay)
    {
        EndGameDataCharacter[] endData1 = new EndGameDataCharacter[2];
        int limit = endGameDataCharacterToDisplay.Length / 2;
        float percentageDamageMax = 0.0f;
        EndGameDataCharacter firstElement = endGameDataCharacterToDisplay[0];
        for (int i = 0; i<limit; i++)
        {
            if (percentageDamageMax < endGameDataCharacterToDisplay[i].getDamagePercentageOfTHisTeam())
            {
                percentageDamageMax = endGameDataCharacterToDisplay[i].getDamagePercentageOfTHisTeam();
                firstElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData1[0] = firstElement;
        percentageDamageMax = 0.0f;
        EndGameDataCharacter secondElement = endGameDataCharacterToDisplay[limit];
        for (int i = limit; i < endGameDataCharacterToDisplay.Length; i++)
        {
            if (percentageDamageMax < endGameDataCharacterToDisplay[i].getDamagePercentageOfTHisTeam())
            {
                percentageDamageMax = endGameDataCharacterToDisplay[i].getDamagePercentageOfTHisTeam();
                secondElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData1[1] = secondElement;

        return endData1;
    }

    //Added by Socrate Louis Deriza, L3C1
    EndGameDataCharacter[] getEndData2(EndGameDataCharacter[] endGameDataCharacterToDisplay)
    {
        EndGameDataCharacter[] endData2 = new EndGameDataCharacter[2];
        int limit = endGameDataCharacterToDisplay.Length / 2;
        int maxKill = 0;
        EndGameDataCharacter firstElement = endGameDataCharacterToDisplay[0];
        for (int i = 0; i < limit; i++)
        {
            if (maxKill < endGameDataCharacterToDisplay[i].numberOfOpponentsEliminated)
            {
                maxKill = endGameDataCharacterToDisplay[i].numberOfOpponentsEliminated;
                firstElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData2[0] = firstElement;
        maxKill = 0;
        EndGameDataCharacter secondElement = endGameDataCharacterToDisplay[limit];
        for (int i = limit; i < endGameDataCharacterToDisplay.Length; i++)
        {
            if (maxKill < endGameDataCharacterToDisplay[i].numberOfOpponentsEliminated)
            {
                maxKill = endGameDataCharacterToDisplay[i].numberOfOpponentsEliminated;
                secondElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData2[1] = secondElement;

        return endData2;
    }

    //Added by Socrate Louis Deriza, L3C1
    EndGameDataCharacter[] getEndData3(EndGameDataCharacter[] endGameDataCharacterToDisplay)
    {
        EndGameDataCharacter[] endData3 = new EndGameDataCharacter[2];
        int limit = endGameDataCharacterToDisplay.Length / 2;
        float percentageActivityMax = 0.0f;
        EndGameDataCharacter firstElement = endGameDataCharacterToDisplay[0];
        for (int i = 0; i < limit; i++)
        {
            if (percentageActivityMax < endGameDataCharacterToDisplay[i].getPercentageOfActivity())
            {
                percentageActivityMax = endGameDataCharacterToDisplay[i].getPercentageOfActivity();
                firstElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData3[0] = firstElement;
        percentageActivityMax = 0.0f;
        EndGameDataCharacter secondElement = endGameDataCharacterToDisplay[limit];
        for (int i = limit; i < endGameDataCharacterToDisplay.Length; i++)
        {
            if (percentageActivityMax < endGameDataCharacterToDisplay[i].getPercentageOfActivity())
            {
                percentageActivityMax = endGameDataCharacterToDisplay[i].getPercentageOfActivity();
                secondElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData3[1] = secondElement;

        return endData3;
    }

    //Added by Socrate Louis Deriza, L3C1
    EndGameDataCharacter[] getEndData4(EndGameDataCharacter[] endGameDataCharacterToDisplay)
    {
        EndGameDataCharacter[] endData4 = new EndGameDataCharacter[2];
        int limit = endGameDataCharacterToDisplay.Length / 2;
        float percentageRegenerationMax = 0.0f;
        EndGameDataCharacter firstElement = endGameDataCharacterToDisplay[0];
        for (int i = 0; i < limit; i++)
        {
            if (percentageRegenerationMax < endGameDataCharacterToDisplay[i].getPercentageOfHpRestored())
            {
                percentageRegenerationMax = endGameDataCharacterToDisplay[i].getPercentageOfHpRestored();
                firstElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData4[0] = firstElement;
        percentageRegenerationMax = 0.0f;
        EndGameDataCharacter secondElement = endGameDataCharacterToDisplay[limit];
        for (int i = limit; i < endGameDataCharacterToDisplay.Length; i++)
        {
            if (percentageRegenerationMax < endGameDataCharacterToDisplay[i].getPercentageOfHpRestored())
            {
                percentageRegenerationMax = endGameDataCharacterToDisplay[i].getPercentageOfHpRestored();
                secondElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData4[1] = secondElement;

        return endData4;
    }

    //Added by Socrate Louis Deriza, L3C1
    EndGameDataCharacter[] getEndData5(EndGameDataCharacter[] endGameDataCharacterToDisplay)
    {
        EndGameDataCharacter[] endData5 = new EndGameDataCharacter[2];
        int limit = endGameDataCharacterToDisplay.Length / 2;
        int nbCase = 0;
        EndGameDataCharacter firstElement = endGameDataCharacterToDisplay[0];
        for (int i = 0; i < limit; i++)
        {
            if (nbCase < endGameDataCharacterToDisplay[i].getnumberOfSlotCrossedByTheCharacter())
            {
                nbCase = endGameDataCharacterToDisplay[i].getnumberOfSlotCrossedByTheCharacter();
                firstElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData5[0] = firstElement;
        nbCase = 0;
        EndGameDataCharacter secondElement = endGameDataCharacterToDisplay[limit];
        for (int i = limit; i < endGameDataCharacterToDisplay.Length; i++)
        {
            if (nbCase < endGameDataCharacterToDisplay[i].getnumberOfSlotCrossedByTheCharacter())
            {
                nbCase = endGameDataCharacterToDisplay[i].getnumberOfSlotCrossedByTheCharacter();
                secondElement = endGameDataCharacterToDisplay[i];
            }
        }
        endData5[1] = secondElement;

        return endData5;
    }

    //Added by Socrate Louis Deriza, L3C1
    void saveDataSkill1(Character currentCharControlled2, Hexa hexaHovered2, int dmg2)
    {
        if (currentCharControlled2.getName() == "Soigneur")
        {
            List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled2, 2);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                getEndGameDataCharacterFromTheList(listCharacter[i]).addAnHealingAction(dmg2);
            }
        }
        else if (currentCharControlled2.getName() == "Valkyrie" || currentCharControlled2.getName() == "Mage")
        {
            List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled2, 2);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                getEndGameDataCharacterFromTheList(currentCharControlled2).addDamage(dmg2);
            }
        }
        else
        {
            if (currentCharControlled2.getName() != "Envouteur")
            {
                getEndGameDataCharacterFromTheList(currentCharControlled2).addDamage(dmg2);
            }
        }
    }

    //Added by Socrate Louis Deriza, L3C1
    void saveDataSkill2(Character currentCharControlled3, Hexa hexaHovered3, int dmg3)
    {
        if (currentCharControlled3.getName() == "Soigneur")
        {
            List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled3, 3);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                getEndGameDataCharacterFromTheList(listCharacter[i]).addAnHealingAction(dmg3);
            }
        }
        else if (currentCharControlled3.getName() == "Voleur")
        {            
            getEndGameDataCharacterFromTheList(currentCharControlled3).addAnHealingAction(dmg3);
            getEndGameDataCharacterFromTheList(currentCharControlled3).addDamage(dmg3);           
        }
        else if (currentCharControlled3.getName() == "Valkyrie" || currentCharControlled3.getName() == "Mage" || currentCharControlled3.getName() == "Archer" || currentCharControlled3.getName() == "Guerrier")
        {
            List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled3, 3);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                getEndGameDataCharacterFromTheList(currentCharControlled3).addDamage(dmg3);
            }
        }
        else
        {
            if (currentCharControlled3.getName() != "Envouteur")
            {
                getEndGameDataCharacterFromTheList(currentCharControlled3).addDamage(dmg3);
            }
        }
    }

    //Added by Julien D'aboville, L3L1 (a revoir)

    void saveDataSkill3(Character currentCharControlled3, Hexa hexaHovered3, int dmg3)
    {
        if (currentCharControlled3.getName() == "Soigneur")
        {
            List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled3, 3);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                getEndGameDataCharacterFromTheList(listCharacter[i]).addAnHealingAction(dmg3);
            }
        }
        else if (currentCharControlled3.getName() == "Voleur")
        {
            getEndGameDataCharacterFromTheList(currentCharControlled3).addAnHealingAction(dmg3);
            getEndGameDataCharacterFromTheList(currentCharControlled3).addDamage(dmg3);
        }
        else if (currentCharControlled3.getName() == "Valkyrie" || currentCharControlled3.getName() == "Mage" || currentCharControlled3.getName() == "Archer" || currentCharControlled3.getName() == "Guerrier")
        {
            List<Character> listCharacter = getAllAllyOrEnnemyInRangeAttack(currentCharControlled3, 3);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                getEndGameDataCharacterFromTheList(currentCharControlled3).addDamage(dmg3);
            }
        }
        else
        {
            if (currentCharControlled3.getName() != "Envouteur")
            {
                getEndGameDataCharacterFromTheList(currentCharControlled3).addDamage(dmg3);
            }
        }
    }

    //Added by Socrate Louis Deriza, L3C1
    public List<Character> getAllAllyOrEnnemyInRangeAttack(Character charac, int numberOfAttack)
    {
        List<Character> listInRange = new List<Character>();
        foreach (Character c in hexaGrid.charList)
        {
            if(charac.getName() == "Soigneur")
            {
                if(numberOfAttack == 3 && charac.team == c.team)
                {
                    if (isInRangeToUseSkill2(charac, c) == true)
                    {
                        listInRange.Add(c);
                    }
                }
                if (numberOfAttack == 2 && charac.team == c.team)
                {
                    if (isInRangeToUseSkill(charac, c) == true)
                    {
                        if(c.getName() != charac.getName() || c.getX() != charac.getX() || c.getY() != charac.getY())
                        {
                            listInRange.Add(c);
                        }
                    }
                }
                if (numberOfAttack == 1 && charac.team == c.team)
                {
                    if(isInRangeToAttack(charac, c))
                    {
                        if (c.getName() != charac.getName() || c.getX() != charac.getX() || c.getY() != charac.getY())
                        {
                            listInRange.Add(c);
                        }
                    }
                }

            }
            else
            {
                if (numberOfAttack == 3 && charac.team != c.team)
                {
                    if (isInRangeToUseSkill2(charac, c) == true)
                    {
                        listInRange.Add(c);
                    }
                }
                if (numberOfAttack == 2 && charac.team != c.team)
                {
                    if (isInRangeToUseSkill(charac, c) == true)
                    {
                        listInRange.Add(c);
                    }
                }
                if (numberOfAttack == 1 && charac.team != c.team)
                {
                    if (isInRangeToAttack(charac, c))
                    {
                        listInRange.Add(c);
                    }
                }
            }
        }
        return listInRange;
    }
    //Author : VALAT Thibault L3Q1
    //Display Bonus hexas in gray (neutral)
    void displayBonus(List<Point> points)
    {
        Debug.Log("displayBonus");
        if (points == null)
        {
            Debug.Log("points.Count = null");
        }
        Debug.Log("points.Count = " + points.Count);
        foreach (Point p in points)
        {
            Hexa point = hexaGrid.getHexa(p);
            if (point.type == HexaType.WALL)
            {
                point.changeType(HexaType.GROUND);
            }
            point.changeType(HexaType.BONUS);
            bonusPoints.Add(point);

            GameObject go = GameObject.Instantiate(hexaTemplate, hexasFolder.transform);
            go.SetActive(true);
            go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x, p.y, -0.015f);
            go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
            go.GetComponent<Renderer>().sharedMaterial = bonusDisplayMat;
            go.GetComponent<Collider>().enabled = false;
            bonusDisplayMat.color = neutralBonusDisplayColor;
        }

    }

    //Author : VALAT Thibault L3Q1
    //Display Bonus hexas in gray (neutral), red or blue and change during the game
    void checkAndUpdateBonusControll()
    {

        bool redInBonusZone = false;
        bool blueInBonusZone = false;

        //Check if there is zero or one or two teams in the bonus zone
        foreach (Character c in hexaGrid.charList)
        {
            if (hexaGrid.getHexa(c.x, c.y).type == HexaType.BONUS && c.team == 0)
                blueInBonusZone = true;
            if (hexaGrid.getHexa(c.x, c.y).type == HexaType.BONUS && c.team == 1)
                redInBonusZone = true;
        }
        //Reset the bonus zone
        if ((!redInBonusZone && !blueInBonusZone) || (redInBonusZone && blueInBonusZone))
        {
            changeBonusColor(neutralBonusDisplayColor);
            giveBonusValue(-1);
        }
        //Give the bonus to the red team
        else if (redInBonusZone && !blueInBonusZone)
        {
            changeBonusColor(redBonusDisplayColor);
            giveBonusValue(1);
        }
        //Give the bonus to the blue team
        else if (!redInBonusZone && blueInBonusZone)
        {
            changeBonusColor(blueBonusDisplayColor);
            giveBonusValue(0);
        }

    }

    //Change the display of the bonus hexas and the bonus card
    //Author : VALAT Thibault L3Q1
    void changeBonusColor(Color teamColor)
    {
        //Change the color of the hexas
        foreach (Hexa h in bonusPoints)
        {
            GameObject go = GameObject.Instantiate(hexaTemplate, hexasFolder.transform);
            go.SetActive(true);
            go.GetComponent<Transform>().position = Hexa.hexaPosToReal(h.x, h.y, -0.015f);
            go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
            go.GetComponent<Renderer>().sharedMaterial = bonusDisplayMat;
            go.GetComponent<Collider>().enabled = false;
            bonusDisplayMat.color = teamColor;
        }

        //Change the color of the bonus card
        switch (bonusTeam)
        {
            case -1:
                BonusCard.transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                BonusCard.transform.GetChild(2).GetComponent<Text>().text = "Bonus"; break;
            case 1:
                BonusCard.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0.2f, 0.2f);
                BonusCard.transform.GetChild(2).GetComponent<Text>().text = "+1 HP"; break;
            case 0:
                BonusCard.transform.GetChild(0).GetComponent<Image>().color = new Color(0.2f, 0.2f, 1);
                BonusCard.transform.GetChild(2).GetComponent<Text>().text = "+1 HP"; break;
        }

    }

    //Author Louis Deriza Socrate L3C1, 15/04/2023
    EndGameDataCharacter[] createEndDataGameList(int sizeEndGameData)
    {
        EndGameDataCharacter[] endGameDataTab = new EndGameDataCharacter[sizeEndGameData];

        for (int i = 0; i < 5; i++)
        {
            if (i < startGameData.charsTeam1.Count)
            {
                Character c = hexaGrid.getCharacterOnHexa(tileW / 2 - 4 + 2 + i, tileH - 2);
                endGameDataTab[i] = new EndGameDataCharacter(c , startGameData.charsTeam1[i], 0);
                
                //hexaGrid.addChar(startGameData.charsTeam1[i], , , 0);
            }
            if (i < startGameData.charsTeam2.Count)
            {
                Character c = hexaGrid.getCharacterOnHexa(tileW / 2 - 4 + 2 + i, 2);
                endGameDataTab[((sizeEndGameData/2)  + i)] = new EndGameDataCharacter(c, startGameData.charsTeam2[i], 1);
                //hexaGrid.addChar(startGameData.charsTeam2[i], , , 1);
            }
        }
        return endGameDataTab;
    }

    //Added by Socrate Louis Deriza, L3C1
    EndGameDataCharacter getEndGameDataCharacterFromTheList(Character character)
    {
        for (int i = 0; i<endGameDataCharacter.Length; i++)
        {
            if (endGameDataCharacter[i].character == character)
            {
                return endGameDataCharacter[i];
            }
        }
        return null;
    }

    //Give the +1HP bonus to the team controlling the bonus
    //Author : VALAT Thibault L3Q1  
    void giveBonusValue(int bonusControlledTeam)
    {
        //Reset the bonus value if the team which had it lost it
        if (bonusControlledTeam == -1 && bonusTeam != -1)
        {
            foreach (Character c in hexaGrid.charList)
                if (c.team == bonusTeam)
                    c.HP--;
            bonusTeam = -1;
        }
        //Give the bonus to the blue team
        else if (bonusControlledTeam == 0 && bonusTeam != 0)
        {
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team == bonusTeam)
                    c.HP--;
                if (c.team == bonusControlledTeam)
                    c.HP++;
            }
            bonusTeam = 0;
        }
        //Give the bonus to the red team
        else if (bonusControlledTeam == 1 && bonusTeam != 1)
        {
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team == bonusTeam)
                    c.HP--;
                if (c.team == bonusControlledTeam)
                    c.HP++;
            }
            bonusTeam = 1;
        }
    }

    // Return the team with the active bonus
    //Author : GOUVEIA Klaus L3Q1
    public static int getBonusTeam()
    {
        return bonusTeam;
    }


    // Return the playerType of character (humaun ...)
    //Author : ??
    PlayerType whoControlsThisChar(Character c)
    {
        return (c.team == 0) ? startGameData.player1Type : startGameData.player2Type;
    }

    //Stock the current hoverred hexa
    //Author : ??
    void getHoveredHexa()
    {
        if ((mousePos.x >= 35 && mousePos.x < 35 + 140 && mousePos.y >= Screen.height - 318 && mousePos.y < Screen.height - 318 + 24) ||
        (mousePos.x >= 35 && mousePos.x < 35 + 140 && mousePos.y >= Screen.height - 348 && mousePos.y < Screen.height - 348 + 24) ||
        (mousePos.x >= 35 && mousePos.x < 35 + 140 && mousePos.y >= Screen.height - 377 && mousePos.y < Screen.height - 377 + 24) ||
        (mousePos.x >= 35 && mousePos.x < 35 + 140 && mousePos.y >= Screen.height - 406 && mousePos.y < Screen.height - 406 + 24) ||
        (mousePos.x >= Screen.width / 2 - 80 && mousePos.x < Screen.width / 2 + 80 && mousePos.y >= Screen.height - 30 && mousePos.y < Screen.height))
        {
            if (hexaHovered != null)
            {
                hexaHoveredOld = hexaHovered;
                hexaHovered = null;
                charHovered = null;
            }
            updateMouseHover = true;
            updateUI = true;
        }
        else
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            bool success = false;
            if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, 100)) success = (raycastHit.transform.gameObject.tag == "Hexa");
            if (success)
            {
                Hexa hexaHit = raycastHit.transform.gameObject.GetComponent<HexaGO>().hexa;
                if (hexaHit != hexaHovered)
                {
                    hexaHoveredOld = hexaHovered;
                    hexaHovered = hexaHit;
                    charHovered = hexaHovered.charOn;
                    updateMouseHover = true;
                    updateUI = true;
                }
            }
            else
            {
                if (hexaHovered != null)
                {
                    hexaHoveredOld = hexaHovered;
                    hexaHovered = null;
                    charHovered = null;
                }
                updateMouseHover = true;
                updateUI = true;
            }
        }
    }

    //get the controlled character
    //Author : ??
    void getControlledCharacter()
    {

        //stocks the hovered hexa
        getHoveredHexa();

        if (Input.GetMouseButton(0) && hexaHovered != null)
        {
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team == currentCharControlled.team)
                {
                    if (hexaHovered.x == c.x && hexaHovered.y == c.y)
                    {
                        currentCharControlled = c;
                    }
                }
            }
        }
    }

    //Set the controlled character
    //Author : ??
    public void setControlledCharacter(Character c)
    {

        currentCharControlled = c;

    }


    //Move the character 
    //Author : ??
    int actionMove(Hexa hexaDestination)
    {
        //Move the character when its possible
        if (hexaDestination != null && (hexaDestination.type == HexaType.GROUND || (hexaDestination.type == HexaType.BONUS)))
        {
            List<Point> path = hexaGrid.findShortestPath(currentCharControlled.x, currentCharControlled.y, hexaDestination.x, hexaDestination.y, currentCharControlled.PM);
            Debug.Log("fin méthode findShortestPath");
            if (path != null && path.Count > 1)
            {
                pathWalk = path;
                pathWalkpos = 0;
                return path.Count;
            }
            else
            {
                /*
                if (path == null)
                {
                    Debug.LogWarning("ActionMove Error(null): " + hexaDestination.x + " " + hexaDestination.y +
                    "\nHexa type : \t" + hexaDestination.type +
                    "\nOn hexa : \t" + charToString(hexaDestination.charOn));
                }
                if (path.Count == 1)
                {
                    Debug.LogWarning("ActionMove Error(=1): " + hexaDestination.x + " " + hexaDestination.y +
                    "\nHexa type : \t" + hexaDestination.type +
                    "\nOn hexa : \t" + charToString(hexaDestination.charOn));
                }
                if (path.Count < 1)
                {
                    Debug.LogWarning("ActionMove Error(>1): " + hexaDestination.x + " " + hexaDestination.y +
                    "\nHexa type : \t" + hexaDestination.type +
                    "\nOn hexa : \t" + charToString(hexaDestination.charOn));
                }*/
                return 0;
            }
        }
        else
        {
            Debug.LogWarning("ActionMove Error: No Hexa found.");
            return 0;
        }
    }

    //Attack with the current character, the hexa clicked
    //Author : ??
    // Edited BY Youcef MEDILEH, L3C1
    //Edited BY julien d'aboville L3L1
    // - Added the possibility to use the second skill
    int actionUseAttack(ActionType attack, Hexa hexaDestination)
    {
        //Set the attack used
        CharsDB.Attack attackUsed_;
        if (attack == ActionType.ATK1) attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].basicAttack;
        else if (attack == ActionType.ATK2) { attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].skill_1; }
        else if (attack == ActionType.ATK3) { attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].skill_2; }//ATK3, by Youcef MEDILEH, L3C1
        else { attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].skill_3; } 
        Debug.Log(CharsDB.list[(int)currentCharControlled.charClass]);
        //Use the attack if it's possible
        if (hexaDestination != null && hexaGrid.hexaInSight(currentCharControlled.x, currentCharControlled.y, hexaDestination.x, hexaDestination.y, attackUsed_.range))
        {
            if (attack == ActionType.ATK2)
            {
                currentCharControlled.skillAvailable = false;
                actionType = ActionType.ATK1;
            }
            else if (attack == ActionType.ATK3)
            {
                currentCharControlled.skillAvailable2 = false;
                actionType = ActionType.ATK1;
            }
            else if (attack == ActionType.ATK4)
            {
                currentCharControlled.skillAvailable3 = false;
                actionType = ActionType.ATK1;
            }
            // Attack animation
            Animator animator = currentCharControlled.go.transform.GetChild(1).GetComponent<Animator>();
            if (animator 
                && attack == ActionType.ATK4 && currentCharControlled.charClass == CharClass.BUCHERON) // action de charge
            {
               //Animation quand il fait la charge
            }
            else if (animator)
            {
                animator.SetTrigger("Attack1Trigger");
            }
            attackUsedAttack = attackUsed_;
            attackUsedTargetHexa = new Point(hexaDestination.x, hexaDestination.y);
            attackUsed = 30; // Delay attack

            // Particles for soigneur
            if (currentCharControlled.charClass == CharClass.SOIGNEUR)
            {
                GameObject go = GameObject.Instantiate(particleHeal);
                go.transform.position = Hexa.hexaPosToReal(hexaDestination.x, hexaDestination.y, 0);
                go.transform.localScale *= 0.1f;
                Destroy(go, 5);
            }
            //Particles for mage
            else if (currentCharControlled.charClass == CharClass.MAGE)
            {
                GameObject go = GameObject.Instantiate(particleExplosion);
                go.transform.position = Hexa.hexaPosToReal(hexaDestination.x, hexaDestination.y, 0);
                go.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                Destroy(go, 5);
            }

            // calculate the angle the character will be facing
            Vector3 v1 = Hexa.hexaPosToReal(hexaDestination.x, hexaDestination.y, 0);
            Vector3 v2 = Hexa.hexaPosToReal(currentCharControlled.x, currentCharControlled.y, 0);
            float x = v1.x - v2.x;
            float y = v1.z - v2.z;
            float d = Mathf.Sqrt(x * x + y * y);
            float cos_ = (x == 0) ? 0 : x / d;
            float angle = Mathf.Acos(cos_);
            if (y < 0) angle = -angle;
            int angleDegrees = (int)((angle * 180) / (Mathf.PI));
            if (angleDegrees < 0) angleDegrees = 360 + angleDegrees;
            angleDegrees = 360 - angleDegrees + 90;
            angleDegrees = (angleDegrees + 5) / 10 * 10;
            //Debug.Log(x + " " + y + " " + cos_ + " " + " " + angle + " " + angleDegrees);
            Transform charModel = currentCharControlled.go.transform.GetChild(1);
            if (charModel) charModel.eulerAngles = new Vector3(0, angleDegrees, 0);
            return attackUsed_.effectValue;
        }
        //When it's not possible
        else
        {
            if (hexaDestination == null)
            {
                Debug.Log("hexaDest null");
            }
            else if (!hexaGrid.hexaInSight(currentCharControlled.x, currentCharControlled.y, hexaDestination.x, hexaDestination.y, attackUsed_.range))
            {
                Debug.Log("hexa not in sight: from " + currentCharControlled.x + "," + currentCharControlled.y + " to " + hexaDestination.x + "," + hexaDestination.y);
            }
        }
        updateMouseHover = true;
        updateUI = true;
        return 0;

    }

    //Use attack
    //edited by Julien L3L1
    //edited by GOUVEIA Klaus, group: L3Q1
    //edited by MEDILEH Youcef, group: L3C1 14/03/2023
    // - Added the possibility to use the skill 2
    // - Added the MASSIVE_HEAL skill type
    // - Added the HEALTH_STEAL skill type
    // - Added the DAMAGE_OVER_TIME skill type
    // - Added the LIGHTNING skill type
    // - Created methods for each skill type
    //edited by CROUZET Oriane, group : L3C1 14/03/2023
    // - Added the RANDOM RANGE skill type
    // - Added the DOOM skill type
    // - Added the FREEZE skill type
    //Author : ?
    void useAttack()
    {
        //Get the characters hitted 
        List<Character> hits = hexaGrid.getCharWithinRange(attackUsedTargetHexa.x, attackUsedTargetHexa.y, attackUsedAttack.rangeAoE);
        // Filter target(s)
        if (!attackUsedAttack.targetsEnemies)
        {
            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].team != currentCharControlled.team)
                {
                    hits.RemoveAt(i); i--;
                }
            }
        }
        if (!attackUsedAttack.targetsAllies)
        {
            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].team == currentCharControlled.team)
                {
                    hits.RemoveAt(i); i--;
                }
            }
        }
        if (!attackUsedAttack.targetsSelf)
        {
            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i] == currentCharControlled)
                {
                    hits.RemoveAt(i); i--;
                }
            }
        }

        if (attackUsedAttack.attackEffect == CharsDB.AttackEffect.DAMAGE_OVER_TIME)
        {
            HandlePoison();
        }
        //Added by Julien L3L1

        if (attackUsedAttack.attackEffect == CharsDB.AttackEffect.CHARGE)
        {
            print("CHARGE 2");
            getHoveredHexa();
            print("hexahovered charge x"+hexaHovered.x+"y"+hexaHovered.y);
            int nbCase = actionMove(hexaHovered);
            currentCharControlled.updatePos(pathWalk[pathWalk.Count - 1].x, pathWalk[pathWalk.Count - 1].y, hexaGrid);
            //    walkingAnimation();
            //Added by Socrate Louis Deriza, L3C1
            //     getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.MOVE);
            //    getEndGameDataCharacterFromTheList(currentCharControlled).addMovement(nbCase);

            //   int nbCase = actionMove(hexaHovered);


            // getEndGameDataCharacterFromTheList(currentCharControlled).addAction(ActionType.MOVE);
            //   getEndGameDataCharacterFromTheList(currentCharControlled).addMovement(nbCase);

            //     charge = true;
        }






        //Give attack effects
        foreach (Character c in hits)
        {
            print("attack");


            HandleShield(c);

           
           



            //If the attack effect is damage, give damages
            switch (attackUsedAttack.attackEffect)
            {
                case CharsDB.AttackEffect.DAMAGE:
                {
                HandleDamage(c); }
                    break;
                //Added by Julien L3L1


                case CharsDB.AttackEffect.INVISIBLE:
                    {
                        print("invisible2");
                        HandleInvisible(c);
                    }
                    break;
                //Added by Julien L3L1

                case CharsDB.AttackEffect.STORM:
                    {
                        print("storm 2");
                        HandleStorm(c);

                    }
                    break;

                //Added by Julien L3L1
                case CharsDB.AttackEffect.SHIELD:
                    {
                        EnableShield(c);
                    }
                    break;


                //Added by Julien L3L1

                case CharsDB.AttackEffect.HOWL:

                    {
                HandleHowl(c);
               

                    }
                    break;

                //Added by Julien L3L1


                case CharsDB.AttackEffect.RAGE:

                    {
                        HandleRage(c);


                    }
                    break;

                // Author: CROUZET Oriane, group : L3C1
                // Date : 14/03/2023

                //If the attack effect is random damage, give random damages between 2 to 5 damage points in addition to the value of the original attack
                case CharsDB.AttackEffect.RANDOM_DAMAGE:
                    {
                        HandleRandomDamage(c);
                        
                    }
                    break;

                // Author: CROUZET Oriane, group : L3C1
                // Date : 15/03/2023
                //If the effect is Doom, the character gets an aura that reduce the ennemy attack by two
                case CharsDB.AttackEffect.DOOM:
                    {
                        HandleDoom(c);
                    }
                    break;

                // Author: MEDILEH Youcef, group : L3C1
                // Date : 15/03/2023
                case CharsDB.AttackEffect.LIGHNING:
                    {
                        HandleLightning(c);
                    }
                    break;
                // Author: CROUZET Oriane, group : L3C1
                // Date : 15/03/2023
                //If the effect is freeze, freeze ennemies for one turn (no actions available)
                case CharsDB.AttackEffect.FREEZE:
                    {
                        HandleFreeze(c);
                    }
                    break;

                // Author: MEDILEH Youcef, groupe : L3C1
                // Date : 14/03/2023
                case CharsDB.AttackEffect.HEALTH_STEALING:
                    {
                        HandleHealthSteal(c);

                    }
                    break;
                //If the attack effect is healing, heal
                case CharsDB.AttackEffect.HEAL:
                    {
                        HandleHeal(c);
                    }
                    break;
                // Author: MEDILEH Youcef, groupe : L3C1
                // Date : 13/03/2023
                case CharsDB.AttackEffect.MASSIVE_HEAL:
                    {
                        HandleMassiveHeal();
                    }
                    break;
                //If the attack effect is PA buffing, buff it
                case CharsDB.AttackEffect.PA_BUFF:
                    {
                        HandlePABuff(c);
                    }
                    break;
                //If the attack effect is damage buffing, buff it
                case CharsDB.AttackEffect.DMG_BUFF:
                    {
                        HandleDMGBuff(c);
                    }
                    break;
            }
        }
        nextTurn();
    }

    // Author: MEDILEH Youcef, groupe : L3C1
    private void HandlePoison()
    {
        foreach (Point p in hexaGrid.getHexasWithinRange(attackUsedTargetHexa.x, attackUsedTargetHexa.y, attackUsedAttack.rangeAoE))
        {
            // get team of the character to set the turn of the poisonned hexa
            if (currentCharControlled.team == 0)
            {
                turnCounterDamageOverTimeTEAM1 = 3;
            }
            else
            {
                turnCounterDamageOverTimeTEAM2 = 3;
            }


            hexaGrid.getHexa(p.x, p.y).isPoisoned = true;
            hexaGrid.getHexa(p.x, p.y).poisonnedDamage = attackUsedAttack.effectValue;
            Debug.Log("hexa " + p.x + "," + p.y + " is poisonned");
            Debug.Log("Damage over time : " + attackUsedAttack.effectValue);
            poisonnedHexas.Add(hexaGrid.getHexa(p.x, p.y));
            // change the color of the hexa
            hexaGrid.getHexa(p.x, p.y).go.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    // Author: MEDILEH Youcef, groupe : L3C1
    private void HandleLightning(Character c)
    {
        Debug.Log("Lightning");
        HandleDamage(c);
        HandleFreeze(c);
        // affichage des noms des personnages touchés


        Debug.Log(c.getName());

    }

    private void HandleDMGBuff(Character c)
    {
        if (!c.dmgbuff)
        {
            c.dmgbuff = true;
            GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "+" + attackUsedAttack.effectValue, Color.yellow, 60);
        }
    }

    private void HandlePABuff(Character c)
    {
        Debug.Log("1: " + c.getPA() + "\n2: " + c.getClassData().basePA);
        if (c.characterPA == c.getClassData().basePA)
        {
            // Create object that shows pa buff
            GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "+" + attackUsedAttack.effectValue, Color.blue, 60);

            if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled, attackUsedAttack.effectValue);

            currentCharControlled.totalDamage += attackUsedAttack.effectValue;
            c.PA = c.characterPA + attackUsedAttack.effectValue;
        }
    }



    // Author: MEDILEH Youcef, groupe : L3C1
    // Permet de soigner tous les alliés
    private void HandleMassiveHeal()
    {
        // heal all allies units
        foreach (Character c2 in hexaGrid.charList)
        {

            if (c2.team == currentCharControlled.team)
            {
                Debug.Log("Heal " + c2.getName() + " for " + attackUsedAttack.effectValue);
                // Create object that shows heal
                GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
                dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
                int heal = attackUsedAttack.effectValue;
                if (heal > c2.HPmax - c2.HP) heal = c2.HPmax - c2.HP;
                dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c2.x, c2.y, "+" + heal, Color.green, 60);

                if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToHeal(currentCharControlled, heal);

                currentCharControlled.totalDamage += heal;
                c2.HP += heal;
            }
        }
    }

    private void HandleHeal(Character c)
    {
        // Create object that shows heal
        GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
        dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
        int heal = attackUsedAttack.effectValue;
        if (heal > c.HPmax - c.HP) heal = c.HPmax - c.HP;
        dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "+" + heal, Color.green, 60);

        if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToHeal(currentCharControlled, heal);

        currentCharControlled.totalDamage += heal;
        c.HP += heal;
    }

    //added by Julien L3L1

    private void HandleHowl(Character c)
    {
        GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
        dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
        int heal = attackUsedAttack.effectValue;
        int damageBuff= attackUsedAttack.effectValue;
        //    int heal = 10;
        if (heal > c.HPmax - c.HP) heal = c.HPmax - c.HP;
        dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "HURLEMENT", Color.green, 60);
        c.HP += heal;
        print("Affichage Howl");
        print(c.HP);
        c.myCharClass.basicAttack.effectValue += damageBuff;
        print(c.myCharClass.basicAttack.effectValue);


    }

    //added by Julien L3L1

    private void HandleInvisible(Character c)
    {
        //  SkinnedMeshRenderer skinnedMeshRenderer = c.go.GetComponent<SkinnedMeshRenderer>();
        //     print(skinnedMeshRenderer.enabled);
        c.go.SetActive(false);
        float temps = attackUsedAttack.effectValue;
        StartCoroutine(DisableInvisible(temps, c));
   //     skinnedMeshRenderer.enabled = false;
        print("invisible");
        
    }


    //added by Julien L3L1
    IEnumerator DisableInvisible(float temps, Character c)
        
    {
        yield return new WaitForSeconds(temps);
        c.go.SetActive(true);

    }
    //added by Julien L3L1
    private void HandleStorm(Character c)
    {
       print("storm");
      foreach (Character c1 in hexaGrid.charList)
        {
            if (c1.team != c.team)
            {
                int hpAfterAttack = c1.getHP() - attackUsedAttack.effectValue;
                c1.setHP(hpAfterAttack);
            }
        }
        
        print("storm :"+startGameData.charsTeam1[0]);

    }

    //added by Julien L3L1
    private void EnableShield(Character c)
    {
        print("Active shied");
        c.setShield(true);
    }

    //added by Julien L3L1
    private void HandleShield(Character c)
    {
        if (c.getShield() == true)
        {
            print("protéger");

            c.HP += attackUsedAttack.effectValue;
            c.setShield(false);

        }

    }

    //added by Julien L3L1
    // Description : active le sort Rage pendant 15 secondes
    private void HandleRage(Character c)
    {
       
        GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
        dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
        int heal = attackUsedAttack.effectValue;
        int damageBuff = attackUsedAttack.effectValue;
        //    int heal = 10;
        if (heal > c.HPmax - c.HP) heal = c.HPmax - c.HP;
        dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "Rage", Color.green, 60);
        c.HP += heal;
        print("Affichage Rage");
        print(c.HP);
        c.myCharClass.basicAttack.effectValue += damageBuff;
        print(c.myCharClass.basicAttack.effectValue);
        StartCoroutine(DisableRage(15, c, damageBuff, heal));

    }

    //added by Julien L3L1
    IEnumerator DisableRage(float temps, Character c,int damageBuff,int heal)

    {
        yield return new WaitForSeconds(temps);
        c.HP -= heal;
        c.myCharClass.basicAttack.effectValue -= damageBuff;
    }




    // Author: MEDILEH Youcef, groupe : L3C1
    // Permet de voler de la vie à un ennemi
    private void HandleHealthSteal(Character c)
    {
        // Create object that shows damage dealt (health steal)
        GameObject healthStolenDisp = GameObject.Instantiate(damageValueDisplay);
        healthStolenDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
        c.HP -= attackUsedAttack.effectValue;
        healthStolenDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "-" + attackUsedAttack.effectValue, Color.red, 60);
        Debug.Log("###### Health stolen from " + c.getName() + " : " + attackUsedAttack.effectValue + " HP ######");
        if (currentCharControlled.HP + attackUsedAttack.effectValue > currentCharControlled.HPmax)
        {
            currentCharControlled.HP = currentCharControlled.HPmax;
        }
        else
        {
            currentCharControlled.HP += attackUsedAttack.effectValue;
        }
        healthStolenDisp.GetComponent<DamageValueDisplay>().setValue(currentCharControlled.x, currentCharControlled.y, "+" + attackUsedAttack.effectValue, Color.green, 60);
        Debug.Log("###### Health restored to " + currentCharControlled.getName() + " : " + attackUsedAttack.effectValue + " HP ######");
        if (whoControlsThisChar(c) == PlayerType.AI_HARD) statsGame.addToDamageTaken(c, attackUsedAttack.effectValue);
        if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled, attackUsedAttack.effectValue);
        // Enemy dies
        bool isDead = IsDead(c);
        if (isDead)
        {
            getEndGameDataCharacterFromTheList(currentCharControlled).hasKilledSomeOne();
        }
    }

    // Author: CROUZET Oriane, groupe : L3C1
    // Permet de geler un ennemi
    private void HandleFreeze(Character c)
    {
        if (!c.freezed)
        {
            c.setFreezed(true);
            // Create object that shows freeze
            GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "Freezed", Color.blue, 60);
        }
    }

    // Author: CROUZET Oriane, groupe : L3C1
    private void HandleDoom(Character c)
    {
        if (!c.doomed)
        {
            c.setDoomed(true);
            // Create object that shows doom
            GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "Doomed", Color.red, 60);
        }
    }

    // Author: CROUZET Oriane, groupe : L3C1
    private void HandleRandomDamage(Character c)
    {
        System.Random rnd = new();
        int randomNumberDamage = rnd.Next(1, 5);

        GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
        if (currentCharControlled.dmgbuff)
        {
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "-" + (attackUsedAttack.effectValue + 1 + randomNumberDamage), Color.red, 60);
        }
        else if (currentCharControlled.doomed)
        {
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "-" + (attackUsedAttack.effectValue + randomNumberDamage - doomDamage), Color.red, 60);
        }
        else
        {
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "-" + (attackUsedAttack.effectValue + randomNumberDamage), Color.red, 60);
        }
        if (whoControlsThisChar(c) == PlayerType.AI_HARD) statsGame.addToDamageTaken(c, (attackUsedAttack.effectValue + randomNumberDamage));
        if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled, (attackUsedAttack.effectValue + randomNumberDamage));

        if (currentCharControlled.dmgbuff)
        {
            currentCharControlled.totalDamage += (attackUsedAttack.effectValue + 1 + randomNumberDamage);
            c.HP -= (attackUsedAttack.effectValue + 1 + randomNumberDamage);
        }
        else if (currentCharControlled.doomed)
        {
            currentCharControlled.totalDamage += (attackUsedAttack.effectValue - doomDamage + randomNumberDamage);
            c.HP -= (attackUsedAttack.effectValue - doomDamage + randomNumberDamage);
            currentCharControlled.setDoomed(false);
        }
        else
        {
            currentCharControlled.totalDamage += (attackUsedAttack.effectValue + randomNumberDamage);
            c.HP -= (attackUsedAttack.effectValue + randomNumberDamage);
        }
        // Enemy dies
        bool isDead = IsDead(c);
        if (isDead)
        {
            getEndGameDataCharacterFromTheList(currentCharControlled).hasKilledSomeOne();
        }
    }

    private void HandleDamage(Character c)
    {
        // Create object that shows damage dealt
        GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
        if (currentCharControlled.dmgbuff)
        {
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "-" + (attackUsedAttack.effectValue + 1), Color.red, 60);
        }
        else if (currentCharControlled.doomed)
        {
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "-" + (attackUsedAttack.effectValue - doomDamage), Color.red, 60);
        }
        else
        {
            dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
            dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "-" + attackUsedAttack.effectValue, Color.red, 60);
        }
        //if (whoControlsThisChar(c) == PlayerType.AI_HARD) statsGame.addToDamageTaken(c, attackUsedAttack.effectValue);
        //if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled, attackUsedAttack.effectValue);

        if (currentCharControlled.dmgbuff)
        {
            currentCharControlled.totalDamage += (attackUsedAttack.effectValue + 1);
            c.HP -= (attackUsedAttack.effectValue + 1);
        }
        else if (currentCharControlled.doomed)
        {
            currentCharControlled.totalDamage += (attackUsedAttack.effectValue - doomDamage);
            c.HP -= (attackUsedAttack.effectValue - doomDamage);
            currentCharControlled.setDoomed(false);
        }
        else
        {
            currentCharControlled.totalDamage += attackUsedAttack.effectValue;
            c.HP -= attackUsedAttack.effectValue;
        }
        // Enemy dies
        bool isDead = IsDead(c);
        if (isDead)
        {
            getEndGameDataCharacterFromTheList(currentCharControlled).hasKilledSomeOne();
        }
    }

    // Author: ??
    // Edited by Youcef MEDILEH, L3C1
    private bool IsDead(Character c)
    {
        if (c.HP <= 0)
        {
            if (whoControlsThisChar(c) == PlayerType.AI_HARD) statsGame.setDead(c, true);
            if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToKills(currentCharControlled, 1);
            c.HP = 0;
            hexaGrid.getHexa(c.x, c.y).charOn = null;
            GameObject.Destroy(c.go);
            for (int i = 0; i < hexaGrid.charList.Count; i++)
            {
                if (hexaGrid.charList[i] == c)
                {
                    GameObject.Destroy(UICharTurnsList[i]);
                    UICharTurnsList.RemoveAt(i);
                    hexaGrid.charList.RemoveAt(i);
                }
            }
            // update currentCharControlled ID
            for (int i = 0; i < hexaGrid.charList.Count; i++)
            {
                if (hexaGrid.charList[i] == currentCharControlled) currentCharControlledID = i;
            }
            // force AI to make a new decision
            decisionSequence = new List<ActionAIPos>();
            // check if there is a winner
            int nbT1 = 0;
            int nbT2 = 0;
            foreach (Character c2 in hexaGrid.charList)
            {
                if (c2.team == 0) nbT1++;
                else nbT2++;
            }
            if (nbT1 == 0)
            {
                winner = 1;
                //getEndGameDataCharacterFromTheList(currentCharControlled).hasKilledSomeOne();
            }
            else if (nbT2 == 0)
            {
                winner = 0;
                //getEndGameDataCharacterFromTheList(currentCharControlled).hasKilledSomeOne();
            }
            return (true);
        }
        return (false);
    }

    //Display the walking animation
    //Author : ??
    //Edited by L3C1, Yuting HUANG, le 03/04/2023, MaxTimeNum
    void walkingAnimation()
    {
        int speed = 6;
        if (pathWalkpos == 0)
        {
            Animator animator = currentCharControlled.go.transform.GetChild(1).GetComponent<Animator>();
            if (animator)
            {
                animator.SetBool("Moving", true);
                animator.SetBool("Running", true);
            }
        }
        if (pathWalkpos < (pathWalk.Count - 1) * speed)
        {
            for (int i = 0; i < 6; i++)
            {
                Point p = HexaGrid.findPos(pathWalk[pathWalkpos / speed].x, pathWalk[pathWalkpos / speed].y, (HexaDirection)i);
                if (p.x == pathWalk[pathWalkpos / speed + 1].x && p.y == pathWalk[pathWalkpos / speed + 1].y)
                {
                    currentCharControlled.setDirection((HexaDirection)i);
                    i = 6;
                }
            }

            float multiplier = (pathWalkpos % speed) / (float)speed;

            float x1 = pathWalk[pathWalkpos / speed].x * 0.75f;
            float x2 = pathWalk[pathWalkpos / speed + 1].x * 0.75f;
            float x = x1 * (1.0f - multiplier) + x2 * multiplier;

            float y1 = pathWalk[pathWalkpos / speed].y * -0.86f + (pathWalk[pathWalkpos / speed].x % 2) * 0.43f;
            float y2 = pathWalk[pathWalkpos / speed + 1].y * -0.86f + (pathWalk[pathWalkpos / speed + 1].x % 2) * 0.43f;
            float y = y1 * (1.0f - multiplier) + y2 * multiplier;
            currentCharControlled.go.transform.position = new Vector3(x + Hexa.offsetX, 0, y + Hexa.offsetY);
            pathWalkpos++;
        }
        else
        {
            currentCharControlled.updatePos(pathWalk[pathWalk.Count - 1].x, pathWalk[pathWalk.Count - 1].y, hexaGrid);
            Animator animator = currentCharControlled.go.transform.GetChild(1).GetComponent<Animator>();
            if (animator)
            {
                animator.SetBool("Moving", false);
                animator.SetBool("Running", false);
            }

            TimeNum = MaxTimeNum;
            nextTurn();

        }
    }

    //Proceed to the next turn
    //Author : ?
    // Edited and refactored by VALAT Thibault, L3Q1
    // Edited by Youcef MEDILEH, L3C1
    // Edited by Julien D'aboville, L3C1

    // - gestion du poison
    // - debut reglage du bug de l'initiative
    void nextTurn()
    {
     

        checkAndUpdateBonusControll();
        displayInitiative();
        currentCharControlled.PA--;

        //Check if the controlled character can use special attack
        if (currentCharControlled.totalDamage >= 10)
        {
            if (currentCharControlled.charClass == CharClass.GUERRIER)
                currentCharControlled.totalDamage = 0;
            else
                currentCharControlled.totalDamage -= 10;
            currentCharControlled.skillAvailable2 = true;
            currentCharControlled.skillAvailable3 = true;

            currentCharControlled.skillAvailable = true;
        }


        //When the controlled character has no more PA, his turn is over
        if (currentCharControlled.PA <= 0)
        {
            Debug.Log("Le tour de " + currentCharControlled.getName() + " est fini");
            print("NEXT TURN");
            print("tour"+tour);
            tour += 1;
            if (turnCounterDamageOverTimeTEAM1 > 0 || turnCounterDamageOverTimeTEAM2 > 0)
            {
                turnCounterDamageOverTimeTEAM1--;
                turnCounterDamageOverTimeTEAM2--;
                // Check each character is they are in poisonned hexa
                foreach (Character c in hexaGrid.charList)
                {
                    // If the character is in a poisonned hexa
                    if (poisonnedHexas.Contains(hexaGrid.getHexa(c.x, c.y)))
                    {
                        // Create object that shows damage
                        GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
                        dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
                        dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x, c.y, "-" + hexaGrid.getHexa(c.x, c.y).poisonnedDamage, Color.red, 60);

                        if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageTaken(currentCharControlled, hexaGrid.getHexa(c.x, c.y).poisonnedDamage);

                        currentCharControlled.totalDamage += hexaGrid.getHexa(c.x, c.y).poisonnedDamage;
                        c.HP -= hexaGrid.getHexa(c.x, c.y).poisonnedDamage;
                        bool isDead =  IsDead(c);
                        if (isDead)
                        {
                            getEndGameDataCharacterFromTheList(currentCharControlled).hasKilledSomeOne();
                        }

                    }
                }
            }
            else
            {
                // all poisonned hexa are removed
                poisonnedHexas.Clear();
            }

            //Reset priority
            do
            {
                hexaGrid.charList.Sort();
                foreach (Character c in hexaGrid.charList)
                {
                    if (c.priority >= 5)
                        c.priority = c.priority - UnityEngine.Random.Range(1, 3);
                    else
                        c.priority--;
                }

                currentCharControlled.resetPriority();
                hexaGrid.charList.Sort();
                currentCharControlled = hexaGrid.charList[0];
                Debug.Log("Le prochain est " + currentCharControlled.getName());
                currentCharControlled.PA = CharsDB.list[(int)currentCharControlled.charClass].basePA;
            } while (currentCharControlled.HP <= 0);

            PlayerType currentPlayerType = (currentCharControlled.team == 0) ? startGameData.player1Type : startGameData.player2Type;
            //Stats to remove
            if (currentPlayerType == PlayerType.AI_HARD || currentPlayerType == PlayerType.AI_MEDIUM)
            {
                statsGame.nextTurn(currentCharControlled);
            }
            actionType = ActionType.MOVE;
            newTurn = 0;
            decisionSequence = new List<ActionAIPos>();
            if (!lockedCamera) cameraPosGoal = new Vector3(currentCharControlled.go.transform.position.x, cameraPosGoal.y, ((hexaGrid.h * -0.43f) * 0.0f + currentCharControlled.go.transform.position.z * 1.0f) - cameraPosGoal.y * 0.75f);
        }
        updateUI = true;
        updateMouseHover = true;
        pathWalk = null;
        checkAndUpdateBonusControll();
        displayInitiative();
        TimeNum = MaxTimeNum;
    }

    //Added by Socrate Louis Deriza, L3C1
    //Edited by L3C1 CROUZET Oriane, 19/04/2023
    public RoleCharacter getRoleOfCharacter(CharClass charclass)
    {
        if (charclass == CharClass.GUERRIER)
        {
            return RoleCharacter.TANKS;
        }
        if (charclass == CharClass.DRUIDE || charclass == CharClass.MAGE)
        {
            return RoleCharacter.MAGICIENS;
        }
        if (charclass == CharClass.ENVOUTEUR || charclass == CharClass.SOIGNEUR)
        {
            return RoleCharacter.SUPPORTS;
        }
        if (charclass == CharClass.ARCHER || charclass == CharClass.VOLEUR)
        {
            return RoleCharacter.ASSASSINS;
        }
            return RoleCharacter.COMBATTANTS;
    }

    //Load a game
    //Author : ?
    //Edited by L3Q1, VALAT Thibault
	//Edited by L3C1, Yuting HUANG, ajoute un map non aleatoire
    public void loadGame()
    {
        hexaGrid = new HexaGrid();
        int nbChar = 0;
        string conn = "URI=file:" + Application.streamingAssetsPath + "/Save/saveGame.db"; //Path to database.
        IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database
        IDbCommand dbcmd = dbconn.CreateCommand();
        IDataReader reader;

        //If the map is ruins
        if (startGameData.mapChosen == 0)
        {
            hexaGrid.createGridFromFile(Application.streamingAssetsPath + "/Data/Map/ruins");
            tileW = hexaGrid.w; tileH = hexaGrid.h;
            ruinsMap.SetActive(true);
            foreach (Hexa hexa in hexaGrid.hexaList)
            {
                if (hexa.type != HexaType.GROUND && hexa.type != HexaType.BONUS)
                {
                    hexa.go.GetComponent<Renderer>().enabled = false;
                }
            }
        }
        // Si on choisit un map libre
         if (startGameData.mapChosen == 2)
        {
            hexaGrid.createGridFromFile(Application.streamingAssetsPath + "/Data/Map/libre");
            tileW = hexaGrid.w; tileH = hexaGrid.h;
            libreMap.SetActive(true);
            foreach (Hexa hexa in hexaGrid.hexaList)
            {
                if (hexa.type != HexaType.GROUND && hexa.type != HexaType.BONUS)
                {
                    hexa.go.GetComponent<Renderer>().enabled = false;
                }
            }
        }
        //If it's a random map
        else
        {
            Hexa.offsetX = -((hexaGrid.w - 1) * 0.75f) / 2;
            Hexa.offsetY = -((hexaGrid.h - 1) * -0.86f + ((hexaGrid.w - 1) % 2) * 0.43f) / 2;

            dbcmd.CommandText = "SELECT type FROM board";
            reader = dbcmd.ExecuteReader();
            Debug.Log("loadGame:  hexaGrid.hexaList.Count " + hexaGrid.hexaList.Count);
            for (int j = 0; j < hexaGrid.h; j++)
            {
                for (int i = 0; i < hexaGrid.w && reader.Read(); i++)
                {
                    hexaGrid.hexaList.Add(new Hexa((HexaType)reader.GetInt32(1), i, j));
                }
            }
        }

        //Load the bonus zone
        dbcmd.CommandText = "SELECT bonusCenterX, bonusCenterY, bonusColor FROM board";
        reader = dbcmd.ExecuteReader();
        reader.Read();


        caseBonus = hexaGrid.getHexa((int)reader.GetInt32(0), (int)reader.GetInt32(1));
        bonusTeam = (int)reader.GetInt32(2);
        displayBonus(hexaGrid.findAllPaths(caseBonus.getx(), caseBonus.gety(), 2));
        reader.Close(); //close the reader to execute a new command
        reader = null;

        dbcmd.CommandText = "SELECT player1, player2, map, w, h, current, NBchar FROM game";
        reader = dbcmd.ExecuteReader();
        reader.Read();
        startGameData.player1Type = (PlayerType)reader.GetInt32(0);
        startGameData.player2Type = (PlayerType)reader.GetInt32(1);
        startGameData.mapChosen = (int)reader.GetInt32(2);
        hexaGrid.w = (int)reader.GetInt32(3);
        hexaGrid.h = (int)reader.GetInt32(4);
        currentCharControlledID = reader.GetInt32(5);
        nbChar = reader.GetInt32(6);


        reader.Close(); //close the reader to execute a new command
        reader = null;


        dbcmd.CommandText = "SELECT class, team, x, y, pa, hp, skillA, directionF, priority FROM characters";
        reader = dbcmd.ExecuteReader();
        for (int i = 0; i < nbChar && reader.Read(); i++)
        {
            CharClass cCharClass = (CharClass)reader.GetInt32(0);
            int cTeam = (int)reader.GetInt32(1);
            int cX = reader.GetInt32(2);
            int cY = reader.GetInt32(3);
            Character c = new Character(cCharClass, this.getRoleOfCharacter(cCharClass), cX, cY, cTeam);
            c.PA = (int)reader.GetInt32(4);
            c.HP = (int)reader.GetInt32(5);
            c.skillAvailable = (reader.GetInt32(6) == 0) ? false : true;
            c.directionFacing = (HexaDirection)reader.GetInt32(7);
            c.priority = (int)reader.GetInt32(8);
            hexaGrid.addChar(c);
        }
        reader.Close(); //close the reader to execute a new command
        reader = null;
        dbcmd.CommandText = "DELETE FROM game";
        dbcmd.ExecuteNonQuery();
        dbcmd.CommandText = "INSERT INTO game (player1) VALUES (" + (-1) + ")";
        dbcmd.ExecuteNonQuery();

        currentCharControlled = hexaGrid.charList[currentCharControlledID];
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        hexaGrid.charList.Sort();
        for (int i = hexaGrid.charList.Count; i <= 10; i++)
            Initiative.transform.GetChild(i).transform.position = new Vector3(10000, 10000, 0);

        checkAndUpdateBonusControll();
        displayInitiative();
    }

    //Save a game
    //Author : ?
    //Edited by L3Q1, VALAT Thibault
    public void saveGame()
    {
        if (!(File.Exists(Application.streamingAssetsPath + "/Save/saveGame.db")))
        {
            Mono.Data.Sqlite.SqliteConnection.CreateFile(Application.streamingAssetsPath + "/Save/saveGame.db");
            string conna = "URI=file:" + Application.streamingAssetsPath + "/Save/saveGame.db"; //Path to database.
            IDbConnection dbconna;
            dbconna = (IDbConnection)new SqliteConnection(conna);
            IDbCommand dbcmda = dbconna.CreateCommand();
            dbconna.Open(); //Open connection to the database.
            dbcmda.CommandText = "BEGIN TRANSACTION";
            dbcmda.ExecuteNonQuery();
            dbcmda.CommandText = "CREATE TABLE IF NOT EXISTS 'game' ('player1'INTEGER,'player2'INTEGER,'map'INTEGER,'w'INTEGER,'h'INTEGER,'current'INTEGER,'NBchar'INTEGER)";
            dbcmda.ExecuteNonQuery();
            dbcmda.CommandText = "CREATE TABLE IF NOT EXISTS 'characters' ('class'	INTEGER,'team'	INTEGER,'x'	INTEGER,'y'	INTEGER,'pa'	INTEGER,'hp'	INTEGER,'skillA'INTEGER,'directionF'	INTEGER, 'totalDamage'	INTEGER, 'priority'	INTEGER)";
            dbcmda.ExecuteNonQuery();
            dbcmda.CommandText = "CREATE TABLE IF NOT EXISTS 'board' ('IDhexa'	INTEGER UNIQUE,'type'  INTEGER,'bonusCenterX'  INTEGER,'bonusCenterY'  INTEGER,'bonusColor' INTEGER,PRIMARY KEY('IDhexa'))";
            dbcmda.ExecuteNonQuery();
            dbcmda.CommandText = "end";
            dbcmda.ExecuteNonQuery();
            dbconna.Close();
        }
        string conn = "URI=file:" + Application.streamingAssetsPath + "/Save/saveGame.db"; //Path to database.

        IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);

        IDbCommand dbcmd = dbconn.CreateCommand();
        dbconn.Open(); //Open connection to the database.


        dbcmd.CommandText = "begin";
        dbcmd.ExecuteNonQuery();
        dbcmd.CommandText = "DELETE FROM game";
        dbcmd.ExecuteNonQuery();
        dbcmd.CommandText = "DELETE FROM board";
        dbcmd.ExecuteNonQuery();
        dbcmd.CommandText = "DELETE FROM characters";
        dbcmd.ExecuteNonQuery();
        dbcmd.CommandText = "DELETE FROM game";
        dbcmd.ExecuteNonQuery();
        dbcmd.CommandText = "DELETE FROM board";
        dbcmd.ExecuteNonQuery();
        dbcmd.CommandText = "INSERT INTO game (player1, player2, map, w, h, current, NBchar) VALUES (" + (byte)startGameData.player1Type + ", " + (byte)startGameData.player2Type + ", " + (byte)startGameData.mapChosen + ", " + hexaGrid.w + ", " + hexaGrid.h + ", " + currentCharControlledID + ", " + hexaGrid.charList.Count + ")";
        dbcmd.ExecuteNonQuery();

        foreach (Character c in hexaGrid.charList)
        {
            dbcmd.CommandText = "INSERT INTO characters (class, team, x, y, pa, hp, skillA, directionF, totalDamage, priority) VALUES (" + (byte)c.charClass + ", " + (byte)c.team + ", " + c.x + ", " + c.y + ", " + (byte)c.PA + ", " + (byte)c.HP + ", " + (byte)(c.skillAvailable ? 1 : 0) + ", " + (byte)c.directionFacing + ", " + (byte)c.totalDamage + ", " + (byte)c.priority + ")";
            dbcmd.ExecuteNonQuery();
        }

        if (startGameData.mapChosen == 1)
        { // If we chose a random map, write it.
            int k = 0;
            for (int j = 0; j < hexaGrid.h; j++)
            {
                for (int i = 0; i < hexaGrid.w; i++)
                {
                    dbcmd.CommandText = "INSERT INTO board (IDhexa, type) VALUES (" + k + ", " + (byte)(hexaGrid.hexaList[k].type) + ")";
                    dbcmd.ExecuteNonQuery();
                    k++;
                }
            }
        }

        dbcmd.CommandText = "INSERT INTO board (bonusCenterX, bonusCenterY, bonusColor) VALUES (" + (byte)caseBonus.getx() + ", " + (byte)caseBonus.gety() + ", " + (byte)bonusTeam + ")";
        dbcmd.ExecuteNonQuery();


        dbcmd.CommandText = "end";
        dbcmd.ExecuteNonQuery();
        dbconn.Close();
        dbconn = null;

    }


    // ##################################################################################################################################################
    // Display Functions used in main
    // ##################################################################################################################################################


    //Display the right character in the top left character card
    //Author : ?
    void displayNewCharTurnList()
    {
        //GameObject go = UICharTurnsList[0];
        UIEnemyChar.SetActive(false);

        UICurrentChar.transform.GetChild(3).GetComponent<Text>().text = currentCharControlled.PA + "";
        UICurrentChar.transform.GetChild(6).GetComponent<Text>().text = currentCharControlled.HP + "/" + currentCharControlled.HPmax;
        UICurrentChar.transform.GetChild(1).GetComponent<RawImage>().texture = charCards[(int)currentCharControlled.charClass];
        UICurrentChar.transform.GetChild(0).GetComponent<Image>().color = (currentCharControlled.team == 0) ? Character.TEAM_1_COLOR : Character.TEAM_2_COLOR;
        if (charHovered != null && charHovered.team == currentCharControlled.team)
        {
            UICurrentChar.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1);
            UICurrentChar.transform.GetChild(3).GetComponent<Text>().text = charHovered.PA + "";
            UICurrentChar.transform.GetChild(6).GetComponent<Text>().text = charHovered.HP + "/" + charHovered.HPmax;
            UICurrentChar.transform.GetChild(1).GetComponent<RawImage>().texture = charCards[(int)charHovered.charClass];
        }
        else if (charHovered != null && charHovered.team != currentCharControlled.team)
        {
            UIEnemyChar.SetActive(true);
            UIEnemyChar.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1);
            UIEnemyChar.transform.GetChild(3).GetComponent<Text>().text = charHovered.PA + "";
            UIEnemyChar.transform.GetChild(6).GetComponent<Text>().text = charHovered.HP + "/" + charHovered.HPmax;
            UIEnemyChar.transform.GetChild(1).GetComponent<RawImage>().texture = charCards[(int)charHovered.charClass];
        }
    }

    //Display Initiative at the bottom on the screen
    //Author : VALAT Thibault, L3Q1
    void displayInitiative()
    {
        Initiative.transform.GetChild(hexaGrid.charList.Count).transform.position = new Vector3(10000, 10000, 0);
        //Display every Initiative case
        for (int i = 0; i < hexaGrid.charList.Count; i++)
            if (hexaGrid.charList[i].HP > 0)
                displayOneInitiativeCase(i, hexaGrid.charList[i]);
    }

    //Display on Initiative case
    //Author : VALAT Thibault, L3Q1
    // Edited by : Youcef MEDILEH, L3C1
    // - Ajout de la gestion de la couleur des cases d'initiative en fonction de l'état du personnage s'il est gelé ou non (freezed)
    void displayOneInitiativeCase(int i, Character c)
    {

        if (c.isFreezed())
        {
            Initiative.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            Initiative.transform.GetChild(i).transform.GetChild(1).GetComponent<RawImage>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            Initiative.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
            Initiative.transform.GetChild(i).transform.GetChild(1).GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
            Initiative.transform.GetChild(i).transform.GetChild(1).GetComponent<RawImage>().texture = charSquares[(int)c.charClass];
            Initiative.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = (c.team == 0) ? Character.TEAM_1_COLOR : Character.TEAM_2_COLOR;
        }
    }





    //Display the action buttons on the bottom left
    //Edited by Socrate Louis Deriza L3C1
    //Edited by Julien D'aboville L3L1

    bool IsPointerOver(RectTransform rectangle){
        Vector2 positonSouris;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(rectangle, Input.mousePosition, null, out positonSouris)){
            if(rectangle.rect.Contains(positonSouris)){
                return true;
            }
        }
        return false;
    }
    void displayActionButtons()
    {
        int iChildrenToSelect;
        if ((actionType == ActionType.ATK1)) iChildrenToSelect = 0;
        else if ((actionType == ActionType.ATK2)) iChildrenToSelect = 1;
        else if ((actionType == ActionType.ATK3)) iChildrenToSelect = 2;
        else if ((actionType == ActionType.ATK4)) iChildrenToSelect = 3;

        else if ((actionType == ActionType.SKIP)) iChildrenToSelect = 4;
        else iChildrenToSelect = 4;
        for (int i = 0; i < 5; i++)
        {
            if ((i == iChildrenToSelect)) UIAction.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
            else UIAction.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Text>().fontStyle = FontStyle.Normal;
        }
        if (!currentCharControlled.skillAvailable)
        {
            UIAction.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = currentCharControlled.totalDamage + " / 10";
        }
        else
        {
            UIAction.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "Compétence 1 (C)";
        }

        if (!currentCharControlled.skillAvailable2)
        {
            UIAction.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = currentCharControlled.totalDamage + " / 10";
        }
        else
        {
            UIAction.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = "Compétence 2 (V)";
        }


        if (!currentCharControlled.skillAvailable3)
        {
            UIAction.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = currentCharControlled.totalDamage + " / 10";
        }
        else
        {
            UIAction.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = "Compétence 3 (F)";
        }

        //Author : L3C1 CROUZET Oriane, 06/05/2023
        //Tooltip 
        Transform child_attaque_basique = UIAction.transform.GetChild(0);
        Transform child_competence1 = UIAction.transform.GetChild(1);
        Transform child_competence2 = UIAction.transform.GetChild(2);
        Transform child_competence3 = UIAction.transform.GetChild(3);
        Transform child_skip = UIAction.transform.GetChild(4);
        Transform child_mouvement = UIAction.transform.GetChild(5);





        if( IsPointerOver(child_attaque_basique.GetComponent<RectTransform>() )){
            UITooltip.SetActive(true);
            tooltipText.text = "Valeur de l'effet : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + "\n Portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString();
        }
        else if( IsPointerOver(child_competence1.GetComponent<RectTransform>() )){
            UITooltip.SetActive(true);
            tooltipText.text = "Valeur de l'effet : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + "\n Portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString();
        }
        else if( IsPointerOver(child_competence2.GetComponent<RectTransform>() )){
            UITooltip.SetActive(true);
            tooltipText.text = "Valeur de l'effet : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + "\n Portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.range.ToString();
        }
        else if( IsPointerOver(child_competence3.GetComponent<RectTransform>() )){
            UITooltip.SetActive(true);
            tooltipText.text = "Valeur de l'effet : " + CharsDB.list[(int)currentCharControlled.charClass].skill_3.effectValue.ToString() + "\n Portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_3.range.ToString();
        }
        else if( IsPointerOver(child_mouvement.GetComponent<RectTransform>() )){
            UITooltip.SetActive(true);
            tooltipText.text = "deplacement de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases.";
        }
        else if( IsPointerOver(child_skip.GetComponent<RectTransform>() )){
            UITooltip.SetActive(true);
            tooltipText.text = "passe le tour " ;
        }
        else{
            UITooltip.SetActive(false);
        }




        //if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 835 && mousePos.y < Screen.height - 835 + 24 * 1.8) //Mouse over move button
        //{
        //    UITooltip.SetActive(true);
        //    tooltipText.text = "Déplacement de " + CharsDB.list[(int)currentCharControlled.charClass].basePM.ToString() + " cases.";
        //}
        //else if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 890 && mousePos.y < Screen.height - 890 + 24 * 1.8) //Mouse over attack button
        //{
        //    UITooltip.SetActive(true);
        //    tooltipText.text = "Valeur de l'effet : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.effectValue.ToString() + "\n Portée : " + CharsDB.list[(int)currentCharControlled.charClass].basicAttack.range.ToString();
        //}
        //else if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 945 && mousePos.y < Screen.height - 945 + 24 * 1.8) //Mouse over special attack 1
        //{
        //    UITooltip.SetActive(true);
        //    tooltipText.text = "Valeur de l'effet : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.effectValue.ToString() + "\n Portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_1.range.ToString();
        //}
        //else if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 1000 && mousePos.y < Screen.height - 1000 + 24 * 1.8) //Mouse special attack 2
        //{
        //    UITooltip.SetActive(true);
        //    tooltipText.text = "Valeur de l'effet : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.effectValue.ToString() + "\n Portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_2.range.ToString();
        //}
//
        //else if (mousePos.x >= 34 && mousePos.x < 34 + 140 * 1.5 && mousePos.y >= Screen.height - 1055 && mousePos.y < Screen.height - 1055 + 24 * 1.8) //Mouse special attack 3
        //{
        //    UITooltip.SetActive(true);
        //    tooltipText.text = "Valeur de l'effet : " + CharsDB.list[(int)currentCharControlled.charClass].skill_3.effectValue.ToString() + "\n Portée : " + CharsDB.list[(int)currentCharControlled.charClass].skill_3.range.ToString();
        //}
        //else
        //{
        //    UITooltip.SetActive(false);
        //}


    }

    

    //Display the current hovered hexa
    //Author : ?
    void displayHoveredHexa()
    {
        if (hexaHovered != null) hexaHovered.hoveredColor();
        if (hexaHoveredOld != null) hexaHoveredOld.defaultColor();
    }

    //Display the possibles hexas to go to
    //Author : ?
    void displayPossiblePaths()
    {
        List<Point> path = hexaGrid.findAllPaths(currentCharControlled.x, currentCharControlled.y, currentCharControlled.PM);
        foreach (Point p in path)
        {
            GameObject go = GameObject.Instantiate(hexaTemplate, hexasFolder.transform);
            go.SetActive(true);
            go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x, p.y, -0.015f);
            go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
            go.GetComponent<Renderer>().sharedMaterial = allPathsDisplayMat;
            go.GetComponent<Collider>().enabled = false;
            pathFinderDisplay.Add(go);
        }
    }

    //Display the shortest path
    //Author : ?
    void displaySortestPath()
    {
        // Display path (create the green hexas)
        if (hexaHovered != null && (hexaHovered.type == HexaType.GROUND || hexaHovered.type == HexaType.BONUS))
        {
            List<Point> path = hexaGrid.findShortestPath(currentCharControlled.x, currentCharControlled.y, hexaHovered.x, hexaHovered.y, currentCharControlled.PM);
            if (path != null)
            {
                path.RemoveAt(0);
                foreach (Point p in path)
                {
                    GameObject go = GameObject.Instantiate(hexaTemplate, hexasFolder.transform);
                    go.SetActive(true);
                    go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x, p.y, -0.014f);
                    go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
                    go.GetComponent<Renderer>().sharedMaterial = pathDisplayMat;
                    go.GetComponent<Collider>().enabled = false;
                    pathFinderDisplay.Add(go);
                }
            }
        }
    }

    //Display the sighted hexas
    //Author : ?
    //Edited by Julien D'aboville L3L1
    void displayLineOfSight()
    {
        List<Point> hexasBlocked = null;
        // List<Point> pointList = hexaGrid.findHexasInSight(currentCharControlled.x, currentCharControlled.y, (actionType == ActionType.ATK1) ? currentCharControlled.getClassData().basicAttack.range : currentCharControlled.getClassData().skill_1.range, out hexasBlocked, currentCharControlled);
        // au lieu d'utilisr un operateur ternaire, on peut utiliser une condition if/else en incluant skill_2
        List<Point> pointList = null;

        if (actionType == ActionType.ATK1)
        {
            pointList = hexaGrid.findHexasInSight(currentCharControlled.x, currentCharControlled.y, currentCharControlled.getClassData().basicAttack.range, out hexasBlocked, currentCharControlled);
        }
        else if (actionType == ActionType.ATK2)
        {
            pointList = hexaGrid.findHexasInSight(currentCharControlled.x, currentCharControlled.y, currentCharControlled.getClassData().skill_1.range, out hexasBlocked, currentCharControlled);
        }
        else if (actionType == ActionType.ATK3)
        {
            pointList = hexaGrid.findHexasInSight(currentCharControlled.x, currentCharControlled.y, currentCharControlled.getClassData().skill_2.range, out hexasBlocked, currentCharControlled);
        }
        else if (actionType == ActionType.ATK4)
        {
            pointList = hexaGrid.findHexasInSight(currentCharControlled.x, currentCharControlled.y, currentCharControlled.getClassData().skill_3.range, out hexasBlocked, currentCharControlled);
        }



        bool hexaHoveredTargetable = false;
        // Display line of sight (Blue hexas)
        foreach (Point p in pointList)
        {
            if (hexaHovered != null && p.x == hexaHovered.x && p.y == hexaHovered.y) hexaHoveredTargetable = true;
            GameObject go = GameObject.Instantiate(hexaTemplate, hexasFolder.transform);
            go.SetActive(true);
            go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x, p.y, -0.015f);
            go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
            go.GetComponent<Renderer>().sharedMaterial = lineOfSightMat;
            go.GetComponent<Collider>().enabled = false;
            lineOfSightDisplay.Add(go);
        }
        // Display blocked hexas (transparent blue hexas)
        foreach (Point p in hexasBlocked)
        {
            GameObject go = GameObject.Instantiate(hexaTemplate, hexasFolder.transform);
            go.SetActive(true);
            go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x, p.y, -0.015f);
            go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
            go.GetComponent<Renderer>().sharedMaterial = blockedSightMat;
            go.GetComponent<Collider>().enabled = false;
            lineOfSightDisplay.Add(go);
        }
        if (hexaHoveredTargetable)
        {
            // List<Point> hexaPos = hexaGrid.getHexasWithinRange(hexaHovered.x, hexaHovered.y, (actionType == ActionType.ATK1) ? currentCharControlled.getClassData().basicAttack.rangeAoE : currentCharControlled.getClassData().skill_1.rangeAoE);
            // if with skill_2
            List<Point> hexaPos = null;
            if (actionType == ActionType.ATK1)
            {
                hexaPos = hexaGrid.getHexasWithinRange(hexaHovered.x, hexaHovered.y, currentCharControlled.getClassData().basicAttack.rangeAoE);
            }
            else if (actionType == ActionType.ATK2)
            {
                hexaPos = hexaGrid.getHexasWithinRange(hexaHovered.x, hexaHovered.y, currentCharControlled.getClassData().skill_1.rangeAoE);
            }
            else if (actionType == ActionType.ATK3)
            {
                hexaPos = hexaGrid.getHexasWithinRange(hexaHovered.x, hexaHovered.y, currentCharControlled.getClassData().skill_2.rangeAoE);
            }
            else if (actionType == ActionType.ATK4)
            {
                hexaPos = hexaGrid.getHexasWithinRange(hexaHovered.x, hexaHovered.y, currentCharControlled.getClassData().skill_3.rangeAoE);
            }


            // Display AoE (red hexas)
            foreach (Point p in hexaPos)
            {
                GameObject go = GameObject.Instantiate(hexaTemplate, hexasFolder.transform);
                go.SetActive(true);
                go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x, p.y, -0.014f);
                go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
                go.GetComponent<Renderer>().sharedMaterial = aoeMat;
                go.GetComponent<Collider>().enabled = false;
                lineOfSightDisplay.Add(go);
            }
        }
    }
}

//Author Socrate Louis Deriza, L3C1
public class EndGameDataCharacter
{
    public static int damageTeam0 = 0;
    public static int damageTeam1 = 0;

    public CharClass typeCharacter;
    public int teamCharacter;
    public int hpCharacter;

    public Character character;

    //dégât infligés par le personnage dans la partie
    public int damageDealt;
    //points de vies soignés au cours de la partie 
    public int hpRestored;
    //nombre d'actions faite par le personnage au cours de la partie
    public int numberOfAction;
    //nombre d'actions dangereuse (attaque) faite par le personnage au cours de la partie (isolation des utilisation d'attaques et des compétences du reste)
    public int numberOfDangerousAction;
    //nombre de case parcouru par le personnage au cours de ces déplacements
    public int numberOfSlotCrossedByTheCharacter;

    public int numberOfOpponentsEliminated;

    public EndGameDataCharacter(Character charac, CharClass chararcter, int team)
    {

        this.typeCharacter = chararcter;
        this.teamCharacter = team;
        
        if(chararcter == CharClass.GUERRIER)
        {
            this.hpCharacter = 17;
        }
        else if (chararcter == CharClass.VALKYRIE)
        {
            this.hpCharacter = 14;
        }
        else if (chararcter == CharClass.ENVOUTEUR)
        {
            this.hpCharacter = 11;
        }
        else if (chararcter == CharClass.DRUIDE)
        {
            this.hpCharacter = 11;
        }
        else if (chararcter == CharClass.SOIGNEUR)
        {
            this.hpCharacter = 13;
        }
        else if (chararcter == CharClass.MAGE)
        {
            this.hpCharacter = 10;
        }
        else if (chararcter == CharClass.ARCHER)
        {
            this.hpCharacter = 13;
        }
        else
        {
            this.hpCharacter = 11;
        }
        this.character = charac;
        this.numberOfAction = 0;
        this.damageDealt = 0;
        this.hpRestored = 0;
        this.numberOfAction = 0;
        this.numberOfDangerousAction = 0;
        this.numberOfSlotCrossedByTheCharacter = 0;
        this.numberOfOpponentsEliminated = 0;


    }

    public void addDamage(int damageCharacter)
    {
        this.damageDealt += damageCharacter;
        if(this.teamCharacter == 0)
        {
            EndGameDataCharacter.damageTeam0 += damageCharacter;
        }
        else
        {
            EndGameDataCharacter.damageTeam1 += damageCharacter;
        }
    }
   
    public void addAction(MainGame.ActionType newAction)
    {
        this.numberOfAction++;
        if(newAction == MainGame.ActionType.ATK1 || newAction == MainGame.ActionType.ATK2 || newAction == MainGame.ActionType.ATK3 || newAction== MainGame.ActionType.ATK4)
        {
            this.numberOfDangerousAction++;
        }
         
    }

    public void addMovement(int numberHexa)
    {
        this.numberOfSlotCrossedByTheCharacter+=numberHexa;
    }

    public void addAnHealingAction(int numberOfHpRestored)
    {
        this.hpRestored += numberOfHpRestored;
        /*for(int i = 0; i< numberOfHpRestored; i++)
        {
            this.hpRestored++;
        }*/
    }

    public float getPercentageOfActivity()
    {
        return ((((float)this.numberOfDangerousAction * 100.0f)/(float) this.numberOfAction));
    }

    public void hasKilledSomeOne()
    {
        this.numberOfOpponentsEliminated++;
    }

    public float getDamagePercentageOfTHisTeam()
    {
        if(teamCharacter == 0)
        {
            return ((((float)this.damageDealt * 100.0f) / (float)EndGameDataCharacter.damageTeam0));
        }
        else
        {
            return ((((float)this.damageDealt * 100.0f) / (float)EndGameDataCharacter.damageTeam1));
        }
    }
    

    public int getnumberOfSlotCrossedByTheCharacter()
    {
        return (this.numberOfSlotCrossedByTheCharacter);
    }

    public float getPercentageOfHpRestored()
    {
        return ((((float)this.hpRestored * 100.0f) / (float)this.hpCharacter));
    }
}
