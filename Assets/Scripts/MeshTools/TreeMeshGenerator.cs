using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MeshTools {
    public class TreeArmature {
        public List<Vector3> trunkVertices;
        public List<BranchArmature> branches;

        public float variance;

        public float trunkHeight;
        public int trunkSegments;

        public float SegmentHeight => trunkHeight / trunkSegments;
    }

    public class BranchArmature {
        public int branchVertexIndex;
        public List<Vector3> branchVertices;
    }

    public static class TreeMeshGenerator {
        private static readonly Vector3 Origin = new Vector3(0, 0, 0);

        public static TreeArmature Armature { get; private set; }

        public static Mesh CreateTree(int vertCount, float radius, float height, int subdivisions, float variance) {
            var meshGen = new MeshGenerator();

            var armature = CreateArmature(subdivisions, height, variance);
            var branches = CreateBranches(2, armature, 3);
            armature.branches = branches;
            Armature = armature;

            // Trunk
            {
                // Create trunk edge loops
                var edgeloops = new List<List<Vector3>>(armature.trunkVertices.Count);
                for (var i = 0; i < armature.trunkVertices.Count; i++) {
                    var circle = CreateCircle(vertCount, radius * Mathf.Sqrt(1 - (float) i / subdivisions), armature.trunkVertices[i]);
                    edgeloops.Add(circle);
                    meshGen.AddVertices(circle);
                }

                // Bridge trunk edge loops
                for (var i = 0; i < subdivisions; i++) {
                    meshGen.BridgeEdgeLoops(edgeloops[i], edgeloops[i + 1]);
                }
            }

            // Branches
            {
                foreach (var branch in branches) {
                    var divisions = branch.branchVertices.Count - 1;
                    
                    var edgeloops = new List<List<Vector3>>(branch.branchVertices.Count);
                    for (var i = 0; i < branch.branchVertices.Count; i++) {
                        var branchPointRadius = radius * Mathf.Sqrt(1 - (float) branch.branchVertexIndex / subdivisions);
                        var circle = CreateCircle(vertCount, branchPointRadius * .7f * Mathf.Sqrt(1 - (float) i / divisions), branch.branchVertices[i]);

                        Vector3 rotationDir;
                        if (i == 0 || i == branch.branchVertices.Count - 1) {
                            rotationDir = branch.branchVertices[i];
                        }
                        else {
                            rotationDir = branch.branchVertices[i - 1] + branch.branchVertices[i];
                        }

                        circle.RotateVertices(branch.branchVertices[i], Quaternion.LookRotation(rotationDir, Vector3.down));
                        
                        edgeloops.Add(circle);
                        meshGen.AddVertices(circle);
                    }

                    // Bridge edge loops
                    for (var i = 0; i < divisions; i++) {
                        meshGen.BridgeEdgeLoops(edgeloops[i], edgeloops[i + 1]);
                    }
                }
            }

            var mesh = meshGen.GetMesh("Tree");
            mesh.RecalculateNormals();

            return mesh;
        }

        private static TreeArmature CreateArmature(int segments, float height, float variance) {
            height += (Random.value * 2 - 1) * variance * height * variance;
            var segmentLength = height / segments;

            var verts = new List<Vector3>(segments + 1);

            var x = Origin.x;
            var z = Origin.z;
            for (var i = 0; i < segments + 1; i++) {
                var y = Origin.y + segmentLength * i + (Random.value * 2 - 1) * variance * (i * .05f);
                verts.Add(new Vector3(x, y, z));

                // Offset x and z
                x += (Random.value * 2 - 1) * variance;
                z += (Random.value * 2 - 1) * variance;
            }

            return new TreeArmature {
                trunkVertices = verts,
                trunkHeight = height,
                trunkSegments = segments,
                variance = variance
            };
        }

        private static List<BranchArmature> CreateBranches(int count, TreeArmature armature, int segmentCountBase) {
            var branches = new List<BranchArmature>();

            var trunkSize = armature.trunkVertices.Count;
            var branchPointCandidates = new List<int>();
            for (var i = 0; i < trunkSize; i++) {
                if (i > Mathf.RoundToInt(trunkSize * .33f) && i < trunkSize - Mathf.RoundToInt(trunkSize * .1f)) {
                    branchPointCandidates.Add(i);
                }
            }

            // Find highest value for normalizing
            var max = branchPointCandidates.Max();

            // Select #count branch points on tree
            var branchPoints = new List<int>(count);
            do {
                for (var i = 0; i < branchPointCandidates.Count && branchPoints.Count < count; i++) {
                    var branchPoint = branchPointCandidates[i];
                    var chance = (float) branchPoint / max * .75f;
                    if (Random.value < chance) {
                        if (!branchPoints.Contains(branchPoint)) {
                            branchPoints.Add(branchPoint);
                        }
                    }
                }
            } while (branchPoints.Count != count);

            foreach (var branchPoint in branchPoints) {
                var branchOrigin = armature.trunkVertices[branchPoint];
                var branch = CreateBranchArmature(branchOrigin, trunkSize - 1 - branchPoint - Mathf.RoundToInt(trunkSize * .1f), armature.SegmentHeight, armature.variance);
                branch.RotateVertices(branchOrigin, Quaternion.Euler(0, Random.Range(0, 360), Random.value * armature.variance));

                branches.Add(new BranchArmature {
                    branchVertices = branch,
                    branchVertexIndex = branchPoint
                });
            }

            armature.branches = branches;

            return branches;
        }

        private static List<Vector3> CreateBranchArmature(
            Vector3 startVertex, int segments, float segmentLength, float variance
        ) {
            // Initialize list with only the starting vertex
            var verts = new List<Vector3>(segments + 1) {startVertex};

            for (var i = 0; i < segments; i++) {
                var currentSegmentLength = segmentLength * (1 - ((float) i / segments) * .5f);
                // Calculate x, y and z
                var x = currentSegmentLength + currentSegmentLength * (Random.value * 2 - 1) * variance;
                var y = (segmentLength * Mathf.Sqrt(i + 1) * (Random.value * .5f + .5f) + (Random.value * 2 - 1) * variance) / (segmentLength * 2);
                var z = (Random.value * 2 - 1) * variance * currentSegmentLength;

                var nextVertex = verts[i] + new Vector3(x, y, z);
                verts.Add(nextVertex);
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
