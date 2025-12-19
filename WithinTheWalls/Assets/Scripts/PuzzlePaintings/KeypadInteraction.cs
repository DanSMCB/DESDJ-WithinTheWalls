using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadInteraction   : MonoBehaviour
{
    private Camera cam;
    private void Awake() => cam = GetComponent<Camera>();
    private void Update()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
                {
                    keypadButton.PressButton();
                }
            }
        }
    }
}