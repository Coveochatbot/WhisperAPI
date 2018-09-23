using System;

namespace WhisperAPI.Models
{
    public class Question
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

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
            unchecked
            {
                return ((this.Id != null ? this.Id.GetHashCode() : 0) * 397) ^ (this.Text != null ? this.Text.GetHashCode() : 0);
            }
        }

        protected bool Equals(Question other)
        {
            return string.Equals(this.Text, other.Text) && this.Id.Equals(other.Id);
        }
    }
}
