using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector] public bool doOnce = false;
    public bool Button1;
    [HideInInspector] public bool PressedButton1;
    public bool Button2;
    [HideInInspector] public bool PressedButton2;
    public bool Button3;
    [HideInInspector] public bool PressedButton3;
    public bool Button4;
    [HideInInspector] public bool PressedButton4;
    public bool Button5;
    [HideInInspector] public bool PressedButton5;
    public bool Button6;
    [HideInInspector] public bool PressedButton6;
    public bool Button7;
    [HideInInspector] public bool PressedButton7;
    public bool Button8;
    [HideInInspector] public bool PressedButton8;
    public bool Button9;
    [HideInInspector] public bool PressedButton9;
    public bool Button0;
    [HideInInspector] public bool PressedButton0;
    public bool ButtonReset;
    [HideInInspector] public bool PressedButtonReset;
    public bool ButtonEnter;
    [HideInInspector] public bool PressedButtonEnter;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Button1) { PressedButton1 = true; }
        else if (Button2) { PressedButton2 = true; }
        else if (Button3) { PressedButton3 = true; }
        else if (Button4) { PressedButton4 = true; }
        else if (Button5) { PressedButton5 = true; }
        else if (Button6) { PressedButton6 = true; }
        else if (Button7) { PressedButton7 = true; }
        else if (Button8) { PressedButton8 = true; }
        else if (Button9) { PressedButton9 = true; }
        else if (Button0) { PressedButton0 = true; }
        else if (ButtonReset) { PressedButtonReset = true; }
        else if (ButtonEnter) { PressedButtonEnter = true; }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Button1) { PressedButton1 = false; }
        else if (Button2) { PressedButton2 = false; }
        else if (Button3) { PressedButton3 = false; }
        else if (Button4) { PressedButton4 = false; }
        else if (Button5) { PressedButton5 = false; }
        else if (Button6) { PressedButton6 = false; }
        else if (Button7) { PressedButton7 = false; }
        else if (Button8) { PressedButton8 = false; }
        else if (Button9) { PressedButton9 = false; }
        else if (Button0) { PressedButton0 = false; }
        else if (ButtonReset) { PressedButtonReset = false; }
        else if (ButtonEnter) { PressedButtonEnter = false; }
    }
}