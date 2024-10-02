using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGenerator 
{
    public static BlockType[,,] GenerateTerrain(float xOffset, float yOffset)
    {
        BlockType[,,] result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        for(int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for(int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float height = Mathf.PerlinNoise((x/2f+xOffset) * .2f, (z/2f+yOffset) * .2f) * 50 + 10;

                for(int y = 0; y < height; y++)
                {
                    result[x, y, z] = BlockType.Grass;
                }
            }
        }

        return result;
    }
}
