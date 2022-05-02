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

    public float lerpSpeed = 0.1f;

    private Vector3[] proximity = new Vector3[3];
    private Vector3[] intensity = new Vector3[3];
    private Vector3[] lerpProximity = new Vector3[3];
    private Vector3[] lerpIntensity = new Vector3[3];

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

        ProcessUsers(faceA, faceAUsers, 0);
        ProcessUsers(faceB, faceBUsers, 1);
        ProcessUsers(faceC, faceCUsers, 2);
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

    void ProcessUsers(Material face, List<float> users, int index) {
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

        lerpProximity[index].x = Mathf.Clamp(1f - ((zone1Dist - 0.75f) * 4f), 0f, 1f);
        lerpProximity[index].y = Mathf.Clamp(1f - ((zone2Dist - 0.5f) * 4f), 0f, 1f);
        lerpProximity[index].z = Mathf.Clamp(1f - zone3Dist * 4f, 0f, 1f);

        lerpIntensity[index].x = zone1Count;
        lerpIntensity[index].y = zone2Count;
        lerpIntensity[index].z = zone3Count;
    }

    void LerpValues(Material face, int index) {
        proximity[index] = Vector3.Lerp(proximity[index], lerpProximity[index], lerpSpeed);
        face.SetVector("_proximity", proximity[index]);

        intensity[index] = Vector3.Lerp(intensity[index], lerpIntensity[index], lerpSpeed);
        face.SetVector("_intensity", intensity[index]);
    }

    // Update is called once per frame
    void Update()
    {
        LerpValues(faceA, 0);
        LerpValues(faceB, 1);
        LerpValues(faceC, 2);
    }
}
