using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : CameraController
{
    [SerializeField] Transform _characterTransform;
    [SerializeField] bool invertYAxis;

    FirstPersonCameraPivot _pivot;

    public float rotateSpeed = 10f;
    public float rotateStiffness = 1.0f;

    public float minRotationX = -90f;
    public float maxRotationX = 90f;

    public float normalFOV = 30f;
    public float zoomFOV = 60f;

    float FOV = 30f;

    Vector3 _rotation;
    // Start is called before the first frame update

    private void Awake()
    {
        _pivot = GameObject.FindObjectOfType<FirstPersonCameraPivot>();
        _pivot.SetController(this);
    }
    void Start()
    {
        FOV = normalFOV;
        _rotation.y = _characterTransform.rotation.eulerAngles.y;
        _rotation.x = transform.localRotation.eulerAngles.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            float rotY = Input.GetAxis("Mouse Y") * rotateSpeed;
            _rotation.x -= invertYAxis ? -rotY : rotY;
            _rotation.y += Input.GetAxis("Mouse X") * rotateSpeed;

            _rotation.x = Mathf.Clamp(_rotation.x, minRotationX, maxRotationX);
        }

        

        var q0 = Quaternion.Euler(_rotation.x, 0.0f, 0.0f);
        var q1= Quaternion.Euler(0.0f, _rotation.y, 0.0f);
        transform.localRotation= Quaternion.Slerp(transform.localRotation, q0, Time.deltaTime * rotateStiffness);
        _characterTransform.localRotation= Quaternion.Slerp(_characterTransform.localRotation, q1, Time.deltaTime * rotateStiffness);

        var rot = transform.rotation.eulerAngles;
        rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);

        if (Input.GetKey(KeyCode.Tab))
        {
            FOV = zoomFOV;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            FOV = normalFOV;
        }

        if (targetCamera.fieldOfView != FOV)
        {
            targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, FOV, Time.deltaTime * 10f);

            float dt = targetCamera.fieldOfView - FOV;

            if (dt < 0.01f)
            {
                targetCamera.fieldOfView = FOV;
            }
        }
    }

    public void SetCharacter(Transform character)
    {
        _characterTransform = character;
    }
}
