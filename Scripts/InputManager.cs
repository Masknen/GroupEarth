using Godot;
using Godot.Collections;
using System;

public partial class InputManager : Node
{
    static private InputManager _instance;

    static public InputManager Instance() {
        if (_instance == null) {
            _instance = new InputManager();
        }
        return _instance;
    }



    private Array<Dictionary<JoyButton, int>> playersPressedButton = new Array<Dictionary<JoyButton, int>>();
    private Array<Dictionary<JoyAxis, int>> playersPressedAxis = new Array<Dictionary<JoyAxis, int>>();

    private Array<Dictionary<JoyButton, int>> playersLastUpdatePressedButton = new Array<Dictionary<JoyButton, int>>();
    private Array<Dictionary<JoyAxis, int>> playersLastUpdatePressedAxis = new Array<Dictionary<JoyAxis, int>>();

    public override void _Ready()
	{
        _instance = this;
        playersPressedButton.Add(new Dictionary<JoyButton, int>());
        playersPressedButton.Add(new Dictionary<JoyButton, int>());
        playersPressedAxis.Add(new Dictionary<JoyAxis, int>());
        playersPressedAxis.Add(new Dictionary<JoyAxis, int>());

        playersLastUpdatePressedButton.Add(new Dictionary<JoyButton, int>());
        playersLastUpdatePressedButton.Add(new Dictionary<JoyButton, int>());
        playersLastUpdatePressedAxis.Add(new Dictionary<JoyAxis, int>());
        playersLastUpdatePressedAxis.Add(new Dictionary<JoyAxis, int>());

        for (int ID = 0; ID < 2; ID++) {
            for (int i = 0; i < (int)JoyButton.SdlMax; i++) {
                playersPressedButton[ID][(JoyButton)i] = 0;
            }
            for (int i = 0; i < (int)JoyAxis.SdlMax; i++) {
                playersPressedAxis[ID][(JoyAxis)i] = 0;
            }
        }
        for (int ID = 0; ID < 2; ID++) {
            for (int i = 0; i < (int)JoyButton.SdlMax; i++) {
                playersLastUpdatePressedButton[ID][(JoyButton)i] = 0;
            }
            for (int i = 0; i < (int)JoyAxis.SdlMax; i++) {
                playersLastUpdatePressedAxis[ID][(JoyAxis)i] = 0;
            }
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        for (int ID = 0; ID < 2; ID++) {
            for (int i = 0; i < (int)JoyButton.SdlMax; i++) {
                playersLastUpdatePressedButton[ID][(JoyButton)i] = playersPressedButton[ID][(JoyButton)i];
                if (Input.IsJoyButtonPressed(ID, (JoyButton)i)) {
                    playersPressedButton[ID][(JoyButton)i]++;
                } else {
                    playersPressedButton[ID][(JoyButton)i] = 0;
                }
            }
            for (int i = 0; i < (int)JoyAxis.SdlMax; i++) {
                playersLastUpdatePressedAxis[ID][(JoyAxis)i] = playersPressedAxis[ID][(JoyAxis)i];
                if (Input.GetJoyAxis(ID, (JoyAxis)i) >= 0.15) {
                    playersPressedAxis[ID][(JoyAxis)i]++;
                } else {
                    playersPressedAxis[ID][(JoyAxis)i] = 0;
                }
            }
        }
	}

    public bool IsJustPressedButton(int ID, JoyButton joyButton) {
        return playersPressedButton[ID][joyButton] == 1;
    }
    public bool IsJustPressedAxis(int ID, JoyAxis joyAxis) {
        return playersPressedAxis[ID][joyAxis] == 1;
    }
    public bool IsJustReleasedButton(int ID, JoyButton joyButton) {
        return playersPressedButton[ID][joyButton] == 0 && playersLastUpdatePressedButton[ID][joyButton] != 0;
    }
    public bool IsJustReleasedAxis(int ID, JoyAxis joyAxis) {
        return playersPressedAxis[ID][joyAxis] == 0 && playersLastUpdatePressedAxis[ID][joyAxis] != 0;
    }

}
