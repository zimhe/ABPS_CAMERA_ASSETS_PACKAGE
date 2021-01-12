using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowAgent : CameraController
{
    public Transform target;

    [Header("Follow")]
    public float followStiffness = 1.0f;
    public float offsetDistance = 0.3f;
    int offsetDirection = -1;
    [Header("Orbit")]
    public bool followRotation = false;
    public bool invertYAxis = false;
    public float rotateSpeed = 10f;
    public float rotateStiffness = 1.0f;
    public float minRotationX = -90f;
    public float maxRotationX = 90f;


    [Header("Height")]
    public float height = 1.5f;
    public float changeHeightSpeed = 1.0f;
    public float changeHeightStiffness = 1.0f;
    public float minHeight = 1.2f;
    public float maxHeight = 2.0f;


    public ZoomWithWallClipProtection zoom;

    Vector3 _position;
    Vector3 _rotation;
    float _distance;

    private void OnEnable()
    {
        if (target == null)
        {
            var locator = GameObject.FindObjectOfType<FollowTargetLocator>();
            if (locator == null)
            {
                return;
            }
            target = locator.GetTarget;
            locator.FollowingCameraXform = targetCamera.transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _position = transform.position;
        _rotation = transform.rotation.eulerAngles;
        _distance = -targetCamera.transform.localPosition.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        SetPosition();
        SetRotation();
        SetHeight();
    }

    void SetPosition()
    {
        _position = Vector3.Lerp(_position, target.position + Vector3.up * height+Vector3.right*offsetDirection*offsetDistance, Time.deltaTime * followStiffness);
        transform.position = _position;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            offsetDirection = -offsetDirection;
        }
    }

    void SetRotation()
    {
        if (!followRotation)
        {
            if (Input.GetMouseButton(1))
            {
                float rotY = Input.GetAxis("Mouse Y") * rotateSpeed;
                _rotation.x -= invertYAxis ? -rotY : rotY;
                _rotation.y += Input.GetAxis("Mouse X") * rotateSpeed;

                _rotation.x = Mathf.Clamp(_rotation.x, minRotationX, maxRotationX);
            }
        }
        else
        {
            _rotation.y = Mathf.Lerp(_rotation.y, target.rotation.eulerAngles.y, Time.deltaTime * rotateStiffness);
        }

        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, _rotation, Time.deltaTime* rotateStiffness);

        var q = Quaternion.Euler(_rotation.x, _rotation.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * rotateStiffness);
    }

    void SetHeight()
    {
        if (Input.GetMouseButton(2))
        {
            float targetHeight = height;
            targetHeight -= Input.GetAxis("Mouse Y") * changeHeightSpeed;
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
            height = targetHeight;
        }
    }

    public override void Take(Camera camera)
    {
        if (zoom != null)
        {
            zoom.m_Cam = camera.transform;
        }
        base.Take(camera);
    }

    public override void Give(CameraController other)
    {
        if (zoom != null)
        {
            zoom.m_Cam = null;
        }
        base.Give(other);
    }
}
