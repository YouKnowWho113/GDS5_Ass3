using System.Collections.Generic;

public static class GameplayInputLock
{
    private static readonly HashSet<string> activeLocks = new HashSet<string>();

    public static bool IsLocked
    {
        get { return activeLocks.Count > 0; }
    }

    public static void Lock(string reason)
    {
        activeLocks.Add(Normalize(reason));
    }

    public static void Unlock(string reason)
    {
        activeLocks.Remove(Normalize(reason));
    }

    public static bool IsLockedBy(string reason)
    {
        return activeLocks.Contains(Normalize(reason));
    }

    public static void ClearAll()
    {
        activeLocks.Clear();
    }

    private static string Normalize(string reason)
    {
        return string.IsNullOrWhiteSpace(reason) ? "Unknown" : reason;
    }
}
