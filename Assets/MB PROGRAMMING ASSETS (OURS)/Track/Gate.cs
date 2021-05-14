using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// MonoBehaviour som holder styr på en port.
/// </summary>
public class Gate : MonoBehaviour
{
    public float planeAngle = 1.58f;
    public float VerticalAngle = 1.58f;

    public bool checkEntryAngle(Vector3 velocity) {
        Vector3 normie = velocity.normalized;
        //tjek om skalarproduktet er større end 0 fordi at så er retningen den rigtige som er difinineret af to vinkler.
        return Vector3.Dot(normie, RotationalUtils.convertRadialToPos(planeAngle, VerticalAngle, 1))>0;
    }

    public UnityEvent<Collider, Gate> collisionEvent;

    void Awake()
    {
        if (collisionEvent == null) collisionEvent = new CollisionEvent();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        //kør eventet
        collisionEvent.Invoke(other, this);
    }

    void OnDrawGizmos()
    {
        //tegn en blå linje som ilusstere hvilken retning dronen skal ramme den på.
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + RotationalUtils.convertRadialToPos(planeAngle, VerticalAngle, 2));
    }
}

/// <summary>
/// ultility classse som indeholder forhold mellem radialt kordinatsystem. 
/// </summary>
public static class RotationalUtils
{
    /// <summary>
    /// funktion som konvertere vinkler og en længde til en vektor
    /// </summary>
    /// <param name="horizontal">den horizontalle vinkel som definere vektoren</param>
    /// <param name="vertical">den horizontalle vinkel som definere vektoren</param>
    /// <param name="radius">længden af vektoren</param>
    /// <returns></returns>
    public static Vector3 convertRadialToPos(float horizontal, float vertical, float radius)
    {
        float x = Mathf.Sin(vertical) * Mathf.Cos(horizontal),
            y = Mathf.Cos(vertical),
            z = Mathf.Sin(vertical) * Mathf.Sin(horizontal);
        return new Vector3(x, y, z) * radius;
    }
}
/// <summary>
/// dummy class for at kunne lave et event ved custom inputs.
/// </summary>
class CollisionEvent : UnityEvent<Collider, Gate>{

}
