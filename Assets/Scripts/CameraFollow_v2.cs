using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow_v2 : MonoBehaviour
{


    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float timeOffset;

    [SerializeField]
    private Vector2 posOffset;

    private Vector3 velocity;
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos =player.transform.position;
        endPos.x-=posOffset.x;
        endPos.y-=posOffset.y;
        endPos.z-=10;
        //*Lerp 
        // transform.position = Vector3.Lerp(startPos, endPos, timeOffset* Time.deltaTime);
    
        transform.position  = Vector3.SmoothDamp(startPos, endPos,ref velocity, timeOffset);
    }
}
