using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;

public class ShapeManager : Singleton<ShapeManager>
{
  // Start is called before the first frame update
  public GameObject[] faces;
  public GameObject[] shapePrefabs;
  public Material[] materials;
  private List<Shape>[] shapes = new List<Shape>[3];
  private List<Shape> deletedShapes = new List<Shape>();
  private Vector3[][] bounds = new Vector3[3][];
  private string[] layers = new string[]{
    "Face A",
    "Face B",
    "Face C"
  };
  public VisualEffect[] particles; 


  void Start()
  {
    for (int i = 0; i < 3; i++)
    {
      shapes[i] = new List<Shape>();
      // get each faces
      bounds[i] = new Vector3[4];
      // get each bounds for each faces
      for (int j = 0; j < 4; j++)
      {
        bounds[i][j] = faces[i].transform.GetChild(j).localPosition;
      }
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
  Vector3 GetShapePosition(Vector3[] faceBounds)
  {
    float margin = 0.002f;
    // GET X VALUE
    // Get random x between minX and maxY range
    float minX = faceBounds[2].x - margin;
    float maxX = faceBounds[3].x + margin;
    float x = Random.Range(minX, maxX);

    // GET Z VALUE
    // Define minY and maxY values
    float minZ = faceBounds[2].z + margin;
    float maxZ = faceBounds[0].z - margin;

    // Get line equations
    // Get slopes
    float leftLineSlope = (faceBounds[0].z - faceBounds[2].z) / (faceBounds[0].x - faceBounds[2].x);
    float rightLineSlope = (faceBounds[1].z - faceBounds[3].z) / (faceBounds[1].x - faceBounds[3].x);
    // Get intercepts (ordonnée à l'origine)
    float leftLineIntercept = faceBounds[0].z - leftLineSlope * faceBounds[0].x;
    float rightLineIntercept = faceBounds[1].z - leftLineSlope * faceBounds[1].x;

    // Project x on both line (with y=mx+b)
    float leftProjection = leftLineSlope * x + leftLineIntercept;
    float rightProjection = rightLineSlope * x + rightLineIntercept;
    // Keep min value to determin Y value, then clamp between min and max
    float maxZShape = Mathf.Clamp(Mathf.Min(leftProjection, rightProjection), minZ, maxZ);
    float z = Random.Range(minZ, maxZShape);

    // GET Y VALUE
    // Get line equation
    // Get slope
    float depthLineSlope = (faceBounds[0].y - faceBounds[2].y) / (faceBounds[0].z - faceBounds[2].z);
    // Get intercept (ordonnée à l'origine)
    float depthLineIntercept = faceBounds[0].y - depthLineSlope * faceBounds[0].z;

    float y = (depthLineSlope * z + depthLineIntercept) + 0.001f;

    return new Vector3(x, y, z);
  }

  void AddShape(int faceIndex)
  {
    GameObject face = faces[faceIndex];
    GameObject prefab = shapePrefabs[Random.Range(0, shapePrefabs.Length)];
    GameObject newShape = Instantiate(prefab, face.transform);

    newShape.transform.localPosition = GetShapePosition(bounds[faceIndex]);
    newShape.transform.localScale = new Vector3(0f,0f,0f);
    int materialIndex= Random.Range(0, materials.Length);
    newShape.transform.GetChild(0).GetComponent<Renderer>().material = materials[materialIndex];
    newShape.transform.GetChild(0).GetComponent<Renderer>().material.SetInt("_Stripes", Random.value >0.5 ? 1 : 0);
    newShape.transform.GetChild(0).Rotate(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f), Space.Self);
    newShape.layer = LayerMask.NameToLayer(layers[faceIndex]);
    newShape.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer(layers[faceIndex]);
    Shape shape = new Shape(0f, 0f, newShape, materialIndex == 0, newShape.transform.localPosition, Random.Range(0.9f, 1.1f) * prefab.transform.localScale);
    shapes[faceIndex].Add(shape);

    AudioManager.Instance.PlayEvent(faceIndex, 0);
    StartCoroutine(ToggleAttractor(faceIndex));
  }
  IEnumerator ToggleAttractor(int faceIndex) {
    particles[faceIndex].SetBool("Attractor", true);
    yield return new WaitForSeconds(0.5f);
    particles[faceIndex].SetBool("Attractor", false);
  }
  void RemoveShape(int faceIndex)
  {
    int lastIndex = shapes[faceIndex].Count - 1;

    AudioManager.Instance.PlayEvent(faceIndex, 1);

    StartCoroutine(RemoveShapeCoroutine(faceIndex,lastIndex));

    
  }
  IEnumerator RemoveShapeCoroutine(int faceIndex, int lastIndex) {
    deletedShapes.Add(shapes[faceIndex][lastIndex]);
    int deletedShapeIndex = deletedShapes.Count-1;
    deletedShapes[deletedShapes.Count-1].targetScaleFactor = 0f;
    shapes[faceIndex].RemoveAt(lastIndex);
    yield return new WaitForSeconds(2);
    if(deletedShapes.ElementAtOrDefault(deletedShapeIndex) != null) {
      Destroy(deletedShapes[deletedShapeIndex].shapeMesh);
      deletedShapes.RemoveAt(deletedShapeIndex);
    }
    
  }
  void UpdateIntensities(List<float> proximities, int faceIndex)
  {
    for (int i = 0; i < shapes[faceIndex].Count; i++)
    {
        shapes[faceIndex][i].targetIntensity = proximities[i];
    }
  }
  void AnimateShape(Shape shape) {
    shape.scaleFactor = Mathf.Lerp(shape.scaleFactor, shape.targetScaleFactor, 0.35f);
    shape.shapeMesh.transform.localScale = shape.realScale * shape.scaleFactor; 
    shape.intensity = Mathf.Lerp(shape.intensity, shape.targetIntensity, 0.1f);

    if(shape.glass) {
      shape.time += 1f * (0.5f + shape.intensity);
      shape.shapeMesh.transform.localPosition = new Vector3(
        shape.startPos.x + Mathf.Sin(shape.time * 2f) * 0.0001f,
        shape.startPos.y,
        shape.startPos.z
      );
    }

    else {
      shape.shapeMesh.transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Proximity", shape.intensity);
    }
  }
  void Update()
  {
    for (int i = 0; i < 3; i++)
    {
      List<Shape> current = shapes[i];
      for (int j = 0; j < current.Count; j++)
      {
        AnimateShape(current[j]);
      }
    }
    for (int i = 0; i < deletedShapes.Count; i++)
      {
        AnimateShape(deletedShapes[i]);
      }
  }
}
