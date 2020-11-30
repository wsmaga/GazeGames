using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSphereBehaviour : MonoBehaviour
{

    private void Start()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 5));
    }

}
