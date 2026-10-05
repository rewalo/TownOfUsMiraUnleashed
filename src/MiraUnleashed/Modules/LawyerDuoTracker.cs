namespace MiraUnleashed.Modules;

public static class LawyerDuoTracker
{
    private static readonly Dictionary<byte, byte> LawyerToClient = new();
    private static readonly Dictionary<byte, HashSet<byte>> ClientToLawyers = new();

    public static void ClearAll()
    {
        LawyerToClient.Clear();
        ClientToLawyers.Clear();
    }

    public static void SetClient(byte lawyerId, byte clientId)
    {
        if (LawyerToClient.TryGetValue(lawyerId, out var oldClientId) &&
            ClientToLawyers.TryGetValue(oldClientId, out var oldSet))
        {
            oldSet.Remove(lawyerId);
            if (oldSet.Count == 0)
            {
                ClientToLawyers.Remove(oldClientId);
            }
        }

        LawyerToClient[lawyerId] = clientId;

        if (!ClientToLawyers.TryGetValue(clientId, out var set))
        {
            set = new HashSet<byte>();
            ClientToLawyers[clientId] = set;
        }
        set.Add(lawyerId);
    }

    public static IReadOnlyCollection<byte> GetLawyers()
    {
        return LawyerToClient.Keys.ToArray();
    }

    public static IReadOnlyCollection<byte> GetClients()
    {
        return ClientToLawyers.Keys.ToArray();
    }
}
