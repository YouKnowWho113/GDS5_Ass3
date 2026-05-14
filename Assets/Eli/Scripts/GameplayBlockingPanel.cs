using UnityEngine;

public class GameplayBlockingPanel : MonoBehaviour
{
    public string lockReason = "UI";

    private void OnEnable()
    {
        GameplayInputLock.Lock(lockReason);
    }

    private void OnDisable()
    {
        GameplayInputLock.Unlock(lockReason);
    }
}