using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class TestCubeClickHandler : MonoBehaviour
{

    public UnityEvent ClickEvent;

    private void OnMouseUpAsButton()
    {
        ClickEvent.Invoke();
    }

}
