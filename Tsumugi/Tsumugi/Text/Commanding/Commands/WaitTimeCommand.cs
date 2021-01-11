namespace Tsumugi.Text.Commanding.Commands
{
    public class WaitTimeCommand : CommandBase
    {
        public int Time { get; }

        public WaitTimeCommand(int time)
        {
            Time = time;
        }
    }
}
