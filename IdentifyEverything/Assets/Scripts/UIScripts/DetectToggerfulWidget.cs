using Unity.UIWidgets.widgets;
using System;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

public class DetectToggerfulWidget : StatefulWidget
{

    internal string name;
    internal Color defaultColor = new Color(0xff8E8E8E);
    internal int id;
    internal bool initFirst = false;
    internal Action<bool> IsSelected;

    public DetectToggerfulWidget(Key key = null) : base(key)
    {
    }


    public override State createState()
    {
        return new ToggerState();
    }
}

public class ToggerState : State<DetectToggerfulWidget>
{

    public override void initState()
    {
        base.initState();
        widget.IsSelected += IsSelectedCallback;
        if (widget.id == 2 && !widget.initFirst)
        {
            widget.IsSelected(true);
            widget.initFirst = true;
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
