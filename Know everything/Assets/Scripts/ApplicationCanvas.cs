using System.Collections.Generic;
using UnityEngine;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.foundation;
using Stack = Unity.UIWidgets.widgets.Stack;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.material;
using Color = Unity.UIWidgets.ui.Color;
using Unity.UIWidgets.ui;
using Transform = Unity.UIWidgets.widgets.Transform;
using TextStyle = Unity.UIWidgets.painting.TextStyle;
using Rect = Unity.UIWidgets.ui.Rect;
using Unity.UIWidgets.animation;
using Material = Unity.UIWidgets.material.Material;
using System;

public class ApplicationCanvas : UIWidgetsPanel
{
    private static MultiLanguage instance;
    public static MultiLanguage language
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<MultiLanguage>("CN");
                Debug.Assert(instance != null, "language.instance is null.");
            }
            return instance;
        }
    }

    protected override Widget createWidget()
    {
        return new WidgetsApp(
                initialRoute: "/",
                pageRouteBuilder: this.pageRouteBuilder,
                routes: new Dictionary<string, WidgetBuilder> {
                    {"/", (context) => new MainScreen()},
                    {"Result", (context) => new Resultfulstate()},
                    {"Circular", (context) => new CircularProgress()}

                }
            );
    }

    protected PageRouteFactory pageRouteBuilder
    {
        get
        {
            return (RouteSettings settings, WidgetBuilder builder) =>
                new PageRouteBuilder(
                    settings: settings,
                    pageBuilder: (BuildContext context, Animation<float> animation,
                        Animation<float> secondaryAnimation) => builder(context),
                    transitionsBuilder: (BuildContext context, Animation<float>
                            animation, Animation<float> secondaryAnimation, Widget child) =>
                        new _FadeUpwardsPageTransition(
                            routeAnimation: animation,
                            child: child
                        )
                );
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        FontManager.instance.addFont(Resources.Load<Font>("Material Icon"));
    }
}





#region Scan view
public class MainScreen : StatefulWidget
{
    public bool hideImage = true;
    internal byte[] bytes;
    internal bool hideSpriteDemo=true;

    public MainScreen(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new MainScreenState();
    }
}

public class MainScreenState : State<MainScreen>
{
    List<Widget> selectItem = new List<Widget>();
    int prevIndex = 0;

    public override void initState()
    {
        base.initState();
        AssingDeviceCamera.GetAssingDeviceCamera.OnTakePhotoCallback += TakePhotoCallback;
        BaiduAI.GetBaiduAI.OnResultCallback += OnResultCallback;
        Debug.Log(ApplicationCanvas.language);
        for (int i = 0; i < ApplicationCanvas.language.resultLanguage.detectType.Count; i++)
        {
            Togger togger = new Togger() { name = ApplicationCanvas.language.resultLanguage.detectType[i], id = i };
            selectItem.Add(togger);
        }

        BaiduAI.GetBaiduAI.detectType = DetectType.General;
        prevIndex = 2;

#if !UNITY_EDITOR
        AssingDeviceCamera.GetAssingDeviceCamera.StartCamera(Window.instance);
        setState();
#endif
    }

    public override void dispose()
    {
        base.dispose();
        AssingDeviceCamera.GetAssingDeviceCamera.OnTakePhotoCallback -= TakePhotoCallback;
        BaiduAI.GetBaiduAI.OnResultCallback -= OnResultCallback;

        selectItem.Clear();
    }

    public override Widget build(BuildContext context)
    {
        MediaQueryData mediaQueryData = MediaQuery.of(context);
        return new Stack(
                alignment: Alignment.center,
                children: new List<Widget>
                {
                    new  Container(
                            color:new Color(0xfffbfbfb),
                            width:Screen.width,
                            height:Screen.height
                        ),
                      new Column(
                            children:new List<Widget>
                            {
                                _buildHeader(),
                                _buildSelect(),
                                 new Container(margin:EdgeInsets.all(50)),
                                _buildButton(ApplicationCanvas.language.selectPhotoButtonName,() => {Debug.Log("Select"); }),
                                new Container(margin:EdgeInsets.all(10)),
                                _buildButton(ApplicationCanvas.language.scanObjectButtonName,()=>{
                                    AssingDeviceCamera.GetAssingDeviceCamera.StartCamera(Window.instance);
                                    widget.hideSpriteDemo=false;
                                }),

                            }
                        )
                }
            );
    }

    private Widget _buildHeader()
    {
        return new Container(
                             alignment:Alignment.center,
                             height: 256,
                             width: 256,
                             margin: EdgeInsets.only(top: 80),
                             decoration: new BoxDecoration(
                                shape: BoxShape.circle,
                                border: Border.all(width: 2, color: new Color(0xFF1E90FF))
                            ),
                            child: new Stack(
                                    children: new List<Widget>
                                    {
                                       new Offstage(
                                           offstage:widget.hideSpriteDemo,
                                           child: new Sample.SpriteDemo()
                                       ),
                                        _buildCircleImage(),
                                        //_buildIcon(),

                                    }
                                )
                 );
    }

    private Widget _buildSelect()
    {

        return new Container(
                width: Screen.width,
                margin: EdgeInsets.only(top: 30),
                child: new Column(
                        children: new List<Widget>
                        {
                            new Container(
                                    padding:EdgeInsets.all(5),
                                    height:30,
                                    child:new PageView(

                                        onPageChanged:OnPageChange,
                                        controller:new PageController(initialPage:2,viewportFraction:0.2f),
                                        children: selectItem
                                    )
                                ),
                              new Container(
                                    padding:EdgeInsets.all(5),
                                    height:5,
                                    decoration:new BoxDecoration(color:new Color(0xff000000),shape:BoxShape.circle)
                            )
                        }
                    )
            );
    }

    private Widget _buildCircleImage()
    {
        ImageProvider imageProvider = null;
        if (widget.bytes != null && widget.bytes.Length > 0)
        {
            imageProvider = new MemoryImage(bytes: widget.bytes);
        }
        else
        {
            imageProvider = new AssetImage("EmptyPage");
        }
        return new Offstage(
            offstage: widget.hideImage,
            child: Transform.rotate(
                    alignment: Alignment.center,
                    //transform: Matrix3.makeRotate(Mathf.PI / 2),
                    degree:Mathf.PI/2,
                    child: new Container(
                            decoration: new BoxDecoration(
                                    image: new DecorationImage(
                                       image: imageProvider,
                                       fit: BoxFit.cover
                                    ),
                                    shape: BoxShape.circle
                                )
                            )
                )
            );
    }

    private Widget _buildButton(string btnName = "Default", VoidCallback onpressCallback = null)
    {
        return new Material(
                    child: new Center(
                             child: new FlatButton(
                   child: new Text(btnName, style: new TextStyle(color: new Color(0xffffffff))),
                       onPressed: onpressCallback,
                       shape: new RoundedRectangleBorder(borderRadius: BorderRadius.all(20)),
                       color: new Color(0xff00BBFF)

                   )
                        )
                );
    }

    private Widget _buildToggerSliderItem(string name)
    {
        return new Container(
                child: new Text(name, textAlign: TextAlign.center)
            );
    }







    //Call back

    private void TakePhotoCallback(byte[] bytes, Window disposable)
    {
        using (disposable.getScope())
        {
            widget.hideImage = false;
            widget.bytes = bytes;
            setState();
        }
    }

    private void OnPageChange(int index)
    {
        BaiduAI.GetBaiduAI.detectType = (DetectType)index;
        Togger cur = selectItem[index] as Togger;
        cur.IsSelected.Invoke(true);


        Togger prevTogger = selectItem[prevIndex] as Togger;
        prevTogger.IsSelected.Invoke(false);

        prevIndex = index;

        setState();
    }

    private void OnResultCallback(string result, Window window)
    {
        using (window.getScope())
        {
            Navigator.pushNamed(context, "Result");
            widget.hideSpriteDemo =true;
            widget.bytes = null;
            setState();
        }
    }
}

public class Togger : StatefulWidget
{
    public Action<bool> IsSelected;
    public string name;
    public Color defaultColor = new Color(0xff8E8E8E);
    public int id;

    public Togger(Key key = null) : base(key)
    {
    }


    public override State createState()
    {
        return new ToggerState();
    }
}

public class ToggerState : State<Togger>
{
    bool initFirst = false;
    public override void initState()
    {
        widget.IsSelected += IsSelectedCallback;
        if (widget.id == 2 && !initFirst)
        {
            widget.IsSelected(true);
            initFirst = true;
        }
    }

    public override void dispose()
    {
        base.dispose();
        widget.IsSelected -= IsSelectedCallback;
        initFirst = false;
    }

    public override Widget build(BuildContext context)
    {
        return new Container(
                 padding: EdgeInsets.only(top: 2),
                 child: new Text(widget.name, textAlign: TextAlign.center, style: new TextStyle(color: widget.defaultColor))
             );
    }


    private void IsSelectedCallback(bool selected)
    {
        widget.defaultColor = selected ? new Color(0xff000000) : new Color(0xff8E8E8E);
        setState();
    }
}

#endregion





#region progress circular
public class CircularProgress : StatefulWidget
{
    internal AnimationController controoler;

    public CircularProgress(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new test();
    }
}

public class test : SingleTickerProviderStateMixin<CircularProgress>
{
    private float strokeWidth = 2;
    private float radius;
    private float strokeCapRound;
    private float value;
    private Color backgroundColor = new Color(0xffeeeeee);
    private float totalAngle;
    private List<Color> colors;
    private List<float> stops;
    Animation<float> animation;
    public override void initState()
    {
        base.initState();
        totalAngle = 6.28f;
        widget.controoler = new AnimationController(vsync: this, duration: new System.TimeSpan(0, 0, seconds: 3));
        animation = new FloatTween(0, 1).animate(widget.controoler);
        bool isForward = true;
        widget.controoler.addListener(() => { setState(); });
        widget.controoler.addStatusListener((status) =>
        {
            if (status == AnimationStatus.forward)
                isForward = true;
            else if (status == AnimationStatus.completed || status == AnimationStatus.dismissed)
            {
                if (isForward)
                    widget.controoler.reverse();
                else
                    widget.controoler.forward();
            }     
        });
        widget.controoler.forward();

    }

    public override Widget build(BuildContext context)
    {
        float offset = 0;

        colors = new List<Color>();
        colors.Add(new Color(0xffff9a9e));
        colors.Add(new Color(0xfffad0c4));
        radius = 50;
        strokeWidth = 10;
        value = 0.2f;

        return new Container(color:new Color(0xffffffff),
            child:Transform.rotate(
                    alignment: Alignment.bottomCenter,
                    degree: 6.28f,
                    child: new CustomPaint(
                    size: Size.fromRadius(radius),
                    painter: new CircularProgressPainter(
                            strokeWidth: strokeWidth,
                            strokeCapRound: false,
                            backgroundColor: backgroundColor,
                            value: animation.value,
                            total: totalAngle,
                            radius: radius,
                            colors: colors
                        )
                    )
                )
            );
    }
}

public class CircularProgressPainter : CustomPainter
{
    private float strokeWidth;
    private bool strokeCapRound;
    private Color backgroundColor;
    private float radius;
    private List<float> stops;
    private float value;
    private float total;
    private List<Color> colors;

    public CircularProgressPainter(float strokeWidth = 10, bool strokeCapRound = false, Color backgroundColor = null, float radius = 1, float total = 2 * Mathf.PI, List<float> stops = null, float value = 0, List<Color> colors = null)
    {
        this.strokeWidth = strokeWidth;
        this.strokeCapRound = strokeCapRound;
        this.backgroundColor = backgroundColor;
        this.radius = radius;
        this.stops = stops;
        this.value = value;
        this.total = total;
        this.colors = colors;

    }

    public void addListener(VoidCallback listener)
    {

    }

    public bool? hitTest(Offset position)
    {
        return true;
    }

    public void paint(Unity.UIWidgets.ui.Canvas canvas, Size size)
    {
        size = Size.fromRadius(radius);
        float offset = strokeWidth / 2;
        float value =this.value;
        value = value.clamp(0.0f, 1.0f)*total;
        float start = 0;

        if (strokeCapRound)
        {
            start = Mathf.Asin(strokeWidth / (size.width - strokeWidth));
        }

        Rect rect = new Offset(offset, offset) & new Size(size.width - strokeWidth, size.height - strokeWidth);
        new Paint
        {
            strokeCap = strokeCapRound ? StrokeCap.round : StrokeCap.butt,
            style = PaintingStyle.stroke,
            strokeWidth = this.strokeWidth
        }.color = backgroundColor;
        canvas.drawArc(rect, start, total, false, new Paint
        {
            strokeCap = strokeCapRound ? StrokeCap.round : StrokeCap.butt,
            style = PaintingStyle.stroke,
            strokeWidth = this.strokeWidth
        });
        //canvas.drawRect(rect, paint);
        if (value > 0)
        {
            new Paint
            {
                strokeCap = strokeCapRound ? StrokeCap.round : StrokeCap.butt,
                style = PaintingStyle.stroke,
                strokeWidth = this.strokeWidth
            }.shader = new SweepGradient(startAngle: 0, endAngle: value, colors: colors, stops: stops).createShader(rect);
            canvas.drawArc(rect, start, value, false, new Paint
            {
                strokeCap = strokeCapRound ? StrokeCap.round : StrokeCap.butt,
                style = PaintingStyle.stroke,
                strokeWidth = this.strokeWidth
            });
        }
    }

    public void removeListener(VoidCallback listener)
    {

    }

    public bool shouldRepaint(CustomPainter oldDelegate) => true;
}


#endregion


#region Result view
public class Resultfulstate : StatefulWidget
{
    public Resultfulstate(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new ResultState();
    }
}
public class ResultState : State<Resultfulstate>
{
    private BaseData baseData;
    public override void initState()
    {
        base.initState();
        string json = BaiduAI.GetBaiduAI.ResultString;
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
    }
    public override void dispose()
    {
        base.dispose();
        AssingDeviceCamera.GetAssingDeviceCamera.StartCamera(Window.instance);
    }
    public override Widget build(BuildContext context)
    { 
        string keyword = ApplicationCanvas.language.resultLanguage.title;
        string image_url = "file:///"+Application.streamingAssetsPath+"/"+ ApplicationCanvas.language.resultLanguage.headerImage;
        string description = ApplicationCanvas.language.resultLanguage.descript;
        string baike_url = string.Empty;


        switch (BaiduAI.GetBaiduAI.detectType)
        {
            case DetectType.Dish:
                DishData dishData = baseData as DishData;
                if (dishData.result.Count >= 1)
                {
                    DishResult result = dishData.result[0];
                    keyword = result.name + string.Format("\nCalorie:{0}", result.calorie);
                    image_url = result.baike_info.image_url ?? image_url;
                    description = result.baike_info.description ?? description;
                    baike_url = result.baike_info.baike_url ?? baike_url;
                }
                return General(image_url, description, keyword, baike_url);
            case DetectType.Car:
                CarData carData = baseData as CarData;
                if (carData.result.Count >= 1)
                {
                    CarResult result = carData.result[0];
                    keyword = result.name + string.Format("\n{0}", result.year);
                    image_url = result.baike_info.image_url ?? image_url;
                    description = result.baike_info.description ?? description;
                    baike_url = result.baike_info.baike_url ?? baike_url;
                }
                return General(image_url, description, keyword, baike_url);
            case DetectType.General:
                GeneralData generalData = baseData as GeneralData;
                if (generalData.result_num > 1)
                {
                    GeneralResult result = generalData.result[0];
                    keyword = result.keyword;
                    image_url = result.baike_info.image_url ?? image_url;
                    description = result.baike_info.description ?? description;
                    baike_url = result.baike_info.baike_url ?? baike_url;
                }
                return General(image_url, description, keyword, baike_url);
            case DetectType.Logo:
                LogoData logoData = baseData as LogoData;
                if (logoData.result_num > 1)
                {
                    LogoResult result = logoData.result[0];
                    keyword = result.name;
                    image_url = result.baike_info.image_url ?? image_url;
                    description = result.baike_info.description ?? description;
                    baike_url = result.baike_info.baike_url ?? baike_url;
                }
                return General(image_url, description, keyword, baike_url);

            case DetectType.Animal:
                AnimalData animalData = baseData as AnimalData;
                if (animalData.result.Count >= 1)
                {
                    AnimalResult result = animalData.result[0];
                    keyword = result.name;
                    image_url = result.baike_info.image_url ?? image_url;
                    description = result.baike_info.description ?? description;
                    baike_url = result.baike_info.baike_url ?? baike_url;
                }
                return General(image_url, description, keyword, baike_url);
            case DetectType.Plant:
                PlantData panltData = baseData as PlantData;
                if (panltData.result.Count >= 1)
                {
                    PlantResult result = panltData.result[0];
                    keyword = result.name;
                    image_url = result.baike_info.image_url ?? image_url;
                    description = result.baike_info.description ?? description;
                    baike_url = result.baike_info.baike_url ?? baike_url;
                }
                return General(image_url, description, keyword, baike_url);
            case DetectType.Landmark:
                LandmarkData landmarkData = baseData as LandmarkData;
                if (landmarkData.result != null)
                {
                    LandmaskResult result = landmarkData.result;
                    keyword = string.IsNullOrEmpty(result.landmark)?keyword: result.landmark;
                }
                return General(image_url, description, keyword,baike_url);
        }
        return new Container();
    }

    #region General
    private Widget General(string image_url = null, string detail = null, string title = null,string baike_url=null)
    {

        List<Widget> bodyWidget = new List<Widget>{GeneralBaikeDetails(detail)};
        if (!string.IsNullOrEmpty(baike_url))
            bodyWidget.Add(GeneralBaikeButton(baike_url));
        return new Container(
                    height: Screen.height,
                    color: new Color(0xfffbfbfb),
                    child: new Column(
                        children: new List<Widget>
                        {
                           _buildHeader(image_url,title,baike_url),
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
                  child: new Text(detail, textAlign: TextAlign.left, style: new TextStyle(fontSize: 16, textBaseline: TextBaseline.ideographic, height: 2))
              );
    }

    private Widget GeneralBaikeButton(string baike_url)
    {
        return new Container(
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
    }
    #endregion

    private Widget _buildHeader(string image_url, string title,string baike_url)
    {
        MediaQueryData mediaQueryData = MediaQuery.of(context);
        int lenght = title.Length;
        int fontSize = 20;
        if (lenght < 5)
            fontSize = 30;
        else if (lenght > 5 && lenght < 12)
            fontSize = 20;
        else if (lenght > 12)
            fontSize = 16;
        return new Container(
                      child: new Stack(
                          alignment: Alignment.topLeft,
                          children: new List<Widget>
                          {
                            new Container(
                                height:mediaQueryData.size.height*0.35f,
                                width:mediaQueryData.size.width,
                                child:Unity.UIWidgets.widgets.Image.network(image_url,fit:BoxFit.cover)
                             ),
                                new Container(
                                decoration:new BoxDecoration(color:Color.fromARGB(100,0,0,0)),
                                width:mediaQueryData.size.width,
                                height:mediaQueryData.size.height*0.35f
                             ),
                            new Container(
                                margin:EdgeInsets.only(top:Screen.height*0.03f,left:30),
                                child:new GestureDetector(
                                        child:new Text(ApplicationCanvas.language.returnButtonName,style:new TextStyle(color:new Color(0xffffffff))),
                                        onTap:()=>{
                                            Navigator.pop(context);
                                            baseData=null;
                                        }
                                 )
                            ),
                            new Container(
                                alignment:Alignment.center,
                                //margin:EdgeInsets.only(top:Screen.height*0.08f),
                                child:new Container(
                                                    margin:EdgeInsets.all(100),
                                                    child:new Text(title,
                                                            style:new TextStyle(color:new Color(0xffffffff),fontSize:fontSize,fontWeight:FontWeight.w700),
                                                            textAlign:TextAlign.center
                                                         )
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


class _FadeUpwardsPageTransition : StatelessWidget
{
    internal _FadeUpwardsPageTransition(
        Key key = null,
        Animation<float> routeAnimation = null, // The route's linear 0.0 - 1.0 animation.
        Widget child = null
    ) : base(key: key)
    {
        this._positionAnimation = _bottomUpTween.chain(_fastOutSlowInTween).animate(routeAnimation);
        this._opacityAnimation = _easeInTween.animate(routeAnimation);
        this.child = child;
    }

    static Tween<Offset> _bottomUpTween = new OffsetTween(
        begin: new Offset(0.0f, 0.25f),
        end: Offset.zero
    );

    static Animatable<float> _fastOutSlowInTween = new CurveTween(curve: Curves.fastOutSlowIn);
    static Animatable<float> _easeInTween = new CurveTween(curve: Curves.easeIn);

    readonly Animation<Offset> _positionAnimation;
    readonly Animation<float> _opacityAnimation;
    public readonly Widget child;

    public override Widget build(BuildContext context)
    {
        return new SlideTransition(
            position: this._positionAnimation,
            child: new FadeTransition(
                opacity: this._opacityAnimation,
                child: this.child
            )
        );
    }
}
