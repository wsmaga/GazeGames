using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    private VoiceControllerInterface voiceControllerInterface;

    [SerializeField]
    private Canvas LoadingScreen;

    [SerializeField]
    private Text DescText;

    [SerializeField]
    private Text HelpText;

    private readonly float TEXT_FADE_TIME = 2f;

    private bool showingHelp = false;

    private void Start()
    {
        LoadingScreen.enabled = false;
        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();
        voiceControllerInterface.StartListening();
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
            case string b when b.Contains("raz") || b.Contains("jeden") || b.Contains("1"):
                LoadingScreen.enabled = true;
                SceneManager.LoadScene("SayTarget", LoadSceneMode.Single);
                break;
            case string c when c.Contains("dwa") || c.Contains("2"):
                LoadingScreen.enabled = true;
                SceneManager.LoadScene("FollowThePath", LoadSceneMode.Single);
                break;
            case string d when d.Contains("trzy") || d.Contains("3") || d.Contains("czy"):
                LoadingScreen.enabled = true;
                SceneManager.LoadScene("Gazuma", LoadSceneMode.Single);
                break;
            case string e when e.Contains("pomoc") || e.Contains("opis"):
                if (showingHelp)
                {
                    showingHelp = false;
                    StartCoroutine(FadeToAlpha(TEXT_FADE_TIME, HelpText));
                    StartCoroutine(UnfadeFromAlpha(TEXT_FADE_TIME, DescText));
                    voiceControllerInterface.StartListening();
                }
                else
                {
                    showingHelp = true;
                    StartCoroutine(FadeToAlpha(TEXT_FADE_TIME, DescText));
                    StartCoroutine(UnfadeFromAlpha(TEXT_FADE_TIME, HelpText));
                    voiceControllerInterface.StartListening();
                }
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

}
