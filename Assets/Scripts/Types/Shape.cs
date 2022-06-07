using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class Shape {
  public float intensity;
  public GameObject shapeMesh;
  public float targetIntensity;
  public bool glass;
  public Vector3 startPos;
  public float time;

  public Shape(float initialIntensity, float initialTargetIntensity, GameObject shape, bool isGlass, Vector3 pos) {
    intensity = initialIntensity;
    targetIntensity = initialTargetIntensity;
    shapeMesh = shape;
    glass = isGlass;
    startPos = pos;
    time = 0f;
  }
}