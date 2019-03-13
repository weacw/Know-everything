using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Baidu.Aip.ImageClassify;
using System;
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
public class AIDetectThread:ICommandBase
{
    readonly private ImageClassify Client;
    readonly private byte[] Bytes;
    readonly private DetectType CurDetectType;

    public AIDetectThread(byte[] _bytes, DetectType _detectType)
    {
        Bytes = _bytes;
        CurDetectType = _detectType;
        Client = new ImageClassify(APIInfo.API_KEY, APIInfo.SECRET_KEY)
        {
            Timeout = 60000
        };

    }

   public void Excute()
    {
        JObject tmp_Result = null;
        Dictionary<string, object> tmp_Options = new Dictionary<string, object> { };
        tmp_Options.Clear();

        try
        {
            switch (CurDetectType)
            {
                case DetectType.Dish:
                    tmp_Options.Add("baike_num", 5);
                    tmp_Options.Add("top_num", 1);
                    tmp_Result = Client.CarDetect(Bytes, tmp_Options);

                    break;
                case DetectType.Car:
                    tmp_Options.Add("baike_num", 5);
                    tmp_Options.Add("filter_threshold", 0.7f);
                    tmp_Result = Client.DishDetect(Bytes, tmp_Options);
                    break;
                case DetectType.General:
                    tmp_Options.Add("baike_num", 5);
                    tmp_Result = Client.AdvancedGeneral(Bytes, tmp_Options);
                    break;
                case DetectType.Logo:
                    tmp_Result = Client.LogoSearch(Bytes);
                    break;
                case DetectType.Animal:
                    tmp_Options.Add("baike_num", 5);
                    tmp_Options.Add("top_num", 6);
                    tmp_Result = Client.AnimalDetect(Bytes, tmp_Options);
                    break;
                case DetectType.Plant:
                    tmp_Options.Add("baike_num", 5);
                    tmp_Result = Client.PlantDetect(Bytes, tmp_Options);
                    break;
                case DetectType.Landmark:
                    tmp_Result = Client.Landmark(Bytes);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}


public class APIInfo
{
    public const string API_KEY = "lkvNjzSEZfDjYhzz4YTuHGRB";
    public const string SECRET_KEY = "qbnH4vDQvb331W7Mh70NwRjHGhZippCV";
}