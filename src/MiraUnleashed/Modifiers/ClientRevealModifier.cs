using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraUnleashed.Options.Roles.Neutral;
using MiraUnleashed.Roles.Neutral;
using MiraUnleashed.Utilities;
using TownOfUs.Modifiers;
using TownOfUs.Utilities;

namespace MiraUnleashed.Modifiers;

/// <summary>
/// Reveal modifier for client - makes client's role visible to their lawyer (if setting is enabled).
/// </summary>
public sealed class ClientRevealModifier : RevealModifier
{
    private readonly RoleBehaviour _role;

    [HideFromIl2Cpp] public bool IsHiddenFromList => true;

    public ClientRevealModifier(RoleBehaviour role)
        : base((int)ChangeRoleResult.Nothing, true, role)
    {
        _role = role;
    }

    public override string ModifierName => "Client Revealed";

    public override void OnActivate()
    {
        base.OnActivate();
        if (RevealRole && ShownRole == null)
        {
            ShownRole = _role ?? Player.Data?.Role;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
        {
            Visible = false;
            return;
        }

        if (!localPlayer.IsRole<LawyerRole>())
        {
            Visible = false;
            return;
        }

        if (!LawyerUtils.HasLawyerClientRelationship(localPlayer, Player))
        {
            Visible = false;
            return;
        }

        Visible = OptionGroupSingleton<LawyerOptions>.Instance.CanSeeClientRole;
    }
}
