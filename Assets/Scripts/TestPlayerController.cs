using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestPlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float turnSpeed = 1.5f;
    public Rigidbody _rb;
    public GameObject take;
    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
        TurnPlayer();

        if (Input.GetKeyDown(KeyCode.E))
        {
            take.GetComponent<Inspection>().TakeObject();
        }       
    }

    void MovePlayer()
    {
        float moveAmount = Input.GetAxisRaw("Vertical") * moveSpeed;

        Vector3 moveDirection = transform.forward * moveAmount;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + moveDirection, Time.deltaTime * moveSpeed);
    }

    void TurnPlayer()
    {
        float turnAmount = Input.GetAxisRaw("Horizontal") * turnSpeed;
        Quaternion turnOffset = Quaternion.Euler(0, turnAmount, 0);
        _rb.MoveRotation(_rb.rotation * turnOffset);
    }
}
