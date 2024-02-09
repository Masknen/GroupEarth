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
    //private Array<Dictionary<JoyButton, int>> playersReleased = new Array<Dictionary<JoyButton, int>>();
    public override void _Ready()
	{
        _instance = this;
        playersPressedButton.Add(new Dictionary<JoyButton, int>());
        playersPressedButton.Add(new Dictionary<JoyButton, int>());
        playersPressedAxis.Add(new Dictionary<JoyAxis, int>());
        playersPressedAxis.Add(new Dictionary<JoyAxis, int>());
        //playersReleased.Add(new Dictionary<JoyButton, int>());
        //playersReleased.Add(new Dictionary<JoyButton, int>());
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        for (int ID = 0; ID < 2; ID++) {
            for (int i = 0; i < (int)JoyButton.SdlMax; i++) {
                if (Input.IsJoyButtonPressed(ID, (JoyButton)i)) {
                    playersPressedButton[ID][(JoyButton)i]++;
                }
                if (!Input.IsJoyButtonPressed(ID, (JoyButton)i)) {
                    playersPressedButton[ID][(JoyButton)i] = 0;
                }
            }
            for (int i = 0; i < (int)JoyAxis.SdlMax; i++) {
                if (Input.GetJoyAxis(ID, (JoyAxis)i) >= 0.15) {
                    playersPressedAxis[ID][(JoyAxis)i]++;
                    if (playersPressedAxis[ID][(JoyAxis)i] < 5)
                        GD.Print(playersPressedAxis[ID][(JoyAxis)i]);
                }
                if (Input.GetJoyAxis(ID, (JoyAxis)i) < 0.15) {
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

}
