using IndependentWork7;

namespace IndependentWork7.Tests;

// Тести конструктора
public class CacheConstructorTests
{
    [Fact]
    public void Constructor_ValidCapacity_CreatesEmptyCache()
    {
        Cache<string, int> cache = new Cache<string, int>(3);

        Assert.Equal(3, cache.Capacity);
        Assert.Equal(0, cache.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_NotPositiveCapacity_ThrowsArgumentOutOfRangeException(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<string, int>(capacity));
    }
}

// Тести методу Add
public class CacheAddTests
{
    [Fact]
    public void Add_NewItem_ItemIsStored()
    {
        Cache<string, string> cache = new Cache<string, string>(3);

        cache.Add("Київ", "+12 °C");

        Assert.True(cache.ContainsKey("Київ"));
        Assert.Equal("+12 °C", cache.Get("Київ"));
    }

    [Fact]
    public void Add_SeveralItems_AllItemsAreStored()
    {
        Cache<int, string> cache = new Cache<int, string>(3);

        cache.Add(1, "один");
        cache.Add(2, "два");
        cache.Add(3, "три");

        Assert.Equal("один", cache.Get(1));
        Assert.Equal("два", cache.Get(2));
        Assert.Equal("три", cache.Get(3));
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueWithoutAddingNewItem()
    {
        Cache<string, string> cache = new Cache<string, string>(3);
        cache.Add("Київ", "+12 °C");

        cache.Add("Київ", "+15 °C");

        Assert.Equal("+15 °C", cache.Get("Київ"));
        Assert.Equal(1, cache.Count);
    }
}

// Тести методу Get
public class CacheGetTests
{
    [Fact]
    public void Get_ExistingKey_ReturnsValue()
    {
        Cache<string, int> cache = new Cache<string, int>(2);
        cache.Add("яблука", 25);

        int value = cache.Get("яблука");

        Assert.Equal(25, value);
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        Cache<string, int> cache = new Cache<string, int>(2);
        cache.Add("яблука", 25);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("груші"));
    }

    [Fact]
    public void Get_EmptyCache_ThrowsKeyNotFoundException()
    {
        Cache<int, string> cache = new Cache<int, string>(2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
    }

    [Fact]
    public void Get_EvictedKey_ThrowsKeyNotFoundException()
    {
        Cache<int, string> cache = new Cache<int, string>(1);
        cache.Add(1, "один");
        cache.Add(2, "два");

        Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
    }
}

// Тести політики FIFO при перевищенні ємності
public class CacheEvictionTests
{
    [Fact]
    public void Add_WhenFull_RemovesOldestItem()
    {
        Cache<string, string> cache = new Cache<string, string>(3);
        cache.Add("A", "1");
        cache.Add("B", "2");
        cache.Add("C", "3");

        cache.Add("D", "4");

        Assert.False(cache.ContainsKey("A"));
    }

    [Fact]
    public void Add_WhenFull_KeepsNewerItemsAndAddsNewOne()
    {
        Cache<string, string> cache = new Cache<string, string>(3);
        cache.Add("A", "1");
        cache.Add("B", "2");
        cache.Add("C", "3");

        cache.Add("D", "4");

        Assert.Equal("2", cache.Get("B"));
        Assert.Equal("3", cache.Get("C"));
        Assert.Equal("4", cache.Get("D"));
    }

    [Fact]
    public void Add_SeveralItemsOverCapacity_RemovesItemsInInsertionOrder()
    {
        Cache<int, string> cache = new Cache<int, string>(2);
        cache.Add(1, "один");
        cache.Add(2, "два");

        cache.Add(3, "три");
        Assert.False(cache.ContainsKey(1));
        Assert.True(cache.ContainsKey(2));

        cache.Add(4, "чотири");
        Assert.False(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
        Assert.True(cache.ContainsKey(4));
    }

    [Fact]
    public void Add_CapacityOne_KeepsOnlyLastItem()
    {
        Cache<int, string> cache = new Cache<int, string>(1);

        cache.Add(1, "один");
        cache.Add(2, "два");
        cache.Add(3, "три");

        Assert.Equal(1, cache.Count);
        Assert.Equal("три", cache.Get(3));
    }

    [Fact]
    public void Get_DoesNotChangeEvictionOrder()
    {
        // FIFO враховує лише порядок додавання, читання не продовжує життя елемента
        Cache<string, string> cache = new Cache<string, string>(2);
        cache.Add("A", "1");
        cache.Add("B", "2");
        cache.Get("A");

        cache.Add("C", "3");

        Assert.False(cache.ContainsKey("A"));
        Assert.True(cache.ContainsKey("B"));
    }

    [Fact]
    public void Add_UpdatingExistingKey_DoesNotChangeEvictionOrder()
    {
        Cache<string, string> cache = new Cache<string, string>(2);
        cache.Add("A", "1");
        cache.Add("B", "2");
        cache.Add("A", "10");

        cache.Add("C", "3");

        Assert.False(cache.ContainsKey("A"));
        Assert.True(cache.ContainsKey("B"));
        Assert.True(cache.ContainsKey("C"));
    }
}

// Тести методу ContainsKey
public class CacheContainsKeyTests
{
    [Fact]
    public void ContainsKey_AddedKey_ReturnsTrue()
    {
        Cache<int, string> cache = new Cache<int, string>(2);
        cache.Add(7, "сім");

        Assert.True(cache.ContainsKey(7));
    }

    [Fact]
    public void ContainsKey_MissingKey_ReturnsFalse()
    {
        Cache<int, string> cache = new Cache<int, string>(2);
        cache.Add(7, "сім");

        Assert.False(cache.ContainsKey(8));
    }

    [Fact]
    public void ContainsKey_EvictedKey_ReturnsFalse()
    {
        Cache<int, string> cache = new Cache<int, string>(1);
        cache.Add(7, "сім");
        cache.Add(8, "вісім");

        Assert.False(cache.ContainsKey(7));
        Assert.True(cache.ContainsKey(8));
    }
}

// Тести властивості Count
public class CacheCountTests
{
    [Fact]
    public void Count_NewCache_IsZero()
    {
        Cache<string, string> cache = new Cache<string, string>(5);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Count_AfterAddingItems_EqualsNumberOfItems()
    {
        Cache<string, string> cache = new Cache<string, string>(5);

        cache.Add("A", "1");
        cache.Add("B", "2");

        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Count_AfterOverflow_EqualsCapacity()
    {
        Cache<int, int> cache = new Cache<int, int>(3);

        for (int i = 1; i <= 10; i++)
        {
            cache.Add(i, i * i);
        }

        Assert.Equal(3, cache.Count);
    }
}
