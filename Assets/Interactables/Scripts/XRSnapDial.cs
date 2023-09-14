using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class XRSnapDial : Interactable
{
    //An event must be provided to allow for the notification of state change
    [System.Serializable]
    public class SnapDialEvent : UnityEvent<int> { }
    [SerializeField] private SnapDialEvent state_changed;

    //The visual object that changes rotation - aka the "Dial"
    [SerializeField] GameObject visual;

    //An int that is used to hold the current state
    int State;

    //A float that determines the distance required to initiate a grab
    [SerializeField] float actionDist;

    //Two vectors are provided to store positions: V0 which is the initial position & V1 which is the current position
    Vector3 V0;
    Vector3 V1;

    //A list must be provided of the desired snap angles
    [SerializeField] List<float> angles;

    //A private bool that determines if the dial has been grabbed & a bool to determine if we're still in the initial frame or not
    bool firstFrame;
    bool isGrabbed;

    //This mostly just checks for a state change 
    public override void Focus(HandSelectorV2 hand, bool state)
    {
        //Determine if it's first frame
        if (!state && !firstFrame) firstFrame = true;

    }

    public override void FocusUpdate(HandSelectorV2 hand, bool state)
    {
    //----Handles the different calculations for V0 & V1 based on the type of input----
        //Check for switch flick
        if (isPressInteraction)
        {
            if (hand.pressDist <= actionDist)
            {
                //If first frame capture the initial position of the finger tip
                if (firstFrame)
                {
                    isGrabbed = true; //Set a bool to acknowledge that the object is being grabbed
                    V0 = transform.InverseTransformPoint(hand.hand.Bones[20].Transform.position);
                    V0.y = 0; //Clear out the y value as it is not necessary 
                    firstFrame = false;

                    return;
                }
                //Else capture the current position of the finger tip
                else
                {
                    V1 = transform.InverseTransformPoint(hand.hand.Bones[20].Transform.position);
                    V1.y = 0; //Clear out the y value as it is not necessary 
                }
            }
            else isGrabbed = false;
        }
        //Check for pinch
        else
        {
            if (hand.pinchDist <= actionDist)
            {
                //If first frame capture the initial position of the hand
                if (firstFrame)
                {
                    isGrabbed = true; //Set a bool to acknowledge that the object is being grabbed
                    V0 = transform.InverseTransformDirection(hand.thumbToIndexVector);
                    V0.y = 0; //Clear out the y value as it is not necessary 
                    firstFrame = false;

                    return;
                }
                //Else capture the current position of the hand
                else
                {
                    V1 = transform.InverseTransformDirection(hand.thumbToIndexVector);
                    V1.y = 0; //Clear out the y value as it is not necessary 
                }

            }
            else isGrabbed = false;

        }
    //---------------------------------------------------------------------------------

    //----This is the logic for the behaviour relating to an object getting grabbed----
        if (isGrabbed)
        {
            //Calculate the cross and dot product of V0 & V1
            Vector3 cross = Vector3.Cross(V0, V1);
            float dot = Vector3.Dot(V0.normalized, V1.normalized);

            //Variables that will be used for the state changing logic
            int previous;
            int next;

            //Logic for determining the previous and next states
            if (State == 0) //Initial State
            {
                next = 1;
                previous = angles.Count - 1;
            }
            else if (State == angles.Count - 1) //Final State
            {
                next = 0;
                previous = State - 1;
            }
            else //Middle States
            {
                next = State + 1;
                previous = State - 1;
            }
            //Determine the direction based on the sign of the cross product and do the appropriate calculations
            if (cross.y < 0f) //Handling the previous state
            {
                //Determine the difference between the current angle (State) and the previous angle (previous)
                float diff = AngleDifference(angles[State], angles[previous]);

                //Check if the current dot product is less than or equal to the difference
                if (dot <= diff)
                {
                    //Transition to next state
                    ChangeState(previous);
                    //Inform the listeners of the state change
                    state_changed.Invoke(State);
                    //Reset the initial position of the finger tip or controller
                    CauseReset(hand);
                }

            }
            else //Handling the next state
            {
                //Determine the difference between the current angle (State) and the next angle (next)
                float diff = AngleDifference(angles[State], angles[next]);

                //Check if the current dot product is less than or equal to the difference
                if (dot <= diff)
                {
                    //Transition to next state
                    ChangeState(next);
                    //Inform the listeners of the state change
                    state_changed.Invoke(State);
                    //Reset the initial position of the finger tip or controller
                    CauseReset(hand);
                }
            }
        }
    //---------------------------------------------------------------------------------
    }

//----Helper Functions----
    //This is a reset function for handling the event of a reset when turning is currently happening
    void CauseReset(HandSelectorV2 hand)
    {
        if (isPressInteraction)
        {
            if (hand.pressDist <= actionDist)
            {
                //Capture the initial position of the finger tip
                V0 = transform.InverseTransformPoint(hand.hand.Bones[20].Transform.position);
                V0.y = 0; //Clear out the y value as it is not necessary 
            }
        }
        //Check for pinch
        else
        {
            //Capture the initial position of the hand
            V0 = transform.InverseTransformPoint(hand.transform.position);
            V0.y = 0; //Clear out the y value as it is not necessary 

        }

    }

    //This a state change function that can be called from outside sources
    public void ChangeState(int state)
    {
        //Set the current state
        State = state;
        //Rotate the dial visual
        visual.transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, angles[state], transform.localRotation.eulerAngles.z);

    }

    //Get the difference of two angles to determine the necessary tolerance for a state transition
    public float AngleDifference(float a1, float a2)
    {
        float result;

        if (a1 > a2)
        {
            result = Mathf.Cos(a1 - a2);
        }
        else
        {
            result = Mathf.Cos(a2 - a1);
        }

        return result;
    }
//------------------------
}
