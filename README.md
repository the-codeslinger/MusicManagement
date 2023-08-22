# Music Management

TODO Description.

This is a replacement of the [music-management-scripts](https://github.com/the-codeslinger/music-management-scripts) 
repository.

Remove migrated V2 table of contents files and replace it with the original V1 variant 
from the backup file.

```powershell
Get-ChildItem -Path ".\" 
    -Recurse 
    -Include "ToC.json_v1bak" | foreach { 
        Remove-Item ($_.DirectoryName + "\\ToC.json"); 
        Rename-Item -Path $_.FullName -NewName ($_.DirectoryName + "\\ToC.json")
    }
```

Clean up migration backup files after random manual verification of correctness.

```powershell
Get-ChildItem -Path ".\" -Recurse -Include "ToC.json_v1bak" | Remove-Item
```


```powershell
.\CreateToc.exe --config .\Config.json
```

```powershell
.\ConvertMusic.exe --config .\Config.json --format mp3
```