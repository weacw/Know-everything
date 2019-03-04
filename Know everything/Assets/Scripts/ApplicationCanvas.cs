using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UIWidgets;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.foundation;
using Stack = Unity.UIWidgets.widgets.Stack;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.material;

public class ApplicationCanvas : WidgetCanvas
{
    protected override string initialRoute => "/";
    protected override Dictionary<string, WidgetBuilder> routes
    {
        get
        {
            return new Dictionary<string, WidgetBuilder>
            {
                {"/",(context)=>new MainScreen() }
            };
        }
    }
}


public class MainScreen : StatelessWidget
{
    private BuildContext context;
    public MainScreen(Key key = null) : base(key)
    {
    }

    public override Widget build(BuildContext context)
    {
        this.context = context;
        return this._buildBackground();
    }

    private Widget _buildBackground()
    {
        return new Container(
                width: Screen.width,
                height: Screen.height,
                color: new Unity.UIWidgets.ui.Color(0xFFFBFBFB),
                child: new Column(
                        children: new List<Widget>
                        {
                            this._buildImageRound(),
                            this._buildButton(),
                        }
                    )
            );
    }



    private Widget _buildImageRound()
    {
        return new Container(
                height: 256,
                width: 256,
                margin: EdgeInsets.only(top: Screen.height * 0.5f-128, left: 64, right: 64),
                decoration: new BoxDecoration(shape: BoxShape.circle, border: Border.all(width: 2, color: new Unity.UIWidgets.ui.Color(0xFF000000)))
                );
    }
    private Widget _buildButton()
    {
        return new Column
        (
            children: new List<Widget>
            {
                new Container(
                        decoration:new BoxDecoration(color:new Unity.UIWidgets.ui.Color(0xff00BBFF),shape:BoxShape.rectangle,borderRadius:BorderRadius.all(20)),
                        margin:EdgeInsets.only(bottom:Screen.height/2),
                        child:new FlatButton(child:new Text("Take photo",style:new TextStyle(color:new Unity.UIWidgets.ui.Color(0xffffffff))))
                    )
            }
        );
    }
}


public class GetResultion
{
    public static Vector2 getResultion(BuildContext context)
    {
        MediaQueryData mediaQueryData = MediaQuery.of(context);
        return new Vector2(mediaQueryData.size.width, mediaQueryData.size.height);
    }
}