namespace ERTS.Dashboard.Communication.Enumerations
{
    public enum ExceptionType : byte
    {
        UnknownException = 0x00,
        InvalidModeException = 0x01,
        NotCalibratedException = 0x02,
        BadMessageTypeException = 0x03,
        BadMessageEndException = 0x04,
        MessageValidationException = 0x05
    }
}
