﻿using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;

namespace AM.E3dc.Rscp.Data.Tests
{
    public class RscpTimestampFixture
    {
        private readonly DateTime now = DateTime.Now;

        [Fact]
        public void CanCreateRscpTimestampFromDateTime()
        {
            var subject = new RscpTimestamp(this.now);

            subject.ToDateTime()
                .Should()
                .Be(this.now);
        }

        [Fact]
        public void CanCreateRscpTimestampFromTimeSpan()
        {
            var subject = new RscpTimestamp(TimeSpan.FromSeconds(2));

            subject.ToTimeSpan()
                .Should()
                .Be(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            var subject = new RscpTimestamp(this.now);

            var bytes = new byte[12];
            bytes.Initialize();
            var span = new Span<byte>(bytes);

            MemoryMarshal.Write(span, ref subject);

            var deserialized = MemoryMarshal.Read<RscpTimestamp>(span);
            deserialized.ToDateTime()
                .Should()
                .Be(this.now);
        }
    }
}
