using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Characters;
using AI_Class;
using Mono.Data.Sqlite;
using System.Data;
using System;


public enum PlayerType : byte { HUMAN, AI_CPU_Offense, AI_EASY, AI_MEDIUM, AI_HARD, AI_CPU_Defense };

// Data structure used to send game info when switching scenes
public class StartGameData
{
    public bool loadSave;
    public List<CharClass> charsTeam1 = new List<CharClass>();
    public List<CharClass> charsTeam2 = new List<CharClass>();
    public PlayerType player1Type;
    public PlayerType player2Type;
    public int mapChosen; // 0 = Ruins, 1 = Random
    public int nbGames;
    public float slider;
    public StartGameData() { }
}

///<summary>
///Functions and methods for the Main menu
///</summary>
public class MainMenu : MonoBehaviour
{
    public static StartGameData startGameData;
    public Texture secondBackground;
    public Texture firstBackground;
    int nb = 5; //nb of characters (1 to 5)
    public GameObject background;
    public GameObject mainMenu;
    public GameObject CharSelectMenu;
    public GameObject advancedOptionsMenu;
    public List<Texture> charCards;
    public GameObject charsTeam1Display;
    public GameObject charsTeam2Display;
    public GameObject teamSelectHighlight;
    public GameObject credits;
    public GameObject guide;
    public Text errorLoad;
    // Main menu buttons
    public Button buttonPlay;
    public Button buttonLoad;
    public Button buttonQuit;
    public Button buttonCredits;
    public Button buttonGuide;
    // Char select menu buttons
    public List<Button> buttonCharCards;
    public List<Button> buttonTeam1Cards;
    public List<Button> buttonTeam2Cards;
    public Button buttonBackTeam1;
    public Button buttonBackTeam2;
    public Button buttonReadyTeam1;
    public Button buttonReadyTeam2;
    public Button buttonToAdvancedOptions;
    // Advanced options menu buttons
    public Button buttonToCharSelect;
    public Button v1;
    public Button v2;
    public Button v3;
    public Button v4;
    public Button v5;
    public Toggle toggleConsoleMode;
    public Slider sliderNbGames;
    public GameObject textNbGames;

    bool v5Pressed = false;
    bool v4Pressed = false;
    bool v3Pressed = false;
    bool v2Pressed = false;
    bool v1Pressed = false;
    bool buttonPlayPressed = false;
    bool buttonLoadPressed = false;
    bool buttonQuitPressed = false;
    bool buttonCreditsPressed = false;
    bool buttonGuidePressed = false;
    bool buttonStatsPressed = false;
    bool[] buttonCharCardsPressed = new bool[9] { false, false, false, false, false, false, false, false,false };//edited by Julien
    bool[] buttonTeam1Enable = new bool[9] { true, true, true, true, true, true, true, true,true };//edited by Julien
    bool[] buttonTeam2Enable = new bool[9] { true, true, true, true, true, true, true, true,true };//edited by Julien
    bool[] buttonTeam1CardsPressed = new bool[5] { false, false, false, false, false };
    bool[] buttonTeam2CardsPressed = new bool[5] { false, false, false, false, false };
    bool buttonBackTeam1Pressed = false;
    bool buttonBackTeam2Pressed = false;
    bool buttonReadyTeam1Pressed = false;
    bool buttonReadyTeam2Pressed = false;
    bool buttonToAdvancedOptionsPressed = false;
    bool buttonToCharSelectPressed = false;

    public Slider slider;
    public Dropdown dropdownPlayer1Type;
    public Dropdown dropdownPlayer2Type;
    public Dropdown dropdownMap;
    PlayerType player1Type;
    PlayerType player2Type;

    List<CharClass> charsTeam1 = new List<CharClass>();
    List<CharClass> charsTeam2 = new List<CharClass>();
    int currentTeam = 0;
    bool consoleMode;
    int nbGames;

    //Awake is called before Start
    void Awake()
    {
        Application.targetFrameRate = 75;
        QualitySettings.vSyncCount = 0;
    }

    // Start is called before the first frame update
    //Edited by Socrate Louis Deriza L3C1
    void Start()
    {
        mainMenu.SetActive(true);
      
        buttonPlay.onClick.AddListener(buttonPlayPressed_);
        buttonLoad.onClick.AddListener(buttonLoadPressed_);
        buttonQuit.onClick.AddListener(buttonQuitPressed_);
        buttonCredits.onClick.AddListener(buttonCreditsPressed_);
        buttonGuide.onClick.AddListener(buttonGuidePressed_);
        charsTeam1Display.transform.GetChild(1).gameObject.SetActive(true);

        replaceCharacterAccordingTheirRole();

        buttonCharCards[0].onClick.AddListener(() => buttonCharCardsPressed_(0));
        buttonCharCards[1].onClick.AddListener(() => buttonCharCardsPressed_(1));
        buttonCharCards[2].onClick.AddListener(() => buttonCharCardsPressed_(2));
        buttonCharCards[3].onClick.AddListener(() => buttonCharCardsPressed_(3));
        buttonCharCards[4].onClick.AddListener(() => buttonCharCardsPressed_(4));
        buttonCharCards[5].onClick.AddListener(() => buttonCharCardsPressed_(5));
        buttonCharCards[6].onClick.AddListener(() => buttonCharCardsPressed_(6));
        buttonCharCards[7].onClick.AddListener(() => buttonCharCardsPressed_(7)); // ajouter Valkyrie, L3C1 Y,H
        buttonCharCards[8].onClick.AddListener(() => buttonCharCardsPressed_(8)); //edited by Julien

        buttonTeam1Cards[0].onClick.AddListener(() => buttonTeam1CardsPressed_(0));
        buttonTeam1Cards[1].onClick.AddListener(() => buttonTeam1CardsPressed_(1));
        buttonTeam1Cards[2].onClick.AddListener(() => buttonTeam1CardsPressed_(2));
        buttonTeam1Cards[3].onClick.AddListener(() => buttonTeam1CardsPressed_(3));
        buttonTeam1Cards[4].onClick.AddListener(() => buttonTeam1CardsPressed_(4));
        buttonTeam2Cards[0].onClick.AddListener(() => buttonTeam2CardsPressed_(0));
        buttonTeam2Cards[1].onClick.AddListener(() => buttonTeam2CardsPressed_(1));
        buttonTeam2Cards[2].onClick.AddListener(() => buttonTeam2CardsPressed_(2));
        buttonTeam2Cards[3].onClick.AddListener(() => buttonTeam2CardsPressed_(3));
        buttonTeam2Cards[4].onClick.AddListener(() => buttonTeam2CardsPressed_(4));

        buttonBackTeam1.onClick.AddListener(buttonBackTeam1Pressed_);
        buttonBackTeam2.onClick.AddListener(buttonBackTeam2Pressed_);
        buttonReadyTeam1.onClick.AddListener(buttonReadyTeam1Pressed_);
        buttonReadyTeam2.onClick.AddListener(buttonReadyTeam2Pressed_);
        buttonToAdvancedOptions.onClick.AddListener(buttonToAdvancedOptionsPressed_);

        v5.onClick.AddListener(button5v5Pressed_);
        v4.onClick.AddListener(button4v4Pressed_);
        v3.onClick.AddListener(button3v3Pressed_);
        v2.onClick.AddListener(button2v2Pressed_);
        v1.onClick.AddListener(button1v1Pressed_);
        buttonToCharSelect.onClick.AddListener(buttonToCharSelectPressed_);

        //show load button if there is a save file
        
        if (File.Exists(Application.streamingAssetsPath + "/Save/saveGame.db"))
        {
            string conn = "URI=file:" + Application.streamingAssetsPath + "/Save/saveGame.db"; //Path to database.
            IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
            
            dbconn.Open(); //Open connection to the database
        
            IDbCommand dbcmd = dbconn.CreateCommand();
            IDataReader reader;
           
            dbcmd.CommandText = "SELECT player1 NBchar FROM game";
            reader = dbcmd.ExecuteReader();
            reader.Read();
            int res = reader.GetInt32(0);
            Debug.Log("Res = " + res);
            
            if (res == -1)
            {
                buttonLoad.gameObject.SetActive(false);
            }
            else
            {
                buttonLoad.gameObject.SetActive(true);
            }
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
        else
        {
            errorLoad.gameObject.SetActive(true);
        }


        consoleMode = true;
        nbGames = 1;
        // Read Options from options file
        if (File.Exists(Application.streamingAssetsPath + "/Data/Options/options"))
        {
            loadOptions();
        }
        else
        {
            saveOptions();
        }
        toggleConsoleMode.isOn = consoleMode;
        sliderNbGames.value = Mathf.Sqrt((float)nbGames);
        if (startGameData != null)
        {
            slider.value = startGameData.slider;
        }
    }

    

    void Update()
    {
        // MAIN MENU

        if (mainMenu.activeInHierarchy)
        {
            // Go to character selection menu
            if (buttonPlayPressed)
            {
                credits.SetActive(false);
                guide.SetActive(false);
                mainMenu.SetActive(false);
                CharSelectMenu.SetActive(true);
                initCharSelectMenu();
                buttonPlayPressed = false;
                background.GetComponent<RawImage>().texture = secondBackground;
            }
            // Load saved game
            if (buttonLoadPressed)
            {
                MainGame.startGameData = new StartGameData();
                MainGame.startGameData.loadSave = true;
                SceneManager.LoadScene(1);
                buttonLoadPressed = false;
            }
            // Show credits
            if (buttonCreditsPressed)
            {
                credits.SetActive(!credits.activeInHierarchy);
                buttonCreditsPressed = false;
            }

            //Guide
            if (buttonGuidePressed)
            {
                guide.SetActive(!guide.activeInHierarchy);
                buttonGuidePressed = false;
            }



            //Stats
            if (buttonStatsPressed)
            {
                SceneManager.LoadScene(2);
                buttonStatsPressed = false;
            }

             //Quit
            if (buttonQuitPressed)
            {
                quitGame();
            }

            

            // *************************
            // CHARACTER SELECTION MENU
            // *************************
        }
        else if (CharSelectMenu.activeInHierarchy)
        {
            // Back to main menu
            if (buttonBackTeam1Pressed)
            {	
                for (int i = 0; i < 9; i++)//Edited by L3C1 Y,H , L3L1 Julien
                {
                    buttonCharCards[i].gameObject.SetActive(true);
                    buttonTeam1Enable[i] = true;
                    buttonTeam2Enable[i] = true;
                }
                mainMenu.SetActive(true);
                background.GetComponent<RawImage>().texture = firstBackground;
                CharSelectMenu.SetActive(false);
                buttonBackTeam1Pressed = false;
            }

            // Back to team 1
            if (buttonBackTeam2Pressed)
            {
                //for (int i = 0; i < 8; i++)
                    //buttonCharCards[i].gameObject.SetActive(buttonTeam1Enable[i]);
                charSelectMenuPreviousPlayer();
                buttonBackTeam2Pressed = false;
            }

            {
                List<CharClass> charsTeam = (currentTeam == 0) ? charsTeam1 : charsTeam2;
                GameObject charsTeamDisplay = (currentTeam == 0) ? charsTeam1Display : charsTeam2Display;
                bool[] buttonTeamCardsPressed = (currentTeam == 0) ? buttonTeam1CardsPressed : buttonTeam2CardsPressed;

                //Display 1, 2, 3, 4 or 5 character slot
                if (v5Pressed)
                {
                    // Activate all character slot 
                    buttonTeam1Cards[4].gameObject.SetActive(true);
                    buttonTeam2Cards[4].gameObject.SetActive(true);
                    buttonTeam1Cards[3].gameObject.SetActive(true);
                    buttonTeam2Cards[3].gameObject.SetActive(true);
                    buttonTeam1Cards[2].gameObject.SetActive(true);
                    buttonTeam2Cards[2].gameObject.SetActive(true);
                    buttonTeam1Cards[1].gameObject.SetActive(true);
                    buttonTeam2Cards[1].gameObject.SetActive(true);

                    nb = 5;

                    testAndDisplayCharRoster();

                    v5Pressed = false;
                }
                if (v4Pressed)
                {
                    for (int i = 4; i > 3; i--)
                    {	//Edited by L3C1 Y,H
                        charsTeam1Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];
                        charsTeam2Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];

                        if (charsTeam1.Count > i)
                        {
                            buttonTeam1Enable[(int)charsTeam1[i]] = true;
                            charsTeam1.RemoveAt(i);
                        }
                        if (charsTeam2.Count > i)
                        {
                            buttonTeam2Enable[(int)charsTeam2[i]] = true;
                            charsTeam2.RemoveAt(i);
                        }

                    }

                    // Activate character slot 1, 2, 3
                    buttonTeam1Cards[3].gameObject.SetActive(true);
                    buttonTeam2Cards[3].gameObject.SetActive(true);
                    buttonTeam1Cards[2].gameObject.SetActive(true);
                    buttonTeam2Cards[2].gameObject.SetActive(true);
                    buttonTeam1Cards[1].gameObject.SetActive(true);
                    buttonTeam2Cards[1].gameObject.SetActive(true);
                    // Deactivate character slot 4
                    buttonTeam1Cards[4].gameObject.SetActive(false);
                    buttonTeam2Cards[4].gameObject.SetActive(false);

                    nb = 4;

                    testAndDisplayCharRoster();

                    v4Pressed = false;
                }
                if (v3Pressed)
                {
                    for (int i = 4; i > 2; i--)
                    {	//Edited by L3C1 Y,H,Edited by L3L1 Julien
                        charsTeam1Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];
                        charsTeam2Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];

                        if (charsTeam1.Count > i)
                        {
                            buttonTeam1Enable[(int)charsTeam1[i]] = true;
                            charsTeam1.RemoveAt(i);
                        }
                        if (charsTeam2.Count   > i)
                        {
                            buttonTeam2Enable[(int)charsTeam2[i]] = true;
                            charsTeam2.RemoveAt(i);
                        }

                        //buttonTeam1Cards[i].gameObject.SetActive(false);
                        //buttonTeam2Cards[i].gameObject.SetActive(false);
                    }
                    // Activate slot 1 and 2
                    buttonTeam1Cards[2].gameObject.SetActive(true);
                    buttonTeam2Cards[2].gameObject.SetActive(true);
                    buttonTeam1Cards[1].gameObject.SetActive(true);
                    buttonTeam2Cards[1].gameObject.SetActive(true);

                    // Deactivate character slot 3 and 4
                    buttonTeam1Cards[4].gameObject.SetActive(false);
                    buttonTeam2Cards[4].gameObject.SetActive(false);
                    buttonTeam1Cards[3].gameObject.SetActive(false);
                    buttonTeam2Cards[3].gameObject.SetActive(false);
                    nb = 3;

                    testAndDisplayCharRoster();

                    v3Pressed = false;
                }
                if (v2Pressed)
                {
                    for (int i = 4; i > 1; i--)
                    {	//Edited by L3C1 Y,H , Edited by L3L1 Julien
                        charsTeam1Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];
                        charsTeam2Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];

                        if (charsTeam1.Count > i)
                        {
                            buttonTeam1Enable[(int)charsTeam1[i]] = true;
                            charsTeam1.RemoveAt(i);
                        }
                        if (charsTeam2.Count > i)
                        {
                            buttonTeam2Enable[(int)charsTeam2[i]] = true;
                            charsTeam2.RemoveAt(i);
                        }

                    }



                    // Activate character slot 1
                    buttonTeam1Cards[1].gameObject.SetActive(true);
                    buttonTeam2Cards[1].gameObject.SetActive(true);

                    // Deactivate character slot 2, 3 and 4
                    buttonTeam1Cards[4].gameObject.SetActive(false);
                    buttonTeam2Cards[4].gameObject.SetActive(false);
                    buttonTeam1Cards[3].gameObject.SetActive(false);
                    buttonTeam2Cards[3].gameObject.SetActive(false);
                    buttonTeam1Cards[2].gameObject.SetActive(false);
                    buttonTeam2Cards[2].gameObject.SetActive(false);
                    nb = 2;

                    testAndDisplayCharRoster();

                    v2Pressed = false;
                }
                if (v1Pressed)
                {
                    for (int i = 4; i > 0; i--)
                    {	//Edited by L3C1 Y,H, Edited by L3L1 Julien
                        charsTeam1Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];
                        charsTeam2Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];

                        if (charsTeam1.Count > i)
                        {
                            buttonTeam1Enable[(int)charsTeam1[i]] = true;
                            charsTeam1.RemoveAt(i);
                        }
                        if (charsTeam2.Count > i)
                        {
                            buttonTeam2Enable[(int)charsTeam2[i]] = true;
                            charsTeam2.RemoveAt(i);
                        }

                    }

                    // Deactivate character slot 1, 2, 3 and 4
                    buttonTeam1Cards[4].gameObject.SetActive(false);
                    buttonTeam2Cards[4].gameObject.SetActive(false);
                    buttonTeam1Cards[3].gameObject.SetActive(false);
                    buttonTeam2Cards[3].gameObject.SetActive(false);
                    buttonTeam1Cards[2].gameObject.SetActive(false);
                    buttonTeam2Cards[2].gameObject.SetActive(false);
                    buttonTeam1Cards[1].gameObject.SetActive(false);
                    buttonTeam2Cards[1].gameObject.SetActive(false);
                    nb = 1;

                    testAndDisplayCharRoster();

                    v1Pressed = false;
                }

                // CHARACTER SELECTION Edited by L3C1 Y,H , Edited By Julien
                for (int i = 0; i < 9; i++)
                {
                    if (buttonCharCardsPressed[i])
                    {
                        if (charsTeam.Count < nb)
                        {
                            charsTeam.Add((CharClass)i);

                            if (currentTeam == 0)
                            {
                                buttonTeam1Enable[i] = false;
                            }
                            else
                            {
                                buttonTeam2Enable[i] = false;
                            }
                            charsTeamDisplay.transform.GetChild(0).transform.GetChild(charsTeam.Count - 1).GetComponent<RawImage>().texture = charCards[i];

                            testAndDisplayCharRoster();
                        }

                        buttonCharCardsPressed[i] = false;
                    }
                }

                // REMOVE CHARACTER FROM TEAM
                for (int i = 0; i < 5; i++)
                {
                    if (buttonTeamCardsPressed[i])
                    {
                        if (charsTeam.Count > i)
                        {
                            int numCard = (int)charsTeam[i];
                            if (currentTeam == 0)
                            {
                                buttonTeam1Enable[numCard] = true;
                            }
                            else
                            {
                                buttonTeam2Enable[numCard] = true;
                            }
                            charsTeam.RemoveAt(i);
                            for (int j = i; j < charsTeam.Count; j++)
                            {
                                charsTeamDisplay.transform.GetChild(0).transform.GetChild(j).GetComponent<RawImage>().texture = charCards[(int)charsTeam[j]];
                            }
                            charsTeamDisplay.transform.GetChild(0).transform.GetChild(charsTeam.Count).GetComponent<RawImage>().texture = charCards[8];//Edited by L3C1 Y,H

                            testAndDisplayCharRoster();
                        }
                        buttonTeamCardsPressed[i] = false;
                    }
                }

                //Verifie si le joueur 2 n'ait pas autant de personnages que le joueur 1
                if (currentTeam == 1 && nb != charsTeam1.Count)
                {
                    charSelectMenuPreviousPlayer();
                }

                // READY
                if (charsTeam.Count == nb)
                {
                    if (currentTeam == 0) charsTeam1Display.transform.GetChild(2).gameObject.SetActive(true);
                    else charsTeam2Display.transform.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    if (currentTeam == 0) charsTeam1Display.transform.GetChild(2).gameObject.SetActive(false);
                    else charsTeam2Display.transform.GetChild(2).gameObject.SetActive(false);
                }
                if (buttonReadyTeam1Pressed)
                {
                    charSelectMenuNextPlayer();
                    buttonReadyTeam1Pressed = false;
                }
                else if (buttonReadyTeam2Pressed)
                {
                    Debug.Log("Start : P1 " + (PlayerType)dropdownPlayer1Type.value + " P2 " + (PlayerType)dropdownPlayer2Type.value);
                    //Debug.Log("Start test : P1 "+ player2Type.ToString()+" P2 test " + player2Type.ToString());
                    // Give Main all the info

                    MainGame.startGameData = new StartGameData();
                    MainGame.startGameData.loadSave = false;
                    MainGame.startGameData.charsTeam1 = charsTeam1;
                    MainGame.startGameData.charsTeam2 = charsTeam2;
                    MainGame.startGameData.player1Type = (PlayerType)dropdownPlayer1Type.value;
                    MainGame.startGameData.player2Type = (PlayerType)dropdownPlayer2Type.value;
                    MainGame.startGameData.mapChosen = dropdownMap.value;
                    MainGame.startGameData.nbGames = nbGames;
                    MainGame.startGameData.slider = slider.value;
                    buttonReadyTeam2Pressed = false;


                    // Mode Console deactivated
                    if (MainGame.startGameData.player1Type != PlayerType.HUMAN && MainGame.startGameData.player2Type != PlayerType.HUMAN &&
                        MainGame.startGameData.player1Type != PlayerType.AI_CPU_Offense && MainGame.startGameData.player2Type != PlayerType.AI_CPU_Offense && consoleMode &&
                        MainGame.startGameData.player1Type != PlayerType.AI_CPU_Defense && MainGame.startGameData.player2Type != PlayerType.AI_CPU_Defense && consoleMode)
                    {
                        // Load Console Mode scene
                        SceneManager.LoadScene(3);
                    }
                    else
                    {
                        SceneManager.LoadScene(1);
                    }
                }

                if (buttonToAdvancedOptionsPressed)
                {
                    advancedOptionsMenu.SetActive(true);
                    CharSelectMenu.SetActive(false);
                    buttonToAdvancedOptionsPressed = false;
                }
            }
        }
        else if (advancedOptionsMenu.activeInHierarchy)
        {
            if (buttonToCharSelectPressed)
            {
                advancedOptionsMenu.SetActive(false);
                CharSelectMenu.SetActive(true);
                buttonToCharSelectPressed = false;
                saveOptions();
            }
            consoleMode = toggleConsoleMode.isOn;
            nbGames = (int)(sliderNbGames.value * sliderNbGames.value);
            textNbGames.GetComponent<Text>().text = "(IA vs IA) nombre de parties : " + nbGames;
        }
    }

    void testAndDisplayCharRoster()
    {	//Edited by L3C1 Y,H , L3L1 Julien,
        Boolean displayRoster = false;

        if (currentTeam == 0 && charsTeam1.Count < nb)
            displayRoster = true;
        else if (currentTeam == 1 && charsTeam2.Count < nb)
            displayRoster = true;

        for (int i = 0; i < 9; i++) //9 persos
            buttonCharCards[i].gameObject.SetActive(displayRoster);
    }

    //Added by Socrate Louis Deriza L3C1
    //Edited by L3C1 CROUZET Oriane, 19/04/2023
    void replaceCharacterAccordingTheirRole()
    {
        RectTransform rectTransform = null;
        Vector3 vGuerrier = buttonCharCards[0].transform.position; //GUERRIER
        Vector3 vVoleur = buttonCharCards[1].transform.position; //VOLEUR
        float initialX = vGuerrier.x;
        float gapX = vVoleur.x - vGuerrier.x;
        float y = vGuerrier.y;
        float z = vGuerrier.z;

        rectTransform = buttonCharCards[8].GetComponent<RectTransform>(); //NAIN 
        rectTransform.position = new Vector3(buttonCharCards[8].transform.position.x, y, z);

        rectTransform = buttonCharCards[6].GetComponent<RectTransform>(); //VALKYRIE
        rectTransform.position = new Vector3((initialX + gapX), y, z);

        rectTransform = buttonCharCards[1].GetComponent<RectTransform>(); //VOLEUR
        rectTransform.position = new Vector3((initialX + 6 * gapX), y, z);

        rectTransform = buttonCharCards[7].GetComponent<RectTransform>(); //DRUIDE
        rectTransform.position = new Vector3((initialX + 2 * gapX), y, z);

        rectTransform = buttonCharCards[2].GetComponent<RectTransform>(); //ARCHER
        rectTransform.position = new Vector3((initialX + 7 * gapX), y, z);

        rectTransform = buttonCharCards[3].GetComponent<RectTransform>(); //ENVOUTEUR
        rectTransform.position = new Vector3((initialX + 5 * gapX), y, z);

        rectTransform = buttonCharCards[5].GetComponent<RectTransform>(); //MAGE
        rectTransform.position = new Vector3((initialX + 3 * gapX), y, z);

        float vX;
        vX = buttonCharCards[7].transform.position.x;
        rectTransform = buttonCharCards[7].GetComponent<RectTransform>();
        rectTransform.position = new Vector3((30 + vX), y, z);

        vX = buttonCharCards[5].transform.position.x;
        rectTransform = buttonCharCards[5].GetComponent<RectTransform>();
        rectTransform.position = new Vector3((30 + vX), y, z);

        vX = buttonCharCards[4].transform.position.x;
        rectTransform = buttonCharCards[4].GetComponent<RectTransform>();
        rectTransform.position = new Vector3((60 + vX), y, z);

        vX = buttonCharCards[3].transform.position.x;
        rectTransform = buttonCharCards[3].GetComponent<RectTransform>();
        rectTransform.position = new Vector3((60 + vX), y, z);

        vX = buttonCharCards[1].transform.position.x;
        rectTransform = buttonCharCards[1].GetComponent<RectTransform>();
        rectTransform.position = new Vector3((100 + vX), y, z);

        vX = buttonCharCards[2].transform.position.x;
        rectTransform = buttonCharCards[2].GetComponent<RectTransform>();
        rectTransform.position = new Vector3((150 + vX), y, z);

        vX = buttonCharCards[7].transform.position.x;
        rectTransform = buttonCharCards[7].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(buttonCharCards[0].transform.position.x, y, z);

        rectTransform = buttonCharCards[0].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(vX, y, z);

        vX = buttonCharCards[5].transform.position.x;
        rectTransform = buttonCharCards[5].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(buttonCharCards[6].transform.position.x, y, z);

        rectTransform = buttonCharCards[6].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(vX, y, z);

        vX = buttonCharCards[1].transform.position.x;
        rectTransform = buttonCharCards[1].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(buttonCharCards[0].transform.position.x, y, z);

        rectTransform = buttonCharCards[0].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(vX, y, z);

        vX = buttonCharCards[2].transform.position.x;
        rectTransform = buttonCharCards[2].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(buttonCharCards[6].transform.position.x, y, z);

        rectTransform = buttonCharCards[6].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(vX, y, z);

        vX = buttonCharCards[5].transform.position.x;
        rectTransform = buttonCharCards[5].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(buttonCharCards[3].transform.position.x, y, z);

        rectTransform = buttonCharCards[3].GetComponent<RectTransform>();
        rectTransform.position = new Vector3(vX, y, z);


    }

    void initCharSelectMenu()
    {	//Edited by L3C1 Y,H ,L3L1 Julien
        charsTeam1 = new List<CharClass>();
        charsTeam2 = new List<CharClass>();
        for (int i = 0; i < 5; i++) charsTeam1Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];
        for (int i = 0; i < 5; i++) charsTeam2Display.transform.GetChild(0).transform.GetChild(i).GetComponent<RawImage>().texture = charCards[9];

        for (int i = 0; i < 5; i++) charsTeam1Display.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>().enabled = true;
        for (int i = 0; i < 5; i++) charsTeam2Display.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>().enabled = false;

        charsTeam1Display.transform.GetChild(1).gameObject.SetActive(true);
        charsTeam1Display.transform.GetChild(2).gameObject.SetActive(false);

        charsTeam2Display.transform.GetChild(1).gameObject.SetActive(false);
        charsTeam2Display.transform.GetChild(2).gameObject.SetActive(false);
    }

    void charSelectMenuNextPlayer()
    {
        currentTeam = 1;
        for (int i = 0; i < 5; i++) charsTeam1Display.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>().enabled = false;
        for (int i = 0; i < 5; i++) charsTeam2Display.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>().enabled = true;

        charsTeam1Display.transform.GetChild(1).gameObject.SetActive(false);
        charsTeam1Display.transform.GetChild(2).gameObject.SetActive(false);

        charsTeam2Display.transform.GetChild(1).gameObject.SetActive(true);
        charsTeam2Display.transform.GetChild(2).gameObject.SetActive(false);

        testAndDisplayCharRoster();

        teamSelectHighlight.transform.localPosition = new Vector3(0, 36 - 183, 0);
    }

    void charSelectMenuPreviousPlayer()
    {
        currentTeam = 0;
        for (int i = 0; i < 5; i++) charsTeam1Display.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>().enabled = true;
        for (int i = 0; i < 5; i++) charsTeam2Display.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>().enabled = false;

        charsTeam1Display.transform.GetChild(1).gameObject.SetActive(true);
        charsTeam1Display.transform.GetChild(2).gameObject.SetActive(true);

        charsTeam2Display.transform.GetChild(1).gameObject.SetActive(false);
        charsTeam2Display.transform.GetChild(2).gameObject.SetActive(false);

        testAndDisplayCharRoster();

        teamSelectHighlight.transform.localPosition = new Vector3(0, 36, 0);
    }


    void saveOptions()
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(Application.streamingAssetsPath + "/Data/Options/options", FileMode.Create)))
        {
            writer.Write((int)((consoleMode) ? 1 : 0));
            writer.Write(nbGames);
        }
    }

    void loadOptions()
    {
        using (BinaryReader reader = new BinaryReader(File.Open(Application.streamingAssetsPath + "/Data/Options/options", FileMode.Open)))
        {
            consoleMode = (reader.ReadInt32() == 0) ? false : true;
            nbGames = reader.ReadInt32();
        }
    }

    public void quitGame()
    {
        Application.Quit();
        Debug.Log("You Quit The Game");
    }

    // Events
    void button5v5Pressed_() { v5Pressed = true; }
    void button4v4Pressed_() { v4Pressed = true; }
    void button3v3Pressed_() { v3Pressed = true; }
    void button2v2Pressed_() { v2Pressed = true; }
    void button1v1Pressed_() { v1Pressed = true; }
    void buttonPlayPressed_() { buttonPlayPressed = true; }
    void buttonLoadPressed_() { buttonLoadPressed = true; }
    void buttonQuitPressed_() { buttonQuitPressed = true; }
    void buttonCreditsPressed_() { buttonCreditsPressed = true; }
    void buttonGuidePressed_() { buttonGuidePressed = true; }
    void buttonStatsPressed_() { buttonStatsPressed = true; }
    void buttonCharCardsPressed_(int i) { buttonCharCardsPressed[i] = true; }
    void buttonTeam1CardsPressed_(int i) { buttonTeam1CardsPressed[i] = true; }
    void buttonTeam2CardsPressed_(int i) { buttonTeam2CardsPressed[i] = true; }
    void buttonBackTeam1Pressed_() { buttonBackTeam1Pressed = true; }
    void buttonBackTeam2Pressed_() { buttonBackTeam2Pressed = true; }
    void buttonReadyTeam1Pressed_() { buttonReadyTeam1Pressed = true; }
    void buttonReadyTeam2Pressed_() { buttonReadyTeam2Pressed = true; }
    void buttonToAdvancedOptionsPressed_() { buttonToAdvancedOptionsPressed = true; }
    void buttonToCharSelectPressed_() { buttonToCharSelectPressed = true; }
}
