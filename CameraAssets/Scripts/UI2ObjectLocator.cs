using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI2ObjectLocator : MonoBehaviour
{
    [SerializeField] Canvas rootCanvas;
    [SerializeField] RectTransform uiXform;
    [SerializeField] Transform targetXform;
    [SerializeField] RectTransform startPoint;
    [SerializeField] RectTransform endPoint;
    [SerializeField] RectTransform line;
    [SerializeField] float pedding = 0f;
    [SerializeField] bool clampRectPos;
    [SerializeField] float scaleMax = 1.0f;
    [SerializeField] float scaleMin = 0.3f;

    float currentScale = 1.0f;

    Vector3[] linePositions;
    private void OnEnable()
    {
        linePositions = new Vector3[4];
      
    }

    Vector3 RectPointIntersect(RectTransform xform, Vector3 point)
    {
        Vector2 p;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(xform, point, null, out p);

        p.x = Mathf.Clamp(p.x, -0.5f*xform.sizeDelta.x,  0.5f * xform.sizeDelta.x);
        p.y = Mathf.Clamp(p.y, -0.5f * xform.sizeDelta.y, 0.5f * xform.sizeDelta.y);

        return new Vector3(p.x,p.y);
      
    }

    void UpdateLine(Vector3 dir)
    {
        float length = dir.magnitude;

        float angle = Vector3.SignedAngle(Vector3.down, dir,Vector3.forward);

        var euler = line.eulerAngles;
        euler.z = angle;
        line.eulerAngles = euler;

        var size = line.sizeDelta;

        size.y = length;

        line.sizeDelta = size;

    }

    Camera GetCurrentCamera
    {
        get
        {

            return MultiCameraManager.Instance==null?Camera.main: MultiCameraManager.Instance.GetCurrentCamera;
        }
    }

    private void LateUpdate()
    {
        var cam = GetCurrentCamera;

 
        var screenPoint = cam.WorldToScreenPoint(targetXform.position);

        if (screenPoint.z < 0)
        {
            screenPoint.x = (Screen.width - screenPoint.x);
            screenPoint.y = 0;
        }

        if (endPoint != null)
        {
            endPoint.position = screenPoint;
        }

        if (uiXform.gameObject.activeSelf)
        {

            var rect = uiXform.rect;

           

            var offset = GetVectorOffset(screenPoint);
            Vector3 outBound = Vector3.zero;

            var position = ScreenClampPoint(rect, screenPoint+offset,out outBound, pedding);


            uiXform.position = Vector3.Lerp(uiXform.position, position, Time.deltaTime * 10);

            startPoint.localPosition = RectPointIntersect(uiXform, screenPoint);


            Vector3 dir = endPoint.position - startPoint.position;

            UpdateLine(dir);
        }
    }

    Vector3 GetVectorOffset(Vector3 inputPoint, float offsetX = 300f, float offsetY = 300f)
    {
        float x;
        float y;

        


        if (inputPoint.y < 0.5f * Screen.height)
        {
            y = offsetY;
            x = 0;
          
        }
        else
        {
            y = 0;

            if (inputPoint.x > 0.5f * Screen.width)
            {
                x = -offsetX;
            }
            else
            {
                x = offsetX;

            }

        }


        return new Vector3(x, y);
    }

    Vector3 ScreenClampPoint(Rect rect, Vector3 inputPoint, out Vector3 outBoundAmount, float pedding = 0f)
    {
        var newpoint = inputPoint;


        newpoint.x = Mathf.Clamp(newpoint.x, 0 - rect.xMin + pedding, Screen.width - rect.xMax - pedding);
        newpoint.y = Mathf.Clamp(newpoint.y, 0 - rect.yMin + pedding, Screen.height - rect.yMax - pedding);

        outBoundAmount = inputPoint - newpoint;


        return newpoint;
    }

    Vector3 ScreenCenter
    {
        get
        {
            return new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        }
    }

    private void OnGUI()
    {
        
    }


}
