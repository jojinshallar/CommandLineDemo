using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scl
{
    /// <summary>
    /// 简单一层多参数命令行示例
    /// </summary>
    public class ReadSampleQuotesCommand
    {
        public async Task<int> InitCommand(string[] args)
        {
            //创建一个名为 --file 的 FileInfo 类型的选项，并将其分配给根命令：
            var fileOption = new Option<FileInfo>(
               name: "--file",
               description: "The file read and display on the console."
           );

            var delayOption = new Option<int>(
                name: "--delay",
                description: "Delay between lines,specified as milliseconds per character in a line.",
                getDefaultValue: () => 42);

            var fgcolorOption = new Option<ConsoleColor>(
                name: "--fgcolor",
                description: "Foreground color of text displayed on the console",
                getDefaultValue: () => ConsoleColor.White);

            var lightModeOption = new Option<bool>(
                name: "--light-mode",
                description: "Background color of text displayed on the console:default is black,light mode is white"
            );

            var rootCommand = new RootCommand("Sample app for System.CommandLine");

            var readCommand = new Command("read", "Read and display the file.")
            {
                fileOption,
                delayOption,
                fgcolorOption,
                lightModeOption
            };
            
            //指定 ReadFile 是在调用根命令时调用的方法：
            readCommand.SetHandler(async (file, delay, fgcolor, lightMode) =>
            {
                await ReadFileAsync(file, delay, fgcolor, lightMode);

            }, fileOption, delayOption, fgcolorOption, lightModeOption);

            rootCommand.AddCommand(readCommand);
            return await rootCommand.InvokeAsync(args);
        }

        async Task ReadFileAsync(FileInfo file, int delay, ConsoleColor fgColor, bool lightMode)
        {
            try
            {
                if (file == null)
                {
                    System.Console.WriteLine($"'--file' is necessary");
                    return;
                }
                Console.BackgroundColor = lightMode ? ConsoleColor.White : ConsoleColor.Black;
                Console.ForegroundColor = fgColor;
                List<string> lines = File.ReadLines(file.FullName).ToList();
                foreach (string line in lines)
                {
                    System.Console.WriteLine(line);
                    await Task.Delay(delay * line.Length);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}