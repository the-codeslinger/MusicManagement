﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;

using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Event;

namespace MusicManagementCore.Service;

/// <summary>
/// Helper class to (recursively) iterate the contents of a directory to find files relevant to 
/// music management. This includes audio files to convert as well as table of contents JSON files.
/// </summary>
public class MusicMgmtFileFinder(InputConfig config)
{
    public delegate void EnterDirectoryHandler(object sender, DirectoryEvent e);

    public delegate void LeaveDirectoryHandler(object sender, DirectoryEvent e);

    public delegate void FoundAudioFileHandler(object sender, AudioFileEvent e);

    public delegate void FoundTableOfContentsFileHandler(object sender,
        TableOfContentsFileEvent e);

    /// <summary>
    /// Emitted when a new directory is scanned. Applies to recursive and non-recursive operations.
    /// </summary>
    public event EnterDirectoryHandler EnterDirectory;

    /// <summary>
    /// Emitted when scanning a directory completed. Applies to recursive and non-recursive operations.
    /// </summary>
    public event LeaveDirectoryHandler LeaveDirectory;

    /// <summary>
    /// Emitted when a file matching the configured extension <cref>InputConfig.Extension</cref> is
    /// found.
    /// </summary>
    public event FoundAudioFileHandler FoundAudioFile;

    /// <summary>
    /// Emitted when a file <cref>StandardFileName.TableOfContents</cref> is found.
    /// </summary>
    public event FoundTableOfContentsFileHandler FoundTableOfContentsFile;

    /// <summary>
    /// Scans the <cref>InputConfig.Path</cref> for files that match <cref>InputConfig.Extension</cref> 
    /// or <cref>StandardFileName.TableOfContents</cref>.
    /// </summary>
    /// <remarks>
    /// For every audio file found, a <cref>FoundAudioFile</cref> event is emitted. For every table 
    /// of contents file found, a <cref>FoundTableOfContentsFile</cref> event is emitted. 
    /// <cref>EnterDirectory</cref> and <cref>LeaveDirectory</cref> are emitted for every directory 
    /// that is scanned, even when recursive scanning is not enabled. In that case, those events 
    /// can be ignored.
    /// </remarks>
    public void Scan()
    {
        Scan(config.Path);
    }

    /// <summary>
    /// Scans the <code>uncompressedDir</code> for files that match the <cref>InputConfig.Extension</cref> 
    /// or <cref>StandardFileName.TableOfContents</cref>. This method ignores the configured input 
    /// path and overrides it with the provided parameter.
    /// </summary>
    /// <param name="uncompressedDir"></param>
    /// <remarks>
    /// For every audio file found, a <cref>FoundAudioFile</cref> event is emitted. For every table 
    /// of contents file found, a <cref>FoundTableOfContentsFile</cref> event is emitted. 
    /// <cref>EnterDirectory</cref> and <cref>LeaveDirectory</cref> are emitted for every directory 
    /// that is scanned, even when recursive scanning is not enabled. In that case, those events 
    /// can be ignored.
    /// </remarks>
    public void Scan(string uncompressedDir)
    {
        if (!Directory.Exists(uncompressedDir)) {
            throw new DirectoryNotFoundException($"Source directory {uncompressedDir} not found.");
        }

        var dirInfo = new DirectoryInfo(uncompressedDir);
        WalkRecursive(dirInfo).Wait();
    }

    private async Task WalkRecursive(DirectoryInfo root)
    {
        EnterDirectory?.Invoke(this, new DirectoryEvent { Path = root.FullName });

        foreach (var fileInfo in root.GetFiles("*.*")) {
            if (MatchesAudioFile(fileInfo)) {
                var audioFile = new UncompressedFile(fileInfo.FullName);
                FoundAudioFile?.Invoke(this, new AudioFileEvent { UncompressedFile = audioFile });
            } else if (MatchesTableOfContentsFile(fileInfo)) {
                FoundTableOfContentsFile?.Invoke(this,
                    new TableOfContentsFileEvent { FileName = fileInfo.FullName });
            }
        }

        LeaveDirectory?.Invoke(this, new DirectoryEvent { Path = root.FullName });

        if (config.Recurse) {
            var tasks = root.GetDirectories().Select(WalkRecursive);
            await Task.WhenAll(tasks);
        }
    }

    private bool MatchesAudioFile(FileSystemInfo fileInfo)
    {
        return fileInfo.Extension == "." + config.Extension;
    }

    private static bool MatchesTableOfContentsFile(FileSystemInfo fileInfo)
    {
        return fileInfo.Name == StandardFileName.TableOfContents;
    }
}