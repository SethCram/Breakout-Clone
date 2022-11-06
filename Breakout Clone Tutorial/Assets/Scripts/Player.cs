using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //move paddle to mouse's x position, but hug edges if too far one way

        float mouseLocationX = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, 50f)).x;

        //error checking: print("Mouse position's x coord: " + mouseLocationX);

        if ( mouseLocationX > BreakoutConstants.LOCAL_POSITION_MAX)
        {
            _rigidbody.MovePosition(new Vector3( BreakoutConstants.LOCAL_POSITION_MAX, BreakoutConstants.LOCAL_PLAYER_Y, 0));
        }
        else if (mouseLocationX < BreakoutConstants.LOCAL_POSITION_MIN) //x used to be '-32.3f'
        {
            _rigidbody.MovePosition(new Vector3( BreakoutConstants.LOCAL_POSITION_MIN, BreakoutConstants.LOCAL_PLAYER_Y, 0)); //x used to be '-32.3f'
        }
        else
        {
            _rigidbody.MovePosition(new Vector3(mouseLocationX, BreakoutConstants.LOCAL_PLAYER_Y, 0));

            //old code: _rigidbody.MovePosition(new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, 50f)).x, -17, 0)); //must convert Screen to World Point 
        }
    }
}
