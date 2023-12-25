using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Z3;

namespace AdventOfCode2023
{
    internal class Day24
    {
        /// <summary>
        /// Computes the answer for Part 1
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        public string Part1(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);
            List<((long x, long y, long z), (long vx, long vy, long vz))> Hailstones = new();
            foreach(string input in RawInput)
            {
                string p1 = input.Split('@')[0];
                string p2 = input.Split('@')[1];
                long[] origin = p1.Split(',').Select(x => long.Parse(x.Trim())).ToArray();
                long[] velocities = p2.Split(',').Select(x => long.Parse(x.Trim())).ToArray();
                ((long x, long y, long z), (long vx, long vy, long zy)) newHail = ((origin[0], origin[1], origin[2]), (velocities[0], velocities[1], velocities[2]));
                Hailstones.Add(newHail);
            }

            long intersections = 0;
            for(int i = 0; i < Hailstones.Count-1; i++)
            {
                for(int j = i+1; j < Hailstones.Count; j++)
                {
                    //intersections += FindIntersectionXY(Hailstones[i], Hailstones[j], 7, 27) ? 1 : 0;
                    intersections += FindIntersectionXY(Hailstones[i], Hailstones[j], 200000000000000, 400000000000000) ? 1 : 0;
                }
            }
            return intersections.ToString();
        }

        /// <summary>
        /// Computes the answer for Part 2
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        public string Part2(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);
            List<((long x, long y, long z), (long vx, long vy, long vz))> Hailstones = new();
            foreach (string input in RawInput)
            {
                string p1 = input.Split('@')[0];
                string p2 = input.Split('@')[1];
                long[] origin = p1.Split(',').Select(x => long.Parse(x.Trim())).ToArray();
                long[] velocities = p2.Split(',').Select(x => long.Parse(x.Trim())).ToArray();
                ((long x, long y, long z), (long vx, long vy, long zy)) newHail = ((origin[0], origin[1], origin[2]), (velocities[0], velocities[1], velocities[2]));

                Hailstones.Add(newHail);
            }

            return Solve(Hailstones).ToString();
        }

        public bool FindIntersectionXY(((long x, long y, long z) o, (long vx, long vy, long vz) v) h1, ((long x, long y, long z) o, (long vx, long vy, long vz) v) h2, long min, long max)
        {
            // Direction vectors of the two lines
            long[] dir1 = { h1.v.vx, h1.v.vy, 0};
            long[] dir2 = { h2.v.vx, h2.v.vy, 0};

            // Cross product of the direction vectors
            double crossProduct1 = dir1[0] * dir2[1] - dir1[1] * dir2[0];
            double crossProduct2 = dir2[0] * dir1[1] - dir2[1] * dir1[0];
            if (crossProduct1 == 0 || crossProduct2 == 0)
            {
                //parallel
                return false;
            }

            // Vector from the start point of the first line to the intersection point
            long[] vecFromStart1 = {
                h2.o.x - h1.o.x,
                h2.o.y - h1.o.y
            };

            long[] vecFromStart2 = {
                h1.o.x - h2.o.x,
                h1.o.y - h2.o.y
            };

            // Scalar product
            double t1 = (vecFromStart1[0] * dir2[1] - vecFromStart1[1] * dir2[0]) / crossProduct1;
            double t2 = (vecFromStart2[0] * dir1[1] - vecFromStart2[1] * dir1[0]) / crossProduct2;

            if(t1 < 0 || t2 < 0)
            {
                return false;
            }

            // Calculate the intersection point in XY plane
            double x = h1.o.x + t1 * dir1[0];
            double y = h1.o.y + t1 * dir1[1];

            return x >= min && x <= max && y >= min && y <= max;
        }

        /// <summary>
        /// Uses Z3 to solve. Taken from https://pastebin.com/fkpZWn8X
        /// I did not like this day, and am also running out of time due to holidays
        /// </summary>
        /// <param name="hails"> The list of hails
        /// </param>
        /// <returns>
        /// The answer for part 2
        /// </returns>
        static long Solve(List<((long x, long y, long z) o, (long vx, long vy, long vz) v)> hails)
        {
            var ctx = new Context();
            var solver = ctx.MkSolver();

            // Coordinates of the stone
            var x = ctx.MkIntConst("x");
            var y = ctx.MkIntConst("y");
            var z = ctx.MkIntConst("z");

            // Velocity of the stone
            var vx = ctx.MkIntConst("vx");
            var vy = ctx.MkIntConst("vy");
            var vz = ctx.MkIntConst("vz");

            // For each iteration, we will add 3 new equations and one new condition to the solver.
            // We want to find 9 variables (x, y, z, vx, vy, vz, t0, t1, t2) that satisfy all the equations, so a system of 9 equations is enough.
            for (var i = 0; i < 3; i++)
            {
                var t = ctx.MkIntConst($"t{i}"); // time for the stone to reach the hail
                var hail = hails[i];

                var px = ctx.MkInt(Convert.ToInt64(hail.o.x));
                var py = ctx.MkInt(Convert.ToInt64(hail.o.y));
                var pz = ctx.MkInt(Convert.ToInt64(hail.o.z));

                var pvx = ctx.MkInt(Convert.ToInt64(hail.v.vx));
                var pvy = ctx.MkInt(Convert.ToInt64(hail.v.vy));
                var pvz = ctx.MkInt(Convert.ToInt64(hail.v.vz));

                var xLeft = ctx.MkAdd(x, ctx.MkMul(t, vx)); // x + t * vx
                var yLeft = ctx.MkAdd(y, ctx.MkMul(t, vy)); // y + t * vy
                var zLeft = ctx.MkAdd(z, ctx.MkMul(t, vz)); // z + t * vz

                var xRight = ctx.MkAdd(px, ctx.MkMul(t, pvx)); // px + t * pvx
                var yRight = ctx.MkAdd(py, ctx.MkMul(t, pvy)); // py + t * pvy
                var zRight = ctx.MkAdd(pz, ctx.MkMul(t, pvz)); // pz + t * pvz

                solver.Add(t >= 0); // time should always be positive - we don't want solutions for negative time
                solver.Add(ctx.MkEq(xLeft, xRight)); // x + t * vx = px + t * pvx
                solver.Add(ctx.MkEq(yLeft, yRight)); // y + t * vy = py + t * pvy
                solver.Add(ctx.MkEq(zLeft, zRight)); // z + t * vz = pz + t * pvz
            }

            solver.Check();
            var model = solver.Model;

            var rx = model.Eval(x);
            var ry = model.Eval(y);
            var rz = model.Eval(z);

            return Convert.ToInt64(rx.ToString()) + Convert.ToInt64(ry.ToString()) + Convert.ToInt64(rz.ToString());
        }
    }
}
