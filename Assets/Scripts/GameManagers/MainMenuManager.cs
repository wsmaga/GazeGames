using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    private VoiceControllerInterface voiceControllerInterface;

    [SerializeField]
    private Canvas LoadingScreen;

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
            case string d when d.Contains("trzy") || d.Contains("3"):
                LoadingScreen.enabled = true;
                SceneManager.LoadScene("Gazuma", LoadSceneMode.Single);
                break;
            default:
                voiceControllerInterface.StartListening();
                break;
        }      
    }

}
