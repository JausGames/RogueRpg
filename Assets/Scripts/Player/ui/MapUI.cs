using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MapUi : MonoBehaviour
{
    [SerializeField] protected GameObject canvas;
    protected bool zoom = false;
    protected float width = 0;
    protected float miniMapFade = 0.6f;

    public abstract void SetPlayerPosition(float x, float y, float cameraSize = 1);

    public virtual void SetWholeScreenMap(bool value)
    {
        if (value && !zoom)
        {
            var rectTrans = GetComponent<RectTransform>();
            rectTrans.localPosition = Screen.width * 0.5f * Vector3.right + Screen.height * 0.5f * Vector3.up;

            var background = GetComponent<Image>();
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.8f);
            rectTrans.sizeDelta = Screen.width * Vector3.right + Screen.height * Vector3.up;
            //rectTrans.localPosition = (Screen.width / 2) * Vector2.right + (Screen.height / 2) * Vector2.up;

            zoom = true;
            var canvasTrans = canvas.GetComponent<RectTransform>();
            canvasTrans.sizeDelta = Screen.width * Vector3.right + Screen.height * Vector3.up;
            //canvasTrans.localPosition = Vector3.zero;

            List<Image> images = new List<Image>();
            images.Add(transform.Find("playerMark").GetComponent<Image>());
            images.AddRange(transform.Find("Canvas").GetComponentsInChildren<Image>());

            foreach (Image img in images)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
            }
        }
        else if (zoom && !value)
        {
            var rectTrans = GetComponent<RectTransform>();
            rectTrans.localPosition = (Screen.width * 0.5f - 20f) * Vector3.right + (Screen.height * 0.5f - 20f) * Vector3.up;
            var background = GetComponent<Image>();
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.2f);
            rectTrans.sizeDelta = 2f * width * Vector3.right + 2f * width * Vector3.up;
            //rectTrans.localPosition = (Screen.width / 2) * Vector2.right + (Screen.height / 2) * Vector2.up;

            zoom = false;
            var canvasTrans = canvas.GetComponent<RectTransform>();
            canvasTrans.sizeDelta = 2f * width * Vector3.right + 2f * width * Vector3.up;
            //canvasTrans.localPosition = Vector3.zero;

            List<Image> images = new List<Image>();
            images.Add(transform.Find("playerMark").GetComponent<Image>());
            images.AddRange(transform.Find("Canvas").GetComponentsInChildren<Image>());

            foreach (Image img in images)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, miniMapFade);
            }
        }
    }
}
