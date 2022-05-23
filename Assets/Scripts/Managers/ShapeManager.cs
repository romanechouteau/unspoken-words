using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeManager : Singleton<ShapeManager>
{
  // Start is called before the first frame update
  public GameObject shapePrefab;
  public GameObject faceA;
  public GameObject faceB;
  public GameObject faceC;
  private List<Shape>[] shapes = new List<Shape>[3];
  private GameObject[] faces = new GameObject[3];

  void Start()
  {
    for (int i = 0; i < 3; i++)
    {
      shapes[i] = new List<Shape>();
      faces[i] = i == 0 ? faceA : i == 1 ? faceB : faceC;
    }

  }
  public void UpdateShapes(List<float> proximities, int faceIndex)
  {

    float delta = shapes[faceIndex].Count - proximities.Count;
    shapes[faceIndex].Sort((a, b) => b.targetIntensity.CompareTo(a.targetIntensity));
    proximities.Sort((a, b) => b.CompareTo(a));

    if (delta < 0f)
    {
      for (int i = 0; i < Mathf.Abs(delta); i++)
      {
        AddShape(faceIndex);
      }
    }
    else if (delta > 0f)
    {
      for (int i = 0; i < delta; i++)
      {
        RemoveShape(faceIndex);
      }
    }
    UpdateIntensities(proximities, faceIndex);

  }

  void AddShape(int faceIndex)
  {
    GameObject face = faces[faceIndex];
    GameObject newShape = Instantiate(shapePrefab, face.transform);
    Shape shape = new Shape(0f, 0f, newShape);
    shapes[faceIndex].Add(shape);
  }
  void RemoveShape(int faceIndex)
  {
    int lastIndex = shapes[faceIndex].Count - 1;
    Destroy(shapes[faceIndex][lastIndex].shapeMesh);
    shapes[faceIndex].RemoveAt(lastIndex);
  }
  void UpdateIntensities(List<float> proximities, int faceIndex)
  {
    for (int i = 0; i < shapes[faceIndex].Count; i++)
    {
      shapes[faceIndex][i].targetIntensity = proximities[i];
    }
  }
  void Update()
  {
    for (int i = 0; i < 3; i++)
    {
      List<Shape> current = shapes[i];
      for (int j = 0; j < current.Count; j++)
      {
        current[j].intensity = Mathf.Lerp(current[j].intensity, current[j].targetIntensity, 0.1f);
        current[j].shapeMesh.transform.localScale = new Vector3((1 + current[j].intensity) * 0.001f, (1 + current[j].intensity) * 0.001f, (1 + current[j].intensity) * 0.001f);
      }
    }
  }
}
