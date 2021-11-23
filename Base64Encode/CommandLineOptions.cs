using CommandLine;

namespace Base64Encode
{
    public class CommandLineOptions
    {
        [Value(index: 0, Required = false, HelpText = "Text to encode or decode to base64.")]
        public string Data { get; set; }
        [Option(shortName: 'd', longName: "decode", HelpText = "Decode to base64.")]
        public bool Decode { get; set; }
        [Option(shortName: 'i', longName: "input", HelpText = "File to convert.")]
        public string Input { get; set; }
        [Option(shortName: 'o', longName: "output", HelpText = "Output path of converted file.")]
        public string Output { get; set; }
        [Option(longName: "help", HelpText = "Display help text.")]
        public bool Help { get; set; }
    }
}
