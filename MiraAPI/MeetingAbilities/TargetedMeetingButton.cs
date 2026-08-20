using System.Collections.Generic;
using MiraAPI.Utilities.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace MiraAPI.MeetingAbilities;

/// <summary>
/// Abstract class for creating custom meeting abilities.
/// </summary>
public abstract class TargetedMeetingButton
{
    /// <summary>
    /// Gets the name of the <see cref="TargetedMeetingButton"/>.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the Max Uses of the button.
    /// Set it to 0 for Infinite uses.
    /// </summary>
    public abstract int MaxUses { get; }

    /// <summary>
    /// Gets or sets the amount of uses left for this button.
    /// </summary>
    public int UsesRemaining { get; set; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="TargetedMeetingButton"/> has limited uses.
    /// </summary>
    public bool LimitedUses => MaxUses != 0;

    /// <summary>
    /// Gets the cooldown time for this button.
    /// </summary>
    public abstract float Cooldown { get; }

    /// <summary>
    /// Gets or sets the current cooldown timer for this button.
    /// </summary>
    public float Timer { get; set; }

    /// <summary>
    /// Gets the cooldown time for this button.
    /// </summary>
    public virtual string CooldownTimerFormatString => "0";

    /// <summary>
    /// Gets the sprite used for this button.
    /// </summary>
    public abstract LoadableAsset<Sprite> Sprite { get; }

    /// <summary>
    /// Gets the <see cref="MeetingButtonUsesMode"/> for this button.
    /// </summary>
    public virtual MeetingButtonUsesMode ButtonUsesMode { get; } = MeetingButtonUsesMode.PerGame;

    /// <summary>
    /// Determines if the button is enabled.
    /// </summary>
    /// <param name="r">The local player's <see cref="RoleBehaviour"/>.</param>
    /// <returns>Whether the button is enabled or not.</returns>
    public abstract bool Enabled(RoleBehaviour r);

    /// <summary>
    /// Callback method for when the button is clicked.
    /// </summary>
    /// <param name="playerVoteArea">The target <see cref="PlayerVoteArea"/>.</param>
    protected abstract void OnClick(PlayerVoteArea playerVoteArea);

    /// <summary>
    /// Handles the button creation.
    /// </summary>
    /// <param name="playerVoteArea">The target <see cref="PlayerVoteArea"/>.</param>
    /// <returns>The <see cref="MeetingAbilityBehaviour"/> instance.</returns>
    public virtual MeetingAbilityBehaviour CreateButton(PlayerVoteArea playerVoteArea)
    {
        if (MeetingHud.Instance == null) return null!;

        var btn = Object.Instantiate(MeetingHud.Instance.PlayerButtonPrefab.ConfirmButton).GetComponent<PassiveButton>();
        btn.gameObject.name = Name + "Button";

        var spriteRenderer = btn.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.LoadAsset();
        spriteRenderer.material = new Material(HudManager.Instance.KillButton.graphic.material);
        spriteRenderer.SetCooldownNormalizedUvs();
        btn.gameObject.SetActive(true);

        btn.OnClick = new Button.ButtonClickedEvent();
        btn.OnClick.AddListener(new System.Action(() => ClickHandler(playerVoteArea)));
        var abilityBehaviour = btn.gameObject.AddComponent<MeetingAbilityBehaviour>();
        abilityBehaviour.Initialize(this, playerVoteArea);
        Timer = Cooldown;
        return abilityBehaviour;
    }

    /// <summary>
    /// Called every <see cref="MeetingHud.Update"/>.
    /// Handles the timer logic.
    /// </summary>
    public virtual void UpdateHandler()
    {
        Timer -= Time.deltaTime;
        Timer = Mathf.Clamp(Timer, 0, int.MaxValue);
        FixedUpdate();
    }

    /// <summary>
    /// Callback method for handling logic every frame.
    /// </summary>
    public virtual void FixedUpdate()
    {
    }

    /// <summary>
    /// Called when the button is clicked
    /// Handles increasing cooldown and decreasing uses remaining.
    /// </summary>
    /// <param name="playerVoteArea">The target <see cref="PlayerVoteArea"/>.</param>
    public virtual void ClickHandler(PlayerVoteArea playerVoteArea)
    {
        if (UsesRemaining <= 0 && LimitedUses) return;
        if (Timer > 0) return;
        Timer = Cooldown;
        UsesRemaining--;
        playerVoteArea.Cancel();
        OnClick(playerVoteArea);
    }

    /// <summary>
    /// Determines whether the target is valid.
    /// </summary>
    /// <param name="playerVoteArea">The target <see cref="PlayerVoteArea"/>.</param>
    /// <returns>Whether the target is valid or not.</returns>
    public virtual bool IsTargetValid(PlayerVoteArea playerVoteArea)
    {
        return playerVoteArea != null && playerVoteArea.PlayerId.Value != PlayerControl.LocalPlayer.Data.PlayerId;
    }
}
