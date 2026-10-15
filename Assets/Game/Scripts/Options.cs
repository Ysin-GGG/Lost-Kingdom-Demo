using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LostKingdom
{
    public static class Options
    {
        public static int FramePerSecond { get => Application.targetFrameRate; set => LK.Instance.StartCoroutine(LK.SetFramePerSecond(value)); }
        public static byte LanguageID { get; set; } = 1;
        public static Vector2 ScreenResolution
        {
            get => LK.ScreenSize;
            set
            {
                List<Display> resolutions = new List<Display>(Display.displays);
                Display last = resolutions.Last();
                if (value.x <= 0 || value.y <= 0) value = new Vector2(last.renderingWidth, last.renderingHeight);
                int w = (int)value.x, h = (int)value.y, idx = -1;
                bool full = last.renderingWidth <= w && last.renderingHeight <= h;
                if ((idx = resolutions.FindIndex((r) => (r.renderingWidth == w) && (r.renderingHeight == h))) >= 0)
                {
                    Display.displays[idx].Activate();
                    Screen.SetResolution(w, h, full);
                }
            }
        }

        public static float MasterVolume { get; set; } = .5f;
        public static float MusicVolume { get; set; } = 1f;
        public static float SFXVolume { get; set; } = 1f;
        public static float AmbientVolume { get; set; } = 1f;
        public static float SelectBoxSmoothness { get; set; } = .25f;
        public static bool AllowPageLoop { get; set; } = true;
    }
}
