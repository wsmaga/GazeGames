using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PathCreation;

[RequireComponent(typeof(Text))]
public class FollowThePathTargetBehaviour : MonoBehaviour
{

    private Text displayedText;
    private float speed = 0.4f;
    private PathCreator pathCreator;
    private bool isRunning = false;
    private float distance = 0.0f;

    private void Start()
    {
        displayedText = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (isRunning && pathCreator)
        {
            distance += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distance);
        }
    }

    private void OnMouseDown()
    {
        SendMessageUpwards("SkipTarget");
    }

    public void SetDisplayedText(string displayedText)
    {
        this.displayedText.text = displayedText;
    }

    public void SetPathCreator(PathCreator pathCreator)
    {
        this.pathCreator = pathCreator;
        distance = 0;
    }

    public void StartRunning()
    {
        isRunning = true;
    }

    public void StopRunning()
    {
        isRunning = false;
    }

    public void ChangeSpeed(float increment)
    {
        speed += increment;
    }

}
