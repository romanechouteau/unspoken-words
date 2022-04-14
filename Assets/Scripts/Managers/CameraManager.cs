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
    private int Detail;
    private Vector2 PixelSize;

    // Groups of pixels that symbolize users
    private List<List<Vector3>> Groups = new List<List<Vector3>>();
    // Pixels that were already checked
    private bool[] Checked;
    // Center position of Groups
    private List<Vector2> Positions = new List<Vector2>();

    void Start()
    {
        Detail = DepthProcessing.GetInteger("_Detail");
        PixelSize = new Vector2(TexResolution.x / Detail, TexResolution.y / Detail);
        DepthTexture = new Texture2D((int)TexResolution.x, (int)TexResolution.y);
        // One dimensional array of checked pixels
        Checked = new bool[Detail*Detail];
    }

    // Update is called once per frame
    void Update()
    {
        // Get Raw texture from RsDevice then apply processing
        Texture tempTexture = DepthRaw.mainTexture;
        Graphics.Blit(tempTexture, Output, DepthProcessing);
        DepthTexture.ReadPixels(new Rect(0, 0, TexResolution.x, TexResolution.y), 0, 0);
        DepthTexture.Apply();

        // Reset
        Groups = new List<List<Vector3>>();
        Checked = new bool[Detail*Detail];

        // For each processed pixel
        for (int y = 0; y < Detail; y++) {
            for (int x = 0; x < Detail; x++) {
                CheckPixel(x,y,true);
            }
        }
        // Clear positions
        Positions.Clear();

        // Get user position from pixel groups
        for(int i = 0; i< Groups.Count; i++) {
            SetGroupPosition(i);
        }

        // Send positions to PositionManager
       PositionManager.Instance.SetPosition(Positions.Count != 0 ? Positions[0] : new Vector2(-1,-1));
    }

    // Check pixel's color and recursively check surrounding pixels
    void CheckPixel(int x, int y, bool first) {
        // Only proceed if pixel exists and is not already checked
        if(x >= 0 && x < Detail && y >= 0 && y < Detail && !Checked[y * Detail + x]) {
            Vector2 coords = GetCenterAt(x,y);
            Color color = GetColorAt(coords);
            Checked[y * Detail + x] = true;

            if (color.r > 0f) {
                // If it comes from the update loop, it's a new Group of pixels
                if(first) {
                    List<Vector3> group = new List<Vector3>();
                    Groups.Add(group);
                }

                Groups[Groups.Count-1].Add(new Vector3((float)x, (float)y, color.r));

                // Call itself to check surrounding pixels
                // top
                CheckPixel(x,y-1,false);
                // left
                CheckPixel(x-1,y,false);
                // right
                CheckPixel(x+1,y,false);
                // bottom
                CheckPixel(x,y+1,false);
            }
        }
    }

    // Get position of a pixel group
    void SetGroupPosition(int index) {
        List<Vector3> group = Groups[index];

        // Calc average position based on each pixel in the pixel group
        Vector2 total = new Vector2(0f,0f);
        float coefficient = 0f;
        for(int i = 0; i < group.Count; i++) {
            Vector3 pixel = group[i];
            // Multiply coords by color intensity then add to total
            total += new Vector2(pixel.x * pixel.z,pixel.y * pixel.z);
            coefficient += pixel.z;

        }

        // Pixel group position on [0,10]
        Vector2 position = total/coefficient;

        // Pixel group position on [-1,1]
        Vector2 normalizedPosition = (position / Detail) * 2 - new Vector2(1,1);
        Positions.Add(normalizedPosition);
    }

    Vector2 GetCenterAt(int x, int y) {
        return new Vector2(x * PixelSize.x + PixelSize.x / 2, y * PixelSize.y + PixelSize.y / 2);
    }

    Color GetColorAt(Vector2 coords) {
        return DepthTexture.GetPixel((int)coords.x, (int)coords.y);
    }
}
