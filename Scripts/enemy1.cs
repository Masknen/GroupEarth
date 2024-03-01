using Godot;
using GodotPlugins.Game;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public partial class enemy1 : CharacterBody3D, IDamagable

{
	
    private enum State {
        Spawning,
        Idle,
        Walking,
        SpellCasting,
        Hit,
        Die,
    }
    private State state = State.Spawning;

    private AnimationPlayer animationPlayer;
    private NavigationAgent3D navAgent;
    private Player target;
    private uint targetOffset;


    // Until have centrebrain stats
    private const float MOVE_SPEED = 3.8f;
    private const float ROT_SPEED = 3;
    private const float DISTANCE_TO_TARGET = 8.5f;
    private const float SHOOT_COOLDOWN = 2.0f;
    private const float SHOOT_OFFSET_SECONDS = 1.6f;

    private int currentHealth = 3;
    private float shootCooldownTick = 0;
    private float animationTiming = 0;

    public override void _Ready() {

        animationPlayer = GetChild<AnimationPlayer>(2);
        navAgent = GetChild<NavigationAgent3D>(3);
        animationPlayer.AnimationFinished += AnimationFinished;

        targetOffset = GD.Randi();

        animationPlayer.Play("Spawn_Ground_Skeletons");
        ArcadeSpawner.Instance.enemies.Add(this);
        //GD.Print(ArcadeSpawner.Instance.enemyArray.Count); 
    }

    private void AnimationFinished(StringName animName) {
        if (animName == "Spawn_Ground_Skeletons") state = State.Walking;
        if (animName == "Spellcast_Long") state = State.Walking;
        if (animName == "Hit_A") state = State.Walking;
        if (animName == "Death_C_Skeletons") QueueFree();
    }

    public override void _Process(double delta) {
        StateMachine();
        if (target == null && PlayerManager.Instance.players.Count >= 1) {
            target = PlayerManager.Instance.players[0];
        }
        if (target != null && state == State.Walking || state == State.SpellCasting) {
            if (GlobalPosition.DistanceTo(target.GlobalPosition) <= DISTANCE_TO_TARGET * 1.5f || state == State.SpellCasting) {
                var direction = GlobalPosition.DirectionTo(target.GlobalPosition);
                RotateToSlerp(new Vector2(direction.X, direction.Z), delta);

                shootCooldownTick -= (float)delta;
                if (shootCooldownTick <= 0 && !target.isDead) {
                    ChangeState(State.SpellCasting);
                    animationTiming -= (float)delta;
                    if (animationTiming <= -SHOOT_OFFSET_SECONDS) {
                        animationTiming = 0;
                        shootCooldownTick = SHOOT_COOLDOWN;
                        FireBall.Fire(Position, Transform);
                        SoundManager.Instance.PlayFireBallSound();
                        state = State.Idle;
                    }
                }
            } else {
                RotateToSlerp(new Vector2(Velocity.X, Velocity.Z), delta);
            }
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (state == State.Walking) {
            MoveToTarget();
            MoveAndSlide();
        }
        if (!IsOnFloor()) {
            Velocity += Vector3.Down;
        }
        ChangeState(State.Walking);
    }

    public bool Hit(Node hitter, int damage) {
        if (state == State.Die) return false;
        currentHealth -= damage;
        if (currentHealth <= 0) {
            ChangeState(State.Die);
            //charge portal
            //PortalAsset.Instance.chargePortal();
            //------
            ArcadeSpawner.Instance.enemies.Remove(this);
            //GD.Print(ArcadeSpawner.Instance.enemyArray.Count);
            //(GetNode("CollisionShape3D") as CollisionShape3D).QueueFree();
            return true;
        }
        ChangeState(State.Hit);
        return true;
    }
    public void Deflect(float yRotation, Node3D target) {
        throw new NotImplementedException();
    }
    public void FriendDeflect(float yRotation) {
        throw new NotImplementedException();
    }
    public void ArcDeflect(float yRotation) {
        throw new NotImplementedException();
    }
    public void Hold() {
        throw new NotImplementedException();
    }
    private void StateMachine() {
        switch (state) {
            case State.Idle:
                animationPlayer.Play("Idle_Combat", 0.1f);
                break;
            case State.Walking:
                animationPlayer.Play("Walking_D_Skeletons", 0.1f);
                break;
            case State.SpellCasting:
                animationPlayer.Play("Spellcast_Long", 0.1f);
                break;
            case State.Hit:
                animationPlayer.Play("Hit_A");
                break;
            case State.Die:
                animationPlayer.Play("Death_C_Skeletons", 0.1f);
                break;
        }
    }
    private void MoveToTarget() {
        
        foreach (var player in PlayerManager.Instance.players) {
            
            if (GlobalPosition.DistanceSquaredTo(target.GlobalPosition) > GlobalPosition.DistanceSquaredTo(player.GlobalPosition)) {
                var nextTarget = target;
                target = player;
                if(target.isDead){
                    target = nextTarget;
                }
                
                break;
            }
            
        }
        if (GameManager.Instance.LastPositionUpdate == 0 && target != null) {
            var targetTimeOffset = (GameManager.Instance.TimeSinceStart + targetOffset) * 0.2f;
            navAgent.TargetPosition = target.GlobalPosition + new Vector3((float)Math.Cos(targetTimeOffset), 0, (float)Math.Sin(targetTimeOffset)) * DISTANCE_TO_TARGET;
        }
        Vector3 direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
        Velocity = (direction * new Vector3(1, 0, 1)) * MOVE_SPEED;
    }
    private void RotateToSlerp(Vector2 inputRotation, double delta) {
        // Calculates angle to create quaternion
        float angle = Vector2.Up.AngleTo(inputRotation);
        Godot.Quaternion quaternionTargetDirection = new Quaternion(-Transform.Basis.Y, angle);
        var quaternion = Transform.Basis.GetRotationQuaternion();

        quaternion = quaternion.Slerp(quaternionTargetDirection, (float)(ROT_SPEED * delta));

        // Sets the rotation to the transform
        Transform3D transform = Transform;
        transform.Basis = new Basis(quaternion);
        Transform = transform;
    }
    private void ChangeState(State newState) {
        if (state == State.Idle || state == State.Walking || newState == State.Hit || newState == State.Die) {
            state = newState;
        }
    }
}

	/*
	
	[Export] public int speed {get; set;} = 1;
  
	public int hp = 3;
	private const float DISTANCE_TO_TARGET = 8.5f;
	private PackedScene fireBall;
	private Player target;
	private CharacterBody3D player1;
	private CharacterBody3D player2;
	private NavigationAgent3D navAgent;
	private Vector3 direction;
	private float MaxTime = 2;
	private float timeTick = 0;
	private float invincibiltyTick = 0;

	private bool isShooting = false;
	private uint targetOffset;

	//for animations
	private string animationMode;
	private string animation;
	private float animationSpeed = 1.0f;
	//---------------------




	public bool isDead = false;

	private bool spawned = true;

	bool IDamagable.Hit(int damage){
		hp += -damage;
		animation = "Hit_B";
		if(hp <= 0){
			isDead = true;
			animation = "Death_A";
            ArcadeSpawner.Instance.enemyArray.Remove(this);
			return false;
        }
		return true;
	}
		
	

	public override void _Ready()
	{
        ((AnimationPlayer)GetNode("AnimationPlayer2")).AnimationFinished += Enemy_AnimationFinished;
        ((AnimationPlayer)GetNode("AnimationPlayer2")).Play("Spawn_Ground_Skeletons");
		ArcadeSpawner.Instance.enemyArray.Add(this);
		//GD.Print(ArcadeSpawner.Instance.enemyArray.Count);
    }

    private void Enemy_AnimationFinished(StringName animName) {
        if (animName == "Spawn_Ground_Skeletons") spawned = false;
		if (animName == "Throw") isShooting = false;
		if (animName == "Hit_B") isShooting = false;
		if (animName == "Walking_D_Skeletons"){isShooting = true; animation = "Throw"; animationSpeed = 1.5f;}
		if (animName == "Death_A") animation  = "Death_A_Pose";
    }
	

	private void AnimationLoop(){
		//animation = animationMode + "," + animationSpeed;
		((AnimationPlayer)GetNode("AnimationPlayer2")).Play(animation,-1,animationSpeed);

	}

	private void searchForClosestPlayer(){
		foreach (var player in PlayerManager.Instance().players) {
            if (GlobalPosition.DistanceSquaredTo(target.GlobalPosition) > GlobalPosition.DistanceSquaredTo(player.GlobalPosition)) {
                target = player;
                break;
            }
        }
		Velocity = Position.DirectionTo(target.GlobalPosition) * speed;
		Vector3 targetAngle = new Vector3(target.Position.X, this.Position.Y, target.Position.Z);
		LookAt(targetAngle);
				
	}
	private void MoveToTarget() {
        foreach (var player in PlayerManager.Instance().players) {
            if (GlobalPosition.DistanceSquaredTo(target.GlobalPosition) > GlobalPosition.DistanceSquaredTo(player.GlobalPosition)) {
                target = player;
                break;
            }
        }
        if (GameManager.Instance.LastPositionUpdate == 0 && target != null) {
            var targetTimeOffset = (GameManager.Instance.TimeSinceStart + targetOffset) * 0.2f;
            navAgent.TargetPosition = target.GlobalPosition + new Vector3((float)Math.Cos(targetTimeOffset), 0, (float)Math.Sin(targetTimeOffset)) * DISTANCE_TO_TARGET;
        }
        Vector3 direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
        Velocity = (direction * new Vector3(1, 0, 1)) * speed;
    }

    public override void _Process(double delta)
	{
		if (!spawned) {
			AnimationLoop();
			if (PlayerManager.Instance().players.Count > 0) {
				//AnimationLoop();
				
				if(!isDead){
					MoveToTarget();
					MoveAndSlide();
				}
				else{
					Position = GlobalPosition;
				}
				
				if(!isShooting && !isDead){
					
					animation = "Walking_D_Skeletons";
				}

				// walking animation
				//((AnimationPlayer)GetNode("AnimationPlayer2")).Play("Walking_D_Skeletons");

				
				timeTick += (float)delta;
				if (timeTick > MaxTime) {
					timeTick -= MaxTime;
					if(isDead){
						
						//GD.Print(ArcadeSpawner.Instance.enemyArray.Count);
						QueueFree();
					}
					//shoot fireball
					FireBall.Fire(Position, Transform);
					

					
					
				}


				/* WILL BE USED LATER TO READ THE MAP AND DODGE WALLS ECT...
				//--get target position and move in that dircetion--
				Velocity = Vector3.Zero;
				nav.TargetPosition = target.GlobalPosition;
				direction = (nav.GetNextPathPosition() - target.GlobalPosition).Normalized();
				Velocity = direction * moveSpeed;
				MoveAndSlide(); 
			}
			else{
				animation = "Idle_B";
				//AnimationLoop();
			}
		}
	}

	/*
	public void die()
	{
		isDead = true;
		if(hp <= 0){
			QueueFree();
 		}
	}
	*/

