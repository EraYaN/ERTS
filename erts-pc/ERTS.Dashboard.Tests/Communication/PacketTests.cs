using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERTS.Dashboard.Communication;
using ERTS.Dashboard.Communication.Enumerations;
using ERTS.Dashboard.Communication.Data;

namespace ERTS.Dashboard.Communication.Tests
{
    [TestClass]
    public class PacketTests
    {
        [TestInitialize]
        public void Init()
        {
            GlobalData.InitCRC();
        }

        [TestMethod]
        public void Packet_WithValidRemoteControlInput_ToByteArray()
        {
            // arrange  
            Packet p = new Packet(MessageType.ModeSwitch)
            {
                Data = new ModeSwitchData(FlightMode.FullControl, FlightMode.Panic,0xF36A2D10)
            };

            // act  
            byte[] actual = p.ToByteArray();

            // assert  
            byte[] expected = new byte[] { 0xFE, 0xFF, (byte)MessageType.ModeSwitch, 0xB5, 0x5D, 0x05, 0x01, 0x10, 0x2D, 0x6A, 0xF3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };
            CollectionAssert.AreEqual(expected, actual, "Packet was not correctly serialized.");
        }

        [TestMethod]
        public void Packet_WithValidByteArrayInput_ToRemoteControl()
        {
            // arrange 
            byte[] input = new byte[] { 0xFE, 0xFF, (byte)MessageType.ModeSwitch, 0xB5, 0x5D, 0x05, 0x01, 0x10, 0x2D, 0x6A, 0xF3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };
            Packet p = new Packet(input);           

            // act  
            FlightMode actualNewMode = ((ModeSwitchData)p.Data).NewMode;
            FlightMode actualFallbackMode = ((ModeSwitchData)p.Data).FallbackMode;
            uint actualAckNumber = ((ModeSwitchData)p.Data).AckNumber;

            // assert  
            FlightMode expectedNewMode = FlightMode.FullControl;
            FlightMode expectedFallbackMode = FlightMode.Panic;
            uint expectedAckNumber = 0xF36A2D10;
            //Assert.IsTrue(p.Validate());
            Assert.AreEqual(expectedNewMode, actualNewMode, "Packet was not correctly deserialized, NewMode was incorrect.");
            Assert.AreEqual(expectedFallbackMode, actualFallbackMode, "Packet was not correctly deserialized, FallbackMode was incorrect.");
            Assert.AreEqual(expectedAckNumber, actualAckNumber, "Packet was not correctly deserialized, AckNumber was incorrect.");

        }

        [TestMethod]
        public void Packet_WithValidExceptionInput_ToByteArray()
        {
            // arrange  
            Packet p = new Packet(MessageType.Exception)
            {
                Data = new ExceptionData(ExceptionType.ValidationException, "No Valid Packet")
            };

            // act  
            byte[] actual = p.ToByteArray();

            // assert  
            byte[] expected = new byte[] { 0xFE, 0xFF, (byte)MessageType.Exception, 0xB5, 0x83, 0x03, 0x4E, 0x6F, 0x20, 0x56, 0x61, 0x6C, 0x69, 0x64, 0x20, 0x50, 0x61, 0x63, 0x6B, 0xFF };
            CollectionAssert.AreEqual(expected, actual, "Packet was not correctly serialized.");
        }

        [TestMethod]
        public void Packet_WithValidByteArrayInput_ToException()
        {
            // arrange 
            byte[] input = new byte[] { 0xFE, 0xFF, (byte)MessageType.Exception, 0xB5, 0x83, 0x03, 0x4E, 0x6F, 0x20, 0x56, 0x61, 0x6C, 0x69, 0x64, 0x20, 0x50, 0x61, 0x63, 0x6B, 0xFF };
            Packet p = new Packet(input);
            byte[] output = p.ToByteArray();
            // act  
            ExceptionType actualExceptionType = ((ExceptionData)p.Data).ExceptionType;
            string actualMessage = ((ExceptionData)p.Data).Message;

            // assert  
            ExceptionType expectedExceptionType = ExceptionType.ValidationException;
            string expectedMessage = "No Valid Packet".Substring(0, ExceptionData.MAX_MESSAGE_LENGTH);
            //Assert.IsTrue(p.Validate());
            Assert.AreEqual(expectedExceptionType, actualExceptionType, "Packet was not correctly deserialized, ExceptionType was incorrect.");
            Assert.AreEqual(expectedMessage, actualMessage, "Packet was not correctly deserialized, Message was incorrect.");

        }
    }
}
