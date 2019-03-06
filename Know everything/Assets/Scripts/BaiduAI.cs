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
    public string path;
    public Newtonsoft.Json.Linq.JObject Result { get; private set; }

    public static System.Action OnResultCallback;
    public static DetectType detectType;

    // Start is called before the first frame update
    void Start()
    {
        client = new Baidu.Aip.ImageClassify.ImageClassify(API_KEY, SECRET_KEY)
        {
            Timeout = 60000  // 修改超时时间
        };
    }

    public IEnumerator AIDetect(byte[] bytes)
    {
        yield return null;
        //try
        {
            switch (detectType)
            {
                case DetectType.General:
                    Result = client.AdvancedGeneral(bytes);
                    break;
                case DetectType.Dish:
                    Result = client.DishDetect(bytes);
                    break;
                case DetectType.Car:
                    Result = client.CarDetect(bytes);
                    break;
                case DetectType.Logo:
                    Result = client.LogoSearch(bytes);
                    break;
                case DetectType.Animal:
                    Result = client.AnimalDetect(bytes);
                    break;
                case DetectType.Plant:
                    Result = client.PlantDetect(bytes);
                    break;
                case DetectType.Landmark:
                    Result = client.Landmark(bytes);
                    break;
            }

            Debug.Log(Result);
            if (OnResultCallback != null) OnResultCallback.Invoke();
            //var tmp_result = result.ToObject<Result>();
        }
        //catch (System.Exception ex)
        //{
        //    Debug.LogError(ex.Message);
        //}
    }
}
