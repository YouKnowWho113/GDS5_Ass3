using System.Collections.Generic;
using UnityEngine;

public static class GameplayInputLock
{
    private static readonly HashSet<string> lockReasons = new HashSet<string>();

    public static bool IsLocked => lockReasons.Count > 0;

    public static void Lock(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return;

        lockReasons.Add(reason);
    }

    public static void Unlock(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return;

        lockReasons.Remove(reason);
    }

    public static void ClearAll()
    {
        lockReasons.Clear();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetOnSceneLoad()
    {
        lockReasons.Clear();
    }
}