using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class Shape {
  public float intensity;
  public GameObject shapeMesh;
  public float targetIntensity;

  public Shape(float initialIntensity, float initialTargetIntensity, GameObject shape) {
    intensity = initialIntensity;
    targetIntensity = initialTargetIntensity;
    shapeMesh = shape;
  }
}