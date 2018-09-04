using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class PhotoController : MonoBehaviour
{
    public GameObject photoview;
    public GameObject[] hidenUI;

    public Texture2D tex;
    public AudioSource auos;
    public Image photoResult;

    string URL = "http://45.77.169.132:9000/v1/beautylook/post";
    string ScreenshotName = "Screenshot.png";

    public void TakeaPhoto()
    {
        auos.Play();

        for (int i = 0; i < hidenUI.Length; i++)
        {
            hidenUI[i].SetActive(false);
        }

        StartCoroutine(Screenshot());

    }

    public void TakeaPicture()
    {
        StartCoroutine(capture());
    }

    IEnumerator capture()
    {
        yield return new WaitForEndOfFrame();
        string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
        ScreenCapture.CaptureScreenshot(ScreenshotName);
    }

    public void UploadPhoto()
    {
        StartCoroutine(UploadAFile(URL));
    }

    //Our coroutine takes a screenshot of the game
    public IEnumerator UploadAFile(string uploadUrl)
    {
        byte[] bytes = tex.EncodeToPNG(); //Can also encode to jpg, just make sure to change the file extensions down below
        Destroy(tex);

        // Create a Web Form, this will be our POST method's data
        var form = new WWWForm();
        //form.AddField("somefield", "somedata");
        form.AddBinaryData("image", bytes, "screenshot.png", "image/png");

        Dictionary<string, string> headers = form.headers;
        headers["usertoken"] = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MTczLCJ1c2VybmFtZSI6InNoaW50YXZyIiwicm9sZSI6ImNvbW11bml0eSIsImlhdCI6MTUzMjQxNTU1M30.fw9lwg9_8vIhQjFulZ65PfwP-IAoFZsbb2hjxGDPmuY";

        //POST the screenshot to GameSparks
        WWW w = new WWW(uploadUrl, form.data, headers);
        yield return w;

        if (w.error != null)
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log(w.text);
        }

    }

    public IEnumerator Screenshot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoResult.sprite = mySprite;

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < hidenUI.Length; i++)
        {
            hidenUI[i].SetActive(true);
        }
        photoview.SetActive(true);
    }
}