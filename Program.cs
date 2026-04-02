using System;
using System.Windows.Forms;

namespace QuickPinner
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new QuickPinnerForm());
        }
    }
}