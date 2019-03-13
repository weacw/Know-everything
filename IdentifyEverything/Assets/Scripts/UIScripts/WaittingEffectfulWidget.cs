using System;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using Color = Unity.UIWidgets.ui.Color;
using Rect = Unity.UIWidgets.ui.Rect;


public class SpritePainter : CustomPainter
{
    Animation<float> animation;

    public SpritePainter(AnimationController controller)
    {
        animation = new FloatTween(0, 1).animate(controller);
    }

    public void addListener(VoidCallback listener)
    {

    }

    public bool? hitTest(Offset position)
    {
        return false;
    }

    public void removeListener(VoidCallback listener)
    {

    }

    public bool shouldRepaint(CustomPainter oldDelegate) => true;

    void Circle(Canvas canvas, Rect rect, float value)
    {
        float opacity = (1 - (value / 4f).clamp(0, 1));
        Color color = Color.fromRGBO(0, 117, 194, opacity);

        float size = rect.width;
        float area = size * size;
        float radius = UnityEngine.Mathf.Sqrt(area * value / 4);

        Paint paint = new Paint();
        paint.color = color;
        canvas.drawCircle(rect.center, radius, paint);
    }

    void CustomPainter.paint(Canvas canvas, Size size)
    {
        Rect rect = Rect.fromLTRB(0, 0, size.width, size.height);
        for (int i = 3; i >= 0; i--)
        {
            Circle(canvas, rect, i + animation.value);
        }
    }
}

public class WaittingEffectfulWidget : StatefulWidget
{
    public WaittingEffectfulWidget(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new WaittingEffectState();
    }
}
public class WaittingEffectState : SingleTickerProviderStateMixin<WaittingEffectfulWidget>
{
    Animation<float> animation;
    AnimationController controller;

    public override void initState()
    {
        base.initState();
        controller = new AnimationController(vsync: this);
        controller.addListener(() => { setState(); });
        _startAnimation();
    }

    public override void dispose()
    {
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new Container(
                alignment: Unity.UIWidgets.painting.Alignment.center,
                child: new CustomPaint(
                        painter: new SpritePainter(controller),
                        child: new SizedBox(
                                width: 200,
                                height: 200
                            )
                    )
            );
    }

    void _startAnimation()
    {
        controller.stop();
        controller.reset();
        controller.repeat(period: new TimeSpan(0, 0, 0, 1));
    }
}

