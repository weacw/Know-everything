using System.Collections.Generic;
using UnityEngine;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.foundation;
using Stack = Unity.UIWidgets.widgets.Stack;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.material;
using Color = Unity.UIWidgets.ui.Color;
using Unity.UIWidgets.ui;
using Transform = Unity.UIWidgets.widgets.Transform;
using TextStyle = Unity.UIWidgets.painting.TextStyle;
using Material = Unity.UIWidgets.material.Material;
using Texture = Unity.UIWidgets.widgets.Texture;



#region Scan view
public class MainScreenfulWidget : StatefulWidget
{
    public bool hideImage = true;
    internal byte[] bytes;
    internal bool hideSpriteDemo = true;
    public static string resultJson;
    public MainScreenfulWidget(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new MainScreenState();
    }
}

public class MainScreenState : State<MainScreenfulWidget>
{
    List<Widget> selectItem = new List<Widget>();
    int prevIndex = 0;
    ImageProvider imageProvider = null;
    bool wasClicked;
    bool hideCamera=true;
    public override void initState()
    {
        base.initState();
        AssingDeviceCamera.GetAssingDeviceCamera.OnTakePhotoCallback += TakePhotoCallback;
        AssingDeviceCamera.GetAssingDeviceCamera.OnCameraStateChange += OnCameraStateChange;

        BaiduAI.GetBaiduAI.OnResultCallback += OnResultCallback;

        for (int i = 0; i < ApplicationCanvas.language.resultLanguage.detectType.Count; i++)
        {
            DetectToggerfulWidget togger = new DetectToggerfulWidget() { name = ApplicationCanvas.language.resultLanguage.detectType[i], id = i };
            selectItem.Add(togger);
        }

        BaiduAI.GetBaiduAI.detectType = DetectType.General;
        prevIndex = 2;

#if !UNITY_EDITOR
        AssingDeviceCamera.GetAssingDeviceCamera.StartCamera();
        setState();
#endif
    }

    public override void dispose()
    {
        base.dispose();
        AssingDeviceCamera.GetAssingDeviceCamera.OnTakePhotoCallback -= TakePhotoCallback;
        AssingDeviceCamera.GetAssingDeviceCamera.OnCameraStateChange -= OnCameraStateChange;
        BaiduAI.GetBaiduAI.OnResultCallback -= OnResultCallback;
        imageProvider = null;
        selectItem.Clear();
    }

    public override Widget build(BuildContext context)
    {
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
                                //_buildButton(ApplicationCanvas.language.selectPhotoButtonName,() => {Debug.Log("Select"); }),
                                new Container(margin:EdgeInsets.all(10)),
                                _buildButton(ApplicationCanvas.language.scanObjectButtonName,()=>{
                                    if(wasClicked)return;
                                    AssingDeviceCamera.GetAssingDeviceCamera.StartCamera();
                                    hideCamera=false;
                                }),
                            }
                        )
                }
            );
    }

    private Widget _buildHeader()
    {
        return new Container(
                             alignment: Alignment.center,
                             //height: 256,
                             //width: 256,
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
                                           child: new WaittingEffectfulWidget()
                                       ),
                                        _buildCircleImage(),
                                       _buildCamTexture(),
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
        if (widget.bytes != null && widget.bytes.Length > 0)
        {
            imageProvider = new MemoryImage(bytes: widget.bytes);
        }
        else
        {
            imageProvider = new AssetImage("search");
        }

        return new Offstage(
                offstage: false,
                child: Transform.rotate(
                        alignment: Alignment.center,
    #if !UNITY_EDITOR
                        degree: Mathf.PI / 2,
    #endif
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

    private Widget _buildCamTexture()
    {
        return AssingDeviceCamera.GetAssingDeviceCamera.m_WebCamTexture == null ?
        new ClipRRect(
            borderRadius: BorderRadius.all(30),
            child: new Container(width: 256, height: 256)
        ):

        new ClipRRect(
            borderRadius: BorderRadius.all(150),
            child: 
            new Container(
            width: 350,
            height: 256,
            child: new Texture(texture: AssingDeviceCamera.GetAssingDeviceCamera.m_WebCamTexture))
        );
    }

    private void TakePhotoCallback(byte[] bytes)
    {
        wasClicked = true;
        using (WindowProvider.of(context).getScope())
        {
            widget.hideImage = false;
            widget.bytes = bytes;
            setState();
        }
    }

    private void OnCameraStateChange(bool state)
    {
        using (WindowProvider.of(context).getScope())
        {
            setState();
        }
    }

    private void OnPageChange(int index)
    {
        BaiduAI.GetBaiduAI.detectType = (DetectType)index;
#pragma warning disable CS0436 // 类型与导入类型冲突
        DetectToggerfulWidget cur = selectItem[index] as DetectToggerfulWidget;
#pragma warning restore CS0436 // 类型与导入类型冲突
        cur.IsSelected.Invoke(true);


#pragma warning disable CS0436 // 类型与导入类型冲突
        DetectToggerfulWidget prevTogger = selectItem[prevIndex] as DetectToggerfulWidget;
#pragma warning restore CS0436 // 类型与导入类型冲突
        prevTogger.IsSelected.Invoke(false);

        prevIndex = index;

        setState();
    }

    private void OnResultCallback(string result, string json)
    {
        wasClicked = false;
        WindowProvider.of(context).runInMain(() => {
            using (WindowProvider.of(context).getScope())
            {
                MainScreenfulWidget.resultJson = json;
                widget.hideSpriteDemo = true;
                widget.bytes = null;
                setState();
                Navigator.pushNamed(context, "Result");
            }
        });
    }
}

#endregion


