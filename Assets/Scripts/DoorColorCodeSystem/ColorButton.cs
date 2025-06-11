using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColorButton : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector] public bool doOnce = false;
    public bool Red;
    [HideInInspector] public bool PressedRed;
    public bool Green;
    [HideInInspector] public bool PressedGreen;
    public bool Blue;
    [HideInInspector] public bool PressedBlue;
    public bool Pink;
    [HideInInspector] public bool PressedPink;
    public bool Yellow;
    [HideInInspector] public bool PressedYellow;
    public bool Brown;
    [HideInInspector] public bool PressedBrown;
    public bool Magenta;
    [HideInInspector] public bool PressedMagenta;
    public bool Cyan;
    [HideInInspector] public bool PressedCyan;
    public bool DarkGreen;
    [HideInInspector] public bool PressedDarkGreen;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Red) { PressedRed = true; }
        else if (Green) { PressedGreen = true; }
        else if (Blue) { PressedBlue = true; }
        else if (Pink) { PressedPink = true; }
        else if (Yellow) { PressedYellow = true; }
        else if (Brown) { PressedBrown = true; }
        else if (Magenta) { PressedMagenta = true; }
        else if (Cyan) { PressedCyan = true; }
        else if (DarkGreen) { PressedDarkGreen = true; }
    }
}