using CameraDataUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera targetCamera;

    public LayerMask _cullingMask;
    public CameraClearFlags _clearFlags;

    

    public CameraDatas cameraBuffer;

    private void OnEnable()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }


    public virtual void Take(Camera camera)
    {
        var pos = Vector3.zero;
        pos.z = cameraBuffer._distance;
        targetCamera = camera;
        camera.transform.parent = transform;
        camera.transform.localPosition = pos;
        camera.transform.localRotation = Quaternion.identity;
        camera.fieldOfView = cameraBuffer._fov;
        camera.cullingMask = _cullingMask;
        camera.clearFlags = _clearFlags;
    }

    public virtual void Give(CameraController other)
    {
        cameraBuffer = new CameraDatas(targetCamera);
        other.Take(targetCamera);
    }
}
