using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
  public float InputAxisX { get; private set; }
  public float InputAxisY { get; private set; }
  public Vector2 InputAxisRaw => new Vector2(InputAxisX, InputAxisY);

  public Vector2 MouseWorldPosition { get; private set; }

  public bool GetShift { get; private set; }

  public void UpdateInput()
  {
    InputAxisX = Input.GetAxis("Horizontal");
    InputAxisY = Input.GetAxis("Vertical");

    MouseWorldPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

    GetShift = Input.GetKey(KeyCode.LeftShift);
  }
}
