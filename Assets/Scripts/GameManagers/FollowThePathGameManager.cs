using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.SceneManagement;

/// <summary>
/// Game manager for the Follow the Path minigame
/// </summary>
[RequireComponent(typeof(VoiceController))]
[RequireComponent(typeof(FollowThePathTargetBehaviour))]
public class FollowThePathGameManager : MonoBehaviour
{
    /// <summary>
    /// Canvas with loading screen to be displayed when loading something
    /// </summary>
    [SerializeField]
    private Canvas LoadingScreen;

    /// <summary>
    /// A target to be driven along the path
    /// </summary>
    private FollowThePathTargetBehaviour target;

    /// <summary>
    /// Available paths for the target
    /// </summary>
    private List<PathCreator> paths = new List<PathCreator>();
    /// <summary>
    /// Current path index
    /// </summary>
    private int currentPath;

    /// <summary>
    /// List of target labels' variants. Each inner list is a single target label with all of it's variants
    /// </summary>
    private List<List<string>> targetLabelsVariants = new List<List<string>>();
    /// <summary>
    /// Currently picked list of target label's variants
    /// </summary>
    private List<string> currentTargetLabelVariants;

    /// <summary>
    /// Interface for voice controller service
    /// </summary>
    private VoiceControllerInterface voiceControllerInterface;

    /// <summary>
    /// When true - the game is waiting before picking another label for a target
    /// </summary>
    private bool waitingToShowNewLabel;
    /// <summary>
    /// Current progress towards picking a new label for the target
    /// </summary>
    private float newLabelTimer;
    /// <summary>
    /// How much time before picking a new label for the target
    /// </summary>
    private readonly float NEW_LABEL_DELAY = 1.5f;

    /// <summary>
    /// Number of targets scored for the current path
    /// </summary>
    private int points;
    /// <summary>
    /// How many targets before changing the path
    /// </summary>
    private readonly int MAX_POINTS = 3;

    private void Start()
    {
        LoadingScreen.enabled = false;  // no loading screen visible at the start

        PrepareTargetLabels();   // load the labels from resources

        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();  // initialize voice controller interface

        GetComponentsInChildren(paths);  // load all the paths
        currentPath = 0;

        target = GetComponentInChildren<FollowThePathTargetBehaviour>();  // initialize the target
        target.SetPathCreator(paths[currentPath]);
        target.StartRunning();

        waitingToShowNewLabel = true;
    }

    private void Update()
    {
        newLabelTimer += Time.deltaTime;
        if (waitingToShowNewLabel && newLabelTimer > NEW_LABEL_DELAY)  // is it time to pick a new target label?
        {
            currentTargetLabelVariants = targetLabelsVariants[UnityEngine.Random.Range(0, targetLabelsVariants.Count - 1)];
            target.SetDisplayedText(currentTargetLabelVariants[0]);
            waitingToShowNewLabel = false;
            voiceControllerInterface.StartListening();
        }
        if (points >= MAX_POINTS)  // is it time to change the path?
        {
            points = 0;
            currentPath++;
            if (currentPath > paths.Count - 1)
                currentPath = 0;
            target.SetPathCreator(paths[currentPath]);
        }
    }

    /// <summary>
    /// Parses the voice recognizer output
    /// </summary>
    /// <param name="results"> Recognized string </param>
    public void OnVoiceRecognizerResults(string results)
    {
        string lowercase = results.ToLower();
        if (lowercase.Contains("wyjdź") || lowercase.Contains("wyjście") || lowercase.Contains("koniec"))  // exiting the minigame
        {
            LoadingScreen.enabled = true;
            SceneManager.LoadScene("MainMenu");
        }
        if (lowercase.Contains("szybciej"))  // speed up or slow down the target's movement
            target.ChangeSpeed(0.1f);
        if (lowercase.Contains("wolniej"))
            target.ChangeSpeed(-0.1f);

        if (AreValidResults(lowercase))  // scoring the target
        {
            ScoreTarget();
        }
        else
        {
            voiceControllerInterface.StartListening();  // restart the recognition after failure
        }
    }

    /// <summary>
    /// Returns true if the results contain the proper target's label
    /// </summary>
    /// <param name="results"> Result string to be checked </param>
    /// <returns></returns>
    private bool AreValidResults(string results)
    {
        if (results.Contains("banan"))  // a debug admin-word for skipping
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
    /// Loading and parsing targets' labels' resource
    /// </summary>
    private void PrepareTargetLabels()
    {
        TextAsset targetLabelsResource = Resources.Load<TextAsset>("TargetLabels");
        List<string> targetLabels = new List<string>(targetLabelsResource.text.ToLower().Split(new[] { "\r\n", "\r" }, System.StringSplitOptions.None));
        foreach (var labelList in targetLabels)
        {
            targetLabelsVariants.Add(new List<string>(labelList.Split(new[] { " " }, System.StringSplitOptions.None)));
        }
    }

    /// <summary>
    /// Used to externally order to skip current target
    /// </summary>
    private void SkipTarget()
    {
        ScoreTarget();
    }

    /// <summary>
    /// Score current target
    /// </summary>
    private void ScoreTarget()
    {
        target.SetDisplayedText("");
        points++;
        waitingToShowNewLabel = true;
        newLabelTimer = 0;
    }

}