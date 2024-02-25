using Godot;
using System;
using System.ComponentModel;



public partial class GreenFireBall : Area3D, IDeflectable
{

    private MeshInstance3D rotateFireball;
    private PackedScene fireBallExplotion;

    public float speed = 5;
    public int baseSpeed = 5;
    public int damage = 1;
    public int sizeOfBall = 0;
    private int speedLimit = 20;

    private Vector3 startPos;

    private int maxSize = 0;

    private CharacterBody3D target;

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
        //greenball.startPos = shooterPos;

        float yRotation = shooterTransform.Basis.GetEuler().Y;
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), shooterPos);
        greenball.Transform = transform;
        greenball.Scale = Vector3.One * 0.001f;
        greenball.Position = shooterPos + Vector3.Forward.Rotated(Vector3.Up, yRotation)*1.5f;

        greenball.fireAnimation = true;
        
    }
    private void closestPlayer(){
        target = PlayerManager.Instance().players[0];
        foreach (var player in PlayerManager.Instance().players) {
            if (player.GlobalPosition.DistanceTo(GlobalPosition) < target.GlobalPosition.DistanceTo(GlobalPosition)) {
                target = player;
            }
        }
    }
    private void closestEnemy(Node3D hitBody) {
        CharacterBody3D newTarget = null;
        foreach (var enemy in ArcadeSpawner.Instance.enemyArray) {
            newTarget ??= enemy;
            if (enemy.GlobalPosition.DistanceTo(GlobalPosition) < newTarget.GlobalPosition.DistanceTo(GlobalPosition) && !enemy.GlobalPosition.IsEqualApprox(hitBody.GlobalPosition)) {
                newTarget = enemy;
            }
        }
        target = newTarget;
        GD.Print(target + " | normal");
    }
    private void closestEnemyErrorFix() {
        CharacterBody3D newTarget = null;
        foreach (var enemy in ArcadeSpawner.Instance.enemyArray) {
            newTarget ??= enemy;
            if (enemy.GlobalPosition.DistanceTo(GlobalPosition) < newTarget.GlobalPosition.DistanceTo(GlobalPosition)) {
                newTarget = enemy;
            }
        }
        target = newTarget;
        GD.Print(target + " | error");
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
        closestPlayer();
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
                if (sizeOfBall <= 0) {
                    QueueFree();
                }
                closestEnemy(body);
            }
            
        }
        isFirstHit = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        if(runOnce){
            //Position = startPos;
            //closestTarget();
            runOnce = false;
        }
        if(!isNotDeflected){
            //LookAt(target.GlobalPosition);
            if (!ArcadeSpawner.Instance.enemyArray.Contains(target) && target != null) {
                closestEnemyErrorFix();
            }
        }
        if (fireAnimation) {
            StartAnimation(delta);
        } else {
            timeTick += (float)delta;
            if (timeTick > fireBallDuration) {
                QueueFree();
            }
            //Position -= Transform.Basis.Z * (float)(speed * delta);
            rotateFireball.RotateZ(0.2f);

            Vector3 directionV3 = Vector3.Zero;
            Vector2 direction = Vector2.Zero;
            if (target != null) {
                try {
                    directionV3 = GlobalPosition.DirectionTo(target.GlobalPosition);
                    direction = new Vector2(directionV3.X, directionV3.Z);
                } catch (Exception) { closestEnemyErrorFix(); }

                RotateToSlerp(direction, delta);
            }
            Position -= Transform.Basis.Z * (float)(speed * delta);
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
    public void Deflect(float yRotation, Node3D target) {
        this.target = (CharacterBody3D)target;
        if(speed < speedLimit){
        speed = baseSpeed + (3 * damage);
        }
        fireBallDuration = 20;
        
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
        isHolding = false;
        isNotDeflected = false;
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

    private void RotateToSlerp(Vector2 inputRotation, double delta) {
        // Calculates angle to create quaternion
        float angle = Vector2.Up.AngleTo(inputRotation);
        Godot.Quaternion quaternionTargetDirection = new Quaternion(-Transform.Basis.Y, angle);
        var quaternion = Transform.Basis.GetRotationQuaternion();

        float turnSpeed = 35.0f / (speed+1);//Seeking amount

        quaternion = quaternion.Slerp(quaternionTargetDirection, (turnSpeed) * (float)delta);

        // Sets the rotation to the transform
        Transform3D transform = Transform;
        transform.Basis = new Basis(quaternion);
        Transform = transform;
    }
}

