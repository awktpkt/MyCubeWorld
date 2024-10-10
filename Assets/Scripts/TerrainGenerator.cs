using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

[CreateAssetMenu(menuName = "MyCubeWorld/Generator")]
public class TerrainGenerator : ScriptableObject
{
    public float BaseHeight = 8;
    public BlockType upperLayer;
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

    public BlockType[] GenerateTerrain(float xOffset, float yOffset)
    {
        
        BlockType[] result = new BlockType[MeshBuilder.ChunkWidth * MeshBuilder.ChunkHeight * MeshBuilder.ChunkWidth];

        for(int x = 0; x < MeshBuilder.ChunkWidth; x++)
        {
            for(int z = 0; z < MeshBuilder.ChunkWidth; z++)
            {
                float height = GetHeight(x * MeshBuilder.BlockScale + xOffset, z * MeshBuilder.BlockScale + yOffset);
                float grassLayerHeight = 1;

                for (int y = 0; y < height / MeshBuilder.BlockScale; y++)
                {
                    int index = x + y * MeshBuilder.ChunkWidthSq + z * MeshBuilder.ChunkWidth;

                    if (height - y* MeshBuilder.BlockScale < grassLayerHeight)
                    {
                        result[index] = upperLayer;
                    }
                    else
                    {
                        result[index] = BlockType.Unknown;
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
