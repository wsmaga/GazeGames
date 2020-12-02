using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemBehaviour : MonoBehaviour
{

    [SerializeField]
    private string Keyword;

    private int direction = 1;
    private float MOVE_SPEED = 0.1f;
    private const float Z_LIMIT = 0.1f;

    private void OnMouseDown()
    {
        SendMessageUpwards("OnVoiceRecognizerResults", Keyword);
    }

    private void Start()
    {
        MOVE_SPEED = Random.Range(0.05f, 0.1f);
        Vector3 newPos = transform.localPosition;
        newPos.z = Random.Range(-Z_LIMIT, Z_LIMIT);
        transform.localPosition = newPos;
    }

    private void Update()
    {
        Vector3 newPos = transform.localPosition;
        newPos.z += direction * MOVE_SPEED * Time.deltaTime;
        if (newPos.z > Z_LIMIT)
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