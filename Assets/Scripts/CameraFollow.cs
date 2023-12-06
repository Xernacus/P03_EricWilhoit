using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followObject = null;

    private void LateUpdate()
    {
        this.transform.position = followObject.position;
        this.transform.rotation = followObject.rotation;
    }
}
