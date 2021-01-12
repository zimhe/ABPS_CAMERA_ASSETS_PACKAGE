using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI2ObjectLocator : MonoBehaviour
{
    [SerializeField] RectTransform uiXform;
    [SerializeField] Transform targetXform;
    [SerializeField] RectTransform indicator;
    [SerializeField] float pedding = 0f;
    [SerializeField] bool clampRectPos;
    [SerializeField] float scaleMax = 1.0f;
    [SerializeField] float scaleMin = 0.3f;

    float currentScale = 1.0f;

    MultiCameraManager _camManager;

    MultiCameraManager CamManager
    {
        get
        {
            if (_camManager == null)
            {
                _camManager = GameObject.FindObjectOfType<MultiCameraManager>();
            }
            return _camManager;
        }
    }




    private void LateUpdate()
    {
        if (uiXform.gameObject.activeSelf)
        {
            var cam = CamManager.GetCurrentCamera;

            var screenPoint = cam.WorldToScreenPoint(targetXform.position);

            float far = cam.farClipPlane;

            var rect = uiXform.rect;

            if (screenPoint.z < 0)
            {
                screenPoint.x = (Screen.width - screenPoint.x);
                screenPoint.y = 0;
            }

            Vector3 outBound=Vector3.zero;

           var position = ScreenClampPoint(rect, screenPoint, out outBound, pedding);



            float t = (50f-screenPoint.z) / 50f;


            currentScale = Mathf.Lerp(scaleMin, scaleMax, t);
            uiXform.localScale = Vector3.one * currentScale;
            uiXform.position = position;


            var pos = new Vector3(outBound.x, outBound.y);

            pos.x = Mathf.Clamp(pos.x, rect.xMin, rect.xMax);
            pos.y = Mathf.Clamp(pos.y, rect.yMin, rect.yMax);

            if (indicator == null)
            {
                return;
            }

            if (pos.x == rect.xMin || pos.x == rect.xMax||pos.y==rect.yMin||pos.y==rect.yMax)
            {
                indicator.localPosition = pos;
                return;
            }
            else
            {
                float vx = pos.x - rect.xMin;
                float vy = pos.y - rect.yMin;

                float px = vx / rect.width;
                float py = vy / rect.height;

                if (px > py)
                {
                    pos.y = pos.y > rect.center.y ? rect.yMax : rect.yMin;
                }

                if (px<py)
                {
                    pos.x = pos.x > rect.center.x ? rect.xMax : rect.xMin;
                }

                indicator.localPosition = pos;

              

               
            }

        }
    }

    Vector3 ScreenClampPoint(Rect rect, Vector3 inputPoint,out Vector3 outBoundAmount,float pedding=0f)
    {
        var newpoint = inputPoint;


        newpoint.x = Mathf.Clamp(newpoint.x, 0-rect.xMin + pedding, Screen.width-rect.xMax - pedding);
        newpoint.y = Mathf.Clamp(newpoint.y, 0-rect.yMin+ pedding, Screen.height -rect.yMax - pedding);

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

   
}
