// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Textbox_Test
{
    public static class Primitives2D
    {
        private static readonly Dictionary<string, List<Vector2>> circleCache = new Dictionary<string, List<Vector2>>();
        private static readonly Dictionary<string, List<Vector2>> arcCache = new Dictionary<string, List<Vector2>>();

        private static Texture2D pixel;

        public static Texture2D GetWhitePixel(this SpriteBatch spriteBatch)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false,
                    SurfaceFormat.Color);
                pixel.SetData(new[] {Color.White});
            }
            return pixel;
        }

        /// <summary>
        ///     Draws a list of connecting points.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="position">Where to position the points</param>
        /// <param name="points">The points to connect with lines</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layerDepth">The layer depth.</param>
        private static void DrawPoints(SpriteBatch spriteBatch, Vector2 position, List<Vector2> points, Color color,
            float thickness, float layerDepth)
        {
            if (points.Count < 2)
                return;

            for (var i = 1; i < points.Count; i++)
            {
                DrawLine(spriteBatch, point1: points[i - 1] + position, point2: points[i] + position,
                    color: color, thickness: thickness, layerDepth: 1f / layerDepth);
            }
        }

        /// <summary>
        ///     Creates a list of vectors that represents a circle.
        /// </summary>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <returns>A list of vectors that, if connected, will create a circle</returns>
        private static List<Vector2> CreateCircle(double radius, int sides)
        {
            // Look for a cached version of this circle.
            var circleKey = radius + "x" + sides;
            List<Vector2> result;
            if (circleCache.TryGetValue(circleKey, out result))
            {
                return result;
            }

            result = new List<Vector2>();
            const double max = 2.0 * Math.PI;
            var step = max / sides;

            for (var theta = 0.0; theta < max; theta += step)
            {
                result.Add(new Vector2((float) (radius * Math.Cos(theta)), (float) (radius * Math.Sin(theta))));
            }

            // Then add the first vector again so it's a complete loop.
            result.Add(new Vector2((float) (radius * Math.Cos(0)), (float) (radius * Math.Sin(0))));

            // Cache this circle so that it can be quickly drawn next time.
            circleCache.Add(circleKey, result);

            return result;
        }

        /// <summary>
        ///     Creates a list of vectors that represents an arc.
        /// </summary>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="sides">The number of sides to generate in the circle that this will cut out from</param>
        /// <param name="startingAngle">The starting angle of arc, 0 being to the east, increasing as you go clockwise</param>
        /// <param name="radians">The radians to draw, clockwise from the starting angle</param>
        /// <returns>A list of vectors that, if connected, will create an arc</returns>
        private static List<Vector2> CreateArc(float radius, int sides, float startingAngle, float radians)
        {
            // Look for a cached version of this arc.
            var arcKey = radius + "x" + sides + "," + startingAngle + "," + radians;
            List<Vector2> result;
            if (arcCache.TryGetValue(arcKey, out result))
            {
                return result;
            }

            result = new List<Vector2>();
            result.AddRange(CreateCircle(radius, sides));
            result.RemoveAt(result.Count - 1); // remove the last point because it's a duplicate of the first

            // The circle starts at (radius, 0)
            var curAngle = 0.0;
            double anglePerSide = MathHelper.TwoPi / sides;

            // "Rotate" to the starting point
            while (curAngle + anglePerSide / 2.0 < startingAngle)
            {
                curAngle += anglePerSide;

                // move the first point to the end
                result.Add(result[0]);
                result.RemoveAt(0);
            }

            // Add the first point, just in case we make a full circle
            result.Add(result[0]);

            // Now remove the points at the end of the circle to create the arc
            var sidesInArc = (int) (radians / anglePerSide + 0.5);
            result.RemoveRange(sidesInArc + 1, result.Count - sidesInArc - 1);

            // Cache this arc so that it can be quickly drawn next time.
            arcCache.Add(arcKey, result);

            return result;
        }

        /// <summary>
        ///     Draws a filled rectangle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float layerDepth)
        {
            // Simply use the function already there
            spriteBatch.Draw(spriteBatch.GetWhitePixel(), rect, null,
                color, 0f, Vector2.Zero, SpriteEffects.None,
                layerDepth: layerDepth == 0f ? 0f : 1f / layerDepth);
        }

        /// <summary>
        ///     Draws a filled rectangle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="angle">The angle in radians to draw the rectangle at</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float angle,
            float layerDepth)
        {
            spriteBatch.Draw(spriteBatch.GetWhitePixel(), rect, null,
                color, angle, Vector2.Zero, SpriteEffects.None,
                layerDepth: layerDepth == 0f ? 0f : 1f / layerDepth);
        }

        /// <summary>
        ///     Draws a filled rectangle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color,
            float layerDepth)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(spriteBatch.GetWhitePixel(), location, null, color,
                0f, Vector2.Zero, size, SpriteEffects.None,
                layerDepth: layerDepth == 0f ? 0f : 1f / layerDepth);
        }

        /// <summary>
        ///     Draws a filled rectangle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="angle">The angle in radians to draw the rectangle at</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color,
            float angle, float layerDepth)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(spriteBatch.GetWhitePixel(), location, null, color,
                angle, Vector2.Zero, size, SpriteEffects.None,
                layerDepth: layerDepth == 0f ? 0f : 1f / layerDepth);
        }

        /// <summary>
        ///     Draws a filled rectangle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The X coordinate of the left side</param>
        /// <param name="y">The Y coordinate of the upper side</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float w, float h, Color color,
            float layerDepth)
        {
            FillRectangle(spriteBatch, location: new Vector2(x, y), size: new Vector2(w, h),
                color: color, angle: 0.0f, layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draws a filled rectangle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The X coordinate of the left side</param>
        /// <param name="y">The Y coordinate of the upper side</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="angle">The angle of the rectangle in radians</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float w, float h, Color color,
            float angle, float layerDepth)
        {
            FillRectangle(spriteBatch, location: new Vector2(x, y), size: new Vector2(w, h),
                color: color, angle: angle, layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draws a rectangle with the thickness provided.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float layerDepth)
        {
            DrawRectangle(spriteBatch, rect, color, 1.0f, layerDepth);
        }

        /// <summary>
        ///     Draws a rectangle with the thickness provided.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness,
            float layerDepth)
        {
            // TODO: Handle rotations
            // TODO: Figure out the pattern for the offsets required and then handle it in the line instead of here

            DrawLine(spriteBatch, point1: new Vector2(rect.X, rect.Y),
                point2: new Vector2(rect.Right, rect.Y), color: color, thickness: thickness,
                layerDepth: layerDepth); // top
            DrawLine(spriteBatch, point1: new Vector2(rect.X + 1f, rect.Y),
                point2: new Vector2(rect.X + 1f, rect.Bottom + thickness), color: color,
                thickness: thickness, layerDepth: layerDepth); // left
            DrawLine(spriteBatch, point1: new Vector2(rect.X, rect.Bottom),
                point2: new Vector2(rect.Right, rect.Bottom), color: color, thickness: thickness,
                layerDepth: layerDepth);
            // bottom
            DrawLine(spriteBatch, point1: new Vector2(rect.Right + 1f, rect.Y),
                point2: new Vector2(rect.Right + 1f, rect.Bottom + thickness),
                color: color, thickness: thickness, layerDepth: layerDepth); // right
        }

        /// <summary>
        ///     Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color,
            float layerDepth)
        {
            DrawRectangle(spriteBatch,
                rect: new Rectangle((int) location.X, (int) location.Y, (int) size.X, (int) size.Y), color: color,
                thickness: 1.0f, layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draws a rectangle with the thickness provided.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color,
            float thickness, float layerDepth)
        {
            DrawRectangle(spriteBatch,
                rect: new Rectangle((int) location.X, (int) location.Y, (int) size.X, (int) size.Y), color: color,
                thickness: thickness, layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draws a line from point1 to point2.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coordinate of the first point</param>
        /// <param name="y1">The Y coordinate of the first point</param>
        /// <param name="x2">The X coordinate of the second point</param>
        /// <param name="y2">The Y coordinate of the second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color,
            float layerDepth)
        {
            DrawLine(spriteBatch, point1: new Vector2(x1, y1), point2: new Vector2(x2, y2),
                color: color, thickness: 1f, layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draws a line from point1 to point2.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coordinate of the first point</param>
        /// <param name="y1">The Y coordinate of the first point</param>
        /// <param name="x2">The X coordinate of the second point</param>
        /// <param name="y2">The Y coordinate of the second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color,
            float thickness, float layerDepth)
        {
            DrawLine(spriteBatch, point1: new Vector2(x1, y1), point2: new Vector2(x2, y2),
                color: color, thickness: thickness, layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draws a line from point1 to point2.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color,
            float layerDepth)
        {
            DrawLine(spriteBatch, point1, point2, color, 1f,
                layerDepth);
        }

        /// <summary>
        ///     Draws a line from point1 to point2 with a thickness.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color,
            float thickness, float layerDepth)
        {
            // calculate the distance between the two vectors
            var distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            var angle = (float) Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color,
                thickness, layerDepth);
        }

        /// <summary>
        ///     Draws a line from point1 to point2.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point">The starting point</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of this line from the starting point in radians</param>
        /// <param name="color">The color to use</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color,
            float layerDepth)
        {
            DrawLine(spriteBatch, point, length, angle, color, 1.0f,
                layerDepth);
        }

        /// <summary>
        ///     Draws a line from point1 to point2.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point">The starting point</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of this line from the starting point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color,
            float thickness, float layerDepth)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(spriteBatch.GetWhitePixel(),
                point,
                null,
                color,
                angle,
                Vector2.Zero,
                scale: new Vector2(length, thickness),
                effects: SpriteEffects.None, layerDepth: layerDepth == 0f ? 0f : 1f / layerDepth);
        }

        public static void PutPixel(this SpriteBatch spriteBatch, float x, float y, Color color)
        {
            PutPixel(spriteBatch, position: new Vector2(x, y), color: color);
        }

        public static void PutPixel(this SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.Draw(spriteBatch.GetWhitePixel(), position, color);
        }

        /// <summary>
        ///     Draw a circle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color,
            float layerDepth)
        {
            DrawPoints(spriteBatch, center, points: CreateCircle(radius, sides),
                color: color, thickness: 1.0f, layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draw a circle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color,
            float thickness, float layerDepth)
        {
            DrawPoints(spriteBatch, center, points: CreateCircle(radius, sides),
                color: color, thickness: thickness, layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draw a circle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The center X of the circle</param>
        /// <param name="y">The center Y of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides,
            Color color, float layerDepth)
        {
            DrawPoints(spriteBatch, position: new Vector2(x, y),
                points: CreateCircle(radius, sides), color: color, thickness: 1.0f,
                layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draw a circle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The center X of the circle</param>
        /// <param name="y">The center Y of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides,
            Color color,
            float thickness, float layerDepth)
        {
            DrawPoints(spriteBatch, position: new Vector2(x, y),
                points: CreateCircle(radius, sides), color: color, thickness: thickness,
                layerDepth: layerDepth);
        }

        /// <summary>
        ///     Draw a arc.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the arc</param>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="startingAngle">The starting angle of arc, 0 being to the east, increasing as you go clockwise</param>
        /// <param name="radians">The number of radians to draw, clockwise from the starting angle</param>
        /// <param name="color">The color of the arc</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawArc(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides,
            float startingAngle,
            float radians, Color color, float layerDepth)
        {
            DrawArc(spriteBatch, center, radius, sides, startingAngle,
                radians, color, 1.0f, layerDepth);
        }

        /// <summary>
        ///     Draw a arc.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the arc</param>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="startingAngle">The starting angle of arc, 0 being to the east, increasing as you go clockwise</param>
        /// <param name="radians">The number of radians to draw, clockwise from the starting angle</param>
        /// <param name="color">The color of the arc</param>
        /// <param name="thickness">The thickness of the arc</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawArc(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides,
            float startingAngle,
            float radians, Color color, float thickness, float layerDepth)
        {
            var arc = CreateArc(radius, sides, startingAngle, radians);
            //List<Vector2> arc = CreateArc2(radius, sides, startingAngle, degrees);
            DrawPoints(spriteBatch, center, arc, color, thickness,
                layerDepth);
        }
    }
}