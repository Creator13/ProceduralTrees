using System;
using System.Diagnostics;
using MeshTools;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(NormalVisualiser))]
public class CylinderBehaviour : MonoBehaviour {
    [SerializeField] private int vertexCount = 24;
    [SerializeField] private float height = 1;
    [SerializeField] private float radius = 1;
    [SerializeField] private Vector3 rotation;

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
        var stopwatch = Stopwatch.StartNew();

        cylinder = MeshGenerator.CreateCylinder(vertexCount, radius, height, Quaternion.Euler(rotation));
        GetComponent<MeshFilter>().sharedMesh = cylinder;

        stopwatch.Stop();
        Debug.Log($"Generating took {stopwatch.Elapsed.TotalMilliseconds:F3}ms");
        Debug.Log($"Cylinder stats | verts: {cylinder.vertexCount} | tris: {cylinder.triangles.Length / 3}");
    }

}
