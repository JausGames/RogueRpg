using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteModifier : MonoBehaviour
{
    [SerializeField] Image image;

    [SerializeField] Texture2D img;
    [SerializeField] Vector3[] quad = new Vector3[] { Vector3.up, Vector3.up + Vector3.right, Vector3.right, Vector3.zero};

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var pt in quad)
            Gizmos.DrawSphere(pt, .2f);
        for(int i = 0; i < quad.Length; i++)
        {
            Gizmos.DrawLine(quad[i], quad[(i+1)%quad.Length]);
        }
    }
    private void OnValidate()
    {
        var modTexture = TestDrawSprite(img, quad);
        var sprite = Sprite.Create(modTexture, new Rect(0, 0, modTexture.width, modTexture.height), new Vector2(0.5f, 0.5f), Mathf.Max(modTexture.width, modTexture.height));
        image.sprite = sprite;
        //spriteRenderer.sprite = sprite;
    }
    Texture2D TestDrawSprite(Texture2D image, Vector3[] quad)
    {
        var A = quad[0];
        var B = quad[1];
        var C = quad[2];
        var D = quad[3];

        float minusX = 0f, minusY = 0f;
        foreach(var pt in quad)
        {
            if (pt.x < minusX) minusX = pt.x;
            if (pt.y < minusY) minusY = pt.y;
        }

        A -= Vector3.right * minusX + Vector3.up * minusY;
        B -= Vector3.right * minusX + Vector3.up * minusY;
        C -= Vector3.right * minusX + Vector3.up * minusY;
        D -= Vector3.right * minusX + Vector3.up * minusY;

        var AB = quad[1] - quad[0];
        var BC = quad[2] - quad[1];
        var CD = quad[3] - quad[2];
        var DA = quad[0] - quad[3];

        var edges = new Vector3[]{ AB, BC, CD, DA };

        var maxEdgeLenght = 0f;
        foreach(var edge in edges)
            if (edge.magnitude > maxEdgeLenght) maxEdgeLenght = edge.magnitude;

        A = (A * image.width) / maxEdgeLenght;
        B = (B * image.width) / maxEdgeLenght;
        C = (C * image.width) / maxEdgeLenght;
        D = (D * image.width) / maxEdgeLenght;

        Texture2D modTexture = new Texture2D(image.width, image.height);

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                modTexture.SetPixel(x, y, new Color(0,0,0,0));
            }
        }

        for (int x = 0; x < image.width; x ++)
        {
            for(int y = 0; y < image.height; y ++)
            {
                //var X = (image.GetPixel(x,y) + xOffset) * xFactor;
                float X = x;
                float Z = y;
                //var Y = points[i].z * height;
                /*var Z = (cell[i].z + zOffset) * zFactor;
                var Y = cell[i].y * 100;*/

                var Q = Vector3.Lerp(A, B, X / image.width);
                var R = Vector3.Lerp(D, C, X / image.width);
                var P = Vector3.Lerp(R, Q, Z / image.height);
                Debug.Log("Draw Sprite : x = " + x + ", P.x = " + Mathf.RoundToInt(P.x));
                Debug.Log("Draw Sprite : y = " + y + ", P.y = " + Mathf.RoundToInt(P.y));
                modTexture.SetPixel(Mathf.RoundToInt(P.x), Mathf.RoundToInt(P.y), image.GetPixel(x, y));
            }

        }

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                if (modTexture.GetPixel(x, y) == null) modTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }

                modTexture.Apply();
        return modTexture;
    }
}
