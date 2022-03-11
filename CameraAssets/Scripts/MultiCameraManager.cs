
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCameraManager : MonoBehaviour
{
    [SerializeField] CameraController[] cameraControllers;
    [SerializeField] Camera _targetCamera;
    [SerializeField] GameObject _targetPlayerObject;
    [SerializeField] bool autoSearchPlayer = false;
    [SerializeField] string playerSearchKey = "";


    public EventArgs camChangeArgs;

    public EventHandler OnCameraChanged;

    public static MultiCameraManager Instance
    {
        get;private set;
    }

    public static bool IsNull
    {
        get => Instance == null;
    }



    // Start is called before the first frame update

    int currentActivedCameraIndex = -1;

    public int ActiveIndex
    {
        get => currentActivedCameraIndex;
    }

    private void OnEnable()
    {
        if (Instance != null)
        {
            Destroy(this);
            throw new System.Exception("You can only have one Multi-Camera Manager in the scene");
        }
        Instance = this;

        if (autoSearchPlayer)
        {

            TryFindPlayer();
        }

        OnCameraChanged = new EventHandler(OnCameraChange);
    }

    bool TryFindPlayer()
    {
        _targetPlayerObject = GameObject.Find(playerSearchKey);

        bool foundPlayer = false;

        if (string.IsNullOrEmpty(playerSearchKey))
        {
            //Debug.LogError("Can't find player, the search key provided is empty");
        }
        else
        {
            if (_targetPlayerObject == null)
            {
                //Debug.LogError("Can't find player, no game object with the name of the given key");
               
            }
            else
            {
                foundPlayer = true;
            }
        }

        return foundPlayer;

     
    }

    private void Awake()
    {
        SetActiveCamera(0);
    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            CycleCameras();
        }
    }

    void FindCameraControllers()
    {

    }

    public void SetPlayerObject(GameObject playerobj)
    {
        _targetPlayerObject = playerobj;
    }

  

    

   

    public void CycleCameras()
    {

        camChangeArgs = new EventArgs();
        

        int next= (currentActivedCameraIndex + 1) % cameraControllers.Length;

        if (next == currentActivedCameraIndex)
        {
            return;
        }

        cameraControllers[next].gameObject.SetActive(true);

        cameraControllers[currentActivedCameraIndex].Give(cameraControllers[next]);

        cameraControllers[currentActivedCameraIndex].gameObject.SetActive(false);

        currentActivedCameraIndex = next;

        OnCameraChanged.Invoke(this, new EventArgs());
    }


    public void OnCameraChange(object sender,EventArgs e)
    {

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
            return _targetCamera;
        }
    }

    public CameraController GetCurrentController
    {
        get => cameraControllers[currentActivedCameraIndex];
    }

    public GameObject GetPlayerObject
    {
        get
        {
            if (_targetPlayerObject == null)
            {
                TryFindPlayer();
            }
            return _targetPlayerObject;
        }
    }
}
