using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshTools {
    public static class BasicShapes {
        private static readonly Vector3 origin = new Vector3(0, 0, 0);
        
        public static Mesh CreateCylinder(int numVertices, float radius, float height, Quaternion q) {
            return CreateCylinder(numVertices, radius, radius, height, q);
        }

        public static Mesh CreateCylinder(int numVertices, float topRadius, float bottomRadius, float height, Quaternion q) {
            var generator = new MeshGenerator();

            var bottomCircle = CreateCircle(numVertices, bottomRadius, 0);
            var topCircle = CreateCircle(numVertices, topRadius, height);

            generator.TriangulateCircle(bottomCircle, FaceDirection.CW);
            generator.BridgeEdgeLoopsSmooth(bottomCircle, topCircle);
            generator.TriangulateCircle(topCircle, FaceDirection.CCW);

            var mesh = generator.GetMesh("Cylinder");
            mesh.RecalculateNormals();

            return mesh;
        }

        internal static List<Vector3> CreateCircle(int n, float r, float yOffset) {
            var circleVertIndices = new List<Vector3>(n);
            var segmentRad = Constants.CIRCLE_RADIANS / n;

            // Generate vertices on circle
            for (var i = 0; i < n; i++) {
                var x = Mathf.Cos(segmentRad * i) * r;
                var z = Mathf.Sin(segmentRad * i) * r;

                var vertex = new Vector3(x, yOffset, z);

                circleVertIndices.Add(vertex);
            }

            return circleVertIndices;
        }
    }
}
