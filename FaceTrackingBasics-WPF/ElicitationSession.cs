using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Impeli
{
    class ElicitationSession
    {
        public String sessionID { set; get; }
        public List<UserProfile> sessionUsers { set; get; }
        public Content content { set; get; }
        public DateTime startTime { set; get; }
        public DateTime endTime { set; get; }

        public ElicitationSession(Content show) 
        {
            this.content = show;
            sessionID = DateTime.Now.ToString("ddMMyyyyhhmmss");
            sessionUsers = new List<UserProfile>();
        }
        public ElicitationSession()
        {
            sessionID = DateTime.Now.ToString("ddMMyyyyhhmmss");
            sessionUsers = new List<UserProfile>();
        }

        public ElicitationSession(List<UserProfile> userProfiles, int contentCounter)
        {
            sessionID = DateTime.Now.ToString("ddMMyyyyhhmmss");
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
        public void setUsers(List<UserProfile> userList)
        {
            sessionUsers = userList;
        }


    }
}
