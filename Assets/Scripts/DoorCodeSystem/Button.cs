using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector] public bool doOnce = false;
    public bool Button1;
    public bool Button2;
    public bool Button3;
    public bool Button4;
    public bool Button5;
    public bool Button6;
    public bool Button7;
    public bool Button8;
    public bool Button9;
    public bool Button0;
    public bool ButtonReset;
    public bool ButtonEnter;
    [HideInInspector] public bool Pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Button1) { Pressed = true; }
        else if (Button2) { Pressed = true; }
        else if (Button3) { Pressed = true; }
        else if (Button4) { Pressed = true; }
        else if (Button5) { Pressed = true; }
        else if (Button6) { Pressed = true; }
        else if (Button7) { Pressed = true; }
        else if (Button8) { Pressed = true; }
        else if (Button9) { Pressed = true; }
        else if (Button0) { Pressed = true; }
        else if (ButtonReset) { Pressed = true; }
        else if (ButtonEnter) { Pressed = true; }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Button1) { Pressed = false; }
        else if (Button2) { Pressed = false; }
        else if (Button3) { Pressed = false; }
        else if (Button4) { Pressed = false; }
        else if (Button5) { Pressed = false; }
        else if (Button6) { Pressed = false; }
        else if (Button7) { Pressed = false; }
        else if (Button8) { Pressed = false; }
        else if (Button9) { Pressed = false; }
        else if (Button0) { Pressed = false; }
        else if (ButtonReset) { Pressed = false; }
        else if (ButtonEnter) { Pressed = false; }
    }
}