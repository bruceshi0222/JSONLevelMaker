using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscPlat : MonoBehaviour
{

    [Header("General Settings")]
    [SerializeField]
    public string mesh;
    [HideInInspector]
    public Vector3 dimensions;
    [HideInInspector]
    public Vector3 position;
    [SerializeField]
    public float timePeriod;
    [HideInInspector]
    public float dist;
    [HideInInspector]
    public Vector3 direction;
    [SerializeField]
    public float cooldown;
    [SerializeField]
    public float waitDelay;

    [Header("The end position of the oscillator")]
    [SerializeField]
    public GameObject directionObj;
    
    // Start is called before the first frame update

    private void Awake()
    {
        dimensions = transform.localScale;
        position = transform.localPosition;
        direction = (directionObj.transform.position - this.transform.position).normalized;
        dist = (directionObj.transform.position - this.transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
