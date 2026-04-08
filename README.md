# L-System Tree Generator (C# Console)

> **A fractal plant simulator built on formal grammar theory — growing infinite botanical complexity from a single character.**

Implemented in C# using a production-quality turtle graphics engine, this project applies **Lindenmayer Systems (L-Systems)** to procedurally generate and animate a branching tree directly in the terminal, rendered in Unicode block characters.

---

## What Is an L-System?

An **L-System** is a type of formal *string-rewriting grammar*, invented by biologist Aristid Lindenmayer in 1968 to model plant cell growth. The key insight is that enormous structural complexity can emerge from applying a tiny set of rules recursively — the same principle behind fractals like the Sierpiński triangle or the Koch snowflake.

This program grows a botanical tree through **3 generations** using:

| Symbol | Meaning |
|--------|---------|
| `F`    | Draw a line segment (move the turtle forward) |
| `+`    | Rotate **clockwise** by the angle delta |
| `-`    | Rotate **counter-clockwise** by the angle delta |
| `[`    | **Push** current position and angle onto the stack |
| `]`    | **Pop** — return to the last saved state (branch end) |

The single production rule drives all growth:

```
F → FF+[+F-F-F]-[-F+F+F]
```

After just **3 generations**, `"F"` expands into a string **thousands of characters long**, encoding every branch, angle, and fork of the tree.

---

## Technical Depth

### 1. String Rewriting (Formal Grammars)
Each generation applies the rule set via a **linear scan with a `StringBuilder`**, achieving O(n) growth per step. The output string length grows exponentially — a direct demonstration of why L-Systems can model the explosive branching of real plants.

```csharp
static string ApplyRules(string current, Dictionary<char, string> rules)
{
    StringBuilder sb = new StringBuilder();
    foreach (char c in current)
        sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
    return sb.ToString();
}
```

### 2. Stack-Based Turtle Graphics
The `[` and `]` symbols implement a **recursive branching mechanic using an explicit stack** — no recursion overhead. When the turtle enters a `[`, it saves its `(x, y, angle)` state. When it hits `]`, it teleports back, allowing side branches to grow without disturbing the main stem.

```csharp
else if (c == '[') stack.Push(new float[] { x, y, angle });
else if (c == ']') { var state = stack.Pop(); x = state[0]; y = state[1]; angle = state[2]; }
```

### 3. Stochastic Angle Jitter
To avoid the mechanical, perfectly-symmetric look of pure L-Systems, **random noise is injected into every turn angle**:

```csharp
else if (c == '+') angle += (angleDelta + (float)(rng.NextDouble() * 10 - 5));
```

This mimics natural environmental variability — no two renders are identical, giving the tree an organic, hand-drawn quality.

### 4. Coordinate Normalisation & Aspect Ratio Correction
After all lines are generated, the renderer **normalises the floating-point coordinate space** to fit the terminal grid. A `widthFactor = 2` compensates for the fact that console characters are roughly twice as tall as they are wide, preventing the tree from appearing squashed.

```csharp
int plotX = (int)Math.Round((rawX - minX) * widthFactor);
int plotY = (int)Math.Round(rawY - minY);
```

### 5. Line Sampling (Bresenham Alternative)
Each line segment is **sampled at t = 0.0 → 1.0 in steps of 0.05** (parametric interpolation), plotting the `█` character at each point. This avoids gaps in diagonal lines without requiring a full Bresenham algorithm implementation.

### 6. Terminal Animation
The tree is grown progressively by passing **substrings of increasing length** to the interpreter, re-rendering with `Console.Clear()` every 5 characters — producing a real-time growth animation with `Thread.Sleep` pacing.

---

## Installation & Usage

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or later)
- Any terminal that supports Unicode block characters (`█`)

### Run

```bash
git clone https://github.com/77natsu77/lsystem-tree-csharp.git
cd lsystem-tree-csharp
dotnet run
```

> **Tip:** Run in Windows Terminal, macOS Terminal, or any modern Linux terminal for best Unicode rendering. Avoid the classic Windows `cmd.exe`.

### Customisation

Open `Program.cs` and tweak:

```csharp
// Change the branching rule
{ 'F', "FF+[+F-F-F]-[-F+F+F]" }

// Increase for a denser, more complex tree (warning: exponential growth!)
int generations = 3;

// Change the base branching angle (25° = realistic plant)
float angleDelta = 25f;

// Speed of the animation (milliseconds per frame)
Thread.Sleep(100);
```

---

## Learning Outcomes

By building this project, I developed hands-on understanding of:

- **Formal Language Theory** — How production grammars (like those in Chomsky's hierarchy) rewrite symbols to generate complex structures; the direct link between CS theory and real-world simulation.
- **Recursive Data Structures** — Using a `Stack<T>` to manage state in a non-recursive traversal, understanding push/pop mechanics at a low level rather than relying on the call stack.
- **Coordinate Geometry & Trigonometry** — Converting polar movement (`angle`, `distance`) into Cartesian `(x, y)` deltas using `Math.Cos` and `Math.Sin`, and normalising a float coordinate space to an integer grid.
- **Emergent Complexity** — Observing how 3 simple rule symbols (`F`, `+`, `-`) and a stack mechanism produce visually rich, self-similar fractal structures — a core insight in computational biology and generative art.
- **Console Rendering Pipeline** — Building a mini render pipeline: geometry generation → bounds calculation → normalisation → sampling → rasterisation → display.
- **Stochastic Simulation** — Injecting controlled randomness to break perfect symmetry and produce naturalistic output, a technique used widely in procedural generation and game development.

---

## Possible Extensions

- [ ] Add multiple axioms/rule sets (Sierpiński triangle, dragon curve, Koch snowflake)
- [ ] Export the line list to an SVG file for high-resolution output
- [ ] Port the renderer to a WinForms/WPF canvas for smooth graphical rendering
- [ ] Add a colour gradient (green → brown) based on branch depth
- [ ] Implement a GUI parameter slider for real-time rule editing

---

## Further Reading

- Prusinkiewicz & Lindenmayer — *The Algorithmic Beauty of Plants* (freely available online) — the canonical textbook on L-Systems
- Paul Bourke's L-System reference: [paulbourke.net](http://paulbourke.net/fractals/lsys/)

---

*Built as part of a personal exploration of generative algorithms and computational biology.*
