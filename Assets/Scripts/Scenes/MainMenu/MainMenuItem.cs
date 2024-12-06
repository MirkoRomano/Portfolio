using UnityEngine;

namespace Portfolio
{
    public interface ISceneInfo
    {
        string GetSceneName();
    }

    public interface IRoteable
    {
        bool CanRotate { set; get; }
    }

    public interface IItemInfo
    {
        string Name { get; }
        string Description { get; }
        int PlayerCount { get; }
    }

    public class MainMenuItem : MonoBehaviour, ISceneInfo, IItemInfo, IRoteable
    {
        /// <summary>
        /// Menu info
        /// </summary>
        [SerializeField]
        private ScriptableMenuItem menuItemInfo;

        /// <summary>
        /// Allow the object to be roteable
        /// </summary>
        public bool CanRotate { set; get; }

        /// <summary>
        /// Item name
        /// </summary>
        string IItemInfo.Name => menuItemInfo.Name;
        
        /// <summary>
        /// Item description
        /// </summary>
        string IItemInfo.Description => menuItemInfo.Description;

        /// <summary>
        /// Player count
        /// </summary>
        int IItemInfo.PlayerCount => menuItemInfo.PlayersCount;

        /// <summary>
        /// Scene name
        /// </summary>
        /// <returns></returns>
        string ISceneInfo.GetSceneName()
        {
            return menuItemInfo.Scene.SceneName;
        }

        private Vector2 currentMousePosition => Input.mousePosition;
        
        private Vector2 lastMousePosition;

        private Vector2 mouseDelta;

        private void Awake()
        {
            lastMousePosition = Vector2.zero;
            mouseDelta = Vector2.zero;
        }

        private void Update()
        {
            mouseDelta = lastMousePosition - currentMousePosition;
            lastMousePosition = currentMousePosition;

            if (!CanRotate)
            {
                return;
            }


        }
    }
}