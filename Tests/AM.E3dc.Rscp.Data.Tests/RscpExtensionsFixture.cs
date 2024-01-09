using System;
using AM.E3dc.Rscp.Data.Values;
using FluentAssertions;
using Xunit;

namespace AM.E3dc.Rscp.Data.Tests
{
    public class RscpExtensionsFixture
    {
        private readonly RscpInt32 valueInContainer;
        private readonly RscpInt8 valueOutsideOfContainer;
        private readonly RscpFrame frame;

        public RscpExtensionsFixture()
        {
            this.valueInContainer = new RscpInt32(RscpTag.BAT_CHARGE_CYCLES, 600);
            this.valueOutsideOfContainer = new RscpInt8(RscpTag.BAT_INDEX, 0);
            var container = new RscpContainer(RscpTag.BAT_DATA) { this.valueInContainer };
            this.frame = new ()
            {
                this.valueOutsideOfContainer,
                container
            };
        }

        [Fact]
        public void ReturnsNullIfValueTypeNotFound()
        {
            RscpValue result = null;
            var action = new Action(
                () =>
                {
                    result = this.frame.Get<RscpInt64>(RscpTag.BAT_CHARGE_CYCLES);
                });

            action.Should()
                .NotThrow();
            result.Should()
                .BeNull();
        }

        [Fact]
        public void ReturnsNullIfRscpTagNotFound()
        {
            RscpValue result = null;
            var action = new Action(
                () =>
                {
                    result = this.frame.Get<RscpInt32>(RscpTag.EMS_PV_ENERGY);
                });

            action.Should()
                .NotThrow();
            result.Should()
                .BeNull();
        }

        [Fact]
        public void CanRetrieveValueFromFrameThatIsNotInAContainer()
        {
            RscpValue result = null;
            var action = new Action(
                () =>
                {
                    result = this.frame.Get<RscpInt8>(RscpTag.BAT_INDEX);
                });

            action.Should().NotThrow();
            result.Should().BeEquivalentTo(this.valueOutsideOfContainer);
        }

        [Fact]
        public void CanRetrieveValueFromFrameThatIsInAContainer()
        {
            RscpValue result = null;
            var action = new Action(
                () =>
                {
                    result = this.frame.Get<RscpInt32>(RscpTag.BAT_CHARGE_CYCLES);
                });

            action.Should().NotThrow();
            result.Should().BeEquivalentTo(this.valueInContainer);
        }
    }
}
