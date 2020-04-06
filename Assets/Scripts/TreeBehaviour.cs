using System;
using System.Diagnostics;
using MeshTools;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(NormalVisualiser))]
public class TreeBehaviour : MonoBehaviour {
    [SerializeField] private int vertexCount = 7;
    [SerializeField] private float height = 10;
    [SerializeField] private float radius = .5f;
    [SerializeField] private int subdivisions = 6;
    [SerializeField] private float variance = .2f;
    [SerializeField] private int branches = 2;
    [SerializeField] private int seed;

    private Mesh cylinder;

    private void Start() {
        Generate();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            Generate();
        }
    }

    private void Generate() {
        var random = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(seed == 0 ? random : seed);

        var stopwatch = Stopwatch.StartNew();

        cylinder = TreeMeshGenerator.CreateTree(vertexCount, radius, height, subdivisions, variance, branches);
        GetComponent<MeshFilter>().sharedMesh = cylinder;

        stopwatch.Stop();
        Debug.Log($"Generating took {stopwatch.Elapsed.TotalMilliseconds:F3}ms");
        Debug.Log($"Tree stats | verts: {cylinder.vertexCount} | tris: {cylinder.triangles.Length / 3}");
    }

}
