using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMover : MonoBehaviour
{
    public float degreesPerSec = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Move scene (left) by radius positive z

        // Average helicopter speed is 295 meters/sec

        // Start moving in positive X direction
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation *= Quaternion.AngleAxis(degreesPerSec * Time.deltaTime, Vector3.down);
    }
}
