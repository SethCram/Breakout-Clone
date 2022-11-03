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

    public override void OnEpisodeBegin()
    {
        //need to give it time to allow level destruction
        GameManager.Instance.SwitchState(GameManager.State.INIT, delay: 2f);

        //spawn randomly on new episode begin
        transform.localPosition = new Vector3(Random.Range(-32.6f, 32.6f), -18, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //inputs for the AI to solve problem

        //AI player position
        sensor.AddObservation(transform.position);

        
        if( ball != null)
        {
            //ball position
           sensor.AddObservation(ball.position);

            //difference between AI player and ball pos's
            sensor.AddObservation(transform.position - ball.position);
        }

        //bricks positions?
        foreach (Brick brick in bricks)
        {
            if(brick != null)
            {
                sensor.AddObservation(brick.GetComponent<Transform>().position);
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

        continuousActions[0] = Input.GetAxisRaw("Horizontal") * 10;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //ML alg only work with numbers
        // doesn't know ab objs or turning or moving

        //Debug.Log(actions.ContinuousActions[0]);

        float moveX = actions.ContinuousActions[0];

        float moveSpeed = 10f;

        transform.position += new Vector3(moveX, 0, 0) * Time.deltaTime * moveSpeed;
    }
}
