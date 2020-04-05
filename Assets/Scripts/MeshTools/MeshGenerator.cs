using System.Collections.Generic;
using UnityEngine;

namespace MeshTools {
    public class MeshGenerator {
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<int> triangles = new List<int>();

        private readonly Vector3 origin;

        public MeshGenerator() {
            origin = new Vector3(0, 0, 0);
        }

        public MeshGenerator(Vector3 origin) {
            this.origin = origin;
        }

        public Mesh GetMesh(string name = "") {
            var mesh = new Mesh {
                name = name, 
                vertices = vertices.ToArray(), 
                triangles = triangles.ToArray()
            };

            return mesh;
        }

        public void RotateVertices(Quaternion q) {
            for (var i = 0; i < vertices.Count; i++) {
                vertices[i] = q * (vertices[i] - origin) + origin;
            }
        }

        public void TriangulateCircle(List<Vector3> circle, FaceDirection dir) {
            // Create center point
            // TODO other triangulation methods don't necessarily need a center point
            var center = new Vector3(0, circle[0].y, 0);
            var centerIndex = vertices.Count;
            vertices.Add(center);

            var n = circle.Count;

            var vertexIndices = AddVertices(circle);

            // Add triangles
            for (var i = 0; i < n; i++) {
                AddTriangle(centerIndex, vertexIndices[i], vertexIndices[(i + 1) % n], dir);
            }
        }

        public void BridgeEdgeLoopsSmooth(List<Vector3> c1, List<Vector3> c2) {
            if (c1.Count != c2.Count) {
                throw new System.ArgumentException("Circle segment count must be equal");
            }

            var n = c1.Count - 1;

            var c1Verts = AddVertices(c1);
            var c2Verts = AddVertices(c2);

            for (var i = 0; i < n; i++) {
                AddQuad(c1Verts[i], c2Verts[i], c2Verts[i + 1], c1Verts[i + 1]);
            }

            AddQuad(c1Verts[n], c2Verts[n], c2Verts[0], c1Verts[0]);
        }
        
        public void BridgeEdgeLoops(List<Vector3> c1, List<Vector3> c2) {
            if (c1.Count != c2.Count) {
                throw new System.ArgumentException("Circle segment count must be equal");
            }

            var n = c1.Count - 1;

            for (var i = 0; i < n; i++) {
                AddQuadNew(c1[i], c2[i], c2[i + 1], c1[i + 1]);
            }

            AddQuadNew(c1[n], c2[n], c2[0], c1[0]);
        }

        public List<int> AddVertices(List<Vector3> original) {
            var indices = new List<int>(original.Count);

            foreach (var i in original) {
                indices.Add(vertices.Count);
                vertices.Add(i);
            }

            return indices;
        }

        public void AddTriangle(int v1, int v2, int v3, FaceDirection dir = FaceDirection.CW) {
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

        public void AddQuad(int v1, int v2, int v3, int v4, FaceDirection dir = FaceDirection.CW) {
            AddTriangle(v1, v2, v3, dir);
            AddTriangle(v3, v4, v1, dir);
        }
        
        public void AddQuadNew(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
            var vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex);
        }
    }
}
