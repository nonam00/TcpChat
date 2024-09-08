namespace ClientApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Form1 form = new Form1();
            form.ShowDialog();
            Application.Run();
        }
    }
}