using CommandLine;
using Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Base64Encode
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await Parser
                .Default
                .ParseArguments<CommandLineOptions>(args)
                .MapResult(async (CommandLineOptions opts) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(opts.Data) && string.IsNullOrEmpty(opts.Input))
                        {
                            Console.WriteLine("You must specify a file path with -i <path> or a text to convert.");
                            return -1;
                        }

                        string result;
                        if (!string.IsNullOrEmpty(opts.Input))
                        {
                            // Buffer size must be multiple of 3 in face of the nature of base64 encoding.
                            var bufferSize = 900;
                            var buffer = new char[bufferSize];
                            using (var fs = new FileStream(opts.Input, FileMode.Open, FileAccess.Read))
                            using (var reader = new StreamReader(fs))
                            {
                                int readChars;
                                do
                                {
                                    readChars = await reader.ReadAsync(buffer, 0, buffer.Length);
                                    var text = string.Concat(buffer).Substring(0, readChars);

                                    if (string.IsNullOrEmpty(opts.Output))
                                        Console.Write(Base64Handler.Encode(text));
                                    else
                                        return 1;
                                } while (readChars != 0);
                            }
                        }
                        else
                        {
                            if (opts.Decode)
                                result = Base64Handler.Decode(opts.Data);
                            else if (opts.Encode)
                                result = Base64Handler.Encode(opts.Data);
                            else
                            {
                                Console.WriteLine("You must specify it the text is going to be encoded with -e or decoded with -d.");
                                return -1;
                            }
                        }

                        return 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        return -3; // Unhandled error
                    }
                },
                errs => Task.FromResult(-1)); // Invalid arguments
        }
    }
}
