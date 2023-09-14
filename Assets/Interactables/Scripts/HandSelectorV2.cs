using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class HandSelectorV2 : MonoBehaviour
{
    //The OVR script that handles hand tracking and references for the thumb and index fingers
    [Header("OVR Components")]
    public OVRSkeleton hand;
    Transform index;
    Transform thumb;

    //Reference to the eye for sighting
    [SerializeField] Transform eye;

    //The variables handle the storage of important information interactables use to make decisions
    [Header("Pose Data (Meant for other scripts to access)")]
    public float pinchDist;
    public float pressDist;
    public Vector3 thumbToIndexMidPoint; //Stores the midpoint of the thumb and index vector
    public Vector3 thumbToIndexVector;   //Stores the thumb and index vector

    //These are the components that will be used for visualizing the different aspects of the selector
    [Header("Visualization Components (Optional)")]
    [SerializeField] LineRenderer pinchLine; //Line between the thumb and index
    [SerializeField] LineRenderer pressLine; //Line towards object hit by the press function
    [SerializeField] LineRenderer pinchTargetLine; //Line towards the object hit by the pinch function
    [SerializeField] bool hasVisualization;

    //This is where the active Interactable will be stored for use 
    [SerializeField] Interactable interactable;

    //On Start the references for thumb and index are set
    private void Start()
    {
        if (hand.Bones.Count > 0)
        {
            index = hand.Bones[20].Transform;
            thumb = hand.Bones[19].Transform;
        }
    }

    //Function for sighting press interactions
    bool TryPress()
    {
        //Raycast object for finding interactables
        RaycastHit hit;

        //Interactable object that stores the found interactable
        Interactable hitObj;

        //Shoot a raycast from the eye in the direction of the tip of the index finger
        if (Physics.Raycast(eye.position, (index.position) - eye.position, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            //Check for rigidbody
            if (hit.rigidbody != null)
            {
                //Check for an interactable on the object
                if (hit.rigidbody.gameObject.GetComponent<Interactable>() != null)
                {
                    //Save the interactble into the temporary object
                    hitObj = (hit.rigidbody != null) ? hit.rigidbody.GetComponent<Interactable>() : null;

                    //Check if the object is a press interaction or not
                    if (hitObj.isPressInteraction)
                    {
                        //Give focus to the interactable
                        GiveFocus(hitObj);

                        //Save the press distance
                        Vector3 v = hand.Bones[20].Transform.position - hit.point;
                        pressDist = v.magnitude;

                        //Return true to allow the main logic to know not to proceed to the next step
                        return true;
                    }
                }
            }
        }
        //Inform the main logic that there was no interactable found
        return false;
    }

    //Function for sighting pinch interactions
    bool TryPinch()
    {
        //Raycast object for finding interactables
        RaycastHit hit;

        //Interactable object that stores the found interactable
        Interactable hitObj;

        //Find the vector between the thumb and index finger
        thumbToIndexVector = index.position - thumb.position;

        //Calculate the midpoint of the vector
        thumbToIndexMidPoint = (index.position + thumb.position) / 2;

        //Shoot a ray from the eye in the direction of the midpoint of the thumb and index
        if (Physics.Raycast(eye.position, (thumbToIndexMidPoint) - eye.position, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            //Check for an interactable on the object
            if (hit.rigidbody != null)
            {
                //Save the interactble into the temporary object
                hitObj = (hit.rigidbody != null) ? hit.rigidbody.GetComponent<Interactable>() : null;

                //Check if the object is a press interaction or not
                if (!hitObj.isPressInteraction)
                {
                    //Give focus to the interactable
                    GiveFocus(hitObj);

                    //Save the pinch distance
                    pinchDist = thumbToIndexVector.magnitude;

                    //Return true to allow the main logic to know not to proceed to the next step
                    return true;
                }
            }
        }

            return false;
    }

    //Focus Handlers
    void GiveFocus(Interactable _interactable)
    {
        //If there is no interactable being focused on do this
        if (interactable == null)
        {
            interactable = _interactable;
            interactable.Focus(this, true);
        }
        //If the interactable is different than the saved interactable swap them out
        else if (interactable != _interactable)
        {
            interactable.Focus(this, false);
            interactable = _interactable;
            interactable.Focus(this, true);
        }
        //Call focus update after the interactable has been updated
        interactable.FocusUpdate(this, true);
    }

    void DropFocus()
    {
        if (interactable != null)
        {
            //Inform the interactable that focus has been lost and set it to null
            interactable.Focus(this, false);
            interactable = null;
        }
    }

    private void Update()
    {
        if (hand.Bones.Count > 0)
        {
            index = hand.Bones[20].Transform;
            thumb = hand.Bones[19].Transform;

            //Call the different detection functions
            if (!TryPinch())
            {
                //If pinch fails to find an interactable try a press interaction instead
                if (!TryPress())
                {
                    //If neither is found focus should be dropped
                    DropFocus();
                }
            }
        }
    }
}
