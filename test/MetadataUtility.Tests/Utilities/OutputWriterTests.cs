// <copyright file="OutputWriterTests.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group.
// </copyright>

namespace MetadataUtility.Tests.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using FluentAssertions.Primitives;
    using MetadataUtility.Models;
    using MetadataUtility.Tests.TestHelpers.Fakes;
    using MetadataUtility.Utilities;
    using Xunit;

    public class OutputWriterTests : IClassFixture<Fakes>
    {
        private readonly Fakes fakes;

        public OutputWriterTests(Fakes fakes)
        {
            this.fakes = fakes;
        }

        [Fact]
        public void OutputWriterWriteJson()
        {
            var stringBuilder = new StringBuilder(4);
            using (var stringWriter = new StringWriter(stringBuilder))
            {
                var jsonSerializer = new MetadataUtility.Serialization.JsonSerializer();

                var output = new OutputWriter(jsonSerializer, stringWriter);

                // generate a fake and write it to the stream
                var a = this.fakes.GetRecording();
                output.Write(a);

                // check the header was written
                var actual = stringBuilder.ToString();
                Assert.StartsWith($"[{Environment.NewLine}", actual);
                Assert.EndsWith("}", actual);

                // generate and write another fake
                var b = this.fakes.GetRecording();
                output.Write(b);

                // finish writing
                output.Dispose();

                actual = stringBuilder.ToString();
                Assert.StartsWith($"[{Environment.NewLine}", actual);
                Assert.EndsWith("]", actual);

                var records = jsonSerializer.Deserialize<Recording>(new StringReader(actual)).ToArray();

                Assert.Equal(records[0], a);
                Assert.Equal(records[1], b);
            }
        }

        [Fact]
        public void OutputWriterWriteCsv()
        {
            var stringBuilder = new StringBuilder(4);
            using (var stringWriter = new StringWriter(stringBuilder))
            {
                var csvSerializer = new MetadataUtility.Serialization.CsvSerializer();

                var output = new OutputWriter(csvSerializer, stringWriter);

                // generate a fake and write it to the stream
                var a = this.fakes.GetRecording();
                output.Write(a);

                // check the header was written
                var actual = stringBuilder.ToString();
                Assert.StartsWith($"{nameof(Recording.Path)},", actual);
                Assert.EndsWith(",", actual);

                // generate and write another fake
                var b = this.fakes.GetRecording();
                output.Write(b);

                // finish writing
                output.Dispose();

                actual = stringBuilder.ToString();
                Assert.StartsWith($"{nameof(Recording.Path)},", actual);
                Assert.Single(Regex.Matches(actual, $"{nameof(Recording.Path)},"));
                Assert.EndsWith(",", actual);

                var records = csvSerializer.Deserialize<Recording>(new StringReader(actual)).ToArray();

                Assert.Equal(records[0], a);
                Assert.Equal(records[1], b);
            }
        }
    }
}
