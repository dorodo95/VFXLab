using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSystem : MonoBehaviour
{
    private Camera m_Camera;
    private Vector2 inputAxis;
    private Vector2 inputDirection;
    private targetSystem m_target;
    
    [SerializeField]
    private Transform target;
    private float distance = 5;

    private Vector3 cameraPos;
    private Quaternion cacheRotation;

    void Start()
    {
        m_Camera = GetComponent<Camera>(); 
        cacheRotation = m_Camera.transform.rotation;
    }

    
    void Update()
    {        
        cameraPos = target.position;
        cameraPos.z -= distance;
        cameraPos.y = m_Camera.transform.position.y;
        m_Camera.transform.position = cameraPos;

        if(targetSystem.currentTarget != null && targetSystem.isTargeting)
        {
            //Vector3 mediumTarget = (targetSystem.currentTarget.transform.position + target.position) / 2;
            //m_Camera.transform.LookAt(mediumTarget, Vector3.up);
        }

        else
        {
            m_Camera.transform.rotation = cacheRotation;
        }
    }
}
