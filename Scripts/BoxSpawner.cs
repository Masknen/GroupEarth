using Godot;
using System;

public partial class BoxSpawner : Marker3D
{
	private InteractableBox interactableBox;
	private const float MAX_TIME_FROM_DESTROYED = 20;
	private float timeFromDestroyedTick = MAX_TIME_FROM_DESTROYED; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(interactableBox == null)
		{
			timeFromDestroyedTick += (float) delta;
		
		}
		if(timeFromDestroyedTick>= MAX_TIME_FROM_DESTROYED)
		{
				interactableBox = GameManager.Instance.interactableBox.Instantiate()as InteractableBox;
				AddChild(interactableBox); 
				interactableBox.GlobalPosition = GlobalPosition;
				timeFromDestroyedTick = 0;

		}
		if(interactableBox != null)
		{
			if(interactableBox.destroy)
			{
				interactableBox.QueueFree();
				interactableBox = null;

			}

		}
	}
}
