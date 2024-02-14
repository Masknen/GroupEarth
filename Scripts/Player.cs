using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Security;
//using System.Diagnostics;
//using System.Security;

//[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class Player : CharacterBody3D, IDamagable {
	public int ID = -1;
	public Player otherPlayer;
	private List<IDeflectable> currentTouching;
	private float deadZone = 0.3f;

    public Stat stats = new Stat();

	private const float PARRY_DURATION = 0.2f;
	private const float PARRY_COOLDOWN = 0.2f;
	private const float DODGE_COOLDOWN = 0.3f;
    private const float INVINCIBILTY_DURATION = 0.15f;

	private float parryDurationTick = 0;
	private float parryCooldownTick = 0;
	private float dodgeCooldownTick = 0;
    private float invincibiltyTick = 0;
	private bool[] isParrying = {false, false, false};
	private bool doDodge = false;


	private AnimationPlayer animationPlayer;
	private MeshInstance3D parryArea;

    private PackedScene fireBall;

    public override void _Ready() {
		currentTouching = new List<IDeflectable>();

		animationPlayer = GetChild<AnimationPlayer>(3);
		parryArea = GetChild<MeshInstance3D>(4);

		GetChild<Area3D>(1).AreaEntered += Player_AreaEntered;
		GetChild<Area3D>(1).AreaExited += Player_AreaExited;

		parryArea.Visible = false;

        fireBall = GD.Load<PackedScene>("res://Scenes/fire_ball.tscn");
        stats.AddStat(Stat.StatType.MaxHealth, 10).AddStat(Stat.StatType.MovementSpeed, 4).AddStat(Stat.StatType.DodgeStrength, 35).AddStat(Stat.StatType.RotationSpeed, 15);
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


            if (!isParrying[0] && !isParrying[1] && !isParrying[2]) {
                Velocity += inputdirectionV3 * (float)(stats.GetStat(Stat.StatType.MovementSpeed));
            } else {
                Velocity = Vector3.Zero;
            }

			if (doDodge) {
				GD.Print("Dodge!");
				dodgeCooldownTick = 0;
				if (!inputdirectionV3.IsEqualApprox(Vector3.Zero)) {
					Velocity += inputdirectionV3 * stats.GetStat(Stat.StatType.DodgeStrength);
					RotateTo(inputDirection);
				} else {
					Velocity += -Transform.Basis.Z * stats.GetStat(Stat.StatType.DodgeStrength);
				}
				doDodge = false;
			}


			// Gets the input vector for right stick, if zero use the inputDirection instead
			Vector2 inputRotation = GetInputVector(JoyAxis.RightX, JoyAxis.RightY, deadZone);// != Vector2.Zero ? GetInputVector(JoyAxis.RightX, JoyAxis.RightY, deadZone) : inputDirection;


			// Rotates player
			if (inputRotation != Vector2.Zero && Velocity.Length() < stats.GetStat(Stat.StatType.MovementSpeed) * 3) {
				RotateToSlerp(inputRotation, delta);
			}
			MoveAndSlide();
		}
	}

    private void HandleInput() {
        NormalDeflect();
        

        if ((InputManager.Instance().IsJustPressedButton(ID, JoyButton.LeftShoulder) || isParrying[1]) && parryCooldownTick >= PARRY_COOLDOWN) {
            GD.Print(ID + " | Friend Parry");
            isParrying[1] = true;
            invincibiltyTick = INVINCIBILTY_DURATION;
            parryCooldownTick = 0;
            foreach (var defleactable in currentTouching) {
                defleactable.FriendDeflect(-GlobalPosition.DirectionTo(otherPlayer.GlobalPosition).SignedAngleTo(Vector3.Forward, Transform.Basis.Y));
            }
        }

        //Charge parry
        if (Input.GetJoyAxis(ID, JoyAxis.TriggerRight) >= 0.15) {
            parryArea.Visible = true;
            float chargeSpeed = 0.05f;
            if (InputManager.Instance().IsJustPressedAxis(ID, JoyAxis.TriggerRight)) {
                parryArea.Scale = Vector3.Zero;
            }
            if (parryArea.Scale < Vector3.One) {
                parryArea.Scale += new Vector3(chargeSpeed, chargeSpeed, chargeSpeed);
            } else {
                parryArea.Scale = Vector3.One;
            }
        }

        if (InputManager.Instance().IsJustReleasedAxis(ID, JoyAxis.TriggerRight)) {
            if (parryArea.Scale > new Vector3(0.95f, 0.95f, 0.95f)) {
                invincibiltyTick = INVINCIBILTY_DURATION / 2.0f;
                var new_fireBall = fireBall.Instantiate();
                (new_fireBall as FireBall).Position = Position;
                float yRotation = Transform.Basis.GetEuler().Y;
                Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
                (new_fireBall as FireBall).Transform = transform;
                GetParent().AddChild(new_fireBall);
            }
            parryArea.Scale = Vector3.One;
            parryArea.Visible = false;
        }
        //



        //if ((InputManager.Instance().IsJustPressedAxis(ID, JoyAxis.TriggerRight) || isParrying[2]) && parryCooldownTick >= PARRY_COOLDOWN) {
        //    GD.Print(ID + " | Arc Parry");
        //    invincibiltyTick = INVINCIBILTY_DURATION/2.0f;
        //    var new_fireBall = fireBall.Instantiate();
        //    GD.Print("fireball!");
        //    (new_fireBall as FireBall).Position = Position;
        //    float yRotation = Transform.Basis.GetEuler().Y;
        //    Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        //    (new_fireBall as FireBall).Transform = transform;
        //    GetParent().AddChild(new_fireBall);

        //    //isParrying[2] = true;
        //    //parryCooldownTick = 0;
        //    //foreach (var defleactable in currentTouching) {
        //    //    defleactable.ArcDeflect(Transform.Basis.GetEuler().Y);
        //    //}
        //}






        if (InputManager.Instance().IsJustPressedAxis(ID, JoyAxis.TriggerLeft) && dodgeCooldownTick >= DODGE_COOLDOWN) {
            doDodge = true;
            invincibiltyTick = INVINCIBILTY_DURATION;
        }
    }

    private void NormalDeflect() {
        //Hold working(with centred circular deflect area)
        if (Input.IsJoyButtonPressed(ID, JoyButton.RightShoulder) && parryCooldownTick >= PARRY_COOLDOWN && parryDurationTick < PARRY_DURATION) {
            parryArea.Visible = true;
            isParrying[0] = true;
            parryDurationTick = 0;
            foreach (var defleactable in currentTouching) {
                defleactable.Hold();
            }
        }

        if (InputManager.Instance().IsJustReleasedButton(ID, JoyButton.RightShoulder)) {
            GD.Print(ID + " | Parry");
            parryCooldownTick = 0;
            invincibiltyTick = INVINCIBILTY_DURATION;
            parryArea.Visible = false;
            isParrying[0] = false;
            foreach (var defleactable in currentTouching) {
                defleactable.Deflect(Transform.Basis.GetEuler().Y);
            }
        }

        // Normal Working
        //if ((InputManager.Instance().IsJustReleasedButton(ID, JoyButton.RightShoulder) || isParrying[0]) && parryCooldownTick >= PARRY_COOLDOWN) {
        //    GD.Print(ID + " | Parry");
        //    isParrying[0] = true;
        //    parryCooldownTick = 0;
        //    foreach (var defleactable in currentTouching) {
        //        defleactable.Deflect(Transform.Basis.GetEuler().Y);
        //    }
        //}
    }

    private void UpdateCooldownTicks(double delta) {
		dodgeCooldownTick += (float)delta;
        invincibiltyTick -= (float)delta;

        if (/*isParrying[0] ||*/ isParrying[1] || isParrying[2]) {
            parryDurationTick += (float)delta;
            parryArea.Visible = true;
            if (parryDurationTick >= PARRY_DURATION) {
                //isParrying[0] = false;
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


        quaternion = quaternion.Slerp(quaternionTargetDirection, (float)(stats.GetStat(Stat.StatType.RotationSpeed) * delta));

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

    bool IDamagable.Hit(int damage) {
        if (invincibiltyTick < 0) {
            GD.Print("HIT FOR: " + damage);
            if (PlayerManager.Instance().debugBoolean) {
                Position = Vector3.Up;
            }
            return true;
        }
        return false;
    }
    //   private string GetDebuggerDisplay()
    //{
    //	return ToString();
    //}
}
