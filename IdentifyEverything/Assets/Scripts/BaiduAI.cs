using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public enum DetectType
{
    Dish,
    Car,
    General,
    Logo,
    Animal,
    Plant,
    Landmark
}
public class BaiduAI : MonoBehaviour
{
    private static BaiduAI instance;
    public static BaiduAI GetBaiduAI
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<BaiduAI>();
            return instance;
        }
    }

    // 设置APPID/AK/SK
    public string APP_ID = "你的 App ID";
    public string API_KEY = "你的 Api Key";
    public string SECRET_KEY = "你的 Secret Key";



    public string ResultString { get; set; }
    public System.Action<string, string> OnResultCallback;
    public DetectType detectType;
    public Thread thread;
    public Baidu.Aip.ImageClassify.ImageClassify client;
    public ManualResetEvent manual = new ManualResetEvent(false);
    private AIDetectThread detect;


    private void Start()
    {
        client = new Baidu.Aip.ImageClassify.ImageClassify(API_KEY, SECRET_KEY)
        {
            Timeout = 60000
        };
        detect = new AIDetectThread();
    }
    private void OnDisable()
    {
        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }

        detect.bytes = null;
        client = null;
    }

    public void AIDetect(byte[] bytes)
    {

        detect.bytes = bytes;

        thread = new Thread(detect.AIDetect);
        thread.Start();
    }
}


public struct AIDetectThread
{
    public byte[] bytes;


    public void AIDetect()
    {
        Newtonsoft.Json.Linq.JObject Result = null;
        try
        {
            Dictionary<string, object> options = new Dictionary<string, object> { };
            options.Clear();
            switch (BaiduAI.GetBaiduAI.detectType)
            {
                case DetectType.General:
                    options.Add("baike_num", 5);
                    Result = BaiduAI.GetBaiduAI.client.AdvancedGeneral(bytes, options);
                    break;
                case DetectType.Dish:
                    options.Add("baike_num", 5);
                    options.Add("filter_threshold", 0.7f);

                    Result = BaiduAI.GetBaiduAI.client.DishDetect(bytes, options);
                    break;
                case DetectType.Car:
                    options.Add("baike_num", 5);
                    options.Add("top_num", 1);
                    Result = BaiduAI.GetBaiduAI.client.CarDetect(bytes, options);
                    break;
                case DetectType.Logo:
                    Result = BaiduAI.GetBaiduAI.client.LogoSearch(bytes);
                    break;
                case DetectType.Animal:
                    options.Add("baike_num", 5);
                    options.Add("top_num", 6);
                    Result = BaiduAI.GetBaiduAI.client.AnimalDetect(bytes, options);
                    break;
                case DetectType.Plant:
                    options.Add("baike_num", 5);
                    Result = BaiduAI.GetBaiduAI.client.PlantDetect(bytes, options);
                    break;
                case DetectType.Landmark:
                    Result = BaiduAI.GetBaiduAI.client.Landmark(bytes);
                    break;
            }
            if (BaiduAI.GetBaiduAI.OnResultCallback != null) BaiduAI.GetBaiduAI.OnResultCallback.Invoke(BaiduAI.GetBaiduAI.ResultString, Result.ToString());
            BaiduAI.GetBaiduAI.thread.Abort();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            BaiduAI.GetBaiduAI.thread.Abort();
        }
    }

}
