﻿using System;
using System.IO;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768StreamTest
    {
        [Fact]
        public void NullArgumentConstructor_ReadMode()
        {
            Assert.Throws<ArgumentNullException>("reader", () => new Base32768Stream((TextReader)null));
        }
        [Fact]
        public void NullArgumentConstructor_WriteMode()
        {
            Assert.Throws<ArgumentNullException>("writer", () => new Base32768Stream((TextWriter)null));
        }
    }
}
