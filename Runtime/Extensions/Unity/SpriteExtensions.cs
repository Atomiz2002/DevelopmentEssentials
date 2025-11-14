// using UnityEditor;
// using UnityEngine;
//
// namespace _ProjectBase.Extensions.Unity {
//
//     public static class SpriteExtensions {
//
// #if UNITY_EDITOR
//         public static Texture2D Texture(this Sprite sprite) {
//             if (!sprite.texture) return new(0, 0);
//
//             Texture2D preview = AssetPreview.GetAssetPreview(sprite);
//
//             if (preview)
//                 preview.filterMode = FilterMode.Point;
//
//             return preview;
//         }
// #endif
//
//     }
//
// }

