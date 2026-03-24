using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal interface IDatabase
{
    // ── Strings: ID generation & config ─────────────────────────────
    string  GenerateId(string entity);                    // INCR counter:{entity}
    void    SetConfig(string key, string value);          // SET config:{key}
    string? GetConfig(string key);                        // GET config:{key}
 
    // ── Hashes: entity storage ───────────────────────────────────────
    void SaveBook(Book book);
    Book?        GetBook(string id);
    List<Book>   GetAllBooks();
    void         DeleteBook(string id);
 
    void            SaveMember(Member member);
    Member?         GetMember(string id);
    List<Member>    GetAllMembers();
 
    // ── Lists: borrow history ────────────────────────────────────────
    void               PushBorrowRecord(BorrowRecord record);   // LPUSH
    List<BorrowRecord> GetBorrowHistory(int limit = 50);        // LRANGE
    List<BorrowRecord> GetMemberBorrowHistory(string memberId);
 
    // ── Lists: waitlist queue ────────────────────────────────────────
    void         EnqueueWaitlist(string bookId, string memberId); // LPUSH
    string?      DequeueWaitlist(string bookId);                  // RPOP
    List<string> GetWaitlist(string bookId);                      // LRANGE
 
    // ── Sets: genre index ────────────────────────────────────────────
    void         AddBookToGenre(string bookId, string genre);     // SADD
    List<string> GetBookIdsByGenre(string genre);                 // SMEMBERS
    List<string> GetGenresForBook(string bookId);                 // SMEMBERS
    List<string> GetBooksInBothGenres(string g1, string g2);      // SINTER
 
    // ── Sets: per-member borrow tracking ────────────────────────────
    void            TrackBorrowedByMember(string memberId, string bookId);   // SADD
    void            UntrackBorrowedByMember(string memberId, string bookId); // SREM
    HashSet<string> GetCurrentlyBorrowedByMember(string memberId);           // SMEMBERS
 
    // ── Sorted Sets: leaderboard ─────────────────────────────────────
    void IncrementBorrowScore(string memberId);                              // ZINCRBY
    List<(string MemberId, double Score)> GetTopReaders(int count = 10);    // ZREVRANGE
    double GetMemberBorrowScore(string memberId);                            // ZSCORE
 
    // ── Sorted Sets: overdue tracking ────────────────────────────────
    void         TrackDueDate(string bookId, string memberId, DateTime due); // ZADD
    void         RemoveDueDate(string bookId);                               // ZREM
    List<string> GetOverdueEntries(DateTime asOf);                           // ZRANGEBYSCORE
 
    // ── JSON: reviews ────────────────────────────────────────────────
    void             SaveReview(BookReview review);         // LPUSH serialised JSON
    List<BookReview> GetReviewsForBook(string bookId);      // LRANGE + deserialise
    double           GetAverageRating(string bookId);
}

}
