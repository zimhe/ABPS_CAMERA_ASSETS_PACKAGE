using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldingButton : Selectable
{
    public Image sliderImage;
    public Text text;
    public HoldingButtonSubmitEvent onSubmit;

    public bool sendMessageUpward = true;
    public string message = "SetSelect";


    public bool toggle;
    public KeyCode holdingKeyCode;

    public float duration = 1.0f;

    public float holdingTime;


    bool holdingStarted;
    bool isHolding;
    protected void OnEnable()
    {
        if (text != null)
        {
            text.text = holdingKeyCode.ToString().ToUpper();
        }


    }

    protected void OnValidate()
    {
        if (text != null)
        {
            text.text = holdingKeyCode.ToString().ToUpper();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(holdingKeyCode))
        {
            holdingStarted = true;

        }

        if (Input.GetKey(holdingKeyCode))
        {
            if (holdingStarted)
            {
                isHolding = true;
            }

        }

        if (Input.GetKeyUp(holdingKeyCode))
        {
            holdingStarted = false;
            isHolding = false;
        }

        if (isHolding)
        {
            if (holdingTime < duration)
            {
                holdingTime += Time.deltaTime;
            }
        }
        else
        {
            if (holdingTime > 0)
            {
                holdingTime -= Time.deltaTime;
            }
        }
        if (sliderImage != null)
        {
            sliderImage.fillAmount = holdingTime / duration;
        }


        if (holdingTime > duration)
        {
            toggle = !toggle;
            onSubmit.Invoke(toggle);

            if (sendMessageUpward)
            {
                gameObject.SendMessageUpwards(message, toggle, SendMessageOptions.DontRequireReceiver);
            }


            holdingTime = 0;
            holdingStarted = false;
            isHolding = false;
        }
    }


    public void IsOn(bool value)
    {
        toggle = value;

        onSubmit.Invoke(toggle);

        if (sendMessageUpward)
        {
            gameObject.SendMessageUpwards(message, toggle, SendMessageOptions.DontRequireReceiver);
        }
    }

    [System.Serializable]
    public class HoldingButtonSubmitEvent : UnityEvent<bool>
    {

    }

}
