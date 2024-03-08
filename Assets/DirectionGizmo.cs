using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionGizmo : MonoBehaviour
{
    [SerializeField] Transform parent;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, parent.position);
    }
}
