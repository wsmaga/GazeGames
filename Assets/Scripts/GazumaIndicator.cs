using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazumaIndicator : MonoBehaviour
{

    [SerializeField]
    private Renderer renderer;

    private const float Z_ROTATION_SPEED = 10.0f;
    private const float Y_ROTATION_SPEED = 5.0f;

    private float timer = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(
            0,
            Y_ROTATION_SPEED * timer,
            Z_ROTATION_SPEED * timer
        );
        transform.rotation = rotation;
    }

    private void OnMouseDown()
    {
        SendMessageUpwards("OnVoiceRecognizerResults", "skip");
    }

    public void SetColor(Color color)
    {
        renderer.material.color = color;
        renderer.material.SetColor("_EmissionColor", color);
    }

}
