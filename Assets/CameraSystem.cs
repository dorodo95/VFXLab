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
    private Transform playerTarget;
    private Vector2 distance = new Vector2(-6.5f, 6.5f);

    private Vector3 cameraPos;

    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }


    void Update()
    {

        if (!targetSystem.isTargeting)
        {
            cameraPos = playerTarget.position;
            cameraPos.x += m_Camera.transform.forward.x * distance.x;
            cameraPos.y += m_Camera.transform.up.y * distance.y;
            cameraPos.z += m_Camera.transform.forward.z * distance.x;
            m_Camera.transform.position = cameraPos;
        }


        if (targetSystem.currentTarget != null && targetSystem.isTargeting)
        {
            Vector3 mediumTarget = (targetSystem.currentTarget.transform.position + playerTarget.position) / 2;
            Vector3 targetDistance = mediumTarget - playerTarget.position;

            targetDistance.x += m_Camera.transform.forward.x * distance.x;
            targetDistance.y += m_Camera.transform.up.y * distance.y;
            targetDistance.z += m_Camera.transform.forward.z * distance.x;

            m_Camera.transform.position = Vector3.MoveTowards(m_Camera.transform.position, playerTarget.position + targetDistance, Time.deltaTime * 25);
        }
    }
}
