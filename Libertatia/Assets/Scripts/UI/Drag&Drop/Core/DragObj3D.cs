using UnityEngine;

namespace EasyDragAndDrop.Core
{
    public class DragObj3D : MonoBehaviour
    {
        [Header("Freeze Drag Axis")] public bool freezeX;
        public bool freezeY;
        public bool freezeZ;

        [Header("Offset Axis When Drag An Object")]
        public float xPositionOffset;

        public float yPositionOffset;
        public float zPositionOffset;

        [Tooltip("Enable Rigidbody Kinematic When Drag")]
        public bool kinematicOnDrag;

        [Tooltip("Object Model Info Component")]
        public Component m_ObjInfoComponent;

        [Header("Drag Event")] public DragAndDropEvent3D onBeginDrag;
        public DragAndDropEvent3D onDrag;
        public DragAndDropEvent3D onEndDrag;

        [HideInInspector] public DragState dragState;

        private Camera _mainCamera;
        private Vector3 _mOffset;
        private float _mZCoord;

        private float _xPosition;
        private float _yPosition;
        private float _zPosition;

        private void Start()
        {
            if (!Camera.main)
            {
                Debug.LogError("Easy Drag And Drop: Main Camera Does Not Exists");
                return;
            }

            _mainCamera = Camera.main;
        }

        public void SetMainCamera(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        private void OnMouseDown()
        {
            var position = gameObject.transform.position;
            _xPosition = position.x;
            _yPosition = position.y;
            _zPosition = position.z;

            _mOffset = position - CalculateObjPosition();

            if (kinematicOnDrag)
            {
                if (TryGetComponent<Rigidbody>(out var rigidbodyComponent))
                    rigidbodyComponent.isKinematic = true;
            }

            dragState = DragState.OnBeginDrag;
            onBeginDrag?.Invoke(this);
        }

        private void OnMouseDrag()
        {
            dragState = DragState.OnDrag;
            transform.position = CalculateObjPosition() + _mOffset;
            FreezePositionOnDrag();
            onDrag?.Invoke(this);
        }

        private void OnMouseUp()
        {
            ReturnPositionOffset();

            if (kinematicOnDrag)
            {
                if (TryGetComponent<Rigidbody>(out var rigidbodyComponent))
                    rigidbodyComponent.isKinematic = false;
            }

            dragState = DragState.OnEndDrag;
            onEndDrag?.Invoke(this);
        }

        private Vector3 CalculateObjPosition()
        {
            _mZCoord = _mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
            var mousePoint = Input.mousePosition;
            mousePoint.z = _mZCoord;
            return _mainCamera.ScreenToWorldPoint(mousePoint);
        }

        private void FreezePositionOnDrag()
        {
            if (freezeX)
            {
                var position = new Vector3(_xPosition + xPositionOffset, transform.position.y, transform.position.z);
                transform.position = position;
            }


            if (freezeY)
            {
                var position = new Vector3(transform.position.x, _yPosition + yPositionOffset, transform.position.z);
                transform.position = position;
            }

            if (freezeZ)
            {
                var position = new Vector3(transform.position.x, transform.position.y, _zPosition + zPositionOffset);
                transform.position = position;
            }
        }

        private void ReturnPositionOffset()
        {
            if (freezeX)
            {
                transform.position = new Vector3(_xPosition, transform.position.y, transform.position.z);
            }

            if (freezeY)
            {
                transform.position = new Vector3(transform.position.x, _yPosition, transform.position.z);
            }

            if (freezeZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, _zPosition);
            }
        }
    }
}