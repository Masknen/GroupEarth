using Godot;
using System;

public partial class BallSpawner : Node3D
{
	private PackedScene ball;
	[Export] private Node3D player;
	[Export] private Node3D spawn;
	private float MaxTime = 2;
	private float timeTick = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ball = GD.Load<PackedScene>("res://TestStuff/testBall.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeTick += (float)delta;
		if (timeTick > MaxTime) {
			timeTick -= MaxTime;

			var instance = ball.Instantiate();
			(instance as testBall).LookAtFromPosition(spawn.GlobalPosition, player.GlobalPosition);
			AddChild(instance);

		}


	}
}
