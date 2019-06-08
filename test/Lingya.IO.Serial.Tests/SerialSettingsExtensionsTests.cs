using System.IO.Ports;
using Xunit;

namespace Lingya.IO.Serial.Tests {
#if DEBUG

    public class SerialSettingsExtensionsTests {

        [Fact]
        public void TestExpression() {
            var settings = new SerialPortSetting();
            settings.SetProperty(s=>s.Handshake, "XOnXOff");
            Assert.Equal(Handshake.XOnXOff, settings.Handshake);

            Assert.NotEqual(7, settings.DataBits);
            settings.SetProperty(s=>s.DataBits,"7");
            Assert.Equal(7,settings.DataBits);
        }
    }
#endif
}