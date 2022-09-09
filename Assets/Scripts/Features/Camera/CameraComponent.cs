using UnityEngine;

namespace Client
{
    struct CameraComponent
    {
        public GameObject CameraObject;
        public GameObject HolderObject;
        public Transform CameraTransform;
        public Transform HolderTransform;
        public AnimationCurve CameraAnimationCurve;
        public Camera Camera;

        public bool isOnBoard;
    }
}