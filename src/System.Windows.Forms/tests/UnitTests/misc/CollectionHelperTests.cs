using System.Collections;
using Xunit;

namespace System.Windows.Forms.misc.Tests
{
    public class CollectionHelperTests
    {
        [Fact]
        public void HashTableCopyTo_throws_when_target_is_null()
        {
            var source = new Dictionary<string, string>();
            var target = (object[])null;
            var index = 0;

            var ex = Assert.Throws<ArgumentNullException>(() => source.HashTableCopyTo(target, index));
        }

        [Fact]
        public void HashTableCopyTo_throws_when_target_rank_greather_than_one()
        {
            var source = new Dictionary<string, string>();
            var target = new object[3, 3];
            var index = 0;

            var ex = Assert.Throws<ArgumentException>(() => source.HashTableCopyTo(target, index));
        }

        [Fact]
        public void HashTableCopyTo_throws_when_index_less_than_zero()
        {
            var source = new Dictionary<string, string>();
            var target = new object[3];
            var index = -2;

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => source.HashTableCopyTo(target, index));
        }

        [Fact]
        public void HashTableCopyTo_throws_when_index_greather_than_target_length()
        {
            var source = new Dictionary<string, string>();
            var target = new object[3];
            var index = 5;

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => source.HashTableCopyTo(target, index));
        }

        [Fact]
        public void HashTableCopyTo_throws_when_target_lowerbound_is_non_zero()
        {
            var source = new Dictionary<string, string>();
            var target = Array.CreateInstance(typeof(double), new int[] { 3 }, new int[] { 2 });
            var index = 0;

            var ex = Assert.Throws<ArgumentException>(() => source.HashTableCopyTo(target, index));
        }

        [Fact]
        public void HashTableCopyTo_throws_when_target_too_small()
        {
            var source = new Dictionary<string, string>()
            {
                { "key-one", "value-one" },
                { "key-two", "value-two" },
            };
            var target = new object[3];
            var index = 2;

            var ex = Assert.Throws<ArgumentException>(() => source.HashTableCopyTo(target, index));
        }

        [Fact]
        public void HashTableCopyTo_throws_when_target_is_null_object_array()
        {
            var source = new Dictionary<string, string>();
            var target = (object[])null;
            var index = 0;

            var ex = Assert.Throws<ArgumentNullException>(() => source.HashTableCopyTo(target, index));
        }

        [Fact]
        public void HashTableCopyTo_successfully_copies_to_KeyValuePair_array()
        {
            var source = new Dictionary<string, string>()
            {
                { "key-one", "value-one" },
                { "key-two", "value-two" },
            };
            var target = new KeyValuePair<string, string>[2];
            var index = 0;

            source.HashTableCopyTo(target, index);

            var firstTargetItem = target[0];
            Assert.Equal(typeof(KeyValuePair<string, string>), firstTargetItem.GetType());
            Assert.Equal("key-one", firstTargetItem.Key);
            Assert.Equal("value-one", firstTargetItem.Value);

            var secondTargetItem = target[1];
            Assert.Equal(typeof(KeyValuePair<string, string>), secondTargetItem.GetType());
            Assert.Equal("key-two", secondTargetItem.Key);
            Assert.Equal("value-two", secondTargetItem.Value);
        }

        [Fact]
        public void HashTableCopyTo_successfully_copies_to_DictionaryEntry_array()
        {
            var source = new Dictionary<string, string>()
            {
                { "key-one", "value-one" },
                { "key-two", "value-two" },
            };
            var target = new DictionaryEntry[2];
            var index = 0;

            source.HashTableCopyTo(target, index);

            var firstTargetItem = target[0];
            Assert.Equal(typeof(DictionaryEntry), firstTargetItem.GetType());
            Assert.Equal("key-one", (string)firstTargetItem.Key);
            Assert.Equal("value-one", (string)firstTargetItem.Value);

            var secondTargetItem = target[1];
            Assert.Equal(typeof(DictionaryEntry), secondTargetItem.GetType());
            Assert.Equal("key-two", (string)secondTargetItem.Key);
            Assert.Equal("value-two", (string)secondTargetItem.Value);
        }

        [Fact]
        public void HashTableCopyTo_successfully_copies_to_Object_array()
        {
            var source = new Dictionary<string, string>()
            {
                { "key-one", "value-one" },
                { "key-two", "value-two" },
            };
            var target = new DictionaryEntry[2];
            var index = 0;

            source.HashTableCopyTo(target, index);

            var firstTargetItem = target[0];
            Assert.Equal(typeof(DictionaryEntry), firstTargetItem.GetType());
            Assert.Equal("key-one", (string)firstTargetItem.Key);
            Assert.Equal("value-one", (string)firstTargetItem.Value);

            var secondTargetItem = target[1];
            Assert.Equal(typeof(DictionaryEntry), secondTargetItem.GetType());
            Assert.Equal("key-two", (string)secondTargetItem.Key);
            Assert.Equal("value-two", (string)secondTargetItem.Value);
        }
    }
}
