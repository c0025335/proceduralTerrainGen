using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPlacer : MonoBehaviour
{
    
    public GameObject terrain;
    [SerializeField] public int xSize = 10;
    [SerializeField] public int zSize = 10;

    void Start()
    {
        if(!terrain){
            Debug.Log("Here");
        }

        Vector3 newPos = transform.position;
        newPos.y = 0;
        
        newPos -= new Vector3((float)xSize/2, 0, 0);
        placeTerrain(newPos);
        
        newPos -= new Vector3((float)xSize, 0, 0);
        placeTerrain(newPos);
        
        newPos += new Vector3((float)xSize*2, 0, 0);
        placeTerrain(newPos);

    }

    void placeTerrain(Vector3 terrainPos){
        Instantiate(terrain, terrainPos, Quaternion.identity);
    }
}
