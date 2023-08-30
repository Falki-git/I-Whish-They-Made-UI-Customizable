using Newtonsoft.Json;
using UnityEngine;

namespace CustomizableUI
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseGroup
    {
        private Vector3 _moveLeft = new Vector3 (-1f, 0f, 0f);
        private Vector3 _moveRight = new Vector3(1f, 0f, 0f);
        private Vector3 _moveUp = new Vector3(0f, 1f, 0f);
        private Vector3 _moveDown = new Vector3(0f, -1f, 0f);

        [JsonProperty]
        public string Name;
        public Transform Transform;
        [JsonProperty]
        public List<ChildGroup> Children;
        public int SelectedChild = 0;
        public Vector3 DefaultPosition;
        public Vector2 ToCenterOffset;
        public Vector2 OffsetToZero;
        public Vector2 ToMaxOffset;
        [JsonProperty]
        public Vector3 Position;
        [JsonProperty]
        private bool isActive;

        public RectTransform RectTransform;

        public float HorizontalLeftFar { get => (float)Math.Round(0f + OffsetToZero.x, 0); }
        public float HorizontalLeftMiddle { get => (float)Math.Round((HorizontalLeftFar + HorizontalCenter) / 2, 0); }
        public float HorizontalCenter { get => (float)Math.Round(0f + OffsetToZero.x + Screen.width / 2 - RectTransform.rect.width / 2 * Manager.Instance.ScaleFactor + ToCenterOffset.x, 0); }
        public float HorizontalRightMiddle { get => (float)Math.Round((HorizontalCenter + HorizontalRightFar) / 2, 0); }
        public float HorizontalRightFar { get => (float)Math.Round(0f + OffsetToZero.x + Screen.width - RectTransform.rect.width * Manager.Instance.ScaleFactor + ToMaxOffset.x, 0); }
        public float VerticalUpperTop { get => (float)Math.Round(0f + OffsetToZero.y + Screen.height - RectTransform.rect.height * Manager.Instance.ScaleFactor + ToMaxOffset.y, 0); }
        public float VerticalUpperMiddle { get => (float)Math.Round((VerticalCenter + VerticalUpperTop) / 2, 0); } 
        public float VerticalCenter { get => (float)Math.Round(0f + OffsetToZero.y + Screen.height / 2 - RectTransform.rect.height / 2 * Manager.Instance.ScaleFactor + ToCenterOffset.y, 0); }
        public float VerticalLowerMiddle { get => (float)Math.Round((VerticalLowerBottom + VerticalCenter) / 2, 0); }
        public float VerticalLowerBottom { get => (float)Math.Round(0f + OffsetToZero.y, 0); }

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                Transform?.gameObject?.SetActive(value);
            }
        }

        public override string ToString()
        {
            return Name ?? String.Empty;
        }

        public void SelectPreviousChild()
        {
            if (Children != null && SelectedChild > 0)
                SelectedChild--;
        }

        public void SelectNextChild()
        {
            if (Children != null && Children.Count > SelectedChild+1)
                SelectedChild++;
        }

        public void ResetToDefault()
        {
            Transform.position = Position = new Vector3(DefaultPosition.x, DefaultPosition.y, DefaultPosition.z);
            IsActive = true;
        }


        ///// MOVE METHODS /////

        public void MoveToHorizontalLeftFar()
        {
            Transform.position = new Vector3(this.HorizontalLeftFar, Transform.position.y, Transform.position.z);
        }

        public void MoveToHorizontalLeftMiddle()
        {
            Transform.position = new Vector3(this.HorizontalLeftMiddle, Transform.position.y, Transform.position.z);
        }

        public void MoveToHorizontalCenter()
        {
            Transform.position = new Vector3(this.HorizontalCenter, Transform.position.y, Transform.position.z);
        }

        public void MoveToHorizontalRightMiddle()
        {
            Transform.position = new Vector3(this.HorizontalRightMiddle, Transform.position.y, Transform.position.z);
        }

        public void MoveToHorizontalRightFar()
        {
            Transform.position = new Vector3(this.HorizontalRightFar, Transform.position.y, Transform.position.z);
        }

        public void MoveToVerticalUpperTop()
        {
            Transform.position = new Vector3(Transform.position.x, this.VerticalUpperTop, Transform.position.z);
        }

        public void MoveToVerticalUpperMiddle()
        {
            Transform.position = new Vector3(Transform.position.x, this.VerticalUpperMiddle, Transform.position.z);
        }

        public void MoveToVerticalCenter()
        {
            Transform.position = new Vector3(Transform.position.x, this.VerticalCenter, Transform.position.z);
        }

        public void MoveToVerticalLowerMiddle()
        {
            Transform.position = new Vector3(Transform.position.x, this.VerticalLowerMiddle, Transform.position.z);
        }
        
        public void MoveToVerticalLowerBottom()
        {
            Transform.position = new Vector3(Transform.position.x, this.VerticalLowerBottom, Transform.position.z);
        }


        ///// NUDGE METHODS /////

        public void NudgeLeft()
        {
            Transform.position += _moveLeft;
        }

        public void NudgeRight()
        {
            Transform.position += _moveRight;
        }

        public void NudgeUp()
        {
            Transform.position += _moveUp;
        }

        public void NudgeDown()
        {
            Transform.position += _moveDown;
        }

        public void AdjustScale()
        {
            // TODO
        }
    }
}
