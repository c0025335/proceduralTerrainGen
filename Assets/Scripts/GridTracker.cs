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
    [SerializeField] public int maxAmountActiveGrids = 200;
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

        if(newlyGeneratedGrids.Count < maxAmountNewlyGeneratedGrids && activeGrids.Count < maxAmountActiveGrids){

            acsendingDistanceToCam(activeGrids);

            List<Vector3> gridPosToSpawn = findNextEmptyPos();
            if(gridPosToSpawn != null){
                foreach(Vector3 pos in gridPosToSpawn){
                    placeGrid(pos);
                }
            }
        }
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

        if(activeGrids.Count > 0){

            List<Vector3> gridPosToSpawn = new List<Vector3>();
            bool unableToGenerateLeft = false;
            bool unableToGenerateRight = false;
            Vector3 closestGridPos = activeGrids[0].transform.position;
            Vector3 resetPos = closestGridPos;
            Vector3 posToCheck = resetPos;

            while(gridPosToSpawn.Count < maxAmountNewlyGeneratedGrids){

                while(!unableToGenerateLeft){
                    posToCheck -= Camera.main.transform.right * gridVars.xSize;
                    
                    Debug.Log("Left Before: " + posToCheck);
                    posToCheck.x = (float)Mathf.RoundToInt(posToCheck.x/gridVars.xSize)*gridVars.xSize;
                    posToCheck.z = (float)Mathf.RoundToInt(posToCheck.z/gridVars.xSize)*gridVars.zSize;
                    posToCheck.y = 0f;
                    Debug.Log("Left After: " + posToCheck);

                    if (posInCameraFrustrum(posToCheck)){

                        existingGridsInPos(posToCheck, gridPosToSpawn);

                    } else {
                        unableToGenerateLeft = true;
                    }
                }

                posToCheck = resetPos;

                while(!unableToGenerateRight){
                    posToCheck += Camera.main.transform.right * gridVars.xSize;
                    Debug.Log("Right Before: " + posToCheck);
                    posToCheck.x = (float)Mathf.RoundToInt(posToCheck.x/gridVars.xSize)*gridVars.xSize;
                    posToCheck.z = (float)Mathf.RoundToInt(posToCheck.z/gridVars.xSize)*gridVars.zSize;
                    posToCheck.y = 0f;
                    Debug.Log("Right After: " + posToCheck);

                    if (posInCameraFrustrum(posToCheck)){

                        existingGridsInPos(posToCheck, gridPosToSpawn);

                    } else {
                        unableToGenerateRight = true;
                    }
                }

                posToCheck = resetPos;
                
                if(unableToGenerateLeft && unableToGenerateRight) {
                    posToCheck += Camera.main.transform.forward * gridVars.xSize;
                    Debug.Log("Forward Before: " + posToCheck);
                    posToCheck.x = (float)Mathf.RoundToInt(posToCheck.x/gridVars.xSize)*gridVars.xSize;
                    posToCheck.z = (float)Mathf.RoundToInt(posToCheck.z/gridVars.xSize)*gridVars.zSize;
                    posToCheck.y = 0f;
                    Debug.Log("Forward After: " + posToCheck);

                    if (posInCameraFrustrum(posToCheck)){

                        existingGridsInPos(posToCheck, gridPosToSpawn);
                        unableToGenerateLeft = false;
                        unableToGenerateRight = false;
                    }

                    resetPos = posToCheck;
                }
            }

            return gridPosToSpawn;
        }
        
        return null;
    }

    public bool posInCameraFrustrum(Vector3 pos){

        Vector3 viewpos = Camera.main.WorldToViewportPoint(pos);
        bool inCameraFrustrum = (viewpos.x > 0 && viewpos.x < 1) && (viewpos.y > 0 && viewpos.y < 1);
        bool inFrontOfCamera = viewpos.z > 0;

        return (inCameraFrustrum && inFrontOfCamera);
    }

    public void existingGridsInPos(Vector3 pos, List<Vector3>posToCompare){
        
        Collider[] existingGrids = Physics.OverlapSphere(pos, (gridVars.xSize/2 - 0.1f));
        if(!posToCompare.Contains(pos) && existingGrids.Length <= 0){
            posToCompare.Add(pos);
        }

    }
}
