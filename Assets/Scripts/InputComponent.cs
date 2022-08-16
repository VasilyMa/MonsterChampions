using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client {
    struct InputComponent {
        public GraphicRaycaster Raycaster;
        public PointerEventData PointerEventData;
        public EventSystem EventSystem;
    }
}