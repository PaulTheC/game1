using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BiomeMap : MonoBehaviour
{
    public Texture2D nonStaticTexture;


    static public Texture2D texture;
    static List<Vector2> points;

    public static float minSize = 0.05f;
    public static float amount = 30;


    [MenuItem("Terrain/GenerateBiomes")]
    public static void generateBiomes()
    {
        points = new List<Vector2>();

        texture = (GameObject.Find("TerrainMaster").GetComponent<BiomeMap>().nonStaticTexture);
        //texture is required
        if (texture == null)
        {
            throw new MissingReferenceException();
        }

        generatePoints();
        generateTexture();
    }


    private static void generatePoints()
    {

        for (int i = 0; i < amount; i++)
        {
            Vector2 point = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            //if the point is near any other point it shouldnt be added
            bool inRange = false;
            foreach (Vector2 point2 in points)
            {
                if(Vector2.Distance(point, point2) < minSize)
                {
                    inRange = true;
                    break; 
                }
            }
            if (!inRange)
            {
                points.Add(point);
            }
        }
    }


    private static Vector2 getBiome(float x, float y)
    {



        //x and y must be within range 0 to 1
        if(x < 0 || y < 0 || x > 1 || y > 1)
        {
            throw new UnityException();
        }

        
        float distance = 1;
        float secondDistance = 1;
        Vector2 nearest = Vector2.one;
        Vector2 second = Vector2.one;
        foreach (Vector2 point in points)
        {
            float d = (point.x - x) * (point.x - x) + (point.y - y) * (point.y - y);
            if(d < distance)
            {
                second = nearest;
                nearest = point;

                secondDistance = distance;
                distance = d;
            }
        }

        //the distance between first and second point 
        //0 = only use first point 
        //1 = only use second point
        float blend = distance / secondDistance;
        

        return second;
    }    

    private static void generateTexture()
    {
        int width = texture.width;
        int height = texture.height;

        for (int y = 0;y < height; y++)
        {
            for(int x = 0;x < width; x++)
            {
                Vector2 point = getBiome(x / (float)texture.width, y / (float)texture.height);
                Color color = new Color(point.x, point.y, 0);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }


    private static Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

}

