using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private TMP_Text FpsText;
    
    private int lastFrameIndex = 0;
    private float[] deltaTimeArray = new float[60];

    void Start()
    {
        FpsText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if(lastFrameIndex > deltaTimeArray.Length - 1) lastFrameIndex = 0;
        deltaTimeArray[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex++;

        FpsText.text = "FPS: " + Mathf.RoundToInt(averageFPS());
    }

    private float averageFPS(){
        float totalTime = 0f;
        foreach(float time in deltaTimeArray) {
            totalTime += time;
        }

        return deltaTimeArray.Length / totalTime;
    }
}
