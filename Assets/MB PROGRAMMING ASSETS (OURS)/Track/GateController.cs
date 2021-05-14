using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class GateController : MonoBehaviour
{
    public Gate[] Gates;
    [SerializeField]
    [InspectorName("Target")]
    short _Target;

    short Target
    {
        get
        {
            return _Target;
        }
        set
        {
            if (value == Gates.Length) courseDone.Invoke();
            _Target = value;
        }
    }

    public UnityEvent courseDone;


    // Start is called before the first frame update
    void Awake()
    {
        HashSet<Gate> children = new HashSet<Gate>(Gates);

        children.UnionWith(transform.GetComponentsInChildren<Gate>());

        Gates = new Gate[children.Count];

        children.CopyTo(Gates, 0);

        foreach (var gate in Gates)
        {
            gate.collisionEvent.AddListener(collisionWithGate);
        }

        courseDone = new UnityEvent();
    }

    private void collisionWithGate(Collider other, Gate hitGate)
    {
        if (Array.IndexOf(Gates, hitGate) == Target)
        {
            if (hitGate.checkEntryAngle(other.attachedRigidbody.velocity))
            {
                Target++;
                Debug.Log("valid hit");
            }
        }
    }
}
