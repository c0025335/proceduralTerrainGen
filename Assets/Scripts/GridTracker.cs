using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTracker : MonoBehaviour
{
    public List<GameObject> activeGrids;
    public List<GameObject> nonActiveGrids;
    
    [SerializeField] public int maxAmountActiveGrids = 10;
    [SerializeField] public int maxAmountNonActiveGrids = 10;
    [SerializeField] public GameObject grid;
    public TerrainGenerator gridVars;

    void Start()
    {
        findTerrainGrids();
        
        if(grid.GetComponent<TerrainGenerator>() == null){
            Debug.Log("Not applicable gameObject.");
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }

        gridVars = grid.GetComponent<TerrainGenerator>();
    }

    void Update()
    {
        findTerrainGrids();

        if(nonActiveGrids.Count >= maxAmountNonActiveGrids) {
            removeOldGrids();
        }

        Debug.Log(activeGrids.Count);
        if(activeGrids.Count == 0 && nonActiveGrids.Count == 0){

            Vector3 startGridPos = Camera.main.transform.position;
            startGridPos.y = 0f;
            startGridPos -= new Vector3((float)gridVars.xSize/2, 0, (float)gridVars.zSize/2);

            placeGrid(startGridPos);
        }

    }

    void findTerrainGrids(){

        activeGrids.Clear();
        nonActiveGrids.Clear();

        GameObject[] visableGrids = GameObject.FindGameObjectsWithTag("visableTerrain");
        if(visableGrids.Length > 0){
            foreach(GameObject grid in visableGrids){
                activeGrids.Add(grid);
            }
        }

        GameObject[] notVisableGrids = GameObject.FindGameObjectsWithTag("notVisableTerrain");
        if(notVisableGrids.Length > 0){
            foreach(GameObject grid in notVisableGrids){
                nonActiveGrids.Add(grid);
            }
        }
    }

    void removeOldGrids(){

        acsendingDistanceToCam(nonActiveGrids);

        int index = nonActiveGrids.Count;
        while(nonActiveGrids.Count > (int)maxAmountNonActiveGrids/2){
            GameObject gridToDestory = nonActiveGrids[index-1];
            nonActiveGrids.RemoveAt(index-1);
            Destroy(gridToDestory.gameObject);
            index--;
        }

    }

    void acsendingDistanceToCam(List<GameObject> grids){
        
        int size = grids.Count;

        List<float> distancesToCam = new List<float>();
        List<GameObject> tempStorage = new List<GameObject>();

        foreach(GameObject grid in grids){
            float distance = Vector3.SqrMagnitude(grid.transform.position - Camera.main.transform.position);
            distancesToCam.Add(distance);
            tempStorage.Add(grid);
        }

        grids.Clear();

        while(tempStorage.Count > 0){

            float minValue = -1f;
            foreach(float distance in distancesToCam){
                if(minValue == -1f || minValue >= distance) minValue = distance;
            }

            int index = distancesToCam.IndexOf(minValue);

            grids.Add(tempStorage[index]);
            distancesToCam.RemoveAt(index);
            tempStorage.RemoveAt(index);

        }

    }

    void placeGrid(Vector3 gridPos){
        Instantiate(grid, gridPos, Quaternion.identity);
        Debug.Log("Placed Grid");
    }
}
