using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{

    public const int ChunkWidth = 20;
    public const int ChunkHeight = 128;
    public const float BlockScale = .5f;

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> triangles = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        Mesh chunkMesh = new Mesh();

        for(int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for(int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x,y,z);
                }
            }
        }

        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.uv = uvs.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if(blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
           blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
           blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight) return BlockType.Air;
            
            Vector2Int adjacentChunkPosition = ChunkData.ChunkPosition;
            if(blockPosition.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += ChunkWidth;
            }
            else if (blockPosition.x >= ChunkWidth)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
            }
            if (blockPosition.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += ChunkWidth;
            }
            else if (blockPosition.z >= ChunkWidth)
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
            }

            if(ParentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }
            else
            {
                return BlockType.Air;
            }
        }
    }

    public void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);
        
        if (GetBlockAtPosition(blockPosition) == 0) return; 

        if(GetBlockAtPosition(blockPosition + Vector3Int.right) == 0) GenerateRightSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0) GenerateLeftSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0) GenerateFrontSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0) GenerateBackSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0) GenerateTopSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0) GenerateBottomSide(blockPosition);
    }
    
    private void GenerateRightSide(Vector3Int blockPosition)
    {

        verticies.Add((new Vector3(1, 0, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition)*BlockScale);

        AddLastVerticiesSquare();
    }
                 
    private void GenerateFrontSide(Vector3Int blockPosition)
    {

        verticies.Add((new Vector3(0, 0, 1) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition)*BlockScale);

        AddLastVerticiesSquare();
    }
                 
    private void GenerateBackSide(Vector3Int blockPosition)
    {

        verticies.Add((new Vector3(0, 0, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition)*BlockScale);

        AddLastVerticiesSquare();
    }
    
    private void GenerateTopSide(Vector3Int blockPosition)
    {

        verticies.Add((new Vector3(0, 1, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition)*BlockScale);

        AddLastVerticiesSquare();
    }
    private void GenerateBottomSide(Vector3Int blockPosition)
    {

        verticies.Add((new Vector3(0, 0, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition)*BlockScale);

        AddLastVerticiesSquare();
    }


    private void AddLastVerticiesSquare()
    {
        uvs.Add(new Vector2(0f/256, 240f/256));
        uvs.Add(new Vector2(0f/256, 256f/256));
        uvs.Add(new Vector2(16f/256, 240f/256));
        uvs.Add(new Vector2(16f/256, 256f/256));
        
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);

        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {

        verticies.Add((new Vector3(0, 0, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition)*BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition)*BlockScale);

        AddLastVerticiesSquare();
    }
}
