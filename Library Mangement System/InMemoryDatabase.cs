using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal class InMemoryDatabase : IDatabase
{
    // ── String equivalents ───────────────────────────────────────────
    private readonly Dictionary<string, int>    _counters = new();
    private readonly Dictionary<string, string> _config   = new();
 
    // ── Hash equivalents ─────────────────────────────────────────────
    private readonly Dictionary<string, Book>   _books   = new();
    private readonly Dictionary<string, Member> _members = new();
 
    // ── List equivalents ─────────────────────────────────────────────
    private readonly List<BorrowRecord>                       _borrowHistory = new();
    private readonly Dictionary<string, Queue<string>>        _waitlists     = new();
    private readonly Dictionary<string, List<BorrowRecord>>   _memberHistory = new();
 
    // ── Set equivalents ──────────────────────────────────────────────
    private readonly Dictionary<string, HashSet<string>> _genreIndex     = new();
    private readonly Dictionary<string, HashSet<string>> _bookGenres     = new();
    private readonly Dictionary<string, HashSet<string>> _memberBorrowed = new();
 
    // ── Sorted Set equivalents ───────────────────────────────────────
    private readonly Dictionary<string, double>  _borrowScores = new();
    private readonly SortedList<double, string>  _dueDates     = new();
 
    // ── JSON equivalent ──────────────────────────────────────────────
    private readonly Dictionary<string, List<BookReview>> _reviews = new();
 
 
    // ── String ops ───────────────────────────────────────────────────
 
    public string GenerateId(string entity)
    {
        if (!_counters.ContainsKey(entity)) _counters[entity] = 0;
        _counters[entity]++;
        return $"{char.ToUpper(entity[0])}{_counters[entity]:D4}"; // B0001
    }
 
    public void    SetConfig(string key, string value) => _config[key] = value;
    public string? GetConfig(string key)
        => _config.TryGetValue(key, out var v) ? v : null;
 
 
    // ── Hash ops ─────────────────────────────────────────────────────
 
    public void       SaveBook(Book book)   => _books[book.ItemId] = book;
    public Book?      GetBook(string id)    => _books.TryGetValue(id, out var b) ? b : null;
    public List<Book> GetAllBooks()         => _books.Values.ToList();
    public void       DeleteBook(string id) => _books.Remove(id);
 
    public void          SaveMember(Member m)   => _members[m.Id] = m;
    public Member?       GetMember(string id)   => _members.TryGetValue(id, out var m) ? m : null;
    public List<Member>  GetAllMembers()        => _members.Values.ToList();
 
 
    // ── List ops (history) ────────────────────────────────────────────
 
    public void PushBorrowRecord(BorrowRecord record)
    {
        _borrowHistory.Insert(0, record);  // mirrors LPUSH — newest at front
 
        if (!_memberHistory.ContainsKey(record.MemberId))
            _memberHistory[record.MemberId] = new();
        _memberHistory[record.MemberId].Insert(0, record);
    }
 
    public List<BorrowRecord> GetBorrowHistory(int limit = 50)
        => _borrowHistory.Take(limit).ToList();
 
    public List<BorrowRecord> GetMemberBorrowHistory(string memberId)
        => _memberHistory.TryGetValue(memberId, out var h) ? h : new();
 
 
    // ── List ops (waitlist queue) ─────────────────────────────────────
 
    public void EnqueueWaitlist(string bookId, string memberId)
    {
        if (!_waitlists.ContainsKey(bookId)) _waitlists[bookId] = new();
        _waitlists[bookId].Enqueue(memberId);
    }
 
    public string? DequeueWaitlist(string bookId)
    {
        if (!_waitlists.TryGetValue(bookId, out var q) || q.Count == 0) return null;
        return q.Dequeue();
    }
 
    public List<string> GetWaitlist(string bookId)
        => _waitlists.TryGetValue(bookId, out var q) ? q.ToList() : new();
 
 
    // ── Set ops (genre index) ─────────────────────────────────────────
 
    public void AddBookToGenre(string bookId, string genre)
    {
        if (!_genreIndex.ContainsKey(genre))   _genreIndex[genre]   = new();
        if (!_bookGenres.ContainsKey(bookId))  _bookGenres[bookId]  = new();
        _genreIndex[genre].Add(bookId);
        _bookGenres[bookId].Add(genre);
    }
 
    public List<string> GetBookIdsByGenre(string genre)
        => _genreIndex.TryGetValue(genre, out var s) ? s.ToList() : new();
 
    public List<string> GetGenresForBook(string bookId)
        => _bookGenres.TryGetValue(bookId, out var s) ? s.ToList() : new();
 
    public List<string> GetBooksInBothGenres(string g1, string g2)
    {
        if (!_genreIndex.TryGetValue(g1, out var s1)) return new();
        if (!_genreIndex.TryGetValue(g2, out var s2)) return new();
        return s1.Intersect(s2).ToList();  // mirrors SINTER
    }
 
 
    // ── Set ops (member tracking) ─────────────────────────────────────
 
    public void TrackBorrowedByMember(string memberId, string bookId)
    {
        if (!_memberBorrowed.ContainsKey(memberId)) _memberBorrowed[memberId] = new();
        _memberBorrowed[memberId].Add(bookId);
    }
 
    public void UntrackBorrowedByMember(string memberId, string bookId)
    {
        if (_memberBorrowed.TryGetValue(memberId, out var s)) s.Remove(bookId);
    }
 
    public HashSet<string> GetCurrentlyBorrowedByMember(string memberId)
        => _memberBorrowed.TryGetValue(memberId, out var s) ? s : new();
 
 
    // ── Sorted Set ops (leaderboard) ──────────────────────────────────
 
    public void IncrementBorrowScore(string memberId)
    {
        if (!_borrowScores.ContainsKey(memberId)) _borrowScores[memberId] = 0;
        _borrowScores[memberId]++;
    }
 
    public List<(string MemberId, double Score)> GetTopReaders(int count = 10)
        => _borrowScores
            .OrderByDescending(kv => kv.Value)  // mirrors ZREVRANGE
            .Take(count)
            .Select(kv => (kv.Key, kv.Value))
            .ToList();
 
    public double GetMemberBorrowScore(string memberId)
        => _borrowScores.TryGetValue(memberId, out var s) ? s : 0;
 
 
    // ── Sorted Set ops (overdue) ──────────────────────────────────────
 
    public void TrackDueDate(string bookId, string memberId, DateTime due)
    {
        double score = new DateTimeOffset(due).ToUnixTimeSeconds();
        _dueDates[score] = $"{bookId}:{memberId}";
    }
 
    public void RemoveDueDate(string bookId)
    {
        var keys = _dueDates.Where(kv => kv.Value.StartsWith(bookId)).ToList();
        foreach (var kv in keys) _dueDates.Remove(kv.Key);
    }
 
    public List<string> GetOverdueEntries(DateTime asOf)
    {
        double cutoff = new DateTimeOffset(asOf).ToUnixTimeSeconds();
        return _dueDates
            .Where(kv => kv.Key <= cutoff)  // mirrors ZRANGEBYSCORE
            .Select(kv => kv.Value)
            .ToList();
    }
 
 
    // ── JSON ops (reviews) ────────────────────────────────────────────
 
    public void SaveReview(BookReview review)
    {
        if (!_reviews.ContainsKey(review.BookId)) _reviews[review.BookId] = new();
        _reviews[review.BookId].Insert(0, review);
    }
 
    public List<BookReview> GetReviewsForBook(string bookId)
        => _reviews.TryGetValue(bookId, out var r) ? r : new();
 
    public double GetAverageRating(string bookId)
    {
        var reviews = GetReviewsForBook(bookId);
        return reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating);
    }
}


}
