using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Material DepthRaw;
    public Material DepthProcessing;
    public Vector2 TexResolution;
    public RenderTexture Output;

    private Texture2D DepthTexture;
    private float Detail;
    private Vector2 PixelSize;
    private List<Vector3> VisiblePixels = new List<Vector3>();

    void Start()
    {
        Detail = DepthProcessing.GetFloat("_Detail");
        PixelSize = new Vector2(TexResolution.x / Detail, TexResolution.y / Detail);
        DepthTexture = new Texture2D((int)TexResolution.x, (int)TexResolution.y);
    }

    // Update is called once per frame
    void Update()
    {
        Texture tempTexture = DepthRaw.mainTexture;
        Graphics.Blit(tempTexture, Output, DepthProcessing);
        DepthTexture.ReadPixels(new Rect(0, 0, TexResolution.x, TexResolution.y), 0, 0);
        DepthTexture.Apply();

        VisiblePixels.Clear();
        for (int i = 0; i < Detail; i++) {
            for (int j = 0; j < Detail; j++) {
                Vector2 coords = GetCenterAt(i, j);
                Color color = GetColorAt(coords);
                if (color.r > 0f) {
                    VisiblePixels.Add(new Vector3(color.r, i, j));
                }
            }
        }
    }

    Vector2 GetCenterAt(int x, int y) {
        return new Vector2(x * PixelSize.x + PixelSize.x / 2, y * PixelSize.y + PixelSize.y / 2);
    }

    Color GetColorAt(Vector2 coords) {
        return DepthTexture.GetPixel((int)coords.x, (int)coords.y);
    }
}
