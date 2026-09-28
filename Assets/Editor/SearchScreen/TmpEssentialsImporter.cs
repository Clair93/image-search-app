using TMPro;
using UnityEditor;

namespace ImageSearch.EditorTools
{
    public static class TmpEssentialsImporter
    {
        [MenuItem("ImageSearch/Import TMP Essentials (Non-Interactive)")]
        public static void Import()
        {
            TMP_PackageResourceImporter.ImportResources(
                importEssentials: true,
                importExamples: false,
                interactive: false);
        }
    }
}
