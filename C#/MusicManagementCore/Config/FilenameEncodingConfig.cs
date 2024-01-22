using System.Collections.Generic;
// ReSharper disable ClassNeverInstantiated.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MusicManagementCore.Config
{
    /// <summary>
    /// Contains the configuration that is used for parsing an audio file's filename
    /// and extract the tags in the correct order.
    /// 
    /// The configuration consists of a <c>Delimiter</c> and a list of expected tags
    /// that shall be parsed as <c>TagFormat</c>. Replacement codes for invalid 
    /// filesystem characters are also part of the configuration.
    /// 
    /// The following code shows the expected format. It can be a separate JSON object
    /// or part of a bigger JSON object.
    /// 
    /// <code>
    /// {
    ///   "TagFormat": [
    ///       "artist",
    ///       "album",
    ///       "genre",
    ///       "year",
    ///       "track",
    ///       "title"
    ///   ],
    ///   "Delimiter": "#",
    ///   "CharacterReplacements": [
    ///     {
    ///       "Character": "#",
    ///       "Replacement": "&35;"
    ///     }
    ///   ]
    /// }
    /// </code>
    /// </summary>
    public class FilenameEncodingConfig
    {
        /// <summary>
        /// A single or several characters that separate a list of audio file meta
        /// data.
        /// </summary>
        public string Delimiter { get; init; }

        /// <summary>
        /// A list of tags as defined in <cref>Constant.MetaTagName</cref> that define
        /// which meta data exists and the ordering in which it appears in the filename.
        /// </summary>
        public List<string> TagFormat { get; init; }

        /// <summary>
        /// A list of all character replacement codes.
        /// </summary>
        public List<CharacterReplacement> CharacterReplacements { get; set; }

        /// <summary>
        /// Replace all HTML encoded characters in the given string with unencoded ones as
        /// listed in <cref>CharacterReplacements</cref>.
        /// </summary>
        /// <param name="value">The value in which to replace all HTML-encoded characters 
        /// that are configured.</param>
        /// <returns>A copy of the value with any configured HTML-encoded characters replaced.</returns>
        public string ReplaceCodeStrings(string value)
        {
            CharacterReplacements.ForEach(c => value = value.Replace(c.Replacement, c.Character));
            return value;
        }

        /// <summary>
        /// Remove all unencoded characters from the given string listed in <cref>CharacterReplacements</cref>.
        /// </summary>
        /// <param name="value">The value from which to remove all characters that are configured.</param>
        /// <returns>A copy of the value without the any configured unencoded characters.</returns>
        public string RemoveCodeStrings(string value)
        {
            CharacterReplacements.ForEach(c => value = value.Replace(c.Character, " "));
            return value.Replace(".", "");
        }
    }

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
    }
}