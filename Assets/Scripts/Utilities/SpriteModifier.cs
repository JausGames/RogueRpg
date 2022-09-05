using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteModifier : MonoBehaviour
{
    [SerializeField] Image image;

    [SerializeField] Texture2D img;
    Vector3[] quad = new Vector3[] { Vector3.up, Vector3.up + Vector3.right, Vector3.right, Vector3.zero };

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
    private void Awake()
    {
        var modTexture = TestDrawSprite(img, quad);
        var sprite = Sprite.Create(modTexture, new Rect(0, 0, modTexture.width, modTexture.height), new Vector2(0.5f, 0.5f), Mathf.Max(modTexture.width, modTexture.height));
        image.sprite = sprite;
        //spriteRenderer.sprite = sprite;
    }
    Texture2D TestDrawSprite(Texture2D image, Vector3[] quad)
    {
        var A = quad[0] * image.width;
        var B = quad[1] * image.width;
        var C = quad[2] * image.width;
        var D = quad[3] * image.width;

        Texture2D modTexture = new Texture2D(image.width, image.height);

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
