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
    [SerializeField] public GameObject player;

    void Start()
    {

        findTerrainGrids();
        
        if(grid.GetComponent<TerrainGenerator>() == null){
            Debug.Log("Not applicable gameObject.");
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }

        gridVars = grid.GetComponent<TerrainGenerator>();

        if(!player) player = GameObject.Find("Player");

        if(activeGrids.Count <= 0){

            Vector3 startGridPos = player.transform.position;
            startGridPos.y = 0f;

            placeGrid(startGridPos);
        }

    }

    void Update()
    {
        float cameraRotationX = player.transform.eulerAngles.x % 360;

        if(cameraRotationX >= 30 || cameraRotationX <= -40) return;

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
            
            } else {
                
                float maxDistance = Mathf.Abs(player.transform.position.y);
                Vector3 cameraPos = player.transform.position;
                Vector3 dir = Vector3.down;
                RaycastHit ray;

                if(cameraPos.y < 0) dir = Vector3.up;

                if(!(Physics.Raycast(cameraPos, dir, out ray, maxDistance))){

                    cameraPos.y = 0;
                    foreach(Vector3 pos in upperAndLowerBoundsGrids(cameraPos)){
                        placeGrid(pos);
                    }
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
                    
                    posToCheck -= player.transform.right * gridVars.xSize;
                    Vector3[] possiblePosForGridSpawns = upperAndLowerBoundsGrids(posToCheck);

                    int beforeCheck = gridPosToSpawn.Count;
                    foreach(Vector3 pos in possiblePosForGridSpawns){
                        if (posInCameraFrustrum(pos)){
                            existingGridsInPos(pos, gridPosToSpawn);
                        }
                    }
                    
                    if(beforeCheck >= gridPosToSpawn.Count || gridPosToSpawn.Count >= maxAmountActiveGrids){
                        unableToGenerateLeft = true;
                    }
                }

                posToCheck = resetPos;

                while(!unableToGenerateRight){
                    
                    posToCheck += player.transform.right * gridVars.xSize;
                    Vector3[] possiblePosForGridSpawns = upperAndLowerBoundsGrids(posToCheck);

                    int beforeCheck = gridPosToSpawn.Count;
                    foreach(Vector3 pos in possiblePosForGridSpawns){
                        if (posInCameraFrustrum(pos)){
                            existingGridsInPos(pos, gridPosToSpawn);
                        }
                    }
                    
                    if(beforeCheck >= gridPosToSpawn.Count || gridPosToSpawn.Count >= maxAmountActiveGrids){
                        unableToGenerateRight = true;
                    }
                }

                posToCheck = resetPos;
                
                if(unableToGenerateLeft && unableToGenerateRight) {

                    posToCheck += player.transform.forward * gridVars.xSize;

                    posToCheck.x = (float)Mathf.RoundToInt(posToCheck.x/gridVars.xSize)*gridVars.xSize;
                    posToCheck.z = (float)Mathf.RoundToInt(posToCheck.z/gridVars.xSize)*gridVars.zSize;
                    posToCheck.y = 0f;

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

    public Vector3[] upperAndLowerBoundsGrids(Vector3 pos){

        float upperX = (float)Mathf.CeilToInt(pos.x/gridVars.xSize) * gridVars.xSize;
        float upperZ = (float)Mathf.CeilToInt(pos.z/gridVars.zSize) * gridVars.zSize;
        float lowerX = (float)Mathf.FloorToInt(pos.x/gridVars.xSize) * gridVars.xSize;
        float lowerZ = (float)Mathf.FloorToInt(pos.z/gridVars.zSize) * gridVars.zSize;

        Vector3[] gridBounds = new Vector3[4];
        gridBounds[0] = new Vector3(upperX, 0 ,upperZ);
        gridBounds[1] = new Vector3(upperX, 0 ,lowerZ);
        gridBounds[2] = new Vector3(lowerX, 0 ,upperZ);
        gridBounds[3] = new Vector3(lowerX, 0 ,lowerZ);

        return gridBounds;
    }

}
