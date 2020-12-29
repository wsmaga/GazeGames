using UnityEngine;
using UnityEngine.UI;
using PathCreation;

/// <summary>
/// Behaviour of the "Follow the path" game's target object.
/// </summary>
[RequireComponent(typeof(Text))]
public class FollowThePathTargetBehaviour : MonoBehaviour
{

    /// <summary>
    /// Current label.
    /// </summary>
    private Text displayedText;
    /// <summary>
    /// Movement speed.
    /// </summary>
    private float speed = 0.4f;
    /// <summary>
    /// Object to access movement path checkpoints.
    /// </summary>
    private PathCreator pathCreator;
    /// <summary>
    /// True when the target is in motion.
    /// </summary>
    private bool isRunning = false;
    /// <summary>
    /// Current distance along the movement path.
    /// </summary>
    private float distance = 0.0f;

    private void Start()
    {
        displayedText = GetComponentInChildren<Text>();  // storing reference to own text children with target label
    }

    private void Update()
    {
        if (isRunning && pathCreator)  // updating target's position along the current movement path
        {
            distance += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distance);
        }
    }

    private void OnMouseDown()
    {
        SendMessageUpwards("SkipTarget");  // clicking is a debug way to skip the target in the editor mode
    }

    /// <summary>
    /// Sets the current target label.
    /// </summary>
    /// <param name="displayedText">String to be displayed as target label.</param>
    public void SetDisplayedText(string displayedText)
    {
        this.displayedText.text = displayedText;
    }

    /// <summary>
    /// Set the current movement path access object.
    /// </summary>
    /// <param name="pathCreator">New object to access a movement path.</param>
    public void SetPathCreator(PathCreator pathCreator)
    {
        this.pathCreator = pathCreator;
        distance = 0;
    }

    /// <summary>
    /// Continues movement along the movement path.
    /// </summary>
    public void StartRunning()
    {
        isRunning = true;
    }

    /// <summary>
    /// Stops movement along the movement path.
    /// </summary>
    public void StopRunning()
    {
        isRunning = false;
    }

    /// <summary>
    /// Sets new movement speed.
    /// </summary>
    /// <param name="increment">New speed value.</param>
    public void ChangeSpeed(float increment)
    {
        speed += increment;
    }

}
