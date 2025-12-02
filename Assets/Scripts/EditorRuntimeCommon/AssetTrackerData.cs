using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace EditorRuntimeCommon
{
    public static class AssetTrackerData
    {
        public struct TrackedAssetData
        {
            [TableColumnWidth(57, Resizable = true)]
            public string address;
            [TableColumnWidth(57, Resizable = true)]
            public int requestAmount;
            [TableColumnWidth(57, Resizable = true)]
            public bool isLoaded;
        }
        
        public static List<TrackedAssetData> TrackedAssets = new List<TrackedAssetData>();
        public static string ActiveAssetLoader;
        
        public static void UpdateEntry(string address, int requestAmount, bool isLoaded)
        {
            // Find the index of the existing entry if it exists and update it, otherwise add a new entry
            var index = TrackedAssets.FindIndex(x => x.address == address);
            if (index != -1)
            {
                TrackedAssets[index] = new TrackedAssetData
                {
                    address = address,
                    requestAmount = requestAmount,
                    isLoaded = isLoaded
                };
            }
            else
            {
                TrackedAssets.Add(new TrackedAssetData
                {
                    address = address,
                    requestAmount = requestAmount,
                    isLoaded = isLoaded
                });
            }
        }
    }
}