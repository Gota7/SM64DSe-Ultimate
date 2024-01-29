using System.Runtime.InteropServices;

namespace SM64DSe
{
    public class ConsoleUtils
    {
        // Check if the application is started from a console
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;

        public static bool AttachConsole()
        {
            return AttachConsole(ATTACH_PARENT_PROCESS);
        }
    }
}