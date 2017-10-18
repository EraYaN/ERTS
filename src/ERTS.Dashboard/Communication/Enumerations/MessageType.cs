namespace ERTS.Dashboard.Communication.Enumerations
{
    public enum MessageType : byte
    {
        Unknown = 0x00, ///Default message type
        //Primary commands (0x01-0x1F)
        ModeSwitch = 0x01, ///Expects Acknowledgement

        //Status messages 0x20-0x2F
        Acknowledge = 0x20,  ///Is Acknowledgement

        //Periodic messages (0x30-0x3F)
        Telemetry = 0x30, ///Expects no Acknowledgement
        RemoteControl = 0x31, ///Expects no Acknowledgement

        //Parameter messages (0x40-0x9F)
        //ActuationParameters = 0x40, ///Expects Acknowledgement.
        ControllerParameters = 0x41, ///Expects Acknowledgement.
        MiscParameters = 0x42, ///Expects Acknowledgement.

        //Flash messages (0xA0-0xAF)
        FlashData = 0xA0, ///Expects no Acknowledgement.

        //Reserved for future use (0xB0-0xDF)

        //Exceptions, system commands and other failure mode related stuff (0xF0 - 0xFD)
        Reset = 0xFB, ///Expects Acknolegdement. Resets the Embedded System
        Kill = 0xFC, ///Expects Acknolegdement. Kills all activity
        Exception = 0xFD ///Expects no Acknolegdement. Reports exception to peer.

        //Reserved (0xFE-0xFF)
    }
}
