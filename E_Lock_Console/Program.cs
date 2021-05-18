using System;

namespace E_Lock_Console
{
    class MainClass
    {
        private static Core app;

        public static void Main(string[] args)
        {
            app = new Core();
            app.Run();
        }
    }
}
