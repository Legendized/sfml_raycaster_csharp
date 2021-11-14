using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SFML.Net_Test
{
    public static class Shaders
    {
        public static Color diffuseWithFog(Color MainColor, float normal, float distToCam, float fogThickness, float fogPower)
        {
            float fog = (distToCam * fogThickness);
            //fog = MathF.Abs(fog);
            if ((fog) > 156) fog = 156;
            float diffuse = MathF.Abs(MathF.Sin(normal / MathF.PI));
            float rfog = fog * (MainColor.R / 255);
            float gfog = fog * (MainColor.G / 255);
            float bfog = fog * (MainColor.B / 255);
            if (rfog >= 240f) rfog = 240f;
            if (gfog >= 240f) gfog = 240f;
            if (bfog >= 240f) bfog = 240f;
            Color outcol = new Color((byte)(((MainColor.R * diffuse) - MathF.Pow(rfog, fogPower)) * Program.gamma), (byte)(((MainColor.G * diffuse) - MathF.Pow(gfog, fogPower)) * Program.gamma), (byte)(((MainColor.B * diffuse) - MathF.Pow(bfog, fogPower)) * Program.gamma));
            return outcol;
        }
        public static Color FogUnlit(Color MainColor, float distToCam)
        {
            float fogThickness = 20f;
            float fog = (distToCam * fogThickness);
            if ((fog) > 156) fog = 156;
            Color outcol = new Color((byte)(MainColor.R - fog), (byte)(MainColor.G - fog), (byte)(MainColor.B - fog));
            return outcol;
        }
    }
}
