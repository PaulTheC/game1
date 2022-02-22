using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

public class BiomeMap : MonoBehaviour
{

    static public Texture2D texture;
    static List<Vector2> points;

    public static float minSize = 0.25f;
    public static int amount = 30;


    [MenuItem("Terrain/GenerateBiomes")]
    public static void generateBiomes()
    {

        texture = Voronoi.createVoronoiTesselationTexture();


        GameObject.Find("RawImage").GetComponent<RawImage>().texture = texture;
        GameObject.Find("Canvas").SetActive(false);
     
            
    }


    public static void getBiomeforPosition()
    {

    }

}

