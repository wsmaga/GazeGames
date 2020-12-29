using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Target's object behaviour for "Gazuma" game.
/// </summary>
public class GazumaBall : MonoBehaviour
{

    /// <summary>
    /// Target's own renderer.
    /// </summary>
    [SerializeField]
    private Renderer renderer;

    /// <summary>
    /// Target's own light source.
    /// </summary>
    [SerializeField]
    private Light light;

    /// <summary>
    /// Target's text children - target's label.
    /// </summary>
    [SerializeField]
    private Text label;

    /// <summary>
    /// Current material and light color.
    /// </summary>
    private Color color;
    /// <summary>
    /// Current number displayed as label.
    /// </summary>
    private int number;

    /// <summary>
    /// All possible material and light's colors.
    /// </summary>
    private Color[] colors =
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.magenta,
        Color.yellow,
        Color.white
    };

    private void Start()
    {
        SetNumber(Random.Range(1, colors.Length + 1));  // setting new random number and color palette
        SetProperties(Random.Range(0, colors.Length));
    }

    private void OnMouseDown()
    {
        SendMessageUpwards("OnVoiceRecognizerResults", number.ToString());  // clicking on target is a debug mean to score the target in editor mode
    }

    /// <summary>
    /// Sets properties of target's material and light source.
    /// </summary>
    /// <param name="number">An integer value used to determine the new colors.</param>
    public void SetProperties(int number)
    {
        color = colors[number];
        renderer.material.color = color;
        renderer.material.SetColor("_EmissionColor", color);
        light.color = color;
    }

    /// <summary>
    /// Assign a new number used as a target's label.
    /// </summary>
    /// <param name="number">New value to assign.</param>
    public void SetNumber(int number)
    {
        this.number = number;
        label.text = number.ToString();
    }

    /// <summary>
    /// Get target's current number.
    /// </summary>
    /// <returns>Value of target's currently assigned number.</returns>
    public int GetNumber() { return number; }

    /// <summary>
    /// Get target's current color.
    /// </summary>
    /// <returns>Currently assigned Color object.</returns>
    public Color GetColor() { return color; }

}
