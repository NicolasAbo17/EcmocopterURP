using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class XRButton : Interactable
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent<bool> { }
    [SerializeField]
    public ButtonEvent button_changed;

    [SerializeField] float pressDistance = .025f;
    [SerializeField] float min;
    [SerializeField] float max;

    bool isPressing;
    bool buttonState;
    bool canToggle = true;

    float startY;
    float currY;
    float deltaY;

    Vector3 initialPosition = Vector3.zero;
    Vector3 currentPosition = Vector3.zero;

    private void Start()
    {
        isPressInteraction = true;
        initialPosition = transform.localPosition;
        currentPosition = transform.localPosition;
        max = currentPosition.y;
    }

    Coroutine recoverRoutine = null;
    bool hasFocus = false;

    //----Is called once when the index finger points at the button----
    public override void Focus(HandSelectorV2 hand, bool state)
    {
        //When the button is focused highlight it (optional)
        hasFocus = state;
        if (state && recoverRoutine == null)
        {
            recoverRoutine = StartCoroutine(DoRecovery());
        }
    }
    //-----------------------------------------------------------------


    IEnumerator DoRecovery()
    {
        bool isRamping = true;
        while (hasFocus || isRamping)
        {

            float dy = (initialPosition.y - transform.localPosition.y) * 0.3f;
            currentPosition.y = transform.localPosition.y + dy;
            if (Mathf.Abs(currentPosition.y) < 0.002f)
            {
                transform.localPosition = initialPosition;
                isRamping = false;
                canToggle = true;
            }
            else
            {
                transform.localPosition = currentPosition;
                isRamping = true;
            }
            Debug.Log("DY:" + dy);
            yield return new WaitForSeconds(.1f);
        }

        transform.localPosition = initialPosition;
        recoverRoutine = null;
    }

    //----Is called like Update() but only when focus is recieved----
    public override void FocusUpdate(HandSelectorV2 hand, bool state)
    {
        //When the button is being pressed call the appropriate function
        if (hand.pressDist <= pressDistance)
        {
            Pressed(hand);
        }
    }
    //---------------------------------------------------------------

    //----Is called when the button is being pressed----
    public void Pressed(HandSelectorV2 hand)
    {


        //When the button is pressed the button should follow the finger similarly to the slider
        if (!isPressing) //This is where everything that happens on the initial press occurs
        {
            Debug.Log("Press started");
            //Set isPressing to TRUE
            isPressing = true;

            //Grab Index Tip position and convert it to the local frame, saving out the y to startY
            startY = transform.InverseTransformPoint(hand.hand.Bones[20].Transform.position).y;
        }
        else if (isPressing)//After the initial press everything happens in here
        {
            Debug.Log("Pressed Reached");
            //Grab Index Tip position and convert it to the local frame, saving out the y to currY
            currY = transform.InverseTransformPoint(hand.hand.Bones[20].Transform.position).y;

            //Calculate deltaY
            deltaY = currY - startY;

            //Apply the deltaY value to a new final position Vector
            Vector3 finalPos = transform.localPosition;
            finalPos.y += deltaY;

            transform.localPosition = finalPos;

            //When the button is at min it should toggle the state of the button (OR) When the button is at min it sets the value to false and when the value is at max the value is set to false
            if (finalPos.y >= min && finalPos.y < max)
            {
                transform.localPosition = finalPos;
            }
            else if (finalPos.y >= max)
            {
                finalPos.y = max;
                transform.localPosition = finalPos;

                canToggle = true;
            }
            else if (finalPos.y < min)
            {
                finalPos.y = min;
                transform.localPosition = finalPos;

                if (canToggle)
                {
                    buttonState = !buttonState;
                    button_changed.Invoke(buttonState);
                    canToggle = false;
                }
            }
        }
    }
    //--------------------------------------------------

}
