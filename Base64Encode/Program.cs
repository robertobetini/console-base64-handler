using CommandLine;
using Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Base64Encode
{
    class Program
    {
        // Buffer size must be multiple of 6 and 8 in face of the nature of base64 encoding.
        const int BUFFER_SIZE = 1200;

        static async Task<int> Main(string[] args)
        {
            return await Parser
                .Default
                .ParseArguments<CommandLineOptions>(args)
                .MapResult(async (CommandLineOptions opts) =>
                {
                    try
                    {
                        ValidateBufferSize(BUFFER_SIZE);

                        if (string.IsNullOrEmpty(opts.Data) && string.IsNullOrEmpty(opts.Input))
                        {
                            Console.WriteLine("You must specify a file path with -i <path> or a text to convert.");
                            return -1;
                        }

                        if (!string.IsNullOrEmpty(opts.Input))
                            await ReadFromFileAndWriteToConsole(opts);
                        else
                            return ProcessDataAndWriteToConsole(opts.Data, opts.Decode);

                        return 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return -3; // Unhandled error
                    }
                },
                errs => Task.FromResult(-1)); // Invalid arguments
        }

        static void ValidateBufferSize(int bufferSize)
        {
            if (bufferSize % 6 != 0 || bufferSize % 8 != 0)
                throw new Exception($"Invalid buffer size: {bufferSize} B.");
        }

        static async Task<int> ReadFromFileAndWriteToConsole(CommandLineOptions opts)
        {
            var buffer = new char[BUFFER_SIZE];
            using (var reaferFs = new FileStream(opts.Input, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(reaferFs))
            {
                int readChars;
                do
                {
                    readChars = await reader.ReadAsync(buffer, 0, buffer.Length);
                    var text = string.Concat(buffer).Substring(0, readChars);

                    if (string.IsNullOrEmpty(opts.Output))
                    {
                        ProcessDataAndWriteToConsole(text, opts.Decode);
                    }
                    else
                    {
                        using (var writerFs = new FileStream(opts.Output, FileMode.OpenOrCreate, FileAccess.Write))
                        using (var writer = new StreamWriter(writerFs))
                        {
                            if (opts.Decode)
                                await writer.WriteAsync(Base64Handler.Decode(text));
                            else
                                await writer.WriteAsync(Base64Handler.Encode(text));
                        }
                            return 1;
                    }
                } while (readChars != 0);
            }

            return 1;
        }

        static int ProcessDataAndWriteToConsole(string text, bool isDecode)
        {
            if (isDecode)
                Console.WriteLine(Base64Handler.Decode(text));
            else
                Console.WriteLine(Base64Handler.Encode(text));
            return 0;
        }
    }
}
