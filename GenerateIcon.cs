using System;

namespace AdvancedClock
{
    /// <summary>
    /// 图标生成程序
    /// </summary>
    class GenerateIcon
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AdvancedClock Icon Generator");
            Console.WriteLine("============================");
            Console.WriteLine();
            
            string outputPath = "icon.ico";
            
            Console.WriteLine("Generating custom clock icon...");
            SvgToIcoConverter.CreateTempIcon(outputPath);
            
            Console.WriteLine();
            Console.WriteLine("Icon generation completed!");
            Console.WriteLine($"Output file: {outputPath}");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}