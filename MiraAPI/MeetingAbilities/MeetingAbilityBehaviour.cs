using System;
using System.Globalization;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace MiraAPI.MeetingAbilities;

[RegisterInIl2Cpp]
public class MeetingAbilityBehaviour(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    private bool _init = false;
    private TargetedMeetingButton _button = null!;
    public PassiveButton Button;
    public PlayerVoteArea VoteArea;
    public SpriteRenderer Renderer;
    public TextMeshPro CooldownText;
    public TextMeshPro UsesText;
    public void Initialize(TargetedMeetingButton button, PlayerVoteArea voteArea)
    {
        if (_init) return;
        VoteArea = voteArea;
        _button = button;
        Renderer = GetComponent<SpriteRenderer>();
        SetFillUp(1, 1);
        if (!TryGetComponent(out Button))
        {
            Error($"Could not initialize MeetingButtonBehaviour for {Button.GetType().Name}, Destroying...");
            Destroy(this);
        }
        _init = true;
        CooldownText = Instantiate(HudManager.Instance.KillButton.cooldownTimerText, transform);
        CooldownText.GetComponent<TextTranslatorTMP>().Destroy();
        CooldownText.transform.localPosition = new Vector3(0, 0, -10);
        CooldownText.m_maxFontSize = 3f;
        CooldownText.color = new Color(1, 1, 1, 0.7f);
        UsesText = Instantiate(CooldownText, transform);
        UsesText.transform.localPosition = new Vector3(0.25f, 0.25f, -10);
        UsesText.transform.localScale = Vector3.one * 0.7f;
    }

    private void OnEnable()
    {
        if (!_init) return;
        Button.enabled = true;
        Renderer.enabled = true;
        if (_button.Enabled(PlayerControl.LocalPlayer.Data.Role) && _button.IsTargetValid(VoteArea)) return;
        Button.enabled = false;
        Renderer.enabled = false;
    }

    public void Update()
    {
        if (!_init) return;
        if (_button.Timer < 0)
        {
            Renderer.material.SetFloat("_Desat", 0.0f);
            CooldownText.text = string.Empty;
            UsesText.text = _button.LimitedUses ? _button.UsesRemaining.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
        else
        {
            Renderer.material.SetFloat("_Desat", 0.5f);
            CooldownText.text = _button.Timer.ToString(_button.CooldownTimerFormatString, NumberFormatInfo.InvariantInfo);
            UsesText.text = string.Empty;
        }
        SetFillUp(_button.Timer, _button.Cooldown);
    }

    public void SetFillUp(float timer, float maxTimer)
    {
        float percentCool = Mathf.Clamp(timer / maxTimer, 0.001f, 1f);
        if (percentCool <= 0) percentCool = 1f;
        Renderer.material.SetFloat("_Percent", percentCool);
    }
}
