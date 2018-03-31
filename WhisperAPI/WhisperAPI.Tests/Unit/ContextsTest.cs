using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class ContextsTest
    {
        private Contexts _contexts;

        public ContextsTest()//Contexts contexts)
        {
            //this._contexts = contexts;

            this._contexts = new Contexts(new DbContextOptionsBuilder<Contexts>().UseInMemoryDatabase("test").Options);
        }

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        [TestCase("1")]
        public void When_retrieving_non_existing_conversation_context_return_new_one(string chatkey)
        {
            ConversationContext conversationcontext = this._contexts[chatkey];
            conversationcontext.StartDate = DateTime.Parse("20000101000000");
            this._contexts.SaveChangesAsync();

            conversationcontext.ChatKey.Should().BeEquivalentTo(chatkey);
            conversationcontext.StartDate.Should().BeCloseTo(DateTime.Parse("20000101000000"));
        }

        [Test]
        [TestCase("1")]
        public void When_retrieving_existing_conversation_context_return_the_context(string chatkey)
        {
            ConversationContext conversationcontext = this._contexts[chatkey];
            conversationcontext.ChatKey.Should().BeEquivalentTo(chatkey);
            conversationcontext.StartDate.Should().BeCloseTo(DateTime.Parse("20000101000000"));
        }
    }
}
