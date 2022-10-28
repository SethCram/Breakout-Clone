using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class HitBallAgent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)
    {
        //ML alg only work with numbers
        // doesn't know ab objs or turning or moving

        Debug.Log(actions.DiscreteActions[0]);
    }
}
