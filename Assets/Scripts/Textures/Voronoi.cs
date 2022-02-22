using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voronoi
{

    static List<Vector2> points;

    public static float minSize;
    public static int amount;

    public static Texture2D createVoronoiTexture(int amount = 30, float pointDistance = 0.15f, int seed = -1)
    {

        if(seed == -1)
        {
            Random.InitState((int)(((Time.realtimeSinceStartup) % 1) * int.MaxValue));
        }
        else
        {
            Random.InitState(seed);
        }

        Voronoi.amount = amount;
        Voronoi.minSize = pointDistance;

        points = new List<Vector2>();
        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGBA32, true);


        generatePoints();
        generateVoronoiTexture(texture);

        return texture;

    }


    public static Texture2D createVoronoiTesselationTexture(int amount = 30, float pointDistance = 0.15f, int seed = -1)
    {

        if (seed == -1)
        {
            Random.InitState((int)(((Time.realtimeSinceStartup) % 1) * int.MaxValue));
        }
        else
        {
            Random.InitState(seed);
        }

        Voronoi.amount = amount;
        Voronoi.minSize = pointDistance;

        points = new List<Vector2>();
        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, true);


        generatePoints();
        generateVoronoiTesselationTexture(texture);

        return texture;

    }





    private static void generatePoints()
    {
        int maxChecks = amount * 20;
        for (int i = 0; i < amount; i++)
        {
            Vector2 point = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            //if the point is near any other point it shouldnt be added
            bool inRange = false;
            foreach (Vector2 point2 in points)
            {
                if (Vector2.Distance(point, point2) < minSize)
                {
                    inRange = true;
                    break;
                }
            }
            if (!inRange)
            {
                points.Add(point);
            }
            else
            {
                i--;
            }


            //making sure no endless loop occurs
            maxChecks--;
            if (maxChecks == 0)
            {
                break;
            }

        }
    }



    //this funktions returns a voronoi tesselation image in the r and g channel
    //and a voronoi texture in b channel
    private static Vector3 getBiome(float x, float y)
    {



        //x and y must be within range 0 to 1
        if (x < 0 || y < 0 || x > 1 || y > 1)
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
            if (d < distance)
            {
                nearest = point;

                distance = d;
            }
        }

        foreach (Vector2 point in points)
        {
            float d = (point.x - x) * (point.x - x) + (point.y - y) * (point.y - y);
            if (d < secondDistance && d != distance)
            {
                second = point;

                secondDistance = d;
            }
        }

        //the distance between first and second point 
        //0 = only use first point 
        //1 = only use second point
        float blend = distance / secondDistance;


        Vector2 lerp = nearest * (1 - blend / 2) + second * (blend / 2);

        return new Vector3(nearest.x, nearest.y, blend);
    }

    private static void generateVoronoiTexture(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 point = getBiome(x / (float)texture.width, y / (float)texture.height);
                Color color = new Color(point.z, point.z, point.z, point.z);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }


    private static void generateVoronoiTesselationTexture(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 point = getBiome(x / (float)texture.width, y / (float)texture.height);
                Color color = new Color(point.x, point.y, 0);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

}
