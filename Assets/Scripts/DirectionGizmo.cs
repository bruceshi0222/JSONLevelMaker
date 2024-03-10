using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionGizmo : MonoBehaviour
{
    [SerializeField] public Transform parent;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, parent.position);
    }

    private void OnValidate()
    {
        
    }
}
