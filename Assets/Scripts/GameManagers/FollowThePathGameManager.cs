using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PathCreation;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VoiceController))]
[RequireComponent(typeof(FollowThePathTargetBehaviour))]
public class FollowThePathGameManager : MonoBehaviour
{

    [SerializeField]
    private Canvas LoadingScreen;

    private FollowThePathTargetBehaviour target;

    private List<PathCreator> paths = new List<PathCreator>();
    int currentPath;

    private TextAsset targetLabelsResource;
    private List<string> targetLabels;
    private List<List<string>> targetLabelsVariants = new List<List<string>>();
    private List<string> currentTargetLabelVariants;

    private VoiceControllerInterface voiceControllerInterface;

    private bool waitingToShowNewLabel;
    private float newLabelTimer;
    private readonly float NEW_LABEL_DELAY = 1.5f;

    private int points;
    private readonly int MAX_POINTS = 3;

    private void Start()
    {
        LoadingScreen.enabled = false;

        PrepareTargetLabels();

        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();

        GetComponentsInChildren(paths);
        currentPath = 0;

        target = GetComponentInChildren<FollowThePathTargetBehaviour>();
        target.SetPathCreator(paths[currentPath]);
        target.StartRunning();

        waitingToShowNewLabel = true;
    }

    private void Update()
    {
        newLabelTimer += Time.deltaTime;
        if (waitingToShowNewLabel && newLabelTimer > NEW_LABEL_DELAY)
        {
            currentTargetLabelVariants = targetLabelsVariants[UnityEngine.Random.Range(0, targetLabelsVariants.Count - 1)];
            target.SetDisplayedText(currentTargetLabelVariants[0]);
            waitingToShowNewLabel = false;
            voiceControllerInterface.StartListening();
        }
        if (points >= MAX_POINTS)
        {
            points = 0;
            currentPath++;
            if (currentPath > paths.Count - 1)
                currentPath = 0;
            target.SetPathCreator(paths[currentPath]);
        }
    }

    public void OnVoiceRecognizerResults(string results)
    {
        string lowercase = results.ToLower();
        if (lowercase.Contains("wyjdź") || lowercase.Contains("wyjście") || lowercase.Contains("koniec"))
        {
            LoadingScreen.enabled = true;
            SceneManager.LoadScene("MainMenu");
        }
        if (lowercase.Contains("szybciej"))
            target.ChangeSpeed(0.1f);
        if (lowercase.Contains("wolniej"))
            target.ChangeSpeed(-0.1f);

        if (AreValidResults(lowercase))
        {
            ScoreTarget();
        }
        else
        {
            voiceControllerInterface.StartListening();
        }
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

    private void PrepareTargetLabels()
    {
        targetLabelsResource = Resources.Load<TextAsset>("TargetLabels");
        targetLabels = new List<string>(targetLabelsResource.text.ToLower().Split(new[] { "\r\n", "\r" }, System.StringSplitOptions.None));
        foreach (var labelList in targetLabels)
        {
            targetLabelsVariants.Add(new List<string>(labelList.Split(new[] { " " }, System.StringSplitOptions.None)));
        }
    }

    private void SkipTarget()
    {
        ScoreTarget();
    }

    private void ScoreTarget()
    {
        target.SetDisplayedText("");
        points++;
        waitingToShowNewLabel = true;
        newLabelTimer = 0;
    }

}