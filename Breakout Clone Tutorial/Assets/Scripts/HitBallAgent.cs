using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HitBallAgent : Agent
{
    public Transform ball { private get; set; }
    //[SerializeField] private Transform[] bricks;

    public override void OnEpisodeBegin()
    {
        GameManager.Instance.SwitchState(GameManager.State.INIT);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //inputs for the AI to solve problem

        //AI player position
        sensor.AddObservation(transform.position);

        //ball position
        if( ball != null)
        {
           sensor.AddObservation(ball.position); 
        }

        //bricks positions?
        foreach (Brick brick in FindObjectsOfType<Brick>())
        {
            sensor.AddObservation(brick.GetComponent<Transform>().position);
        }
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //ML alg only work with numbers
        // doesn't know ab objs or turning or moving

        //Debug.Log(actions.ContinuousActions[0]);

        float moveX = actions.ContinuousActions[0];

        float moveSpeed = 1f;

        transform.position += new Vector3(moveX, 0, 0) * Time.deltaTime * moveSpeed;
    }
}
