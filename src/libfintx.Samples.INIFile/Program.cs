using libfintx.Setttings;

namespace libfintx.Samples.INIFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var iniFile = new IniFile();

            iniFile.Section("foo").Comment = "This is a test";
            iniFile.Section("foo").Set("test", "1");

            iniFile.Save("libfintx.ini");
        }
    }
}
