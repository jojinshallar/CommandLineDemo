using System.CommandLine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Scl
{
    /// <summary>
    /// 多层多指令命令行示例
    /// </summary>
    public class QuoteCommand
    {
        //全局的fileOption
        private Option<FileInfo> fileOption;
        public async Task<int> InitCommand(string[] args)
        {
            fileOption = new Option<FileInfo>(
                name: "--file",
                description: "文件路径，不传默认为本地sampleQuotes.txt文件",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        return new FileInfo("sampleQuotes.txt");
                    }
                    string filePath = result.Tokens.FirstOrDefault()?.Value;
                    if (!File.Exists(filePath))
                    {
                        result.ErrorMessage = "文件不存在";
                        return null;
                    }
                    else
                    {
                        return new FileInfo(filePath);
                    }
                });

            var rootCommand = new RootCommand("读取文件App");
            var quotesCommand = new Command("quotes", "处理包含引号的文件");

            rootCommand.AddGlobalOption(fileOption);

            quotesCommand.AddCommand(BuildReadCommand());
            quotesCommand.AddCommand(BuildAddCommand());
            quotesCommand.AddCommand(BuildDeleteCommand());

            rootCommand.AddCommand(quotesCommand);

            return await rootCommand.InvokeAsync(args);
        }

        private Command BuildAddCommand()
        {
            var quoteArgument = new Argument<string>(
                           name: "quote",
                           description: "引用的文本");

            var bylineArgument = new Argument<string>(
                name: "quote",
                description: "引用的署名");
            var addCommand = new Command("add", "向文件中添加一个条目");
            addCommand.AddArgument(quoteArgument);
            addCommand.AddArgument(bylineArgument);
            addCommand.AddAlias("insert");

            addCommand.SetHandler(async (file, quote, byline) =>
            {
                await AddToFileAsync(file, quote, byline);
            }, fileOption, quoteArgument, bylineArgument);
            return addCommand;
        }

        private Command BuildDeleteCommand()
        {
            var searchTermsOption = new Option<string[]>(
                   name: "--search-terms",
                   description: "删除条目时要搜索的字符串。")
            {
                IsRequired = true,
                AllowMultipleArgumentsPerToken = true
            };
            var deleteCommand = new Command("delete", "从文件中删除行。");
            deleteCommand.AddOption(searchTermsOption);

            deleteCommand.SetHandler(async (file, searchTerms) =>
            {
                await DeleteFromFileAsync(file, searchTerms);
            }, fileOption, searchTermsOption);
            return deleteCommand;
        }

        private Command BuildReadCommand()
        {
            var delayOption = new Option<int>(
               name: "--delay",
               description: "读取一行后的延迟，默认为当前行字符数量的毫秒。",
               getDefaultValue: () => 42);

            var fgColorOption = new Option<ConsoleColor>(
                name: "--fgColor",
                description: "控制台中显示文本的前景色。",
                getDefaultValue: () => ConsoleColor.White);

            var lightModeOption = new Option<bool>(
                name: "--light-mode",
                description: "控制台中显示的文本背景颜色:默认为黑色，浅色模式为白色。",
                getDefaultValue: () => false);


            var readCommand = new Command("read", "读取并显示文件。"){
                delayOption,fgColorOption,lightModeOption
            };

            readCommand.SetHandler(async (file, delay, fgColor, lightMode) =>
            {
                await ReadFileAsync(file, delay, fgColor, lightMode);
            }, fileOption, delayOption, fgColorOption, lightModeOption);
            return readCommand;
        }

        async Task AddToFileAsync(FileInfo file, string quote, string byline)
        {
            System.Console.WriteLine("Adding to file");
            using (StreamWriter writer = file.AppendText())
            {
                await writer.WriteLineAsync($"{Environment.NewLine}{Environment.NewLine}{quote}");
                await writer.WriteLineAsync($"{Environment.NewLine}-{byline}");
                await writer.FlushAsync();
            }
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

        async Task DeleteFromFileAsync(FileInfo file, string[] searchTerms)
        {
            Console.WriteLine("Deleting from file");
            await File.WriteAllLinesAsync(
                file.FullName, File.ReadLines(file.FullName)
                    .Where(line => searchTerms.All(s => !line.Contains(s))).ToList());
        }
    }
}