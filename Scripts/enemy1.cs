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
	[Export] public int speed {get; set;} = 1;
  
	public int hp = 3;
	private PackedScene fireBall;
	private Node3D target;
	private CharacterBody3D player1;
	private CharacterBody3D player2;
	private NavigationAgent3D nav;
	private Vector3 direction;
	private float MaxTime = 2;
	private float timeTick = 0;
	private float invincibiltyTick = 0;

	private bool isShooting = false;

	//for animations
	private string animationMode;
	private string animation;
	private float animationSpeed = 1.0f;
	//---------------------




	public bool isDead = false;

	private bool spawned = true;

	bool IDamagable.Hit(int damage){
		hp += -damage;
		if(hp <= 0){
			QueueFree();
		}
		return true;
	}
		
	

	public override void _Ready()
	{
        ((AnimationPlayer)GetNode("AnimationPlayer2")).AnimationFinished += Enemy_AnimationFinished;
        ((AnimationPlayer)GetNode("AnimationPlayer2")).Play("Spawn_Ground_Skeletons");

    }

    private void Enemy_AnimationFinished(StringName animName) {
        if (animName == "Spawn_Ground_Skeletons") spawned = false;
		if (animName == "Throw") isShooting = false;
		if (animName == "Walking_D_Skeletons"){isShooting = true; animation = "Throw"; animationSpeed = 1.5f;}
    }
	

	private void AnimationLoop(){
		//animation = animationMode + "," + animationSpeed;
		((AnimationPlayer)GetNode("AnimationPlayer2")).Play(animation,-1,animationSpeed);

	}

	private void searchForClosestPlayer(){
		player1 = PlayerManager.Instance().players[0];
		player2 = PlayerManager.Instance().players[1];
		if (player1.GlobalPosition.DistanceTo(GlobalPosition) <
		player2.GlobalPosition.DistanceTo(GlobalPosition)) {
			target = player1;
		} else { target = player2; }
		Velocity = Position.DirectionTo(target.GlobalPosition) * speed;
		Vector3 targetAngle = new Vector3(target.Position.X, this.Position.Y, target.Position.Z);
		LookAt(targetAngle);
				
	}

    public override void _Process(double delta)
	{
		if (!spawned) {
			AnimationLoop();
			if (PlayerManager.Instance().players.Count > 0) {
				//AnimationLoop();
				searchForClosestPlayer();
				if(!isShooting){
					animation = "Walking_D_Skeletons";
				}

				// walking animation
				//((AnimationPlayer)GetNode("AnimationPlayer2")).Play("Walking_D_Skeletons");

				MoveAndSlide();
				timeTick += (float)delta;
				if (timeTick > MaxTime) {
					timeTick -= MaxTime;
					((AnimationPlayer)GetNode("AnimationPlayer2")).AnimationFinished += Enemy_AnimationFinished;
					//shoot fireball
					FireBall.Fire(Position, Transform);
					//((AnimationPlayer)GetNode("AnimationPlayer2")).AnimationFinished += Enemy_AnimationFinished;

					
					
				}


				/* WILL BE USED LATER TO READ THE MAP AND DODGE WALLS ECT...
				//--get target position and move in that dircetion--
				Velocity = Vector3.Zero;
				nav.TargetPosition = target.GlobalPosition;
				direction = (nav.GetNextPathPosition() - target.GlobalPosition).Normalized();
				Velocity = direction * moveSpeed;
				MoveAndSlide(); */
			}
			else{
				animation = "Idle_B";
				//AnimationLoop();
			}
		}
	}
	
	public void fireFireBall(){
		//casting animation
		
		var new_fireBall = fireBall.Instantiate();
		(new_fireBall as FireBall).Position = Position;
		//LookAt(target.GlobalPosition);

		// Max Changes
		float yRotation = -GlobalPosition.DirectionTo(target.GlobalPosition).SignedAngleTo(Vector3.Forward, Vector3.Up);
		Transform3D transform = new Transform3D(new Basis(Transform.Basis.Y, yRotation), Position);
		(new_fireBall as FireBall).Transform = transform;
		// \Max Changes


		//(new_fireBall as fireball).LookAtFromPosition(Position, target.GlobalPosition);
		GetParent().AddChild(new_fireBall);
		
		
		
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
}
