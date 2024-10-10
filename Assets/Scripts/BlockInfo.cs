using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyCubeWorld/Blocks/BlockInfo")]
public class BlockInfo : ScriptableObject
{
    public BlockType type;
    public Vector2 pixelsOffset;

    public AudioClip stepSound;
}
