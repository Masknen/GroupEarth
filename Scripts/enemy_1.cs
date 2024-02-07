using Godot;
using System;

public partial class enemy_1 : Node3D
{
	private Node3D magePos;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		magePos = GetNode<CharacterBody3D>("enemy1/Evil_Mage");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = magePos.Position;
	}
}
