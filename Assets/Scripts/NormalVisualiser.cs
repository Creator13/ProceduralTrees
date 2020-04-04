using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class NormalVisualiser : MonoBehaviour {
    [SerializeField] private bool showMeshInfoOnSelect = true;
    [SerializeField] private bool showGizmos = true;
    
    private void OnEnable() {
        Selection.selectionChanged += PrintMeshInfo;
    }

    private void OnDisable() {
        Selection.selectionChanged -= PrintMeshInfo;
    }

    private void OnDestroy() {
        Selection.selectionChanged -= PrintMeshInfo;
    }

    private void PrintMeshInfo() {
        if (!showMeshInfoOnSelect) return;
        
        if (Selection.activeGameObject == gameObject) {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            Debug.Log($"Mesh info: {mesh.vertexCount} vertices - {mesh.triangles.Length} tris");
        }
    }

    private void OnDrawGizmos() {
        if (!showGizmos) return;
        
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        if (!mesh) return;

        Gizmos.color = Color.black;
        foreach (var vertex in mesh.vertices) {
            Gizmos.DrawSphere(vertex + transform.position, .05f);
        }

        Gizmos.color = Color.red;
        for (var i = 0; i < mesh.vertexCount; i++) {
            try {
                Gizmos.DrawRay(mesh.vertices[i] + transform.position, mesh.normals[i]);
            }
            catch (IndexOutOfRangeException _) { }
        }
    }
}
