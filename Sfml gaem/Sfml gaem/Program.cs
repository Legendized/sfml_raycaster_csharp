using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

namespace SFML.Net_Test //todo: make interpolate between rays so that we can get more than 2000 fps which is essential for gaming
{
    class Program
    {
        public static RenderWindow window;
        public static Clock clock;
        static float timeBeforeUpdate = 0;
        static float prevTimeBeforeUpdate = 0;
        public static float deltaTime;
        public static float fov = 25f;
        public static uint rays = 512;
        public static float gamma = 5;
        public static int colliderSize = 24;

        public const float rad2deg = 57.29578f;
        public const float deg2rad = 0.01745329237f;
        public const float P2 = MathF.PI / 2;
        public const float P3 = 3 * MathF.PI / 2;

        public static bool wPressed, aPressed, sPressed, dPressed;

        public static float px, py, pdx, pdy, angle = 0f, moveSpeed = 200f, lookSpeed = 150f;

        public static Vector2u resolution = new Vector2u(1024, 512);

        public static int mapX = 8, mapY = 8, mapS = 64;
        public static uint[] map = new uint[64]         //the map array. Edit to change level but keep the outer walls
        {
        1,1,1,1,1,1,1,1,
        1,0,1,0,0,1,0,1,
        1,0,1,0,0,0,0,1,
        1,0,1,0,0,0,0,1,
        1,0,0,0,0,0,1,1,
        1,1,0,0,0,1,0,1,
        1,0,0,0,0,0,0,1,
        1,1,1,1,1,1,1,1
        };
        static void drawPlayer(RenderWindow window)
        {
            CircleShape cs = new CircleShape(8f);
            cs.FillColor = new Color(255, 255, 0);
            cs.Position = new Vector2f(px - 8, py - 8);
            window.Draw(cs);

            Vertex v1 = new Vertex(new Vector2f((px), (py)));
            Vertex v2 = new Vertex(new Vector2f((px) + (pdx*48), (py) + (pdy * 48)));
            v1.Color = Color.Red;
            v2.Color = Color.Green;
            Vertex[] vertarray = new Vertex[2];
            vertarray[0] = v1;
            vertarray[1] = v2;

            window.Draw(vertarray, PrimitiveType.Lines);
        }

        static float dist(float ax, float ay, float bx, float by, float ang)
        {
            return (MathF.Sqrt((bx - ax) * (bx - ax) + (by - ay) * (by - ay)) );
        }

        static RaycastHit[] drawRays3D(RenderWindow window)
        {
            RaycastHit[] rayhits;
            rayhits = new RaycastHit[rays];
            float[] distances = new float[rays];
            float[] normals   = new float[rays];

            int r, mx, my = 0, mp, dof; float rx = 0, ry = 0, ra = 0, xo = 0, yo = 0;
            
            float rayangle = angle / 360;
            ra = (1-rayangle) * (MathF.PI*2);

            ra = ra - (fov * deg2rad);
            if (ra<0) { ra += 2 * MathF.PI; } if(ra>2*MathF.PI) { ra -= 2 * MathF.PI; }

            for (r = 0; r < rays; r++)
            {
                //check horizontal lines
                float disH = 1000000, hx = px, hy = py;
                dof = 0;
                float aTan = -1 / MathF.Tan(ra);
                if (ra > MathF.PI) { ry = (((int)py >> 6) << 6) - 0.0001f; rx = (py - ry) * aTan + px; yo = -64; xo = -yo * aTan; } //looking up
                if (ra < MathF.PI) { ry = (((int)py >> 6) << 6) + 64f;     rx = (py - ry) * aTan + px; yo = 64;  xo = -yo * aTan; } //looking down
                if (ra == 0 || ra == MathF.PI) { rx = px; ry = py; dof = 8; } //looking straight left or right
                while (dof < 8)
                {
                    mx = (int)(rx) >> 6; my = (int)(ry) >> 6; mp = my * mapX + mx;
                    if (mp < 0) mp = 0;
                    if (mp > mapS) mp = mapS;
                    if (mp < mapX * mapY && map[mp] == 1) //hit wall
                    {
                        hx = rx; hy = ry;
                        disH = dist(px, py, hx, hy, ra);
                        dof = 8;
                    }
                    else //didnt hit wall
                    {
                        rx += xo;
                        ry += yo;
                        dof+=1;
                    }

                }

                //check vertical lines
                float disV = 1000000, vx = px, vy = py;
                dof = 0;
                float nTan =-MathF.Tan(ra);
                if (ra>(MathF.PI/2) && ra<(3*MathF.PI/2)) { rx = (((int)px >> 6) << 6) - 0.0001f; ry = (px - rx) * nTan + py; xo =-64; yo = -xo * nTan; } //looking left
                if (ra<(MathF.PI/2) || ra>(3*MathF.PI/2)) { rx = (((int)px >> 6) << 6) + 64f;     ry = (px - rx) * nTan + py; xo = 64; yo = -xo * nTan; } //looking right
                if (ra == 0 || ra == MathF.PI) { rx = px; ry = py; dof = 8; } //looking straight up or down
                while (dof < 8)
                {
                    mx = (int)(rx) >> 6; my = (int)(ry) >> 6; mp = my * mapX + mx;
                    if (mp < 0) mp = 0;
                    if (mp > mapS) mp = mapS;
                    if (mp < mapX * mapY && map[mp] == 1) //hit wall
                    {
                        vx = rx; vy = ry;
                        disV = dist(px, py, vx, vy, ra);
                        dof = 8;
                    }
                    else //didnt hit wall
                    {
                        rx += xo;
                        ry += yo;
                        dof += 1;
                    }

                }

                ra += (deg2rad/(rays/fov*0.5f)); 
                if (ra < 0) { ra += 2 * MathF.PI; }
                if (ra > 2 * MathF.PI) { ra -= 2 * MathF.PI; }

                float nv;
                nv = 1f;
                if (disV < disH) { rx = vx; ry = vy; nv = 0.25f; }
                if (disH < disV) { rx = hx; ry = hy; nv = 0.5f;  }

                Vertex v1 = new Vertex(new Vector2f(px, py));
                Vertex v2 = new Vertex(new Vector2f(rx, ry));

                v1.Color = Color.Red;
                v2.Color = Color.Red;
                Vertex[] vertarray = new Vertex[2];
                vertarray[0] = v1;
                vertarray[1] = v2;

                float ca = -(angle*deg2rad) - ra;
                if (ca < 0) { ca += 2 * MathF.PI; }
                if (ca > 2 * MathF.PI) { ca -= 2 * MathF.PI; }

                window.Draw(vertarray, PrimitiveType.Lines);

                rayhits[r] = new RaycastHit(dist(px, py, rx, ry, ra) * MathF.Cos(ca), nv);
            }

            return rayhits;
        }

        static void drawMap2D(RenderWindow window)
        {
            float resMult = 1f;
            int x, y, xo, yo;
            for (y = 0; y < mapY; y++)
            {
                for (x = 0; x < mapX; x++)
                {
                    RectangleShape rs = new RectangleShape(new Vector2f(mapS * resMult * 0.95f, mapS * resMult * 0.95f));
                    xo = (int)(x * mapS * resMult);
                    yo = (int)(y * mapS * resMult);
                    rs.Position = new Vector2f(xo, yo);
                    rs.FillColor = Color.Black;
                    if (map[y * mapX + x] == 1) rs.FillColor = Color.White;
                    window.Draw(rs);
                }
            }
        }

        static void Main()
        {
            clock = new Clock();
            px = 150f; py = 400f; angle = 90f;

            window = new RenderWindow(new Window.VideoMode(resolution.X, resolution.Y), "test");
            window.SetActive();
            window.KeyPressed += OnKeyPressed;
            window.KeyReleased += OnKeyReleased;
            window.Closed += CloseGame;
            window.Resized += window_resize;

            pdx = MathF.Cos(angle / rad2deg); pdy = -MathF.Sin(angle / rad2deg);
            pdx = MathF.Cos(angle / rad2deg); pdy = -MathF.Sin(angle / rad2deg);

            while (window.IsOpen)
            {
                rays = resolution.X / 2;
                timeBeforeUpdate = clock.ElapsedTime.AsSeconds();
                deltaTime = timeBeforeUpdate - prevTimeBeforeUpdate;
                prevTimeBeforeUpdate = clock.ElapsedTime.AsSeconds();

                Console.WriteLine("fps " + MathF.Round(1f/deltaTime));

                window.Clear(new Color(64, 64, 64));
                window.DispatchEvents();

                drawMap2D(window);
                RaycastHit[] rayHits = drawRays3D(window);

                //perform rotation
                if (aPressed) { angle += lookSpeed * deltaTime; pdx = MathF.Cos(angle / rad2deg); pdy = -MathF.Sin(angle / rad2deg); }
                if (dPressed) { angle -= lookSpeed * deltaTime; pdx = MathF.Cos(angle / rad2deg); pdy = -MathF.Sin(angle / rad2deg); }

                //check collisions
                int xo = 0; if (pdx < 0) { xo = -colliderSize; } else { xo = colliderSize; }
                int yo = 0; if (pdy < 0) { yo = -colliderSize; } else { yo = colliderSize; }
                int ipx = (int)px / 64, ipx_add_xo = (int)(px + xo) / 64, ipx_sub_xo = (int)(px - xo) / 64;
                int ipy = (int)py / 64, ipy_add_yo = (int)(py + yo) / 64, ipy_sub_yo = (int)(py - yo) / 64;

                if (wPressed)
                {
                    if (map[ipy*mapX        + ipx_add_xo] == 0) { px += (pdx * deltaTime * moveSpeed); }
                    if (map[ipy_add_yo*mapX + ipx       ] == 0) { py += (pdy * deltaTime * moveSpeed); }
                }
                if (sPressed)
                {
                    if (map[ipy * mapX + ipx_sub_xo] == 0) { px -= (pdx * deltaTime * moveSpeed); }
                    if (map[ipy_sub_yo * mapX + ipx] == 0) { py -= (pdy * deltaTime * moveSpeed); }
                }

                if (angle < 0f) angle += 360f;
                if (angle > 360f) angle -= 360f;

                //Console.WriteLine(angle);

                //render 2d objects
                drawPlayer(window);
                //render walls
                for (uint i = resolution.X/2; i < resolution.X; i++)
                {
                    float dist = ((mapS*resolution.X)/rayHits[i-resolution.X/2].distance)/2; if (dist > resolution.Y/2) dist = resolution.Y/2;
                    Color col = Shaders.diffuseWithFog(Color.Green, rayHits[i - resolution.X / 2].normalInRadians, rayHits[i - resolution.X / 2].distance, 0.01f);
                    window.Draw(v2lsdraw((uint)dist, i, col), PrimitiveType.Lines);
                }

                //render 3d objects
                drawCrosshair(window);

                window.Display();
            }
        }

        public static Vertex[] v2lsdraw(uint size, uint offset, Color color)
        {
            Vertex v1 = new Vertex(new Vector2f(offset, (resolution.Y/2) - size));
            Vertex v2 = new Vertex(new Vector2f(offset, (resolution.Y / 2) + size));
            Vertex[] va;
            va = new Vertex[2];
            v1.Color = color;
            v2.Color = color;
            va[0] = v1;
            va[1] = v2;

            return va;
        }

        public static RaycastHit InterpolateRay(RaycastHit hit1, RaycastHit hit2)
        {
            return new RaycastHit(lerp(hit1.distance, hit2.distance, 0.5f), lerp(hit1.normalInRadians, hit2.normalInRadians, 0.5f));
        }

        static void OnKeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (RenderWindow)sender;
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }


            if (e.Code == Keyboard.Key.A)
            {
                aPressed = true;
            }
            if (e.Code == Keyboard.Key.W)
            {
                wPressed = true;
            }
            if (e.Code == Keyboard.Key.S)
            {
                sPressed = true;
            }
            if (e.Code == Keyboard.Key.D)
            {
                dPressed = true;
            }
        }
        static void OnKeyReleased(object sender, SFML.Window.KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.A)
            {
                aPressed = false;
            }
            if (e.Code == Keyboard.Key.W)
            {
                wPressed = false;
            }
            if (e.Code == Keyboard.Key.S)
            {
                sPressed = false;
            }
            if (e.Code == Keyboard.Key.D)
            {
                dPressed = false;
            }
        }

        static void CloseGame(object sender, EventArgs e)
        {
            Console.WriteLine("shutting down");
            window.Close();
        }

        static void drawCrosshair(RenderWindow window)
        {
            RectangleShape rsx1 = new RectangleShape(new Vector2f(14, 4));
            RectangleShape rsx2 = new RectangleShape(new Vector2f(14, 4));
            RectangleShape rsy  = new RectangleShape(new Vector2f(4, 34));

            rsx1.FillColor = new Color(255, 255, 255, 64);
            rsx2.FillColor = new Color(255, 255, 255, 64);
            rsy.FillColor  = new Color(255, 255, 255, 64);

            uint rh = resolution.X / 2;
            uint rq = resolution.X / 4;

            rh -= 4;

            rsy.Position  = new Vector2f(rh + rq, (resolution.Y/2) - 14);
            rsx1.Position = new Vector2f(rh + rq-14, resolution.Y / 2);
            rsx2.Position = new Vector2f(rh + rq + 4, resolution.Y / 2);

            window.Draw(rsy);
            window.Draw(rsx1);
            window.Draw(rsx2);
        }

        static void window_resize(object sender, SizeEventArgs e)
        {
            window.Size = resolution;
        }
        static float lerp( float a, float b, float alpha)
        {
            return (a * (1f - alpha) + b * alpha);
        }
    }
}
