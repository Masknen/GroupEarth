using Godot;
using System;

public partial class enemy_3 : CharacterBody3D, IDamagable, IDeflectable
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
    private const float MOVE_SPEED = 1.8f;
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
        if (target == null && PlayerManager.Instance().players.Count >= 1) {
            target = PlayerManager.Instance().players[0];
        }
        if (target != null && state == State.Walking || state == State.SpellCasting) {
            if (GlobalPosition.DistanceTo(target.GlobalPosition) <= DISTANCE_TO_TARGET * 1.5f || state == State.SpellCasting) {
                var direction = GlobalPosition.DirectionTo(target.GlobalPosition);
                RotateToSlerp(new Vector2(direction.X, direction.Z), delta);

                shootCooldownTick -= (float)delta;
                if (shootCooldownTick <= 0) {
                    ChangeState(State.SpellCasting);
                    animationTiming -= (float)delta;
                    if (animationTiming <= -SHOOT_OFFSET_SECONDS) {
                        animationTiming = 0;
                        shootCooldownTick = SHOOT_COOLDOWN;
                        GreenFireBall.Fire(Position, Transform);
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

    public bool Hit(int damage) {
        if (state == State.Die) return false;
        currentHealth -= damage;
        if (currentHealth <= 0) {
            ChangeState(State.Die);
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
        foreach (var player in PlayerManager.Instance().players) {
            if (GlobalPosition.DistanceSquaredTo(target.GlobalPosition) > GlobalPosition.DistanceSquaredTo(player.GlobalPosition) && !target.isDead) {
                target = player;
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
