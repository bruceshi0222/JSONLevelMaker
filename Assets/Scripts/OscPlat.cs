using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class OscPlat : MonoBehaviour
{
    enum State
    {
        RUNNING, WAITING, COOLDOWN
    };

    [Header("General Settings")]
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
    public Transform directionObj;

    [Header("Click to refresh the end position")]

    [SerializeField]
    public bool refreshThingsPlease;


    //stuff to animate
    float timer = 0.0f;
    bool isReturning = false;
    Vector3 debugPosition = Vector3.zero;
    State state = State.RUNNING;
    float frequency = 1.0f;
    float lastTime = 0.0f;
    // Start is called before the first frame update


    [SerializeField] MeshFilter unityMeshFilter;
    Mesh unityMesh;

    public void OnValidate()
    {

        dimensions = transform.localScale; 
        position = transform.localPosition;
        ValidatePositions();
        frequency = 1.0f / timePeriod;

        if(unityMeshFilter)unityMesh = unityMeshFilter.sharedMesh;

    }

    private void Reset()
    {
        if (!directionObj)
        {
            GameObject gizmo = new GameObject("DirectionGizmo");
            gizmo.transform.parent = transform;
            gizmo.transform.localPosition = Vector3.zero;
            DirectionGizmo comp = gizmo.AddComponent<DirectionGizmo>();
            comp.parent = transform;

            directionObj = gizmo.transform;
        }
    }
    private void ValidatePositions()
    {
        try //im so sorry for this
        {
            direction = (directionObj.position - this.transform.position).normalized;
            dist = (directionObj.position - this.transform.position).magnitude;
        }
        catch(MissingReferenceException) { }

    }

    private void OnDrawGizmosSelected()
    {
        float deltaTime = Time.realtimeSinceStartup - lastTime;
        lastTime = Time.realtimeSinceStartup;
        timer += deltaTime;
        switch (state)
        {
            case State.WAITING:
                {
                    if (timer >= waitDelay)
                    {
                        float leftover = timer - waitDelay;
                        state = State.RUNNING;
                        timer = 0.5f * 1 / frequency + leftover;
                        isReturning = true;
                        UpdateOscillation(deltaTime);
                    }
                    break;
                }
            case State.COOLDOWN:
                {
                    if (timer >= cooldown)
                    {
                        float leftover = timer - cooldown;

                        state = State.RUNNING;
                        timer = 0.0f + leftover;
                        UpdateOscillation(deltaTime);
                    }
                    break;
                }
            case State.RUNNING:
                {
                    UpdateOscillation(deltaTime);
                    break;
                }

            default:
                break;
        }
        Gizmos.color = Vector4.Scale(Color.blue, new Vector4(1, 1, 1, 0.5f));

        if(!unityMesh)Gizmos.DrawCube(debugPosition, transform.localScale * 0.99f);
        else Gizmos.DrawMesh(unityMesh, debugPosition, transform.rotation, transform.localScale * 0.99f);
    }

    private void UpdateOscillation(float dt)
    {
        if (timer * frequency >= 0.5f && !isReturning)
        {
            float leftover = timer - 0.5f * 1.0f / frequency;
            timer = leftover;
            state = State.WAITING;
            return;
        }
        if (timer * frequency >= 1.0f && isReturning)
        {
            float leftover = timer - 1 / frequency;
            isReturning = false;
            timer = 0.0f + leftover;
            state = State.COOLDOWN;
            return;
        }
        float cosTimer = (-1.0f * Mathf.Cos((timer * frequency * Mathf.PI * 2)) + 1) * 0.5f; //this gets a value from 0 to 1 where 0 is the initial value 
        debugPosition = position + direction.normalized * cosTimer * dist;

    }
}
