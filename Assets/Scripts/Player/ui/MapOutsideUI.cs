using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOutsideUI : MapUi
{
    private void Awake()
    {
        width = GetComponent<RectTransform>().rect.width / 2f;

        var canvasTrans = canvas.GetComponent<RectTransform>();
        canvasTrans.sizeDelta = 2f * width * Vector3.right + 2f * width * Vector3.up;
    }
    public override void SetPlayerPosition(float x, float y, float cameraSize = 1)
    {
        //a bit too far
        if (!zoom) canvas.GetComponent<RectTransform>().localPosition = new Vector3((-x / (cameraSize + 8f)) * width * 2f - width, (-y / (cameraSize + 8f)) * width * 2f - width);
        else canvas.GetComponent<RectTransform>().localPosition = new Vector3(-x * Screen.width - Screen.width * 0.5f, -y * Screen.width - Screen.height * 0.5f);

    }
}
