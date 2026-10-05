using TownOfUs;
using UnityEngine;

namespace MiraUnleashed;

public static class MiraUnleashedColors
{
    public static Color Trapper => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(166, 209, 179, 255);
    public static Color SerialKiller => new Color32(58, 102, 192, 255);
    public static Color Witch => Palette.ImpostorRed;
    public static Color Forestaller => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(241, 196, 15, 255);
    public static Color Wraith => Palette.ImpostorRed;
    public static Color Mirage => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(222, 168, 94, 255);
    public static Color Hacker => Palette.ImpostorRed;
    public static Color Injector => Palette.ImpostorRed;
    public static Color Charlatan => Palette.ImpostorRed;
    public static Color Scavenger => new Color32(139, 69, 19, 255);
}
