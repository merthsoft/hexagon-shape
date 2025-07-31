using System.Collections.Generic;
using Verse;

namespace Merthsoft.DesignatorShapes.Hexagon;

public class Filled : Verse.DrawStyle
{
    public override void Update(IntVec3 origin, IntVec3 target, List<IntVec3> buffer)
    {
        buffer.Clear();
        buffer.AddRange(Hexagons.Draw(origin.x, origin.y, origin.z, target.x, target.y, target.z, true, 0, 1, true));
    }
}

public class Outline : Verse.DrawStyle
{
    public override void Update(IntVec3 origin, IntVec3 target, List<IntVec3> buffer)
    {
        buffer.Clear();
        buffer.AddRange(Hexagons.Draw(origin.x, origin.y, origin.z, target.x, target.y, target.z, false, 0, 1, true));
    }
}

public class MidpointFilled : Verse.DrawStyle
{
    public override void Update(IntVec3 origin, IntVec3 target, List<IntVec3> buffer)
    {
        buffer.Clear();
        buffer.AddRange(Hexagons.MidpointHexagon(origin, target, true, 0, 1, true));
    }
}

public class MidpointOutline : Verse.DrawStyle
{
    public override void Update(IntVec3 origin, IntVec3 target, List<IntVec3> buffer)
    {
        buffer.Clear();
        buffer.AddRange(Hexagons.MidpointHexagon(origin, target, false, 0, 1, true));
    }
}