using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    private const int ViewRadius = 5;

    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer ChunkPrefab;
    public TerrainGenerator Generator;

    public Camera mainCamera;
    public Vector2Int currentPlayerChunk;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        StartCoroutine(Generate(false));
    }

    private IEnumerator Generate(bool wait)
    {
        for (int x = currentPlayerChunk.x - ViewRadius; x < currentPlayerChunk.x + ViewRadius; x++)
        {
            for (int y = currentPlayerChunk.y - ViewRadius; y < currentPlayerChunk.y + ViewRadius; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                if (ChunkDatas.ContainsKey(chunkPosition)) continue;

                LoadChunkAt(chunkPosition);

                if (wait) yield return new WaitForSecondsRealtime(0.2f);
            }
        }
    }

    [ContextMenu("Regenerate world")]
    public void Regenerate()
    {
        Generator.Init();

        foreach(var chunkData in ChunkDatas)
        {
            Destroy(chunkData.Value.Renderer.gameObject);
        }

        ChunkDatas.Clear();

        StartCoroutine(Generate(false));
    }

    private void LoadChunkAt(Vector2Int chunkPosition)
    {
        float xPos = chunkPosition.x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
        float zPos = chunkPosition.y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;

        ChunkData chunkData = new ChunkData();
        chunkData.ChunkPosition = chunkPosition;
        chunkData.Blocks = Generator.GenerateTerrain(xPos, zPos);
        ChunkDatas.Add(chunkPosition, chunkData);

        ChunkRenderer chunk = Instantiate(ChunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
        chunk.ChunkData = chunkData;
        chunk.ParentWorld = this;

        chunkData.Renderer = chunk;
    }

    private void Update()
    {
        Vector3Int playerWorldPos = Vector3Int.FloorToInt(mainCamera.transform.position / ChunkRenderer.BlockScale);
        Vector2Int playerChunk = GetChunkContainingBlock(playerWorldPos);

        if(playerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = playerChunk;
            StartCoroutine(Generate(true));
        }
    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        Vector2Int chunkPosition = new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWidth, blockWorldPos.z / ChunkRenderer.ChunkWidth);

        if (blockWorldPos.x < 0) chunkPosition.x--;
        if (blockWorldPos.y < 0) chunkPosition.y--;

        return chunkPosition;
    }

}
