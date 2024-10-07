using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public float BaseHeight = 8;
    public NoiseOctaveSettings[] Octaves;
    public NoiseOctaveSettings DomainWarp;
    
    [System.Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    private FastNoiseLite[] octaveNoises;
    private FastNoiseLite warpNoise;

    public void Awake()
    {
        Init();
    }

    public void Init()
    {
        octaveNoises = new FastNoiseLite[Octaves.Length];
        for (int i = 0; i < Octaves.Length; i++)
        {
            octaveNoises[i] = new FastNoiseLite();
            octaveNoises[i].SetNoiseType(Octaves[i].NoiseType);
            octaveNoises[i].SetFrequency(Octaves[i].Frequency);
        }

        warpNoise = new FastNoiseLite();
        warpNoise.SetNoiseType(DomainWarp.NoiseType);
        warpNoise.SetFrequency(DomainWarp.Frequency);
        warpNoise.SetDomainWarpAmp(DomainWarp.Amplitude);
    }

    public BlockType[,,] GenerateTerrain(float xOffset, float yOffset)
    {
        
        BlockType[,,] result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        for(int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for(int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float height = GetHeight(x * ChunkRenderer.BlockScale + xOffset, z * ChunkRenderer.BlockScale + yOffset);
                float grassLayerHeight = 1;

                for(int y = 0; y < height / ChunkRenderer.BlockScale; y++)
                {
                    if(height - y*ChunkRenderer.BlockScale < grassLayerHeight)
                    {
                        result[x, y, z] = BlockType.Grass;
                    }
                    else
                    {
                        result[x, y, z] = BlockType.Unknown;
                    }
                }
            }
        }

        return result;
    }

    private float GetHeight(float x, float y)
    {
        warpNoise.DomainWarp(ref x, ref y);
        
        float result = BaseHeight;

        for(int i = 0; i < Octaves.Length; i++)
        {
            float noise = octaveNoises[i].GetNoise(x,y);
            result += noise * Octaves[i].Amplitude / 2;
        }

        return result;
    }
}
