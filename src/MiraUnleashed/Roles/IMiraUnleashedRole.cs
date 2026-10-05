using MiraAPI.Roles;
using TownOfUs.Roles;

namespace MiraUnleashed.Roles;

public interface IMiraUnleashedRole : ITownOfUsRole
{
    string ICustomRole.IdPrefix => "MiraUnleashed.Role";

    string ICustomRole.IdPart
    {
        get
        {
            var typeName = GetType().Name;

            return typeName.EndsWith("Role", StringComparison.Ordinal)
                ? typeName[..^4]
                : typeName;
        }
    }
}
