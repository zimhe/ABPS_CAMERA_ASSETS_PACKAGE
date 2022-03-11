using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraPivot : MonoBehaviour
{
    public Transform _characterTransform;
   public void SetController(FirstPersonCameraController controller)
    {
        controller.transform.parent = transform;
        controller.transform.localPosition = Vector3.zero;
        controller.transform.localRotation = Quaternion.identity;
        controller.SetCharacter(_characterTransform);
    }
}
