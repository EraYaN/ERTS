namespace ERTS.Dashboard.Communication.Enumerations
{
    public enum ExceptionType : byte
    {
        UnknownException = 0x00,
        InvalidModeException = 0x01,
        NotCalibratedException = 0x02,
        ValidationException = 0x03
    }
}
