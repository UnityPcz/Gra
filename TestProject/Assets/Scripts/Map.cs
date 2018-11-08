using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{

    public GameObject placeholder;
    public const int chunksize = 64;
    public Tilemap[,] Chunk;
    public GameObject[,] ChunkGameobject;
    public int width;
    public int height;
    public Texture2D texture;
    public GameObject ChunkPrefab;
    public Tile tileToSpawn;    
    int iks = 0;
    int igrek = 0;
    public bool loaded = false;

    private void Start()
    {
        InitializeMap();
        SpawnMap();
        FillChunk(0, 1);
        Time.timeScale = 0;
    }

    // ładowanie mapy w kilku krokach bo tak lepiej działa (można zafreezować grę na początku dopóki loaded = false)
    private void Update()
    {
        if (!loaded)
        {
            FillChunk(iks, igrek);
            iks++;
            if (iks == Chunk.GetLength(0))
            {
                igrek++;
                iks = 0;
            }
            if (igrek >= Chunk.GetLength(1))
            {
                loaded = true;
                Destroy(placeholder);
                Time.timeScale = 1;
            }
        }
        
    }

    //inicjalizacja kilku zmiennych
    void InitializeMap()
    {
        width = texture.width;
        height = texture.height;
               
    }

    //spawn tilemap aka chunków (pustych)
    public void SpawnMap()
    {
        if (Chunk != null || ChunkGameobject != null)
        {
            Chunk = null;
            foreach (GameObject g in ChunkGameobject)
            {
                Destroy(g);
            }
            ChunkGameobject = null;
        }
       
        int NumOfChunksX = (width / chunksize) + 1;
        int NumOfChunksY = (height / chunksize) + 1;
        Chunk = new Tilemap[NumOfChunksX, NumOfChunksY];
        ChunkGameobject = new GameObject[NumOfChunksX, NumOfChunksY];

        for (int i = 0; i < NumOfChunksX; i++)
        {
            for (int j = 0; j < NumOfChunksY; j++)
            {
                ChunkGameobject[i, j] = Instantiate(ChunkPrefab, transform);                
                Chunk[i, j] = ChunkGameobject[i, j].transform.GetChild(0).GetComponent<Tilemap>();
            }
        }
    }

    // wypełnia chunk o współrzędnych (X,Y) tilesami o kolorze pobranym ze zmiennej texture
    void FillChunk(int X, int Y)
    {
        for (int i = 0; i < chunksize; i++)
        {
            for (int j = 0; j < chunksize; j++)
            {
                int x = (X * 64) + i;
                int y = (Y * 64) + j;
                if (x < width && y < height)
                {
                    Color c = texture.GetPixel((X * 64) + i, (Y * 64) + j);
                    if (c.a > 0.5f)
                    {
                        tileToSpawn.color = c;                        
                        Chunk[X,Y].SetTile(new Vector3Int(x, y, 0), tileToSpawn);
                    }
                }
            }
        }        
    }

}
