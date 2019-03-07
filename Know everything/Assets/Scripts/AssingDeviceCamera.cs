using System.Collections;
using UnityEngine;


public class AssingDeviceCamera : MonoBehaviour
{
    public Camera m_Camera;
    public WebCamTexture m_WebCamTexture { get; private set; }
    public UnityEngine.UI.RawImage m_RawImage;

    private bool m_CameraIsPlaying;
    public System.Action<byte[],Unity.UIWidgets.ui.Window> OnTakePhotoCallback;
    private static AssingDeviceCamera Instance;
    public static AssingDeviceCamera GetAssingDeviceCamera
    {
        get
        {
            if (Instance == null)
                Instance = FindObjectOfType<AssingDeviceCamera>();
            return Instance;
        }
    }


    public void StartCamera(Unity.UIWidgets.ui.Window instance)
    {
        if(!m_CameraIsPlaying)
            StartCoroutine(InvokeCamera());
        else
            StartCoroutine(TakePhoto(instance));
    }

        
    private IEnumerator InvokeCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (m_WebCamTexture == null)
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                string devicesName = devices[0].name;

                m_WebCamTexture = new WebCamTexture(devicesName, Screen.width, Screen.height, 60)
                {
                    wrapMode = TextureWrapMode.Repeat
                };
                m_RawImage.transform.localScale = new Vector3(-1.7f, 1, 1);
                m_RawImage.transform.localEulerAngles = new Vector3(0, 0, 90);
                m_RawImage.texture = m_WebCamTexture;
            }

            m_WebCamTexture.Play();
            m_CameraIsPlaying = true;
            m_RawImage.gameObject.SetActive(true);
        }
    }

    private IEnumerator TakePhoto(Unity.UIWidgets.ui.Window instance)
    {
        yield return new WaitForEndOfFrame();

        //Screen shot
        Texture2D t2d = new Texture2D(m_WebCamTexture.width, m_WebCamTexture.height, TextureFormat.RGB24, false);
        Color[] colors = m_WebCamTexture.GetPixels(0,0, m_WebCamTexture.width, m_WebCamTexture.height);
        t2d.SetPixels(colors);
        t2d.Apply(false);
        t2d.Compress(true);
        byte[] bytes = t2d.EncodeToJPG(80);

        //Destroy camera and texture
        m_CameraIsPlaying = false;
        m_WebCamTexture.Pause();

        //Destroy(m_WebCamTexture);
        //m_WebCamTexture = null;

        if (OnTakePhotoCallback != null) 
        {
            OnTakePhotoCallback.Invoke(bytes, instance);
        }

        m_RawImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
         BaiduAI.GetBaiduAI.AIDetect(bytes, instance);
    }
}
