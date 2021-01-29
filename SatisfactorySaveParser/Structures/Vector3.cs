namespace SatisfactorySaveParser.Structures
{
    public record Vector3(float X = 0, float Y = 0, float Z = 0)
    {
        public float X { get; set; } = X;
        public float Y { get; set; } = Y;
        public float Z { get; set; } = Z;

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }
    }
}
