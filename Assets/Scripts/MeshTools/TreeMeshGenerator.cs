using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MeshTools {
    public static class TreeMeshGenerator {
        private static readonly Vector3 origin = new Vector3(0, 0, 0);

        public static Mesh CreateTree(int vertCount, float radius, float height, int subdivisions, float variance) {
            var meshGen = new MeshGenerator(origin);

            var armature = CreateArmature(subdivisions, height, variance);
            var edgeloops = new List<List<Vector3>>(armature.Count);
            for (var i = 0; i < armature.Count; i++) {
                var circle = CreateCircle(vertCount, radius * Mathf.Sqrt(1 - (float) i / subdivisions), armature[i]);
                edgeloops.Add(circle);
                meshGen.AddVertices(circle);
            }

            for (var i = 0; i < subdivisions; i++) {
                meshGen.BridgeEdgeLoops(edgeloops[i], edgeloops[i + 1]);
            }

            var mesh = meshGen.GetMesh("Tree");
            mesh.RecalculateNormals();

            return mesh;
        }

        private static List<Vector3> CreateArmature(int segments, float height, float variance) {
            var segmentLength = height / segments;

            var verts = new List<Vector3>(segments + 1);

            var x = origin.x;
            var z = origin.z;
            for (var i = 0; i < segments + 1; i++) {
                var y = origin.y + segmentLength * i + (Random.value * 2 - 1) * variance * (i * .05f);
                verts.Add(new Vector3(x, y, z));

                // Offset x and z
                x += (Random.value * 2 - 1) * variance;
                z += (Random.value * 2 - 1) * variance;
            }

            return verts;
        }

        private static List<Vector3> CreateCircle(int n, float r, Vector3 offset) {
            var circleVertIndices = new List<Vector3>(n);
            var segmentRad = Constants.CIRCLE_RADIANS / n;

            // Generate vertices on circle
            for (var i = 0; i < n; i++) {
                var x = Mathf.Cos(segmentRad * i) * r;
                var z = Mathf.Sin(segmentRad * i) * r;

                var vertex = new Vector3(x + offset.x, offset.y, z + offset.z);

                circleVertIndices.Add(vertex);
            }

            return circleVertIndices;
        }
    }
}
