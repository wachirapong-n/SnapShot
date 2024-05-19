using System;
using System.Collections;

using System.IO;
using UnityEngine;


public class SnapShotManager : MonoBehaviour
{

    public Camera ARcamera;
    
    public void Capture()
    {
        StartCoroutine(CaptureScreen());
    }


    IEnumerator CaptureScreen()
    {

        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height = Screen.height;
        RenderTexture rt = new RenderTexture(width, height, 24);
        ARcamera.targetTexture = rt;
        var currentRT = RenderTexture.active;
        RenderTexture.active = ARcamera.targetTexture;

        ARcamera.Render();

        Texture2D image = new Texture2D(width, height);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();

        RenderTexture.active = currentRT;
        Texture2D resizedImg = Resize(image, 256, 256);
        Texture2D croppedImg = ImageCropping(resizedImg, 224, 224);
        byte[] bytes = croppedImg.EncodeToPNG();
        string filename = DateTime.Now.ToString("s") + ".png";
        string filepath = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllBytes(filepath, bytes);

        Destroy(rt);
        Destroy(image);
    }
    private Texture2D Resize(Texture2D texture2D,int targetX,int targetY)
    {
        RenderTexture rt=new RenderTexture(targetX, targetY,24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D,rt);
        Texture2D result=new Texture2D(targetX,targetY);
        result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        result.Apply();
        return result;
    }
    private Texture2D ImageCropping(Texture2D resizedImg, int targetWidth, int targetHeight)
    {
        Texture2D croppedTexture = new Texture2D(targetWidth, targetHeight);
        int offsetX = (resizedImg.width - targetWidth) / 2;
        int offsetY = (resizedImg.height - targetHeight) / 2;
        croppedTexture.SetPixels(resizedImg.GetPixels(offsetX, offsetY, targetWidth, targetHeight));
        croppedTexture.Apply();
        return croppedTexture;
    }
}