using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscPlat : MonoBehaviour
{

    [Header("General Settings")]
    [SerializeField]
    public string mesh;
    [SerializeField]
    public Vector3 dimensions;
    [SerializeField]
    public Vector3 position;
    [SerializeField]
    public float timePeriod;
    [SerializeField]
    public float dist;
    [SerializeField]
    public Vector3 direction;
    [SerializeField]
    public float cooldown;
    [SerializeField]
    public float waitDelay;

    [Header("Set this object if you dont know the direction")]
    [SerializeField]
    public GameObject directionObj;
    
    // Start is called before the first frame update
    void Start()
    {
        if(direction == Vector3.zero){
            direction = directionObj.transform.position - this.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
