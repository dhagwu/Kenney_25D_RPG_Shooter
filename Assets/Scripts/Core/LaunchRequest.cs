public enum LaunchAction
{
    None,
    NewGame,
    Continue
}

public static class LaunchRequest
{
    public static LaunchAction PendingAction = LaunchAction.None;
}