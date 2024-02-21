using Godot;
using System;


public partial class enemy2 : CharacterBody3D, IDamagable, IDeflectable {


	private Stat stat;

	private Node3D target;
	private AnimationPlayer animationPlayer;

    private bool spawned = true;

    public override void _Ready() {
        animationPlayer = GetChild<AnimationPlayer>(2);
        animationPlayer.AnimationFinished += AnimationFinished;


        animationPlayer.Play("Spawn_Ground_Skeletons");
    }

    private void AnimationFinished(StringName animName) {
        if (animName == "Spawn_Ground_Skeletons") spawned = false;
    }

    public override void _Process(double delta) {

	}

    public override void _PhysicsProcess(double delta) {
        
    }

    public bool Hit(int damage) {
        throw new NotImplementedException();
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
