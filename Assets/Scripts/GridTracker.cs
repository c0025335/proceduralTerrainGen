using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTracker : MonoBehaviour
{
    public List<GameObject> activeGrids;
    public List<GameObject> nonActiveGrids;
    public List<GameObject> newlyGeneratedGrids;

    [SerializeField] public int maxAmountNewlyGeneratedGrids = 10;
    [SerializeField] public int maxAmountNonActiveGrids = 100;
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

        if(activeGrids.Count <= 0){

            Vector3 startGridPos = Camera.main.transform.position;
            startGridPos.y = 0f;

            placeGrid(startGridPos);
        }

    }

    void Update()
    {
        findTerrainGrids();

        if(nonActiveGrids.Count >= maxAmountNonActiveGrids) {
            removeOldGrids();
        }

        if(newlyGeneratedGrids.Count < maxAmountNewlyGeneratedGrids){

            acsendingDistanceToCam(activeGrids);

            List<Vector3> gridPosToSpawn = findNextEmptyPos();
            if(gridPosToSpawn != null){
                foreach(Vector3 pos in gridPosToSpawn){
                    placeGrid(pos);
                }
            }
        }

        /*
        Need to change heavily now, after centering mesh to being on the gameObject's center
        if(activeGrids.Count > 0 && activeGrids.Count < maxAmountActiveGrids){

            acsendingDistanceToCam(activeGrids);
            
            //Straight Line from camera
            Vector3 nextGridPos = activeGrids[0].transform.position;
            for(int i = activeGrids.Count; i < maxAmountActiveGrids; i++){
                nextGridPos += (Camera.main.transform.forward.normalized) * gridVars.xSize;
                nextGridPos.y = 0f;
                placeGrid(nextGridPos);
            }
            
        }

        Collider[] gridColliders = Physics.OverlapSphere(Camera.main.transform.position, 12.5f);
        if(gridColliders.Length <= 0){
            
            if(activeGrids.Count >= 10){
                acsendingDistanceToCam(activeGrids);
                GameObject gridToDestory = activeGrids[activeGrids.Count-1];
                activeGrids.RemoveAt(activeGrids.Count-1);
                Destroy(gridToDestory.gameObject);
            }

            Vector3 undernethCam = Camera.main.transform.position;
            undernethCam.y = 0f;
            placeGrid(undernethCam);
            
        }
        */

    }

    void findTerrainGrids(){

        activeGrids.Clear();
        nonActiveGrids.Clear();
        newlyGeneratedGrids.Clear();

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

        GameObject[] newGrids = GameObject.FindGameObjectsWithTag("newlyGeneratedTerrain");
        if(newGrids.Length > 0){
            foreach(GameObject grid in newGrids){
                newlyGeneratedGrids.Add(grid);
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

        Collider[] gridColliders = Physics.OverlapSphere(gridPos, (gridVars.xSize/2 - 0.1f));
        
        if(gridColliders.Length <= 0){
            GameObject newGrid = Instantiate(grid, gridPos, Quaternion.identity);
            newGrid.tag = "newlyGeneratedTerrain";
        } else {
            Debug.Log("Failed");
            Debug.Log("Pos of Detected: " + gridColliders[0].transform.position);
            Debug.Log("Pos of Attempted: " + gridPos);
        }

        findTerrainGrids();

    }

    public List<Vector3> findNextEmptyPos(){

        List<Vector3> gridPosToSpawn = new List<Vector3>();
        bool unableToGenerateLeft = false;
        bool unableToGenerateRight = false;

        if(activeGrids.Count > 0){

            Vector3 posToCheck = activeGrids[activeGrids.Count-1].transform.position;
            Debug.Log("Active Grid: " + posToCheck);

            while(gridPosToSpawn.Count < maxAmountNewlyGeneratedGrids && unableToGenerateLeft == false){
                
                posToCheck -= Camera.main.transform.right * gridVars.xSize;

                Vector3 viewpos = Camera.main.WorldToViewportPoint(posToCheck);
                bool inCameraFrustrum = (viewpos.x > 0 && viewpos.x < 1) && (viewpos.y > 0 && viewpos.y < 1);
                bool inFrontOfCamera = viewpos.z > 0;
                
                if (inCameraFrustrum && inFrontOfCamera){
                    
                    Debug.Log("Area in Camera: " + posToCheck);

                    Collider[] existingGrids = Physics.OverlapSphere(posToCheck, (gridVars.xSize/2 - 0.1f));
                    if(!gridPosToSpawn.Contains(posToCheck)){
                        gridPosToSpawn.Add(posToCheck);
                        Debug.Log("Need to be made: " + posToCheck);
                    }

                } else {
                    Debug.Log("Left point not in camera");
                    unableToGenerateLeft = true;
                }
            }

            posToCheck = activeGrids[activeGrids.Count-1].transform.position;

            while(gridPosToSpawn.Count < maxAmountNewlyGeneratedGrids && unableToGenerateRight == false){
                
                posToCheck += Camera.main.transform.right * gridVars.xSize;

                Vector3 viewpos = Camera.main.WorldToViewportPoint(posToCheck);
                bool inCameraFrustrum = (viewpos.x > 0 && viewpos.x < 1) && (viewpos.y > 0 && viewpos.y < 1);
                bool inFrontOfCamera = viewpos.z > 0;
                
                if (inCameraFrustrum && inFrontOfCamera){
                    
                    Debug.Log("Area in Camera: " + posToCheck);

                    Collider[] existingGrids = Physics.OverlapSphere(posToCheck, (gridVars.xSize/2 - 0.1f));
                    if(!gridPosToSpawn.Contains(posToCheck)){
                        gridPosToSpawn.Add(posToCheck);
                        Debug.Log("Need to be made: " + posToCheck);
                    }

                } else {
                    Debug.Log("Right point not in camera");
                    unableToGenerateRight = true;
                }
            }

            return gridPosToSpawn;
            //Check Right side
            //Move Forward
        }
        
        return null;
    }
}
