﻿using System;
using Xunit;

namespace CodeConnect.MemoryMappedQueue.Tests
{
    [Collection("Basic operation tests")]
    public class BasicOperationTests : MemoryMappedQueueTests
    {
        [Fact]
        public void SimpleDataTransfer()
        {
            var initialSize = 50;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    byte[] testData = new byte[3];
                    writeQueue.Enqueue(testData);
                    var receivedData = readQueue.Dequeue();

                    Assert.Equal(testData, receivedData);
                }
            }
        }

        [Fact]
        public void EmptyQueueGivesNull()
        {
            var initialSize = 50;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {

                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    var receivedData = readQueue.Dequeue();

                    Assert.Null(receivedData);
                }
            }
        }

        [Fact]
        public void QueueAcceptsZeros()
        {
            var initialSize = 24;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    // Note that each enqueue operation writes 8 bytes (4 for data and 4 for its length)
                    byte[] testData = { 0 }; // 4 + 1
                    writeQueue.Enqueue(testData); // 5 bytes filled

                    var receivedData = readQueue.Dequeue();
                    Assert.Equal(testData, receivedData);
                }
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new byte[0])]
        public void QueueRefusesNullArray(byte[] testData)
        {
            var initialSize = 24;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    Assert.Throws(typeof(ArgumentNullException), () => writeQueue.Enqueue(testData));
                }
            }
        }

        [Fact]
        public void FillToBrinkSlowReceiver()
        {
            System.Diagnostics.Debug.WriteLine("FillToBrinkSlowReceiver");
            var initialSize = 24;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    // Note that each enqueue operation writes 8 bytes (4 for data and 4 for its length)
                    byte[] testData = { 1, 2, 3, 4 }; // 4 + 4
                    writeQueue.Enqueue(testData); // 8 bytes filled

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData2 = { 5, 6, 7, 8 }; // 4 + 4
                    writeQueue.Enqueue(testData2); // 16 bytes filled

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData3 = { 9, 10, 11, 12 }; // 4 + 4
                    writeQueue.Enqueue(testData3); // 24 bytes filled

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    var receivedData = readQueue.Dequeue();
                    Assert.Equal(testData, receivedData);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    var receivedData2 = readQueue.Dequeue();
                    Assert.Equal(testData2, receivedData2);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    var receivedData3 = readQueue.Dequeue();
                    Assert.Equal(testData3, receivedData3);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());
                }
            }
        }

        [Fact]
        public void FillToBrinkReceiveAndFillAgain()
        {
            System.Diagnostics.Debug.WriteLine("FillToBrinkReceiveAndFillAgain");
            var initialSize = 24;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    // Note that each enqueue operation writes 8 bytes (4 for data and 4 for its length)
                    byte[] testData = { 1, 2, 3, 4 }; // 4 + 4
                    writeQueue.Enqueue(testData); // 8 bytes filled

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData2 = { 5, 6, 7, 8 }; // 4 + 4
                    writeQueue.Enqueue(testData2); // 16 bytes filled

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData3 = { 9, 10, 11, 12 }; // 4 + 4
                    writeQueue.Enqueue(testData3); // 24 bytes filled

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    var receivedData = readQueue.Dequeue();
                    Assert.Equal(testData, receivedData);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData4 = { 1, 2, 3, 4 }; // 4 + 4
                    writeQueue.Enqueue(testData4); // 8 bytes filled in the overflow

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    var receivedData2 = readQueue.Dequeue();
                    Assert.Equal(testData2, receivedData2);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData5 = { 5, 6, 7, 8 }; // 4 + 4
                    writeQueue.Enqueue(testData5); // 16 bytes filled in the overflow

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    var receivedData3 = readQueue.Dequeue();
                    Assert.Equal(testData3, receivedData3);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());
                }
            }
        }

        [Fact]
        public void MultipleDataTransfersOutOfMemory()
        {
            var initialSize = 24;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    byte[] testData = { 1, 2, 3, 4 }; // 4 + 4
                    writeQueue.Enqueue(testData); // 8 bytes filled

                    byte[] testData2 = { 5, 6, 7, 8 }; // 4 + 4
                    writeQueue.Enqueue(testData2); // 16 bytes filled

                    byte[] testData3 = { 9, 10, 11, 12, 0 }; // 5 + 4
                    Assert.Throws(typeof(OutOfMemoryException), () => writeQueue.Enqueue(testData3)); // 25 bytes filled
                }
            }
        }

        [Fact]
        public void MultipleDataTransfersWithOverflow()
        {
            System.Diagnostics.Debug.WriteLine("MultipleDataTransfersWithOverflow");
            var initialSize = 32;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    byte[] testData = { 1, 2, 3, 4, 5, 6, 7, 8 }; // 8 + 4
                    writeQueue.Enqueue(testData); // 12 B filled
                    var receivedData = readQueue.Dequeue();
                    Assert.Equal(testData, receivedData);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData2 = { 9, 10, 11, 12, 13, 14, 15, 16 }; // 8 + 4
                    writeQueue.Enqueue(testData2); // 24 B filled
                    var receivedData2 = readQueue.Dequeue();
                    Assert.Equal(testData2, receivedData2);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData3 = { 17, 18, 19, 20 }; // 4 + 4
                    writeQueue.Enqueue(testData3); // 32 B filled
                    var receivedData3 = readQueue.Dequeue();
                    Assert.Equal(testData3, receivedData3);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData4 = { 21, 22, 23, 24 }; // 4 + 4
                    writeQueue.Enqueue(testData4); // 40 B filled
                    var receivedData4 = readQueue.Dequeue();
                    Assert.Equal(testData4, receivedData4);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());
                }
            }
        }

        [Fact]
        public void MultipleDataTransfersWithTrickyOverflow()
        {
            System.Diagnostics.Debug.WriteLine("MultipleDataTransfersWithTrickyOverflow");

            var initialSize = 30;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    byte[] testData = { 1, 2, 3, 4, 5, 6, 7, 8 }; // 8 + 4
                    writeQueue.Enqueue(testData); // 12 B filled
                    var receivedData = readQueue.Dequeue();
                    Assert.Equal(testData, receivedData);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData2 = { 9, 10, 11, 12, 13, 14, 15, 16 }; // 8 + 4
                    writeQueue.Enqueue(testData2); // 24 B filled
                    var receivedData2 = readQueue.Dequeue();
                    Assert.Equal(testData2, receivedData2);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());

                    byte[] testData3 = { 17, 18, 19, 20, 21, 22 }; // 6 + 4
                    writeQueue.Enqueue(testData3); // 34 B filled
                    var receivedData3 = readQueue.Dequeue();
                    Assert.Equal(testData3, receivedData3);

                    System.Diagnostics.Debug.WriteLine(writeQueue.Diagnostics());
                    System.Diagnostics.Debug.WriteLine(readQueue.Diagnostics());
                }
            }
        }

        [Fact]
        public void OutOfMemory()
        {
            var initialSize = 16;
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    byte[] testData = new byte[5]; // 5 + 4
                    writeQueue.Enqueue(testData);
                    Assert.Throws(typeof(OutOfMemoryException), () => writeQueue.Enqueue(testData));
                }
            }
        }
        
        [Fact]
        public void TooMuchAtOnce()
        {
            var initialSize = 16; // dataSize of 12 will hit the limit
            using (var writeQueue = new MemoryMappedQueue(initialSize, true, AccessName))
            {
                using (var readQueue = new MemoryMappedQueue(initialSize, false, AccessName))
                {
                    byte[] testData = new byte[13]; // 13(dataSize) + 4
                    Assert.Throws(typeof(OutOfMemoryException), () => writeQueue.Enqueue(testData));
                }
            }
        }
    }
}
