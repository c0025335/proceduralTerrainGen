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

        transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * 720f * sensitivity * Time.fixedDeltaTime;
    }
}
