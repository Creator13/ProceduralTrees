using System.Collections.Generic;
using UnityEngine;

namespace MeshTools {
    public static class MeshUtils {
        public static void PrintMeshInfo(Mesh mesh) {
            Debug.Log($"Mesh info for mesh {mesh.name}: {mesh.vertexCount} vertices - {mesh.triangles.Length} tris");
        }

        public static void RotateVertices(this List<Vector3> input, Vector3 origin, Quaternion q) {
            for (var i = 0; i < input.Count; i++) {
                input[i] = q * (input[i] - origin) + origin;
            }
        }
    }
}
