using BepInEx.Logging;
using UnityEngine;

namespace CustomizableUI
{
    public class ChildGroup : BaseGroup
    {
        private ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("CustomizableUI.ChildGroup");

        public BaseGroup Parent;        

        public ChildGroup() { }

        public ChildGroup(Transform transform, BaseGroup parent)
        {
            Name = Utility.Instance.GetGroupName(transform.name);
            Transform = transform;
            Parent = parent;
            Children = ChildGroup.GetAllChildren(transform, this);
            DefaultPosition = Position = Transform.position;
            IsActive = Transform.gameObject.activeSelf;
        }

        public static List<ChildGroup> GetAllChildren(Transform transform, BaseGroup parent)
        {
            List<ChildGroup> children = null;
            
            if (transform.childCount > 0)
            {
                children = new List<ChildGroup>();

                for (int i = 0; i < transform.childCount; i++)
                {
                    children.Add(new ChildGroup(transform.GetChild(i), parent));
                }
            }

            return children;
        }

        public void LoadData(ChildGroup loadedGroup)
        {
            Transform.position = loadedGroup.Position;
            IsActive = loadedGroup.IsActive;

            try
            {
                if (Children != null && loadedGroup.Children != null && Children.Count == loadedGroup.Children.Count)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        Children[i].LoadData(loadedGroup.Children[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error loading data from child groups. Children data won't be changed.\n{ex}");
            }
        }
    }
}
