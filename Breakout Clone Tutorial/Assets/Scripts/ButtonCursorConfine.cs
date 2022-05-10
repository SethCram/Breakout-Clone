using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCursorConfine : Button, IPointerDownHandler
{
    public override void OnPointerDown(PointerEventData eventData) // required interface when using the OnPointerDown method.
    {
        print("Cursor should be confined to game");

        //confine cursor to game window
        Cursor.lockState = CursorLockMode.Confined;

        base.OnPointerDown(eventData);

        //could also fullscreen here if desired
    }

}
