using Godot;
using System;

public partial class NumberPopup : Label3D
{
    private float lifeTime = 1.5f;

    public static void Create(int number, Vector3 position, Node3D parent) {
        var numberPopup = GD.Load<PackedScene>("res://Scenes/GUI Scenes/number_popup.tscn");
        var instance = numberPopup.Instantiate();
        var new_numberPopup = (instance as Label3D);
        parent.GetParent().AddChild(instance);
        //new_numberPopup.Transform = transform;
        new_numberPopup.Rotation = MiddleNode.Instance.camera.Rotation;
        new_numberPopup.Position = parent.Position + new_numberPopup.Transform.Basis.Z * 5;

        new_numberPopup.Text = "" + number;
    }

    public override void _Process(double delta) {
        lifeTime -= (float)delta;
        if (lifeTime < 0 ) {
            QueueFree();
        }
    }
}
