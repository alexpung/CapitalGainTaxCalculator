﻿using CapitalGainCalculator.Model;
using CapitalGainCalculator.Parser;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CapitalGainCalculator.Test
{
    public class FileParseControllerTest
    {
        [Fact]
        public void TestReadingNoValidFileInFolder()
        {
            List<TaxEvent> mockResult = new() { new Trade(), new Dividend() };
            Mock<ITaxEventFileParser> mock = new();
            mock.Setup(f => f.ParseFile(It.IsAny<string>())).Returns(mockResult);
            mock.Setup(f => f.CheckFileValidity(It.IsAny<string>())).Returns(false);

            FileParseController fileParseController = new(new List<ITaxEventFileParser>() { mock.Object });
            IList<TaxEvent> result = fileParseController.ParseFolder(@".\Test\Resource");
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void TestReadingValidFileInFolder()
        {
            List<TaxEvent> mockResult = new() { new Trade(), new Dividend() };
            Mock<ITaxEventFileParser> mock = new();
            mock.Setup(f => f.ParseFile(It.IsAny<string>())).Returns(mockResult);
            mock.Setup(f => f.CheckFileValidity(It.IsAny<string>())).Returns(true);

            FileParseController fileParseController = new(new List<ITaxEventFileParser>() { mock.Object });
            IList<TaxEvent> result = fileParseController.ParseFolder(@".\Test\Resource");
            result.Count.ShouldBe(4);
        }

        [Theory]
        [InlineData(true, true, false, true, 4)]
        [InlineData(false, true, false, true, 2)]
        [InlineData(false, true, true, true, 5)]
        [InlineData(false, false, false, true, 3)]
        [InlineData(false, false, true, true, 6)]
        public void TestReadingWithTwoParsers(bool mock1Call1, bool mock1Call2, bool mock2Call1, bool mock2Call2, int expectedCount)
        {
            List<TaxEvent> mockResult = new() { new Trade(), new Dividend() };
            List<TaxEvent> mockResult2 = new() { new Trade(), new Dividend(), new StockSplit() };
            Mock<ITaxEventFileParser> mock = new();
            mock.Setup(f => f.ParseFile(It.IsAny<string>())).Returns(mockResult);
            mock.SetupSequence(f => f.CheckFileValidity(It.IsAny<string>())).Returns(mock1Call1).Returns(mock1Call2);
            Mock<ITaxEventFileParser> mock2 = new();
            mock2.Setup(f => f.ParseFile(It.IsAny<string>())).Returns(mockResult2);
            mock2.SetupSequence(f => f.CheckFileValidity(It.IsAny<string>())).Returns(mock2Call1).Returns(mock2Call2);

            FileParseController fileParseController = new(new List<ITaxEventFileParser>() { mock.Object, mock2.Object });
            IList<TaxEvent> result = fileParseController.ParseFolder(@".\Test\Resource");
            result.Count.ShouldBe(expectedCount);
        }
    }
}
