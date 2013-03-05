using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceTrackingBasics
{

    public class UserProfile
    {
        private int _genderVote = 0;

        public String gender
        {
            set
            {
                if (string.Equals(value, "Male", StringComparison.OrdinalIgnoreCase))
                    this._genderVote = this._genderVote + 1;
                else if (string.Equals(value, "Female",  StringComparison.OrdinalIgnoreCase))
                    this._genderVote = this._genderVote - 1;
            }

            get
            {
                if (this._genderVote == 0)
                    return "Unknown";
                else if (this._genderVote > 0)
                    return "Male";
                else
                    return "Female";
            }
        }

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
