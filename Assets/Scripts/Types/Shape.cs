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
  public float scaleFactor;
  public float targetScaleFactor;
  public Vector3 realScale;

  public Shape(float initialIntensity, float initialTargetIntensity, GameObject shape, bool isGlass, Vector3 pos, Vector3 rScale) {
    intensity = initialIntensity;
    targetIntensity = initialTargetIntensity;
    shapeMesh = shape;
    glass = isGlass;
    startPos = pos;
    time = 0f;
    scaleFactor=0f;
    targetScaleFactor=1f;
    realScale=rScale;
  }
}