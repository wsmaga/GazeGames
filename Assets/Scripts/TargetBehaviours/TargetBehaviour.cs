using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetBehaviour : MonoBehaviour
{

    public Text DisplayedText;
    private const float MAX_POSITION_DELTA = 0.05f;
    private float minDistanceFromCenter;

    private Vector3 LocalMovementTarget
    {
        get
        {
            return new Vector3(0, 0, 1.0f);
        }
    }

    private void Start()
    {
        minDistanceFromCenter = CalculateMinDistanceFromCenter();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.localPosition, LocalMovementTarget) > minDistanceFromCenter)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, LocalMovementTarget, Time.deltaTime * MAX_POSITION_DELTA);
        }
        transform.LookAt(2 * transform.position - new Vector3(0, 0, 0), Camera.main.transform.up);
    }

    private void OnMouseDown()
    {
        SendMessageUpwards("SkipTarget");
    }

    public void SetDisplayedText(string displayedText)
    {
        DisplayedText.text = displayedText;
    }

    private float CalculateMinDistanceFromCenter()
    {
        int screenPixelWidth = Camera.main.pixelWidth;
        int screenPixelHeight = Camera.main.pixelHeight;
        Vector3 cameraPointAtWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.5f * screenPixelWidth, 0.5f * screenPixelHeight, 1.0f));
        float smallerScreenDimension = Mathf.Min(screenPixelHeight, screenPixelWidth);
        Vector3 rightmostWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.5f * screenPixelWidth + 0.5f * smallerScreenDimension, 0.5f * screenPixelHeight, 1.0f));
        return 0.9f * Vector3.Distance(cameraPointAtWorldPoint, rightmostWorldPoint);
    }

}
