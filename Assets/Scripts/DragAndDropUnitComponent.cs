using UnityEngine;
namespace Client {
    struct DragAndDropUnitComponent {
        public int entity;
        public Vector3 defaultPos;
        public Quaternion defaultRot;
        public Transform defaultParent;
        public GameObject unit;
    }
}