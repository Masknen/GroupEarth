using Godot;
using System;


public partial class enemy2 : CharacterBody3D, IDamagable, IDeflectable {


	private Stat stat;

	private Player target;
	private AnimationPlayer animationPlayer;
    private NavigationAgent3D navAgent;

    private bool spawned = true;
    private float LastPositionUpdate = 0;

    public override void _Ready() {
        animationPlayer = GetChild<AnimationPlayer>(2);
        navAgent = GetChild<NavigationAgent3D>(3);
        animationPlayer.AnimationFinished += AnimationFinished;

        navAgent.DebugEnabled = true;

        animationPlayer.Play("Spawn_Ground_Skeletons");
    }

    private void AnimationFinished(StringName animName) {
        if (animName == "Spawn_Ground_Skeletons") spawned = false;
    }

    public override void _Process(double delta) {

	}

    public override void _PhysicsProcess(double delta) {
        if (!spawned) {
            foreach (var player in PlayerManager.Instance().players) {
                if (target == null) {
                    target = player;
                }

                if (GlobalPosition.DistanceSquaredTo(target.GlobalPosition) > GlobalPosition.DistanceSquaredTo(player.GlobalPosition)) {
                    target = player;
                }
            }
            LastPositionUpdate += (float)delta;
            if (LastPositionUpdate > 3) {
                navAgent.TargetPosition = target.GlobalPosition + new Vector3((float)Math.Cos(GameManager.Instance.TimeSinceStart), 0, (float)Math.Sin(GameManager.Instance.TimeSinceStart)) * 3;
                LastPositionUpdate = 0;
            }


            Vector3 direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition());

            Velocity = direction * 5;

            MoveAndSlide();
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
}
