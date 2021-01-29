namespace SatisfactorySaveParser.Structures
{
    public record Vector4(float X = 0, float Y = 0, float Z = 0, float W = 1)
    {
        public float X { get; set; } = X;
        public float Y { get; set; } = Y;
        public float Z { get; set; } = Z;
        public float W { get; set; } = W;

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z} W: {W}";
        }
    }
}
