using CommandLine;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppendNumberingFilenameByDate
{
    class Program
    {
        public class Options
        {
            [Option('d', "directory-path", Required = true, HelpText = "The folder to rename files from")]
            public string DirectoryPath { get; set; }

            [Option('l', "last-days", Required = false, HelpText = "The number of recent days (inclusive) to look at and rename. Set negative to set no filter. Set zero for current day only.")]
            public int LastDaysToRename { get; set; } = -1;
        }

        static void Main(string[] args)
        {
            var options = Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    // Check folder existance
                    if (!Directory.Exists(o.DirectoryPath))
                    {
                        Console.WriteLine($"Directory does not exist: {o.DirectoryPath}");
                        return;
                    }

                    Console.WriteLine($"Renaming files in: {o.DirectoryPath}");
                    AppendNumberingToFileNames(o.DirectoryPath, o.LastDaysToRename);
                });
        }

        /// <summary>
        /// Appends a numbering to file names by created date.
        /// Files on the same day collectively follow the same numbering
        /// Numbering is in ascending order, higher numbers is later in date
        /// </summary>
        /// <param name="path"></param>
        /// <param name="days"></param>
        private static void AppendNumberingToFileNames(string path, int days = -1)
        {
            var files = Directory.GetFiles(path)
                .Select(fp => ((DateTime CreatedDate, string Path))(File.GetLastWriteTime(fp), fp));

            //Apply date filtering
            if (days > 0)
            {
                var dateAgo = DateTime.Now.Date.AddDays(-days);
                Console.WriteLine($"Renaming all files from date (inclusive): {dateAgo.ToShortDateString()}");
                files = files.Where(f => f.CreatedDate >= dateAgo);
            }

            var filesByDate = files.GroupBy(f => f.CreatedDate.ToShortDateString());

            //Process renamings
            var totalFilesRenamed = 0;
            foreach (var dateGroup in filesByDate)
            {
                var i = 0;
                var numberOfFilesInDate = (int) Math.Floor(Math.Log10(dateGroup.Count()));          //Compute pad length on count of files
                var minPadLength = Math.Max(3, numberOfFilesInDate);

                var renamedDetectRegex = new Regex("_\\d+$");

                //Rename files in date groups
                foreach(var file in dateGroup)
                {
                    var parentFolder = Directory.GetParent(file.Path);
                    var oldFileName = Path.GetFileNameWithoutExtension(file.Path);
                    var fileExtension = Path.GetExtension(file.Path);
                    var numberPadded = i.ToString().PadLeft(minPadLength, '0');
                    var newFileName = $"{oldFileName}_{numberPadded}{fileExtension}";
                    var newFileNamePath = Path.Combine(parentFolder.FullName, newFileName);

                    // Skip rename if already renamed
                    var hasBeenRenamed = renamedDetectRegex.IsMatch(oldFileName);
                    if (!hasBeenRenamed)
                    {
                        Console.WriteLine($"Renaming file: {oldFileName}{fileExtension} |=> {newFileName}");
                        File.Move(file.Path, newFileNamePath);
                        totalFilesRenamed++;
                    }

                    i++;
                }
            }

            Console.WriteLine($"Renaming complete. Total files renamed={totalFilesRenamed}");
        }
    }
}
