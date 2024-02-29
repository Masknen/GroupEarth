using Godot;
using System;

public partial class InteractableBox : RigidBody3D, IDeflectable

{
	private float deflectForce = 20;
    private PackedScene  fireBallExplotion;
    public bool destroy = false; 
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
        if(LinearVelocity.Length()>3f && !body.IsInGroup("floor"))
        {
            if (body as GridMap != null) 
            { 
                destroy = true;
                SoundManager.Instance.PlayFireBallHitSound();
                hitExplosion(); 
            }

            if (body as IDamagable != null) 
            {
                if ((body as IDamagable).Hit((int)LinearVelocity.Length()))
                {
                    SoundManager.Instance.PlayFireBallHitSound();
                    hitExplosion();
                    NumberPopup.Create((int)LinearVelocity.Length(), (body as Node3D).GlobalPosition, body as Node3D);
                    destroy = true; 
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
