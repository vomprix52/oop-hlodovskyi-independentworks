namespace IndependentWork8;

// Сценарій 2: теги статей блогу на основі HashSet
public class TagManager
{
    // Ідентифікатор статті -> множина її тегів
    private Dictionary<string, HashSet<string>> _articleTags;

    // Конструктор
    public TagManager()
    {
        _articleTags = new Dictionary<string, HashSet<string>>();
    }

    // Додає тег; повтор, навіть з іншим регістром, ігнорується. Повертає true, якщо тег новий
    public bool AddTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _articleTags[articleId] = tags;
        }

        return tags.Add(tag.Trim());
    }

    // Видаляє тег зі статті, повертає true, якщо він був
    public bool RemoveTag(string articleId, string tag)
    {
        if (_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            return tags.Remove(tag.Trim());
        }

        return false;
    }

    // Теги однієї статті; повертається копія, щоб зовнішній код не змінив внутрішню множину
    public HashSet<string> GetTags(string articleId)
    {
        if (_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            return new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase);
        }

        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    // Усі унікальні теги з усіх статей (об'єднання множин)
    public HashSet<string> GetAllUniqueTags()
    {
        HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (HashSet<string> tags in _articleTags.Values)
        {
            result.UnionWith(tags);
        }

        return result;
    }

    // Теги, які є в обох статтях (перетин множин)
    public HashSet<string> FindCommonTags(string articleId1, string articleId2)
    {
        HashSet<string> result = GetTags(articleId1);
        result.IntersectWith(GetTags(articleId2));
        return result;
    }

    // Теги, які є в першій статті, але немає в другій (різниця множин)
    public HashSet<string> FindDifferentTags(string articleId1, string articleId2)
    {
        HashSet<string> result = GetTags(articleId1);
        result.ExceptWith(GetTags(articleId2));
        return result;
    }
}
