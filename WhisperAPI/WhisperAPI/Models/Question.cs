using System;

namespace WhisperAPI.Models
{
    public enum QuestionStatus
    {
        /// <summary>
        /// Question was generated and probably sent to client and showed in the UI
        /// </summary>
        None,

        /// <summary>
        /// Question was clicked in the UI, we don't know if it was sent or not yet.
        /// </summary>
        Clicked,

        /// <summary>
        /// Question was detected in an agent message and we are expecting answer in upcoming messages
        /// </summary>
        AnswerPending,

        /// <summary>
        /// Question was answered
        /// </summary>
        Answered,

        /// <summary>
        /// Question rejected
        /// </summary>
        Rejected,
    }

    public abstract class Question
    {
        public Guid Id { get; set; }

        public QuestionStatus Status { get; set; }

        public abstract string Text { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Question)obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        protected bool Equals(Question other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}
