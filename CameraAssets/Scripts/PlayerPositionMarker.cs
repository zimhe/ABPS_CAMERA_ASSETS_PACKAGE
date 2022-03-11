using CameraDataUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionMarker : MonoBehaviour
{
    [SerializeField] Pan pan;
    [SerializeField] RectTransform uiXform;
    [SerializeField] float pedding;
    
    FollowTargetLocator locator;

    Transform target;
    bool stickToTarget;

    Camera cam;
    Vector3 pivotPosition;
    private void OnEnable()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        bool hasTarget=TryGetTarget();

        cam = Camera.main;

        stickToTarget = true;
      
    }

   

    public bool TryGetTarget()
    {
        if (MultiCameraManager.Instance==null|| MultiCameraManager.Instance.GetPlayerObject == null)
        {
            uiXform.gameObject.SetActive(false);
            return false;
        }
        else
        {
            locator = MultiCameraManager.Instance.GetPlayerObject.GetComponent<FollowTargetLocator>();
            if (locator != null)
            {
                target = locator.GetTarget;
                uiXform.gameObject.SetActive(true);
                return true;
            }
            else
            {
                uiXform.gameObject.SetActive(false);
                return false;
            }
        }

    }

    // Update is called once per frame
   
    public void GoToTarget()
    {
        pan.SetPosition(target.position);
    }

    public void GoTOPosition(Vector3 position)
    {
        pan.SetPosition(position);
    }

    public void ToggleStickToTarget()
    {
        stickToTarget = !stickToTarget;
    }
    public void StickToTarget()
    {
        stickToTarget = true;
    }


    private void Update()
    {
        if (target == null)
        {
            if (!TryGetTarget()) return;
            
        }

        if (uiXform.gameObject.activeSelf&&target!=null)
        {
            var screenPoint = cam.WorldToScreenPoint(target.position);
            var rect = uiXform.rect;

            if (screenPoint.z < 0)
            {
                screenPoint.x = (Screen.width - screenPoint.x);
                screenPoint.y = 0;
            }

            Vector3 outBound = Vector3.zero;

            var position = ScreenClampPoint(rect, screenPoint, out outBound, pedding);

            uiXform.position = position;

        }

        if (stickToTarget)
        {
            GoToTarget();
        }

        if (pan.GetTension > 10f)
        {
            stickToTarget = false;
        }

    }

    Vector3 ScreenClampPoint(Rect rect, Vector3 inputPoint, out Vector3 outBoundAmount, float pedding = 0f)
    {
        var newpoint = inputPoint;


        newpoint.x = Mathf.Clamp(newpoint.x, 0 - rect.xMin + pedding, Screen.width - rect.xMax - pedding);
        newpoint.y = Mathf.Clamp(newpoint.y, 0 - rect.yMin + pedding, Screen.height - rect.yMax - pedding);

        outBoundAmount = inputPoint - newpoint;


        return newpoint;
    }

}
