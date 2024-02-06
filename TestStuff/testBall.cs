using Godot;
using System;

public partial class testBall : Node3D
{
	[Export] public float speed = 150;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position -= Transform.Basis.Z * (float)(speed * delta);
	}
}
