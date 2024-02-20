using Godot;
using System;

public partial class NumberPopup : Label3D
{
    private float lifeTime = 2f;

    public static void Create(int number, Vector3 position, Node3D hitThing) {
        var numberPopup = GD.Load<PackedScene>("res://Scenes/GUI Scenes/number_popup.tscn");
        var instance = numberPopup.Instantiate();
        var new_numberPopup = instance.GetChild<Label3D>(0);
        hitThing.GetParent().AddChild(instance);
        new_numberPopup.Transform = MiddleNode.Instance.camera.Transform;
        new_numberPopup.Position = position + new_numberPopup.Transform.Basis.Z * 5;

        new_numberPopup.Text = "" + number;
    }


    public override void _Process(double delta) {
        lifeTime -= (float)delta;
        if (lifeTime < 0 ) {
            QueueFree();
        }
    }
}
