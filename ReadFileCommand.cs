using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scl
{
    /// <summary>
    /// 多层单命令命令行示例
    /// </summary>
    public class ReadFileCommand
    {
        public async Task<int> InitCommand(string[] args)
        {
            var fileOption = new Option<FileInfo>(
               name: "--file",
               description: "The file read and display on the console."
           );
            var rootCommand = new RootCommand("Sample app for System.CommandLine");
            rootCommand.AddOption(fileOption);

            rootCommand.SetHandler((file) =>
            {
                ReadFile(file);
            }, fileOption);
            return await rootCommand.InvokeAsync(args);
        }

        void ReadFile(FileInfo file)
        {
            if (file == null)
            {
                System.Console.WriteLine($"'--file' is necessary");
                return;
            }
            File.ReadLines(file.FullName).ToList().ForEach(line => System.Console.WriteLine(line));
        }
    }

}