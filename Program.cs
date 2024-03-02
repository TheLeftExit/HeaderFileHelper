/*
string[] headerFilePaths = [
    @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\shared\windowsx.h",
    @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\um\WinUser.h",
    @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\um\CommCtrl.h"
];*/

var rootDir = @"C:\Program Files (x86)\Windows Kits\10\Include";
var headerFilePaths = Directory.EnumerateFiles(rootDir, "*.h", SearchOption.AllDirectories)
    .Where(x => !x.Contains("winrt"));

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

;