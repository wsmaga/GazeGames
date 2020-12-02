using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VoiceController))]
public class SayTargetGameManager : MonoBehaviour
{

    [SerializeField]
    private Canvas LoadingScreen;

    public GameObject TargetPrefab;

    private float TARGET_SPAWN_CIRCLE_RADIUS;

    private TextAsset targetLabelsResource;
    private List<string> targetLabels;
    private List<string> currentTargetLabelVariants;

    private VoiceControllerInterface voiceControllerInterface;
    private GameObject spawnedTarget;

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
        LoadingScreen.enabled = false;

        targetLabelsResource = Resources.Load<TextAsset>("TargetLabels");
        targetLabels = new List<string>(targetLabelsResource.text.ToLower().Split(new[] { "\r\n", "\r" }, System.StringSplitOptions.None));

        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();

        TARGET_SPAWN_CIRCLE_RADIUS = CalculateTargetSpawnCircleRadius();

        SpawnRandomTarget();
        voiceControllerInterface.StartListening();
    }

    private float CalculateTargetSpawnCircleRadius()
    {
        int screenPixelWidth = Camera.main.pixelWidth;
        int screenPixelHeight = Camera.main.pixelHeight;
        Vector3 cameraPointAtWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.5f * screenPixelWidth, 0.5f * screenPixelHeight, 1.0f));
        float smallerScreenDimension = Mathf.Min(screenPixelHeight, screenPixelWidth);
        Vector3 rightmostWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.5f * screenPixelWidth + 0.5f * smallerScreenDimension, 0.5f * screenPixelHeight, 1.0f));
        return Vector3.Distance(cameraPointAtWorldPoint, rightmostWorldPoint);
    }

    public void OnVoiceRecognizerResults(string results)
    {
        string lowercase = results.ToLower();
        if (lowercase.Contains("wyjdź") || lowercase.Contains("wyjście") || lowercase.Contains("koniec"))
        {
            SceneManager.LoadScene("MainMenu");
            LoadingScreen.enabled = true;
        }

        if (AreValidResults(lowercase))
        {
            Destroy(spawnedTarget);
            SpawnRandomTarget();
        }
        voiceControllerInterface.StartListening();
    }

    private bool AreValidResults(string results)
    {
        if (results.Contains("banan"))
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

    public void SkipTarget()
    {
        Destroy(spawnedTarget);
        SpawnRandomTarget();
        voiceControllerInterface.StartListening();
    }

    private void SpawnRandomTarget()
    {
        spawnedTarget = Instantiate(TargetPrefab, this.transform);
        spawnedTarget.transform.localPosition = NewTargetPosition;
        currentTargetLabelVariants = new List<string>(targetLabels[Random.Range(0, targetLabels.Count - 1)].Split(' '));
        spawnedTarget.GetComponent<TargetBehaviour>().SetDisplayedText(currentTargetLabelVariants[0]);
    }

}