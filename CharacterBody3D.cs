using Godot;
using System;

public partial class CharacterBody3D : Godot.CharacterBody3D
{
    [Export] int ID = 0;

    Vector3 Direction = Vector3.Zero;

    [Export] float speed = 15;
    public override void _PhysicsProcess(double delta){
        Vector2 inputRotation = new Vector2(Rotation.X, Rotation.Y);
        if (Input.GetJoyAxis(ID, JoyAxis.RightX) > 0.3f || Input.GetJoyAxis(ID, JoyAxis.RightX) < -0.3f)
        {
            Direction.X = Input.GetJoyAxis(ID, JoyAxis.RightX);
            GD.Print("X = ", Direction.X);
        }
        if (Input.GetJoyAxis(ID, JoyAxis.RightY) > 0.3f || Input.GetJoyAxis(ID, JoyAxis.RightY) < -0.3f)
        {
            Direction.Z = Input.GetJoyAxis(ID, JoyAxis.RightY);
            GD.Print("Y = ", Direction.Y);
        }

        Velocity = Direction * speed;
        MoveAndSlide();
        
    }
}
