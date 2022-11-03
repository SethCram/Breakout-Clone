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
    Transform _transform;
    private Vector3 moveToCenter;
    private Vector3 moveToCenterBottom;
    private bool checkingifStuck = false;
    private bool yStuck = false;
    private bool xStuck = false;
    private AudioSource brickHitAudio;

    Vector3 middlePoint;
    private Vector3 middlePlayer;

    //public AudioClip brickHitAudio;

    // Start is called before the first frame update
    void Start()
    {

        _rigidbodyComp = GetComponent<Rigidbody>();

        _renderer = GetComponent<Renderer>();

        brickHitAudio = GetComponent<AudioSource>();

        Invoke("Launch", 0.5f);

        middlePoint = new Vector3(0, 0, 0);

        middlePlayer = new Vector3(0, -17, 0);
    }

    private void Launch()
    {
        _rigidbodyComp.velocity = Vector3.up * _speed;
    }



    // Update is called once per frame
    void Update()
    {
        _transform = GetComponent<Transform>();

        moveToCenter = middlePoint - _transform.position;

        moveToCenterBottom = middlePlayer - transform.position;
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

        if (_renderer.isVisible == false) //if ball not on any camera's screen
        {
            GameManager.Instance.Balls--; //subtrcts 1 from amt of balls game manager has

            //if AI playing
            HitBallAgent hitBallAgent = FindObjectOfType<HitBallAgent>();
            if(hitBallAgent != null)
            {
                hitBallAgent.AddReward(-10f);
            }

            Destroy(gameObject);
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
                hitBallAgent.AddReward(+1f);
            }
        }
        else if( collision.gameObject.tag == "Player" )
        {
            //if AI playing
            HitBallAgent hitBallAgent = FindObjectOfType<HitBallAgent>();
            if(hitBallAgent != null)
            {
                hitBallAgent.AddReward(+0.1f);
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
