using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Behaviour of target object from "Say the target" game.
/// </summary>
public class TargetBehaviour : MonoBehaviour
{

    /// <summary>
    /// Displayed target's label.
    /// </summary>
    public Text DisplayedText;
    /// <summary>
    /// Max position change per frame.
    /// </summary>
    private const float MAX_POSITION_DELTA = 0.05f;
    /// <summary>
    /// Min distance from center that the target can achieve.
    /// </summary>
    private float minDistanceFromCenter;

    /// <summary>
    /// Point that the target moves to.
    /// </summary>
    private Vector3 LocalMovementTarget
    {
        get
        {
            return new Vector3(0, 0, 1.0f);
        }
    }

    private void Start()
    {
        minDistanceFromCenter = CalculateMinDistanceFromCenter();  // calculating min distance value based on display dimensions
    }

    private void Update()
    {
        if (Vector3.Distance(transform.localPosition, LocalMovementTarget) > minDistanceFromCenter)  // moving the target towards the goal while the distance remains big enough
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, LocalMovementTarget, Time.deltaTime * MAX_POSITION_DELTA);
        }
        transform.LookAt(2 * transform.position - new Vector3(0, 0, 0), Camera.main.transform.up);  // looking at the camera
    }

    private void OnMouseDown()
    {
        SendMessageUpwards("SkipTarget");  // clicking the target in editor mode results in skipping the target by the game manager
    }

    /// <summary>
    /// Sets currently displayed label.
    /// </summary>
    /// <param name="displayedText">New string to be displayed as label.</param>
    public void SetDisplayedText(string displayedText)
    {
        DisplayedText.text = displayedText;
    }

    /// <summary>
    /// Calculates the min distance from the center that the target can achieve during movement, based on display's dimensions.
    /// </summary>
    /// <returns>Min distance in world space.</returns>
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
