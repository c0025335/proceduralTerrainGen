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
    public List<Color> colorsList;
    public float PerlinNoiseZoomX = 0.3f;
    public float PerlinNoiseZoomZ = 0.3f;
    public float PerlinNoiseScale = 2f;
    public Gradient terrainGradient;
    public bool wireMesh = true;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        AABB = GetComponent<BoxCollider>();
        
        AABB.size = new Vector3(xSize, PerlinNoiseScale, zSize);
        AABB.center = new Vector3(0, PerlinNoiseScale/2, 0);
        
        updateMesh();
    }

    void createMeshVerticesAndColours(){

        Vector3 meshOffsetPos = transform.position;
        meshOffsetPos -= new Vector3((float)xSize/2, 0, (float)zSize/2);

        for(int z = 0; z <= zSize; z++){
            for(int x = 0; x <= xSize; x++){
                float y = Mathf.PerlinNoise((meshOffsetPos.x + x) * PerlinNoiseZoomX, (meshOffsetPos.z + z) * PerlinNoiseZoomZ) * PerlinNoiseScale;
                verticesList.Add(new Vector3((float)(x - xSize/2), y, (float)(z - zSize/2)));

                float gradienValue = y/PerlinNoiseScale;
                colorsList.Add(terrainGradient.Evaluate(gradienValue));
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
        colorsList.Clear();
        mesh.Clear();

        createMeshVerticesAndColours();
        createMeshTriangles();

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglePointList.ToArray();
        mesh.colors = colorsList.ToArray();
        mesh.RecalculateNormals();

    }

    private void OnDrawGizmos(){
        
        if(verticesList == null || wireMesh == false) return;
        for (int i = 0; i < verticesList.Count; i++) Gizmos.DrawSphere(verticesList.ToArray()[i], 0.05f);
        Gizmos.color = Color.black;
        Gizmos.DrawWireMesh(mesh, transform.position);

    }
}