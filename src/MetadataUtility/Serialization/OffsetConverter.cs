// <copyright file="OffsetConverter.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group.
// </copyright>

namespace MetadataUtility.Serialization
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using NodaTime;
    using NodaTime.Text;

    /// <summary>
    /// A CsvHelper converter for Nodatime <see cref="Duration"/> values.
    /// </summary>
    public class OffsetConverter : DefaultTypeConverter
    {
        private static readonly OffsetPattern OffsetPattern = OffsetPattern.GeneralInvariantWithZ;

        /// <inheritdoc />
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var offset = (Offset)value;

            return OffsetPattern.Format(offset);
        }

        /// <inheritdoc />
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text == null)
            {
                // ReSharper disable once ExpressionIsAlwaysNull - throws an error
                return base.ConvertFromString(text, row, memberMapData);
            }

            return OffsetPattern.Parse(text).Value;
        }
    }
}
