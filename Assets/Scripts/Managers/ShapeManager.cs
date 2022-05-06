using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeManager : Singleton<ShapeManager>
{
    // Start is called before the first frame update
   
    private List<float>[] shapes = new List<float>[3];

    void Start()
    {
        for(int i = 0; i <3; i++) {
            shapes[i] = new List<float>();
        }
        
    }
    public void UpdateShapes(List<float> proximities, int faceIndex) {
        
        float delta = shapes[faceIndex].Count - proximities.Count;
        shapes[faceIndex].Sort((a,b) => b.CompareTo(a));
        proximities.Sort((a,b) => b.CompareTo(a));

        if(delta < 0f) {
            for(int i = 0; i <Mathf.Abs(delta); i++) {
                AddShape(faceIndex);
            }
        }
        else if(delta >0f) {
            for(int i = 0; i <delta; i++) {
                RemoveShape(faceIndex);
            }
        }

    }

    void AddShape(int faceIndex) {
        shapes[faceIndex].Add(0f);
    }
    void RemoveShape(int faceIndex) {
        shapes[faceIndex].RemoveAt(shapes[faceIndex].Count - 1);
    }
    void UpdateIntensities(List<float> proximities, int faceIndex) {
        for(int i = 0; i < shapes[faceIndex].Count; i ++) {
            shapes[faceIndex][i] = proximities[i];
        }
    }
}
