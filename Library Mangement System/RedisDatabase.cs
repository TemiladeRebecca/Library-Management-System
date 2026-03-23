using StackExchange.Redis;
using System.Text.Json;
using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal class RedisDatabase : IDatabase
    {
        private readonly StackExchange.Redis.IDatabase _db;
        private readonly IConnectionMultiplexer _redis;

        public RedisDatabase(string connectionString = "localhost:6379")
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
        }

        // string operations
        public string GenerateId(string entity)
        {
            long id = _db.StringIncrement($"counter:{entity}");  // INCR
            return $"{char.ToUpper(entity[0])}{id:D4}";
        }
    
        public void SetConfig(string key, string value)
            => _db.StringSet($"config:{key}", value);             // SET
    
        public string? GetConfig(string key)
        {
            var val = _db.StringGet($"config:{key}");             // GET
            return val.HasValue ? val.ToString() : null;
        }

        // Hash operations
        public void SaveBook(Book book)
        {
            var key = $"book:{book.ItemId}";
            _db.HashSet(key, new HashEntry[]              // HSET
            {
                new("id",          book.ItemId),
                new("title",       book.Title),
                new("author",      book.Author),
                new("isbn",        book.Isbn),
                new("publishYear", book.PublishYear.ToString()),
                new("borrowedBy",  book.BorrowedByMemberId ?? ""),
            });
    
            _db.StringSet($"book:{book.ItemId}:available",
                        book.IsAvailable() ? "true" : "false");
    
            foreach (var genre in book.Genres)
            {
                _db.SetAdd($"book:{book.ItemId}:genres", genre);  // SADD
                _db.SetAdd($"genre:{genre.ToLower()}:books", book.ItemId);
            }
    
            _db.SetAdd("books:all", book.ItemId);
        }
    
        public Book? GetBook(string id)
        {
            var hash = _db.HashGetAll($"book:{id}");               // HGETALL
            if (hash.Length == 0) return null;
    
            var f = hash.ToDictionary(h => h.Name.ToString(), h => h.Value.ToString());
            var genres = _db.SetMembers($"book:{id}:genres")
                            .Select(g => g.ToString()).ToList();
    
            return new Book(
                f["id"], f["title"], f["author"], f["isbn"],
                int.Parse(f["publishYear"]),
                genres,
                string.IsNullOrEmpty(f["borrowedBy"]) ? null : f["borrowedBy"]
            );
        }
    
        public List<Book> GetAllBooks()
        {
            var ids = _db.SetMembers("books:all");                  // SMEMBERS
            return ids.Select(id => GetBook(id.ToString()))
                    .Where(b => b != null).Cast<Book>().ToList();
        }
    
        public void DeleteBook(string id)
        {
            _db.KeyDelete($"book:{id}");                           // DEL
            _db.SetRemove("books:all", id);
        }

        public void SaveMember(Member member)
        {
            var memberKey = $"member:{member.Id}";
            var borrowKey = $"member:{member.Id}:borrowed";

            _db.HashSet(memberKey, new HashEntry[]
            {
                new("id", member.Id),
                new("name", member.Name),
                new("memberShipType", member.MembershipType)
            });
        }
    
        // SaveMember and GetMember follow the same Hash pattern as SaveBook / GetBook.
        // Implement them yourself using HSET / HGETALL.


    // List Operations — History and Waitlist
        public void PushBorrowRecord(BorrowRecord record)
        {
            var json = JsonSerializer.Serialize(record);
            _db.ListLeftPush("borrow:history", json);              // LPUSH
            _db.ListLeftPush($"member:{record.MemberId}:history", json);
        }
    
        public List<BorrowRecord> GetBorrowHistory(int limit = 50)
{
    return _db.ListRange("borrow:history", 0, limit - 1)
              .Select(v => JsonSerializer.Deserialize<BorrowRecord>(v.ToString()))
              .Where(r => r != null)          // filter nulls
              .ToList()!;
}
    
        public void EnqueueWaitlist(string bookId, string memberId)
            => _db.ListLeftPush($"book:{bookId}:waitlist", memberId); // LPUSH
    
        public string? DequeueWaitlist(string bookId)
        {
            var val = _db.ListRightPop($"book:{bookId}:waitlist");  // RPOP — FIFO
            return val.HasValue ? val.ToString() : null;
        }
    
        public List<string> GetWaitlist(string bookId)
            => _db.ListRange($"book:{bookId}:waitlist", 0, -1)     // LRANGE
                .Select(v => v.ToString()).ToList();




    }
    
}
