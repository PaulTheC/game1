using UnityEngine;
using UnityEditor;
using System;


public class TerrainMaster : MonoBehaviour
{

    //constants
    public const string ground = "Ground";

    //quasi constant variables
    public static float CHUNK_SIZE;
    public static int VIEW_DISTANCE;

    public float chunkSize = 16;
    public int viewDistance = 5;
    public GameObject prefabL;
    public float noiseSize = 100;

    public static GameObject reference;
    public static GameObject prefab;
    private static GameObject[] chunks;

    private float time = 10;


    private void Start()
    {

        removeTerrain();
        generateTerrain();
        

    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 1)
        {
            time = 0;
            updateTerrain();
        }
    }

    [MenuItem("Terrain/GenerateTerrain")]
    public static void generateTerrain()
    {

        reference = GameObject.Find("TerrainMaster");
        prefab = reference.GetComponent<TerrainMaster>().prefabL;
        CHUNK_SIZE = reference.GetComponent<TerrainMaster>().chunkSize;
        VIEW_DISTANCE = reference.GetComponent<TerrainMaster>().viewDistance;


        chunks = new GameObject[(VIEW_DISTANCE * 2 + 1) * (VIEW_DISTANCE * 2 + 1)];

        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i] = Instantiate(prefab, reference.transform.position, reference.transform.rotation);
            chunks[i].transform.SetParent(reference.transform);
            chunks[i].layer = LayerMask.NameToLayer(ground);
            chunks[i].tag = ground;
            chunks[i].name = UnityEngine.Random.Range(0, 1000).ToString();
            chunks[i].GetComponent<MeshGeneratorV2>().xSize = (int)CHUNK_SIZE;
            chunks[i].GetComponent<MeshGeneratorV2>().zSize = (int)CHUNK_SIZE;
            chunks[i].GetComponent<MeshGeneratorV2>().scale = reference.GetComponent<TerrainMaster>().noiseSize;
        }

    }

    [MenuItem("Terrain/UpdateTerrain")]
    public static void updateTerrain()
    {

        double time = -(int)(Time.timeSinceLevelLoad * 1000f) % 1000;


        if (chunks == null)
        {
            chunks = GameObject.FindGameObjectsWithTag(ground);
        }

        Vector3 origin;
        if (!Application.isPlaying) {
            origin = Camera.current.transform.position;
        }
        else { 
            origin = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        }

        Vector3 modulo = new Vector3(origin.x % CHUNK_SIZE, 0, origin.z % CHUNK_SIZE);
        Vector3 pos = origin - modulo;



        GameObject[] copy = (GameObject[])chunks.Clone();


        for(int x = -VIEW_DISTANCE, i = 0; x <= VIEW_DISTANCE; x++)
        {
            for(int y = -VIEW_DISTANCE; y <= VIEW_DISTANCE; y++)
            {
                Vector3 position = new Vector3(pos.x + x * CHUNK_SIZE, 0, pos.z + y * CHUNK_SIZE);


                GameObject test = checkIfPositionAlreadyExists(position, copy, copy[i]);

                if (test != null && test.GetComponent<MeshGeneratorV2>().getMesh() != null)
                {
                    
                    chunks[i].transform.position = position;
                    chunks[i].GetComponent<MeshGeneratorV2>().scale = reference.GetComponent<TerrainMaster>().noiseSize;
                    chunks[i].GetComponent<MeshGeneratorV2>().setMesh(test.GetComponent<MeshGeneratorV2>().getMesh());
                    chunks[i].transform.localScale = Vector3.one;
                    
                    i++;
                    continue;
                }

                chunks[i].transform.position = position;
                chunks[i].GetComponent<MeshGeneratorV2>().scale = reference.GetComponent<TerrainMaster>().noiseSize;
                chunks[i].GetComponent<MeshGeneratorV2>().createMesh();
                chunks[i].transform.localScale = Vector3.one;
                i++;
            }
        }


    }


    private static GameObject checkIfPositionAlreadyExists(Vector3 position, GameObject[] chunks, GameObject c)
    {

        foreach(GameObject chunk in chunks)
        {
            if(chunk.transform.position == position )
            {
                //Debug.Log("found    " + c.name + "     " + chunk.name);
                return chunk;
            }
        }
        return null;

    }



    [MenuItem("Terrain/RemoveTerrain")]
    public static void removeTerrain()
    {
        reference = GameObject.Find("TerrainMaster");
        foreach (MeshGeneratorV2 mesh in reference.GetComponentsInChildren<MeshGeneratorV2>())
        {
            if (!Application.isPlaying) {
                DestroyImmediate(mesh.gameObject);
            }else
            {
                Destroy(mesh.gameObject);
            }
        }
    }

}
