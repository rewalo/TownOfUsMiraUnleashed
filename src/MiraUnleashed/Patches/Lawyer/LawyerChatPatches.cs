using MiraAPI.GameOptions;
using MiraUnleashed.Networking;
using MiraUnleashed.Options.Roles.Neutral;
using MiraUnleashed.Roles.Neutral;
using MiraUnleashed.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using TownOfUs;
using TownOfUs.Modules;
using TownOfUs.Patches.Options;
using TownOfUs.Utilities;

namespace MiraUnleashed.Patches.Lawyer;

public static class LawyerChatPatches
{
    [MethodRpc((uint)MiraUnleashedRpc.SendLawyerChat)]
    public static void RpcSendLawyerChat(PlayerControl player, string text)
    {
        if (!OptionGroupSingleton<LawyerOptions>.Instance.LawyerChat)
        {
            return;
        }

        var localPlayer = PlayerControl.LocalPlayer;

        var isClientOfThisLawyer = LawyerUtils.IsClientOfLawyer(localPlayer, player.PlayerId);
        var isDeadAndKnows = GameHistory.IsFullyDead(localPlayer) &&
                             OptionGroupSingleton<TownOfUs.Options.GeneralOptions>.Instance.TheDeadKnow;

        var shouldMarkUnread = false;

        if (player.AmOwner)
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{TownOfUsColors.Lawyer.ToHtmlStringRGBA()}>{MiraLocaleManager.Get("MiraUnleashed.Lawyer.ChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                text, bubbleType: BubbleType.Other, onLeft: false);
            shouldMarkUnread = true;
        }

        else if (isClientOfThisLawyer)
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{TownOfUsColors.Lawyer.ToHtmlStringRGBA()}>{MiraLocaleManager.Get("MiraUnleashed.Lawyer.ChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                text, bubbleType: BubbleType.Other, onLeft: true);
            shouldMarkUnread = true;
        }

        else if (isDeadAndKnows)
        {
            var wasClientOfThisLawyer = LawyerUtils.IsClientOfLawyer(localPlayer, player.PlayerId);

            var deadPlayerLawyerRole = localPlayer.GetRole<LawyerRole>();
            var wasLawyerOfSender = deadPlayerLawyerRole != null &&
                                    deadPlayerLawyerRole.Client != null &&
                                    deadPlayerLawyerRole.Client.PlayerId == player.PlayerId;

            var canSee = wasClientOfThisLawyer || wasLawyerOfSender;

            if (canSee)
            {
                MiscUtils.AddTeamChat(player.Data,
                    $"<color=#{TownOfUsColors.Lawyer.ToHtmlStringRGBA()}>{MiraLocaleManager.Get("MiraUnleashed.Lawyer.ChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                    text, bubbleType: BubbleType.Other, onLeft: !player.AmOwner);
                shouldMarkUnread = true;
            }
        }

        if (shouldMarkUnread && MeetingHud.Instance != null)
        {
            var chats = TeamChatPatches.TeamChatManager.GetAllAvailableChats();
            var hasForcedChat = chats.Any(c => c.IsForced);
            var currentChat = TeamChatPatches.CurrentChatIndex >= 0 && TeamChatPatches.CurrentChatIndex < chats.Count
                ? chats[TeamChatPatches.CurrentChatIndex]
                : null;
            if ((!TeamChatPatches.TeamChatActive || currentChat == null || currentChat.Priority != 50) && !hasForcedChat)
            {
                TeamChatPatches.TeamChatManager.MarkChatAsUnread(50);
            }
        }
    }

    [MethodRpc((uint)MiraUnleashedRpc.SendClientChat)]
    public static void RpcSendClientChat(PlayerControl player, string text)
    {
        if (!OptionGroupSingleton<LawyerOptions>.Instance.LawyerChat)
        {
            return;
        }

        var localPlayer = PlayerControl.LocalPlayer;

        var isLawyerOfThisClient = LawyerUtils.HasLawyerClientRelationship(localPlayer, player);
        var isDeadAndKnows = GameHistory.IsFullyDead(localPlayer) &&
                             OptionGroupSingleton<TownOfUs.Options.GeneralOptions>.Instance.TheDeadKnow;

        var shouldMarkUnread = false;

        if (player.AmOwner)
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{TownOfUsColors.Lawyer.ToHtmlStringRGBA()}>{MiraLocaleManager.Get("MiraUnleashed.Lawyer.ClientChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                text, bubbleType: BubbleType.Other, onLeft: false);
            shouldMarkUnread = true;
        }

        else if (isLawyerOfThisClient)
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{TownOfUsColors.Lawyer.ToHtmlStringRGBA()}>{MiraLocaleManager.Get("MiraUnleashed.Lawyer.ClientChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                text, bubbleType: BubbleType.Other, onLeft: true);
            shouldMarkUnread = true;
        }

        else if (isDeadAndKnows)
        {
            var wasLawyerOfThisClient = LawyerUtils.HasLawyerClientRelationship(localPlayer, player);

            var wasClientOfSender = LawyerUtils.IsClientOfLawyer(localPlayer, player.PlayerId);

            var canSee = wasLawyerOfThisClient || wasClientOfSender;

            if (canSee)
            {
                MiscUtils.AddTeamChat(player.Data,
                    $"<color=#{TownOfUsColors.Lawyer.ToHtmlStringRGBA()}>{MiraLocaleManager.Get("MiraUnleashed.Lawyer.ClientChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                    text, bubbleType: BubbleType.Other, onLeft: !player.AmOwner);
                shouldMarkUnread = true;
            }
        }

        if (shouldMarkUnread && MeetingHud.Instance != null)
        {
            var chats = TeamChatPatches.TeamChatManager.GetAllAvailableChats();
            var hasForcedChat = chats.Any(c => c.IsForced);
            var currentChat = TeamChatPatches.CurrentChatIndex >= 0 && TeamChatPatches.CurrentChatIndex < chats.Count
                ? chats[TeamChatPatches.CurrentChatIndex]
                : null;
            if ((!TeamChatPatches.TeamChatActive || currentChat == null || currentChat.Priority != 50) && !hasForcedChat)
            {
                TeamChatPatches.TeamChatManager.MarkChatAsUnread(50);
            }
        }
    }
}
