using Godot;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public partial class enemy1 : CharacterBody3D
{
	[Export] public int moveSpeed {get; set;} = 1;

  
	[Signal] public delegate void HitEventHandler();
  
	private int hp = 1;

	private Node3D target;
	private CharacterBody3D player1;
	private CharacterBody3D player2;
	private NavigationAgent3D nav;
	private Vector3 direction;
	
	public override void _Process(double delta)
	{
		player1 = PlayerManager.Instance().players[0];
		player2 = PlayerManager.Instance().players[1];
		if(player1.GlobalPosition.DistanceTo(Position)<
		player2.GlobalPosition.DistanceTo(Position)){
			target = player1;
		}else { target = player2;}
		//target = player1;
		Velocity = Position.DirectionTo(target.Position) * moveSpeed;
		LookAtFromPosition(Position, target.GlobalPosition);
		MoveAndSlide();
		
		/* WILL BE USED LATER TO READ THE MAP AND DODGE WALLS ECT...
		//--get target position and move in that dircetion--
		Velocity = Vector3.Zero;
		nav.TargetPosition = target.GlobalPosition;
		direction = (nav.GetNextPathPosition() - target.GlobalPosition).Normalized();
		Velocity = direction * moveSpeed;
		MoveAndSlide(); */
	}

	//make method to shoot a object twoards the closest player...
	public void die()
	{
		if(hp <= 0){
			QueueFree();
		}
	}
	public void isHit()
	{
		EmitSignal(SignalName.Hit);
		hp += -1;
		die();  
	}

}
