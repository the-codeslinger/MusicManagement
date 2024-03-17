using MusicManagementCore.Domain.ToC.V2;
using System.Collections;
using System.Collections.Generic;

namespace MusicManagementCore.Tests.Utils
{
    public class FilenameParserTests
    {
        private const string FILENAME_FULL = "Demons & Wizards#III#Power Metal#2020#01#Diabolic.wav";
        private const string FILENAME_PARTIAL = "Demons & Wizards#01#Diabolic.wav";
        private const string DELIMITER = "#";

        private static readonly List<string> TAG_FORMAT_FULL = new() {
            "Artist",
            "Album",
            "Genre",
            "Year",
            "TrackNumber",
            "Title"
        };
        private static readonly List<string> TAG_FORMAT_PARTIAL = new() {
            "Artist",
            "TrackNumber",
            "Title"
        };

        private static readonly MetaData META_DATA_FULL = new() {
            Artist = "Demons & Wizards",
            Album = "III",
            Genre = "Power Metal",
            Year = "2020",
            TrackNumber = "01",
            Title = "Diabolic"
        };
        private static readonly MetaData META_DATA_PARTIAL = new() {
            Artist = "Demons & Wizards",
            TrackNumber = "01",
            Title = "Diabolic"
        };
        private static readonly MetaData META_DATA_FORMAT_MISMATCH_1 = new() {
            Artist = "Demons & Wizards",
            TrackNumber = "III",
            Title = "Power Metal"
        };
        private static readonly MetaData META_DATA_FORMAT_MISMATCH_2 = new() {
            Artist = "Demons & Wizards",
            Album = "01",
            Genre = "Diabolic"
        };

        public class DataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new() {
                // Regularly matching case
                new object[] { FILENAME_FULL, TAG_FORMAT_FULL, META_DATA_FULL },
                new object[] { FILENAME_PARTIAL, TAG_FORMAT_PARTIAL, META_DATA_PARTIAL },
                // Asymmetric filename tags or format tags
                new object[] { FILENAME_FULL, TAG_FORMAT_PARTIAL, META_DATA_FORMAT_MISMATCH_1 },
                new object[] { FILENAME_PARTIAL, TAG_FORMAT_FULL, META_DATA_FORMAT_MISMATCH_2 },
                // Empty filename or format tags
                new object[] { "", TAG_FORMAT_FULL, new MetaData() },
                new object[] { FILENAME_FULL, new List<string>(), new MetaData() }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /*
        [Theory]
        [ClassData(typeof(DataGenerator))]
        public void ParseMetaDataFromFilename(
            string filename, List<string> tagFormat, MetaData expectedMetaData)
        {
            // Given / Arrange
            var inputConfig = new FilenameEncodingConfig {
                Delimiter = DELIMITER,
                TagFormat = tagFormat
            };
            var parser = new FilenameParser(inputConfig);

            // When / Act
            var metaData = parser.ParseMetaData(filename);

            // Then / Assert
            Assert.Equal(expectedMetaData, metaData);
        }
        */
    }
}
