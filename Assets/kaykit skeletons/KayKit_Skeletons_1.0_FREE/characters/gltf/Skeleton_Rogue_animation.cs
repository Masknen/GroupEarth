using Godot;
using System;

public partial class Skeleton_Rogue_animation : Node3D
{
    public override void _Process(double delta)
    {
        ((AnimationPlayer)GetNode("AnimationPlayer")).Play("Walking_A");
    }
    
}
