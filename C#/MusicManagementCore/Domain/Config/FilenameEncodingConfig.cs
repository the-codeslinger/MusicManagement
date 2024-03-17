using System.Collections.Generic;

// ReSharper disable ClassNeverInstantiated.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MusicManagementCore.Domain.Config
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
        public List<CharacterReplacement> CharacterReplacements { get; init; }

        /// <summary>
        /// Replace all HTML encoded characters in the given string with unencoded ones as
        /// listed in <cref>CharacterReplacements</cref>.
        /// </summary>
        /// <param name="value">The value in which to replace all HTML-encoded characters 
        /// that are configured.</param>
        /// <returns>A copy of the value with any configured HTML-encoded characters replaced.</returns>
        public string ReplaceCodeStrings(string value)
        {
            CharacterReplacements.ForEach(c => value = c.ReplaceCode(value));
            return value;
        }

        /// <summary>
        /// Replace all human-readable characters in the given string with HTML encoded ones as
        /// listed in <cref>CharacterReplacements</cref>.
        /// </summary>
        /// <param name="value">The value in which to replace all human-readable characters 
        /// with the configured HTML ones.</param>
        /// <returns>A copy of the value with any configured human-readable character replaced.</returns>
        public string InsertCodeStrings(string value)
        {
            CharacterReplacements.ForEach(c => value = c.InsertCode(value));
            return value;
        }
    }
}