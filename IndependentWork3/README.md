# Звіт з аналізу інкапсуляції в Open-Source проєкті

Самостійна робота №3. Тема: аналіз інкапсуляції в open-source проєктах, практики валідації полів.

## 1. Обраний проєкт

- **Назва:** .NET Runtime (dotnet/runtime)
- **Посилання на GitHub:** https://github.com/dotnet/runtime

Я обрав цей проєкт, бо це стандартна бібліотека .NET, і її класами ми постійно користуємося в лабораторних. Було цікаво подивитися, як вони влаштовані всередині.

Для аналізу я взяв три класи:

- List\<T\>: [List.cs](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs)
- Version: [Version.cs](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Version.cs)
- HttpClient: [HttpClient.cs](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Net.Http/src/System/Net/Http/HttpClient.cs)

Посилання ведуть на конкретну версію коду, щоб номери рядків не змінилися.

## 2. Аналіз інкапсуляції

### Клас: `List<T>`

**Опис класу:** це звичайний список, який сам збільшується, коли додаєш елементи. Всередині він зберігає масив і кількість елементів.

**Поля** ([рядки 23–30](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs#L23-L30)):

```csharp
private const int DefaultCapacity = 4;

internal T[] _items;
internal int _size;
internal int _version;

private static readonly T[] s_emptyArray = new T[0];
```

Публічних полів немає. Поля `_items`, `_size` і `_version` мають модифікатор `internal`, а не `private`. Це означає, що їх бачить тільки код всередині самої бібліотеки, а з нашої програми до них не дістатися. Тобто для нас вони все одно закриті.

**Властивості** ([Capacity](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs#L97-L124), [Count](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs#L127)):

```csharp
public int Capacity
{
    get => _items.Length;
    set
    {
        if (value < _size)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
        }
        // ... далі створюється новий масив і туди копіюються елементи
    }
}

public int Count => _size;
```

`Capacity` можна і читати, і змінювати. Але в `set` не просто присвоюється значення, а перевіряється, чи воно не менше за кількість елементів, і потім створюється новий масив. `Count` тільки для читання, бо кількість елементів змінюється лише через `Add`, `Remove` і подібні методи. Автоматичних властивостей і властивостей тільки для запису тут немає.

**Індексатори/Оператори** ([рядки 143–164](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs#L143-L164)):

```csharp
public T this[int index]
{
    get
    {
        if ((uint)index >= (uint)_size)
        {
            ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
        }
        return _items[index];
    }

    set
    {
        if ((uint)index >= (uint)_size)
        {
            ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
        }
        _items[index] = value;
        _version++;
    }
}
```

Індексатор дозволяє писати `list[0]`, як з масивом. Він перевіряє, щоб індекс не виходив за межі списку. Масив всередині зазвичай більший, ніж кількість елементів, тому без цієї перевірки можна було б прочитати порожні комірки. Перевантажених операторів у класі немає.

### Клас: `Version`

**Опис класу:** зберігає номер версії, наприклад `8.0.1.0`.

**Поля** ([рядки 26–29](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Version.cs#L26-L29)):

```csharp
private readonly int _Major;
private readonly int _Minor;
private readonly int _Build;
private readonly int _Revision;
```

Усі поля приватні і `readonly`, тобто їх можна задати тільки в конструкторі. Після створення об'єкт змінити не можна.

**Властивості** ([рядки 100–110](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Version.cs#L100-L110)):

```csharp
public int Major => _Major;
public int Minor => _Minor;
public int Build => _Build;
public int Revision => _Revision;

public short MajorRevision => (short)(_Revision >> 16);
public short MinorRevision => (short)(_Revision & 0xFFFF);
```

Усі властивості тільки для читання. Перші чотири просто повертають поля, а останні дві обчислюються з поля `_Revision`. Автоматичних властивостей немає.

Перевірка даних тут зроблена в конструкторі ([рядки 31–42](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Version.cs#L31-L42)), бо `set` немає:

```csharp
public Version(int major, int minor, int build, int revision)
{
    ArgumentOutOfRangeException.ThrowIfNegative(major);
    ArgumentOutOfRangeException.ThrowIfNegative(minor);
    ArgumentOutOfRangeException.ThrowIfNegative(build);
    ArgumentOutOfRangeException.ThrowIfNegative(revision);

    _Major = major;
    _Minor = minor;
    _Build = build;
    _Revision = revision;
}
```

Тому створити версію з від'ємним числом не вийде.

**Індексатори/Оператори** ([рядки 466–503](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Private.CoreLib/src/System/Version.cs#L466-L503)):

```csharp
public static bool operator ==(Version? v1, Version? v2)
{
    if (v2 is null)
    {
        return v1 is null;
    }

    return ReferenceEquals(v2, v1) || v2.Equals(v1);
}

public static bool operator !=(Version? v1, Version? v2) => !(v1 == v2);

public static bool operator >(Version? v1, Version? v2) => v2 < v1;
```

У класі перевантажені оператори `==`, `!=`, `<`, `<=`, `>`, `>=`. Завдяки цьому версії можна порівнювати просто, наприклад `if (v1 < v2)`. Без перевантаження `==` порівнював би посилання, і дві однакові версії вважалися б різними. Індексаторів немає.

### Клас: `HttpClient`

**Опис класу:** використовується для відправки HTTP-запитів на сервер і отримання відповідей.

**Поля** ([рядки 18–34](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Net.Http/src/System/Net/Http/HttpClient.cs#L18-L34)):

```csharp
private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromSeconds(100);
private static readonly TimeSpan s_maxTimeout = TimeSpan.FromMilliseconds(int.MaxValue);

private volatile bool _operationStarted;
private volatile bool _disposed;

private Uri? _baseAddress;
private TimeSpan _timeout;
private int _maxResponseContentBufferSize;
```

Усі поля приватні. Для налаштувань (`_baseAddress`, `_timeout`) є публічні властивості з перевіркою. А для службових полів (`_operationStarted`, `_disposed`) властивостей взагалі немає, бо змінювати їх ззовні не можна.

**Властивості** ([рядки 49–50](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Net.Http/src/System/Net/Http/HttpClient.cs#L49-L50)):

```csharp
public HttpRequestHeaders DefaultRequestHeaders =>
    _defaultRequestHeaders ??= new HttpRequestHeaders();
```

`DefaultRequestHeaders` тільки для читання, і об'єкт створюється лише тоді, коли до нього вперше звертаються. Інші властивості (`BaseAddress`, `Timeout`, `MaxResponseContentBufferSize`) мають і `get`, і `set` з перевіркою. Їх я розібрав у наступному розділі. Автоматичних властивостей, індексаторів і операторів у цьому класі немає.

## 3. Практики валідації у властивостях

**Приклад 1:** властивість `Timeout` у `HttpClient` ([рядки 105–118](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Net.Http/src/System/Net/Http/HttpClient.cs#L105-L118)). Час очікування повинен бути більше нуля і не більше максимуму.

```csharp
public TimeSpan Timeout
{
    get => _timeout;
    set
    {
        if (value != s_infiniteTimeout)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, s_maxTimeout);
        }
        CheckDisposedOrStarted();
        _timeout = value;
    }
}
```

Тут використані готові методи `ThrowIfLessThanOrEqual` і `ThrowIfGreaterThan`. Вони роблять те саме, що `if` з `throw`, тільки коротше.

**Приклад 2:** властивість `BaseAddress` у `HttpClient` ([рядки 86–103](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Net.Http/src/System/Net/Http/HttpClient.cs#L86-L103)). Адреса може бути порожньою, але якщо вона є, то має бути повною (з `https://...`).

```csharp
public Uri? BaseAddress
{
    get => _baseAddress;
    set
    {
        if (value is not null && !value.IsAbsoluteUri)
        {
            throw new ArgumentException(SR.net_http_client_absolute_baseaddress_required, nameof(value));
        }

        CheckDisposedOrStarted();
        _baseAddress = value;
    }
}
```

Це звичайна перевірка через `if` і виняток `ArgumentException`.

**Приклад 3:** метод `CheckDisposedOrStarted` ([рядки 745–752](https://github.com/dotnet/runtime/blob/699100ec729dddc77dcafa770b3b9e177357ec41/src/libraries/System.Net.Http/src/System/Net/Http/HttpClient.cs#L745-L752)). Він викликається в `set` майже всіх властивостей `HttpClient`.

```csharp
private void CheckDisposedOrStarted()
{
    ObjectDisposedException.ThrowIf(_disposed, this);
    if (_operationStarted)
    {
        throw new InvalidOperationException(SR.net_http_operation_started);
    }
}
```

Тут перевіряється не саме значення, а стан об'єкта. Якщо клієнт уже відправив запит, налаштування змінювати не можна. Перевірку винесли в окремий метод, щоб не писати її в кожній властивості.

Атрибутів валідації з `System.ComponentModel.DataAnnotations` (наприклад `[Required]` або `[Range]`) у цих класах немає. Я думаю, це тому, що атрибути самі нічого не перевіряють, їх перевіряє інший код, наприклад ASP.NET при обробці форм. А тут класи повинні перевіряти дані одразу при присвоєнні.

**Висновки щодо валідації:** мені сподобалося, що помилка виникає одразу, коли передали неправильне значення, а не десь пізніше. Також по типу винятку зрозуміло, в чому проблема. Мінус у тому, що з самої властивості не видно, які значення допустимі, треба дивитися в код. І винятки працюють повільно, тому для перевірки того, що вводить користувач, краще підходять методи типу `TryParse`.

## 4. Загальні висновки

У всіх трьох класах інкапсуляція дотримана: публічних полів немає, і змінити дані можна тільки через властивості та методи з перевірками.

Я помітив, що кожен клас захищений по-своєму. `Version` взагалі не можна змінити після створення, тому перевірка є тільки в конструкторі. `List<T>` змінюється постійно, тому перевірки стоять у `Capacity` та в індексаторі. А `HttpClient` перевіряє ще й те, чи можна зараз змінювати налаштування.

Ще я зрозумів, що інкапсуляція дозволяє змінювати внутрішню реалізацію і не ламати програми, які користуються класом. Наприклад, `Count` у `List<T>` просто повертає поле `_size`, але ззовні цього не видно.

Для своїх проєктів я візьму такі ідеї: перевіряти значення в `set` і в конструкторі, робити властивості тільки для читання, якщо значення не повинно змінюватися, і виносити однакові перевірки в окремий метод.
