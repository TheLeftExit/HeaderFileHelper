string[] headerFilePaths = [
    @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\shared\windowsx.h",
    @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\um\WinUser.h",
    @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\um\CommCtrl.h"
];

//var rootDir = @"C:\Program Files (x86)\Windows Kits\10\Include";
//var headerFilePaths = Directory.EnumerateFiles(rootDir, "*.h", SearchOption.AllDirectories)
//    .Where(x => !x.Contains("winrt"));

var allDirectives = new List<string>();
var allInstructions = new List<string>();

foreach(var filePath in headerFilePaths)
{
    var file = File.ReadAllText(filePath);

    var fileNoComments = file.WithoutComments();

    var directives = fileNoComments
        .GetDirectives()
        .CleanUp();
    allDirectives.AddRange(directives);

    var instructions = fileNoComments
        .GetInstructions()
        .CleanUp();
    allInstructions.AddRange(instructions);
}

var constantDefinitions = allDirectives
    .Where(x =>
    {
        Span<Range> ranges = stackalloc Range[3];
        var span = x.AsSpan().Slice(1);
        if (span.Split(ranges, ' ') < 3) return false;

        var directive = span[ranges[0]];
        var symbol = span[ranges[1]];
        var value = span[ranges[2]];

        if (directive is not "define") return false;
        foreach (char c in symbol) if (char.IsLower(c) || c is '(') return false;
        return true;
    }).ToList();

;