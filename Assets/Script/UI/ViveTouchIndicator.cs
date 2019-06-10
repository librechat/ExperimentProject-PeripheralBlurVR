using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveTouchIndicator : MonoBehaviour
{
    [SerializeField]
    RectTransform indicator;
    [SerializeField]
    float scale = 2.0f;

    private void Update()
    {
        Vector2 axis = InputManager.GetMoveAxis();
        indicator.anchoredPosition = axis * scale;
    }
}
