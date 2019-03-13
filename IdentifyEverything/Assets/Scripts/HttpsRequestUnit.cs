using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HttpsRequestUnit : MonoBehaviour
{
    internal Action<string> failed;
    internal Action<string> successed;

    private static HttpsRequestUnit instance;
    public static HttpsRequestUnit GetHttpsRequest
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<HttpsRequestUnit>();
            return instance;
        }
    }


    internal IEnumerator Request(string _requestUrl, string _postMethod, int _timeout)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest(_requestUrl, _postMethod) { timeout = _timeout };
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError)
        {
            if (successed != null)
                successed.Invoke(unityWebRequest.downloadHandler.text);
        }
        else
        {
            //TODO:Error
            if (failed != null)
                failed.Invoke(unityWebRequest.error);
        }
    }
}
