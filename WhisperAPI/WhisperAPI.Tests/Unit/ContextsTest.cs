using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class ContextsTest
    {
        private Contexts _contexts;

        public ContextsTest()
        {
            this._contexts = new Contexts(new DbContextOptionsBuilder<Contexts>().UseInMemoryDatabase("contextDB").Options);
        }

        public DateTime GetDate()
        {
            return new DateTime(2000, 01, 01, 0, 0, 0);
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
            conversationcontext.StartDate = this.GetDate();
            this._contexts.SaveChangesAsync();

            conversationcontext.ChatKey.Should().BeEquivalentTo(chatkey);
            conversationcontext.StartDate.Should().BeCloseTo(this.GetDate());
        }

        [Test]
        [TestCase("1")]
        public void When_retrieving_existing_conversation_context_return_the_context(string chatkey)
        {
            ConversationContext conversationcontext = this._contexts[chatkey];
            conversationcontext.StartDate = this.GetDate();
            this._contexts.SaveChangesAsync();

            conversationcontext = this._contexts[chatkey];
            conversationcontext.ChatKey.Should().BeEquivalentTo(chatkey);
            conversationcontext.StartDate.Should().BeCloseTo(this.GetDate());
        }
    }
}
