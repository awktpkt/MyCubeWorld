using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public static class MeshBuilder
{

    public const int ChunkWidth = 20;
    public const int ChunkWidthSq = ChunkWidth * ChunkWidth;
    public const int ChunkHeight = 128;
    public const float BlockScale = .5f;

    public static GameWorld.GeneratedMeshData GenerateMesh(ChunkData chunkData)
    {
        List<GameWorld.GeneratedMeshVertex> verticies = new List<GameWorld.GeneratedMeshVertex>();
        
        int maxY = 0;
        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    if(GenerateBlock(x, y, z, verticies, chunkData))
                    {
                        if (maxY < y) maxY = y;
                    }
                }
            }
        }

        GameWorld.GeneratedMeshData mesh = new GameWorld.GeneratedMeshData();
        mesh.Vertices = verticies.ToArray();

        Vector3 boundsSize = new Vector3(ChunkWidth, maxY, ChunkWidth) * BlockScale;
        mesh.Bounds = new Bounds(boundsSize / 2, boundsSize);
        mesh.Data = chunkData;

        return mesh;
    }

    private static BlockType GetBlockAtPosition(Vector3Int blockPosition, ChunkData chunkData)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
           blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
           blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            int index = blockPosition.x + blockPosition.y * ChunkWidthSq + blockPosition.z * ChunkWidth;
            return chunkData.Blocks[index];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight) return BlockType.Air;

            if (blockPosition.x < 0)
            {
                if (chunkData.LeftChunk == null)
                {
                    return BlockType.Air;
                }
                blockPosition.x += ChunkWidth;
                int index = blockPosition.x + blockPosition.y * ChunkWidthSq + blockPosition.z * ChunkWidth;
                return chunkData.LeftChunk.Blocks[index];
            }
            else if (blockPosition.x >= ChunkWidth)
            {
                if (chunkData.RightChunk == null)
                {
                    return BlockType.Air;
                }
                blockPosition.x -= ChunkWidth;
                int index = blockPosition.x + blockPosition.y * ChunkWidthSq + blockPosition.z * ChunkWidth;
                return chunkData.RightChunk.Blocks[index];
            }
            if (blockPosition.z < 0)
            {
                if (chunkData.BackChunk == null)
                {
                    return BlockType.Air;
                }
                blockPosition.z += ChunkWidth;
                int index = blockPosition.x + blockPosition.y * ChunkWidthSq + blockPosition.z * ChunkWidth;
                return chunkData.BackChunk.Blocks[index];
            }
            else if (blockPosition.z >= ChunkWidth)
            {
                if (chunkData.ForwardChunk == null)
                {
                    return BlockType.Air;
                }
                blockPosition.z -= ChunkWidth;
                int index = blockPosition.x + blockPosition.y * ChunkWidthSq + blockPosition.z * ChunkWidth;
                return chunkData.ForwardChunk.Blocks[index];
            }

            return BlockType.Air;
        }
    }

    public static bool GenerateBlock(int x, int y, int z, List<GameWorld.GeneratedMeshVertex> verticies, ChunkData chunkData)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);

        BlockType blockType = GetBlockAtPosition(blockPosition, chunkData);
        if (blockType == BlockType.Air) return false;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right, chunkData) == 0)
        {
            GenerateRightSide(blockPosition, verticies, blockType);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.left, chunkData) == 0)
        {
            GenerateLeftSide(blockPosition, verticies, blockType);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward, chunkData) == 0)
        {
            GenerateFrontSide(blockPosition, verticies, blockType);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.back, chunkData) == 0)
        {
            GenerateBackSide(blockPosition, verticies, blockType);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.up, chunkData) == 0)
        {
            GenerateTopSide(blockPosition, verticies, blockType);
        }
        if (blockPosition.y > 0 && GetBlockAtPosition(blockPosition + Vector3Int.down, chunkData) == 0)
        {
            GenerateBottomSide(blockPosition, verticies, blockType);
        }

        return true;
    }

    private static void GenerateRightSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies, BlockType blockType)
    {

        GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

        vertex.normalX = sbyte.MaxValue;
        vertex.normalY = 0;
        vertex.normalZ = 0;
        vertex.normalW = 1;
        GetUvs(blockType, out vertex.uvX, out vertex.uvY);

        vertex.pos = (new Vector3(1, 0, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 1, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 0, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 1, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);

    }

    private static void GenerateFrontSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies, BlockType blockType)
    {

        GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

        vertex.normalX = 0;
        vertex.normalY = 0;
        vertex.normalZ = sbyte.MaxValue;
        vertex.normalW = 1;
        GetUvs(blockType, out vertex.uvX, out vertex.uvY);

        vertex.pos = (new Vector3(0, 0, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 0, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(0, 1, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 1, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);

    }

    private static void GenerateBackSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies, BlockType blockType)
    {

        GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

        vertex.normalX = 0;
        vertex.normalY = 0;
        vertex.normalZ = sbyte.MinValue;
        vertex.normalW = 1;
        GetUvs(blockType, out vertex.uvX, out vertex.uvY);

        vertex.pos = (new Vector3(0, 0, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(0, 1, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 0, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 1, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);

    }

    private static void GenerateTopSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies, BlockType blockType)
    {

        GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

        vertex.normalX = 0;
        vertex.normalY = sbyte.MaxValue;
        vertex.normalZ = 0;
        vertex.normalW = 1;
        GetUvs(blockType, out vertex.uvX, out vertex.uvY);

        vertex.pos = (new Vector3(0, 1, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(0, 1, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 1, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 1, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);

    }
    private static void GenerateBottomSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies, BlockType blockType)
    {

        GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

        vertex.normalX = 0;
        vertex.normalY = sbyte.MinValue;
        vertex.normalZ = 0;
        vertex.normalW = 1;
        GetUvs(blockType, out vertex.uvX, out vertex.uvY);

        vertex.pos = (new Vector3(0, 0, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 0, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(0, 0, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(1, 0, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);

    }

    private static void GenerateLeftSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies, BlockType blockType)
    {

        GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

        vertex.normalX = sbyte.MinValue;
        vertex.normalY = 0;
        vertex.normalZ = 0;
        vertex.normalW = 1;
        GetUvs(blockType, out vertex.uvX, out vertex.uvY);

        vertex.pos = (new Vector3(0, 0, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(0, 0, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(0, 1, 0) + blockPosition) * BlockScale;
        verticies.Add(vertex);
        vertex.pos = (new Vector3(0, 1, 1) + blockPosition) * BlockScale;
        verticies.Add(vertex);

    }

    private static void GetUvs(BlockType blockType, out ushort x, out ushort y)
    {

        //BlockInfo info = Blocks.GetInfo(blockType);

        //if(info != null)
        //{
        //    uv = info.pixelsOffset / 256;
        //}
        //else
        //{
        //    uv = Blocks.GetInfo(BlockType.Unknown).pixelsOffset / 256;
        //}

        if (blockType == BlockType.Grass)
        {
            x = 0;
            y = 240 * 256;
        }
        else if (blockType == BlockType.Unknown)
        {
            x = 16 * 256;
            y = 240 * 256;
        }
        else
        {
            x = 16 * 256;
            y = 240 * 256;
        }
    }
}
