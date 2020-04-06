using System;
using System.Collections.Generic;
using System.Diagnostics;
using MeshTools;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace TreeGenerator {
    public class TreeGenerator_OLD : MonoBehaviour {
        [SerializeField, Range(0, 1)] private float min, max = 1;
        [SerializeField] private float scale = 1;
        [SerializeField] private float distanceFromCenter = 1;
        [SerializeField] private int amount = 5;

        private readonly List<GameObject> gameObjects = new List<GameObject>();

        private void Start() {
            GenerateTree();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.G)) {
                GenerateTree();
            }
        }

        private void GenerateTree() {
            GenerateSpheres(amount);
        }

        private void GenerateSpheres(int amt) {
            var stopwatch = Stopwatch.StartNew();

            Clear();

            var baseSphere = CreateSphere();
            baseSphere.transform.parent = transform;
            baseSphere.transform.localScale = Vector3.one;
            baseSphere.transform.localPosition = Vector3.zero;

            for (var i = 0; i < amt; i++) {
                var sphere = CreateSphere();
                sphere.transform.localScale = scale * Mathf.Lerp(Random.value, min, max) * Vector3.one;

                var randomPos = Random.onUnitSphere * distanceFromCenter;
                while (randomPos.y < 0) {
                    randomPos = Random.onUnitSphere * distanceFromCenter;
                }

                sphere.transform.parent = baseSphere.transform;
                sphere.transform.localPosition = randomPos;
            }

            stopwatch.Stop();
            Debug.Log($"Generating took {stopwatch.ElapsedMilliseconds}ms");
        }

        private GameObject CreateSphere() {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            var coll = sphere.GetComponent<Collider>();
            if (coll) Destroy(coll);

            gameObjects.Add(sphere);
            return sphere;
        }

        private void Clear() {
            gameObjects.ForEach(Destroy);
            gameObjects.Clear();
        }

        private T ExecuteTimed<T>(Func<T> method) {
            var stopwatch = Stopwatch.StartNew();

            var result = method.Invoke();

            stopwatch.Stop();
            Debug.Log($"Generating took {stopwatch.ElapsedMilliseconds}ms");

            return result;
        }
    }
}
