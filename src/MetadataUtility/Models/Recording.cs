// <copyright file="Recording.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group.
// </copyright>

namespace MetadataUtility.Models
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using NodaTime;

    /// <summary>
    /// A audio recording captured by a sensor or monitor.
    /// </summary>
    public class Recording
    {
        private string sourcePath;

        /// <summary>
        /// Gets or sets the path to the filename as read by the program.
        /// </summary>
        public string SourcePath
        {
            get => this.sourcePath;
            set
            {
                this.sourcePath = value;
                this.Directory = Path.GetDirectoryName(this.sourcePath);
            }
        }

        /// <summary>
        /// Gets the directory of the recording. Used internally.
        /// </summary>
        [JsonIgnore]
        public string Directory { get; private set; }

        /// <summary>
        /// Gets or sets the file extension as read by the program.
        /// It includes the period.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the name of the file as read by the program
        /// without the extension.
        /// </summary>
        public string Stem { get; set; }

        /// <summary>
        /// Gets or sets a recommended name.
        /// </summary>
        /// <remarks>
        /// This is a suggested name for the file that is better suited to archiving purposes.
        /// </remarks>
        public string RecommendedName { get; set; }

        /// <summary>
        /// Gets the original filename of the recording.
        /// </summary>
        public string Name => this.Stem + this.Extension;

        /// <summary>
        /// Gets or sets the start date of the recording.
        /// This is extracted either from the filename or from the metadata
        /// included in the recording.
        /// </summary>
        //public MetadataSource<OffsetDateTime>? StartDate { get; set; }
        public OffsetDateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets a Checksum calculated for the file.
        /// This checksum is calculated by EMU.
        /// </summary>
        public Checksum CalculatedChecksum { get; set; }

        /// <summary>
        /// Gets or sets the duration of the recording.
        /// </summary>
        public Duration DurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets the number of channels in the recording.
        /// </summary>
        public byte Channels { get; set; }

        /// <summary>
        /// Gets or sets the sample rate of the recording.
        /// </summary>
        public uint SampleRateHertz { get; set; }

        /// <summary>
        /// Gets or sets the bit rate.
        /// </summary>
        public uint BitsPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the numbers of bits used to quantize each sample.
        /// </summary>
        public byte BitDepth { get; set; }

        /// <summary>
        /// Gets or sets an IANA Media Type.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes in this file.
        /// </summary>
        public ulong FileLengthBytes { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Sensor"/> object
        /// that describes the sensor that produced this recording.
        /// </summary>
        public Sensor Sensor { get; set; }

        /// <summary>
        /// Gets or sets the location of the sensor
        /// when this recording was started.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets a list of locations captured while this
        /// recording was running.
        /// </summary>
        public IList<Location> AllLocations { get; set; }

        /// <summary>
        /// Gets or sets a list of errors found in this audio file.
        /// </summary>
        public IList<Error> Errors { get; set; } = new List<Error>();

        /// <summary>
        /// Gets or sets a list of errors found in this audio file.
        /// </summary>
        public IList<Warning> Warnings { get; set; } = new List<Warning>();

        /// <summary>
        /// Gets or sets a Checksum calculated for the file.
        /// </summary>
        /// <remarks>
        /// This is a checksum produced by the sensor.
        /// </remarks>
        public Checksum EmbeddedChecksum { get; set; }

        /// <summary>
        /// Gets or sets the date on the sensor for which this
        /// recording ended.
        /// </summary>
        /// <remarks>
        /// This field is useful for calculating drift in the sensor
        /// clock during recording.
        /// </remarks>
        public OffsetDateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier for the memory card
        /// that this recording was stored on.
        /// </summary>
        /// <remarks>
        /// Such as https://www.cameramemoryspeed.com/sd-memory-card-faq/reading-sd-card-cid-serial-psn-internal-numbers/.
        /// </remarks>
        public string StorageCardIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the Expected duration of the recording.
        /// </summary>
        public Duration? ExpectedDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets a key-value store of other information not yet codified by the standard.
        /// </summary>
        public Dictionary<string, string> OtherFields { get; set; }

        /// <summary>
        /// Gets or sets the path to the filename is it was renamed by Emu.
        /// </summary>
        public string RenamedPath { get; set; }
    }
}
