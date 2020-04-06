using System;
using MeshTools;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class NormalVisualiser : MonoBehaviour {
    public bool showMeshInfoOnSelect = true;
    public bool showGizmos = true;
    public bool drawLines = false;
    
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
            MeshUtils.PrintMeshInfo(mesh);
        }
    }

    private void OnDrawGizmos() {
        if (!showGizmos) return;
        
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        if (!mesh) return;

        // Draw sphere for each point
        Gizmos.color = Color.black;
        foreach (var vertex in mesh.vertices) {
            Gizmos.DrawSphere(vertex + transform.position, .05f);
        }

        // Draw each normal
        Gizmos.color = Color.red;
        for (var i = 0; i < mesh.vertexCount; i++) {
            try {
                Gizmos.DrawRay(mesh.vertices[i] + transform.position, mesh.normals[i]);
            }
            catch (IndexOutOfRangeException _) { }
        }

        // Draw vertex lines
        if (!drawLines) return;
        Gizmos.color = Color.green;
        for (var i = 0; i < mesh.vertexCount - 1; i++) {
            Gizmos.DrawLine(mesh.vertices[i] + transform.position, mesh.vertices[i + 1] + transform.position);
        }
    }
}
