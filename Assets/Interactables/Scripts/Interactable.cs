using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool isPressInteraction = false;
    public abstract void Focus(HandSelector hand, bool state);
    public abstract void FocusUpdate(HandSelector hand, bool state);
}
