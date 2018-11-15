using NUnit.Framework;
using WhisperAPI.Models.MegaGenial;

namespace WhisperAPI.Tests.Unit.MegaGenial
{
    [TestFixture]
    public class ModuleDetectorTest
    {
        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingNoModuleThenNoneModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var module = detector.DetectModule("Je vois des dragons géants");
            Assert.AreEqual(Module.None, module);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAWireModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var module = detector.DetectModule("J'observe des fils avec des lumières et des étoiles");
            Assert.AreEqual(Module.WireComplicated, module);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAMazeModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var module = detector.DetectModule("Je vois un module avec des cercles verts et un triangle rouge");
            Assert.AreEqual(Module.Maze, module);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingASimonSaysModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var module = detector.DetectModule("Je vois un module 4 carrés de couleurs");
            Assert.AreEqual(Module.SimonSays, module);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAWireSequenceModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var module = detector.DetectModule("Je vois des chiffres de 1 à 3 et des lettres a b c avec des fils");
            Assert.AreEqual(Module.WireSequence, module);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAMemoryModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var module = detector.DetectModule("Je vois un écran avec des chiffres");
            Assert.AreEqual(Module.Memory, module);
        }
    }
}
