using Godot;
using System;
using System.Collections.Generic;
public partial class Player : CharacterBody3D, IDamagable {

	public int ID = -1;
	public Player otherPlayer;

	private List<IDeflectable> currentTouching;
	private float deadZone = 0.3f;

    public Stat stats = new Stat();

	private const float PARRY_DURATION        = 0.2f;
	private const float PARRY_COOLDOWN        = 0.2f;
	private const float DODGE_COOLDOWN        = 0.3f;
    private const float INVINCIBILTY_DURATION = 0.3f;

	private float parryDurationTick = 0;
	private float parryCooldownTick = 0;
	private float dodgeCooldownTick = 0;
    private float invincibiltyTick  = 0;
	private bool[] isParrying = {false, false, false};
    private bool doDeflect = false;
	private bool doDodge = false;


	private AnimationPlayer animationPlayer;
	private MeshInstance3D parryArea;
    private float VisualCatchSize = 1.35f;
    private StandardMaterial3D parryOverrideMaterial;
    private MeshInstance3D playerMarker;

    private PackedScene fireBall;

    public override void _Ready() {
		currentTouching = new List<IDeflectable>();

		animationPlayer = GetChild<AnimationPlayer>(3);
		parryArea = GetChild<MeshInstance3D>(4);

        GetChild<Area3D>(1).AreaEntered += ParryAreaEntered;
		GetChild<Area3D>(1).AreaExited += ParryAreaExited;

        parryOverrideMaterial = new StandardMaterial3D();
        parryOverrideMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        parryOverrideMaterial.AlbedoColor = new Color(0, 0, 1, 0.1f);
        parryArea.MaterialOverride = parryOverrideMaterial;
        parryArea.Scale = Vector3.One * VisualCatchSize;
        //parryArea.Visible = false;

        fireBall = GD.Load<PackedScene>("res://Scenes/fire_ball.tscn");
        
        stats.AddStat(Stat.StatType.MaxHealth, 10).AddStat(Stat.StatType.MovementSpeed, 4).AddStat(Stat.StatType.DodgeStrength, 35).AddStat(Stat.StatType.RotationSpeed, 15);
        stats.AddStat(Stat.StatType.CurrentHealth, stats.GetStat(Stat.StatType.MaxHealth));
	}

	public override void _Process(double delta) {
		if (ID != -1) {
			HandleInput();
			//UpdateCooldownTicks(delta);


            if (Input.IsJoyButtonPressed(ID, JoyButton.LeftShoulder)) {
                for (int i = 0; i < 10; i++) {
                    FireBall.Fire(Position, Transform.Rotated(Vector3.Up, (float)(Math.PI/5 * i)));
                }
            }
        }
	}


    public override void _PhysicsProcess(double delta) {
		if (ID != -1) {
            UpdateDeflect(delta);

            if (!IsOnFloor()) {
                Velocity -= new Vector3(0, 1, 0);
            }

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
		}
	    MoveAndSlide();
	}

    public void setID(int id) {
        this.ID = id;
        playerMarker = GetChild<MeshInstance3D>(5);
        StandardMaterial3D material3D = new StandardMaterial3D();
        switch (ID) {
            case 0:
                material3D.AlbedoColor = new Color(1.0f, 221.0f/255.0f, 0.0f, 1.0f);
                playerMarker.MaterialOverride = material3D;
                break; 
            case 1:
                material3D.AlbedoColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                playerMarker.MaterialOverride = material3D;
                break;
        }
    }
    bool IDamagable.Hit(int damage) {
        if (invincibiltyTick < 0) {
            stats.ModifyStat(Stat.StatType.CurrentHealth, -damage);
            if (stats.GetStat(Stat.StatType.CurrentHealth) <= 0) {
                Position = Vector3.Up;
                stats.setStat(Stat.StatType.CurrentHealth, stats.GetStat(Stat.StatType.MaxHealth));
            }
            return true;
        }
        return false;
    }

	private void ParryAreaExited(Area3D area) {
		currentTouching.Remove(area as IDeflectable);
	}
	private void ParryAreaEntered(Area3D area) {
		if (area as IDeflectable != null) {
            if (!isParrying[0]) {
                currentTouching.Add(area as IDeflectable);
            } else {
                area.QueueFree();
            }
		}
	}

    private void HandleInput() {
        if (Godot.Input.IsJoyButtonPressed(ID, JoyButton.RightShoulder)) {
            isParrying[0] = true;

            parryOverrideMaterial.AlbedoColor = new Color(0, 0, 1, 1);
            parryArea.MaterialOverride = parryOverrideMaterial;
        }
        if (InputManager.Instance().IsJustPressedButton(ID, JoyButton.RightShoulder)) {
            parryArea.Scale = Vector3.One * VisualCatchSize;
        }
        if (InputManager.Instance().IsJustReleasedButton(ID, JoyButton.RightShoulder)) {
            isParrying[0] = false;
            doDeflect = true;
            parryArea.Scale = Vector3.One * VisualCatchSize;

            parryOverrideMaterial.AlbedoColor = new Color(0, 0, 1, 0.1f);
            parryArea.MaterialOverride = parryOverrideMaterial;
        }
        if (InputManager.Instance().IsJustPressedAxis(ID, JoyAxis.TriggerLeft) && dodgeCooldownTick >= DODGE_COOLDOWN) {
            doDodge = true;
            invincibiltyTick = INVINCIBILTY_DURATION;
        }
    }


    private void UpdateDeflect(double delta) {
        if (isParrying[0]) {
            foreach (var _item in currentTouching) {
                var item = (_item as Node3D);
                if (item == null) continue;

                _item.Hold();
                if (item.GlobalPosition.DistanceTo(this.GlobalPosition) > 2.5f) {
                    Vector3 inwards = item.GlobalPosition.DirectionTo(this.GlobalPosition);
                    item.GlobalPosition += inwards * (item.GlobalPosition.DistanceTo(this.GlobalPosition) * (float)(delta*3));
                }
            }

            if (!parryArea.Scale.IsEqualApprox(Vector3.One)) {
                parryArea.Scale = parryArea.Scale.Lerp(Vector3.One, (float)(delta * 15));
            }
        }
        if (doDeflect) {
            doDeflect = false;

            var raycast = GetChild<RayCast3D>(6);
            Node3D target = null;

            float scanAngle = (float)(Math.PI/4);
            raycast.Transform = new Transform3D(new Basis(Vector3.Up, -scanAngle), Vector3.Zero);
            for (float i = -scanAngle; i <= scanAngle; i += (float)scanAngle/40) {
                raycast.ForceRaycastUpdate();
                var _object = raycast.GetCollider();
                target = target == null ? _object as Node3D : target;

                if ((_object as Node3D) != null) {
                    var possibleTarget = _object as Node3D;
                    if (Position.DirectionTo(target.Position).AngleTo(-Transform.Basis.Z) > Math.Abs(i)) {
                        target = _object as Node3D;
                    }
                }
                raycast.Transform = new Transform3D(new Basis(Vector3.Up, i), Vector3.Zero);
            }

            foreach (var _item in currentTouching) {
                var item = (_item as Node3D);
                if (item == null) continue;

                if (target != null) {
                    var direction = item.GlobalPosition.DirectionTo(target.GlobalPosition);
                    GD.Print(direction);
                    float yRotation = -direction.SignedAngleTo(Vector3.Forward, Vector3.Up);
                    _item.Deflect(yRotation);
                } else {
                    _item.Deflect(Transform.Basis.GetEuler().Y);
                }
            }
        }
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
        if (Godot.Input.GetJoyAxis(ID, joyAxisX) > deadZone || Godot.Input.GetJoyAxis(ID, joyAxisX) < -deadZone || Godot.Input.GetJoyAxis(ID, joyAxisY) > deadZone || Godot.Input.GetJoyAxis(ID, joyAxisY) < -deadZone) {
			return new Vector2(Godot.Input.GetJoyAxis(ID, joyAxisX), Godot.Input.GetJoyAxis(ID, joyAxisY)).Normalized();
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
}
