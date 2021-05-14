using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// MonoBehaviour som holder styr portene.
/// </summary>
public class GateController : MonoBehaviour
{
    public Gate[] Gates;
    [SerializeField]
    [InspectorName("Target")]
    short _Target;

    /// <summary>
    /// property som indeholder logic for hvornår at der er forløbet en runde.
    /// </summary>
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

    /// <summary>
    /// event som triggers når det er at dronen har været igennem alle porte
    /// </summary>
    public UnityEvent courseDone;


    // Start is called before the first frame update
    void Awake()
    {
        //find alle gates som tilbage i spillet udover dem som kan blive indsat fra inspectoren.
        //lav et array som kun kan indeholde unike værdier engang og start udfra inspectoren.
        HashSet<Gate> children = new HashSet<Gate>(Gates);

        //tilføj de resterende porte
        children.UnionWith(transform.GetComponentsInChildren<Gate>());

        //reset portene i lageret for at kunne copier værdierne fra hashsettet til gatesne
        Gates = new Gate[children.Count];
        //kopier portene over i den rigtige port array
        children.CopyTo(Gates, 0);
        //tilføj event listener til alle gates.
        foreach (var gate in Gates)
        {
            gate.collisionEvent.AddListener(collisionWithGate);
        }

        courseDone = new UnityEvent();
    }

    /// <summary>
    /// funktion som bliver brugt til port collision events fra gate classen.
    /// </summary>
    /// <param name="other">den collider som tilhøre porten</param>
    /// <param name="hitGate">objectet som tilhøre porten</param>
    private void collisionWithGate(Collider other, Gate hitGate)
    {
        //tjek om porten er i gate array'et og om det er den næste port som dronen skal ramme
        if (Array.IndexOf(Gates, hitGate) == Target)
        {
            //tjek om dronen har ramt den det rigtige sted.
            if (hitGate.checkEntryAngle(other.attachedRigidbody.velocity))
            {
                Target++;//set målet til næste port.
                Debug.Log("valid hit");
            }
        }
    }
}
