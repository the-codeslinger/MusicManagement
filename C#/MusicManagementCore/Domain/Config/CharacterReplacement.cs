namespace MusicManagementCore.Domain.Config;

/// <summary>
/// Contains a list of characters and their replacement code for use in file and
/// folder names.
/// 
/// NTFS and other filesystems do not allow a certain number of characters. Such
/// characters are replaced by HTML codes when audio files are initially read
/// from disc and the most important meta data is encoded in the file's name.
/// </summary>
public class CharacterReplacement
{
    /// <summary>
    /// The human-readable character, e.g., "#", "?", and more.
    /// </summary>
    public string Character { get; set; }

    /// <summary>
    /// The HTML code used to encode the character.
    /// </summary>
    public string Replacement { get; set; }

    /// <summary>
    /// Replace all occurrences of the HTML encoded <cref>Replacement</cref> in the given string
    /// with the unencoded <cref>Character</cref>.
    /// </summary>
    /// <param name="value">The value in which to replace the HTML-encoded character.</param>
    /// <returns>A copy of the value with any configured HTML-encoded character replaced.</returns>
    public string ReplaceCode(string value)
    {
        return value.Replace(Replacement, Character);
    }

    /// <summary>
    /// Replace all occurrences of the human-readable <cref>Character</cref> in the given string with 
    /// the <cref>Replacement</cref>.
    /// </summary>
    /// <param name="value">The value in which to replace the human-readable character.</param>
    /// <returns>A copy of the value with any configured human-readable character replaced.</returns>
    public string InsertCode(string value)
    {
        return value.Replace(Character, Replacement);
    }
}