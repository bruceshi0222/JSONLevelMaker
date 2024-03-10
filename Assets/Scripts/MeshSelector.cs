using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSelector : MonoBehaviour
{
    public enum MeshName
    {
        Cube,
        Sphere,
        brickwall,
        ironfence,
        paltformlarge,
        paltformsmall,
        pipesupphigh,
        towersquarearch,
        towersquareBase,
        towersquarewindow,
        wall,
        wallCorner,
        wallHalf,
        wallNarrow,
        wallNarrowCen
    }

    public enum Extension
    {
       obj,msh
    }

    [SerializeField] public MeshName mesh;
    [SerializeField] public Extension extension;
}
