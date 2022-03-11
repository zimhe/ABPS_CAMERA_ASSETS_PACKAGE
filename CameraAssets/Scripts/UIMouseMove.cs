using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIMouseMove : MonoBehaviour
{
    [SerializeField] RectTransform rect_xform;
    [SerializeField] RectTransform rect_parent;
    [SerializeField] float pedding = 0f;
    [SerializeField] bool snapToBound;
    public float _sensitivity = 1.0f;
    public float _stiffness = 5.0f;

    public Vector3 position;

    Vector3 mousePosition = Vector3.zero;

    private void OnEnable()
    {
        position = rect_xform.localPosition;
    }

    public void SetMousePosition()
    {
        mousePosition = Input.mousePosition;
    }

    public void SetPosition()
    {
       
        var dx = Input.GetAxis("Mouse X") * _sensitivity;
        var dy = Input.GetAxis("Mouse Y") * _sensitivity;

        var newPos = Input.mousePosition;

        var vel = newPos - mousePosition;

        print($"Set Position X:{dx},Y:{dy}");

        position += vel;
        mousePosition = newPos;
    }

    public void SnapPositionToBound()
    {
        if (snapToBound)
        {
            position = PositionToParentBound(rect_parent.rect, rect_xform.rect, position, pedding);
        }
    }

    private void LateUpdate()
    {
        if (rect_xform.gameObject.activeSelf)
        {

            var rect = rect_xform.rect;
            var parent = rect_parent.rect;

            position = ParentClampPoint(parent, rect, position, pedding);


            rect_xform.localPosition = Vector3.Lerp(rect_xform.localPosition, position, Time.deltaTime * _stiffness);

        }
    }

    Vector3 ParentClampPoint(Rect parent_rect,Rect rect, Vector3 inputPoint, float pedding = 0f)
    {

        inputPoint.x = Mathf.Clamp(inputPoint.x, pedding, parent_rect.width - rect.width - pedding);
        inputPoint.y = Mathf.Clamp(inputPoint.y, pedding, parent_rect.height - rect.height - pedding);


        return inputPoint;
    }

    Vector3 PositionToParentBound(Rect parent_rect, Rect rect, Vector3 inputPoint, float pedding = 0f)
    {
       

        float dist_Xmin = inputPoint.x;
        float dist_Xmax = parent_rect.width- inputPoint.x - rect.width;

        float dist_Ymin = inputPoint.y;
        float dist_Ymax= parent_rect.height - inputPoint.y - rect.height;

        if (dist_Xmin < dist_Xmax)
        {
            inputPoint.x = pedding;
        }
        else
        {
            inputPoint.x = parent_rect.width - rect.width - pedding;
        }

        if (dist_Ymin < dist_Ymax)
        {
            inputPoint.y = pedding;
        }
        else
        {
            inputPoint.y= parent_rect.height - rect.height - pedding;
        }

        return inputPoint;
    }


}
