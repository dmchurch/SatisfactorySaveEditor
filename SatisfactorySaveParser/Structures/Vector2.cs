namespace SatisfactorySaveParser.Structures
{
    public record Vector2(float X = 0, float Y = 0)
    {
        public float X { get; set; } = X;
        public float Y { get; set; } = Y;

        public override string ToString()
        {
            return $"X: {X} Y: {Y}";
        }
    }
}
