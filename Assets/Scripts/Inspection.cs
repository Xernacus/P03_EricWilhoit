using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Inspection : MonoBehaviour
{
    public GameObject camera;
    [Header("Keybinds")]
    [Tooltip("Key to take object")]
    private KeyCode interactKey = KeyCode.E;
    [Tooltip("Key to rotate object right after taking it")]
    private KeyCode rotateRightKey = KeyCode.R;
    [Tooltip("Key to rotate object up after taking it")]
    private KeyCode rotateUpKey = KeyCode.T;
    [Header("General Settings")]
    [Tooltip("Layers to check for player")]
    public LayerMask layers;
    [Tooltip("Radius to check for player in")]
    public float pickupDistance;
    [Tooltip("Position in front of camera")]
    [SerializeField]
    private Vector3 offset;
    [Tooltip("Rotation of object")]
    [SerializeField]
    private Vector3 rotation;
    [Tooltip("Rotation of object")]
    [SerializeField]
    private float scale;


    private float cameraOffset = 1;
    private float _rotationSpeed = 0.1f;
    private float _moveSpeed = 1f;
    private Boolean taking = false;
    private Boolean taken = false;
    private Boolean returning = false;
    private Vector3 _originalSizeBounds;
    private Vector3 _originalSize;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Collider[] _colliders = new Collider[50];
    private int _numColliders;
    

    public void Update()
    {
        if (Input.GetKey(interactKey))
        {
            TakeObject();
        }
    }
    public void TakeObject()
    {
        if (!taken && !taking && !returning)
        {
            if (Physics.OverlapSphereNonAlloc(transform.position, pickupDistance, _colliders, layers, QueryTriggerInteraction.Collide) > 0)
            {
                StartCoroutine(TakingUpdate());
            }           
        }
        else if (taken && !taking && !returning)
        {
            StartCoroutine(ReturningUpdate());
        }
        
    }

    IEnumerator TakingUpdate()
    {
        Debug.Log("Taking");
        taking = true;
        _originalSizeBounds = gameObject.GetComponent<BoxCollider>().bounds.size;
        _originalSize = gameObject.transform.localScale;
        _originalPosition = gameObject.transform.position;
        _originalRotation = gameObject.transform.rotation;
        float timeCount = 0.0f;
        while (gameObject.transform.rotation != Quaternion.Euler(rotation) || gameObject.transform.position != camera.transform.position + camera.transform.forward * cameraOffset + offset)
        {
            timeCount = timeCount + Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), timeCount * _rotationSpeed);
            transform.position = Vector3.MoveTowards(transform.position, camera.transform.position + camera.transform.forward * 1 + offset, Time.deltaTime * _moveSpeed);

            if (gameObject.GetComponent<BoxCollider>().bounds.size.magnitude > 0.8f)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 0.99f;
            }
            else if (gameObject.GetComponent<BoxCollider>().bounds.size.magnitude < 0.6f)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 1.01f;
            }

            yield return null;
        }
        taking = false;
        StartCoroutine(TakenUpdate());
    }
    IEnumerator TakenUpdate()
    {
        taken = true;
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, camera.transform.position + camera.transform.forward * 1 + offset, Time.deltaTime * _moveSpeed);
            if (Input.GetKey(rotateRightKey))
            {
                transform.rotation = transform.rotation * Quaternion.Euler(1.2f, 0, 0);
            }
            if (Input.GetKey(rotateUpKey))
            {
                transform.rotation = transform.rotation * Quaternion.Euler(0, 1.2f, 0);
            }
            yield return null;
        }
    }
    IEnumerator ReturningUpdate()
    {
        StopCoroutine(TakenUpdate());
        Debug.Log("Returning");
        returning = true;
        float timeCount = 0.0f;
        while (gameObject.transform.rotation != _originalRotation || gameObject.transform.position != _originalPosition)
        {
            timeCount = timeCount + Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, timeCount * _rotationSpeed);
            transform.position = Vector3.MoveTowards(transform.position, _originalPosition, Time.deltaTime * _moveSpeed);

            if (gameObject.GetComponent<BoxCollider>().bounds.size.magnitude > _originalSizeBounds.magnitude + _originalSizeBounds.magnitude * .02)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 0.99f;
            }
            else if (gameObject.GetComponent<BoxCollider>().bounds.size.magnitude < _originalSizeBounds.magnitude - _originalSizeBounds.magnitude * .02)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 1.01f;
            }

            yield return null;
        }
        gameObject.transform.localScale = _originalSize;
        returning = false;
        taken = false;
    }
}
