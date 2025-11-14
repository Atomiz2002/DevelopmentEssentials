using System;
using DevelopmentEssentials.Extensions.CS;
using JetBrains.Annotations;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class TextureExtensions {

        /// Trims the pixels of the specified color (default = transparent) from a Texture, returning a new Texture2D.
        public static Texture2D Trimmed(this Texture texture, bool uniform = false, Color color = default) =>
            !texture ? null : Trimmed(texture.Read(), uniform, color);

        public static Texture2D Trimmed(this Texture2D texture, bool uniform = false, Color color = default) {
            if (!texture)
                return null;

            Color[] pixels = texture.GetPixels();

            int width  = texture.width;
            int height = texture.height;

            int xMin = width,  xMax = 0;
            int yMin = height, yMax = 0;

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++) {
                Color pixel = pixels[x + y * width];

                if (pixel == color) continue;
                if (pixel.a == 0 && color.a == 0) continue;

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

            if (uniform) {
                if (w < h)
                    xMin -= (h - w) / 2;
                else if (h < w)
                    yMin -= (w - h) / 2;

                w = h = Math.Max(w, h);
            }

            Texture2D trimmedTexture = new(w, h);
            Color[]   finalPixels    = new Color[w * h];

            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++) {
                try {
                    finalPixels[x + y * w] = pixels[(x + xMin) + (y + yMin) * width];
                }
                catch (IndexOutOfRangeException) {
                    finalPixels[x + y * w] = Color.clear;
                }
            }

            trimmedTexture.SetPixels(finalPixels);
            trimmedTexture.Apply();

            return trimmedTexture;
        }

        public static void Trim(this Texture texture, bool uniform = false) {
            if (texture is Texture2D t)
                t.Trim(uniform);
            else
                texture.Read().Trim(uniform);
        }

        public static void Trim(this Texture2D texture, bool uniform = false) {
            Texture2D newTexture = Trimmed(texture, uniform);
            texture.Reinitialize(newTexture.width, newTexture.height);
            texture.SetPixels(newTexture.GetPixels());
            texture.Apply();
            newTexture.DestroySmart();
        }

        /// Supposedly better for runtime and useless in editor
        [Pure]
        public static Texture2D ReadGPU(this RenderTexture texture) {
            Texture2D t = new(texture.width, texture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(texture, t);
            return t;
        }

        [Pure]
        public static Texture2D Read(this RenderTexture texture) {
            Texture2D tex = new(texture.width, texture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = texture;

            tex.ReadPixels(new(0, 0, texture.width, texture.height), 0, 0);
            tex.Apply();

            RenderTexture.active = null;
            return tex;
        }

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

        [Pure]
        public static Texture2D Read(this Texture texture, Rect rect = default) {
            if (!texture) return null;

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

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            return result;
        }

        public static Texture2D Underlay(this Texture2D texture, Color bg) {
            if (!texture) return null;

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
        public static Sprite ToSprite(this Texture2D texture) =>
            Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(.5f, .5f));

        [Pure]
        public static Texture2D ToTexture2D(this Sprite sprite) {
            if (!sprite || !sprite.texture) return null;

            return sprite.texture.Read(sprite.rect);
        }

    }

}