using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbiting : MonoBehaviour
{
    [SerializeField]
    private float centerDistance;

    [SerializeField]
    [Range(0, 360)]
    private float currentAngle;

    [SerializeField]
    private float rotationRate;
    [SerializeField]
    private bool rotate;
    [SerializeField]
    private float mouseWheelSpeed;
    [SerializeField]
    private float mouseRotationSpeed;

    private Vector3 rotationCenter;

    private Vector3 initialOffset;
    private float initialAngle;

    private void Awake()
    {
        rotationCenter = RotationCenter;
        initialOffset = (transform.position - rotationCenter).normalized;
        initialAngle = currentAngle;
    }

    private Vector3 RotationCenter => transform.position + transform.forward * centerDistance;

    private void UpdatePositionBasedOnAngle()
    {
        var angleDelta = currentAngle - initialAngle;
        var rotator = Quaternion.AngleAxis(angleDelta, Vector3.up);
        var newOffset = rotator * initialOffset;
        var position = rotationCenter + newOffset * centerDistance;
        var rotation = Quaternion.LookRotation(-newOffset, Vector3.up);
        transform.position = position;
        transform.rotation = rotation;
    }

    private void Update()
    {
        centerDistance -= Input.GetAxis("Mouse ScrollWheel") * mouseWheelSpeed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.S))
            rotate = !rotate;
        if (!rotate && Input.GetMouseButton(0))
        {
            currentAngle = (currentAngle + mouseRotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime) % 360;
        }
    }

    private void LateUpdate()
    {
        if (rotate)
        {
            currentAngle = (currentAngle + rotationRate * Time.deltaTime) % 360;
        }
        UpdatePositionBasedOnAngle();
    }

    private void OnDrawGizmosSelected()
    {
        var position = RotationCenter;
        if (Application.isPlaying)
            position = rotationCenter;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, position);
        Gizmos.DrawSphere(position, 0.1f);
    }
}
