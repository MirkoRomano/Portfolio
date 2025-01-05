using UnityEngine;

namespace Portfolio.MainMenu
{
    [CreateAssetMenu(fileName = "Item Menu", menuName = "ScriptableObjects/ItemMenu")]
    public class ScriptableMenuItem : ScriptableObject
    {
        /// <summary>
        /// Game name
        /// </summary>
        public string Name;

        /// <summary>
        /// Game description
        /// </summary>
        [TextArea]
        public string Description;

        /// <summary>
        /// Game players count
        /// </summary>
        public int PlayersCount;

        /// <summary>
        /// Game scene
        /// </summary>
        public SceneField Scene;
    }
}