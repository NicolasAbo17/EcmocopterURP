using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialRatchet : MonoBehaviour
{

    float input;

    [System.Serializable]
    public class RatchetEvent : UnityEvent<int> { }
    [SerializeField]
    public RatchetEvent state_changed;

    [SerializeField] List<float> angles;

    int lastState;

    public void GetInput(float _input)
    {
        input = _input;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool IsCloserToZero()
    {
        float absDiff360 = Mathf.Abs(360f - input);
        float absDiffLastAngle = Mathf.Abs(angles[angles.Count - 1] - input);
        //Debug.Log("Diff360: " + absDiff360 +" DiffLast: " + absDiffLastAngle);
        if (absDiff360 < absDiffLastAngle)
            return true;
        else
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(input);

        for (int i = 0; i < angles.Count; i++)
        {
            //Special Case: First angle
            if (i == 0)
            {
                //Debug.Log(IsCloserToZero());
                if (input >= angles[i] && input < angles[i + 1])
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, angles[i], transform.localRotation.eulerAngles.z);
                    if (lastState != i) state_changed.Invoke(i);
                    lastState = i;
                }
                else if (IsCloserToZero())
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, angles[i], transform.localRotation.eulerAngles.z);
                    if (lastState != i) state_changed.Invoke(i);
                    lastState = i;
                    return;
                }
            }
            //Special Case: Last angle
            else if (i == angles.Count - 1)
            {
                if (input >= angles[i])
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, angles[i], transform.localRotation.eulerAngles.z);
                    if (lastState != i) state_changed.Invoke(i);
                    lastState = i;
                }
            }
            else if (i < angles.Count - 1)
            {
                if (input >= angles[i] && input < angles[i + 1])
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, angles[i], transform.localRotation.eulerAngles.z);
                    if (lastState != i) state_changed.Invoke(i);
                    lastState = i;
                }
            }        
        }
    }
}
