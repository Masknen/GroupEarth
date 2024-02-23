using Godot;
using System;

public partial class InteractableBox : RigidBody3D, IDeflectable, IDamagable

{
	private float deflectForce = 5;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _PhysicsProcess(double delta)
    {
		

    }

    public void Deflect(float yRotation)
    {
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
		ApplyImpulse(Transform.Basis.Z*deflectForce);

    }
    
    public bool Hit(int damage)
    {
        throw new NotImplementedException();
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
