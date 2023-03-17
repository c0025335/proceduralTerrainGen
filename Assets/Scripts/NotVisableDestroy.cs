using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotVisableDestroy : MonoBehaviour
{

    Camera cam;
    Plane[] planes;
    Collider terrainCollider;
    public MeshRenderer terrainRender;

    void Start()
    {
        cam = Camera.main;
        terrainCollider =  GetComponent<Collider>();
        terrainRender = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if(GeometryUtility.TestPlanesAABB(planes, terrainCollider.bounds)){
            inCameraFrustrum();
        } else {
            notInCameraFrustrum();
        }
    }

    void inCameraFrustrum(){
        terrainRender.enabled = true;
        gameObject.tag = "visableTerrain";
    }

    void notInCameraFrustrum(){
        terrainRender.enabled = false;
        gameObject.tag = "notVisableTerrain";
    }
}
