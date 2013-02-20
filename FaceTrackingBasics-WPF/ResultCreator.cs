using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceTrackingBasics
{

    class ResultCreator
    {
        private CsvExport _csvExport;

        public ResultCreator()
        {
            _csvExport = new CsvExport();
        }


        public void saveStaticUserProfiles(ElicitationSession session, List<UserProfile> userProfiles)
        {
            foreach (UserProfile user in userProfiles)
            {
                _csvExport.AddRow();
                _csvExport["SessionID"] = session.sessionID;
                _csvExport["Start Time"] = session.startTime;
                _csvExport["End Time"] = session.endTime;
                _csvExport["# of Users"] = userProfiles.Count;
                _csvExport["User ID"] = user.userID;
                _csvExport["Gender"] = user.gender;
                _csvExport["Is Child"] = user.userIsChild;
            }
        }


        public void saveDynamicUserProfiles(ElicitationSession session, List<UserProfile> userProfiles)
        {
            foreach (UserProfile user in userProfiles)
            {
                _csvExport.AddRow();
                _csvExport["SessionID"] = session.sessionID;
                _csvExport["Start Time"] = session.startTime;
                _csvExport["End Time"] = session.endTime;
                _csvExport["Is User Sleeping"] = user.userSleeping;
                _csvExport["Is User Distracted"] = user.userDistracted;
            }
        }


        public void ExportToFile(string path)
        {
            this._csvExport.ExportToFile(path);
        }
    }
}
