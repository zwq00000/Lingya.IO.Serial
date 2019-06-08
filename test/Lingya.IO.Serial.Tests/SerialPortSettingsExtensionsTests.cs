using System.IO.Ports;
using System.Security.Principal;
using Lingya.Views;
using Xunit;

namespace Lingya.IO.Serial.Tests {
    public class SerialPortSettingsExtensionsTests {

        [Fact]
        public void TestFrom() {
            var setting = new SerialPortSetting();
            setting.AssignFrom("COM3,9600,7,Two");
            Assert.Equal("COM3",setting.PortName);
            Assert.Equal(9600,setting.BaudRate);
            Assert.Equal(7, setting.DataBits);
            Assert.Equal(StopBits.Two,setting.StopBits);
        }

        [Fact]
        public void TestFrom1() {
            var setting = new SerialPortSetting();
            setting.AssignFrom("COM3,9600,7,Two,None,None,DTR,rts");
            Assert.Equal("COM3", setting.PortName);
            Assert.Equal(9600, setting.BaudRate);
            Assert.Equal(7, setting.DataBits);
            Assert.Equal(StopBits.Two, setting.StopBits);
            Assert.True(setting.Dtr);
            Assert.True(setting.Rts);
        }

    }

    public class PortSettingControlTests {

        [Fact]
        public void TestShowView() {
            var ctl = new PortSettingControl();
            
        }

    }
}