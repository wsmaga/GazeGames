using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interface to communicate between game managers and voice recognition library.
/// </summary>
[RequireComponent(typeof(VoiceController))]
public class VoiceControllerInterface : MonoBehaviour
{

    /// <summary>
    /// Text field to display recognition errors.
    /// </summary>
    public Text errorLabel;
    /// <summary>
    /// Text field to display recognition results.
    /// </summary>
    public Text debugLabel;

    /// <summary>
    /// Library object instance.
    /// </summary>
    private VoiceController voiceController;

    #region MonoBehaviour methods

    void Start()
    {
        voiceController = GetComponent<VoiceController>();  // initializing library instance
    }

    private void OnEnable()
    {
        VoiceController.resultsCallback += OnRecognizerResults;  // registering callbacks that are invoked for recognition results and errors
        VoiceController.errorCallback += OnRecognizerError;
    }

    private void OnDisable()
    {
        VoiceController.resultsCallback -= OnRecognizerResults;  // unloading registered callbacks
        VoiceController.errorCallback -= OnRecognizerError;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Orders the library to begin the recognition.
    /// </summary>
    public void StartListening()
    {
        voiceController.GetSpeech();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Acts as a callback to receive recognition results.
    /// </summary>
    /// <param name="results">Recognition results string.</param>
    private void OnRecognizerResults(string results)
    {
        debugLabel.text = "Received results: " + results;  // printing data to text fields
        errorLabel.text = "";
        SendMessageUpwards("OnVoiceRecognizerResults", results);  // sending data to parent game manager
    }

    /// <summary>
    /// Acts as a callback to receive recognition error messages.
    /// </summary>
    /// <param name="error">Received error message.</param>
    private void OnRecognizerError(string error)
    {
        errorLabel.text = error;  // printing  error to the text field
        voiceController.GetSpeech();  // restarting the recognition
    }

    #endregion

}
