namespace DynamicQueryable {

    public enum DateTimeType {
        //
        // Summary:
        //     Do not change TimeZone for DateTime.
        Unspecified = 0,
        //
        // Summary:
        //     Change DateTime's TimeZone to UTC.
        //     Data in DB is stored as UTC
        Utc = 1,
        //
        // Summary:
        //     Change DateTime's TimeZone to Local.
        //     Data in DB is stored as Local
        Local = 2,
        //
        // Summary:
        //     Do not change DateTime's data but only switch to Utc
        //     Hack for EF Core bug, choose this to stop EF from adding zone info in Sql queries
        ForceUtc = 3
    }
}
