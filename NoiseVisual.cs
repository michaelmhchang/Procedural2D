using Godot;
using Godot.NativeInterop;
using System;
using System.Drawing;
using System.Linq;

public partial class NoiseVisual : Sprite2D
{
    Line2D x, y;

    public override void _Ready()
    {
        x = new Line2D();

        x.AddPoint(new Vector2(0, 0));
        x.AddPoint(new Vector2(100, 0));

        GD.Print(x.DefaultColor);

        x.DefaultColor = new Godot.Color(0x007777FF);

        GD.Print(x.DefaultColor);




        AddChild(x);
    }
}
