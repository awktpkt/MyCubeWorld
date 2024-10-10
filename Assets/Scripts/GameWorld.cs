using JetBrains.Annotations;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    private const int ViewRadius = 5;

    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer ChunkPrefab;
    public TerrainGenerator Generator;

    public Camera mainCamera;
    public Vector2Int currentPlayerChunk;

    private ConcurrentQueue<GeneratedMeshData> meshingResults = new ConcurrentQueue<GeneratedMeshData>();

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct GeneratedMeshVertex
    {
        public Vector3 pos;
        public sbyte normalX, normalY, normalZ, normalW;
        public ushort uvX, uvY;
    }

    public class GeneratedMeshData 
    {
        public GeneratedMeshVertex[] Vertices;
        public Bounds Bounds;
        public ChunkData Data;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        ChunkRenderer.InitTriangles();

        Generator.Init();
        StartCoroutine(Generate(false));
    }

    private IEnumerator Generate(bool wait)
    {
        int loadRadius = ViewRadius + 1;
        Vector2Int center = currentPlayerChunk;

        List<ChunkData> loadingChunks = new List<ChunkData>();
        for (int x = center.x - loadRadius; x <= center.x + loadRadius; x++)
        {
            for (int y = center.y - loadRadius; y <= center.y + loadRadius; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                if (ChunkDatas.ContainsKey(chunkPosition)) continue;

                ChunkData loadingChunkData = LoadChunkAt(chunkPosition);
                loadingChunks.Add(loadingChunkData);

                if (wait) yield return null;
            }
        }

        //yield return new WaitWhile(() =>
        //{
        //    foreach(ChunkData c in loadingChunks)
        //    {
        //        if (c.State == ChunkDataState.StartedLoading) return true;
        //    }

        //    return false;
        //});

        while (loadingChunks.Any(c => c.State == ChunkDataState.StartedLoading))
        {
            yield return null;
        }

        for (int x = center.x - ViewRadius; x <= center.x + ViewRadius; x++)
        {
            for (int y = center.y - ViewRadius; y <= center.y + ViewRadius; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                ChunkData chunkData = ChunkDatas[chunkPosition];

                if (chunkData.Renderer != null) continue;

                SpawnChunkRenderer(chunkData);

                if (wait) yield return null;
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

    private ChunkData LoadChunkAt(Vector2Int chunkPosition)
    {
        float xPos = chunkPosition.x * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
        float zPos = chunkPosition.y * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;

        ChunkData chunkData = new ChunkData();
        chunkData.State = ChunkDataState.StartedLoading;
        chunkData.ChunkPosition = chunkPosition;

        ChunkDatas.Add(chunkPosition, chunkData);

        Task.Factory.StartNew(() =>
        {
            chunkData.Blocks = Generator.GenerateTerrain(xPos, zPos);
            chunkData.State = ChunkDataState.Loaded;
        });

        return chunkData;
    }

    private void SpawnChunkRenderer(ChunkData chunkData)
    {
        ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.left, out chunkData.LeftChunk);
        ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.right, out chunkData.RightChunk);
        ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.up, out chunkData.ForwardChunk);
        ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.down, out chunkData.BackChunk);

        chunkData.State = ChunkDataState.StartedMeshing;

        Task.Factory.StartNew(() =>
        {
            GeneratedMeshData meshData = MeshBuilder.GenerateMesh(chunkData);
            meshingResults.Enqueue(meshData);
        });
    }

    private void Update()
    {
        Vector3Int playerWorldPos = Vector3Int.FloorToInt(mainCamera.transform.position / MeshBuilder.BlockScale);
        Vector2Int playerChunk = GetChunkContainingBlock(playerWorldPos);

        if(playerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = playerChunk;
            StartCoroutine(Generate(true));
        }

        if (meshingResults.TryDequeue(out GeneratedMeshData meshData))
        {
            float xPos = meshData.Data.ChunkPosition.x * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
            float zPos = meshData.Data.ChunkPosition.y * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;

            ChunkRenderer chunk = Instantiate(ChunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
            chunk.ChunkData = meshData.Data;
            chunk.ParentWorld = this;

            chunk.SetMesh(meshData);

            meshData.Data.Renderer = chunk;
            meshData.Data.State = ChunkDataState.SpawnedInWorld;
        }
    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        Vector2Int chunkPosition = new Vector2Int(blockWorldPos.x / MeshBuilder.ChunkWidth, blockWorldPos.z / MeshBuilder.ChunkWidth);

        if (blockWorldPos.x < 0) chunkPosition.x--;
        if (blockWorldPos.y < 0) chunkPosition.y--;

        return chunkPosition;
    }

}
