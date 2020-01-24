using UnityEditor;
using System.IO;

/// <summary>
/// This class overvwrites automaticly import type of all images form directory AtomaticIcons.
/// </summary>
public class AutomaticIconsPostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.Contains(string.Concat("Resources", Path.DirectorySeparatorChar, "Icons")))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
        }
    }
}
