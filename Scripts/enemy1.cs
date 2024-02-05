using Godot;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;

public partial class enemy1 : CharacterBody3D
{
    [Export] public int moveSpeed = 10;
  
    [Signal] public delegate void HitEventHandler();
  
    private int hp = 1;

    private Node3D target;

    public override void _Ready()
    {
        target = GetNode<CharacterBody3D>("res://Scenes//player.tscn");
    }

    

    public override void _Process(double delta)
    {
        
    }
    public void isHit()
    {
		EmitSignal(SignalName.Hit);
        hp += -1;
        if(hp == 0){
            QueueFree();
        }
        
	}

}
