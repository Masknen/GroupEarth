using Godot;
using System;
using System.Linq.Expressions;

public partial class FireBall : Area3D, IDeflectable
{

    public int speed = 10;
    public int damage = 0;
    

    private float fireBallDuration = 20;
    private float timeTick = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += FireBall_BodyEntered;
        var mech = GetNode<MeshInstance3D>("MeshInstance3D");
        
        
    }

    private void FireBall_BodyEntered(Node3D body) {
        GD.Print(body);
        if (body as IDamagable != null) {
            (body as IDamagable).Hit(10);
        }
        if(body is enemy1){
            (body as enemy1).hp += -damage;
            (body as enemy1).die();
            GD.Print("works");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
        timeTick += (float)delta;
        if(timeTick > fireBallDuration){
            QueueFree();
        }
        Position -= Transform.Basis.Z * (float)(speed * delta);
        
    }

    public void Deflect(float yRotation) {
        speed += 10;
        damage += 1;
        fireBallDuration = 20;
        GD.Print(damage);
        GD.Print("Hit");
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
    }

    public void FriendDeflect(float yRotation) {
        GD.Print("Hit");
        speed += 10;
        damage += 1;
        fireBallDuration = 20;
        GD.Print(damage);
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
