using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotVisableDestroy : MonoBehaviour
{
    void OnBecameInvisible(){
        Destroy(gameObject);
    }
}
