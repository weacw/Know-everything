using System.Collections;
using UnityEngine;

public class AssingDeviceCamera : MonoBehaviour, ICommandBase
{
    public WebCamTexture GetWebCamTexture { get; private set; }
    private IEnumerator AssingCameraCoroutinue;
    private bool inited;



    public void Excute()
    {
        if (!inited)
        {
            inited = true;
            AssingCameraCoroutinue = AssingCamera();
        }
        StartCoroutine(AssingCameraCoroutinue);
    }


    /// <summary>
    /// 申请访问设备相机、开启相机
    /// </summary>
    /// <returns>The camera.</returns>
    private IEnumerator AssingCamera()
    {
        //申请设备相机权限
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (GetWebCamTexture == null)
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                string tmp_IndexDeviceName = devices[0].name;
                GetWebCamTexture = new WebCamTexture(tmp_IndexDeviceName, 320, 480, 30)
                {
                    wrapMode = TextureWrapMode.Repeat
                };
                if (!GetWebCamTexture.isPlaying)
                    GetWebCamTexture.Play();
            }
        }
        else 
        { 
            //TODO:用户拒绝相机访问，提示
        }
        StopCoroutine(AssingCameraCoroutinue);
    }
}
