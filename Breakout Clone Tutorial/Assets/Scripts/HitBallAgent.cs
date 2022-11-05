using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HitBallAgent : Agent
{
    public Transform ball { private get; set; }
    public Brick[] bricks;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        //need to give it time to allow level destruction
        GameManager.Instance.SwitchState(GameManager.State.INIT, delay: 2f);

        //spawn randomly on new episode begin
        transform.localPosition = new Vector3(Random.Range(GameManager.LOCAL_POSITION_MIN, GameManager.LOCAL_POSITION_MAX), -18, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //inputs for the AI to solve problem

        //AI player position
        sensor.AddObservation(transform.position);

        
        if( ball != null)
        {
            //should avoid destroying the ball? but would still be destroyed tween scene changes

            //ball position
           sensor.AddObservation(ball.position);

            //difference between AI player and ball pos's
            //sensor.AddObservation(transform.position - ball.position);

            //print($"Observation size = {sensor.ObservationSize()}");
        }

        //bricks positions?
        foreach (Brick brick in bricks)
        {
            if(brick != null)
            {
                //sensor.AddObservation(brick.GetComponent<Transform>().position);
            }
            
        }
        
    }

    /// <summary>
    /// Modify actions recieved by OnActionRecieved() for manual AI moving.
    /// Used if MLagents-learn not running or Behavior type set to Hueristics.
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
    }

    /// <summary>
    /// Upon action reveived, move AI player 
    ///  if they desired to move somewhere within bounds.
    /// </summary>
    /// <param name="actions"></param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        //ML alg only work with numbers
        // doesn't know ab objs or turning or moving

        //Debug.Log(actions.ContinuousActions[0]);

        float moveX = actions.ContinuousActions[0];

        float desiredX = transform.position.x + moveX;

        //if don't desire to move out of bounds
        if( !(desiredX >  GameManager.LOCAL_POSITION_MAX) &&
            !(desiredX < GameManager.LOCAL_POSITION_MIN) )
            {
                //apply motion
                _rigidbody.MovePosition(new Vector3(desiredX, GameManager.LOCAL_PLAYER_Y, 0)); 
            }
    }
}
