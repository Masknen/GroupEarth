using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
public partial class Player : CharacterBody3D, IDamagable, IDeflectable {
    private enum State {
        Idle,
        Walking,
        IsDeflecting,
        Deflect,
        Dodge,
        Hit,
        Dead,
    }
    private State state = State.Idle;

	public int ID = -1;

	private List<IDeflectable> currentTouching;

    public Stat stats = new Stat();

	private const float DEAD_ZONE               = 0.3f;
	private const float DODGE_COOLDOWN          = 0.3f;
    private const float DEFLECT_COOLDOWN        = 0.8f;
    private const float INVINCIBILITY_DURATION  = 0.3f;
    private const float VISUAL_CATCH_SCALE_MULT = 1.35f;
    private const float VISUAL_CATCH_ALPHA_MIN  = 0.05f;
    public  const float VISUAL_CATCH_ALPHA_MAX  = 0.6f;
    private Vector3 VISUAL_CATCH_SCALE          = Vector3.One * VISUAL_CATCH_SCALE_MULT;
    private const float AIM_HELP_ANGLE          = (float)(Math.PI / 6);
    private const float TIME_TO_RESSURECT       = 30.0f;
    private const float CAST_COST = VISUAL_CATCH_ALPHA_MAX * 0.75f;

    private const float DEFLECT_COST = 0.15f;
    private const float DEFLECT_REGENERATION = 0.15f;
    private const float DEFLECT_HIT_DECREASE = 0.15f;

	private float dodgeCooldownTick = 0;
    private float deflectCooldownTick = 0;
    private float invincibilityTick = 0;
    //--- added by kalle
    private float timeRessTick      = 0;
    //--- added by kalle
    public float currentVisualCatchAlpha = VISUAL_CATCH_ALPHA_MAX;

    private bool spawned      = true;
    private bool isDeflecting = false;
    private bool doDeflect    = false;
	private bool doDodge      = false;
    private bool isHit        = false;
    private bool onScreen = true;
    //---added by kalle 
    private bool isDead       = false;
    //--added by kalle

    private AnimationPlayer animationPlayer;
    private MeshInstance3D playerMarker;
	private MeshInstance3D deflectArea;
    private MeshInstance3D deflectSphere;
    private StandardMaterial3D parryOverrideMaterial;
    private RayCast3D raycast;
    //---added by kalle
    private Node3D deathEffect;
    private Node3D mageCharacter;
    private PackedScene deathExplosion;
    private PackedScene ressEffect;
    //---added by kalle
    public override void _Ready() {
		currentTouching = new List<IDeflectable>();

		deflectArea = GetChild<MeshInstance3D>(3);
        deflectSphere = GetChild<MeshInstance3D>(3).GetChild<MeshInstance3D>(0);
        raycast = GetChild<RayCast3D>(5);
        animationPlayer = GetChild(2).GetChild<AnimationPlayer>(1);
        //---added by kalle
        deathEffect = GetChild<Node3D>(7);
        mageCharacter = GetChild<Node3D>(2);

        deathExplosion = GD.Load<PackedScene>("res://AnimationScenes/PlayerDeathExplosion.tscn");
        ressEffect = GD.Load<PackedScene>("res://AnimationScenes/PlayerRessEffect.tscn");

        deathEffect.Visible = false;
        //---added by kalle

        GetChild<Area3D>(1).AreaEntered += ParryAreaEntered;
        GetChild<Area3D>(1).BodyEntered += ParryAreaEnteredBody;
		GetChild<Area3D>(1).AreaExited += ParryAreaExited;
        GetChild<Area3D>(1).BodyExited += ParryAreaExitedBody;
        GetChild<VisibleOnScreenNotifier3D>(8).ScreenEntered += ScreenEntered;
        GetChild<VisibleOnScreenNotifier3D>(8).ScreenExited += ScreenExited; ;
        animationPlayer.AnimationFinished += AnimationFinished;

        parryOverrideMaterial = new StandardMaterial3D();
        parryOverrideMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        parryOverrideMaterial.AlbedoColor = new Color(0, 0, 1, currentVisualCatchAlpha);
        deflectArea.MaterialOverride = parryOverrideMaterial;
        deflectArea.Scale = VISUAL_CATCH_SCALE;
        
        stats.AddStat(Stat.StatType.MaxHealth, 10).AddStat(Stat.StatType.MovementSpeed, 4).AddStat(Stat.StatType.DodgeStrength, 35).AddStat(Stat.StatType.RotationSpeed, 15);
        stats.AddStat(Stat.StatType.CurrentHealth, stats.GetStat(Stat.StatType.MaxHealth));

        animationPlayer.Play("Spawn_Air");
	}



    public override void _Process(double delta) {
		if (ID != -1 && !spawned) {
            UpdateCooldownTicks(delta); //Important be infront
			HandleInput();
            StateMachine();
            if(isDead){
                UpdateRessurect(delta);
            }

            //Test Code/Debug
            if (InputManager.Instance().IsJustPressedAxis(ID, JoyAxis.TriggerRight) && PlayerManager.Instance().debugBoolean) {
                for (int i = 0; i < 10; i++) {
                    FireBall.Fire(Position, Transform.Rotated(Vector3.Up, (float)(Math.PI/5 * i)));
                }
            }
        }
	}
    public override void _PhysicsProcess(double delta) {
        if (!onScreen) {
            Velocity += GlobalPosition.DirectionTo(MiddleNode.Instance.GlobalPosition) * GlobalPosition.DistanceSquaredTo(MiddleNode.Instance.GlobalPosition)/100;
        }

		if (ID != -1 && !spawned) {
            UpdateDeflect(delta);

            if (!IsOnFloor()) {
                Velocity += Vector3.Down;
            }

			if (Velocity.Length() > 20) {
				Velocity = Velocity * 0.75f;
			} else {
				Velocity = Velocity * 0.65f;
			}

			// Gets the input vector for left stick and applies it to position
			Vector2 inputDirection = InputManager.GetInputVector(ID, JoyAxis.LeftX, JoyAxis.LeftY, DEAD_ZONE);
			Vector3 inputdirectionV3 = new Vector3(inputDirection.X, 0, inputDirection.Y);

            if (!isDeflecting && !isHit) {
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
			Vector2 inputRotation = InputManager.GetInputVector(ID, JoyAxis.RightX, JoyAxis.RightY, DEAD_ZONE);

			// Rotates player
			if (inputDirection != Vector2.Zero && !isHit) {
				RotateToSlerp(inputDirection, delta);
			}
		}
	    MoveAndSlide();
	}

    public void setID(int id) {
        this.ID = id;
        playerMarker = GetChild<MeshInstance3D>(4);
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
    public bool Hit(int damage) {
        if (invincibilityTick < 0 && !isDead) {
            stats.ModifyStat(Stat.StatType.CurrentHealth, -damage);
            state = State.Hit;
            invincibilityTick = INVINCIBILITY_DURATION;
            isHit = true;
            if (stats.GetStat(Stat.StatType.CurrentHealth) <= 0) {
                //--added by kalle
                playerDeathExplosion();
                mageCharacter.Visible = false;
                deathEffect.Visible = true;
                isDead = true;
                //state = State.Dead;
                //--added by kalle
                //Position = Vector3.Up;
                //stats.setStat(Stat.StatType.CurrentHealth, stats.GetStat(Stat.StatType.MaxHealth));
            }
           
            
            return true;
        }
        return false;
    }

    private void StateMachine() {
        switch (state) {
            case State.Idle:
                if (IsOkToPlayAnimation())
                    animationPlayer.Play("Idle", 0.75f);
                break;
            case State.Walking:
                if (IsOkToPlayAnimation())
                    animationPlayer.Play("Running_C", 0.75f);
                break;
            case State.IsDeflecting:
                if (IsOkToPlayAnimation())
                    animationPlayer.Play("Spellcasting", 0.75f);
                break;
            case State.Deflect:
                animationPlayer.Play("Spellcast_Shoot", -1, 1.5f);
                break;
            case State.Dodge:
                if (IsOkToPlayAnimation())
                    animationPlayer.Play("Dodge_Forward", -0.5f, 1.5f);
                break;
            case State.Hit:
                animationPlayer.Play("Hit_B", -1, 1.7f);
                break;
            //--added by kalle
            case State.Dead:
                
                break;
            //--added by kalle
        }
        if (InputManager.GetInputVector(ID, JoyAxis.LeftX, JoyAxis.LeftY, DEAD_ZONE).IsZeroApprox()) {
            state = State.Idle;
        } else {
            state = State.Walking;
        }
        if (isDeflecting) {
            state = State.IsDeflecting;
        }
        if (doDeflect) {
            state = State.Deflect;
        }
        if (doDodge) {
            state = State.Dodge;
        }
    }
    //--added by kalle
    private void playerDeathExplosion() {
        var new_deathExploation = deathExplosion.Instantiate();
        (new_deathExploation as PlayerDeathExplosion).Position = Position;
        GetParent().AddChild(new_deathExploation);
    }
    private void playerRessEffect() {
        var new_ressEffect = ressEffect.Instantiate();
        (new_ressEffect as PlayerRessEffect).Position = Position;
        GetParent().AddChild(new_ressEffect);
    }
    private void UpdateRessurect(double delta){ 
        timeRessTick += (float)delta;
        if (timeRessTick > TIME_TO_RESSURECT - 0.4f) {
            playerRessEffect();
        }
        if (timeRessTick > TIME_TO_RESSURECT) {
            timeRessTick = 0;
            mageCharacter.Visible = true;
            deathEffect.Visible = false;
            isDead = false;
            state = State.Idle;
            stats.setStat(Stat.StatType.CurrentHealth, stats.GetStat(Stat.StatType.MaxHealth) / 2);
        }
    }
    //--added by kalle
    private bool IsOkToPlayAnimation() {
        return animationPlayer.CurrentAnimation != "Spellcast_Shoot" && animationPlayer.CurrentAnimation != "Dodge_Forward"
            && animationPlayer.CurrentAnimation != "Hit_B";
    }
    private void ParryAreaExitedBody(Node body){
        currentTouching.Remove(body as IDeflectable);
    }
	private void ParryAreaExited(Area3D area) {
		currentTouching.Remove(area as IDeflectable);
	}
    private void ParryAreaEnteredBody(Node body){
        if (body as IDeflectable != null) {
            currentTouching.Add(body as IDeflectable);
        }
    }
	private void ParryAreaEntered(Area3D area) {
		if (area as IDeflectable != null) {
            if (!isDeflecting) {
                currentTouching.Add(area as IDeflectable);
            }
		}
	}
    private void ScreenEntered() {
        onScreen = true;
    }
    private void ScreenExited() {
        onScreen = false;
    }
    private void AnimationFinished(StringName animName) {
        if (animName == "Spawn_Air") spawned = false;
    }

    private void HandleInput() {
        if (InputManager.Instance().IsJustPressedButton(ID, JoyButton.RightShoulder) && currentVisualCatchAlpha > VISUAL_CATCH_ALPHA_MIN && deflectCooldownTick <= 0 && !isDead) {
            isDeflecting = true;
            deflectArea.Scale = VISUAL_CATCH_SCALE;
            deflectSphere.Visible = true;
        }
        if ((InputManager.Instance().IsJustReleasedButton(ID, JoyButton.RightShoulder) || currentVisualCatchAlpha < VISUAL_CATCH_ALPHA_MIN) && isDeflecting) {
            isDeflecting = false;
            doDeflect = true;
            deflectArea.Scale = VISUAL_CATCH_SCALE;
            deflectSphere.Visible = false;
            invincibilityTick = INVINCIBILITY_DURATION;
            deflectCooldownTick = DEFLECT_COOLDOWN;
        }
        if (InputManager.Instance().IsJustPressedButton(ID, JoyButton.LeftShoulder) && dodgeCooldownTick >= DODGE_COOLDOWN && !isDead) {
            doDodge = true;
            invincibilityTick = INVINCIBILITY_DURATION;
        }
        if (InputManager.Instance().IsJustPressedAxis(ID, JoyAxis.TriggerRight) && currentVisualCatchAlpha > CAST_COST) {
            currentVisualCatchAlpha -= CAST_COST;
            SetVisualCatchAlpha(currentVisualCatchAlpha);
            FireBall.Fire(Position, Transform);
            invincibilityTick = INVINCIBILITY_DURATION;
        }
    }

    private void UpdateDeflect(double delta) {
        if (isDeflecting) {
            foreach (var _item in currentTouching) {
                var item = (_item as Node3D);
                if (item == null) continue;

                _item.Hold();
                if (item.GlobalPosition.DistanceTo(this.GlobalPosition) > 2.5f) {
                    Vector3 inwards = item.GlobalPosition.DirectionTo(this.GlobalPosition);
                    item.GlobalPosition += inwards * (item.GlobalPosition.DistanceTo(this.GlobalPosition) * (float)(delta*3));
                }
            }

            if (!deflectArea.Scale.IsEqualApprox(Vector3.One)) {
                deflectArea.Scale = deflectArea.Scale.Lerp(Vector3.One, (float)(delta * 15));
            }
            currentVisualCatchAlpha -= (float)(delta*DEFLECT_COST);
            SetVisualCatchAlpha(currentVisualCatchAlpha);
        }
        if (doDeflect) {
            doDeflect = false;
            Node3D target = null;

            raycast.Transform = new Transform3D(new Basis(Vector3.Up, -AIM_HELP_ANGLE), Vector3.Zero);
            for (float i = -AIM_HELP_ANGLE; i <= AIM_HELP_ANGLE; i += (float)AIM_HELP_ANGLE/40) {
                raycast.ForceRaycastUpdate();
                var _object = raycast.GetCollider();
                target = target == null ? _object as Node3D : target;

                if ((_object as Node3D) != null) {
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
                    float yRotation = -direction.SignedAngleTo(Vector3.Forward, Vector3.Up);
                    _item.Deflect(yRotation, target);
                } else {
                    _item.Deflect(Transform.Basis.GetEuler().Y, target);
                }
            }
        }
    }
    private void UpdateCooldownTicks(double delta) {
		dodgeCooldownTick += (float)delta;
        invincibilityTick -= (float)delta;
        deflectCooldownTick -= (float)delta;
        if (currentVisualCatchAlpha < VISUAL_CATCH_ALPHA_MAX && !isDeflecting) {
            currentVisualCatchAlpha += (float)(DEFLECT_REGENERATION*delta);
            SetVisualCatchAlpha(currentVisualCatchAlpha);
        }
        if (invincibilityTick < 0) {
            isHit = false;
        }
    }
	private void RotateToSlerp(Vector2 inputRotation, double delta) {
        // Calculates angle to create quaternion
        float angle = Vector2.Up.AngleTo(inputRotation);
        Godot.Quaternion quaternionTargetDirection = new Quaternion(-Transform.Basis.Y, angle);
        var quaternion = Transform.Basis.GetRotationQuaternion();

        quaternion = quaternion.Slerp(quaternionTargetDirection, (float)(stats.GetStat(Stat.StatType.RotationSpeed) * delta));

        // Sets the rotation to the transform
        Transform = new Transform3D(new Basis(quaternion), Transform.Origin);
    }
    private void RotateTo(Vector2 inputRotation) {
        // Calculates angle to create quaternion
        float angle = Vector2.Up.AngleTo(inputRotation);
        Godot.Quaternion quaternionTargetDirection = new Quaternion(-Transform.Basis.Y, angle);

        // Sets the rotation to the transform
        Transform3D transform = Transform;
        transform.Basis = new Basis(quaternionTargetDirection);
        Transform = transform;
    }
    private void SetVisualCatchAlpha(float visualCatchAlpha) {
        parryOverrideMaterial.AlbedoColor = new Color(0, 0, 1, visualCatchAlpha);
        currentVisualCatchAlpha = visualCatchAlpha;
        deflectArea.MaterialOverride = parryOverrideMaterial;
        deflectSphere.MaterialOverride = parryOverrideMaterial;
    }

    public void Deflect(float yRotation, Node3D target) {
        Transform3D transform = new Transform3D(new Basis(Vector3.Up, yRotation), Position);
        Transform = transform;
        Velocity = (-Transform.Basis.Z * 10);
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
