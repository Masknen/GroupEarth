using Godot;
using System;
using System.Runtime;

public partial class FireBallSpawner : Node3D
{
	private PackedScene fireBall;
	[Export] private Node3D target;
	
	
	private float MaxTime = 2;
	private float timeTick = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fireBall = GD.Load<PackedScene>("res://Scenes/fire_ball.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		//fix for closest target...
		target = PlayerManager.Instance().players[0];
		timeTick += (float)delta;
		if (timeTick > MaxTime) {
			timeTick -= MaxTime;
			GD.Print("fireball!");

			var instance = fireBall.Instantiate();
			(instance as FireBall).LookAtFromPosition(Position, target.GlobalPosition);
			AddChild(instance);

		}


	}
}

