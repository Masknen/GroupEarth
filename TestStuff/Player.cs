using Godot;
using System;
using System.Diagnostics;
using System.Security;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class Player : Node3D
{
	[Export] private int ID = -1;
	[Export] private float speed;
	[Export] private Player otherPlayer;

	private testBall currentTouching;

	private float deadZone = 0.3f;

	public override void _Ready() {
		GetChild<Area3D>(0).AreaEntered += Player_AreaEntered;
		GetChild<Area3D>(0).AreaExited += Player_AreaExited;
	}

	private void Player_AreaExited(Area3D area) {
		currentTouching = null;
	}

	private void Player_AreaEntered(Area3D area) {
		currentTouching = area.GetParent() as testBall;
	}

	public override void _Process(double delta) {
		if (Input.IsJoyButtonPressed(ID, JoyButton.RightShoulder)) {
			Vector2 shootDireciton = Vector2.Zero;
			shootDireciton.X = Input.GetJoyAxis(ID, JoyAxis.RightX);
			shootDireciton.Y = Input.GetJoyAxis(ID, JoyAxis.RightY);
			if (currentTouching != null) {
				currentTouching.LookAt(new Vector3(shootDireciton.X, 0, shootDireciton.Y) * 100);
				currentTouching.speed += 10.1f;
			}
			
		}
		if (Input.IsJoyButtonPressed(ID, JoyButton.LeftShoulder)) {
			if (currentTouching != null) {
				currentTouching.LookAt(otherPlayer.GlobalPosition);

			currentTouching.speed += 0.1f;
			}

		}
	}

	public override void _PhysicsProcess(double delta) {
		//Vector2 inputDirection = Vector2.Zero;
		Vector2 inputDirection = Vector2.Zero;

		if (Input.GetJoyAxis(ID, JoyAxis.LeftX) > deadZone || Input.GetJoyAxis(ID, JoyAxis.LeftX) < -deadZone || Input.GetJoyAxis(ID, JoyAxis.LeftY) > deadZone || Input.GetJoyAxis(ID, JoyAxis.LeftY) < -deadZone)
		{
			//GD.Print("jadu");
			inputDirection.X = Input.GetJoyAxis(ID, JoyAxis.LeftX);
			inputDirection.Y = Input.GetJoyAxis(ID, JoyAxis.LeftY);
		}

		Position += new Vector3(inputDirection.X, 0, inputDirection.Y) * (float)(delta * speed);
		//GD.Print(inputDirection.X, " & ", inputDirection.Y);
		
		Vector2 inputRotation = new Vector2(inputDirection.X, inputDirection.Y);

		if(Input.GetJoyAxis(ID, JoyAxis.RightX) > deadZone || Input.GetJoyAxis(ID, JoyAxis.RightX) < -deadZone || Input.GetJoyAxis(ID, JoyAxis.RightY) > deadZone || Input.GetJoyAxis(ID, JoyAxis.RightY) < -deadZone)
		{
			inputRotation.X = Input.GetJoyAxis(ID, JoyAxis.RightX);
			inputRotation.Y = Input.GetJoyAxis(ID, JoyAxis.RightY);
		}

		this.LookAt(new Vector3(inputRotation.X, 0 , inputRotation.Y) * 100);
		//GD.Print(inputRotation.X, inputRotation.Y);
	}

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
