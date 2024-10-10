using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    public BlockDatabase Blocks;

    private Mesh chunkMesh;

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();

    private static int[] triangles;

    public static void InitTriangles()
    {
        triangles = new int[65536 * 6 / 4];

        int vertexNum = 4;
        for(int i = 0; i < triangles.Length; i += 6)
        {
            triangles[i] = vertexNum - 4;
            triangles[i + 1] = vertexNum - 3;
            triangles[i + 2] = vertexNum - 2;
            triangles[i + 3] = vertexNum - 3;
            triangles[i + 4] = vertexNum - 1;
            triangles[i + 5] = vertexNum - 2;

            vertexNum += 4;
        }
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        
        chunkMesh = new Mesh();

        GetComponent<MeshFilter>().mesh = chunkMesh;
    }

    public void SetMesh(GameWorld.GeneratedMeshData meshData)
    {

        VertexAttributeDescriptor[] layout = new VertexAttributeDescriptor[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.SNorm8, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.UNorm16, 2)
        };

        chunkMesh.SetVertexBufferParams(meshData.Vertices.Length, layout);
        chunkMesh.SetVertexBufferData(meshData.Vertices,0,0,meshData.Vertices.Length,0,MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds);

        int trianglesCount = meshData.Vertices.Length / 4 * 6;

        chunkMesh.SetIndexBufferParams(trianglesCount, IndexFormat.UInt32);
        chunkMesh.SetIndexBufferData(triangles, 0, 0, trianglesCount, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds);

        chunkMesh.subMeshCount = 1;
        chunkMesh.SetSubMesh(0, new SubMeshDescriptor(0, trianglesCount));

        chunkMesh.bounds = meshData.Bounds;

        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
