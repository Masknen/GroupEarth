using Godot;
using GodotPlugins.Game;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public partial class enemy1 : CharacterBody3D
{
	[Export] public int speed {get; set;} = 1;

  
	[Signal] public delegate void HitEventHandler();
  
	private int hp = 1;
	private PackedScene fireBall;
	private Node3D target;
	private CharacterBody3D player1;
	private CharacterBody3D player2;
	private NavigationAgent3D nav;
	private Vector3 direction;
	private float MaxTime = 2;
	private float timeTick = 0;

    public override void _Ready()
    {
		fireBall = GD.Load<PackedScene>("res://Scenes/FireBall.tscn");
        
    }

    public override void _Process(double delta)
	{
		player1 = PlayerManager.Instance().players[0];
		/*
		player2 = PlayerManager.Instance().players[1];
		if(player1.GlobalPosition.DistanceTo(Position)<
		player2.GlobalPosition.DistanceTo(Position)){
			target = player1;
		}else { target = player2;}
		*/
		target = player1;
		Velocity = Position.DirectionTo(target.Position) * speed;
		LookAtFromPosition(Position, target.GlobalPosition);
		MoveAndSlide();
		timeTick += (float)delta;
		if (timeTick > MaxTime) {
			timeTick -= MaxTime;
			fireFireBall();
		}
		
		/* WILL BE USED LATER TO READ THE MAP AND DODGE WALLS ECT...
		//--get target position and move in that dircetion--
		Velocity = Vector3.Zero;
		nav.TargetPosition = target.GlobalPosition;
		direction = (nav.GetNextPathPosition() - target.GlobalPosition).Normalized();
		Velocity = direction * moveSpeed;
		MoveAndSlide(); */
	}
	public void fireFireBall(){
		var new_fireBall = fireBall.Instantiate();
		GD.Print("fireball!");
		(new_fireBall as fireball).Position = Position;
		//LookAt(target.GlobalPosition);
		(new_fireBall as fireball).LookAtFromPosition(Position, target.GlobalPosition);
		GetParent().AddChild(new_fireBall);
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
