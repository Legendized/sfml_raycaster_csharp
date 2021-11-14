using System;
using System.Collections.Generic;
using System.Text;
using SFML.System;

#nullable enable

namespace SFML.Net_Test
{
    public class RaycastHit
    {
        public float distance;
        public float normalInRadians;

        public RaycastHit(float distance, float normalInRadians)
        {
            this.distance = distance;
            this.normalInRadians = normalInRadians;
        }
    }
}
