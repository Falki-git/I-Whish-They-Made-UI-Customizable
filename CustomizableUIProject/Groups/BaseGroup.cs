using Newtonsoft.Json;
using UnityEngine;

namespace CustomizableUI
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseGroup
    {
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

        public void MoveToHorizontalCenter()
        {
            Transform.position = new Vector3(0f + OffsetToZero.x + Screen.width/2 - RectTransform.rect.width/2 + ToCenterOffset.x, Transform.position.y, Transform.position.z);
        }

        public void MoveToVerticalCenter()
        {
            Transform.position = new Vector3(Transform.position.x, 0f + OffsetToZero.y + Screen.height/2 - RectTransform.rect.height/2 +ToCenterOffset.y, Transform.position.z);
        }

        public void MoveToTop()
        {
            // TODO
        }
        public void MoveToBottom()
        {
            // TODO
        }

        public void MoveToFarLeft()
        {
            // TODO
        }

        public void MoveToFarRight()
        {
            // TODO
        }

        public void AdjustScale()
        {
            // TODO
        }
    }
}
