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
        if (target == null)
        {
            locator = GameObject.FindObjectOfType<FollowTargetLocator>();
            target = locator.GetTarget;
        }

        cam = Camera.main;
    }

    // Update is called once per frame
   
    public void GoToTarget()
    {
        pan.SetPosition(target.position);
    }

    public void ToggleStickToTarget()
    {
        stickToTarget = !stickToTarget;
    }


    private void Update()
    {
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
