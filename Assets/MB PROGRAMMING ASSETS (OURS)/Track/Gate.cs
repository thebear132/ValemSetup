using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gate : MonoBehaviour
{
    public float planeAngle = 1.58f;
    public float VerticalAngle = 1.58f;

    public bool checkEntryAngle(Vector3 velocity) {
        Vector3 normie = velocity.normalized;
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

        collisionEvent.Invoke(other, this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + RotationalUtils.convertRadialToPos(planeAngle, VerticalAngle, 2));
    }
}

public static class RotationalUtils
{
    public static Vector3 convertRadialToPos(float horizontal, float vertical, float radius)
    {
        float x = Mathf.Sin(vertical) * Mathf.Cos(horizontal),
            y = Mathf.Cos(vertical),
            z = Mathf.Sin(vertical) * Mathf.Sin(horizontal);
        return new Vector3(x, y, z) * radius;
    }
}

class CollisionEvent : UnityEvent<Collider, Gate>{

}
