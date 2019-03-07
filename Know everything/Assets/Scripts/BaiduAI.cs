using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    // 设置APPID/AK/SK
    public string APP_ID = "你的 App ID";
    public string API_KEY = "你的 Api Key";
    public string SECRET_KEY = "你的 Secret Key";


    private Baidu.Aip.ImageClassify.ImageClassify client;

    public static string ResultString { get; private set; }
    public static System.Action<string,Unity.UIWidgets.ui.Window> OnResultCallback;
    public static DetectType detectType;

    // Start is called before the first frame update
    void Start()
    {
        client = new Baidu.Aip.ImageClassify.ImageClassify(API_KEY, SECRET_KEY)
        {
            Timeout = 60000  // 修改超时时间
        };
    }

    public IEnumerator AIDetect(byte[] bytes,Unity.UIWidgets.ui.Window window)
    {
        yield return null;
        Newtonsoft.Json.Linq.JObject Result=null;
        try
        {
            Dictionary<string, object> options = new Dictionary<string, object>{};
            options.Clear();

            switch (detectType)
            {
                case DetectType.General:
                    options.Add("baike_num", 5);
                    Result = client.AdvancedGeneral(bytes, options);
                    break;
                case DetectType.Dish:
                    options.Add("baike_num", 5);
                    options.Add("filter_threshold", 0.7f);

                    Result = client.DishDetect(bytes, options);
                    break;
                case DetectType.Car:
                    options.Add("baike_num", 5);
                    options.Add("top_num", 1);
                    Result = client.CarDetect(bytes, options);
                    break;
                case DetectType.Logo:
                    Result = client.LogoSearch(bytes);
                    break;
                case DetectType.Animal:
                    options.Add("baike_num", 5);
                    options.Add("top_num", 6);
                    Result = client.AnimalDetect(bytes, options);
                    break;
                case DetectType.Plant:
                    options.Add("baike_num", 5);
                    Result = client.PlantDetect(bytes, options);
                    break;
                case DetectType.Landmark:
                    Result = client.Landmark(bytes);
                    break;
            }

            ResultString = Result.ToString();
            if (OnResultCallback != null) OnResultCallback.Invoke(ResultString,window);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}
