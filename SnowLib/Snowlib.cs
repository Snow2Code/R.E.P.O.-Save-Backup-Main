using System.Diagnostics;

namespace SnowLib
{
    public class Snowlib
    {
        public void Output(string message)
        {
            //Console.WriteLine(message);
            Debug.WriteLine(message); // Swapped to .NET Standard 2.0, so we have to use Debug.


            //Debug.WriteLine(message);
            //MessageBox.Show(message, "SnowLib");
        }
        public void test()
        {
            Output("Test. Meow!");
        }
    }
}
