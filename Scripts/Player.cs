using Godot;
using System;
using System.Diagnostics;
using System.Security;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class Player : CharacterBody3D
{
	[Export] private int ID = -1;
	[Export] private float speed;
	[Export] private Player otherPlayer;
	private float rotationSpeed = 10.0f;

	private testBall currentTouching;

	private float deadZone = 0.3f;

	public override void _Ready() {
		GetChild<Area3D>(1).AreaEntered += Player_AreaEntered;
		GetChild<Area3D>(1).AreaExited += Player_AreaExited;

	}

	private void Player_AreaExited(Area3D area) {
		currentTouching = null;
	}

	private void Player_AreaEntered(Area3D area) {
		currentTouching = area.GetParent() as testBall;
	}

	public override void _Process(double delta) {
		if (Input.IsJoyButtonPressed(ID, JoyButton.RightShoulder)) {
			if (currentTouching != null) {
				currentTouching.Rotation = Rotation; 
				currentTouching.speed += 1.0f;
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
		// Gets the input vector for left stick and applies it to position
		Vector2 inputDirection = GetInputVector(JoyAxis.LeftX, JoyAxis.LeftY, deadZone);
		Position += new Vector3(inputDirection.X, 0, inputDirection.Y) * (float)(delta * speed);

		// Gets the input vector for right stick, if zero use the inputDirection instead
		Vector2 inputRotation = GetInputVector(JoyAxis.RightX, JoyAxis.RightY, deadZone) != Vector2.Zero ? GetInputVector(JoyAxis.RightX, JoyAxis.RightY, deadZone) : inputDirection;

		// Rotates player
		if (inputRotation != Vector2.Zero) {
			// Calculates angle to create quaternion
			float angle = Vector2.Up.AngleTo(inputRotation);
			Godot.Quaternion quaternionTargetDirection = new Quaternion(-Transform.Basis.Y, angle);
			var quaternion = Transform.Basis.GetRotationQuaternion();

			
			quaternion = quaternion.Slerp(quaternionTargetDirection, (float)(rotationSpeed*delta));

			// Sets the rotation to the transform
			Transform3D transform = Transform;
			transform.Basis = new Basis(quaternion);
			Transform = transform;
		}
		MoveAndSlide();
	}

	private Vector2 GetInputVector(JoyAxis joyAxisX, JoyAxis joyAxisY, float deadZone) {
		if (Input.GetJoyAxis(ID, joyAxisX) > deadZone || Input.GetJoyAxis(ID, joyAxisX) < -deadZone || Input.GetJoyAxis(ID, joyAxisY) > deadZone || Input.GetJoyAxis(ID, joyAxisY) < -deadZone) {
			return new Vector2(Input.GetJoyAxis(ID, joyAxisX), Input.GetJoyAxis(ID, joyAxisY)).Normalized();
		}
		return Vector2.Zero;
	}

	private string GetDebuggerDisplay()
	{
		return ToString();
	}
}
