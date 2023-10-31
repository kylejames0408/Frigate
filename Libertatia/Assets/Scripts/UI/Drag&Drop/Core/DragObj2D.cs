using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyDragAndDrop.Core
{
    public class DragObj2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Header("Set-up Variable")] public string mainCanvasName = "Canvas";
        public bool isReturnPosition = true;
        public bool isDropSwap = true;
        public bool dropAsFirstSibling = false;
        public Component m_ObjInfoComponent;
        
        [Header("Drag Event")] 
        public DragAndDropEvent2D onBeginDrag;
        public DragAndDropEvent2D onDrag;
        public DragAndDropEvent2D onEndDrag;
        
        [HideInInspector] 
        public DragState dragState;

        [HideInInspector] public DropSlot m_dropSlot = null;

        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;
        private RectTransform _rectTransform;
        private Transform _dropParent;
        private Vector2 m_InitialPosition;
        private bool _initialIsReturnPos;

        private void Awake()
        {
            if (m_Canvas == null)
            {
                m_Canvas = GameObject.Find(mainCanvasName).GetComponent<Canvas>();
            }

            _rectTransform = GetComponent<RectTransform>();
            m_CanvasGroup = transform.gameObject.AddComponent<CanvasGroup>();
            _dropParent = transform.parent;
            m_InitialPosition = _rectTransform.position;
            _initialIsReturnPos = isReturnPosition;
            dragState = DragState.BeforeDrag;
        }

        public void Initialize(Component component, string canvasName)
        {
            m_ObjInfoComponent = component;
            m_Canvas = GameObject.Find(canvasName).GetComponent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_dropSlot = null;
            dragState = DragState.OnBeginDrag;
            m_CanvasGroup.alpha = .6f;
            m_CanvasGroup.blocksRaycasts = false;
            transform.SetParent(m_Canvas.transform);
            isReturnPosition = _initialIsReturnPos;
            m_InitialPosition = _rectTransform.position;
            onBeginDrag?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                dragState = DragState.OnDrag;
                _rectTransform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;
                onDrag?.Invoke(this);
                //Debug.Log("Delta: " + eventData.delta);
            }
            else if(m_Canvas.renderMode == RenderMode.WorldSpace)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Canvas.transform as RectTransform, eventData.delta, null, out pos);
                Vector3 v3Pos = m_Canvas.transform.TransformPoint(pos);
                
                _rectTransform.anchoredPosition += new Vector2(v3Pos.x, v3Pos.y) / 1.8f /*m_Canvas.scaleFactor*/;
                onDrag?.Invoke(this);
            }   
        }
    

        public void OnEndDrag(PointerEventData eventData)
        {
            dragState = DragState.OnEndDrag;

            if (isReturnPosition)
                SetInitialPosition();

            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.blocksRaycasts = true;
            transform.SetParent(_dropParent);
            transform.SetAsLastSibling();
            if (dropAsFirstSibling)
                transform.SetAsFirstSibling();

            onEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!isDropSwap) return;

            if (m_dropSlot == null) return;

            if (!eventData.pointerDrag.TryGetComponent(typeof(DragObj2D), out var draggableObj))
                return;

            SetInitialPosition();
            m_dropSlot.SetDropObj((DragObj2D)draggableObj);
        }

        public void SetDropParent(Transform trans)
        {
            _dropParent = trans;
        }

        public void SetRectPosition(Vector2 position)
        {
            _rectTransform.position = position;
        }

        public void SetInitialPosition()
        {
            _rectTransform.position = m_InitialPosition;
        }
    }
}