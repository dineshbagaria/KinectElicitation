using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceTrackingBasics
{
    class ElicitationSession
    {
        public String sessionID { set; get; }
        public List<UserProfile> sessionUsers { set; get; }
        public Content content { set; get; }

        public ElicitationSession(Content show) 
        {
            this.content = show;
            sessionID = DateTime.Today.ToLongDateString();
            sessionUsers = new List<UserProfile>();
        }
        public ElicitationSession()
        {
            sessionID = DateTime.Today.ToLongDateString();
            sessionUsers = new List<UserProfile>();
        }


        public void AddUser(UserProfile user)
        {
            sessionUsers.Add(user);
        }
        public void AddUser(List<UserProfile> userList)
        {
            sessionUsers.AddRange(userList);
        }


    }
}
