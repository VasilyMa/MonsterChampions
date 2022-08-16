using UnityEngine;

namespace Client {
    struct DragCardEvent {
        public GameObject CardObject;
        public Vector3 DefaultPos;
        public Transform DefaultParent;
    }
}