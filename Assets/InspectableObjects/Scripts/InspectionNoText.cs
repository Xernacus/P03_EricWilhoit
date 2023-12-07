using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectionNoText : MonoBehaviour
{

    [Header("Keybinds")]
    [Tooltip("Key to take object")]
    [SerializeField]
    private KeyCode interactKey = KeyCode.E;
    [Tooltip("Key to rotate object right after taking it")]
    [SerializeField]
    private KeyCode rotateRightKey = KeyCode.R;
    [Tooltip("Key to rotate object up after taking it")]
    [SerializeField]
    private KeyCode rotateUpKey = KeyCode.T;

    [Header("General Settings")]
    [Tooltip("Player camera")]
    public GameObject camera;
    [Tooltip("Layers to check for player")]
    public LayerMask layers;
    [Tooltip("Radius to check for player in")]
    public float pickupDistance;
    [Tooltip("Distance in front of camera to hold object")]
    [SerializeField]
    private float _cameraOffset = 1;
    [Tooltip("Rotation of object")]
    [SerializeField]
    private Vector3 _rotation;



    private float _rotationSpeed = 0.02f;
    private float _moveSpeed = 1f;
    private Boolean _taking = false;
    private Boolean _taken = false;
    private Boolean _returning = false;
    private Vector3 _originalSizeBounds;
    private Vector3 _originalSize;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Collider[] _colliders = new Collider[50];
    private BoxCollider _collider;
    public void Start()
    {
        _collider = gameObject.GetComponent<BoxCollider>();
    }
    public void Update()
    {
        if (Input.GetKey(interactKey))
        {
            TakeObject();
        }
    }
    public void TakeObject()
    {
        if (!_taken && !_taking && !_returning)
        {
            if (Physics.OverlapSphereNonAlloc(transform.position, pickupDistance, _colliders, layers, QueryTriggerInteraction.Collide) > 0)
            {
                StartCoroutine(TakingUpdate());
            }
        }
        else if (_taken && !_taking && !_returning)
        {
            StartCoroutine(ReturningUpdate());
        }

    }

    IEnumerator TakingUpdate()
    {
        _taking = true;
        _originalSizeBounds = _collider.bounds.size;
        _originalSize = gameObject.transform.localScale;
        _originalPosition = gameObject.transform.position;
        _originalRotation = gameObject.transform.rotation;
        float timeCount = 0.0f;

        while (gameObject.transform.rotation != Quaternion.Euler(_rotation) || gameObject.transform.position != camera.transform.position + camera.transform.forward * _cameraOffset)
        {
            timeCount = timeCount + Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_rotation), timeCount * _rotationSpeed);
            transform.position = Vector3.MoveTowards(transform.position, camera.transform.position + camera.transform.forward, Time.deltaTime * _moveSpeed);

            if (_collider.bounds.size.magnitude > 0.8f)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 0.999f;
            }
            else if (_collider.bounds.size.magnitude < 0.6f)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 1.001f;
            }
            yield return null;
        }
        _taking = false;
        StartCoroutine(TakenUpdate());
    }
    IEnumerator TakenUpdate()
    {
        _taken = true;
        while (!_returning)
        {
            transform.position = Vector3.MoveTowards(transform.position, camera.transform.position + camera.transform.forward, Time.deltaTime * _moveSpeed);
            if (Input.GetKey(rotateRightKey))
            {
                transform.rotation = transform.rotation * Quaternion.Euler(90f * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(rotateUpKey))
            {
                transform.rotation = transform.rotation * Quaternion.Euler(0, 90f * Time.deltaTime, 0);
            }
            yield return null;
        }
        _taken = false;
    }
    IEnumerator ReturningUpdate()
    {
        _returning = true;
        float timeCount = 0.0f;
        while (gameObject.transform.rotation != _originalRotation || gameObject.transform.position != _originalPosition)
        {
            timeCount = timeCount + Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, timeCount * _rotationSpeed);
            transform.position = Vector3.MoveTowards(transform.position, _originalPosition, Time.deltaTime * _moveSpeed);

            if (_collider.bounds.size.magnitude > _originalSizeBounds.magnitude + _originalSizeBounds.magnitude * .02)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 0.999f;
            }
            else if (_collider.bounds.size.magnitude < _originalSizeBounds.magnitude - _originalSizeBounds.magnitude * .02)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 1.001f;
            }

            yield return null;
        }
        gameObject.transform.localScale = _originalSize;

        _returning = false;
        _taken = false;
    }
}
