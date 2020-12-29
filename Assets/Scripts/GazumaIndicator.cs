using UnityEngine;

/// <summary>
/// Behaviour for indicator object from "Gazuma" game.
/// </summary>
public class GazumaIndicator : MonoBehaviour
{

    /// <summary>
    /// Object's own renderer.
    /// </summary>
    [SerializeField]
    private Renderer renderer;

    /// <summary>
    /// Rotation speed for Z axis.
    /// </summary>
    private const float Z_ROTATION_SPEED = 10.0f;
    /// <summary>
    /// Rotation speed for Y axis.
    /// </summary>
    private const float Y_ROTATION_SPEED = 5.0f;

    /// <summary>
    /// How many seconds passed since the beginning of object's existence.
    /// </summary>
    private float timer = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;
        Quaternion rotation = new Quaternion();  // updating rotation
        rotation.eulerAngles = new Vector3(
            0,
            Y_ROTATION_SPEED * timer,
            Z_ROTATION_SPEED * timer
        );
        transform.rotation = rotation;
    }

    private void OnMouseDown()
    {
        SendMessageUpwards("OnVoiceRecognizerResults", "skip");  // clicking the indicator in editor mode results in changing it's color by the game manager
    }

    /// <summary>
    /// Sets new color for the object's material and light source.
    /// </summary>
    /// <param name="color">New Color object,</param>
    public void SetColor(Color color)
    {
        renderer.material.color = color;
        renderer.material.SetColor("_EmissionColor", color);
    }

}
