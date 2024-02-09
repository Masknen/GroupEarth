using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Security;
//using System.Diagnostics;
//using System.Security;

//[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class Player : CharacterBody3D
{
	public int ID = -1;
	public Player otherPlayer;
	private List<IDeflectable> currentTouching;
	private float deadZone = 0.3f;

	[Export] private float movementSpeed;
	private float dodgeStrength = 35.0f;
	private float rotationSpeed = 15.0f;

	private const float PARRY_DURATION = 0.2f;
	private const float PARRY_COOLDOWN = 0.5f;
	private const float DODGE_COOLDOWN = 0.0f;

	private float parryDurationTick = 0;
	private float parryCooldownTick = 0;
	private float dodgeCooldownTick = 0;
	private bool[] isParrying = {false, false, false};
	private bool doDodge = false;


	private AnimationPlayer animationPlayer;
	private MeshInstance3D parryArea;

	public override void _Ready() {
		currentTouching = new List<IDeflectable>();

		animationPlayer = GetChild<AnimationPlayer>(3);
		parryArea = GetChild<MeshInstance3D>(4);

		GetChild<Area3D>(1).AreaEntered += Player_AreaEntered;
		GetChild<Area3D>(1).AreaExited += Player_AreaExited;

		parryArea.Visible = false;
	}

	private void Player_AreaExited(Area3D area) {
		currentTouching.Remove(area as IDeflectable);
	}

	private void Player_AreaEntered(Area3D area) {
		if (area as IDeflectable != null) {
			currentTouching.Add(area as IDeflectable);
		}
	}

	public override void _Process(double delta) {
		if (ID != -1) {
			HandleInput();

			UpdateCooldownTicks(delta);
        }
	}

    public override void _PhysicsProcess(double delta) {
		if (ID != -1) {
			if (Velocity.Length() > 20) {
				Velocity = Velocity * 0.75f;
			} else {
				Velocity = Velocity * 0.65f;
			}

			// Gets the input vector for left stick and applies it to position
			Vector2 inputDirection = GetInputVector(JoyAxis.LeftX, JoyAxis.LeftY, deadZone);
			Vector3 inputdirectionV3 = new Vector3(inputDirection.X, 0, inputDirection.Y);

			Velocity += inputdirectionV3 * (float)(movementSpeed);

			if (doDodge) {
				GD.Print("Dodge!");
				dodgeCooldownTick = 0;
				if (!inputdirectionV3.IsEqualApprox(Vector3.Zero)) {
					Velocity += inputdirectionV3 * dodgeStrength;
					RotateTo(inputDirection);
				} else {
					Velocity += -Transform.Basis.Z * dodgeStrength;
				}
				doDodge = false;
			}


			// Gets the input vector for right stick, if zero use the inputDirection instead
			Vector2 inputRotation = GetInputVector(JoyAxis.RightX, JoyAxis.RightY, deadZone);// != Vector2.Zero ? GetInputVector(JoyAxis.RightX, JoyAxis.RightY, deadZone) : inputDirection;


			// Rotates player
			if (inputRotation != Vector2.Zero && Velocity.Length() < movementSpeed * 3) {
				RotateToSlerp(inputRotation, delta);
			}
			MoveAndSlide();
		}
	}

    private void HandleInput() {
        if ((InputManager.Instance().IsJustPressedButton(ID, JoyButton.RightShoulder) || isParrying[0]) && parryCooldownTick >= PARRY_COOLDOWN) {
            GD.Print(ID + " | Parry");
            isParrying[0] = true;
            parryCooldownTick = 0;
            foreach (var defleactable in currentTouching) {
                defleactable.Deflect(Transform.Basis.GetEuler().Y);
            }
        }

        if ((InputManager.Instance().IsJustPressedButton(ID, JoyButton.LeftShoulder) || isParrying[1]) && parryCooldownTick >= PARRY_COOLDOWN) {
            GD.Print(ID + " | Friend Parry");
            isParrying[1] = true;
            parryCooldownTick = 0;
            foreach (var defleactable in currentTouching) {
                defleactable.FriendDeflect(-GlobalPosition.DirectionTo(otherPlayer.GlobalPosition).SignedAngleTo(Vector3.Forward, Transform.Basis.Y));
            }
        }

        if ((InputManager.Instance().IsJustPressedAxis(ID, JoyAxis.TriggerRight) || isParrying[2]) && parryCooldownTick >= PARRY_COOLDOWN) {
            GD.Print(ID + " | Arc Parry");
            isParrying[2] = true;
            parryCooldownTick = 0;
            foreach (var defleactable in currentTouching) {
                defleactable.ArcDeflect(Transform.Basis.GetEuler().Y);
            }
        }

        if (InputManager.Instance().IsJustPressedAxis(ID, JoyAxis.TriggerLeft) && dodgeCooldownTick >= DODGE_COOLDOWN) {
            doDodge = true;
        }
    }
    private void UpdateCooldownTicks(double delta) {
		dodgeCooldownTick += (float)delta;

        if (isParrying[0] || isParrying[1] || isParrying[2]) {
            parryDurationTick += (float)delta;
            parryArea.Visible = true;
            if (parryDurationTick >= PARRY_DURATION) {
                isParrying[0] = false;
                isParrying[1] = false;
                isParrying[2] = false;
                parryDurationTick = 0;
                parryArea.Visible = false;
            }
        } else {
            parryCooldownTick += (float)delta;
        }
    }

	private Vector2 GetInputVector(JoyAxis joyAxisX, JoyAxis joyAxisY, float deadZone) {
        if (Input.GetJoyAxis(ID, joyAxisX) > deadZone || Input.GetJoyAxis(ID, joyAxisX) < -deadZone || Input.GetJoyAxis(ID, joyAxisY) > deadZone || Input.GetJoyAxis(ID, joyAxisY) < -deadZone) {
			return new Vector2(Input.GetJoyAxis(ID, joyAxisX), Input.GetJoyAxis(ID, joyAxisY)).Normalized();
		}
		return Vector2.Zero;
	}
	private void RotateToSlerp(Vector2 inputRotation, double delta) {
        // Calculates angle to create quaternion
        float angle = Vector2.Up.AngleTo(inputRotation);
        Godot.Quaternion quaternionTargetDirection = new Quaternion(-Transform.Basis.Y, angle);
        var quaternion = Transform.Basis.GetRotationQuaternion();


        quaternion = quaternion.Slerp(quaternionTargetDirection, (float)(rotationSpeed * delta));

        // Sets the rotation to the transform
        Transform3D transform = Transform;
        transform.Basis = new Basis(quaternion);
        Transform = transform;
    }
    private void RotateTo(Vector2 inputRotation) {
        // Calculates angle to create quaternion
        float angle = Vector2.Up.AngleTo(inputRotation);
        Godot.Quaternion quaternionTargetDirection = new Quaternion(-Transform.Basis.Y, angle);
        var quaternion = Transform.Basis.GetRotationQuaternion();

        // Sets the rotation to the transform
        Transform3D transform = Transform;
        transform.Basis = new Basis(quaternionTargetDirection);
        Transform = transform;
    }
 //   private string GetDebuggerDisplay()
	//{
	//	return ToString();
	//}
}
