using System;

using SQLite;

namespace OnSight
{
    public class InspectionModel
    {
        public InspectionModel() => Id = Guid.NewGuid().ToString();

        [Unique, PrimaryKey]
        public string Id { get; set; }

        public string InspectionTitle { get; set; } = string.Empty;

        public DateTimeOffset InspectionDateUTC { get; set; }

        public string InspectionNotes { get; set; } = string.Empty;
    }
}
