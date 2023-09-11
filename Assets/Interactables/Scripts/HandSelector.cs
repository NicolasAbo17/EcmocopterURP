using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class HandSelector : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] LineRenderer pointerLine;
    public OVRSkeleton hand;
    [SerializeField] float actionDist = .05f;
    [SerializeField] float pokeDist = .025f;
    [SerializeField] bool hasVisualization = true;

    public float pressDist;
    public float pinchDist;

    public Vector3 thumbToIndexMidPoint;

    public Interactable interactable;

    //----Eye Hand Stuff----
    [SerializeField] Transform eye;

    float eyeToHand;
    float eyeToHit;

    Vector3 eyeToHandVec;
    Vector3 eyeToHandCurrVec;
    Vector3 eyeToHitVec;

    float ratio;
    //----------------------

    int handedness = 1;


    private void Start()
    {
        if (hand.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft)
        {
            handedness = -1;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (hand.Bones.Count > 0)
        {
            RaycastHit hit;

            //----Thumb to Finger----

            //Creates a vector (vec) between the thumb and index fingers
            Vector3 vec = hand.Bones[20].Transform.position - hand.Bones[19].Transform.position;
            pinchDist = vec.magnitude;

            //Calculate the mid point between the two fingers
            thumbToIndexMidPoint = (hand.Bones[20].Transform.position + hand.Bones[19].Transform.position) / 2;

            //Check if there's an interactable between the thumb and index finger
            if (Physics.Raycast(hand.Bones[20].Transform.position, (hand.Bones[19].Transform.position - hand.Bones[20].Transform.position).normalized, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.rigidbody != null)
                {
                    if (hit.rigidbody.gameObject.GetComponent<Interactable>() != null)
                    {
                        Interactable pinchobj = hit.rigidbody.gameObject.GetComponent<Interactable>();

                        //If the interactable is not a press interaction 
                        if (!pinchobj.isPressInteraction)
                        {
                            if (interactable == null)
                            {
                                interactable = pinchobj;
                                interactable.Focus(this, true);
                            }
                            else if (interactable != pinchobj)
                            {
                                interactable.Focus(this, false);
                                interactable = pinchobj;
                                interactable.Focus(this, true);
                            }

                            interactable.FocusUpdate(this, true);
                        }
                    }
                    else
                    {
                        if (interactable != null)
                        {
                            interactable.Focus(this, false);
                            interactable = null;
                        }

                    }
                }
            }

            if (hasVisualization) //Handles the visualization of the vectors used
            {
                line.SetPosition(0, hand.Bones[20].Transform.position);
                line.SetPosition(1, hand.Bones[19].Transform.position);

                if (pinchDist <= actionDist)
                {
                    line.startColor = Color.red;
                    line.endColor = Color.red;
                }
                else
                {
                    line.startColor = Color.green;
                    line.endColor = Color.green;
                }
            }
            //-----------------------

            //----Finger to World----
             if (hasVisualization) pointerLine.SetPosition(0, hand.Bones[20].Transform.position);

            if (Physics.Raycast(eye.position, (hand.Bones[20].Transform.position) - eye.position, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (hasVisualization) pointerLine.SetPosition(1, hit.point);
                Interactable hitobj = (hit.rigidbody != null ) ? hit.rigidbody.GetComponent<Interactable>() : null;

                //if the hit object is an Interactable then call focus update on the object
                if ( hitobj != null )
                {
                    if (hitobj.isPressInteraction)
                    { 
                        if (interactable == null)
                        {
                            interactable = hitobj;
                            interactable.Focus(this, true);
                        }
                        else if (interactable != hitobj)
                        {
                            interactable.Focus(this, false);
                            interactable = hitobj;
                            interactable.Focus(this, true);
                        }


                        Vector3 v = hand.Bones[20].Transform.position - hit.point;
                        pressDist = v.magnitude;
                        hitobj.FocusUpdate(this, true);

                        if (hasVisualization)
                        {
                            pointerLine.startColor = Color.green;
                            pointerLine.endColor = Color.green;
                        }
                    }
                }

                else
                {
                    if (interactable != null)
                    {
                        interactable.Focus(this, false);
                        interactable = null;
                    }

                    if (hasVisualization)
                    {
                        pointerLine.startColor = Color.blue;
                        pointerLine.endColor = Color.blue;
                    }
                }
               
            }
            else
            {
                if (hasVisualization) pointerLine.SetPosition(1, hand.Bones[20].Transform.position);
            }

            //-----------------------

        }
    }
}
