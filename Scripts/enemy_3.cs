using Godot;
using System;

public partial class enemy_3 : CharacterBody3D, IDamagable
{
    [Export] public float speed {get; set;} = 0.1f;

	public float hp = 1;
    private float MaxTime = 6;
	private float timeTick = 0;

    private PackedScene skeleton;
	private Node3D target;
	private CharacterBody3D player1;
	private CharacterBody3D player2;

	private Vector3 direction;

    public override void _Ready()
    {
        skeleton = GD.Load<PackedScene>("res://Scenes/enemy_2.tscn");
        ArcadeSpawner.Instance.enemyArray.Add(this);
        //GD.Print(ArcadeSpawner.Instance.enemyArray.Count);
    }



    public override void _Process(double delta){

		if (PlayerManager.Instance().players.Count > 0) {
			player1 = PlayerManager.Instance().players[0];
			player2 = PlayerManager.Instance().players[1];
			if(player1.GlobalPosition.DistanceTo(GlobalPosition)<
			player2.GlobalPosition.DistanceTo(GlobalPosition)){
				target = player1;
			}else { target = player2;}
		
		Velocity = Position.DirectionTo(target.GlobalPosition) * speed;
		Vector3 targetAngle = new Vector3(target.Position.X, this.Position.Y,target.Position.Z);
		LookAt(targetAngle);
		MoveAndSlide();
        timeTick += (float)delta;
			if (timeTick > MaxTime) {
				timeTick -= MaxTime;
				//summonSkeleton();
			}
        

	}
    }

	/*
    private void summonSkeleton(){
        var new_skeleton = skeleton.Instantiate();
		GD.Print("summoning skeleton!");
		(new_skeleton as enemy2).Position = Position;
        GetParent().AddChild(new_skeleton);
    }
	*/
    bool IDamagable.Hit(int damage){
		//GD.Print(damage);
		hp += -damage;
		if(hp <= 0){
            ArcadeSpawner.Instance.enemyArray.Remove(this);
            //GD.Print(ArcadeSpawner.Instance.enemyArray.Count);
            QueueFree();
			return true;
		}
		if(damage <= 0){
			return false;
		}
		else{
			return true;
		} 
	}
}
