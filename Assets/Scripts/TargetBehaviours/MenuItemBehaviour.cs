using UnityEngine;

/// <summary>
/// Behaviour of a menu item object.
/// </summary>
public class MenuItemBehaviour : MonoBehaviour
{

    /// <summary>
    /// Keyword/number associated with this item.
    /// </summary>
    [SerializeField]
    private string Keyword;

    /// <summary>
    /// Current direction of floating.
    /// </summary>
    private int direction = 1;
    /// <summary>
    /// Floating speed.
    /// </summary>
    private float MOVE_SPEED = 0.3f;
    /// <summary>
    /// How far the item floats back and forth in world space.
    /// </summary>
    private const float Z_LIMIT = 0.04f;

    private void OnMouseDown()
    {
        SendMessageUpwards("OnVoiceRecognizerResults", Keyword);  // clicking the menu item in editor mode results in registering this item
    }

    private void Start()
    {
        MOVE_SPEED = Random.Range(0.05f, 0.1f);  // randomize the position and speed of float
        Vector3 newPos = transform.localPosition;
        newPos.z = Random.Range(-Z_LIMIT, Z_LIMIT);
        transform.localPosition = newPos;
    }

    private void Update()
    {
        Vector3 newPos = transform.localPosition;  // update the item's position
        newPos.z += direction * MOVE_SPEED * Time.deltaTime;
        if (newPos.z > Z_LIMIT)  // reversing the float direction when limit reached
        {
            newPos.z = Z_LIMIT;
            direction *= -1;
            MOVE_SPEED = Random.Range(0.05f, 0.1f);
        }
        else if (newPos.z < -Z_LIMIT)
        {
            newPos.z = -Z_LIMIT;
            direction *= -1;
            MOVE_SPEED = Random.Range(0.05f, 0.1f);
        }
        transform.localPosition = newPos;
    }

}