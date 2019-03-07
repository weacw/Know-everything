using System;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;
using Rect = Unity.UIWidgets.ui.Rect;
using TextStyle = Unity.UIWidgets.painting.TextStyle;
using Transform = Unity.UIWidgets.widgets.Transform;

namespace Sample.CircularProgress
{
    public class CircularProgressSample : UIWidgetsPanel
    {
        protected override Widget createWidget()
        {
            return new Container(
                    alignment: Alignment.center,
                    width:Screen.width,
                    height:Screen.height,
                    color:new Color(0xfffbfbfb),
                    child: new SpriteDemo()
                );
        }
    }
   

    #region  circular progress
    public class CircularProgress : StatefulWidget
    {
        internal AnimationController controoler;

        public CircularProgress(Key key = null) : base(key)
        {
        }

        public override State createState()
        {
            return new CircularProgressAnimationState();
        }
    }

    public class CircularProgressAnimationState : SingleTickerProviderStateMixin<CircularProgress>
    {
        /// <summary>
        /// 画笔宽度
        /// </summary>
        private float strokeWidth = 20;

        /// <summary>
        /// 圆的半径
        /// </summary>
        private float radius;

        /// <summary>
        /// 两端是否为圆角
        /// </summary>
        private bool strokeCapRound=false;

        /// <summary>
        /// 进度值
        /// </summary>
        private float value;

        /// <summary>
        /// 进度条底层背景颜色
        /// </summary>
        private Color backgroundColor = new Color(0xffeeeeee);

        /// <summary>
        /// 进度条的总弧度,2*Mathf.PI整圆
        /// </summary>
        private float totalAngle= 6.28f;

        /// <summary>
        /// 进度条颜色-渐变色
        /// </summary>
        private List<Color> colors;

        /// <summary>
        /// 进度条颜色-渐变色终止点
        /// </summary>
        //private List<float> stops;

        /// <summary>
        /// 进度数值过度-For demo
        /// </summary>
        Animation<float> animation;


        public override void initState()
        {
            base.initState();

            #region 数值动画
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
            #endregion

        }

        public override Widget build(BuildContext context)
        {
            float offset = 0;
            if (strokeCapRound)
            {
                offset = Mathf.Asin(strokeWidth / (radius * 2 - strokeWidth));
            }

            colors = new List<Color>
            {
                new Color(0xffff9a9e),
                new Color(0xfffad0c4)
            };

            radius = 150;
            strokeWidth = 10;
            value = 0.2f;

            return new Container(
                child: Transform.rotate(
                        alignment: Alignment.center,
                        degree:-3.14f/2f-offset,
                        child: new CustomPaint(
                        size: Size.fromRadius(radius),
                        painter: new CircularProgressPainter(
                                strokeWidth: strokeWidth,
                                strokeCapRound: strokeCapRound,
                                backgroundColor:backgroundColor,
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
        private float totalAngle;
        private List<Color> colors;

        public CircularProgressPainter(float strokeWidth = 10, bool strokeCapRound = false, Color backgroundColor = null, float radius = 1, float total = 2 * Mathf.PI, List<float> stops = null, float value = 0, List<Color> colors = null)
        {
            this.strokeWidth = strokeWidth;
            this.strokeCapRound = strokeCapRound;
            this.backgroundColor = backgroundColor;
            this.radius = radius;
            this.stops = stops;
            this.value = value;
            this.totalAngle = total;
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
            if(radius>0)
                size = Size.fromRadius(radius);

            float offset = strokeWidth / 2f;
            this.value = this.value.clamp(0.0f, 1.0f) * totalAngle;
            float start = 0;

            if (strokeCapRound)
            {
                start = Mathf.Asin(strokeWidth / (size.width - strokeWidth));
            }

            Rect rect = new Offset(offset, offset) & new Size(size.width - strokeWidth, size.height - strokeWidth);
            Paint paint = new Paint
            {
                strokeCap = strokeCapRound ? StrokeCap.round : StrokeCap.butt,
                style = PaintingStyle.stroke,
                strokeWidth = this.strokeWidth
            };

            paint.color = backgroundColor;
            canvas.drawArc(rect, start, totalAngle, false, paint);
            if (value > 0)
            {
                paint.shader = new SweepGradient(startAngle: 0, endAngle: value, colors: colors, stops: stops).createShader(rect);
                canvas.drawArc(rect, start, value, false, paint);
            }
        }

        public void removeListener(VoidCallback listener)
        {

        }

        public bool shouldRepaint(CustomPainter oldDelegate) => true;
    }


    #endregion
}