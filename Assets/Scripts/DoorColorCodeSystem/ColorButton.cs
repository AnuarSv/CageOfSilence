using UnityEngine;
using UnityEngine.EventSystems;

public class ColorButton : MonoBehaviour, IPointerDownHandler
{
    public bool Red;
    public bool Green;
    public bool Blue;
    public bool Pink;
    public bool Yellow;
    public bool Brown;
    public bool Magenta;
    public bool Cyan;
    public bool DarkGreen;
    [HideInInspector] public bool Pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Red) { Pressed = true; }
        else if (Green) { Pressed = true; }
        else if (Blue) { Pressed = true; }
        else if (Pink) { Pressed = true; }
        else if (Yellow) { Pressed = true; }
        else if (Brown) { Pressed = true; }
        else if (Magenta) { Pressed = true; }
        else if (Cyan) { Pressed = true; }
        else if (DarkGreen) { Pressed = true; }
    }
}