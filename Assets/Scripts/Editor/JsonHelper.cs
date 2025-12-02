#if UNITY_EDITOR
using System.IO;
using GameContext.TicketHunt;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Utils/JsonHelper")]
    public class JsonHelper : ScriptableObject
    {
        public TextAsset asset;
        public TicketHuntData data;
        
        

        [Button]
        public void Load()
        {
            data = JsonUtility.FromJson<TicketHuntData>(asset.text);
        }
        
        [Button]
        public void Save()
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(AssetDatabase.GetAssetPath(asset), json);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif