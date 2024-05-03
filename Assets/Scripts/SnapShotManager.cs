using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnapShotManager : MonoBehaviour
{

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
        GetComponent<Camera>().targetTexture = rt;
        var currentRT = RenderTexture.active;
        RenderTexture.active = GetComponent<Camera>().targetTexture;

        GetComponent<Camera>().Render();

        Texture2D image = new Texture2D(width, height);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();

        RenderTexture.active = currentRT;

        byte[] bytes = image.EncodeToPNG();
        string filename = DateTime.Now.ToString("s") + ".png";
        string filepath = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllBytes(filepath, bytes);

        Destroy(rt);
        Destroy(image);
    }
}