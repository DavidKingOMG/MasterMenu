using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MamboDMA.Games.ArcRaiders
{
    internal class ArcEntityESP
    {
        //  ***** May need to update Render parameters later
        public static void Render(
           bool drawBoxes, bool drawNames, bool drawDistance, bool drawSkeletons,
           float maxDistMeters, float maxSkelDistMeters,
           Vector4 colorPlayer, Vector4 colorBot,
           Vector4 colorBoxVisible, Vector4 colorBoxInvisible,
           Vector4 colorSkelVisible, Vector4 colorSkelInvisible,
           float zoomEff // <= pass MathF.Max(1f, Players.Zoom)
       )
        {
            // ***** Make Render Logic Here

        }
    }
}
