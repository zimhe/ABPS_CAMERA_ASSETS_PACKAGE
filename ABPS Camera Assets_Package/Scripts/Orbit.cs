using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * Notes
 */

namespace CameraDataUtilities
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [RequireComponent(typeof(CameraBehaviors))]
    public class Orbit : MonoBehaviour
    {
        [SerializeField]
        private float _sensitivity = 5.0f;

        [SerializeField]
        private float _stiffness = 10.0f;

        [SerializeField] bool autoOrbit = false;
        [SerializeField] float speed = 1.0f;
        private Transform _pivot;
        CameraBehaviors cameraBehaviors;

        public bool interactable = true;


        private Vector3 _rotation;

        private float _minRotationX = -90.0f;
        private float _maxRotationX = 90.0f;




        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            cameraBehaviors = GetComponent<CameraBehaviors>();
            _pivot = cameraBehaviors.GetPivot;
            _rotation = _pivot.rotation.eulerAngles;
         
        }


        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            if (Input.GetMouseButton(1) && interactable)
            {
                _rotation.x -= Input.GetAxis("Mouse Y") * _sensitivity;
                _rotation.y += Input.GetAxis("Mouse X") * _sensitivity;
                _rotation.x = Mathf.Clamp(_rotation.x, _minRotationX, _maxRotationX);
            }

            if (autoOrbit)
            {
                _rotation.y += speed;
            }

            var q = Quaternion.Euler(_rotation.x, _rotation.y, 0.0f);
            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, q, Time.deltaTime * _stiffness);
        }

        public void SetRotation(Vector3 rotation)
        {
            _rotation = rotation;

        }
    }
}
