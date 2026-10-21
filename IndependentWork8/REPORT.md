# Звіт до самостійної роботи №8

Тема: вивчення `Dictionary<TKey, TValue>` та `HashSet<T>` у бібліотеках .NET.

## 1. Результати аналізу open-source проєктів

Я дослідив дві популярні бібліотеки:

- **Dapper**, micro-ORM для роботи з базами даних: https://github.com/DapperLib/Dapper
- **Serilog**, бібліотека для логування: https://github.com/serilog/serilog

Посилання ведуть на конкретні версії коду, щоб номери рядків не змінилися.

### Приклад 1. Dictionary: таблиця типів у Dapper

Файл: [Dapper/SqlMapper.cs](https://github.com/DapperLib/Dapper/blob/eb47546a408bbf6c178bda4f04dc205bc51ffbfe/Dapper/SqlMapper.cs), поле в [рядку 171](https://github.com/DapperLib/Dapper/blob/eb47546a408bbf6c178bda4f04dc205bc51ffbfe/Dapper/SqlMapper.cs#L171), заповнення в [рядках 202–257](https://github.com/DapperLib/Dapper/blob/eb47546a408bbf6c178bda4f04dc205bc51ffbfe/Dapper/SqlMapper.cs#L202-L257).

```csharp
private static Dictionary<Type, TypeMapEntry> typeMap;

static SqlMapper()
{
    typeMap = new Dictionary<Type, TypeMapEntry>(41 /* ... */)
    {
        [typeof(byte)] = DbType.Byte,
        [typeof(sbyte)] = DbType.SByte,
        [typeof(short)] = DbType.Int16,
        [typeof(int)] = DbType.Int32,
        // ... ще кілька десятків типів
    };
}
```

Використання в методі `LookupDbType` ([рядки 463–483](https://github.com/DapperLib/Dapper/blob/eb47546a408bbf6c178bda4f04dc205bc51ffbfe/Dapper/SqlMapper.cs#L463-L483)):

```csharp
if (typeMap.TryGetValue(type, out var mapEntry))
{
    if ((mapEntry.Flags & TypeMapEntryFlags.SetType) == 0)
    {
        return null;
    }
    return mapEntry.DbType;
}
```

**Контекст.** Коли ми передаємо в SQL-запит параметр, наприклад число типу `int`, Dapper має сказати базі даних, якого він типу (`DbType.Int32`). Словник зіставляє тип C# з типом бази даних.

**Чому саме Dictionary.** Тут потрібен саме пошук «за ключем знайти значення», тобто за типом знайти `DbType`. Цей пошук виконується для кожного параметра кожного запиту, тому має бути швидким. Якби це був `List` пар, довелося б перебирати близько 45 записів щоразу. Ще мені сподобалося, що в конструктор передано 41, тобто наперед задано місткість. Тоді словник не буде збільшуватися під час заповнення.

**Складність.** `TryGetValue` працює за O(1) в середньому. Заповнення займає O(n) один раз при запуску.

**Особливості.** Ключем є `Type`, а в нього `Equals` і `GetHashCode` вже правильно реалізовані, тому окремий comparer не потрібен. Цікаво, як Dapper змінює словник у методі `AddTypeMap` ([рядки 293–310](https://github.com/DapperLib/Dapper/blob/eb47546a408bbf6c178bda4f04dc205bc51ffbfe/Dapper/SqlMapper.cs#L293-L310)):

```csharp
SetTypeMap(new Dictionary<Type, TypeMapEntry>(snapshot) { [type] = value });
```

Він не додає елемент у старий словник, а створює копію з новим елементом і замінює весь словник. Я зрозумів, що це зроблено тому, що `Dictionary` не безпечний для кількох потоків. Якщо один потік читає, а інший у цей час додає, можна отримати помилку. А заміна посилання на новий словник безпечна, бо старий ніхто не змінює. Додавання тут рідкісне, а читання постійне, тому копіювання себе виправдовує.

### Приклад 2. HashSet: вбудовані скалярні типи в Serilog

Файл: [src/Serilog/Capturing/PropertyValueConverter.cs](https://github.com/serilog/serilog/blob/81af75f3a5e28ec9f5be18ef74357a9914a51745/src/Serilog/Capturing/PropertyValueConverter.cs), [рядки 23–32](https://github.com/serilog/serilog/blob/81af75f3a5e28ec9f5be18ef74357a9914a51745/src/Serilog/Capturing/PropertyValueConverter.cs#L23-L32):

```csharp
static readonly HashSet<Type> BuiltInScalarTypes = new()
{
    typeof(decimal),
    typeof(string),
    typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan),
    typeof(Guid), typeof(Uri),
    // ...
};
```

Використання ([рядки 428–433](https://github.com/serilog/serilog/blob/81af75f3a5e28ec9f5be18ef74357a9914a51745/src/Serilog/Capturing/PropertyValueConverter.cs#L428-L433)):

```csharp
static bool IsValidDictionaryKeyType(Type valueType)
{
    return valueType.IsPrimitive ||
           BuiltInScalarTypes.Contains(valueType) ||
           valueType.IsEnum;
}
```

**Контекст.** Коли в лог записується об'єкт, Serilog вирішує, як його зберегти: як просте значення (число, рядок, дата) чи розібрати на властивості. Множина містить типи, які треба вважати простими значеннями.

**Чому саме HashSet.** Тут не потрібне значення для ключа, а лише відповідь на питання «чи є цей тип у списку». Саме для цього і є `HashSet`. `Dictionary` тут зайвий, бо значення нікуди було б класти. `List` чи масив працювали б, але `Contains` у них перебирає всі елементи. А логування викликається дуже часто, тому кожна перевірка має бути швидкою.

**Складність.** `Contains` працює за O(1) в середньому, а в `List` було б O(n).

**Особливості.** Множина `static readonly`: створюється один раз на всю програму, і посилання на неї замінити не можна. У тому самому файлі є ще один `HashSet<string>` для назв уже оброблених властивостей ([рядок 435](https://github.com/serilog/serilog/blob/81af75f3a5e28ec9f5be18ef74357a9914a51745/src/Serilog/Capturing/PropertyValueConverter.cs#L435)), щоб не записати властивість з однаковою назвою двічі. Він позначений `[ThreadStatic]`, тобто в кожного потоку своя множина, і проблеми з потоками немає.

### Приклад 3. Dictionary і HashSet разом: позиційні параметри в Dapper

Файл: [Dapper/SqlMapper.cs](https://github.com/DapperLib/Dapper/blob/eb47546a408bbf6c178bda4f04dc205bc51ffbfe/Dapper/SqlMapper.cs), метод `PassByPosition`, [рядки 1907–1951](https://github.com/DapperLib/Dapper/blob/eb47546a408bbf6c178bda4f04dc205bc51ffbfe/Dapper/SqlMapper.cs#L1907-L1951):

```csharp
Dictionary<string, IDbDataParameter> parameters = new(StringComparer.Ordinal);

foreach (IDbDataParameter param in cmd.Parameters)
{
    if (!string.IsNullOrEmpty(param.ParameterName)) parameters[param.ParameterName] = param;
}
var consumed = new HashSet<string>(StringComparer.Ordinal);

cmd.CommandText = CompiledRegex.PseudoPositional.Replace(cmd.CommandText, match =>
{
    string key = match.Groups[1].Value;
    if (!consumed.Add(key))
    {
        throw new InvalidOperationException("When passing parameters by position, each parameter can only be referenced once");
    }
    else if (parameters.TryGetValue(key, out IDbDataParameter? param))
    {
        // ... параметр додається в команду в потрібному порядку
        parameters.Remove(key);
        return "?";
    }
    // ...
});
```

**Контекст.** Деякі бази даних не розуміють іменованих параметрів (`@name`) і вимагають позиційні (`?`). Dapper переписує текст запиту: кожне ім'я параметра замінює на `?` і переставляє параметри в тому порядку, в якому вони трапляються в SQL.

**Чому саме ці колекції.**
- `Dictionary<string, IDbDataParameter>` потрібен, щоб за іменем з тексту запиту швидко знайти об'єкт параметра.
- `HashSet<string> consumed` запам'ятовує, які імена вже використані. Цікаво, що `Add` тут одночасно додає і перевіряє: він повертає `false`, якщо ім'я вже було. Тобто один виклик замінює `Contains` + `Add`. Якщо параметр трапився двічі, викидається виняток, бо в позиційному режимі так не можна.

**Складність.** Для кожного знайденого в запиті імені виконується `Add` і `TryGetValue`, обидва O(1). Отже весь метод працює за O(k), де k це кількість параметрів у запиті. Зі списками було б O(k²).

**Особливості.** Обидві колекції створені з `StringComparer.Ordinal`, тобто порівняння рядків точне, з урахуванням регістру і без правил мови. Так швидше, і `@Id` та `@id` вважаються різними параметрами.

## 2. Порівняльна таблиця

| Операція | List\<T\> | Dictionary\<TKey,TValue\> | HashSet\<T\> | Коли використовувати |
|----------|-----------|---------------------------|--------------|----------------------|
| Додавання | O(1)* | O(1)* | O(1)* | List: коли важливий порядок і можуть бути повтори. Dictionary і HashSet: коли треба швидкий пошук |
| Пошук за значенням | O(n) | N/A** | O(1) | HashSet: коли треба лише знати, чи є елемент |
| Пошук за ключем | N/A*** | O(1) | N/A | Dictionary: коли за ключем треба отримати пов'язані дані |
| Видалення | O(n) | O(1) | O(1) | Dictionary або HashSet: коли елементи часто видаляються |
| Перевірка наявності | O(n) | O(1) | O(1) | Dictionary (`ContainsKey`) або HashSet (`Contains`), якщо перевірок багато |

\* В середньому. Якщо внутрішній масив заповнений, колекція створює більший і копіює туди всі елементи, і тоді це O(n). Тому, коли кількість відома наперед, краще задати місткість у конструкторі, як у Dapper.

\** У `Dictionary` є `ContainsValue`, але він перебирає всі значення, тобто працює за O(n).

\*** У `List` є доступ за індексом `list[i]` за O(1), але індекс не є ключем, бо це лише номер позиції.

O(1) для `Dictionary` і `HashSet` означає середній випадок. Якщо хеш-функція погана і багато ключів отримують однаковий хеш, пошук сповільнюється до O(n).

## 3. Висновки про вибір колекції

**Коли Dictionary краще за List.** Коли треба за ключем знайти пов'язані дані: за назвою файлу знайти його метадані, за типом знайти тип БД. У `List` довелося б щоразу перебирати всі елементи. На 10 елементах різниці не видно, а на тисячах і при частих запитах вона величезна.

**Коли HashSet оптимальний.** Коли потрібні лише унікальні значення без пов'язаних даних: теги, вже оброблені імена, дозволені типи. А також коли потрібні операції над множинами: об'єднання (`UnionWith`), перетин (`IntersectWith`), різниця (`ExceptWith`). Зі списками їх довелося б писати вручну вкладеними циклами.

**Підводні камені, які я помітив:**

1. **Порядок не гарантований.** `Dictionary` і `HashSet` не зберігають елементи в якомусь певному порядку. У своїй програмі я сортую результати перед виводом.
2. **Індексатор кидає виняток.** `dictionary[key]` для відсутнього ключа викидає `KeyNotFoundException`. Якщо ключа може не бути, краще `TryGetValue`, як у всіх трьох прикладах з open-source.
3. **Comparer впливає на результат.** Без comparer рядки `"C#"` і `"c#"` різні. Якщо регістр не важливий, треба одразу передати `StringComparer.OrdinalIgnoreCase`, інакше з'являться дублікати. Для своїх класів-ключів треба перевизначати і `Equals`, і `GetHashCode`, інакше два однакові за змістом об'єкти вважатимуться різними.
4. **Не можна змінювати ключ після додавання.** Якщо об'єкт уже лежить у `HashSet` і змінити поле, від якого залежить хеш, колекція його більше не знайде.
5. **Не безпечні для кількох потоків.** Dapper вирішує це копіюванням словника, а Serilog окремою множиною для кожного потоку (`[ThreadStatic]`). Ще є `ConcurrentDictionary`.
6. **Займають більше пам'яті, ніж List**, бо зберігають хеші і внутрішню таблицю. Для кількох елементів, які просто перебираються, `List` простіший і не гірший.

## 4. Приклади з власного коду

Повний вивід програми є в [README.md](README.md).

### Сценарій 1: FileMetadataCache (Dictionary)

```csharp
private Dictionary<string, FileMetadata> _cache;

public FileMetadataCache()
{
    _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);
}

public FileMetadata? Get(string fileName)
{
    if (_cache.TryGetValue(fileName, out FileMetadata? metadata))
    {
        return metadata;
    }

    return null;
}
```

Ключ це назва файлу. Я використав `StringComparer.OrdinalIgnoreCase`, бо в Windows `report.docx` і `REPORT.DOCX` це один і той самий файл. У виводі видно, що `Get("REPORT.DOCX")` знаходить `report.docx`. `Get` повертає `null` замість винятку, бо відсутність файлу в кеші є нормальною ситуацією.

### Сценарій 2: TagManager (HashSet)

```csharp
private Dictionary<string, HashSet<string>> _articleTags;

public HashSet<string> FindCommonTags(string articleId1, string articleId2)
{
    HashSet<string> result = GetTags(articleId1);
    result.IntersectWith(GetTags(articleId2));
    return result;
}
```

Теги кожної статті зберігаються в `HashSet` з `OrdinalIgnoreCase`, тому `"c#"` не додається вдруге до статті, де вже є `"C#"`. Методи `GetAllUniqueTags`, `FindCommonTags` і `FindDifferentTags` використовують `UnionWith`, `IntersectWith` і `ExceptWith`. `GetTags` повертає копію множини, щоб зовнішній код не міг змінити внутрішні дані.

Comparer важливий і при об'єднанні. В article-1 тег записаний як `ООП`, а в article-3 як `ооп`. У `GetAllUniqueTags` я теж створюю множину з `OrdinalIgnoreCase`, тому в загальному списку тег з'являється лише один раз. Якби я забув передати comparer у цю множину, там були б обидва варіанти.

### Сценарій 3: SearchIndex (Dictionary + HashSet)

```csharp
private Dictionary<string, HashSet<int>> _invertedIndex;

public void AddDocument(int docId, string content)
{
    foreach (string word in SplitIntoWords(content))
    {
        if (!_invertedIndex.TryGetValue(word, out HashSet<int>? documents))
        {
            documents = new HashSet<int>();
            _invertedIndex[word] = documents;
        }

        documents.Add(docId);
    }
}

public HashSet<int> SearchMultiple(params string[] words)
{
    // ...
    HashSet<int> result = Search(words[0]);
    for (int i = 1; i < words.Length; i++)
    {
        result.IntersectWith(Search(words[i]));
    }

    return result;
}
```

Це інвертований індекс, як у пошукових системах: для кожного слова зберігається множина номерів документів. `Dictionary` дає швидкий доступ до слова, а `HashSet` гарантує, що номер документа не повториться, навіть якщо слово трапляється в документі кілька разів. Пошук за кількома словами є перетином множин (`IntersectWith`). Щоб знайти документи зі словом, не треба перебирати всі документи, достатньо одного звернення до словника.
