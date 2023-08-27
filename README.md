# Music Management

This repository replaces the Python [music-management-scripts](https://github.com/the-codeslinger/music-management-scripts) 
repository.

The Python scripts did their job, but contained to much copy-and-paste code. My
Python knowledge does not go far enough to properly modularize the code and share
frequently used constants and logic across different scripts. Additionally, these
scripts do not support compilations with tracks of different artists. The focus
was always on albums by a single artist. But, there is always this one exception
to the rule, and I finally wanted to digitalize it.

Therefore, I rewrote the core pieces in C# on the .NET platform. Since the target
is .NET Core, it should also work on other platforms.

## Worflow

The following workflow is expected for these utilities to make sense.

* Rip audio discs to WAV files, like a cave man, and encode all information in
  the filename.
* Parse the filenames and create a JSON file, the so-called table of contents file,
  that lists all tracks and their meta data. Rename the files to a simpler name.
* Convert all tracks by reading the ToC.json file.

## Why

1. I'm a child of the eighties. It is what we did when I was young, and I never 
   gave it up. For some reason, music is different from other digital media for me.
2. Audio discs can degrade with age and this is a form of preservation.
3. Maybe I want to change the compressed format at some point. This way of archiving
   simplifies the process.

## Usage

When audio files are ripped to WAV, all relevant meta information must be encoded
in each track's filename. My use case requires the following attributes:

* Artist
* Album
* Genre
* Year of release
* Track number
* Track title

I use '#' as a separator for all the information. Here is an example:

    Týr#Eric the Red#Pagan Metal#2006#02#Regin smiður.wav

Not all filesystems support all characters. NTFS, for example, does not allow ':'.
The pound sign '#' would also be a problem, as it is my separator.

To work around these issues, I configure [Exact Audio Copy](https://www.exactaudiocopy.de/)
to replace certain characters with HTML codes during the ripping process. When 
the table of contents file is created, the HTML codes are converted to the respective
characters again. The list of replacement codes can be customized and is not fixed
anymore, like in the Python scripts.

### Configuration Format

Instead of passing all settings on the command line, I opted for a JSON configuration
file. The settings usually do not change, so I considered it more convenient to 
figure out the settings once and reuse them over and over again. Also: all utilities
work on the same config file.

A complete working configuration example can be found [here](Config.json).

```json
{
    "Input": {
        "Path": "E:\\Music\\Compilation",
        "Recurse": true,
        "Extension": "wav"
    },
    "Output": {
        "Format": "Genre\\Artist\\Year - Album\\TrackNumber - Title",
        "Converters": [
            {
                "Type": "MP3",
                "Output": {
                    "Path": "E:\\Music\\Compressed\\MP3"
                },
                "Command": {
                    "Args": [ "-V2", "%input%", "%output%" ],
                    "Bin": "E:\\Apps\\Lame\\lame.exe"
                }
            }
        ]
    },
    "FilenameEncoding": {
        "TagFormat": [
            "Artist",
            "Album",
            "Genre",
            "Year",
            "TrackNumber",
            "Title"
        ],
        "Delimiter": "#",
        "CharacterReplacements": [
            {
                "Character": "#",
                "Replacement": "&35;"
            }
        ]
    }
}
```

**Input**

The uncompressed WAV input files. Defines the location, whether to scan recursively,
and what file extension to look out for. If the coverter supports other formats than
WAV, you may use that instead.

**Output**

This section defines the output format relative to the output path that is set in 
the converter configuration. I have listed the supported meta tags earlier in this
README. You can create a folder hierarchy based on the tags or a flat structure.
It only depends on how you construct the `Format`. No file extension is required 
here.

You can set as many encoders as you need, if you work with multiple formats. The
only placeholders passed to the converter are `%input%` and `%output%`. They contain
the uncompressed source file and the target output file.

**FilenameEncoding**

The filename parser is configured in this section. `TagFormat` defines the order 
in which the meta information appears in the filename. The `Delimiter` can be set
to anything that makes sense to you, and `CharacterReplacements` is a list of all
HTML codes that must be replaced by the corresponding characters during ToC creation.

### Table of Contents File Format

The `ToC.json` file is a list of tracks and their meta information. Version 1 of 
the file was geared towards and artist's album and managed the artist name, album 
title, genre, and year at the top level of the file. This does not work for compilations
with many different artists. Therefore, all data is managed per track, resulting
in some unfortunate duplication of data. It is the most flexible approach, though,
without using different file formats for different purposes.

Whether a record is a compilation is determined based on how many different artists
are detected. If it is more than one, it is considered a compilation.

I intend to use the hashes to detect changes to the ToC.json and update the affected
converted file's meta information. This is not implemented yet.

The `filename` property contains the long name before creating the table of contents
file and the shortened name after. This way, the source of the information is always
available for debugging.

```json
{
  "version": "2",
  "CoverHash": "F35D144050AACE3DE09588A436AD54448B213835E62BC71B86490725B02510A6",
  "tracks": [
    {
      "compilation": false,
      "artist": "Týr",
      "album": "Eric the Red",
      "genre": "Pagan Metal",
      "year": "2006",
      "number": "01",
      "track": "The edge",
      "filename": {
        "long": "Týr#Eric the Red#Pagan Metal#2006#01#The edge.wav",
        "short": "01 - The edge.wav"
      },
      "hash": "5455DA9A92AEA56AE5F65C9EF587412C356A18A25A9C1A95ED58A091BBBF9513"
    }
  ]
}
```

### Create Table of Contents

Since all information for the operation is in the configuration file, only the path
to that file is required on the command line.

Existing V1 table of contents file as created by the Python scripts are migrated to
V2 by this tool. A backup is retained with the name `ToC.json_v1bak`.

```powershell
.\CreateToc.exe --config Config.json
OR
.\CreateToc.exe -c Config.json
```

### Convert Audio Files

Converting the audio files additionally requires the converter format on the command
line.

```powershell
.\ConvertMusic.exe --config Config.json --format mp3
OR
.\ConvertMusic.exe -c Config.json -f mp3
```

## Miscellaneous

Remove migrated V2 table of contents file and replace it with the original V1 variant 
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
