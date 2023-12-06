using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspection : MonoBehaviour
{
    public GameObject camera;
    [Header("Settings")]
    [Tooltip("Position in front of camera")]
    [SerializeField]
    private Vector3 offset;
    [Tooltip("Rotation of object")]
    [SerializeField]
    private Vector3 rotation;

    private float _rotationSpeed = 1f;
    private float _moveSpeed = 1f;
    private Boolean taking;
    private Boolean returning;

    public void TakeObject()
    {
        StartCoroutine(InspectionUpdate());
    }

    IEnumerator InspectionUpdate()
    {
        taking = true;
        while (gameObject.transform.rotation != Quaternion.Euler(rotation) && gameObject.transform.position != camera.transform.position + offset)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, rotation, Time.deltaTime * _rotationSpeed, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, camera.transform.position + offset, Time.deltaTime * _moveSpeed);
            yield return null;
        }
        taking = false;
    }
}
