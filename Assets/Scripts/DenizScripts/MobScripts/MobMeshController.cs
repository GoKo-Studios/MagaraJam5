using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Controllers {
    public class MobMeshController : MonoBehaviour
    {
        private MobManager _manager;
        private List<GameObject> _meshList;

        private void PopulateMeshList() {
            _meshList = new List<GameObject>();
            GameObject[] meshes = ResourceLoader.LoadResources<GameObject>("Objects/PoolableObjects");
            foreach (GameObject mesh in meshes) {
                GameObject obj = Instantiate(mesh, transform);
                _meshList.Add(obj);
                obj.SetActive(false);
            }
        }

        private void OnEnable() {
            _manager.OnSetup += OnSetup;
            _manager.OnClear += OnClear;
        }

        private void OnDisable() {
            _manager.OnSetup -= OnSetup;
            _manager.OnClear -= OnClear;
        }
        
        private void Awake() {
            _manager = GetComponentInParent<MobManager>();

            PopulateMeshList();
        }

        private void SetupMesh() {
            foreach (GameObject mesh in _meshList) {
                if (_manager.Data.Mesh.GetComponent<MeshTag>().Tag == mesh.GetComponent<MeshTag>().Tag) {
                    mesh.SetActive(true);
                }
            }
        }

        private void ResetMesh() {
           foreach (GameObject mesh in _meshList) {
            mesh.SetActive(false);
           }
        }

        private void OnSetup() {
            SetupMesh();
        }

        private void OnClear() {
            ResetMesh();
        }
    }
}

