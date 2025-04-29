namespace Core.Primitive
{
    public readonly struct Tick(double position, string label, bool major)
    {
        public double Position => position;
        public string Label => label;
        public bool MajorPos => major;

        public static Tick Major(double pos, string label) => new(pos, label, true);

        public static Tick Minor(double pos) => new(pos, string.Empty, false);
    }
}
