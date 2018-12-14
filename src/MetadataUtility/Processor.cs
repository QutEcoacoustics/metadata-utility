// <copyright file="Processor.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group.
// </copyright>

namespace MetadataUtility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using MetadataUtility.Filenames;
    using MetadataUtility.Models;
    using MetadataUtility.Utilities;
    using Microsoft.Extensions.Logging;
    using NodaTime;

    /// <summary>
    /// Processes each audio recording.
    /// Has metadata extraction methods, as well as transforms, and deep data quality checks.
    /// </summary>
    public class Processor
    {
        private readonly ILogger<Processor> logger;
        private readonly FilenameParser filenameParser;
        private readonly OutputWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="filenameParser">A filename parser.</param>
        /// <param name="writer">The sink to send the output.</param>
        public Processor(ILogger<Processor> logger, FilenameParser filenameParser, OutputWriter writer)
        {
            this.logger = logger;
            this.filenameParser = filenameParser;
            this.writer = writer;
        }

        /// <summary>
        /// Processes a single recording.
        /// </summary>
        /// <param name="path">The path to the file to process.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Recording> ProcessFile(string path)
        {
            this.logger.LogInformation("Processing file {0}", path);

            await Task.Yield();

            var recording = new Recording();
            recording.Path = path;
            recording.Stem = Path.GetFileNameWithoutExtension(path);

            // step 0. validate
            var file = new FileInfo(path);
            if (!file.Exists)
            {
                throw new FileNotFoundException($"Could not find the file {file}");
            }

            // step 1. parse filename
            var parsedName = this.filenameParser.Parse(file.Name);
            recording.Extension = parsedName.Extension;

            if (parsedName.OffsetDateTime.HasValue)
            {
                recording.StartDate = parsedName.OffsetDateTime.Value;
            }
            else
            {
                this.logger.LogWarning("Could not unambiguously parse date for {0}", path);
            }

            recording.Location = parsedName.Location;

            //recording.StartDate = MetadataSource<OffsetDateTime>.Provenance.Calculated.Wrap<OffsetDateTime>(parsedName.OffsetDateTime.Value);
            this.logger.LogDebug("Parsed filename: {@0}", parsedName);

            this.logger.LogDebug("Completed file {0}", path);
            return await Task.FromResult(recording);
        }

        /// <summary>
        /// Renames a file according to an archival standard.
        /// </summary>
        /// <param name="recording">The metadata required to rename the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Recording> RenameFile(Recording recording)
        {
            await Task.Yield();

            return recording;
        }

        /// <summary>
        /// Performs deep checks of the target recording.
        /// </summary>
        /// <remarks>
        /// Deep checks are checks that analyze all the frames or bytes of a given file.
        /// </remarks>
        /// <param name="recording">The recording to analyze.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Recording> DeepCheck(Recording recording)
        {
            await Task.Yield();

            return recording;
        }

        /// <summary>
        /// Writes the recording out to a sink.
        /// </summary>
        /// <param name="recording">The recording to write.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Recording> Write(Recording recording)
        {
            await Task.Run(
                () =>
                {
                    lock (this.writer)
                    {
                        this.writer.Write(recording);
                    }
                });

            return recording;
        }

        /// <summary>
        /// Runs through all the process steps.
        /// </summary>
        /// <param name="path">The path to the recording to process.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Recording> All(string path)
        {
            var recording = await this.ProcessFile(path);
            recording = await this.Write(recording);

            return recording;
        }
    }
}
