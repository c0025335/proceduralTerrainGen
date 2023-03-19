using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{   
    public Mesh mesh;
    public BoxCollider AABB;
    public int xSize = 10;
    public int zSize = 10;
    public List<Vector3> verticesList;
    public List<int> trianglePointList;
    public float PerlinNoiseZoomX = 0.3f;
    public float PerlinNoiseZoomZ = 0.3f;
    public float PerlinNoiseScale = 2f;
    public bool wireMesh = true;
    public List<Vector3> verticesDebuggingList;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        AABB = GetComponent<BoxCollider>();
        
        //Temp fix so it won't be detected when placing next grid next to it
        AABB.size = new Vector3(xSize - 1, 2, zSize - 1);
        AABB.center = new Vector3((float)xSize/2 - 1, 1, (float)zSize/2 - 1);
        
        updateMesh();
    }

    void createMeshVertices(){

        for(int z = 0; z <= zSize; z++){
            for(int x = 0; x <= xSize; x++){
                float y = Mathf.PerlinNoise((transform.position.x + x) * PerlinNoiseZoomX, (transform.position.z + z) * PerlinNoiseZoomZ) * PerlinNoiseScale;
                verticesList.Add(new Vector3(x, y, z));
                verticesDebuggingList.Add(new Vector3((transform.position.x + x), y, (transform.position.z + z)));
            }
        }

    }

    void createMeshTriangles(){

        for(int z = 0; z < zSize; z++){
            for(int x = 0; x < xSize; x++){
                trianglePointList.Add(x + (z * (zSize + 1)));
                trianglePointList.Add(x + xSize + 1 + (z * (zSize + 1)));
                trianglePointList.Add(x + 1 + (z * (zSize + 1)));

                trianglePointList.Add(x + xSize + 1 + (z * (zSize + 1)));
                trianglePointList.Add(x + xSize + 2 + (z * (zSize + 1)));
                trianglePointList.Add(x + 1 + (z * (zSize + 1)));
            }
        }

    }

    void updateMesh()
    {
        verticesList.Clear();
        trianglePointList.Clear();
        verticesDebuggingList.Clear();
        mesh.Clear();

        createMeshVertices();
        createMeshTriangles();

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglePointList.ToArray();
        mesh.RecalculateNormals();

    }

    private void OnDrawGizmos(){
        
        if(verticesDebuggingList == null || wireMesh == false) return;
        for (int i = 0; i < verticesList.Count; i++) Gizmos.DrawSphere(verticesDebuggingList.ToArray()[i], 0.05f);
        Gizmos.color = Color.black;
        Gizmos.DrawWireMesh(mesh, transform.position);

    }
}