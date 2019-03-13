using Unity.UIWidgets.widgets;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.animation;

public class FadeUpwardsPageTransition : StatelessWidget
{
    internal FadeUpwardsPageTransition(
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
