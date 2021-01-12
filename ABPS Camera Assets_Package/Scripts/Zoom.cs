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
    public class Zoom : MonoBehaviour
    {
        [SerializeField]
        private float _sensitivity = 1.0f;

        [SerializeField]
        private float _stiffness = 5.0f;

        
         Transform _cameraTransform;

       CameraBehaviors cameraBehaviors;
        

        private float _distance = 50.0f;
        public float _minDistance = 1.0f;
        public float _maxDistance = 100.0f;
        public bool interactable = true;
        

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            cameraBehaviors = GetComponent<CameraBehaviors>();
            _cameraTransform = cameraBehaviors.GetCameraTransfrom;
            _distance = -_cameraTransform.localPosition.z;
        }




        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            if (interactable)
            {
                _distance -= Input.GetAxis("Mouse ScrollWheel") * _sensitivity * _distance;
            }
        
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);

            var p = _cameraTransform.localPosition;
            p.z = Mathf.Lerp(p.z, -_distance, Time.deltaTime * _stiffness);

            _cameraTransform.localPosition = p;
        }

        public void SetDistance(float dist)
        {
            _distance =-dist;
        }
    }
}
