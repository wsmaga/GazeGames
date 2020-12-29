using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Say the target game manager
/// </summary>
[RequireComponent(typeof(VoiceController))]
public class SayTargetGameManager : MonoBehaviour
{

    /// <summary>
    /// Canvas to be displayed during loading
    /// </summary>
    [SerializeField]
    private Canvas LoadingScreen;

    /// <summary>
    /// Base prefab of the game target
    /// </summary>
    public GameObject TargetPrefab;

    /// <summary>
    /// Radius of the spawn circle for the targets
    /// </summary>
    private float TARGET_SPAWN_CIRCLE_RADIUS;

    /// <summary>
    /// Available target labels and their variants
    /// </summary>
    private List<string> targetLabels;
    /// <summary>
    /// Currently picked target label and it's variants
    /// </summary>
    private List<string> currentTargetLabelVariants;

    /// <summary>
    /// Voice recognizer interface
    /// </summary>
    private VoiceControllerInterface voiceControllerInterface;
    /// <summary>
    /// A reference to currently spawned target
    /// </summary>
    private GameObject spawnedTarget;

    /// <summary>
    /// A position on a spawning circle for a newly-spawned target
    /// </summary>
    private Vector3 NewTargetPosition
    {
        get
        {
            float randomT = Random.Range(0, 2 * Mathf.PI);
            return new Vector3(
                TARGET_SPAWN_CIRCLE_RADIUS * Mathf.Cos(randomT),
                TARGET_SPAWN_CIRCLE_RADIUS * Mathf.Sin(randomT),
                1.0f
            );
        }
    }

    private void Start()
    {
        LoadingScreen.enabled = false;  // no loading at the beginning

        TextAsset targetLabelsResource = Resources.Load<TextAsset>("TargetLabels");  // loading target labels
        targetLabels = new List<string>(targetLabelsResource.text.ToLower().Split(new[] { "\r\n", "\r" }, System.StringSplitOptions.None));

        TARGET_SPAWN_CIRCLE_RADIUS = CalculateTargetSpawnCircleRadius();  // calculating spawn circle radius based on the display dimensions
        SpawnRandomTarget();

        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();  // initializing voice recognizer
        voiceControllerInterface.StartListening();
    }

    /// <summary>
    /// Calculates a radius for the spawn circle, based on display's dimensions
    /// </summary>
    /// <returns></returns>
    private float CalculateTargetSpawnCircleRadius()
    {
        // world coordinates of display's center point:
        int screenPixelWidth = Camera.main.pixelWidth;
        int screenPixelHeight = Camera.main.pixelHeight;
        Vector3 cameraPointAtWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.5f * screenPixelWidth, 0.5f * screenPixelHeight, 1.0f));
        
        // world coordinates of the outermost, still-visible display point, to the right of the center of the display:
        float smallerScreenDimension = Mathf.Min(screenPixelHeight, screenPixelWidth);
        Vector3 rightmostWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.5f * screenPixelWidth + 0.5f * smallerScreenDimension, 0.5f * screenPixelHeight, 1.0f));

        // returned radius is the distance between the two points:
        return Vector3.Distance(cameraPointAtWorldPoint, rightmostWorldPoint);
    }

    /// <summary>
    /// Parses the voice recognizer's results.
    /// </summary>
    /// <param name="results">Voice recognizer results string</param>
    public void OnVoiceRecognizerResults(string results)
    {
        string lowercase = results.ToLower();

        if (lowercase.Contains("wyjdź") || lowercase.Contains("wyjście") || lowercase.Contains("koniec"))  // exiting the app
        {
            SceneManager.LoadScene("MainMenu");
            LoadingScreen.enabled = true;
        }

        if (AreValidResults(lowercase))  // scoring the target
        {
            Destroy(spawnedTarget);
            SpawnRandomTarget();
        }

        voiceControllerInterface.StartListening();
    }

    /// <summary>
    /// Compares recognized string with currently picked target label variations.
    /// </summary>
    /// <param name="results">Voice recognizer results.</param>
    /// <returns>True when results contains one of the currently picked label variations. False otherwise.</returns>
    private bool AreValidResults(string results)
    {
        if (results.Contains("banan"))  // Safe-word for debug purposes
            return true;
        else
        {
            foreach (var possibleLabel in currentTargetLabelVariants)
            {
                if (results.Contains(possibleLabel))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Skips current target - spawns a fresh one.
    /// </summary>
    public void SkipTarget()
    {
        Destroy(spawnedTarget);
        SpawnRandomTarget();
        voiceControllerInterface.StartListening();
    }

    /// <summary>
    /// Spawns a new target and picks a random label for it.
    /// </summary>
    private void SpawnRandomTarget()
    {
        spawnedTarget = Instantiate(TargetPrefab, transform);
        spawnedTarget.transform.localPosition = NewTargetPosition;
        currentTargetLabelVariants = new List<string>(targetLabels[Random.Range(0, targetLabels.Count - 1)].Split(' '));
        spawnedTarget.GetComponent<TargetBehaviour>().SetDisplayedText(currentTargetLabelVariants[0]);
    }

}