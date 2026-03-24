using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Library_Management_System
{
    internal interface IMemberRepository
    {
        void Add(Member member);
        Member? FindById(string id);
        List<Member> GetAll();

    }

    internal class MemberRepository : IMemberRepository 
    {
        private readonly IDatabase _db;

        public MemberRepository(IDatabase db)
        {
            _db = db; 
        }

        public void Add(Member member)
        {
            _db.SaveMember(member);
            
        }

        public Member? FindById(string id)
        {
           return  _db.GetMember(id);
        }

        public List<Member> GetAll()
        {
            return _db.GetAllMembers();
        }
       
    }
}
