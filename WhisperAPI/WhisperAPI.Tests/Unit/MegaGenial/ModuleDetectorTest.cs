﻿using NUnit.Framework;
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
            var modules = detector.DetectModuleList("Je vois des dragons géants");
            Assert.IsEmpty(modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAWireModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("J'observe des fils avec des lumières et des étoiles");
            Assert.Contains((Module.WireComplicated, 3), modules);
        }

        [Test]
        [TestCase]
        public void RealLifeTest1()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("6fil");
            Assert.Contains((Module.WireComplicated, 1), modules);
        }

        [Test]
        [TestCase]
        public void RealLifeTest2()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("Écran");
            Assert.Contains((Module.WhosFirst, 1), modules);
            Assert.Contains((Module.Memory, 1), modules);
        }

        [Test]
        [TestCase]
        public void RealLifeTest3()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("Code");
            Assert.Contains((Module.Password, 1), modules);
        }

        [Test]
        [TestCase]
        public void RealLifeTest4()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("Il y a un ecran vert et il es ecrit");
            Assert.Contains((Module.Password, 2), modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAMazeModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("Je vois un module avec des cercles verts et un triangle rouge");
            Assert.Contains((Module.Maze, 4), modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingASimonSaysModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("Je vois un module 4 carrés de couleurs");
            Assert.Contains((Module.SimonSays, 2), modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAWireSequenceModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("Je vois des chiffres de 1 à 3 et des lettres a b c avec des fils");
            Assert.Contains((Module.WireSequence, 4), modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAMemoryModuleThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("Je vois un écran avec des chiffres");
            Assert.Contains((Module.Memory, 2), modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingAMemoryModuleWithTyposThenSaidModuleIsReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("Je vois un ecran avec des chifre");
            Assert.Contains((Module.Memory, 1), modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingManyModulesThenSaidModulesAreReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("J'aime les fils");
            Assert.Contains((Module.WireComplicated, 1), modules);
            Assert.Contains((Module.WireSequence, 1), modules);
            Assert.Contains((Module.WireSimple, 1), modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingManyModulesWithATypoThenSaidModulesAreReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("J'aime les fisl");
            Assert.Contains((Module.WireComplicated, 1), modules);
            Assert.Contains((Module.WireSequence, 1), modules);
            Assert.Contains((Module.WireSimple, 1), modules);
        }

        [Test]
        [TestCase]
        public void WhenSentenceIsDescribingManyModulesWithAMajorTypoThenNoModulesAreReturned()
        {
            var detector = new ModuleDetector();
            var modules = detector.DetectModuleList("J'aime les filles");
            Assert.IsEmpty(modules);
        }
    }
}
