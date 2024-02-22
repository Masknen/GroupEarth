using Godot;
using System;



public partial class GreenFireBall : Area3D, IDeflectable
{

    private MeshInstance3D rotateFireball;
    private PackedScene fireBallExplotion;

    public int speed = 5;
    public int baseSpeed = 5;
    public int damage = 1;
    public int sizeOfBall = 0;
    private int speedLimit = 20;

    private int maxSize = 0;

    

    private bool isFirstHit = true;
    private bool isHolding = false;
    

    private float fireBallDuration = 20;
    private float timeTick = 0;

    private bool fireAnimation = false;
    // Called when the node enters the scene tree for the first time.

    public static void Fire(Vector3 shooterPos, Transform3D shooterTransform) {
        var fireBall = GD.Load<PackedScene>("res://Scenes/green_fire_ball.tscn");
        var new_fireBall = fireBall.Instantiate();

        // TODO Change to projectile manager
        PlayerManager.Instance().AddChild(new_fireBall);
        //

        float yRotation = shooterTransform.Basis.GetEuler().Y;
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), shooterPos);
        (new_fireBall as GreenFireBall).Transform = transform;
        (new_fireBall as GreenFireBall).Position = shooterPos + Vector3.Forward.Rotated(Vector3.Up, yRotation)*1.5f;
        (new_fireBall as GreenFireBall).Scale = Vector3.One * 0.001f;


        (new_fireBall as GreenFireBall).fireAnimation = true;
    }

    private void hitExplosion() {
        var new_fireBallExploation = fireBallExplotion.Instantiate();
        (new_fireBallExploation as ImpactExplotion).Position = Position;
        GetParent().AddChild(new_fireBallExploation);
    }

    public override void _Ready() {
        fireBallExplotion = GD.Load<PackedScene>("res://AnimationScenes/ImpactExplosion.tscn");
        rotateFireball = GetNode<MeshInstance3D>("MeshInstance3D");
        
        BodyEntered += FireBall_BodyEntered;
        //change color of the ball based on damage
    }
    private void FireBall_BodyEntered(Node3D body) {
        if (body as GridMap != null) { QueueFree(); hitExplosion(); }

        if (body as IDamagable != null) {
            if ((body as IDamagable).Hit(damage)) {
                hitExplosion();
                NumberPopup.Create(damage, body.GlobalPosition, body);
                damage -= 1;
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
            rotateFireball.RotateZ(0.2f);
        }
        if(isHolding){
            continuousSizeUp(delta);
        }

    }


    private void continuousSizeUp(double delta){
        float timeTick2 = 0;
            float MaxTime = 0.5f;
            timeTick2 += (float)delta;
				if (timeTick > MaxTime && maxSize < 6) {
					timeTick -= MaxTime;
					sizeUp();
                    maxSize += 1;
                    damage += 1; 
                    sizeOfBall += 1;
				}

    }
    public void Deflect(float yRotation) {
        if(speed < speedLimit){
        speed = baseSpeed + (3 * damage);
        }
        fireBallDuration = 20;
        
        
        /*
        //change color
        StandardMaterial3D material = new StandardMaterial3D();  
        material.AlbedoColor = new Color(1,0.2f * damage, 0);
        rotateFireball.MaterialOverride = material;
        //color changed
        */
        
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
        isHolding = false;
        maxSize = 0;
    }

    public void FriendDeflect(float yRotation) {
        damage += 1;
        speed = baseSpeed + (3 * damage);
        fireBallDuration = 20;
        /*
        //change color
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(0, 1, 0.2f * damage);
        rotateFireball.MaterialOverride = material;
        //color changed
        */
        

        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
    }

    public void ArcDeflect(float yRotation) {
        throw new NotImplementedException();
    }

    public void Hold() {
        speed = 0;
        isHolding = true;
    }
     
     public void sizeUp(){
        float newScale = 1 + (0.3f * maxSize);
        Vector3 newSize = new Vector3(newScale,newScale,newScale);
        rotateFireball.Scale = newSize;
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
        float newScale = 1 + (0.3f * damage); //- 0.3f;
        Vector3 newSize = new Vector3(newScale,newScale,newScale);
        rotateFireball.Scale = newSize;
        //revertColor();
    }
   
    public void revertColor(){
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 0.2f * damage -0.2f, 0);
        rotateFireball.MaterialOverride = material;

    }
    

    private void StartAnimation(double delta) {
        Scale = Scale.Lerp(Vector3.One, (float)(delta * 30));
        if (Scale.LengthSquared() >= 2.8f) {
            Scale = Vector3.One;
            fireAnimation = false;
        }
    }
}

