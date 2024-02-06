using Godot;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public partial class enemy1 : CharacterBody3D
{
	[Export] public int moveSpeed = 10;
  
	[Signal] public delegate void HitEventHandler();
  
	private int hp = 1;

	private Node3D target;
	private Node3D player1;
	private Node3D player2;
	private NavigationAgent3D nav;

	private Vector3 direction;
	private Vector3 enemyPosition;

	public override void _Ready()
	{
		//set player1 and player2
		player1 = PlayerManager.Instance().players[0];
		player2 = PlayerManager.Instance().players[1];
		enemyPosition = GlobalPosition;
	}

	

	public override void _Process(double delta)
	{
		//--calculate distance to players and set closest to target--
		//update enemy position
		enemyPosition = GlobalPosition;
		//decide target
		if(player1.Position.DistanceTo(enemyPosition)<
		player2.Position.DistanceTo(enemyPosition)){
			target = player1;
		}
		else{
			target = player2;
		}
		//--get target position and move in that dircetion--
		Velocity = Vector3.Zero;
		nav.TargetPosition = target.GlobalPosition;
		direction = (nav.GetNextPathPosition() - target.GlobalPosition).Normalized();
		Velocity = direction * moveSpeed;
		MoveAndSlide();  
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
