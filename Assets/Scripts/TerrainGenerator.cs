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
    public Material mat;
    public Color[] keyColours = new Color[4];
    [Range(0f, 1f)] public float[] colourHeights = new float[4];
    [Range(0f, 1f)] public float[] colourBlends = new float[4];
    public bool wireMesh = true;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        AABB = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        
        AABB.size = new Vector3(xSize, PerlinNoiseScale, zSize);
        AABB.center = new Vector3(0, PerlinNoiseScale/2, 0);
        
        updateMesh();
        materialSettings();
    }

    void Update()
    {
        updateMesh();
        materialSettings();
    }

    void createMeshVertices(){

        Vector3 meshOffsetPos = transform.position;
        meshOffsetPos -= new Vector3((float)xSize/2, 0, (float)zSize/2);

        for(int z = 0; z <= zSize; z++){
            for(int x = 0; x <= xSize; x++){
                float y = Mathf.PerlinNoise((meshOffsetPos.x + x) * PerlinNoiseZoomX, (meshOffsetPos.z + z) * PerlinNoiseZoomZ) * PerlinNoiseScale;
                verticesList.Add(new Vector3((float)(x - xSize/2), y, (float)(z - zSize/2)));
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

    void updateMesh(){
        
        verticesList.Clear();
        trianglePointList.Clear();
        mesh.Clear();

        createMeshVertices();
        createMeshTriangles();

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglePointList.ToArray();
        mesh.RecalculateNormals();

    }

    void materialSettings(){
        mat.SetFloat("PerlinNoiseScale", PerlinNoiseScale);
        mat.SetColorArray("keyColours", keyColours);
        mat.SetFloatArray("colourHeights", colourHeights);
        mat.SetFloatArray("colourBlends", colourBlends);
    }

    private void OnDrawGizmos(){
        
        if(verticesList == null || wireMesh == false) return;
        for (int i = 0; i < verticesList.Count; i++) Gizmos.DrawSphere(verticesList.ToArray()[i], 0.05f);
        Gizmos.color = Color.black;
        Gizmos.DrawWireMesh(mesh, transform.position);

    }
}