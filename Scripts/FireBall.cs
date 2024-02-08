using Godot;
using System; 
public partial class fireball : Area3D, IDeflectable
{

    public int speed = 10;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Position -= Transform.Basis.Z * (float)(speed * delta);
    }

    public void Deflect(float yRotation) {
        GD.Print("Hit");
        Transform3D transform = new Transform3D(new Basis(Transform.Basis.Y, yRotation), Position);
        Transform = transform;
    }

    public void FriendDeflect(float yRotation) {
        GD.Print("Hit");
        Transform3D transform = new Transform3D(new Basis(Transform.Basis.Y, yRotation), Position);
        Transform = transform;
    }

    public void ArcDeflect(float yRotation) {
        throw new NotImplementedException();
    }
}
