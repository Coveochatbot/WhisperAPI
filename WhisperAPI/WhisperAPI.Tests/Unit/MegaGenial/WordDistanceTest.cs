using NUnit.Framework;
using WhisperAPI.Models.MegaGenial;

namespace WhisperAPI.Tests.Unit.MegaGenial
{
    [TestFixture]
    public class WordDistanceTest
    {
        [Test]
        [TestCase]
        public void WhenASpaceTypoIsMadeThenASmallDistanceIsReturned()
        {
            var word1 = "LeMot";
            var word2 = "Le Mot";
            var distance = DistanceCalculator.GetDistance(DistanceCalculator.ConvertWord(word1), DistanceCalculator.ConvertWord(word2));
            Assert.AreEqual(1, distance);
        }

        [Test]
        [TestCase]
        public void WhenALetterTypoIsMadeThenASmallDistanceIsReturned()
        {
            var word1 = "LeMot";
            var word2 = "LeMor";
            var distance = DistanceCalculator.GetDistance(DistanceCalculator.ConvertWord(word1), DistanceCalculator.ConvertWord(word2));
            Assert.AreEqual(1, distance);
        }

        [Test]
        [TestCase]
        public void WhenAnInversionTypoIsMadeThenASmallDistanceIsReturned()
        {
            var word1 = "LeMot";
            var word2 = "LeMto";
            var distance = DistanceCalculator.GetDistance(DistanceCalculator.ConvertWord(word1), DistanceCalculator.ConvertWord(word2));
            Assert.AreEqual(1, distance);
        }

        [Test]
        [TestCase]
        public void WhenABigTypoIsMadeThenAMaxDistanceIsReturned()
        {
            var word1 = "LeMot";
            var word2 = "LeMto43";
            var distance = DistanceCalculator.GetDistance(DistanceCalculator.ConvertWord(word1), DistanceCalculator.ConvertWord(word2));
            Assert.AreEqual(int.MaxValue, distance);
        }

        [Test]
        [TestCase]
        public void WhenNoTypeAreMadeThenANullDistanceIsReturned()
        {
            var word1 = "LeMot";
            var word2 = "LeMot";
            var distance = DistanceCalculator.GetDistance(DistanceCalculator.ConvertWord(word1), DistanceCalculator.ConvertWord(word2));
            Assert.AreEqual(0, distance);
        }
    }
}
