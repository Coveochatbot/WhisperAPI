using System;
using System.Collections.Generic;
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
        private DateTime _startTime;

        public ContextsTest()
        {
            this._contexts = new Contexts(new DbContextOptionsBuilder<Contexts>().UseInMemoryDatabase("contextDB").Options);
            this._startTime = DateTime.Now;
        }

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        [Order(0)]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void When_retrieving_non_existing_conversation_context_then_return_new_one_with_starttime_set_to_now(string chatkey)
        {
            ConversationContext conversationcontext = this._contexts[chatkey];
            this._startTime = DateTime.Now;
            this._contexts.SaveChangesAsync();

            conversationcontext.ChatKey.Should().BeEquivalentTo(chatkey);
            conversationcontext.StartDate.Should().BeCloseTo(this._startTime, 1000);
        }

        [Test]
        [Order(1)]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void When_retrieving_existing_conversation_context_then_return_the_context(string chatkey)
        {
            ConversationContext conversationcontext = this._contexts[chatkey];
            conversationcontext.ChatKey.Should().BeEquivalentTo(chatkey);
            conversationcontext.StartDate.Should().BeCloseTo(this._startTime, 1000);
        }

        [Test]
        [Order(2)]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void When_life_span_is_not_expired_then_context_is_not_deleted(string chatkey)
        {
            IEnumerable<ConversationContext> removedContext = this._contexts.RemoveContextOlderThan(new TimeSpan(1, 0, 0, 0));

            removedContext.Should().BeEmpty();
        }

        [Test]
        [Order(3)]
        [TestCase("1")]
        public void When_life_span_expired_then_context_is_deleted(string chatkey)
        {
            ConversationContext conversationcontext = this._contexts[chatkey];
            conversationcontext.StartDate = conversationcontext.StartDate.Subtract(new TimeSpan(2, 0, 0, 0));

            this._contexts.SaveChangesAsync();
            IEnumerable<ConversationContext> removedContext = this._contexts.RemoveContextOlderThan(new TimeSpan(1, 0, 0, 0));

            removedContext.Should().OnlyContain(x => x.Equals(conversationcontext));
        }
    }
}
