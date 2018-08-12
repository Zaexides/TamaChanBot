using System;

namespace TamaChanBot.API
{
    //Effectively a copy of Discord.NET's GuildPermission
    [Flags]
    public enum Permission : ulong
    {
        CreateInvite                    = 0x00000001,
        Kick                            = 0x00000002,
        Ban                             = 0x00000004,
        Admin                           = 0x00000008,
        ManageChannels                  = 0x00000010,
        ManageServer                    = 0x00000020,
        AddReactions                    = 0x00000040,
        ViewAuditLog                    = 0x00000080,
        ReadMessages                    = 0x00000400,
        SendMessages                    = 0x00000800,
        SendTTSMessages                 = 0x00001000,
        ManageMessages                  = 0x00002000,
        EmbedLinks                      = 0x00004000,
        AttachFiles                     = 0x00008000,
        ReadHistory                     = 0x00010000,
        MentionEveryone                 = 0x00020000,
        UseExternalEmojis               = 0x00040000,
        VCConnect                       = 0x00100000,
        VCSpeak                         = 0x00200000,
        VCMuteMembers                   = 0x00400000,
        VCDeafenMembers                 = 0x00800000,
        VCMoveMembers                   = 0x01000000,
        VCUseVoiceActivityDetection     = 0x02000000,
        VCPriority                      = 0x00000100,
        ChangeNickname                  = 0x04000000,
        ManageNicknames                 = 0x08000000,
        ManageRoles                     = 0x10000000,
        ManageWebhooks                  = 0x20000000,
        ManageEmojis                    = 0x40000000,
    }
}
