using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnchorPlacement : MonoBehaviour
{
    [SerializeField] Transform reference;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = reference.position;

        newPosition.y = 0;

        transform.position = newPosition;
    }

}
