namespace Tsumugi.Text.Commanding.Commands
{
    public class JumpCommand : CommandBase
    {
        public string Target { get; }

        public JumpCommand(string target)
        {
            Target = target;
        }
    }
}
