using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TwoWayDict;

namespace TwoWayDictTest
{
    [TestClass]
    public class TwoWayDictionaryTests
    {
        private TwoWayDictionary<int, string> _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new TwoWayDictionary<int, string>();
        }

        [TestMethod]
        public void TwoWayDictionary_Add_ShouldAddItemToBothDictionaries()
        {
            int i = 1;
            string s = "one";

            _sut.Add(i, s);

            _sut._forwardDict[i].Should().Be(s);
            _sut._reverseDict[s].Should().Be(i);
            _sut[i].Should().Be(s);
            _sut.GetByKey(i).Should().Be(s);
            _sut.GetByValue(s).Should().Be(i);
        }

        [TestMethod]
        public void TwoWayDictionary_Add_ShouldUpdateItemInBothDictionaries()
        {
            int i = 1;
            string s = "changed";
            _sut.Add(i, "original");

            _sut[i] = s;

            _sut._forwardDict[i].Should().Be(s);
            _sut._reverseDict[s].Should().Be(i);
            _sut[i].Should().Be(s);
            _sut.GetByKey(i).Should().Be(s);
            _sut.GetByValue(s).Should().Be(i);
        }

        [TestMethod]
        public void TwoWayDictionary_Add_WhenKeyValueAdded_AddingValueKeyShouldFail()
        {
            int i = 1;
            string s = "one";

            _sut.Add(i, s);

            _sut.Invoking(x => x.Add(i, "anything else"))
                .Should().Throw<ArgumentException>()
                .WithMessage($"The collection already contains an item for {i}.");

            _sut.Invoking(x => x.Add(666, s))
                .Should().Throw<ArgumentException>()
                .WithMessage($"The collection already contains an item for {s}.");
        }
    }
}
