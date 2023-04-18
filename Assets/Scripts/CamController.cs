using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] public float movementSpeed = 10f;
    [SerializeField] public float sensitivity = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        transform.position = new Vector3(0, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W)){
            transform.position += transform.forward * movementSpeed*Time.fixedDeltaTime;
        }
        if(Input.GetKey(KeyCode.A)){
            transform.position += -transform.right * movementSpeed*Time.fixedDeltaTime;
        }
        if(Input.GetKey(KeyCode.S)){
            transform.position += -transform.forward * movementSpeed*Time.fixedDeltaTime;
        }
        if(Input.GetKey(KeyCode.D)){
            transform.position += transform.right * movementSpeed*Time.fixedDeltaTime;
        }

        Vector3 cameraRotation = transform.eulerAngles;

        float xAxisRotation = -Input.GetAxis("Mouse Y") * 720f * sensitivity * Time.fixedDeltaTime;
        float yAxisRotation = Input.GetAxis("Mouse X") * 720f * sensitivity * Time.fixedDeltaTime;

        yAxisRotation += cameraRotation.y;
        xAxisRotation += cameraRotation.x;

        if(xAxisRotation < 180 && xAxisRotation > 0) xAxisRotation = Mathf.Clamp(xAxisRotation, 0f, 90f);
        
        if(xAxisRotation < 360 && xAxisRotation > 180) xAxisRotation = Mathf.Clamp(xAxisRotation, 270f, 360f);

        transform.rotation = Quaternion.Euler(new Vector3(xAxisRotation, yAxisRotation, 0));

    }
}
