using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main menu game manager
/// </summary>
public class MainMenuManager : MonoBehaviour
{

    /// <summary>
    /// Voice recognizer interface
    /// </summary>
    private VoiceControllerInterface voiceControllerInterface;

    /// <summary>
    /// Canvas to display during loading
    /// </summary>
    [SerializeField]
    private Canvas LoadingScreen;

    /// <summary>
    /// Text to be displayed alongside menu controls
    /// </summary>
    [SerializeField]
    private Text DescText;

    /// <summary>
    /// Text with helping info to be displayed in place of DescText at the user demand
    /// </summary>
    [SerializeField]
    private Text HelpText;

    /// <summary>
    /// How many seconds does the text fade to or from full alpha
    /// </summary>
    private readonly float TEXT_FADE_TIME = 2f;

    /// <summary>
    /// Set to true when the help text is currently displayed instead of default description text
    /// </summary>
    private bool showingHelp = false;

    private void Start()
    {
        LoadingScreen.enabled = false;  // no loading at the beginning
        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();  // initializing voice recognizer
        voiceControllerInterface.StartListening();
    }

    /// <summary>
    /// Parse voice recognizer results
    /// </summary>
    /// <param name="results">Voice recognizer results</param>
    public void OnVoiceRecognizerResults(string results)
    {
        StartCoroutine(StartRecognizingAfterDelay(results));
    }

    /// <summary>
    /// Parses voice recognizer results after a small delay
    /// </summary>
    /// <param name="results">Results to be parsed</param>
    /// <returns></returns>
    private IEnumerator StartRecognizingAfterDelay(string results)
    {
        yield return new WaitForSeconds(0.1f);
        string lowercase = results.ToLower();
        switch (lowercase)
        {
            case string a when a.Contains("wyjście") || a.Contains("wyjdź") || a.Contains("koniec"):  // end the application
                LoadingScreen.enabled = true;
                Application.Quit();
                break;
            case string b when b.Contains("raz") || b.Contains("jeden") || b.Contains("1"):  // picking a minigame
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
            case string e when e.Contains("pomoc") || e.Contains("opis"):  // switch between two texts
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
