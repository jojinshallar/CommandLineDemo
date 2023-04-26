using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scl
{
    public class Program
    {
        static async Task<int> Main(string[] args)
        {
            //return await (new ReadFileCommand()).InitCommand(args);
            //return await (new ReadSampleQuotesCommand()).InitCommand(args);
            return await (new QuoteCommand()).InitCommand(args);
        }

      
    }
}