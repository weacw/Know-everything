using System.Collections.Generic;

[System.Serializable]
public class BaseData
{
    public long log_id;
}


[System.Serializable]
public class Baike_info
{
    public string baike_url;
    public string image_url;
    public string description;
}



#region General data
[System.Serializable]
public class GeneralResult
{
    public double score;
    public string root;
    public Baike_info baike_info;
    public string keyword;
}


[System.Serializable]
public class GeneralData: BaseData
{
    public int result_num;
    public List<GeneralResult> result;
}
#endregion


#region Dish data
[System.Serializable]
public class DishResult
{
    public string name;
    public float calorie;
    public bool has_calorie;
    public float probability;
    public Baike_info baike_info;
}

[System.Serializable]
public class DishData: BaseData
{
    public int result_num;
    public List<DishResult> result;
}
#endregion


#region Logo data
[System.Serializable]
public class LogoResult
{
    public int type;
    public string name;
    public Dictionary<string,int> location;
    public float probability;
    public Baike_info baike_info;
}

[System.Serializable]
public class LogoData: BaseData
{
    public int result_num;
    public List<LogoResult> result;
}
#endregion


#region Plant data

[System.Serializable]
public class PlantResult
{
    public string name;
    public float score;
    public Baike_info baike_info;
}

[System.Serializable]
public class PlantData: BaseData
{
    public int result_num;
    public List<PlantResult> result;
}
#endregion



#region Animal data
[System.Serializable]
public class AnimalResult
{
    public string name;
    public float score;
    public Baike_info baike_info;
}

[System.Serializable]
public class AnimalData: BaseData
{
    public List<AnimalResult> result;
}
#endregion


#region Car data
[System.Serializable]
public class CarResult
{
    public string name;
    public float score;
    public string year;
    public Baike_info baike_info;
}

[System.Serializable]
public class CarData: BaseData
{
    public Location_result location_result;
    public List<CarResult> result;
    public string color_result;
}

[System.Serializable]
public class Location_result
{
    public int width;
    public int top;
    public int height;
    public int left;
}
#endregion



#region Landmark data
[System.Serializable]
public class LandmaskResult
{
    public string landmark;
}

[System.Serializable]
public class LandmarkData: BaseData
{
    public LandmaskResult result;
}
#endregion