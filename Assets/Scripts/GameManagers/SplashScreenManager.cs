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

    /// <summary>
    /// Set to true when the user provides necessary permissions for the app.
    /// </summary>
    private bool hasPermissions = false;

    private void Start()
    {
        LoadingScreen.enabled = false;  // no loading screen at the start

#if PLATFORM_ANDROID
        Permission.RequestUserPermission(Permission.Microphone);  // requesting permission for microphone at the very start
#endif
    }

    private void OnGUI()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))  // permissions not granted:
        {
            splashScreenState = SplashScreenState.PermissionsDenied;
            Color currColor = PermissionsWaitText.color;
            currColor.a = 0;
            PermissionsWaitText.color = currColor;

            currColor = PermissionsDeniedText.color;
            currColor.a = 1;
            PermissionsDeniedText.color = currColor;
        }
        else  // permissions granted:
        {
            splashScreenState = SplashScreenState.DisplayIntroduction;
            hasPermissions = true;
        }
    }

    private void Update()
    {
        if (hasPermissions)
        {
            hasPermissions = false;
            StartCoroutine(FadeToAlpha(TEXT_FADE_TIME, PermissionsWaitText));
            StartCoroutine(FadeToAlpha(TEXT_FADE_TIME, PermissionsDeniedText));
            StartCoroutine(UnfadeFromAlpha(TEXT_FADE_TIME, IntroductionText));
            voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();  // initializing voice recognizer
            voiceControllerInterface.StartListening();
        }
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
                switch (splashScreenState)
                {
                    case SplashScreenState.PermissionsDenied:
                        LoadingScreen.enabled = true;
                        Application.Quit();
                        break;
                    case SplashScreenState.DisplayIntroduction:
                        LoadingScreen.enabled = true;
                        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                        break;
                    default:
                        break;
                }
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

}
