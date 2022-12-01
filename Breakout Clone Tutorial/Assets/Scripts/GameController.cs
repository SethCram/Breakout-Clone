using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject GameEnviron;

    public float xSeperatedBy = 100;

    public int trainingEnvirons = 50;

    public bool training = true;

    // Start is called before the first frame update
    void Start()
    {
        //if training
        if( training == true )
        {
            float xDistanceFromZero = xSeperatedBy;
            //create training environs
            for (int i = 0; i < trainingEnvirons; i++)
            {
                Instantiate(GameEnviron, transform.position + Vector3.right * xDistanceFromZero, Quaternion.identity);
                xDistanceFromZero += xSeperatedBy;
            }
        }
    }
}
