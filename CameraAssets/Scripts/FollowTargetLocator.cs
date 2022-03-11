using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetLocator : MonoBehaviour
{
    [SerializeField] FirstPersonCameraPivot pivot;
    [SerializeField] Transform target;
    [SerializeField] Transform followingCameraXform;


    public Transform GetTarget
    {
        get => target;
    }

    public Transform FollowingCameraXform
    {
        get => followingCameraXform;
        set => followingCameraXform = value;
    }

}
