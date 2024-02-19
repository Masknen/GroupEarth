using Godot;
using System;
using System.Linq.Expressions;

public partial class FireBall : Area3D, IDeflectable
{

    public int speed = 5;
    public int baseSpeed = 5;
    public int damage = 1;

    public int sizeOfBall = 0;
    private int speedLimit = 20;


    

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
        (new_fireBall as FireBall).Position = shooterPos + Vector3.Forward.Rotated(Vector3.Up, yRotation)*2.0f;
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
                sizeDown();
                sizeOfBall -= 1;
                if(sizeOfBall <= 0){
                   QueueFree(); 
                }    
                
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
            MeshInstance3D rotateFireball = GetNode<MeshInstance3D>("MeshInstance3D");
            rotateFireball.RotateZ(2);
        }
    }


    public void Deflect(float yRotation) {
        damage += 1;
        if(speed < speedLimit){
        speed = baseSpeed + (3 * damage);
        }
        fireBallDuration = 20;
        sizeOfBall += 1;

        //change color
        MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        StandardMaterial3D material = new StandardMaterial3D();  
        material.AlbedoColor = new Color(1,0.2f * damage, 0);
        mesh.MaterialOverride = material;
        //color changed
        
        //change size -- maybe tie this to a powerup bool? 
        sizeUp();

        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
    }

    public void sizeUp(){
        MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        float newScale = 1 + (0.3f * damage);
        Vector3 newSize = new Vector3(newScale,newScale,newScale);
        mesh.Scale = newSize;
        /*-- works but somehow overrides the deflect angle back at the enemy?? 
        GetNode<MeshInstance3D>("Trail3D").Scale = Vector3.One;
        GetNode<MeshInstance3D>("Trail3D2").Scale = Vector3.One;
        GetNode<MeshInstance3D>("Trail3D").GlobalTransform = mesh.GlobalTransform;
        GetNode<MeshInstance3D>("Trail3D2").GlobalTransform = mesh.GlobalTransform;

        //GetNode<CollisionShape3D >("CollisionShape3D ").Scale = mesh.Scale;
        //collisionShapeSize.Scale = newSize;
        //change size
        */

    }
    public void sizeDown(){
         MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        float newScale = 1 + (0.3f * damage) - 0.3f;
        Vector3 newSize = new Vector3(newScale,newScale,newScale);
        mesh.Scale = newSize;
        revertColor();
    }
    public void revertColor(){
        MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 0.2f * damage -0.2f, 0);
        mesh.MaterialOverride = material;

    }

    public void FriendDeflect(float yRotation) {
        damage += 1;
        if(speed < speedLimit){
        speed = baseSpeed + (3 * damage);
        }
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
