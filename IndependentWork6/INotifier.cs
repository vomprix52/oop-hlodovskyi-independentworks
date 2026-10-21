namespace IndependentWork6;

// Спільний контракт для всіх каналів сповіщень
public interface INotifier
{
    // Назва каналу для виводу
    string ChannelName { get; }

    // Відправляє повідомлення, повертає true, якщо відправка вдалася
    bool Send(string message);
}
