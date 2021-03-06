﻿using Xunit;

namespace DequeNet.Tests.ConcurrentDeque
{
    public class IsEmptyTests
    {
        [Fact]
        public void IsEmptyReturnsTrueIfDequeIsEmpty()
        {
            var deque = new ConcurrentDeque<int>();

            Assert.True(deque.IsEmpty);
        }

        [Fact]
        public void IsEmptyReturnsFalseIfDequeHasItems()
        {
            var deque = new ConcurrentDeque<int>(new[] {1});

            Assert.False(deque.IsEmpty);
        }
    }
}
