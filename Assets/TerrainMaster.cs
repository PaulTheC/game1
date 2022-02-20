using UnityEngine;
using UnityEditor;
using System;


public class TerrainMaster : MonoBehaviour
{
    public GameObject prefabL;


    public static int viewDistance = 5;
    public static GameObject reference;
    public static GameObject prefab;
    private static GameObject[] chunks;

    private float time = 10;


    private void Start()
    {

        chunks = GameObject.FindGameObjectsWithTag("Ground");

        if (chunks.Length == 0)
        {
            generateTerrain();
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 10)
        {
            time = 0;
            updateTerrain();
            Debug.Log("updated");
        }
    }

    [MenuItem("Terrain/GenerateTerrain")]
    public static void generateTerrain()
    {

        reference = GameObject.Find("TerrainMaster");
        prefab = reference.GetComponent<TerrainMaster>().prefabL;


        chunks = new GameObject[(viewDistance * 2 + 1) * (viewDistance * 2 + 1)];

        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i] = Instantiate(prefab, reference.transform.position, reference.transform.rotation);
            chunks[i].transform.SetParent(reference.transform);
            chunks[i].layer = LayerMask.NameToLayer("Ground");
            chunks[i].tag = "Ground";

        }

    }

    [MenuItem("Terrain/UpdateTerrain")]
    public static void updateTerrain()
    {

        double time = -(int)(Time.timeSinceLevelLoad * 1000f) % 1000;


        if (chunks == null)
        {
            chunks = GameObject.FindGameObjectsWithTag("Ground");
        }

        Vector3 origin;
        try
        {
            origin = Camera.current.transform.position;
        }catch(NullReferenceException e)
        {
            origin = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        }
        Vector3 modulo = new Vector3(origin.x % 25, 0, origin.z % 25);
        Vector3 pos = origin - modulo;

        for(int x = -viewDistance, i = 0; x <= viewDistance; x++)
        {
            for(int y = -viewDistance; y <= viewDistance; y++)
            {
                chunks[i].transform.position = new Vector3(pos.x + x * 25, 0, pos.z + y * 25);
                chunks[i].GetComponent<MeshGeneratorV2>().createMesh();
                chunks[i].transform.localScale = Vector3.one;
                i++;
            }
        }

        Debug.Log("It took " + (time + (int)(Time.timeSinceLevelLoad * 1000f) % 1000) + " milliseconds to update. Started at "+ time);

    }

}
