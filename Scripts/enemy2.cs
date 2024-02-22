using Godot;
using System;


public partial class enemy2 : CharacterBody3D, IDamagable, IDeflectable {

	private AnimationPlayer animationPlayer;
    private NavigationAgent3D navAgent;
    private Player target;
    private uint targetOffset;

    private bool spawned = true;


    // Until have centrebrain stats
    private const float MOVE_SPEED = 2.5f;
    private const float ROT_SPEED = 4;
    private const float DISTANCE_TO_TARGET = 7;
    private const float SHOOT_COOLDOWN = 2.0f;

    private float shootTick = 0;

    public override void _Ready() {

        animationPlayer = GetChild<AnimationPlayer>(2);
        navAgent = GetChild<NavigationAgent3D>(3);
        animationPlayer.AnimationFinished += AnimationFinished;

        navAgent.DebugEnabled = true;
        targetOffset = GD.Randi();

        animationPlayer.Play("Spawn_Ground_Skeletons");

    }

    private void AnimationFinished(StringName animName) {
        if (animName == "Spawn_Ground_Skeletons") spawned = false;
    }

    public override void _Process(double delta) {
        if (target == null && PlayerManager.Instance().players.Count >= 1) {
            target = PlayerManager.Instance().players[0];
        }

        if (target != null && !spawned) {
            if (GlobalPosition.DistanceTo(target.GlobalPosition) <= DISTANCE_TO_TARGET * 1.5f) {
                var direction = GlobalPosition.DirectionTo(target.GlobalPosition);
                RotateToSlerp(new Vector2(direction.X, direction.Z), delta);

                if (shootTick <= 0) {
                    FireBall.Fire(Position, Transform.Rotated(Vector3.Up, (float)(Math.PI / 12f)));
                    FireBall.Fire(GlobalPosition, Transform.Rotated(Vector3.Up, -(float)(Math.PI / 12f)));
                    shootTick = SHOOT_COOLDOWN;
                }
            }
            else {
                RotateToSlerp(new Vector2(Velocity.X, Velocity.Z), delta);
            }

            if (shootTick > 0) {
                shootTick -= (float)delta;
            }
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (!spawned) {
            MoveToTarget();

            MoveAndSlide();
        }
        if (!IsOnFloor()) {
            Velocity += Vector3.Down;
        }
    }


    public bool Hit(int damage) {
        QueueFree();
        return true;
    }

    public void Deflect(float yRotation) {
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
}
