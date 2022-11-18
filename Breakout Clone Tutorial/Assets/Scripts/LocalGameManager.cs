﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class LocalGameManager : MonoBehaviour
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
    public GameObject panelPlayAItext;

    //to keep track of what panel active: (mainly for help screen)
    private GameObject currPanel;

    public GameObject[] levels;

    GameObject _currBall; //to store instantiated Ball
    Ball _currBallBallComp;
    GameObject _currLevel; //bc instantiating lvls as objects
    GameObject _currPlayer;
    bool _isSwitchingState;

    //music:
    private AudioSource audioSrc;
    public AudioClip[] clipStorage;
    private List<AudioClip> clipsLeftToPlay; //to load all possible audio clips into at start of game
    private bool musicShouldPlay;

    //state stuff:
    public enum State { MENU, INIT, PLAY, LEVELCOMPLETED, LOADLEVEL, GAMEOVER, PAUSE, HELP };
    private State _state;
    private State unpauseState;

    public bool DrBC_Mode;

    public bool training = false;
    [SerializeField] private GameObject playerAIPrefab;

    private HitBallAgent hitBallAgentAI;

    private Stopwatch rewardStopWatch;

    private Stopwatch demoStopWatch;
    private bool demoActive = false;
    public float demoSecondsTimeout = 10f;
    private Vector3 prevMousePos = Vector3.zero;

    //private int LEFT_CLICK = 0;

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        //go to menu when start game
        SwitchState(State.MENU); //call def'd state names

        audioSrc = GetComponent<AudioSource>();

        clipsLeftToPlay = new List<AudioClip>(); //init list
        ResetClipsToPlay();

        if(training)
        {
            //mute the audio
            audioSrc.mute = true;

            //don't wait for menu
            SwitchState(State.INIT);
        }
        //if not training
        else
        {
            //reset cam to proper spot
            FindObjectOfType<Camera>().GetComponent<Transform>().position = new Vector3(0, 0, BreakoutConstants.SOLO_CAM_START_Z);
            FindObjectOfType<Camera>().orthographicSize = BreakoutConstants.SOLO_CAM_SIZE;
        }

        //Could use: PlayerPrefs.DeleteKey("highscore"); //resets highscore everytime start a session
    }

    // Update is called once per frame
    void Update()
    {
        //if left click
        /*
        if (Input.GetMouseButtonDown(LEFT_CLICK))
        {
            Debug.Log("Cursor should be confined to game screen.");

            //confine cursor to game window
            Cursor.lockState = CursorLockMode.Confined;
        }
        */

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

        //escape or p to pause
        if ( Input.GetButtonDown("Cancel") && _state != State.MENU && _state != State.PAUSE ) //if pressed escape key, not in menu or paused
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

                //check to see if demo stopwatch is done
                if( demoStopWatch != null && !demoActive && demoStopWatch.ElapsedMilliseconds > demoSecondsTimeout * 1000)
                {
                    //if demo time

                    demoActive = true;

                    SwitchState(State.INIT); //will possibly trigger mult times before state switch?
                }

                //if any key pressed or mouse moved
                if( Input.anyKeyDown || Input.mousePosition != prevMousePos )
                {
                    //restart stop watch
                    demoStopWatch = Stopwatch.StartNew();

                    //debug: print("demo stopwatch restarted");
                }
                //update mouse pos
                prevMousePos = Input.mousePosition;

                break;
            case State.INIT:
                break;
            case State.PLAY:
                
                //if not in dr bc mode and no balls left, gameover
                if( !DrBC_Mode  && Balls < 0 )
                {
                    SwitchState(State.GAMEOVER);
                }
                //if (in dr bc mode or have balls left) and ball out of bounds
                else if( _currBallBallComp.outOfBounds )
                {
                    //respawn ball
                    _currBallBallComp.Respawn();

                    Balls--;

                    _currBallBallComp.outOfBounds = false;
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
                if( Input.anyKeyDown && !training)
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

    #endregion Unity Methods

    #region Audio

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

    #endregion Audio

    #region Getters and Setters

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

    #endregion Getters and Setters

    #region Buttons

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
        demoStopWatch.Stop();

        panelHelp.SetActive(true);

        //also have to clear curr panel:
        currPanel.SetActive(false);
    }

    public void Help_BackClicked() 
    {
        demoStopWatch = Stopwatch.StartNew();

        panelHelp.SetActive(false);

        //also have to set curr panel:
        currPanel.SetActive(true);

        //also need to set unpauseState? no, should still be set
    }

    #endregion Buttons

    //apply pause settings:
    public void pauseApp()
    {
        musicShouldPlay = false; //should pause music
        Cursor.visible = true;
        Time.timeScale = 0; //pause time
    }

    #region State

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

                //if new highscore achieved + not invinsible + demo not active (AI just played game)
                if (Score > PlayerPrefs.GetInt("highscore") && DrBC_Mode == false && !demoActive) //don't have to inititalize playerprefs vals bc starts at 0
                {
                    PlayerPrefs.SetInt("highscore", Score);
                }
                highscoreText.text = "HIGHSCORE: " + PlayerPrefs.GetInt("highscore");

                //make sure demo deactive now
                demoActive = false;

                //start demo stop watch
                demoStopWatch = Stopwatch.StartNew();

                panelMenu.SetActive(true);
                break;
            case State.INIT: //for resetting score and such
                Cursor.visible = false;
                panelPlay.SetActive(true); //panelPlay = game overlay
                Score = 0; //also updated in setter
                Level = 0;
                Balls = 3;

                //stop demo stop watch
                demoStopWatch.Stop();

                if(_currLevel != null) //so doesn't destroy 1st time around
                {
                    //print($"{_currLevel} should be destroyed on INIT.");

                    Destroy(_currLevel);
                }

                if (_currPlayer == null)
                {
                    if(training || demoActive)
                    {
                        _currPlayer = Instantiate(playerAIPrefab, parent: transform.parent);
                        hitBallAgentAI = _currPlayer.GetComponent<HitBallAgent>();
                        
                        panelPlayAItext.SetActive(true);
                    }
                    else
                    {
                        _currPlayer = Instantiate(playerPrefab, parent: transform.parent);

                        panelPlayAItext.SetActive(false);
                    }
                }
                //if already player
                else
                {
                    //if AI playing
                    if( hitBallAgentAI != null)
                    {
                        //respawn AI
                        hitBallAgentAI.Respawn();
                    }
                }

                if(_currBall == null)
                {
                    //spawn ball
                    _currBall = Instantiate(
                        ballPrefab, parent: transform.parent
                    );

                    _currBallBallComp = _currBall.GetComponent<Ball>();

                    //if AI playing
                    if( hitBallAgentAI != null)
                    {
                        //fill out AI and ball fields for one another
                        hitBallAgentAI.ball = _currBall.GetComponent<Transform>();
                        _currBallBallComp.hitBallAgentAI = hitBallAgentAI;
                    }

                    //if training
                    if( training )
                    {
                        //mute brick hit audio
                        _currBallBallComp.GetComponent<AudioSource>().mute = true;
                    }
                }
                SwitchState(State.LOADLEVEL);
                break;
            case State.PLAY:
                //debug lvl changes: SwitchState(State.LEVELCOMPLETED, delay: 2);

                //if AI playing
                if (hitBallAgentAI != null)
                {
                    rewardStopWatch = Stopwatch.StartNew();

                    //print($"Start Elapsed ms = {rewardStopWatch.ElapsedMilliseconds}");
                }

                break;
            case State.LEVELCOMPLETED:

                //destroy ball and level:
                //Destroy(_currBall);
                _currBall.SetActive(false);
                Destroy(_currLevel);

                //if AI playing
                if (hitBallAgentAI != null)
                {
                    //give higher reward quicker the level's beaten (typically addition of around 4 at 200_000 ms elapsed)
                    hitBallAgentAI.AddReward(1_000_000 * (1/rewardStopWatch.ElapsedMilliseconds) );

                    //deubg: print($"End Elapsed ms = {rewardStopWatch.ElapsedMilliseconds}");
                }
                    
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
                    _currLevel = Instantiate(levels[Level], parent: transform.parent);

                    _currBallBallComp.Respawn();

                    /*
                    //if AI playing
                    if(hitBallAgentAI != null)
                    {
                        //find all bricks in lvl
                        Brick[] bricks = FindObjectsOfType<Brick>();

                        //overwrite prev bricks arr
                        hitBallAgentAI.bricks = new Brick[bricks.Length];
                        //set to agent bricks
                        hitBallAgentAI.bricks = bricks;
                    }
                    */

                    SwitchState(State.PLAY);
                }
                break;
            case State.GAMEOVER:
                panelGameOver.SetActive(true);

                _currBall.SetActive(false);

                //if AI playing
                if(hitBallAgentAI != null)
                {
                    //if running in inference
                    if( hitBallAgentAI.runningInInference )
                    {
                        //print reward at end of episode
                        print($"Cumulative reward: {hitBallAgentAI.GetCumulativeReward()}");
                    }

                    hitBallAgentAI.EndEpisode();

                    //if training
                    if( training )
                    {
                        //reset to init after delay
                        SwitchState(State.INIT, delay: 2f);
                    }
                    //running in inference(/hueristic?)
                    else
                    {
                        //go back to main menu after delay
                        SwitchState(State.MENU, delay: 2f);
                    }
                }

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

    #endregion State
}