using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    [SerializeField]
    float time = 0;

    // Update is called once per frame
    void Update()
    {
        time += 10 * Time.deltaTime;

        gameObject.transform.Rotate(new Vector3(0, 0, time));
        //transform.rotation.eulerAngles =;
    }
}
