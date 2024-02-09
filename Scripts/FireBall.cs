using Godot;
using System; 
public partial class FireBall : Area3D, IDeflectable
{

    public int speed = 10;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += FireBall_BodyEntered;
    }

    private void FireBall_BodyEntered(Node3D body) {
        GD.Print(body);
        if (body as IDamagable != null) {
            (body as IDamagable).Hit(10);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Position -= Transform.Basis.Z * (float)(speed * delta);
    }

    public void Deflect(float yRotation) {
        speed = 10;
        GD.Print("Hit");
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
    }

    public void FriendDeflect(float yRotation) {
        GD.Print("Hit");
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
    }

    public void ArcDeflect(float yRotation) {
        throw new NotImplementedException();
    }

    public void Hold() {
        speed = 0;
    }
}
