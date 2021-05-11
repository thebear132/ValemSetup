using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 1000;


    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime)); //Applier en rotation til rotoreren
    }
    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }

}
