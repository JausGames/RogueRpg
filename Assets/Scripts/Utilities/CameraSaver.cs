using System.IO;
using UnityEngine;

public class CameraSaver
{
    public Texture2D CameraToTexure(Camera cam)
    {
        /*RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        cam.Render();

        Texture2D Image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        Image.Apply();
        return Image;*/
        /*RenderTexture outputMap = new RenderTexture(textureSize, textureSize, 32);
        outputMap.name = "Whatever";
        outputMap.enableRandomWrite = true;
        outputMap.Create();
        //Put the above stuff in Awake()  if you need to update this every frame...
        RenderTexture.active = outputMap;
        GL.Clear(true, true, Color.black);
        Graphics.Blit(mainTexture, outputMap, rtMat);*/

        RenderTexture rendText = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        cam.Render();


        Texture2D cameraImage = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.ARGB32, false, true);
        cameraImage.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        cameraImage.Apply();
        //RenderTexture.active = rendText;

        return cameraImage;
    }

    public Sprite CameraToSprite(Camera cam)
    {
        var texture = CameraToTexure(cam);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), Mathf.Max(texture.width, texture.height));
    }

}