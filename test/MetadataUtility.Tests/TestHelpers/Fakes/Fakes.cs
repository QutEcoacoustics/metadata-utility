// <copyright file="Fakes.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group.
// </copyright>

namespace MetadataUtility.Tests.TestHelpers.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Bogus;
    using MetadataUtility.Models;
    using NodaTime;
    using NodaTime.Extensions;
    using Xunit;

    public class Fakes : IDisposable
    {
        private static readonly string[] KnownHashes = new[] { "SHA-2-256", "SHA-2-512" };
        private static readonly uint[] KnownSampleRates = new uint[] { 16_000, 22_050, 44_100, 48_000 };

        public Faker<Recording> GetRecording()
        {
            return new Faker<Recording>()
                .RuleForType(typeof(Checksum), f => this.GetChecksum())
                .RuleFor(x => x.Extension, f => f.PickRandom(".wav", ".flac"))
                .RuleFor(x => x.Stem, (f, x) => f.System.FileName())
                .RuleFor(x => x.Path, (f, x) => f.System.DirectoryPath() + '/' + x.Stem + x.Extension)
                .RuleFor(
                    x => x.StartDate,
                    f => Provenance.FileHeader.Wrap(f.Noda().ZonedDateTime.Recent(7).ToOffsetDateTime()))
                .RuleFor(x => x.DurationSeconds, f => f.Noda().Duration(Duration.FromHours(24)))
                .RuleFor(x => x.Channels, f => f.Random.UInt(1, 4))
                .RuleFor(x => x.SampleRateHertz, f => f.PickRandom(KnownSampleRates))
                .RuleFor(x => x.BitsPerSecond, f => f.Random.UInt(22050 * 16, 96000 * 16))
                .RuleFor(x => x.BitDepth, f => f.PickRandom(8, 16, 24))
                .RuleFor(x => x.MediaType, f => f.PickRandom("audio/wave", "audio/flac"))
                .RuleFor(x => x.FileLengthBytes, f => (ulong)f.Random.Long(2_000_000_000L))
                .RuleFor(x => x.EndDate, (f, x) => Provenance.Calculated.Wrap(x.StartDate?.Value + x.DurationSeconds))
                .RuleFor(x => x.StorageCardIdentifier, f => f.Random.AlphaNumeric(16))
                .RuleFor(x => x.ExpectedDurationSeconds, f => Duration.FromHours(24));
        }

        public Faker<Checksum> GetChecksum()
        {
            return new Faker<Checksum>()
                .StrictMode(true)
                .RuleFor(x => x.Type, f => f.PickRandom(KnownHashes))
                .RuleFor(x => x.Value, f => f.Random.AlphaNumeric(256 / 16));
        }

        public void Dispose()
        {
        }

        [CollectionDefinition(nameof(Fakes))]
        public class FakesCollection : ICollectionFixture<Fakes>
        {
            // This class has no code, and is never created. Its purpose is simply
            // to be the place to apply [CollectionDefinition] and all the
            // ICollectionFixture<> interfaces.
        }
    }
}
