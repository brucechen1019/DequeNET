﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DequeNet.Test.Common;
using Xunit;
using Xunit.Extensions;

namespace DequeNet.Unit
{
    public partial class ConcurrentDequeFixture
    {
        private const int ThreadCount = 20;
        private const int RunningTime = 3000;

        // ReSharper disable AccessToModifiedClosure
        [Fact]
        [Trait("Category", "LongRunning")]
        public void ConcurrentPushRightMaintainsRightPointersIntegrity()
        {
            //Arrange
            long pushCount = 0;
            bool cancelled = false;


            var deque = new ConcurrentDeque<int>();

            //keep adding items to the deque
            ThreadStart pushRight = () =>
                                     {
                                         while (!cancelled)
                                         {
                                             deque.PushRight(0);
                                             Interlocked.Increment(ref pushCount);
                                         }
                                     };

            //Act
            pushRight.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

            //Assert
            //traverse the deque from left to right
            long nodesCount = deque.GetNodes().LongCount();
            Assert.True(nodesCount > 0);
            Assert.Equal(pushCount, nodesCount);
        }

        [Fact]
        [Trait("Category", "LongRunning")]
        public void ConcurrentPushRightMaintainsLeftPointersIntegrity()
        {
            //Arrange
            long pushCount = 0;
            bool cancelled = false;

            var deque = new ConcurrentDeque<int>();

            //keep adding items to the deque
            ThreadStart pushRight = () =>
            {
                while (!cancelled)
                {
                    deque.PushRight(0);
                    Interlocked.Increment(ref pushCount);
                }
            };

            //Act
            pushRight.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

            //Assert
            //traverse the deque from right to left
            long nodesCount = deque.GetNodesReverse().LongCount();
            Assert.True(nodesCount > 0);
            Assert.Equal(pushCount, nodesCount);
        }

        [Fact]
        [Trait("Category", "LongRunning")]
        public void ConcurrentPushRightMaintainsValueIntegrity()
        {
            //Arrange
            long sum = 0;
            bool cancelled = false;

            var deque = new ConcurrentDeque<int>();

            //keep adding items to the deque
            ThreadStart pushRight = () =>
            {
                Random rnd = new Random();

                while (!cancelled)
                {
                    int val = rnd.Next(1, 11);
                    deque.PushRight(val);
                    Interlocked.Add(ref sum, val);
                }
            };

            //Act
            pushRight.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

            //Assert
            //traverse the deque from left to right
            long actualSum = deque.GetNodes().Sum(n => n._value);
            Assert.True(actualSum > 0);
            Assert.Equal(sum, actualSum);
        }

        [Fact]
        [Trait("Category", "LongRunning")]
        public void TryPopRightIsAtomic()
        {
            //Arrange
            const int initialCount = 5000000;
            const double stopAt = initialCount*0.9;

            int popCount = 0;
            var deque = new ConcurrentDeque<int>();

            for (int i = 0; i < initialCount; i++)
                deque.PushRight(i);

            ThreadStart popRight = () =>
                                    {
                                        while (popCount <= stopAt)
                                        {
                                            int i;
                                            Assert.True(deque.TryPopRight(out i));
                                            Interlocked.Increment(ref popCount);
                                        }
                                    };
            //Act
            popRight.RunInParallel(ThreadCount);

            //Assert
            int remainingNodes = deque.GetNodes().Count();
            Assert.True(remainingNodes > 0);
            Assert.Equal(initialCount - popCount, remainingNodes);
        }

        [Fact]
        [Trait("Category", "LongRunning")]
        public void ConcurrentPushLeftMaintainsRightPointersIntegrity()
        {
            //Arrange
            long pushCount = 0;
            bool cancelled = false;
            var deque = new ConcurrentDeque<int>();

            //keep adding items to the deque
            ThreadStart pushLeft = () =>
            {
                while (!cancelled)
                {
                    deque.PushLeft(0);
                    Interlocked.Increment(ref pushCount);
                }
            };

            //Act
            pushLeft.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

            //Assert
            //traverse the deque from left to right
            long nodesCount = deque.GetNodes().LongCount();
            Assert.True(nodesCount > 0);
            Assert.Equal(pushCount, nodesCount);
        }

        [Fact]
        [Trait("Category", "LongRunning")]
        public void ConcurrentPushLeftMaintainsLeftPointersIntegrity()
        {
            //Arrange
            long pushCount = 0;
            bool cancelled = false;

            var deque = new ConcurrentDeque<int>();

            //keep adding items to the deque
            ThreadStart pushLeft = () =>
            {
                while (!cancelled)
                {
                    deque.PushLeft(0);
                    Interlocked.Increment(ref pushCount);
                }
            };

            //Act
            pushLeft.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

            //Assert
            //traverse the deque from right to left
            long nodesCount = deque.GetNodesReverse().LongCount();
            Assert.True(nodesCount > 0);
            Assert.Equal(pushCount, nodesCount);
        }

        [Fact]
        [Trait("Category", "LongRunning")]
        public void ConcurrentPushLeftMaintainsValueIntegrity()
        {
            //Arrange
            long sum = 0;
            bool cancelled = false;

            var deque = new ConcurrentDeque<int>();

            //keep adding items to the deque
            ThreadStart pushLeft = () =>
            {
                Random rnd = new Random();

                while (!cancelled)
                {
                    int val = rnd.Next(1, 11);
                    deque.PushLeft(val);
                    Interlocked.Add(ref sum, val);
                }
            };

            //Act
            pushLeft.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

            //Assert
            //traverse the deque from left to right
            long actualSum = deque.GetNodes().Sum(n => n._value);
            Assert.True(actualSum > 0);
            Assert.Equal(sum, actualSum);
        }

        [Fact]
        [Trait("Category", "LongRunning")]
        public void TryPopLeftIsAtomic()
        {
            //Arrange
            const int initialCount = 5000000;
            const double stopAt = initialCount * 0.9;

            int popCount = 0;
            var deque = new ConcurrentDeque<int>();

            for (int i = 0; i < initialCount; i++)
                deque.PushLeft(i);

            ThreadStart popLeft = () =>
            {
                while (popCount <= stopAt)
                {
                    int i;
                    Assert.True(deque.TryPopLeft(out i));
                    Interlocked.Increment(ref popCount);
                }
            };
            //Act
            popLeft.RunInParallel(ThreadCount);

            //Assert
            int remainingNodes = deque.GetNodes().Count();
            Assert.True(remainingNodes > 0);
            Assert.Equal(initialCount - popCount, remainingNodes);
        }

        [Fact]
        public void EnumeratorIncludesItemsConcurrentlyPushedOntoTheRightEnd()
        {
            //Arrange
            var deque = new ConcurrentDeque<int>();
            deque.PushRight(0);
            deque.PushRight(1);

            //Act
            var iterator = deque.GetEnumerator();
            iterator.MoveNext();
            iterator.MoveNext();
            deque.PushRight(2);

            //Assert
            Assert.True(iterator.MoveNext());
            Assert.Equal(2, iterator.Current);
        }


        [Fact]
        public void EnumeratorIncludesItemsConcurrentlyPoppedFromTheLeftEnd()
        {
            //Arrange
            int item;
            var deque = new ConcurrentDeque<int>();
            deque.PushRight(0);
            deque.PushRight(1);

            //Act
            var iterator = deque.GetEnumerator();
            iterator.MoveNext();
            deque.TryPopLeft(out item);
            deque.TryPopLeft(out item);

            //Assert
            Assert.True(iterator.MoveNext());
            Assert.Equal(1, iterator.Current);
        }

        [Fact]
        public void EnumeratorDoesntIncludeItemsConcurrentlyPushedOntoTheLeftEnd()
        {
            //Arrange
            int item;
            var deque = new ConcurrentDeque<int>();
            deque.PushRight(0);
            deque.PushRight(1);

            //Act
            var iterator = deque.GetEnumerator();
            iterator.MoveNext();
            deque.TryPopLeft(out item);
            deque.TryPopLeft(out item);
            deque.PushLeft(2);

            //Assert
            Assert.True(iterator.MoveNext());
            Assert.Equal(1, iterator.Current);
            Assert.False(iterator.MoveNext());
        }

        [Fact]
        public void EnumeratorDoesntIncludeItemsConcurrentlyPoppedFromTheRightEnd()
        {
            //Arrange
            var deque = new ConcurrentDeque<int>();
            deque.PushRight(0);

            //Act
            var iterator = deque.GetEnumerator();
            iterator.MoveNext();
            deque.PushRight(1);

            //Assert
            Assert.True(iterator.MoveNext());
            Assert.Equal(1, iterator.Current);
            Assert.False(iterator.MoveNext());
        }

        // ReSharper enable AccessToModifiedClosure
    }
}
