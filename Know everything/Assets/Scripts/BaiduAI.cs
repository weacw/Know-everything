using System.Collections;
using System.Collections.Generic;
using System.IO;
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



    public  string ResultString { get; set; }
    public  System.Action<string, Unity.UIWidgets.ui.Window> OnResultCallback;
    public  DetectType detectType;
    public  Thread thread;
    public  Baidu.Aip.ImageClassify.ImageClassify client;

    private void Start()
    {
        client = new Baidu.Aip.ImageClassify.ImageClassify(API_KEY, SECRET_KEY)
        {
            Timeout = 60000  // 修改超时时间
        };
    }

    public  void AIDetect(byte[] bytes, Unity.UIWidgets.ui.Window window)
    {
        AIDetectThread detect = new AIDetectThread();
        thread = new Thread((obj) => {
                detect.AIDetect(bytes, window);
          });
        thread.Start();
    }

}


public class AIDetectThread
{
    public void AIDetect(byte[] bytes, Unity.UIWidgets.ui.Window window)
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

            BaiduAI.GetBaiduAI.ResultString = Result.ToString();
            if (BaiduAI.GetBaiduAI.OnResultCallback != null) BaiduAI.GetBaiduAI.OnResultCallback.Invoke(BaiduAI.GetBaiduAI.ResultString, window);
            BaiduAI.GetBaiduAI.thread.Abort();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

}
