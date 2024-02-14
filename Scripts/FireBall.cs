using Godot;
using System;
using System.Linq.Expressions;

public partial class FireBall : Area3D, IDeflectable
{

    public int speed = 10;
    public int baseSpeed = 10;
    public int damage = 0;
    

    private float fireBallDuration = 20;
    private float timeTick = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += FireBall_BodyEntered;
        
        //change color of the ball based on damage
        
        
        
    }

    private void FireBall_BodyEntered(Node3D body) {
        if (body as GridMap != null) QueueFree();

        if (body as IDamagable != null) {
            if ((body as IDamagable).Hit(10)) {
                QueueFree();
            }
        }
        if(body is enemy1){
            (body as enemy1).hp += -damage;
            (body as enemy1).die();
           

            if(damage > 0){
                if((body as enemy1).isDead){
                    QueueFree();
                }
            }
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
        damage += 1;
        speed = baseSpeed + (3 * damage);
        fireBallDuration = 20;

        //change color
        MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        StandardMaterial3D material = new StandardMaterial3D();  
        material.AlbedoColor = new Color(1,0,01 * damage,0);
        mesh.MaterialOverride = material;
        //color changed

        GD.Print(damage);
        GD.Print("Hit");
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
    }

    public void FriendDeflect(float yRotation) {
        GD.Print("Hit");
        damage += 1;
        speed = baseSpeed + (3 * damage);
        fireBallDuration = 20;

        //change color
        MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 0, 01 * damage, 0);
        mesh.MaterialOverride = material;
        //color changed

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
