using Godot;
using System;

public partial class PlayerGUI : Control
{
    private ProgressBar[] healthBar = new ProgressBar[2];
    private ProgressBar[] chargeBar = new ProgressBar[2];

    private float[] targetHealthValue = new float[2];
    private float[] targetChargeValue = new float[2];

    public override void _Process(double delta) {
        healthBar[0] = GetChild(0).GetChild<ProgressBar>(0);
        healthBar[1] = GetChild(1).GetChild<ProgressBar>(0);
        chargeBar[0] = GetChild(0).GetChild<ProgressBar>(1);
        chargeBar[1] = GetChild(1).GetChild<ProgressBar>(1);

        foreach (var player in PlayerManager.Instance.players) {
            if (player.ID >= 0) {
                targetHealthValue[player.ID] = (player.stats.GetStat(Stat.StatType.CurrentHealth) / (float)(player.stats.GetStat(Stat.StatType.MaxHealth)) * 100.0f);
                healthBar[player.ID].Value = Mathf.Lerp(healthBar[player.ID].Value, targetHealthValue[player.ID], delta*10);

                targetChargeValue[player.ID] = (player.currentVisualCatchAlpha / (float)(Player.VISUAL_CATCH_ALPHA_MAX) * 100.0f);
                chargeBar[player.ID].Value = Mathf.Lerp(chargeBar[player.ID].Value, targetChargeValue[player.ID], delta * 10);
            }
        }
    }
}
