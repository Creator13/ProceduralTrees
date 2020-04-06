using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshTools {
    public class ArmatureVisualizer : MonoBehaviour {
        private TreeArmature armature;
        [SerializeField] private bool keepChildren;
        
        private void Update() {
            if (armature != TreeMeshGenerator.Armature) {
                armature = TreeMeshGenerator.Armature;

                if (!keepChildren) {
                    foreach (Transform obj in transform) {
                        Destroy(obj.gameObject);
                    }
                }
                
                var lines = new List<List<Vector3>> {armature.trunkVertices};
                armature.branches?.ForEach(branch => lines.Add(branch.branchVertices));
                
                var i = 0;
                foreach (var vertList in lines) {
                    var obj = new GameObject($"branch{i}");
                    obj.transform.SetParent(transform, false);
                    
                    var gen = new MeshGenerator();
                    gen.AddVertices(vertList);
                    
                    var mf = obj.AddComponent<MeshFilter>();
                    mf.sharedMesh = gen.GetMesh($"branch{i}");
                    
                    obj.AddComponent<MeshRenderer>();
                    
                    var vis = obj.AddComponent<NormalVisualiser>();
                    vis.drawLines = true;
                    vis.showGizmos = true;
                    vis.showMeshInfoOnSelect = false;
                    i++;
                }
            }
        }
    }
}
