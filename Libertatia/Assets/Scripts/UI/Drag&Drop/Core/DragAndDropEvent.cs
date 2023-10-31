using System;
using UnityEngine.Events;

namespace EasyDragAndDrop.Core
{
    [Serializable] public class DragAndDropEvent2D : UnityEvent<DragObj2D> { }
    [Serializable] public class DragAndDropEvent3D : UnityEvent<DragObj3D> { }
}
