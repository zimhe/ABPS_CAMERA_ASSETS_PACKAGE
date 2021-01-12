using OutlineEffect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCameraManager : MonoBehaviour
{
    [SerializeField] CameraController[] cameraControllers;
    [SerializeField] Camera _targetCamera;



    // Start is called before the first frame update

    int currentActivedCameraIndex = -1;

    private void Awake()
    {
        SetActiveCamera(0);
    
    }

    void FindCameraControllers()
    {

    }



    public void CycleCameras()
    {
        int next= (currentActivedCameraIndex + 1) % cameraControllers.Length;

        cameraControllers[next].gameObject.SetActive(true);

        cameraControllers[currentActivedCameraIndex].Give(cameraControllers[next]);

        cameraControllers[currentActivedCameraIndex].gameObject.SetActive(false);

        currentActivedCameraIndex = next;
    }

    public void SetActiveCamera(int index)
    {
        if (currentActivedCameraIndex == index)
        {
            return;
        }

        if (currentActivedCameraIndex == -1)
        {
            cameraControllers[index].gameObject.SetActive(true);
            cameraControllers[index].Take(_targetCamera);
            currentActivedCameraIndex = index;
        }
        else
        {
            cameraControllers[index].gameObject.SetActive(true);
            cameraControllers[currentActivedCameraIndex].Give(cameraControllers[index]);
            cameraControllers[currentActivedCameraIndex].gameObject.SetActive(false);
            currentActivedCameraIndex = index;
        }

       
        for (int i = 0; i < cameraControllers.Length; i++)
        {
            if (i != currentActivedCameraIndex)
            {
                cameraControllers[i].gameObject.SetActive(false);
            }
        }
    }

    public Camera GetCurrentCamera
    {
        get
        {
            return cameraControllers[currentActivedCameraIndex].GetComponentInChildren<Camera>();
        }
    }
}
