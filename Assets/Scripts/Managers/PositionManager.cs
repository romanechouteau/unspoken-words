using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionManager : Singleton<PositionManager>
{
    private Vector2 center = new Vector2(0f, 0f);
    private Vector2 abscissa = new Vector2(1f, 0f);
    private Vector2 ordinate = new Vector2(0f, 1f);

    private List<float> faceAUsers = new List<float>();
    private List<float> faceBUsers = new List<float>();
    private List<float> faceCUsers = new List<float>();

    public Material faceA;
    public Material faceB;
    public Material faceC;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetPositions(List<Vector2> positions) {
        faceAUsers.Clear();
        faceBUsers.Clear();
        faceCUsers.Clear();

        for(int i=0; i<positions.Count; i++) {
            SortUsers(positions[i]);
        }
        ShapeManager.Instance.UpdateShapes(faceAUsers, 0);
        ShapeManager.Instance.UpdateShapes(faceBUsers, 1);
        ShapeManager.Instance.UpdateShapes(faceCUsers, 2);

    }

    void SortUsers(Vector2 position) {
        float proximity = 1f - Mathf.Clamp(Vector2.Distance(center, position), 0f,1f);
        float tempAngle = Vector2.SignedAngle(abscissa, position);
        float angle = tempAngle > 0f ? tempAngle : 360f + tempAngle;

        if (angle > 210 && angle <= 330) {
            faceAUsers.Add(proximity);
        }

        if (angle > 90 && angle <= 210) {
            faceBUsers.Add(proximity);
        }

        if (angle <= 90 || angle > 330) {
            faceCUsers.Add(proximity);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
