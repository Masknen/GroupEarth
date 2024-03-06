using Godot;
using System;

public partial class HealthPotion : RigidBody3D, IDeflectable
{
	
	private float deflectForce = 20;
    private PackedScene  fireBallExplotion;
    public bool destroy = false; 
    private int health = 2; // how meny health shoul be every potion. 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        BodyEntered += HealthPotionEntered;
        MaxContactsReported = 10;
        fireBallExplotion = GD.Load<PackedScene>("res://AnimationScenes/ImpactExplosion.tscn");
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _PhysicsProcess(double delta)
    {
      
    }

    private void HealthPotionEntered(Node body)
    {
        
        if (body as Player != null) 
        {
             // to controll att palyer not have more then normal health with taking potion. 
            (body as Player).stats.ModifyStat(Stat.StatType.CurrentHealth ,health);
            NumberPopupHealth.Create(health, Position, body as Node3D);
            if ((body as Player).stats.GetStat(Stat.StatType.CurrentHealth) >= (body as Player).stats.GetStat(Stat.StatType.MaxHealth))
            {
                (body as Player).stats.setStat(Stat.StatType.CurrentHealth, (body as Player).stats.GetStat(Stat.StatType.MaxHealth));

            }
            this.QueueFree();
            
        }
        
        
    }

    public void Deflect(float yRotation, Node3D target)
    {
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
		ApplyImpulse(-Transform.Basis.Z*deflectForce + Vector3.Up*deflectForce/4);
        health++; 
    }



	public void Hold()
    {
        
	}
	public void FriendDeflect(float yRotation)
    {
        throw new NotImplementedException();
    }

    public void ArcDeflect(float yRotation)
    {
        throw new NotImplementedException();
    }
}
