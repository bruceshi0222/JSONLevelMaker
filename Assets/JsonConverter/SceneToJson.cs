using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


/// <summary>
/// Add Cubes to the GroundRoot object and set StartPoint EndPOint's transform. 
/// dont change names in the inspector, if you can thanks. 
/// One cube = 1 meter so scale is treated like that :]
/// </summary>


public class SceneToJson : MonoBehaviour
{
    [SerializeField] string levelName;
    [Serializable]
    class GameObjectPrimitive { 
        public GameObjectPrimitive() { }
        public GameObjectPrimitive(string pMesh, Vector3 dims, Quaternion rot, Vector3 pos, float invMass, 
                                string phType,  string volType, Vector3 colExt, float colRadius, bool shouldNet){
            mesh =  pMesh;
            dimensions = dims;
            rotation = rot;
            position = pos;
            inverseMass = invMass;
            physicType = phType;
            colliderExtents = colExt;
            colliderRadius = colRadius;
            position   = pos;
            shouldNetwork = false;
        }

        public string mesh;
        public Vector3 dimensions;
        public Quaternion rotation;
        public Vector3 position;
        public float inverseMass;
        public string physicType;
        public string volumeType;
        public Vector3 colliderExtents;
        public float colliderRadius;
        public bool shouldNetwork;
    
    }

    [Serializable]
    class Oscillaters : GameObjectPrimitive {
        public Oscillaters(string pMesh, Vector3 dims, Quaternion rot, Vector3 pos, string phType, string volType, Vector3 colExt, float colRadius, float tPeriod, float pDist, Vector3 pDir, float pcooldown, float pwait)
        {
            mesh = pMesh;
            dimensions = dims;
            rotation = rot;
            position = pos;
            inverseMass = 0.0f;
            physicType = phType;
            colliderExtents = colExt;
            colliderRadius = colRadius;
            position = pos;
            shouldNetwork = true;
            timePeriod = tPeriod;
            dist = pDist;
            direction = pDir;
            cooldown = pcooldown;
            waitDelay = pwait;
        }
        public float timePeriod;
        public float dist;
        public Vector3 direction;
        public float cooldown;
        public float waitDelay;
        
    }

    [Serializable]
    class SpringInfo : GameObjectPrimitive
    {
        public SpringInfo(string pMesh, Vector3 dims, Quaternion rot, Vector3 pos, string phType, string volType, Vector3 colExt, float colRadius,
            Vector3 dir, float f, float activeT, bool continuous, float continuousF)
        {
            mesh = pMesh;
            dimensions = dims;
            rotation = rot;
            position = pos;
            inverseMass = 0.0f;
            physicType = phType;
            colliderExtents = colExt;
            colliderRadius = colRadius;
            position = pos;
            shouldNetwork = false;
            direction = dir;
            force = f;
            activeTime = activeT;
            isContinuous = continuous;
            continuousForce = continuousF;

        }
        public Vector3 direction;
        public float force;
        public float activeTime;
        public bool isContinuous;
        public float continuousForce;

    }

    [Serializable]
    class LightInfo
    {
        public Vector3 position;
        public Vector4 colour;
        public float radius;
    }

    [Serializable]
    class Stage {
        public Stage(Vector3 StartPos, Vector3 EndPos, Vector3 DeathPlane){
            StartPoint =        StartPos;
            EndPoint =          EndPos;
            this.DeathPlane =        DeathPlane;

            primitiveGameObject =   new List<GameObjectPrimitive>();
            oscList =               new List<Oscillaters>();
            harmOscList =               new List<Oscillaters>();
            checkPoints =           new List<Vector3>();
            pointLights = new List<LightInfo>();
            springs = new List<SpringInfo>();
        }
        public int getListCount(){
            return primitiveGameObject.Count;
        }
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public Vector3 DeathPlane;
        public List<Vector3> checkPoints;
        public List<GameObjectPrimitive> primitiveGameObject;
        public List<Oscillaters> oscList;
        public List<Oscillaters> harmOscList;
        public List<LightInfo> pointLights;
        public List<SpringInfo> springs;
        
    }

    Stage level;

    private void Start(){

        Debug.Log("WORKING!!!");
        GameObject GroundR  = GameObject.Find("GroundRoot");
        GameObject Start    = GameObject.Find("StartPoint");
        GameObject End      = GameObject.Find("EndPoint");
        GameObject OscR     = GameObject.Find("OscillatingPlatforms");
        GameObject CPR      = GameObject.Find("Checkpoints");
        GameObject DP       = GameObject.Find("DeathPlane");
        GameObject HarmOscR       = GameObject.Find("HarmfulOscillators");
        GameObject LightR       = GameObject.Find("Lights");
        GameObject SpringR       = GameObject.Find("Springs");

        if(GroundR == null || Start == null || End == null || OscR == null || CPR == null || DP == null || HarmOscR == null || LightR == null || SpringR == null)
        {
            Debug.LogError("No essestial objects. Check for ground, start, or end");
            return;
        }

        level = new Stage(Start.transform.position, End.transform.position, DP.transform.position);

        CreateGroundObjects     (GroundR.transform);
        CreateOscillatorObjects (OscR.transform);
        CreateHarmfulOscillatorObjects(HarmOscR.transform);
        CreateCheckPoints(CPR.transform);
        CreateLights(LightR.transform);
        CreateSprings(SpringR.transform);

        Debug.Log("Loaded!");
        string json = JsonUtility.ToJson(level);
        WriteJson(json);

    }


    private void WriteJson(string json){
        File.WriteAllText(Application.dataPath + "/" + levelName + ".json", json);
    }

    //iterate through all kdis of groundroot and add to level
    private void CreateGroundObjects(Transform GroundRoot){     
        if(GroundRoot.childCount ==0){
            Debug.LogError("No ground Objects!");
            return;
        }

        foreach(Transform child in GroundRoot){

            // Debug.Log(child.GetComponent<MeshFilter>().sharedMesh.name);
            GameObjectPrimitive temp = new GameObjectPrimitive(
            GetMeshName(child.gameObject),
            child.transform.localScale, 
            child.transform.rotation, 
            child.transform.position,
            child.GetComponent<Rigidbody>().mass, child.tag, child.GetComponent<Collider>().GetType().ToString(), new Vector3(0,0,0), 0, true);

            if(child.GetComponent<Collider>().GetType() == typeof(BoxCollider)){
                temp.volumeType = "box";
                temp.colliderExtents = Vector3.Scale(child.transform.localScale, child.GetComponent<BoxCollider>().size);
                temp.colliderRadius = 0;
                // Debug.Log("box");

            } else if (child.GetComponent<Collider>().GetType() == typeof(SphereCollider)){
                temp.volumeType = "sphere";
                temp.colliderRadius = child.transform.localScale.x  * child.GetComponent<SphereCollider>().radius;
                temp.colliderExtents = new Vector3(0,0,0);
                // Debug.Log("circle");
            }
            level.primitiveGameObject.Add(temp);

            Debug.Log("Ground Added");
        } 

        if(GroundRoot.childCount != level.getListCount()){
            Debug.LogError("Error in Ground Block Children");
        }else{

        }
    }

    
   private void CreateOscillatorObjects(Transform OscillatorRoot){
        if(OscillatorRoot.childCount == 0){
            Debug.Log("No Oscillators in level");
            return;
        }

        foreach(Transform child in OscillatorRoot){
            OscPlat data = child.GetComponent<OscPlat>();
            
            Oscillaters tempOs = new Oscillaters(
                GetMeshName(child.gameObject), 
                data.dimensions,
                child.rotation,
                data.position,
                "",
                child.GetComponent<Collider>().GetType().ToString(),
                new Vector3(0, 0, 0),
                0,
                data.timePeriod,
                data.dist,
                data.direction,
                data.cooldown,
                data.waitDelay
            );

            if (child.GetComponent<Collider>().GetType() == typeof(BoxCollider))
            {
                tempOs.volumeType = "box";
                tempOs.colliderExtents = Vector3.Scale(child.transform.localScale,child.GetComponent<BoxCollider>().size);
                tempOs.colliderRadius = 0;
                // Debug.Log("box");

            }
            else if (child.GetComponent<Collider>().GetType() == typeof(SphereCollider))
            {
                tempOs.volumeType = "sphere";
                tempOs.colliderRadius = child.GetComponent<SphereCollider>().radius;
                tempOs.colliderExtents = new Vector3(0, 0, 0);
                // Debug.Log("circle");
            }
            level.oscList.Add(tempOs);
            Debug.Log("Added Oscillator");
        }
   }

    private void CreateHarmfulOscillatorObjects(Transform harmfulOscillatorRoot)
    {
        if (harmfulOscillatorRoot.childCount == 0)
        {
            Debug.Log("No Harmful Oscillators in level");
            return;
        }

        foreach (Transform child in harmfulOscillatorRoot)
        {
            OscPlat data = child.GetComponent<OscPlat>();

            Oscillaters tempOs = new Oscillaters(
                GetMeshName(child.gameObject),
                data.dimensions,
                child.rotation,
                data.position,
                "",
                child.GetComponent<Collider>().GetType().ToString(),
                new Vector3(0, 0, 0),
                0,
                data.timePeriod,
                data.dist,
                data.direction,
                data.cooldown,
                data.waitDelay
            );
            if (child.GetComponent<Collider>().GetType() == typeof(BoxCollider))
            {
                tempOs.volumeType = "box";
                tempOs.colliderExtents = Vector3.Scale(child.transform.localScale, child.GetComponent<BoxCollider>().size);
                tempOs.colliderRadius = 0;
                // Debug.Log("box");

            }
            else if (child.GetComponent<Collider>().GetType() == typeof(SphereCollider))
            {
                tempOs.volumeType = "sphere";
                tempOs.colliderRadius = child.GetComponent<SphereCollider>().radius;
                tempOs.colliderExtents = new Vector3(0, 0, 0);
                // Debug.Log("circle");
            }
            level.harmOscList.Add(tempOs);
            Debug.Log("Added Harmful Oscillator");
        }
    }

    private void CreateCheckPoints(Transform CheckRoot){
        if(CheckRoot.childCount == 0){
            Debug.LogError("No CheckPoints");
            return;
        }

        foreach(Transform child in CheckRoot){
            level.checkPoints.Add(child.transform.position);
            Debug.Log("Added CheckPoint");

        }

   }

    void CreateLights(Transform root)
    {
        foreach (Transform child in root)
        {
            Light lightComponent = child.GetComponent<Light>();
            LightInfo lightInfo = new LightInfo();
            lightInfo.radius = lightComponent.range;
            lightInfo.colour = lightComponent.color;
            lightInfo.position = child.transform.position;

            level.pointLights.Add(lightInfo);
            Debug.Log("ADDED LIGHT!");
        }
    }

    void CreateSprings(Transform root)
    {
        foreach (Spring child in root.GetComponentsInChildren<Spring>())
        {
            SpringInfo spring = new SpringInfo(
                GetMeshName(child.gameObject),
                child.dimensions,
                child.transform.rotation,
                child.position,
                "",
                child.GetComponent<Collider>().GetType().ToString(),
                new Vector3(0, 0, 0),
                0,
                child.direction,
                child.force,
                child.activeTime,
                child.isContinuous,
                child.continuousForce
            );

            if (child.GetComponent<Collider>().GetType() == typeof(BoxCollider))
            {
                spring.volumeType = "box";
                spring.colliderExtents = Vector3.Scale(child.transform.localScale, child.GetComponent<BoxCollider>().size);
                spring.colliderRadius = 0;
                // Debug.Log("box");

            }
            else if (child.GetComponent<Collider>().GetType() == typeof(SphereCollider))
            {
                spring.volumeType = "sphere";
                spring.colliderRadius = child.GetComponent<SphereCollider>().radius;
                spring.colliderExtents = new Vector3(0, 0, 0);
                // Debug.Log("circle");
            }
            Debug.Log("Added Spring!");
            level.springs.Add(spring);
        }

    }


    string GetMeshName(GameObject obj)
    {
        string meshName = "";
        MeshSelector meshInfo;
        if (obj.TryGetComponent<MeshSelector>(out meshInfo)){
            meshName += meshInfo.mesh.ToString();
            meshName += ".";
            meshName += meshInfo.extension.ToString();
            return meshName;
        }

        return "Cube.msh";
    }
}