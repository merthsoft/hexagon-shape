using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Merthsoft.DesignatorShapes.Hexagon;

public static class Hexagons
{
    public static IEnumerable<IntVec3> MidpointHexagon(IntVec3 s, IntVec3 t, bool filled, int rotation, int thickness, bool fillCorners)
    {
        var x1 = s.x;
        var z1 = s.z;
        var x2 = t.x;
        var z2 = t.z;

        if (x2 < x1)
            swap(ref x1, ref x2);
        if (z2 < z1)
            swap(ref z1, ref z2);
        var r = Math.Max(x2 - x1, z2 - z1);
        return RadialHexagon(s.x, s.y, s.z, r, r, filled, rotation, thickness, fillCorners);
    }

    public static IEnumerable<IntVec3> RadialHexagon(int x, int y, int z, int xRadius, int zRadius, bool fill, int rotation, int thickness, bool fillCorners)
        => Draw(x - xRadius, y, z - zRadius, x + xRadius, y, z + zRadius, fill, rotation, thickness, fillCorners);

    private static void swap<T>(ref T item1, ref T item2)
    {
        var temp = item1;
        item1 = item2;
        item2 = temp;
    }

    public static IEnumerable<IntVec3> Line(IntVec3 vert1, IntVec3 vert2, int thickness, bool fillCorners)
        => Line(vert1.x, vert1.y, vert1.z, vert2.x, vert2.y, vert2.z, thickness, fillCorners);

    public static IEnumerable<IntVec3> Line(int x1, int y1, int z1, int x2, int y2, int z2, int thickness, bool fillCorners)
    {
        var ret = new HashSet<IntVec3> {
            new(x1, y1, z1),
            new(x2, y2, z2)
        };

        var deltaX = Math.Abs(x1 - x2);
        var deltaZ = Math.Abs(z1 - z2);
        var stepX = x2 < x1 ? 1 : -1;
        var stepZ = z2 < z1 ? 1 : -1;

        var err = deltaX - deltaZ;

        while (true)
        {
            ret.Add(new(x2, y1, z2));
            if (x2 == x1 && z2 == z1)
                break;
            var e2 = 2 * err;

            if (e2 > -deltaZ)
            {
                err -= deltaZ;
                x2 += stepX;
            }

            if (x2 == x1 && z2 == z1)
                break;
            if (fillCorners && thickness == 1)
                ret.Add(new(x2, y1, z2));

            if (e2 < deltaX)
            {
                err += deltaX;
                z2 += stepZ;
            }
        }

        return ret;
    }
    public static IEnumerable<IntVec3> Fill(IEnumerable<IntVec3> outLine)
    {
        var ret = new HashSet<IntVec3>();
        foreach (var lineGroup in outLine.GroupBy(vec => vec.z))
            if (lineGroup.Count() == 1)
                ret.Add(lineGroup.First());
            else
            {
                var sorted = lineGroup.OrderBy(v => v.x);
                var point1 = sorted.First();
                var point2 = sorted.Last();
                ret.AddRange(HorizontalLine(point1.x, point2.x, point1.y, lineGroup.Key, 1, true));
            }

        return ret;
    }
    
    public static IEnumerable<IntVec3> HorizontalLine(int x1, int x2, int y, int z, int thickness, bool direction)
    {
        if (x1 > x2)
            swap(ref x1, ref x2);
        return Range(x1, x2 - x1 + 1).SelectMany(x => Range(0, thickness, direction).Select(t => new IntVec3(x, y, z + t)));
    }

    private static IEnumerable<int> Range(int start, int count, bool direction = true)
    {
        if (count < 0)
        {
            count = -count;
            direction = !direction;
        }

        for (var i = 0; i < count; i++)
            yield return start + (direction ? i : -i);
    }

    public static IEnumerable<IntVec3> Draw(int sx, int sy, int sz, int tx, int ty, int tz, bool fill, int rotation, int thickness, bool fillCorners)
    {
        if (tx < sx)
            swap(ref sx, ref tx);
        if (tz < sz)
            swap(ref sz, ref tz);
        IntVec3 A, B, C, D, E, F;

        if (rotation == 0)
        {
            var w = tx - sx;
            var h = tz - sz;
            var mz = h / 2 + sz;
            var wt = w / 4;

            A = new IntVec3(sx, sy, mz);
            B = new IntVec3(sx + wt, sy, sz);
            C = new IntVec3(sx + wt, sy, tz);
            D = new IntVec3(tx - wt, sy, sz);
            E = new IntVec3(tx - wt, sy, tz);
            F = new IntVec3(tx, sy, mz);
        }
        else
        {
            var w = tx - sx;
            var h = tz - sz;
            var mx = w / 2 + sx;
            var ht = h / 4;

            A = new IntVec3(mx, sy, sz);
            B = new IntVec3(tx, ty, sz + ht);
            C = new IntVec3(sx, sy, sz + ht);
            D = new IntVec3(tx, ty, tz - ht);
            E = new IntVec3(sx, sy, tz - ht);
            F = new IntVec3(mx, ty, tz);
        }

        var ret = new HashSet<IntVec3>();

        ret.AddRange(Line(A, B, thickness, fillCorners));
        ret.AddRange(Line(B, D, thickness, fillCorners));
        ret.AddRange(Line(F, D, thickness, fillCorners));
        ret.AddRange(Line(A, C, thickness, fillCorners));
        ret.AddRange(Line(C, E, thickness, fillCorners));
        ret.AddRange(Line(F, E, thickness, fillCorners));

        return fill ? Fill(ret) : ret;
    }
}