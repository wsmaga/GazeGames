using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VoiceController))]
public class VoiceControllerInterface : MonoBehaviour
{

    public Text errorLabel;
    public Text debugLabel;

    private VoiceController voiceController;

    #region MonoBehaviour methods

    void Start()
    {
        voiceController = GetComponent<VoiceController>();
    }

    private void OnEnable()
    {
        VoiceController.resultsCallback += OnRecognizerResults;
        VoiceController.errorCallback += OnRecognizerError;
    }

    private void OnDisable()
    {
        VoiceController.resultsCallback -= OnRecognizerResults;
        VoiceController.errorCallback -= OnRecognizerError;
    }

    #endregion

    #region Public methods

    public void StartListening()
    {
        voiceController.GetSpeech();
    }

    #endregion

    #region Private methods

    private void OnRecognizerResults(string results)
    {
        debugLabel.text = "Received results: " + results;
        errorLabel.text = "";
        SendMessageUpwards("OnVoiceRecognizerResults", results);
    }

    private void OnRecognizerError(string error)
    {
        errorLabel.text = error;
        voiceController.GetSpeech();
    }

    #endregion

}
