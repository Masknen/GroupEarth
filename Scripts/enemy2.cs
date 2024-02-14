using Godot;
using System;


public partial class enemy2 : CharacterBody3D
{
	[Export] public int speed {get; set;} = 1;
	private Node3D target;
	private CharacterBody3D player1;
	private CharacterBody3D player2;

	private Vector3 direction;

   

	public override void _Process(double delta){

		if (PlayerManager.Instance().players.Count > 0) {
			player1 = PlayerManager.Instance().players[0];
			player2 = PlayerManager.Instance().players[1];
			if(player1.GlobalPosition.DistanceTo(GlobalPosition)<
			player2.GlobalPosition.DistanceTo(GlobalPosition)){
				target = player1;
			}else { target = player2;}
		
		Velocity = Position.DirectionTo(target.GlobalPosition) * speed;
		LookAt(target.GlobalPosition);
		MoveAndSlide();

	}
	}
}
