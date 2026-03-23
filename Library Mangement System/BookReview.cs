using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal struct BookReview
{
    public string BookId     { get; }
    public string MemberId   { get; }
    public int    Rating     { get; }   
    public string Comment    { get; }
    public DateTime ReviewedOn { get; }
 
    public BookReview(string bookId, string memberId, int rating, string comment)
    {
        BookId     = bookId;
        MemberId   = memberId;
        Rating     = rating;
        Comment    = comment;
        ReviewedOn = DateTime.UtcNow;
    }
}


}
