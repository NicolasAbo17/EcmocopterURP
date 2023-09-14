using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool isPressInteraction = false;
    public abstract void Focus(HandSelectorV2 hand, bool state);
    public abstract void FocusUpdate(HandSelectorV2 hand, bool state);
}
