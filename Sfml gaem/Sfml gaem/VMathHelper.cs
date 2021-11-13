using System;
using System.Collections.Generic;
using System.Text;
using SFML.System;

namespace SFML.Net_Test
{
    public static class VMathHelper
    {
        public static Vector2f NormalizeVector(Vector2f input)
        {
            float greatestValue = input.X;
            if (greatestValue <= input.Y) greatestValue = input.Y;

            return input / greatestValue;
        }

        //TODO: write dot product and cross product functions
    }
}
