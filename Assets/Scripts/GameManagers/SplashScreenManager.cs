using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using UnityEngine.UI;

/// <summary>
/// Splash screen game manager.
/// </summary>
public class SplashScreenManager : MonoBehaviour
{

    /// <summary>
    /// Canvas to be displayed during loading times.
    /// </summary>
    [SerializeField]
    private Canvas LoadingScreen;

    /// <summary>
    /// Text to be displayed during the wait for permissions.
    /// </summary>
    [SerializeField]
    private Text PermissionsWaitText;

    /// <summary>
    /// Text to be displayed after user denied necessary permissions.
    /// </summary>
    [SerializeField]
    private Text PermissionsDeniedText;

    /// <summary>
    /// Text to be displayed after permissions were granted and application is ready to proceed.
    /// </summary>
    [SerializeField]
    private Text IntroductionText;

    /// <summary>
    /// How long the texts fade to or from full alpha color, in seconds.
    /// </summary>
    private readonly float TEXT_FADE_TIME = 3f;

    /// <summary>
    /// Voice recognizer interface.
    /// </summary>
    private VoiceControllerInterface voiceControllerInterface;

    /// <summary>
    /// Describes the manager's current state(waiting for permissions, ready to proceed, etc.)
    /// </summary>
    private SplashScreenState splashScreenState = SplashScreenState.CheckPermissions;

    private void Start()
    {
        LoadingScreen.enabled = false;  // no loading screen at the start

#if PLATFORM_ANDROID
        Permission.RequestUserPermission(Permission.Microphone);  // requesting permission for microphone at the very start
#endif
        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();  // initializing voice recognizer
        voiceControllerInterface.StartListening();

        ChangeState();  // after initialization - first state change
    }

    /// <summary>
    /// Parses voice recognizer results.
    /// </summary>
    /// <param name="results">Voice recognizer results string.</param>
    public void OnVoiceRecognizerResults(string results)
    {
        StartCoroutine(StartRecognizingAfterDelay(results));
    }

    /// <summary>
    /// Parses voice recognizer results after a small delay.
    /// </summary>
    /// <param name="results">Voice recognizer result string.</param>
    /// <returns></returns>
    private IEnumerator StartRecognizingAfterDelay(string results)
    {
        yield return new WaitForSeconds(0.1f);
        string lowercase = results.ToLower();
        switch (lowercase)
        {
            case string a when a.Contains("wyjście") || a.Contains("wyjdź") || a.Contains("koniec"):  // exiting the app
                LoadingScreen.enabled = true;
                Application.Quit();
                break;
            case string b when b.Contains("dalej") || b.Contains("okej") || b.Contains("tak"):  // proceed with the manager's state
                ChangeState();
                break;
            default:
                voiceControllerInterface.StartListening();  // try again to recognize the speech after failuree
                break;
        }
    }

    // Text fading based on: https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
    private IEnumerator FadeToAlpha(float t, Text text)
    {
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    private IEnumerator UnfadeFromAlpha(float t, Text text)
    {
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    /// <summary>
    /// Changes the game manager's state depending on current state and various conditions.
    /// </summary>
    private void ChangeState()
    {
        switch (splashScreenState)
        {
            case SplashScreenState.CheckPermissions:  // just checked for user to grant the app permissions:
                if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))  // permissions not granted:
                {
                    splashScreenState = SplashScreenState.PermissionsDenied;
                    StartCoroutine(FadeToAlpha(TEXT_FADE_TIME, PermissionsWaitText));
                    StartCoroutine(UnfadeFromAlpha(TEXT_FADE_TIME, PermissionsDeniedText));
                }
                else  // permissions granted:
                {
                    splashScreenState = SplashScreenState.DisplayIntroduction;
                    StartCoroutine(FadeToAlpha(TEXT_FADE_TIME, PermissionsWaitText));
                    StartCoroutine(UnfadeFromAlpha(TEXT_FADE_TIME, IntroductionText));
                }
                break;
            case SplashScreenState.PermissionsDenied:  // just denied app permissions by the user:
                break;
            case SplashScreenState.DisplayIntroduction:  // just received permissions from the user:
                LoadingScreen.enabled = true;
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                break;
            default:
                break;
        }
    }

}
