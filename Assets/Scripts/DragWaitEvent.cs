using UnityEngine;

namespace Client {
    struct DragWaitEvent {
        public GameObject CardObject;
        public Vector3 DefaultPos;
        public Transform DefaultParent;
        public float timerDrag;
        public Vector3 dragPosition;
    }
}