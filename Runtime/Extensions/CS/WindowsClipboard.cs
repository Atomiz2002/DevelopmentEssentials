#if UNITY_EDITOR && !SIMULATE_BUILD
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.CS {

    public static class WindowsClipboard {

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint RegisterClipboardFormat(string lpszFormat);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        private const uint CF_DIBV5      = 17;
        private const uint GMEM_MOVEABLE = 0x0002;

        public static Texture2D CopyTextureToClipboard(this Texture2D texture) {
            if (texture == null)
                return texture;

            // 1. Target Apps: Keep raw PNG data with original transparent pixels intact
            byte[] pngBytes   = texture.EncodeToPNG();
            IntPtr hGlobalPng = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr) pngBytes.Length);
            IntPtr ptrPng     = GlobalLock(hGlobalPng);
            Marshal.Copy(pngBytes, 0, ptrPng, pngBytes.Length);
            GlobalUnlock(hGlobalPng);
            uint cfPng = RegisterClipboardFormat("PNG");

            // 2. Win + V Preview: Blend pixels onto a neutral background to prevent rendering artifacts
            Color32[] pixels = texture.GetPixels32();
            int       width  = texture.width;
            int       height = texture.height;

            int headerSize   = 124;
            int imageSize    = width * height * 4;
            int totalDibSize = headerSize + imageSize;

            byte[] bmpData = new byte[totalDibSize];

            Array.Copy(BitConverter.GetBytes(headerSize), 0, bmpData, 0, 4);
            Array.Copy(BitConverter.GetBytes(width), 0, bmpData, 4, 4);
            Array.Copy(BitConverter.GetBytes(height), 0, bmpData, 8, 4);
            Array.Copy(BitConverter.GetBytes((short) 1), 0, bmpData, 12, 2);
            Array.Copy(BitConverter.GetBytes((short) 32), 0, bmpData, 14, 2);
            Array.Copy(BitConverter.GetBytes(3), 0, bmpData, 16, 4);
            Array.Copy(BitConverter.GetBytes(imageSize), 0, bmpData, 20, 4);

            Array.Copy(BitConverter.GetBytes(0x00FF0000), 0, bmpData, 40, 4);
            Array.Copy(BitConverter.GetBytes(0x0000FF00), 0, bmpData, 44, 4);
            Array.Copy(BitConverter.GetBytes(0x000000FF), 0, bmpData, 48, 4);
            Array.Copy(BitConverter.GetBytes(0xFF000000), 0, bmpData, 52, 4);
            Array.Copy(BitConverter.GetBytes(0x73524742), 0, bmpData, 56, 4);

            // Win + V preview background
            Color32 bgColor = new(0, 0, 0, 0);

            int index = headerSize;

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    Color32 pixel = pixels[y * width + x];

                    // Manual alpha-blending math over our solid bgColor
                    float alpha = pixel.a / 255f;
                    byte  r     = (byte) (pixel.r * alpha + bgColor.r * (1f - alpha));
                    byte  g     = (byte) (pixel.g * alpha + bgColor.g * (1f - alpha));
                    byte  b     = (byte) (pixel.b * alpha + bgColor.b * (1f - alpha));

                    bmpData[index]     =  b;
                    bmpData[index + 1] =  g;
                    bmpData[index + 2] =  r;
                    bmpData[index + 3] =  0; // Win + V preview background alpha
                    index              += 4;
                }
            }

            IntPtr hGlobalDib = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr) totalDibSize);
            IntPtr ptrDib     = GlobalLock(hGlobalDib);
            Marshal.Copy(bmpData, 0, ptrDib, totalDibSize);
            GlobalUnlock(hGlobalDib);

            // 3. Commit both variations to Windows
            if (OpenClipboard(IntPtr.Zero)) {
                EmptyClipboard();
                SetClipboardData(cfPng, hGlobalPng);
                SetClipboardData(CF_DIBV5, hGlobalDib);
                CloseClipboard();
            }

            return texture;
        }

    }

}
#endif