using System.Drawing;

namespace SM64DSe.core.utils.SP2
{
    public class CommandInfo
    {
        public enum State
        {
            WAITING,
            SUCCESS,
            FAILED,
            WARNING,
        }
    
        public CommandInfo(string command)
        {
            this.command = command;
        }
    
        public override string ToString()
        {
            return "[" + GetStateChar() + "] " + command;
        }
    
        public SolidBrush GetTextColor()
        {
            switch (state)
            {
                case State.SUCCESS:
                    return new SolidBrush(Color.Green);
                case State.FAILED:
                    return new SolidBrush(Color.Red);
                case State.WARNING:
                    return new SolidBrush(Color.Orange);
                case State.WAITING:
                default:
                    return new SolidBrush(Color.Black);
            }
        }
    
        private string GetStateChar()
        {
            switch (state)
            {
                case State.SUCCESS:
                case State.WARNING:
                    return "V";
                case State.FAILED:
                    return "X";
                case State.WAITING:
                default:
                    return "-";
            }
        }
    
        public string command = "";
        public string description = "";
        public State state = State.WAITING;
    }
}