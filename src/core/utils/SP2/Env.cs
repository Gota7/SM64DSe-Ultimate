namespace SM64DSe.core.utils.SP2
{
    public class Env
    {
        private string Name;
        private string Value;

        public Env(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string GetName()
        {
            return this.Name;
        }

        public string GetValue()
        {
            return this.Value;
        }
    }
}