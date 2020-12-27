using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using UnityEngine.UI;

public class SplashScreenManager : MonoBehaviour
{

    [SerializeField]
    private Canvas LoadingScreen;

    [SerializeField]
    private Text PermissionsWaitText;

    [SerializeField]
    private Text PermissionsDeniedText;

    [SerializeField]
    private Text IntroductionText;

    private readonly float TEXT_FADE_TIME = 3f;

    private VoiceControllerInterface voiceControllerInterface;

    private SplashScreenState splashScreenState = SplashScreenState.CheckPermissions;

    private void Start()
    {
        LoadingScreen.enabled = false;

#if PLATFORM_ANDROID
        Permission.RequestUserPermission(Permission.Microphone);
#endif
        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();
        voiceControllerInterface.StartListening();
        ChangeState();
    }

    public void OnVoiceRecognizerResults(string results)
    {
        StartCoroutine(StartRecognizingAfterDelay(results));
    }

    private IEnumerator StartRecognizingAfterDelay(string results)
    {
        yield return new WaitForSeconds(0.1f);
        string lowercase = results.ToLower();
        switch (lowercase)
        {
            case string a when a.Contains("wyjście") || a.Contains("wyjdź") || a.Contains("koniec"):
                LoadingScreen.enabled = true;
                Application.Quit();
                break;
            case string b when b.Contains("dalej") || b.Contains("okej") || b.Contains("tak"):
                ChangeState();
                break;
            default:
                voiceControllerInterface.StartListening();
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

    private void ChangeState()
    {
        switch (splashScreenState)
        {
            case SplashScreenState.CheckPermissions:
                if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
                {
                    splashScreenState = SplashScreenState.PermissionsDenied;
                    StartCoroutine(FadeToAlpha(TEXT_FADE_TIME, PermissionsWaitText));
                    StartCoroutine(UnfadeFromAlpha(TEXT_FADE_TIME, PermissionsDeniedText));
                }
                else
                {
                    splashScreenState = SplashScreenState.DisplayIntroduction;
                    StartCoroutine(FadeToAlpha(TEXT_FADE_TIME, PermissionsWaitText));
                    StartCoroutine(UnfadeFromAlpha(TEXT_FADE_TIME, IntroductionText));
                }
                break;
            case SplashScreenState.PermissionsDenied:
                break;
            case SplashScreenState.DisplayIntroduction:
                LoadingScreen.enabled = true;
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                break;
            default:
                break;
        }
    }

}
