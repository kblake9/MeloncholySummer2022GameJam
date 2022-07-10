using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraFollow : MonoBehaviour
{
    [Header("Basic Properties")]
    [SerializeField]
    private Transform target;

    [SerializeField]
    private bool following;

    [SerializeField, Range(0,100), Tooltip("The higher the value, the more time it will take for the camera to follow the player")]
    private float smoothness = 3f;

    [Header("Position Offset")]
    [SerializeField]
    private float offX;
    public float OffX
    {
        set
        {
            offX = value;
        }
    }
    [SerializeField]
    private float offY;
    public float OffY
    {
        set
        {
            offY = value;
        }
    }
    [SerializeField]
    private float offZ;
    public float OffZ
    {
        set
        {
            offZ = value;
        }
    }

    private Vector3 velocity = Vector3.zero;
    private Vector3 offset;

    public Transform Target
    {
        set
        {
            target = value;
        }
    }

    void FixedUpdate()
    {
        if (following)
        {
            offset = new Vector3(offX, offY, offZ);
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothness * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        
        //transform.LookAt(target);
    }
    //For Button Purposes

    public void AdjustXValue(float x)
    {
        offset += new Vector3(x, 0, 0);
    }

    public void AdjustYValue(float y)
    {
        offset += new Vector3(0, y, 0);
    }

    public void AdjustZValue(float z)
    {
        offset += new Vector3(0, 0, z);
    }
}
