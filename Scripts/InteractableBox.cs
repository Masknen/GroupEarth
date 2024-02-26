using Godot;
using System;

public partial class InteractableBox : RigidBody3D, IDeflectable, IDamagable

{
	private float deflectForce = 30;
    private PackedScene  fireBallExplotion;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        BodyEntered += BoxBodyEntered;
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
    private void BoxBodyEntered(Node body)
    {
        GD.Print(LinearVelocity.Length());
        if(LinearVelocity.Length()>3f)
        {
            if (body as GridMap != null) 
            { 
                QueueFree(); 
                hitExplosion(); 
            }

            if (body as IDamagable != null) 
            {
                if ((body as IDamagable).Hit((int)LinearVelocity.Length()))
                {
                    hitExplosion();
                    NumberPopup.Create((int)LinearVelocity.Length(), (body as Node3D).GlobalPosition, body as Node3D);
                    QueueFree();
                }
            }
        }
    }

    public void Deflect(float yRotation, Node3D target)
    {
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
		ApplyImpulse(-Transform.Basis.Z*deflectForce + Vector3.Up*deflectForce/4);
    }

    
    public bool Hit(int damage)
    {
        throw new NotImplementedException();
    }
    private void hitExplosion() {
        var new_fireBallExploation = fireBallExplotion.Instantiate();
        (new_fireBallExploation as ImpactExplotion).Position = Position;
        GetParent().AddChild(new_fireBallExploation);
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
