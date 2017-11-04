using Telerik.Windows.Controls;

namespace FBFCheckManagement.WPF.HelperClass
{
    public class ScheduleDragBehavior : ScheduleViewDragDropBehavior
    {
        public override bool CanStartDrag(DragDropState state)
        {
            return false;
        }

        public override bool CanDrop(DragDropState state)
        {
            return false;
        }

        public override bool CanStartResize(DragDropState state)
        {
            return false;
        }

        public override bool CanResize(DragDropState state)
        {
            return false;
        }
    }
}