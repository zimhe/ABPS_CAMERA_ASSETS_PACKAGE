using System.Collections;
using System.Collections.Generic;
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
    public class Pan : MonoBehaviour
    {
        [SerializeField]
        private float _sensitivity = 1.0f;

        [SerializeField]
        private float _stiffness = 5.0f;

        public bool interactable = true;

         Transform _pivot;
        Transform _cameraTransform;
         CameraBehaviors cameraBehaviors;
        private Vector3 _position;

        float tension = 0;

        bool holding = false;
        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
          
            cameraBehaviors = GetComponent<CameraBehaviors>();
            _pivot = cameraBehaviors.GetPivot;
            _cameraTransform = cameraBehaviors.GetCameraTransfrom;
            _position = _pivot.position;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            if(Input.GetMouseButton(2)&&interactable)
            {
                holding = true;
                var t = _sensitivity * _cameraTransform.localPosition.z * 0.1f; // sensitivity is proportional to distance from pivot
                var dx = Input.GetAxis("Mouse X") * t;
                var dy = Input.GetAxis("Mouse Y") * t;
                _position += _cameraTransform.TransformVector(dx, dy, 0.0f);
                tension = dx * dx + dy * dy;

            }
            else
            {
                holding = false;
            }

          
            _pivot.position = Vector3.Lerp(_pivot.position, _position, Time.deltaTime * _stiffness);
        }

        public float GetTension
        {
            get => tension;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector3 position)
        {
            if (!holding)
            {
                _position = position;
            }
         
        }
    }
}
