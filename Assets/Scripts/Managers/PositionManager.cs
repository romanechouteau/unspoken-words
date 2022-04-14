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

        ProcessUsers(faceA, faceAUsers);
        ProcessUsers(faceB, faceBUsers);
        ProcessUsers(faceC, faceCUsers);
    }

    void SortUsers(Vector2 position) {
        float distance = Vector2.Distance(center, position);
        float tempAngle = Vector2.SignedAngle(abscissa, position);
        float angle = tempAngle > 0f ? tempAngle : 360f + tempAngle;

        if (angle > 210 && angle <= 330) {
            faceAUsers.Add(distance);
        }

        if (angle > 90 && angle <= 210) {
            faceBUsers.Add(distance);
        }

        if (angle <= 90 || angle > 330) {
            faceCUsers.Add(distance);
        }
    }

    void ProcessUsers(Material face, List<float> users) {
        float zone1Dist = 5f;
        float zone2Dist = 5f;
        float zone3Dist = 5f;
        int zone1Count = 0;
        int zone2Count = 0;
        int zone3Count = 0;

        for(int i = 0; i<users.Count; i++) {
            float distance = users[i];
            if(distance <= 1f && distance > 0.75f) {
                zone1Count++;
                if(distance < zone1Dist) {
                    zone1Dist = distance;
                }
            }
            else if(distance <= 0.75f && distance > 0.5f) {
                zone2Count++;
                if(distance < zone2Dist) {
                    zone2Dist = distance;
                }
            }
            else if(distance <= 0.25f) {
                zone3Count++;
                if(distance < zone3Dist) {
                    zone3Dist = distance;
                }
            }
        }

        face.SetVector("_proximity", new Vector3(
            Mathf.Clamp(1f - ((zone1Dist - 0.75f) * 4f), 0f, 1f),
            Mathf.Clamp(1f - ((zone2Dist - 0.5f) * 4f), 0f, 1f),
            Mathf.Clamp(1f - zone3Dist * 4f, 0f, 1f)
        ));
        face.SetVector("_intensity", new Vector3(
            zone1Count,
            zone2Count,
            zone3Count
        ));

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(position);
    }
}
