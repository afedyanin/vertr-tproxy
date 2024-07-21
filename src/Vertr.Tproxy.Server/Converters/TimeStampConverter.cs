namespace Vertr.Tproxy.Server.Converters;

internal static class TimeStampConverter
{
    public static DateTime? Convert(this Google.Protobuf.WellKnownTypes.Timestamp timeStamp)
        => timeStamp == null || (timeStamp.Seconds == 0 && timeStamp.Nanos == 0) ? null : timeStamp.ToDateTime();
}
