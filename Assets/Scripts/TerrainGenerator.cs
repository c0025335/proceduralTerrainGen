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
    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;
    public Texture2D[] textures = new Texture2D[4];
    [Range(0f, 1f)] public float[] textureStartHeights = new float[4];
    [Range(0f, 1f)] public float[] textureBlends = new float[4];
    public float[] textureScales = new float[4];
    public bool texturesOn = true;
    public List<Color> coloursList;
    public Gradient colourGradient;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        AABB = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        
        AABB.size = new Vector3(xSize, PerlinNoiseScale, zSize);
        AABB.center = new Vector3(0, PerlinNoiseScale/2, 0);
        
        updateMesh();

        if(texturesOn){
            materialSettings();
        } else {
            colourSettings();
        }

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

    Texture2DArray generateTextureArray(Texture2D[] textures2D){
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, 4, textureFormat, true);
        for(int i = 0; i < 4; i++){
            textureArray.SetPixels(textures2D[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }

    void materialSettings(){

        mat.SetFloat("PerlinNoiseScale", PerlinNoiseScale);
        mat.SetFloatArray("textureBlends", textureBlends);
        mat.SetFloatArray("textureStartHeights", textureStartHeights);
        mat.SetFloatArray("textureScales", textureScales);

        Texture2DArray textureArray = generateTextureArray(textures);
        mat.SetTexture("terrainTextures", textureArray);
    }

    void colourSettings(){
        
        coloursList.Clear();
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

        foreach(Vector3 vertex in verticesList){
            coloursList.Add(colourGradient.Evaluate(vertex.y/PerlinNoiseScale));
        }

        mesh.colors = coloursList.ToArray();

    }
}