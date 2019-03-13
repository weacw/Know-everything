using System.Collections.Generic;
using UnityEngine;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.animation;

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
                    {"/", (context) => new MainScreenfulWidget()},
                    {"Result", (context) => new ResultfulsWidget()},
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
                        new FadeUpwardsPageTransition(
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