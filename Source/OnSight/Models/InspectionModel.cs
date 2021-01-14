using System;
using System.ComponentModel;
using SQLite;

namespace OnSight
{
    public record InspectionModel
    {
        public InspectionModel() => Id = Guid.NewGuid().ToString();

        [Unique, PrimaryKey]
        public string Id { get; init; }

        public string InspectionTitle { get; init; } = string.Empty;

        public DateTimeOffset InspectionDateUTC { get; init; }

        public string InspectionNotes { get; init; } = string.Empty;
    }
}

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class IsExternalInit { }
}