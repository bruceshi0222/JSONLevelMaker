using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{

    [Header("General Settings")]
    [SerializeField]
    public string mesh;
    [HideInInspector]
    public Vector3 dimensions;
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 direction;
    [SerializeField]
    public float force;
    [SerializeField]
    public float activeTime;
    [SerializeField]
    public bool isContinuous;
    [SerializeField]
    public float continuousForce;

    [SerializeField]
    public Transform directionObject;

    // Start is called before the first frame update

    private void Awake()
    {
        dimensions = transform.localScale;
        position = transform.position;
        direction = (directionObject.transform.position - transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
