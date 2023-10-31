using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace EasyDragAndDrop.Core
{
    public class DropSlot : MonoBehaviour, IDropHandler
    {
        [Header("Set-up Variable")] public string mainCanvasName = "Canvas";

        [Header("Set Drop Object Parent")] public bool setDropObjParent;
        public GameObject parentLocation;

        [Header("Set Drop Object Position")] public bool setDropObjPosition = true;

        [Header("On Drop Event")] public DragAndDropEvent2D onDrop;

        [FormerlySerializedAs("DropObj")] [HideInInspector]
        public DragObj2D dropObj2D;

        private void Awake()
        {
            if (parentLocation == null)
                parentLocation = GameObject.Find(mainCanvasName);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (!eventData.pointerDrag.TryGetComponent(typeof(DragObj2D), out var draggableObj))
                return;

            SetDropObj((DragObj2D) draggableObj);
        }

        public void SetDropObj(DragObj2D dragObj2D)
        {
            dropObj2D = dragObj2D;
            dropObj2D.m_dropSlot = this;
            SetDragObjParent();
            SetDragObjPosition();
            onDrop?.Invoke(dropObj2D);
        }

        private void SetDragObjParent()
        {
            if (!setDropObjParent) return;
            dropObj2D.SetDropParent(parentLocation.transform);
            dropObj2D.transform.SetAsLastSibling();
        }

        private void SetDragObjPosition()
        {
            if (setDropObjPosition)
            {
                dropObj2D.isReturnPosition = false;
                dropObj2D.SetRectPosition(GetComponent<RectTransform>().position);
            }
            else
            {
                dropObj2D.SetInitialPosition();
            }
        }
    }
}