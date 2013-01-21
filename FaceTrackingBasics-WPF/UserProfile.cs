using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceTrackingBasics
{

    public class UserProfile
    {
        public String gender { set; get; }

        public Boolean userIsChild { set; get; }

        public Boolean userSleeping { set; get; }

        public Boolean userDistracted { set; get; }
        public long userID { set; get; }

        public UserProfile(long userID, String gender, Boolean userIsChild, Boolean userSleeping, Boolean userDistracted)
        {
            this.userID = userID;
            this.gender = gender;
            this.userIsChild = userIsChild;
            this.userSleeping = userSleeping;
            this.userDistracted = userDistracted;

        }
        public UserProfile()
        { }
    }
}
