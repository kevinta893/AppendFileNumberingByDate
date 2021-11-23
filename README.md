# AppendFileNumberingByDate
Renames files by appending a 0 padded number at the end of them by date in ascending date/time order.

Skips renaming if the file has already been renamed the intended number. Deleted files will cause subsequent files in the numbering scheme to be renamed and name collisions may occur.

## How to Run

Rename all files in directory
```
./AppendNumberingFilenameByDate.exe -d "PathToDirectoryToRenameFilesIn"
```

Rename files in the last 7 days in directory
```
./AppendNumberingFilenameByDate.exe -d "PathToDirectoryToRenameFilesIn" -l 7
```

## Options

| Flag | Name | Description |
|-------------|-----------------------|-------------|
| -d | --directory-path | Folder to rename files in |
| -l | --last-days | The number of recent days (inclusive) to look at and rename. Set negative to set no filter. Set zero for current day only. |
