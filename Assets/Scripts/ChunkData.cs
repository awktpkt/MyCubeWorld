
using UnityEngine;

public class ChunkData
{
    public ChunkDataState State;
    
    public Vector2Int ChunkPosition;
    public BlockType[] Blocks;
    public ChunkRenderer Renderer;

    public ChunkData LeftChunk;
    public ChunkData RightChunk;
    public ChunkData ForwardChunk;
    public ChunkData BackChunk;

}

public enum ChunkDataState
{
    StartedLoading,
    Loaded,
    StartedMeshing,
    SpawnedInWorld,
    Unloaded
}
