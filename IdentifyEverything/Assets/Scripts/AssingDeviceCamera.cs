using System;
using System.Collections;
using UnityEngine;


public class AssingDeviceCamera : MonoBehaviour
{
    /// <summary>
    /// 相机画面
    /// </summary>
    /// <value>The m web cam texture.</value>
    public WebCamTexture m_WebCamTexture { get; private set; }

    /// <summary>
    /// 由于UIWidget目前还不兼容webcam，故用此方法来进行兼容
    /// </summary>
    public UnityEngine.UI.RawImage m_RawImage;

    /// <summary>
    /// 判断相机是否已经开启
    /// </summary>
    private bool m_CameraIsPlaying;

    /// <summary>
    /// 拍照后的回调
    /// </summary>
    public Action<byte[]> OnTakePhotoCallback;

    /// <summary>
    /// 相机状态发生改变，关闭 or 开启
    /// </summary>
    public Action<bool> OnCameraStateChange;


    private static AssingDeviceCamera Instance;
    /// <summary>
    /// 单例
    /// </summary>
    /// <value>The get assing device camera.</value>
    public static AssingDeviceCamera GetAssingDeviceCamera
    {
        get
        {
            if (Instance == null)
                Instance = FindObjectOfType<AssingDeviceCamera>();
            return Instance;
        }
    }

    /// <summary>
    /// 启动相机
    /// </summary>
    public void StartCamera()
    {
        if (!m_CameraIsPlaying)
            StartCoroutine(InvokeCamera());
        else
            StartCoroutine(TakePhoto());
    }

    /// <summary>
    /// 申请访问调用设备的摄像头
    /// </summary>
    /// <returns>The camera.</returns>
    private IEnumerator InvokeCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (m_WebCamTexture == null)
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                string devicesName = devices[0].name;

                m_WebCamTexture = new WebCamTexture(devicesName, 640, 960, 12)
                {
                    wrapMode = TextureWrapMode.Repeat
                };
                m_RawImage.transform.localScale = new Vector3(-1.7f, 1, 1);
#if !UNITY_EDITOR
                m_RawImage.transform.localEulerAngles = new Vector3(0, 0, 90);
#endif
                m_RawImage.texture = m_WebCamTexture;
            }
            if(!m_WebCamTexture.isPlaying)
                m_WebCamTexture.Play();
            m_CameraIsPlaying = true;

            m_RawImage.gameObject.SetActive(true);
            if(OnCameraStateChange!=null)
            OnCameraStateChange.Invoke(true);
        }
    }


    /// <summary>
    /// 拍照用于识别
    /// </summary>
    /// <returns>The photo.</returns>
    private IEnumerator TakePhoto()
    {
        yield return new WaitForEndOfFrame();

        //截取镜头图片
        int startPoint = (int)(m_WebCamTexture.width * 0.2f);
        int endPoint = (int)(m_WebCamTexture.width * 0.65f);
        Texture2D t2d = new Texture2D(m_WebCamTexture.width, m_WebCamTexture.height, TextureFormat.RGB24, false);
        Color[] colors = m_WebCamTexture.GetPixels(startPoint, 0, endPoint, m_WebCamTexture.height);
        t2d.SetPixels(startPoint, 0, endPoint, t2d.height, colors);
        t2d.Apply(false);


        //清空不再使用的资源释放内存
        m_CameraIsPlaying = false;
        m_WebCamTexture.Stop();
        Destroy(m_WebCamTexture);
        m_WebCamTexture = null;

        if(m_RawImage.texture)
            Destroy(m_RawImage.texture);
        m_RawImage.texture = null;
        m_RawImage.gameObject.SetActive(false);

        byte[] bytes = t2d.EncodeToJPG(50);
        Destroy(t2d);

        GC.Collect();

        //相机状态改变
        if (OnCameraStateChange != null) OnCameraStateChange.Invoke(false);

        //将拍照下来的图片显示到UIWidgets ui上
        if (OnTakePhotoCallback != null) OnTakePhotoCallback.Invoke(bytes);

        //延时上传防止UI卡顿
        yield return new WaitForSeconds(0.25f);
        BaiduAI.GetBaiduAI.AIDetect(bytes);


    }
}
