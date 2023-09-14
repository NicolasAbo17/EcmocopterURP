using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class XRDial : Interactable
{
    [System.Serializable]
    public class DialEvent : UnityEvent<float> { }
    [SerializeField]
    private DialEvent dial_changed;

    [SerializeField] float actionDist;

    bool firstFrame = true;

    Vector3 initialGripPos;
    Vector3 currGripPos;

    //Is called once when the interactable is initially between the thumb and index finger
    public override void Focus(HandSelectorV2 hand, bool state)
    {
        if  (!state) firstFrame = true;
    }

    public override void FocusUpdate(HandSelectorV2 hand, bool state)
    {
        if (isGripped(hand))
        {
            if (firstFrame)
            {
                //Convert the mid point between the two fingers to the local frame
                initialGripPos = transform.InverseTransformPoint(hand.hand.Bones[20].Transform.position);

                //Clear out the y value from the vector as it is not needed
                initialGripPos.y = 0;

                //Mark the first frame as complete
                firstFrame = false;

            }
            else
            {
                //Convert the current mid point into the local frame
                currGripPos = transform.InverseTransformPoint(hand.hand.Bones[20].Transform.position);

                //Clear out the y value again
                currGripPos.y = 0;

                //Calculate the change in rotation around the spin axis
                transform.localRotation *= Quaternion.FromToRotation(initialGripPos, currGripPos);

                //Calculate the dial value from 0f to 360f
                Vector3 vec = transform.localRotation * Vector3.forward;
                float rads = Mathf.Acos(Vector3.Dot(vec.normalized, Vector3.forward));
                float degs = rads * (180f / Mathf.PI);

                if (vec.x < 0)
                {
                    degs = (360 - degs);
                }

                //Invoke the event passing along the 0f to 360f float value
                dial_changed.Invoke(degs);
                //Debug.Log(degs);
            }
        }
        else
        {
            if (!firstFrame) firstFrame = true;
        }
        
    }

    //Checks if the object is currently being gripped
    bool isGripped(HandSelectorV2 hand)
    {
        if (hand.pressDist <= actionDist)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
