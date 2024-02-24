using Godot;
using System;
using System.ComponentModel;



public partial class GreenFireBall : Area3D, IDeflectable
{

    private MeshInstance3D rotateFireball;
    private PackedScene fireBallExplotion;

    private CharacterBody3D player1;
	private CharacterBody3D player2;

    public float speed = 5;
    public int baseSpeed = 5;
    public int damage = 1;
    public int sizeOfBall = 0;
    private int speedLimit = 20;

    private Vector3 startPos;

    private int maxSize = 0;

    private Node3D target;

    private bool isFirstHit = true;
    private bool isHolding = false;
    private bool isNotDeflected = true;
    private bool runOnce = true;
    

    private float fireBallDuration = 20;
    private float timeTick = 0;

    private bool fireAnimation = false;
    // Called when the node enters the scene tree for the first time.

    public static void Fire(Vector3 shooterPos, Transform3D shooterTransform) {
        var fireBall = GD.Load<PackedScene>("res://Scenes/green_fire_ball.tscn");
        var new_fireBall = fireBall.Instantiate();
        var greenball = (new_fireBall as GreenFireBall);
        PlayerManager.Instance().AddChild(new_fireBall);
        greenball.startPos = shooterPos;
        greenball.fireAnimation = true;
        float yRotation = shooterTransform.Basis.GetEuler().Y;
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), shooterPos);
        greenball.Transform = transform;
        greenball.Scale = Vector3.One * 0.001f;
        greenball.Position = shooterPos + Vector3.Forward.Rotated(Vector3.Up, yRotation)*1.5f;
        greenball.fireAnimation = true;
        
    }
    private void closestTarget(){
        player1 = PlayerManager.Instance().players[0];
		player2 = PlayerManager.Instance().players[1];
		if (player1.GlobalPosition.DistanceTo(GlobalPosition) <
		player2.GlobalPosition.DistanceTo(GlobalPosition)) {
			target = player1;
		} else { target = player2; }
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
    }
    private void FireBall_BodyEntered(Node3D body) {
        if (body as GridMap != null) { QueueFree(); hitExplosion(); }

        if (body as IDamagable != null) {
            
            if ((body as IDamagable).Hit(damage) && !isFirstHit) {
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
        isFirstHit = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        if(runOnce){
            Position = startPos;
            closestTarget();
            runOnce = false;
        }
        if(isNotDeflected){
        LookAt(target.GlobalPosition);
        Position = Position.MoveToward(target.GlobalPosition, speed * (float)delta);
        }
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
        isNotDeflected = false;
        if(speed < speedLimit){
        speed = baseSpeed + (3 * damage);
        }
        fireBallDuration = 20;
        
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
        isHolding = false;
        
    }

    public void FriendDeflect(float yRotation) {
        //not in use
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
  

    }
    public void sizeDown(){
        float newScale = 1 + (0.3f * damage);
        Vector3 newSize = new Vector3(newScale,newScale,newScale);
        rotateFireball.Scale = newSize;

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

