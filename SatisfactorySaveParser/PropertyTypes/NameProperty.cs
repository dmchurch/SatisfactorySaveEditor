namespace SatisfactorySaveParser.PropertyTypes
{
    public class NameProperty : StrProperty
    {
        public new const string TypeName = nameof(NameProperty);
        public override string PropertyType => TypeName;

        public NameProperty(string propertyName, int index = 0) : base(propertyName, index)
        {
        }
    }
}
