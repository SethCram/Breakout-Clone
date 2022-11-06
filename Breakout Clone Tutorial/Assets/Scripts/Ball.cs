using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float _speed = 30f; //20f; //he uses '_' at start of vars that are private
    Rigidbody _rigidbodyComp;
    Vector3 _velocity;
    Renderer _renderer;
    private Vector3 moveToCenter;
    private Vector3 moveToCenterBottom;
    private bool checkingifStuck = false;
    private bool yStuck = false;
    private bool xStuck = false;
    private AudioSource brickHitAudio;

    [HideInInspector] public HitBallAgent hitBallAgentAI;

    [HideInInspector] public Boolean outOfBounds = false;

//public AudioClip brickHitAudio;

// Start is called before the first frame update
void Start()
    {

        _rigidbodyComp = GetComponent<Rigidbody>();

        _renderer = GetComponent<Renderer>();

        brickHitAudio = GetComponent<AudioSource>();

        //Respawn();
    }

    /// <summary>
    /// Randomly respawn ball through stopping its motion and launching it after a delay.
    /// </summary>
    public void Respawn()
    {
        print("Respawn Ball.");

        //stop ball motion
        _rigidbodyComp.velocity = Vector3.zero;

        //teleport ball to rando start pos
        transform.localPosition = new Vector3(
            UnityEngine.Random.Range(BreakoutConstants.LOCAL_POSITION_MIN, BreakoutConstants.LOCAL_POSITION_MAX),
            UnityEngine.Random.Range(BreakoutConstants.BALL_SPAWN_LOCAL_MIN_Y, BreakoutConstants.BALL_SPAWN_LOCAL_MAX_Y),
            0
        );

        //activate ball
        gameObject.SetActive(true);

        //launch it w/ a delay
        Invoke("Launch", 0.5f);
    }

    private void Launch()
    {
        _rigidbodyComp.velocity = Vector3.up * _speed;
    }

    // Update is called once per frame
    void Update()
    {
        moveToCenter = -transform.localPosition;

        moveToCenterBottom = Vector3.up*BreakoutConstants.LOCAL_PLAYER_Y - transform.localPosition;
    }

    //physics:
    private void FixedUpdate()
    {
        _rigidbodyComp.velocity = _rigidbodyComp.velocity.normalized * _speed; //normalized sets amplitude to 1, so speed = velocity of rigidbody

        _velocity = _rigidbodyComp.velocity; //without updating veloctiy before collision, it wont bounce when hits the paddle?

        //tries to unstuck if ball side to side at all: 
        if (0 <= _velocity.y && _velocity.y < 1 && checkingifStuck == false || -1 < _velocity.y && _velocity.y <= 0 && checkingifStuck == false)
        {
            StartCoroutine(Stuck());
        }

        //tries to unstuck if ball stuck up and down in corner:
        if (0 == _velocity.x && checkingifStuck == false )
        {
            StartCoroutine(Stuck());
        }

        //if ball's a bit below the player and not marked as out of bounds yet
        if (
            transform.localPosition.y < BreakoutConstants.LOCAL_PLAYER_Y - 3f &&
            !outOfBounds
        ) 
        {
            //GameManager.Instance.Balls--; //subtrcts 1 from amt of balls game manager has

            //if AI playing
            if(hitBallAgentAI != null)
            {
                hitBallAgentAI.AddReward(-1f);
            }

            outOfBounds = true;
        }
    }

    private IEnumerator Stuck()
    {
        print("Checking if stuck");

        checkingifStuck = true;

        yield return new WaitForSeconds(5);

        
        if (0 <= _velocity.y && _velocity.y < 1 || -1 < _velocity.y && _velocity.y <= 0 )
        {
            print("y is stuck");

            yStuck = true;

        }

        if (0 == _velocity.x || -0.1 <=_velocity.x && _velocity.x < 0.1)
        {
            print("x is stuck");

            xStuck = true;

        }

        checkingifStuck = false;


    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "brick")
        {
            //print("Brick hit audio should play");
            brickHitAudio.Play(); //play hit audio

            //if AI playing
            HitBallAgent hitBallAgent = FindObjectOfType<HitBallAgent>();
            if(hitBallAgent != null)
            {
                //hitBallAgent.AddReward(+1f);
            }
        }

        if (xStuck)
        {
            print("Fixed x being stuck");

            _rigidbodyComp.velocity = moveToCenter;

            xStuck = false;
        }
        else if (yStuck)
        {
            print("Fixed y being stuck");

            _rigidbodyComp.velocity = moveToCenterBottom;

            //_rigidbodyComp.velocity = new Vector3(18, -2, 0);

            yStuck = false;
        }
        else
        {
            _rigidbodyComp.velocity = Vector3.Reflect(_velocity, collision.contacts[0].normal); //reflects incoming velocity based on normal of 1st contact point
        }
    }
}
