using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class Bezier
    {
        // public static Vector2 LineLerp(Vector2 p1, Vector2 p2, float t)
        // {
        //     float x = Mathf.Lerp(p1.x, p2.x, t);
        //     float y = Mathf.Lerp(p1.y, p2.y, t);
        //
        //     return new Vector2(x, y);
        // }

        //Interpolates between three control points with a quadratic bezier curve, with the interpolant t
        private static Vector3 QuadraticBezierInterpolation(Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var a = Vector3.Lerp(p1, p2, t);
            var b = Vector3.Lerp(p2, p3, t);
            return Vector3.Lerp(a, b, t);
        }

        public static IReadOnlyList<Vector3> AddSegments(IReadOnlyList<Vector3> input, int segments)
        {
            var result = new List<Vector3>();
            for (var index = 1; index < input.Count; index++)
            {
                for (var s = 0; s < segments; s++)
                {
                    var t = Mathf.InverseLerp(0, segments, s);
                    result.Add(Vector3.Lerp(input[index-1], input[index], t));
                }
            }
            return result;
        }

        public static IReadOnlyList<Vector3> Create(IReadOnlyList<Vector3> inputPositions, int segments)
        {
            if (inputPositions.Count <= 2)
                return inputPositions;
            
            var result = new List<Vector3>();

            var index = 0;
            while (index <= inputPositions.Count - 3)
            {
                var p0 = inputPositions[index];
                var p1 = inputPositions[index + 1];
                var p2 = inputPositions[index + 2];

                if (IsCorner(p0, p1, p2))
                {
                    for (var s = 0; s <= segments; s++)
                    {
                        var t = s * 1f / segments;
                        var point = QuadraticBezierInterpolation(p0, p1, p2, t);
                        result.Add(point);
                    }

                    index += 2;
                }
                else
                {
                    result.Add(p0);
                    index++;
                }
            }
            
            for (;index < inputPositions.Count; index++)
            {
                result.Add(inputPositions[index]);
            }
    
            
            // result.Add(inputPositions[0]);
            // result.Add(inputPositions[1]);
            //
            // var segmentsNormalized = 1f / segments;
            //
            // for (var index = 2; index < inputPositions.Count - 3; index+=3)
            // {
            //     for (var t = 0f; t < 1; t+=segmentsNormalized)
            //     {
            //         var point = QuadraticBezierInterpolation(
            //             inputPositions[index], 
            //             inputPositions[index+1],
            //             inputPositions[index+2],
            //             t);
            //         result.Add(point);
            //     }
            // }

            return result;
            
            bool IsCorner(Vector3 p0, Vector3 p1, Vector3 p2)
            {
                var minX = Mathf.Min(p0.x, p1.x, p2.x);
                var maxX = Mathf.Max(p0.x, p1.x, p2.x);
                
                var minY = Mathf.Min(p0.y, p1.y, p2.y);
                var maxY = Mathf.Max(p0.y, p1.y, p2.y);

                var r = Mathf.Abs(maxX - minX) > 0.1f && Mathf.Abs(maxY - minY) > 0.1f;
                return r;
            }
        }
    }
}