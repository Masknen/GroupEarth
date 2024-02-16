using Godot;
using System;
using System.Linq.Expressions;

public partial class FireBall : Area3D, IDeflectable
{

    public int speed = 10;
    public int baseSpeed = 10;
    public int damage = 3;
    

    private float fireBallDuration = 20;
    private float timeTick = 0;

    private bool fireAnimation = false;
    // Called when the node enters the scene tree for the first time.

    public static void Fire(Vector3 shooterPos, Transform3D shooterTransform) {
        var fireBall = GD.Load<PackedScene>("res://Scenes/fire_ball.tscn");
        var new_fireBall = fireBall.Instantiate();

        // TODO Change to projectile manager
        PlayerManager.Instance().AddChild(new_fireBall);
        //

        float yRotation = shooterTransform.Basis.GetEuler().Y;
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), shooterPos);
        (new_fireBall as FireBall).Transform = transform;
        (new_fireBall as FireBall).Position = shooterPos + Vector3.Forward.Rotated(Vector3.Up, yRotation)*1.2f;
        (new_fireBall as FireBall).Scale = Vector3.One * 0.001f;


        (new_fireBall as FireBall).fireAnimation = true;
    }

    public override void _Ready()
    {
        BodyEntered += FireBall_BodyEntered;
        //change color of the ball based on damage
        
        
        
    }
    private void FireBall_BodyEntered(Node3D body) {
        if (body as GridMap != null) QueueFree();

        if (body as IDamagable != null) {
            if ((body as IDamagable).Hit(damage)) {
                QueueFree();
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (fireAnimation) {
            StartAnimation(delta);
        } else {

            timeTick += (float)delta;
            if (timeTick > fireBallDuration) {
                QueueFree();
            }
            Position -= Transform.Basis.Z * (float)(speed * delta);
        }
    }


    public void Deflect(float yRotation) {
        damage += 1;
        speed = baseSpeed + (3 * damage);
        fireBallDuration = 20;

        //change color
        MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        StandardMaterial3D material = new StandardMaterial3D();  
        material.AlbedoColor = new Color(1,0.2f * damage, 0);
        mesh.MaterialOverride = material;
        //color changed

        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
    }

    public void FriendDeflect(float yRotation) {
        damage += 1;
        speed = baseSpeed + (3 * damage);
        fireBallDuration = 20;

        //change color
        MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 0.2f * damage, 0);
        mesh.MaterialOverride = material;
        //color changed

        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
    }

    public void ArcDeflect(float yRotation) {
        throw new NotImplementedException();
    }

    public void Hold() {
        speed = 0;
    }

    private void StartAnimation(double delta) {
        Scale = Scale.Lerp(Vector3.One, (float)(delta * 30));
        if (Scale.LengthSquared() >= 2.8f) {
            Scale = Vector3.One;
            fireAnimation = false;
        }
    }
}
