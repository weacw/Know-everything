using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Multi language")]
public class MultiLanguage : ScriptableObject
{
    public enum Language
    {
        CN,
        US
    }

    public Language languageType;
    public string returnButtonName;
    public string scanObjectButtonName;
    public string selectPhotoButtonName;
    public string detailsButtonName;
    public ResultLanguage resultLanguage;


}


    [System.Serializable]
public class ResultLanguage
{
    public string title;
    public string descript;
    public string headerImage;
    public List<string> detectType = new List<string>();
    public string calorie;
}
