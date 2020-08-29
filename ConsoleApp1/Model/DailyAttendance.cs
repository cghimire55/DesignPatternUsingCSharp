using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Model
{
    public class DailyAttendance
    {
        public string UserId { get; set; }
        public DateTime AttendanceMinTime { get; set; }
        public DateTime AttendanceMaxTime { get; set; }
        public string MacAddress { get; set; }
    }
    public class AttendanceModel
    {
        public int Id { get; set; }
        public DateTime AttendanceDateFrom { get; set; }

        public string AttendanceDateFromBS { get; set; }
        public DateTime AttendanceDateTo { get; set; }

        public string AttendanceDateToBS { get; set; }
        public DateTime RequestDate { get; set; }

        public string RequestDateBS { get; set; }

        public string EmpCode { get; set; }
        public string UserId { get; set; }
        public TimeSpan LogTime { get; set; }

        public string Remarks { get; set; }
        public int Direction { get; set; }

        public int Status { get; set; }

        public int Source { get; set; }

        public int RecommendStatus { get; set; }

        public string RecommendRemarks { get; set; }

        public int Recommend2Status { get; set; }

        public string Recommend2Remarks { get; set; }

        public string LoginRequestIP { get; set; }

        public string LoginMACAddress { get; set; }

        public string MapLocation { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovalRemarks { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
    public enum AttendanceDirection
    {
        Checkin = 1,
        CheckOut = 2,
        Breakin = 3,
        BreakOut = 4
    }
    public enum AttendanceSource
    {
        Web,
        Wifi,
    }
    public enum ApprovalStatus
    {
        None = 0,
        Pending = 1,
        Forward = 2,
        Reject = 3,
        ReDraft = 4,
        Approved = 5,
    }
}
