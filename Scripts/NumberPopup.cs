using Godot;
using System;

public partial class NumberPopup : Label3D
{
    private float lifeTime = 3;

    public static void Create(int number, Transform3D transform, Node parent) {
        var numberPopup = GD.Load<PackedScene>("res://Scenes/GUI Scenes/number_popup.tscn");
        var instance = numberPopup.Instantiate();
        var new_numberPopup = (instance as Label3D);
        parent.GetParent().AddChild(instance);
        new_numberPopup.Transform = transform;
        new_numberPopup.Position += Vector3.Up*5;
        new_numberPopup.Position += -Vector3.Forward;

        new_numberPopup.Text = "" + number;
    }

    public override void _Process(double delta) {
        lifeTime -= (float)delta;
        if (lifeTime < 0 ) {
            QueueFree();
        }
    }
}
