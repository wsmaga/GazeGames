/// <summary>
/// Describes possible states in which the SplashScreen Manager can be
/// </summary>
public enum SplashScreenState
{
    /// <summary>
    /// Currently checking for necessary permissions
    /// </summary>
    CheckPermissions,
    /// <summary>
    /// Permissions were denied by the user
    /// </summary>
    PermissionsDenied,
    /// <summary>
    /// Displaying an introduction text for the user
    /// </summary>
    DisplayIntroduction
}
