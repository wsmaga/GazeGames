using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    private VoiceControllerInterface voiceControllerInterface;

    private void Start()
    {
        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();
        voiceControllerInterface.StartListening();
    }

    public void OnVoiceRecognizerResults(string results)
    {
        string lowercase = results.ToLower();
        switch (lowercase)
        {
            case string a when a.Contains("wyjście") || a.Contains("wyjdź") || a.Contains("koniec"):
                Application.Quit();
                break;
            case string b when b.Contains("raz") || b.Contains("jeden") || b.Contains("1"):
                SceneManager.LoadScene("SayTarget", LoadSceneMode.Single);
                break;
            case string c when c.Contains("dwa") || c.Contains("2"):
                SceneManager.LoadScene("FollowThePath", LoadSceneMode.Single);
                break;
            default:
                voiceControllerInterface.StartListening();
                break;
        }
    }

}
