using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace ImageSearch.EditorTools
{
    public static class KoreanFontAssetSetup
    {
        private const string AssetPath = "Assets/Fonts/MalgunGothicSDF.asset";

        [MenuItem("ImageSearch/Setup Korean Font Asset")]
        public static TMP_FontAsset EnsureKoreanFontAsset()
        {
            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetPath);
            if (existing != null)
            {
                TMP_Settings.defaultFontAsset = existing;
                return existing;
            }

            var fontAsset = TMP_FontAsset.CreateFontAsset("Malgun Gothic", "Regular");
            if (fontAsset == null)
            {
                Debug.LogError("[KoreanFontAssetSetup] Could not find 'Malgun Gothic' on this machine.");
                return null;
            }

            Directory.CreateDirectory("Assets/Fonts");
            AssetDatabase.CreateAsset(fontAsset, AssetPath);

            if (fontAsset.atlasTexture != null)
            {
                fontAsset.atlasTexture.name = "Atlas";
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAsset);
            }

            if (fontAsset.material != null)
            {
                fontAsset.material.name = "Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);

            TMP_Settings.defaultFontAsset = fontAsset;
            return fontAsset;
        }
    }
}
