namespace Tsumugi.Text.Commanding.Commands
{
    public class LabelCommand : CommandBase
    {
        public string Name { get; }

        public string Headline { get; }

        public LabelCommand(string name, string headline)
        {
            Name = name;
            Headline = headline;
        }
    }
}
