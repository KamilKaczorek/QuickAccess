using System;
using System.IO;
using System.Linq;

namespace QuickAccess.ExpressionParser.Demo.CodeAnalysis
{
    public sealed class CSharpCodeLinesAnalyzer
    {
        public sealed class CodeLinesInfo
        {
            internal CodeLinesInfo(
                in int classesCount,
                in int structsCount,
                in int enumsCount,
                in int interfacesCount,
                in int numberOfLines,
                in int filesCount)
            {
                ClassesCount = classesCount;
                StructsCount = structsCount;
                EnumsCount = enumsCount;
                InterfacesCount = interfacesCount;
                LinesCount = numberOfLines;
                FilesCount = filesCount;
            }


            public int ClassesCount { get; }
            public int StructsCount { get; }
            public int EnumsCount { get; }
            public int InterfacesCount { get; }

            public int TotalTypesCount => ClassesCount + StructsCount + EnumsCount + InterfacesCount;

            public int FilesCount { get; }
            public int LinesCount { get; }

            public override string ToString()
            {
                return
                    $"FilesCount={FilesCount}, LinesCount={LinesCount}, TotalTypes={TotalTypesCount} (Interfaces={InterfacesCount}, Classes={ClassesCount}, Structs={StructsCount}, Enums={EnumsCount})";
            }
        }

        public static CodeLinesInfo CalculateNumberOfLines(string folder)
        {
            var filesCount = 0;
            var linesCount = 0;
            var classCount = 0;
            var structCount = 0;
            var enumCount = 0;
            var interfacesCount = 0;


            foreach (var csFile in Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories))
            {
                filesCount++;
                var content = File.ReadAllText(csFile);

                linesCount += content.Count(c => c == '\n');

                classCount += GetWholeWordCount(content, "class");
                structCount += GetWholeWordCount(content, "struct");
                enumCount += GetWholeWordCount(content, "enum");
                interfacesCount += GetWholeWordCount(content, "interface");
            }


            return new CodeLinesInfo(classCount, structCount, enumCount, interfacesCount, linesCount, filesCount);
        }



        private static int GetWholeWordCount(string content, string txt)
        {
            if (string.IsNullOrWhiteSpace(txt) || string.IsNullOrWhiteSpace(content) || content.Length < txt.Length)
            {
                return 0;
            }

            var count = 0;
            var idx = -1;
            do
            {
                idx = content.IndexOf(txt, idx + 1, StringComparison.Ordinal);

                if (idx >= 0)
                {
                    if (idx > 0)
                    {
                        var prev = content[idx - 1];

                        if (!(prev == '\n' || prev == '\r' || prev == '\t' || prev == ' '))
                        {
                            continue;
                        }
                    }

                    if (idx + txt.Length < content.Length)
                    {
                        var next = content[idx + txt.Length];

                        if (!(next == '\n' || next == '\r' || next == '\t' || next == ' '))
                        {
                            continue;
                        }
                    }

                    count++;
                }
            }
            while (idx >= 0);

            return count;
        }
    }
}