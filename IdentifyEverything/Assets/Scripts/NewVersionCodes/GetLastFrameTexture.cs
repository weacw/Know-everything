using UnityEngine;
using System.Collections;

public class GetLastFrameTexture :ICommandBase
{
    private WebCamTexture webCamTexture;
    private IEnumerator GetLastFrameCoroutinue;
    private bool inited;
    private MonoBehaviour mono;
    private ICommandBase OnGetLastFrameCallback;

    public GetLastFrameTexture(WebCamTexture _webCamTexture,MonoBehaviour _mono,ICommandBase _onGetLastFrameCallback)
    {
        this.webCamTexture = _webCamTexture;
        if (!inited)
            GetLastFrameCoroutinue = GetLastFrame();
        this.mono = _mono;
        this.OnGetLastFrameCallback = _onGetLastFrameCallback;
        inited = true;
    }

    public void Excute()
    {
        this.mono.StartCoroutine(GetLastFrameCoroutinue);
    }


    private IEnumerator GetLastFrame()
    {
        yield return new WaitForEndOfFrame();

        //截取镜头图片
        int startPoint = (int)(webCamTexture.width * 0.2f);
        int endPoint = (int)(webCamTexture.width * 0.65f);

        Texture2D t2d = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
        Color[] colors = webCamTexture.GetPixels(startPoint, 0, endPoint, webCamTexture.height);
        t2d.SetPixels(startPoint, 0, endPoint, t2d.height, colors);
        t2d.Apply(false);


        webCamTexture.Stop();
        Object.Destroy(webCamTexture);
        this.mono.StopCoroutine(GetLastFrameCoroutinue);
    }
}
