using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazumaBall : MonoBehaviour
{

    [SerializeField]
    private Renderer renderer;

    [SerializeField]
    private Light light;

    [SerializeField]
    private Text label;

    private Color color;
    private int number;

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
        SetNumber(Random.Range(1, colors.Length + 1));
        SetProperties(Random.Range(0, colors.Length));
    }

    private void OnMouseDown()
    {
        SendMessageUpwards("OnVoiceRecognizerResults", number.ToString());
    }

    public void SetProperties(int number)
    {
        color = colors[number];
        renderer.material.color = color;
        renderer.material.SetColor("_EmissionColor", color);
        light.color = color;
    }

    public void SetNumber(int number)
    {
        this.number = number;
        label.text = number.ToString();
    }

    public int GetNumber() { return number; }

    public Color GetColor() { return color; }

}
