using System.Collections.Generic;
using UnityEngine;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.foundation;
using Stack = Unity.UIWidgets.widgets.Stack;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.material;
using Color = Unity.UIWidgets.ui.Color;
using Unity.UIWidgets.ui;
using TextStyle = Unity.UIWidgets.painting.TextStyle;


#region Result view
public class ResultfulsWidget : StatefulWidget
{
    public ResultfulsWidget(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new ResultState();
    }
}
public class ResultState : State<ResultfulsWidget>
{
    List<Widget> bodyWidget = new List<Widget>();

    private BaseData baseData;
    string keyword = ApplicationCanvas.language.resultLanguage.title;
    string image_url = ApplicationCanvas.language.resultLanguage.headerImage;
    string description = ApplicationCanvas.language.resultLanguage.descript;
    string baike_url = string.Empty;

    public override void initState()
    {
        base.initState();
        string json = MainScreenfulWidget.resultJson;
        if (string.IsNullOrEmpty(json)) return;
        switch (BaiduAI.GetBaiduAI.detectType)
        {
            case DetectType.Dish:
                baseData = JsonUtility.FromJson<DishData>(json);
                break;
            case DetectType.General:
                baseData = JsonUtility.FromJson<GeneralData>(json);
                break;
            case DetectType.Car:
                baseData = JsonUtility.FromJson<CarData>(json);
                break;
            case DetectType.Logo:
                baseData = JsonUtility.FromJson<LogoData>(json);
                break;
            case DetectType.Animal:
                baseData = JsonUtility.FromJson<AnimalData>(json);
                break;
            case DetectType.Plant:
                baseData = JsonUtility.FromJson<PlantData>(json);
                break;
            case DetectType.Landmark:
                baseData = JsonUtility.FromJson<LandmarkData>(json);
                break;
        }
        if (baseData != null)
        {
            Setdatas();
        }
    }

    public override void dispose()
    {
        base.dispose();
         keyword = ApplicationCanvas.language.resultLanguage.title;
         image_url = ApplicationCanvas.language.resultLanguage.headerImage;
         description = ApplicationCanvas.language.resultLanguage.descript;
         baike_url = string.Empty;
        baseData = null;
        bodyWidget.Clear();
        setState();
        AssingDeviceCamera.GetAssingDeviceCamera.StartCamera();
    }

    public override Widget build(BuildContext context)
    {
        return General(image_url, description, keyword, baike_url);
    }

    private void Setdatas()
    {
        if (baseData is DishData dishData && dishData.result.Count >= 1)
        {
            DishResult result = dishData.result[0];
            keyword = result.name + string.Format("\nCalorie:{0}", result.calorie);
            image_url = result.baike_info.image_url ?? image_url;
            description = result.baike_info.description ?? description;
            baike_url = result.baike_info.baike_url ?? baike_url;
        }
        else if (baseData is CarData carData && carData.result.Count >= 1)
        {
            CarResult result = carData.result[0];
            keyword = result.name + string.Format("\n{0}", result.year);
            image_url = result.baike_info.image_url ?? image_url;
            description = result.baike_info.description ?? description;
            baike_url = result.baike_info.baike_url ?? baike_url;
        }
        else if (baseData is LogoData logoData && logoData.result.Count >= 1)
        {
            LogoResult result = logoData.result[0];
            keyword = result.name;
            image_url = result.baike_info.image_url ?? image_url;
            description = result.baike_info.description ?? description;
            baike_url = result.baike_info.baike_url ?? baike_url;
        }
        else if (baseData is GeneralData generalData && generalData.result.Count >= 1)
        {
            GeneralResult result = generalData.result[0];
            keyword = result.keyword;
            image_url = result.baike_info.image_url ?? image_url;
            description = result.baike_info.description ?? description;
            baike_url = result.baike_info.baike_url ?? baike_url;
        }
        else if (baseData is AnimalData animalData && animalData.result.Count >= 1)
        {
            AnimalResult result = animalData.result[0];
            keyword = result.name;
            image_url = result.baike_info.image_url ?? image_url;
            description = result.baike_info.description ?? description;
            baike_url = result.baike_info.baike_url ?? baike_url;
        }
        else if (baseData is PlantData panltData && panltData.result.Count >= 1)
        {
            PlantResult result = panltData.result[0];
            keyword = result.name;
            image_url = result.baike_info.image_url ?? image_url;
            description = result.baike_info.description ?? description;
            baike_url = result.baike_info.baike_url ?? baike_url;
        }
        else if (baseData is LandmarkData landmarkData && landmarkData.result != null)
        {
            LandmaskResult result = landmarkData.result;
            keyword = string.IsNullOrEmpty(result.landmark) ? keyword : result.landmark;
        }
        setState();
    }


    #region General
    private Widget General(string image_url, string detail, string title, string baike_url)
    {
        bodyWidget.Add(GeneralBaikeDetails(detail));
        if (!string.IsNullOrEmpty(baike_url))
            bodyWidget.Add(GeneralBaikeButton(baike_url));

        return new Container(
                    height: Screen.height,
                    color: new Color(0xfffbfbfb),
                    child: new Column(
                        children: new List<Widget>
                        {
                           _buildHeader(image_url,title),
                           _buildBody(bodyWidget),
                        }
                )
            );
    }

    private Widget GeneralBaikeDetails(string detail)
    {
        return new Container(
                  alignment: detail.Length < 20 ? Alignment.center : Alignment.topLeft,
                  margin: EdgeInsets.all(30),
                  child: new Text(detail, textAlign: TextAlign.left, style: new TextStyle(fontSize: 15, textBaseline: TextBaseline.ideographic, height: 2))
              );
    }

    private Widget GeneralBaikeButton(string baike_url) => new Container(
                child: new Center(
                        child: new FlatButton(
                                    child: new Text(ApplicationCanvas.language.detailsButtonName, style: new TextStyle(color: new Color(0xffffffff))),
                                    shape: new RoundedRectangleBorder(borderRadius: BorderRadius.all(20.0f)),
                                    color: new Color(0xff00BBFF),
                                    onPressed: () =>
                                    {
                                        if (!string.IsNullOrEmpty(baike_url))
                                        {
                                            Application.OpenURL(baike_url);
                                        }
                                    }
                            )
                    )
        );
    #endregion

    private Widget _buildHeader(string image_url, string title)
    {
        int lenght = title.Length;
        int fontSize = 20;
        if (lenght < 5)
            fontSize = 30;
        else if (lenght > 5 && lenght < 12)
            fontSize = 20;
        else if (lenght > 12)
        fontSize = 16;
        if (string.IsNullOrEmpty(title)) Debug.Assert(false, "Title is null or empty");
        return new Container(
                      child: new Stack(
                          alignment: Alignment.center,
                          children: new List<Widget>
                          {
                            new Container(
                                height:200,
                                width:Screen.width,
                                child:Unity.UIWidgets.widgets.Image.network(image_url,fit:BoxFit.cover)
                             ),
                                new Container(
                                decoration:new BoxDecoration(color:Color.fromARGB(100,0,0,0)),
                                width:Screen.width,
                                height:200,
                                child:new Column(
                                        children:new List<Widget>
                                        {
                                             new Container(
                                                margin:EdgeInsets.only(top:50,left:10),
                                                alignment:Alignment.topLeft,
                                                child:new GestureDetector(
                                                        child:new Text(ApplicationCanvas.language.returnButtonName,style:new TextStyle(fontSize:15f,color:new Color(0xfbffffff))),
                                                        onTap:()=>{
                                                            Navigator.pop(context);
                                                            baseData=null;
                                                        }
                                                 )
                                            ),
                                            new Container(
                                                alignment:Alignment.center,
                                                margin:EdgeInsets.only(top:50),
                                                child:new Container(
                                                                    alignment:Alignment.center,
                                                                    child:new Text(title,
                                                                            style:new TextStyle(color:new Color(0xffffffff),fontSize:fontSize,fontWeight:FontWeight.w700),
                                                                            textAlign:TextAlign.center
                                                                         )
                                                          )
                                                ),

                                        }
                                    )
                             ),
                           }
                       )
                 );
    }

    private Widget _buildBody(List<Widget> widgets)
    {
        return new Container(
                 child: new Flexible(
                     child: new ListView(
                         children: widgets
                     )
                 )
             );
    }
}



#endregion

