using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LSystems_Logic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. SETUP THE SYSTEM
            string axiom = "F";
            var rules = new Dictionary<char, string>
            {
                // Rule: Every 'F' (segment) becomes a stem with two branches
                { 'F', "FF+[+F-F-F]-[-F+F+F]" }
            };

            string currentGen = axiom;
            int generations = 3;

            // 2. GROW THE STRING
            for (int i = 0; i < generations; i++)
            {
                currentGen = ApplyRules(currentGen, rules);
                Console.WriteLine($"Generation {i + 1} Length: {currentGen.Length}");
            }

            // 3. INTERPRET THE COORDINATES
            Console.WriteLine("\n--- Processing Coordinates ---");
            InterpretTurtle(currentGen);

            Console.ReadLine();

            // 3. ANIMATED INTERPRETATION
            Console.WriteLine("\n--- Growing Tree ---");
            for (int i = 1; i <= currentGen.Length; i++)
            {
                // We only clear if we are actually drawing something new
                // To prevent flicker, you could only redraw every 5-10 characters
                if (i % 5 == 0 || i == currentGen.Length)
                {
                    Console.Clear();
                    Console.WriteLine($"Growth Progress: {i}/{currentGen.Length}");

                    // We pass a SUBSTRING to the interpreter
                    InterpretTurtle(currentGen.Substring(0, i));

                    Thread.Sleep(100); // Adjust for speed
                }
            }
        }

        static string ApplyRules(string current, Dictionary<char, string> rules)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in current)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }
            return sb.ToString();
        }

        static void InterpretTurtle(string instructions)
        {
            float x = 0, y = 0, angle = -90;
            float angleDelta = 25f;
            Stack<float[]> stack = new Stack<float[]>();
            List<Line> LineList = new List<Line>();
            Random rng = new Random();

            // 1. GENERATE LINES
            foreach (char c in instructions)
            {
                if (c == 'F')
                {
                    float rad = angle * (float)Math.PI / 180f;
                    float newX = x + (float)Math.Cos(rad);
                    float newY = y + (float)Math.Sin(rad);
                    LineList.Add(new Line { X1 = x, Y1 = y, X2 = newX, Y2 = newY });
                    x = newX; y = newY;
                }
                else if (c == '+') angle += (angleDelta + (float)(rng.NextDouble() * 10 - 5));
                else if (c == '-') angle -= (angleDelta + (float)(rng.NextDouble() * 10 - 5));
                else if (c == '[') stack.Push(new float[] { x, y, angle });
                else if (c == ']')
                {
                    var state = stack.Pop();
                    x = state[0]; y = state[1]; angle = state[2];
                }
            }

            // 2. CALCULATE BOUNDS
            float minX = LineList.Min(l => Math.Min(l.X1, l.X2));
            float maxX = LineList.Max(l => Math.Max(l.X1, l.X2));
            float minY = LineList.Min(l => Math.Min(l.Y1, l.Y2));
            float maxY = LineList.Max(l => Math.Max(l.Y1, l.Y2));

            // 3. CANVAS SETUP (Applying the Aspect Ratio multiplier to Width)
            int widthFactor = 2;
            int gridW = (int)Math.Ceiling((maxX - minX) * widthFactor) + 3;
            int gridH = (int)Math.Ceiling(maxY - minY) + 2;

            char[,] canvas = new char[gridW, gridH];

            for (int r = 0; r < gridW; r++)
                for (int c = 0; c < gridH; c++)
                    canvas[r, c] = ' ';

            // 4. THE SAMPLER
            foreach (var line in LineList)
            {
                for (float t = 0; t <= 1; t += 0.05f) // Finer sampling for no gaps
                {
                    float rawX = line.X1 + t * (line.X2 - line.X1);
                    float rawY = line.Y1 + t * (line.Y2 - line.Y1);

                    // Apply the normalization and the width factor
                    int plotX = (int)Math.Round((rawX - minX) * widthFactor);
                    int plotY = (int)Math.Round(rawY - minY);

                    if (plotX >= 0 && plotX < gridW && plotY >= 0 && plotY < gridH)
                        canvas[plotX, plotY] = '█';
                }
            }

            // 5. RENDER
            for (int c = gridH - 1; c >= 0; c--)
            {
                for (int r = 0; r < gridW; r++)
                    Console.Write(canvas[r, c]);
                Console.WriteLine();
            }
        }

        struct Line
        {
            public float X1, Y1; // Start
            public float X2, Y2; // End
        }

    }
}