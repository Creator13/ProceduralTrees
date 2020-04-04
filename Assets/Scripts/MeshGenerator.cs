using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshTools {
    public static class MeshGenerator {
        private enum FaceDirection { CW, CCW }

        private const float CircleRadians = 2 * Mathf.PI;

        private static readonly Vector3 origin = new Vector3(0, 0, 0);

        private static readonly List<Vector3> vertices = new List<Vector3>();
        private static readonly List<int> triangles = new List<int>();

        public static Mesh CreateCylinder(int numVertices, float radius, float height, Quaternion q) {
            return CreateCylinder(numVertices, radius, radius, height, q);
        }

        public static Mesh CreateCylinder(int numVertices, float topRadius, float bottomRadius, float height, Quaternion q) {
            vertices.Clear();
            triangles.Clear();

            var mesh = new Mesh();

            var bottomCircle = CreateCircle(numVertices, bottomRadius, 0);
            var topCircle = CreateCircle(numVertices, topRadius, height);

            CreateCylinderCap(bottomCircle, FaceDirection.CW);
            BridgeCircles(bottomCircle, topCircle);
            CreateCylinderCap(topCircle, FaceDirection.CCW);

            var rotatedVerts = new List<Vector3>();
            vertices.ForEach(v => rotatedVerts.Add(q * (v - origin) + origin));

            mesh.name = "Cylinder";
            mesh.vertices = rotatedVerts.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }

        private static void CreateCylinderCap(List<int> circle, FaceDirection dir) {
            // Create center point
            // TODO other triangulation methods don't necessarily need a center point
            var center = new Vector3(0, vertices[circle[0]].y, 0);
            var centerIndex = vertices.Count;
            vertices.Add(center);

            var n = circle.Count;
            
            // Add triangles
            for (var i = 0; i < n; i++) {
                AddTriangle(centerIndex, circle[i], circle[(i + 1) % n], dir);
            }
        }

        private static List<int> CreateCircle(int n, float r, float yOffset) {
            var circleVertIndices = new List<int>(n);
            var segmentRad = CircleRadians / n;

            // Generate vertices on circle
            for (var i = 0; i < n; i++) {
                var x = Mathf.Cos(segmentRad * i) * r;
                var z = Mathf.Sin(segmentRad * i) * r;

                var vertex = new Vector3(x, yOffset, z);

                // Record index of vertex to add
                circleVertIndices.Add(vertices.Count);
                // Add vertex
                vertices.Add(vertex);
            }

            return circleVertIndices;
        }

        private static void BridgeCircles(List<int> c1, List<int> c2) {
            if (c1.Count != c2.Count) {
                throw new ArgumentException("Circle segment count must be equal");
            }
            
            var n = c1.Count - 1;

            var c1Copy = CopyVertices(c1);
            var c2Copy = CopyVertices(c2);
            
            for (var i = 0; i < n; i++) {
                AddQuad(c1Copy[i], c2Copy[i], c2Copy[i + 1], c1Copy[i + 1]);
            }

            AddQuad(c1Copy[n], c2Copy[n], c2Copy[0], c1Copy[0]);
        }

        private static List<int> CopyVertices(List<int> original) {
            var copy = new List<int>(original.Count);
            
            foreach (var i in original) {
                copy.Add(vertices.Count);
                vertices.Add(vertices[i]);
            }

            return copy;
        }

        private static void AddTriangle(int v1, int v2, int v3, FaceDirection dir = FaceDirection.CW) {
            triangles.Add(v1);
            if (dir == FaceDirection.CW) {
                triangles.Add(v2);
                triangles.Add(v3);
            }
            else {
                triangles.Add(v3);
                triangles.Add(v2);
            }
        }

        private static void AddQuad(int v1, int v2, int v3, int v4, FaceDirection dir = FaceDirection.CW) {
            AddTriangle(v1, v2, v3, dir);
            AddTriangle(v3, v4, v1, dir);
        }
    }
}
