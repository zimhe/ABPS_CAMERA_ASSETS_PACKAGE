using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManagerRemote : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite[] cameraSprites;
    // Start is called before the first frame update
    void Start()
    {
        if (button == null)
        {
            return;
        }
        button.onClick.AddListener(() => CycleCamera());
        CheckIcon();

        if (MultiCameraManager.Instance != null)
        {
            MultiCameraManager.Instance.OnCameraChanged += new System.EventHandler(delegate { CheckIcon(); });
        }
    }

   

    public void CycleCamera()
    {
        if (MultiCameraManager.Instance != null)
        {
            MultiCameraManager.Instance.CycleCameras();
        }
    }

    public void SecActiveCamera(int index)
    {
        if (MultiCameraManager.Instance != null)
        {
            MultiCameraManager.Instance.SetActiveCamera(index);
        }
    }

    public void CheckIcon()
    {
        if (MultiCameraManager.Instance != null)
        {
            //MultiCameraManager.Instance.CycleCameras();
            int active = MultiCameraManager.Instance.ActiveIndex;

            if (active != -1 && active < cameraSprites.Length && buttonImage != null)
            {
                buttonImage.sprite = cameraSprites[active];

            }

        }
    }
}
