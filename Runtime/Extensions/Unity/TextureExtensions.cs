using System;
using System.Linq;
using DevelopmentEssentials.Extensions.CS;
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DevelopmentEssentials.Extensions.Unity {

    public static class TextureExtensions {

        public static void CaptureSnapshot(this Camera camera, out Texture2D texture, int width = 0, int height = 0, bool trim = false, bool pointFilter = false) =>
            texture = camera.CaptureSnapshot(width, height, trim, pointFilter);

        public static Texture2D CaptureSnapshot(this Camera camera, int width = 0, int height = 0, bool trim = false, bool pointFilter = false) {
#if DEVELOPMENT_TOOLS_EDITOR_UNITY_URP
            float renderScale = UniversalRenderPipeline.asset.renderScale;
            UniversalRenderPipeline.asset.renderScale = 1;
#endif
            bool cameraInitiallyEnabled = camera.isActiveAndEnabled;

            camera.SetActive(true);

            if (width == 0) width   = Screen.width;
            if (height == 0) height = (int) ((float) Screen.height / Screen.width * width);

            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            Texture2D     snapshot      = new(width, height, TextureFormat.ARGB32, false);

            if (pointFilter)
                snapshot.filterMode = FilterMode.Point;

            camera.targetTexture          = renderTexture;
            camera.forceIntoRenderTexture = true;

            camera.Render();

            RenderTexture.active = renderTexture;

            snapshot.ReadPixels(new(0, 0, width, height), 0, 0); // Reads from RenderTexture.active
            snapshot.Apply();

            if (trim)
                snapshot.Trim();

            RenderTexture.active          = null;
            camera.targetTexture          = null;
            camera.forceIntoRenderTexture = false;
            RenderTexture.ReleaseTemporary(renderTexture);

            camera.SetActive(cameraInitiallyEnabled);

#if DEVELOPMENT_TOOLS_EDITOR_UNITY_URP
            UniversalRenderPipeline.asset.renderScale = renderScale;
#endif
            return snapshot;
        }

        /// Trims the pixels from the texture, returning a new Texture2D.
        /// <param name="trimColors">defaults to <c>pixel.a == 0</c></param>
        public static Texture2D Trimmed(this Texture texture, bool square = false, Func<Color, bool> trimColors = null) =>
            !texture ? null : texture.Read().Trimmed(square, trimColors);

        /// <inheritdoc cref="Trimmed(UnityEngine.Texture,bool,System.Func{UnityEngine.Color,bool})"/>
        public static Texture2D Trimmed(this Texture2D texture, bool square = false, Func<Color, bool> trimColors = null) {
            if (!texture || !texture.isReadable)
                return null;

            trimColors ??= c => c.a == 0;

            Color[] pixels = texture.GetPixels();

            int width  = texture.width;
            int height = texture.height;

            int xMin = width,  xMax = 0;
            int yMin = height, yMax = 0;

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++) {
                Color pixel = pixels[x + y * width];

                if (trimColors.InvokeSafe(pixel))
                    continue;

                if (x < xMin) xMin = x;
                if (x > xMax) xMax = x;
                if (y < yMin) yMin = y;
                if (y > yMax) yMax = y;
            }

            int w = xMax - xMin + 1;
            int h = yMax - yMin + 1;

            if (w <= 0 || h <= 0) {
                // Debug.LogError("Trimmed texture resulted in a 0x0 size.");
                return null; // or clear texture
            }

            if (square) {
                if (w < h)
                    xMin -= (h - w) / 2;
                else if (h < w)
                    yMin -= (w - h) / 2;

                w = h = Math.Max(w, h);
            }

            Texture2D trimmedTexture = new(w, h) {
                filterMode          = texture.filterMode,
                anisoLevel          = texture.anisoLevel,
                alphaIsTransparency = texture.alphaIsTransparency
            };

            Color[] finalPixels = new Color[w * h];

            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++) {
                int sourceX = xMin + x;
                int sourceY = yMin + y;

                if (sourceX >= 0 && sourceX < width && sourceY >= 0 && sourceY < height) {
                    finalPixels[x + y * w] = pixels[sourceX + sourceY * width];
                }
                else {
                    finalPixels[x + y * w] = Color.clear;
                }
            }

            trimmedTexture.SetPixels(finalPixels);
            trimmedTexture.Apply();

            return trimmedTexture;
        }

        /// Trims the pixels from the texture.
        /// <param name="trimColors">defaults to <c>pixel.a == 0</c></param>
        public static void Trim([CanBeNull] this Texture texture, bool uniform = false, Func<Color, bool> trimColors = null) {
            if (!texture)
                return;

            if (texture.Is(out Texture2D t))
                t.Trim(uniform);
            else
                texture.Read().Trim(uniform, trimColors);
        }

        /// <inheritdoc cref="Trim"/>
        public static void Trim([CanBeNull] this Texture2D texture, bool uniform = false, Func<Color, bool> trimColors = null) {
            if (!texture || !texture.isReadable)
                return;

            Texture2D newTexture = texture.Trimmed(uniform, trimColors);
            texture.Reinitialize(newTexture.width, newTexture.height);
            texture.SetPixels(newTexture.GetPixels());
            texture.Apply();
            newTexture.DestroySmart();
        }

        public static T SetFilter<T>([CanBeNull] this T texture, FilterMode mode) where T : Texture {
            if (texture)
                texture.filterMode = mode;

            return texture;
        }

        /// Supposedly better for runtime and useless in editor
        [Pure]
        public static Texture2D ReadGPU([CanBeNull] this RenderTexture texture) {
            if (!texture)
                return null;

            Texture2D t = new(texture.width, texture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(texture, t);
            return t.SetFilter(texture.filterMode);
        }

        [Pure]
        public static Texture2D Read([CanBeNull] this RenderTexture texture) {
            if (!texture)
                return null;

            Texture2D tex = new(texture.width, texture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = texture;

            tex.ReadPixels(new(0, 0, texture.width, texture.height), 0, 0);
            tex.Apply();

            RenderTexture.active = null;
            return tex;
        }

        // TODO Func<Color, Color> pixelColorModifier = null (replace/remove select pixels)
        [Pure]
        public static Texture2D Read([CanBeNull] this Texture texture, Rect rect = default) {
            if (!texture)
                return null;

            if (rect == default)
                rect = new(0, 0, texture.width, texture.height);

            int w = Mathf.CeilToInt(rect.width).Replace(texture.width);
            int h = Mathf.CeilToInt(rect.height).Replace(texture.height);

            Vector2 offset = new(rect.xMin / texture.width, rect.yMin / texture.height);
            Vector2 scale  = new(rect.width / texture.width, rect.height / texture.height);

            Texture2D     result        = new(w, h, TextureFormat.ARGB32, false);
            RenderTexture renderTexture = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.ARGB32);
            RenderTexture.active = renderTexture;

            Graphics.Blit(texture, renderTexture, scale, offset);

            result.ReadPixels(new(0, 0, w, h), 0, 0);
            result.Apply();
            result.SetFilter(texture.filterMode);

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);

            return result;
        }

        [Pure]
        public static Vector2Int Dimensions(this Texture2D tex) => new(tex.width, tex.height);

        private static Material underlayMaterial;

        private static Material UnderlayMaterial {
            get {
                if (underlayMaterial)
                    return underlayMaterial;

                Shader shader = Shader.Find("Sprites/Default");

                if (!shader) {
                    Debug.LogError("Required blending shader not found.");
                    return null;
                }

                underlayMaterial = new(shader);

                return underlayMaterial;
            }
        }

        public static Texture2D Underlay([CanBeNull] this Texture2D texture, Color bg) {
            if (!texture)
                return null;

            RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height);
            RenderTexture.active = rt;

            GL.Clear(true, true, bg.linear);
            Graphics.Blit(texture, rt, UnderlayMaterial);

            texture.ReadPixels(new(0, 0, rt.width, rt.height), 0, 0);
            texture.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return texture;
        }

        [Pure]
        public static Sprite ToSprite([CanBeNull] this Texture2D texture, Rect rect = default, Vector2? pivot = null, float ppu = 100f) {
            if (!texture)
                return null;

#if UNITY_EDITOR && !SIMULATE_BUILD
            string          path     = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
                ppu = importer.spritePixelsPerUnit;
#endif

            if (rect == default)
                rect = new(0, 0, texture.width, texture.height);

            return Sprite.Create(texture, rect, pivot ?? new(.5f, .5f), ppu);
        }

        [Pure]
        public static Texture2D ToTexture2D([CanBeNull] this Sprite sprite) {
            if (!sprite || !sprite.texture)
                return null;

            return sprite.texture.Read(sprite.rect);
        }

    }

}