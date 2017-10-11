﻿using AGV.Map.Common;
using SharpGL;
using System.Collections.Generic;

namespace AGV.Map.Core
{
    /// <summary>
    /// 座標轉換，實際座標轉 2D 文字座標
    /// </summary>
    public delegate IPair DelGL2Text2D(IPair gl);

    public static class GLText2D
    {
        /// <summary>
        /// 顯示文字字體
        /// </summary>
        public static readonly System.Drawing.Font TextFont = new System.Drawing.Font("Arial", 24);

        /// <summary>
        /// 執行緒鎖
        /// </summary>
        private static readonly object mKey = new object();

        /// <summary>
        /// 顯示文字
        /// </summary>
        private static List<DisplayText> DisplayTextList = new List<DisplayText>();

        /// <summary>
        /// 顯示文字顏色
        /// </summary>
        public static IColor TextColor { get; } = new Color(System.Drawing.Color.Black);

        /// <summary>
        /// 清除所有的 Text 資訊
        /// </summary>
        public static void ClearText(this OpenGL gl)
        {
            lock (mKey)
            {
                DisplayTextList.Clear();
                gl.DrawText(0, 0, TextColor.R / 255.0f, TextColor.G / 255.0f, TextColor.B / 255.0f, TextFont.Name, TextFont.Size, "");
            }
        }

        /// <summary>
        /// 印出所有的 Text 資訊
        /// </summary>
        public static void DrawTextList(this OpenGL gl, DelGL2Text2D convert)
        {
            lock (mKey)
            {
                foreach (var item in DisplayTextList)
                {
                    IPair screen = convert(item.Position);
                    gl.DrawText(screen.X, screen.Y, TextColor.R / 255.0f, TextColor.G / 255.0f, TextColor.B / 255.0f, TextFont.Name, TextFont.Size, item.Text);
                }
                DisplayTextList.Clear();
            }
        }

        /// <summary>
        /// 繪製顯示文字
        /// </summary>
        public static void PushText(this OpenGL gl, int x, int y, string format, params object[] arg)
        {
            gl.PushText(x, y, string.Format(format, arg));
        }

        /// <summary>
        /// 繪製顯示文字
        /// </summary>
        public static void PushText(this OpenGL gl, int x, int y, string str)
        {
            lock (mKey)
                DisplayTextList.Add(new DisplayText(x, y, str));
        }

        /// <summary>
        /// 繪製顯示文字
        /// </summary>
        public static void PushText(this OpenGL gl, IPair position, string str)
        {
            gl.PushText(position.X, position.Y, str);
        }

        /// <summary>
        /// 繪製顯示文字
        /// </summary>
        public static void PushText(this OpenGL gl, IPair position, string format, params object[] arg)
        {
            gl.PushText(position.X, position.Y, string.Format(format, arg));
        }

        /// <summary>
        /// 顯示文字結構
        /// </summary>
        private struct DisplayText
        {
            public DisplayText(int x, int y, string text)
            {
                Position = new Pair(x, y);
                Text = text;
            }

            public IPair Position { get; }
            public string Text { get; set; }
        }
    }
}