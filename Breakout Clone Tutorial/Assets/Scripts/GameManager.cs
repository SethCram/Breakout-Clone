using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public Text scoreText;
    public Text ballsText;
    public Text LevelText;
    public Text highscoreText;
    //public Text titleText; //don't need bc aren't changing or altering text

    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelLevelCompleted;
    public GameObject panelGameOver;
    public GameObject panelPause;
    public GameObject panelHelp;

    //to keep track of what panel active: (mainly for help screen)
    private GameObject currPanel;

    public GameObject[] levels;

    GameObject _currBall; //to store instantiated Ball
    GameObject _currLevel; //bc instantiating lvls as objects
    GameObject _currPlayer;
    bool _isSwitchingState;

    //music:
    private AudioSource audioSrc;
    public AudioClip[] clipStorage;
    private List<AudioClip> clipsLeftToPlay; //to load all possible audio clips into at start of game
    private bool musicShouldPlay;

    //not good practice: (singleton variable?)
    public static GameManager Instance { get; private set; } //allows a prefab to 'get' an instance of the GameManager (can't set outside tho)

    //state stuff:
    public enum State { MENU, INIT, PLAY, LEVELCOMPLETED, LOADLEVEL, GAMEOVER, PAUSE, HELP };
    private State _state;
    private State unpauseState;

    public bool DrBC_Mode;

    private int LEFT_CLICK = 0;

    // Start is called before the first frame update
    void Start()
    {
        //go to menu when start game
        SwitchState(State.MENU); //call def'd state names

        Instance = this; //initializes instance

        audioSrc = GetComponent<AudioSource>();

        clipsLeftToPlay = new List<AudioClip>(); //init list
        ResetClipsToPlay();

        //Could use: PlayerPrefs.DeleteKey("highscore"); //resets highscore everytime start a session
    }

    // Update is called once per frame
    void Update()
    {
        //if left click
        if (Input.GetMouseButtonDown(LEFT_CLICK))
        {
            Debug.Log("Cursor should be confined to game screen.");

            //confine cursor to game window
            Cursor.lockState = CursorLockMode.Confined;
        }

        //stop playing music if it should stop:
        if (musicShouldPlay == false)
        {
            //audioSrc.Stop();
            audioSrc.Pause();
        }

        //play random clip if audio src not playing but it should be (runs at end of previous clip):
        if (audioSrc.isPlaying != true && musicShouldPlay == true)
        {
            PlayRandomClip();
        }

        if ( Input.GetKeyDown(KeyCode.Escape) && _state != State.MENU && _state != State.PAUSE ) //if pressed escape key, not in menu or paused
        {
            unpauseState = _state;

            SwitchState(State.PAUSE); //go to pause state
        }

        switch (_state) //deping on state running in, constantly do these checks
        {
            case State.MENU:
                //update currPanel if needed:
                if( currPanel != panelMenu)
                {
                    currPanel = panelMenu;    
                }

                break;
            case State.INIT:
                break;
            case State.PLAY:
                //only instantiates new ball w/ not currently one in play:
                if(_currBall == null) 
                {
                    //if dr bc mode or balls left:
                    if( DrBC_Mode == true || Balls > 0 )
                    {
                        _currBall = Instantiate(ballPrefab);
                    }
                    else //no balls left and not in dr bc mode
                    {
                        SwitchState(State.GAMEOVER);
                    }
                }

                if(_currLevel != null && _currLevel.transform.childCount == 0 && _isSwitchingState == false) //if no lvls left + not switching states
                {
                    SwitchState(State.LEVELCOMPLETED);
                }
                break;
            case State.LEVELCOMPLETED:
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
                if( Input.anyKeyDown)
                {
                    SwitchState(State.MENU);
                }
                break;
            case State.PAUSE:
                //update currPanel if needed:
                if (currPanel != panelPause)
                {
                    currPanel = panelPause;
                }
                break;
            case State.HELP:
                break;
        }
    }

    //play a random clip from 'clips' list and remove it from the list:
    private void PlayRandomClip()
    {
        if (clipsLeftToPlay.Count > 0) //if clips left to play
        {
            //assign random clip to audio src:
            audioSrc.clip = clipsLeftToPlay[Random.Range(0, clipsLeftToPlay.Count)]; //should it be 'clipsLeftToPlay.Count+1' since far edge of range exclusive?

            //remove selected clip from list:
            clipsLeftToPlay.Remove(audioSrc.clip);

            //play clip:
            audioSrc.Play();

            musicShouldPlay = true;
        }
       
    }

    private void ResetClipsToPlay()
    {
        //clear curr list before adding audio clips:
        clipsLeftToPlay.Clear();
        foreach (AudioClip storedClip in clipStorage)
        {
            clipsLeftToPlay.Add(storedClip);
        }
    }


    private int _score;

    public int Score
    {
        get { return _score; }
        set { _score = value;
            scoreText.text = "SCORE: " + _score; //update score txt
        }
    }

    private int level;

    public int Level
    {
        get { return level; }
        set { level = value;
            LevelText.text = "LEVEL: " + level;
        }
    }

    private int _balls;

    public int Balls
    {
        get { return _balls; }
        set { _balls = value;
            ballsText.text = "LIVES: " + _balls; //whenever change balls, text updated
        }
    }

    //attach this method to PlayButton by linking gamemanager to this button under "On Click ()" and selecting this method:
    public void PlayClicked() //need public to show in inspector
    {
        SwitchState(State.INIT);
    }

    public void DrBC_PlayClicked()
    {
        SwitchState(State.INIT);

        DrBC_Mode = true; //while set, lives will never go below max
    }

    public void ResumeClicked()
    {
        SwitchState( unpauseState);
    }

    public void ExitClicked()
    {
        print("Close Application");

        //UnityEditor.EditorApplication.isPlaying = false; //not needed when exported, just for testing (crashes exporting)

        Application.Quit();
    }

    public void MenuClicked()
    {
        SwitchState(State.MENU);
    }

    public void HelpClicked()
    {
        panelHelp.SetActive(true);

        //also have to clear curr panel:
        currPanel.SetActive(false);
    }

    public void Help_BackClicked() 
    {
        panelHelp.SetActive(false);

        //also have to set curr panel:
        currPanel.SetActive(true);

        //also need to set unpauseState? no, should still be set
    }

    //to change state to passed in state:
    public void SwitchState(State newState, float delay = 0) //aka next state logic process?
    {
        //print("Switch state to " + newState);

        //waits zero seconds unless 'delay' arg overwritten:
        StartCoroutine(SwitchDelay(newState, delay));
    }

    private IEnumerator SwitchDelay(State newState, float delay) //changes _state to specified state and passes to end then begin after changing
    {
        _isSwitchingState = true;

        yield return new WaitForSeconds(delay);

        EndState();

        _state = newState; //bc just changed states below

        BeginState(newState);

        _isSwitchingState = false;

    }

    //does operations based on current state
    private void BeginState(State newState) //aka output logic process
    {
        //print("Current state: " + newState);

        switch (newState)
        {
            case State.MENU:

                panelPlay.SetActive(false); //disable the overlay UI

                DrBC_Mode = false; //rst dr bc mode

                ResetClipsToPlay();
                PlayRandomClip();

                //makes sure no lvl in background when menu is up:
                if( _currLevel != null)
                {
                    Destroy(_currLevel);
                }

                if( _currBall != null)
                {
                    Destroy(_currBall);
                }

                if (_currPlayer != null)
                {
                    Destroy(_currPlayer);
                }

                Cursor.visible = true;

                //if new highscore achieved + not invinsible
                if (Score > PlayerPrefs.GetInt("highscore") && DrBC_Mode == false) //don't have to inititalize playerprefs vals bc starts at 0
                {
                    PlayerPrefs.SetInt("highscore", Score);
                }
                highscoreText.text = "HIGHSCORE: " + PlayerPrefs.GetInt("highscore");

                panelMenu.SetActive(true);
                break;
            case State.INIT: //for resetting score and such
                Cursor.visible = false;
                panelPlay.SetActive(true); //panelPlay = game overlay
                Score = 0; //also updated in setter
                Level = 0;
                Balls = 3;

                if(_currLevel != null) //so doesn't destroy 1st time around
                {
                    Destroy(_currLevel);
                }
                if (_currPlayer == null)
                {
                    _currPlayer = Instantiate(playerPrefab);
                }
                SwitchState(State.LOADLEVEL);
                break;
            case State.PLAY:
                break;
            case State.LEVELCOMPLETED:
                
                //destroy ball and level:
                Destroy(_currBall);
                Destroy(_currLevel);

                Level++; //add to level method

                panelLevelCompleted.SetActive(true);

                SwitchState(State.LOADLEVEL,2f);
                break;
            case State.LOADLEVEL:
                //no more lvls to load:
                if (Level >= levels.Length) //capitalized 'Level' not lowercase 'level'? (Level is the method?)
                {
                    SwitchState(State.GAMEOVER);
                }
                else
                {
                    _currLevel = Instantiate(levels[Level]);

                    SwitchState(State.PLAY);
                }
                break;
            case State.GAMEOVER:
                panelGameOver.SetActive(true);
                break;
            case State.PAUSE:
                panelPause.SetActive(true);
                pauseApp(); //apply pause settings
                break;
                /*
            case State.HELP:
                //enable help panel
                panelHelp.SetActive(true);

                //panelPause.SetActive(false); //should be done w/ leaving pause state

                //make sure still acts like paused:
                pauseApp(); //may not be needed

                break;
                */
        }
    }

    //apply pause settings:
    public void pauseApp()
    {
        musicShouldPlay = false; //should pause music
        Cursor.visible = true;
        Time.timeScale = 0; //pause time
    }

    private void EndState()
    {
        //print("State ending: " + _state);

        switch (_state)
        {
            case State.MENU:
                panelMenu.SetActive(false);
                break;
            case State.INIT:
                break;
            case State.PLAY:
                break;
            case State.LEVELCOMPLETED:
                panelLevelCompleted.SetActive(false);
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
                musicShouldPlay = false;
                panelPlay.SetActive(false);
                panelGameOver.SetActive(false);
                break;
            case State.PAUSE:
                //should resume playing paused audio src :
                audioSrc.Play();
                musicShouldPlay = true;

                Cursor.visible = false;
                panelPause.SetActive(false);
                Time.timeScale = 1; //resume time
                break;
                /*
            case State.HELP:
                //disable help panel:
                panelHelp.SetActive(false);
                break;
                */
        }
    }
}



