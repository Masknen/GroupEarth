using Godot;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public partial class enemy1 : CharacterBody3D
{
    [Export] public int moveSpeed = 10;
    [Export] public float accelerationSpeed = 10.0f;
  
    [Signal] public delegate void HitEventHandler();
  
    private int hp = 1;

    private Node3D target;
    private NavigationAgent3D nav;

    private Vector3 direction;

    public override void _Ready()
    {
        target = GetNode<CharacterBody3D>("res://Scenes//player.tscn");
    }

    

    public override void _Process(double delta)
    {
        
        //need to make some kind of AI tracking - NOT TESTED
        Velocity = Vector3.Zero;
        nav.TargetPosition = target.GlobalPosition;
        direction = nav.GetNextPathPosition() - target.GlobalPosition;
        direction = direction.Normalized();
        Velocity = Velocity.Lerp(direction * moveSpeed, accelerationSpeed);
        MoveAndSlide();  
    }
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
