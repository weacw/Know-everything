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
using Image = Unity.UIWidgets.widgets.Image;
using System;

public class ApplicationCanvas : WidgetCanvas
{
    protected override string initialRoute => "/";
    protected override Dictionary<string, WidgetBuilder> routes
    {
        get
        {
            return new Dictionary<string, WidgetBuilder>
            {
                {"EntryView",(context)=>new EntryView() },
                {"2",(context)=>new CircularProgress()},
                {"/",(context)=>new MainScreen()},
                {"Result",(context)=>new Resultfulstate()}
            };
        }
    }

    protected override PageRouteFactory pageRouteBuilder
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


#region Entry view
public class EntryView : StatefulWidget
{
    public int index = 0;
    public EntryView(Key key=null) : base(key)
    {
    }

    public override State createState()
    {
        return new EntryState();
    }
}

public class EntryState : State<EntryView>
{
    public override Widget build(BuildContext context)
    {
        return  this._buildBody();
    }


    private Widget _pageProduct(string _url)
    {
        MediaQueryData mediaQueryData = MediaQuery.of(context);
        return new Stack(
                alignment: Alignment.topCenter,
                children: new List<Widget>
                {
                    new Container(color:new Color(0xfffbfbfb)),
                    new Container(
                            child:Image.asset(_url,fit:BoxFit.cover),width:Screen.width,height:Screen.height
                        ),
                    new Container(
                            margin:EdgeInsets.only(top:mediaQueryData.size.height*0.85f),
                            child:_buildButton("Let's Go",OnLetsGoButtonClicked)

                    ),
                }
            );
    }


    private void OnLetsGoButtonClicked() 
    {
        Navigator.pushName(context, "MainScreen");
       BaiduAI.detectType = (DetectType)widget.index;
    }

    private Widget _buildBody()
    {
        return new Container(
                    child: new PageView(
                        onPageChanged:(i)=>widget.index=i,
                        children: new List<Widget>
                        {
                            _pageProduct("animal"),
                            _pageProduct("car"),
                            _pageProduct("food"),
                            _pageProduct("landmark"),
                            //_pageProduct("http://imgsrc.baidu.com/imgad/pic/item/29381f30e924b899daf7d31b64061d950b7bf6ce.jpg"),
                            //_pageProduct("http://imgsrc.baidu.com/imgad/pic/item/29381f30e924b899daf7d31b64061d950b7bf6ce.jpg"),
                            //_pageProduct("http://imgsrc.baidu.com/imgad/pic/item/29381f30e924b899daf7d31b64061d950b7bf6ce.jpg"),
                            //_pageProduct("http://imgsrc.baidu.com/imgad/pic/item/29381f30e924b899daf7d31b64061d950b7bf6ce.jpg"),
                        }
                    )
                );
    }

    private Widget _buildCircleImage(string image,string name)
    {
        return new Container(
                    height: 80,
                    width: 80,
                    margin: EdgeInsets.only(left: 50),
                    decoration: new BoxDecoration(
                            image: new DecorationImage(
                                   image: new NetworkImage(url: image),
                                   fit: BoxFit.cover
                                ),
                                shape: BoxShape.circle,
                                border: Border.all(new Color(0xffffffff), 2)
                        ),
                        child:new Text(name,style:new TextStyle(color:new Color(0xffffffff)))
                );
    }

    private Widget _buildButton(string btnName = "Default", VoidCallback onpressCallback = null)
    {
        return new FlatButton(
                       child: new Text(btnName, style: new TextStyle(color: new Color(0xffffffff))),
                       onPressed: onpressCallback,
                       shape: new RoundedRectangleBorder(borderRadius: BorderRadius.all(20)),
                       color: new Color(0xff00BBFF)
                   );
    }
}

#endregion





#region Scan view
public class MainScreen : StatefulWidget
{
    public bool hideImage=true;
    internal byte[] bytes;
  
    public MainScreen(Key key=null) : base(key)
    {
    }

    public override State createState()
    {
        return new MainScreenState();
    }
}

public class MainScreenState : SingleTickerProviderStateMixin<MainScreen>
{
    private BuildContext context;
    List<Widget> selectItem = new List<Widget>();
    int prevIndex = 0;
    int curIndex = 2;
    AnimationController controller;
    Animation<int> alpha;
    Animation<float> position;
    public List<int> iconsPoint = new List<int>
    {
        0x03e8,
        0x03ef,
        0x03ea,
        0x03eb,
        0x03ee,
        0x03ec,
        0x03e9,
    };

    public override void initState()
    {
        base.initState();
        AssingDeviceCamera.GetAssingDeviceCamera.m_Action += TakePhotoCallback;
        BaiduAI.OnResultCallback += OnResultCallback;


        for (int i = 0; i < Enum.GetNames(typeof(DetectType)).Length; i++)
        {
            Togger togger = new Togger() { name = ((DetectType)i).ToString(),  id = i };
            selectItem.Add(togger);
        }

        BaiduAI.detectType = DetectType.General;
        prevIndex = 2;

        controller = new AnimationController(duration: new TimeSpan(0, 0, 0, 0, milliseconds: 500), vsync: this);
        CurvedAnimation curved = new CurvedAnimation(parent: controller, curve: Curves.easeIn);
        alpha = new IntTween(0, 255).animate(curved);
        position = new FloatTween(begin: 10f, end:0f).animate(curved);
        controller.addListener(() => {
            setState();
        });


        controller.addStatusListener((status) => { 
            if(status == AnimationStatus.completed)
            {
                AssingDeviceCamera.GetAssingDeviceCamera.StartCoroutine(WaitToRest());
            }

        });
        controller.forward();
#if !UNITY_EDITOR
        AssingDeviceCamera.GetAssingDeviceCamera.StartCamera();
        setState();
#endif
    }
  
    public override void dispose()
    {
        base.dispose();
        AssingDeviceCamera.GetAssingDeviceCamera.m_Action -= TakePhotoCallback;
        BaiduAI.OnResultCallback -= OnResultCallback;

        selectItem.Clear();
    }

    public override Widget build(BuildContext context)
    {
        this.context = context;
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
                                _buildButton("Select Image",() => {Debug.Log("Select"); }),
                                new Container(margin:EdgeInsets.all(10)),
                                _buildButton("Take Photo",AssingDeviceCamera.GetAssingDeviceCamera.StartCamera),

                            }
                        )
                }
            );
    }

    private Widget _buildHeader()
    {
        return new Container(
                             height: 256,
                             width: 256,
                             margin: EdgeInsets.only(top: 80, bottom: 20),
                             decoration: new BoxDecoration(
                                shape: BoxShape.circle,
                                border: Border.all(width: 2, color: new Color(0xFF1E90FF))
                            ),
                            child: new Stack(
                                    children:new List<Widget>
                                    {
                                        _buildCircleImage(),
                                        //_buildIcon(),

                                    }
                                )
                 );
    }

    private Widget _buildIcon()
    {

        return new Container(
                    transfrom:Matrix3.makeTrans(position.value,0),
                    alignment: Alignment.center,
                    child: new Column(
                            children:new List<Widget>
                            {
                                new Container(
                                        margin:EdgeInsets.all(25)
                                    ),
                                new Icon(new IconData(iconsPoint[prevIndex], "Material Icon"), size: 150, color: Color.fromARGB(alpha.value, 0, 0, 0))
                            }
                        )
                );
    }


    private Widget _buildSelect()
    {

        return new Container(
                width: Screen.width,
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
        return new Offstage(
            offstage: widget.hideImage,
            child: new Transform(
                    alignment:Alignment.center,
                    transform:Matrix3.makeRotate(Mathf.PI/2),
                    child: new Container(
                            decoration: new BoxDecoration(
                                    image: new DecorationImage(
                                       image: new MemoryImage(widget.bytes),
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
                    child:new Center(
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
                child:new Text(name, textAlign: TextAlign.center)
            );
    }

    private void TakePhotoCallback(byte[] bytes)
    {
        widget.hideImage = false;
        widget.bytes = bytes;
        setState();
    }

    private void OnPageChange(int index)
    {
        controller.reset();

        BaiduAI.detectType = (DetectType)index;
        Togger cur = selectItem[index] as Togger;
        cur.IsSelected.Invoke(true);


        Togger prevTogger = selectItem[prevIndex] as Togger;
        prevTogger.IsSelected.Invoke(false);

        prevIndex = index;

        controller.forward();

        setState();
    }

    private System.Collections.IEnumerator WaitToRest()
    {
        yield return new WaitForSeconds(1);
        controller.reset();

    }

    private void OnResultCallback()
    {
        Debug.Log("Call back");
        Navigator.pushName(context, "Result");
    }
}

public class Togger : StatefulWidget
{
    public Action<bool> IsSelected;
    public string name;
    public Color defaultColor= new Color(0xff8E8E8E);
    public int id;

    public Togger(Key key=null) : base(key)
    {
    }


    public override State createState()
    {
        return new ToggerState();
    }
}

public class ToggerState : State<Togger>
{
    public override void initState()
    {
        widget.IsSelected += IsSelectedCallback;
        if(widget.id == 2)
        {
            widget.IsSelected(true);
        }
    }

    public override void dispose()
    {
        base.dispose();
        widget.IsSelected -= IsSelectedCallback;
    }

    public override Widget build(BuildContext context)
    {
        return new Container(
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

    public CircularProgress(Key key=null) : base(key)
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
    private float totalAngle = 2 * Mathf.PI;
    private List<Color> colors;
    private List<float> stops;

    public override void initState()
    {
        base.initState();
        widget.controoler = new AnimationController(vsync: this, duration: new System.TimeSpan(0,0,seconds: 3));
        bool isForward = true;
        widget.controoler.addStatusListener((status) => {
            if (status == AnimationStatus.forward)
                isForward = true;
            else if(status == AnimationStatus.completed|| status == AnimationStatus.dismissed)
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
        colors.Add(new Color(0xff0000CD));
        colors.Add(new Color(0xffFF00FF));
        radius = 50;
        strokeWidth = 10;
        value = 0.2f;

        return Transform.rotate(
                alignment: Alignment.bottomCenter,
                degree: Mathf.PI * 2,
                child: new CustomPaint(
                size: Size.fromRadius(radius),
                painter: new CircularProgressPainter(
                        strokeWidth: strokeWidth,
                        strokeCapRound: false,
                        backgroundColor: backgroundColor,
                        value: widget.controoler.value,
                        total: totalAngle,
                        radius: radius,
                        colors: colors
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

    public CircularProgressPainter(float strokeWidth = 10, bool strokeCapRound = false, Color backgroundColor = null, float radius = 1, float total = 2*Mathf.PI, List<float> stops = null, float value = 0,List<Color> colors=null)
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
        float value = this.value;
        value = value.clamp(0.0f, 1.0f) * total;
        float start = 0;


        if (strokeCapRound)
        {
            start = Mathf.Asin(strokeWidth / (size.width - strokeWidth));
        }

        Rect rect = new Offset(offset, offset) & new Size(size.width - strokeWidth, size.height - strokeWidth);
        Paint paint = new Paint()
        {
            strokeCap = strokeCapRound ? StrokeCap.round : StrokeCap.butt,
            style = PaintingStyle.stroke,
            strokeWidth = this.strokeWidth
        };

        paint.color = backgroundColor;
        canvas.drawArc(rect, start, total, false, paint);

    
        if (value > 0)
        {
            paint.shader = new SweepGradient(startAngle:0,endAngle:value,colors:colors,stops:stops).createShader(rect);
        }
        canvas.drawArc(rect, start, value, false, paint);
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
    internal string result;
    public Resultfulstate(Key key=null) : base(key)
    {
    }

    public override State createState()
    {
        return new ResultState();
    }
}
public class ResultState : State<Resultfulstate>
{
    public override void initState()
    {
        base.initState();
        //widget.result = UnityEngine.Object.FindObjectOfType<BaiduAI>().Result.ToString();
        setState();
    }
    public override Widget build(BuildContext context)
    {
        return new Container(
                child: new Stack(
                        children:new List<Widget>
                        {
                            new Container(
                                    width:Screen.width,
                                    height:Screen.height,
                                    color:new Color(0xfffbfbfb),
                                    child:new Text(widget.result,maxLines:100)
                                )
                        }
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
