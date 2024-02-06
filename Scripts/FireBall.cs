using Godot;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;

public partial class FireBall : Node3D
{
	[Export] public float speed = 100;

	


	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position -= Transform.Basis.Z * (float)(speed * delta);
	}
}
