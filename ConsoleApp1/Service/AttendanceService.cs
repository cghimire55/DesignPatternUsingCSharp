using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ConsoleApp1.Model;

namespace ConsoleApp1.Service
{
    public class AttendanceService
    {
       // private readonly string ConnString = ConfigurationManager.ConnectionStrings["SmartHajir"].ConnectionString.ToString();
        public static void SyncAttendance()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartHajir"].ConnectionString);
            using (conn)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region insert if any pending attendance
                        var pendingData = conn.Query<DailyAttendance>("exec SpGetPendingAttendance", transaction: trans).ToList();
                        if (pendingData.Count() > 0)
                        {
                            foreach (var item in pendingData)
                            {
                                string EmployeeCode = string.Empty;
                                EmployeeCode = conn.Query<string>($@"select EmpCode from Employees where UserId='{item.UserId}'", transaction: trans).FirstOrDefault();
                                AddAttendance(item, item.UserId, EmployeeCode, conn, trans);
                            }
                        }
                        #endregion

                        //var query = $@"select UserId,max(AttendanceTime) AttendanceMaxTime,min(AttendanceTime) AttendanceMinTime,MobileMacAddress MacAddress from AttendanceFromWifis group by UserId,MobileMacAddress,convert(date,AttendanceTime) order by AttendanceMaxTime desc";
                        var query = $@"select UserId,max(AttendanceTime) AttendanceMaxTime,min(AttendanceTime) AttendanceMinTime,MobileMacAddress MacAddress from AttendanceFromWifis where convert(date,attendancetime )=convert(date,getdate()) group by UserId,MobileMacAddress order by AttendanceMaxTime desc";
                        var result = conn.Query<DailyAttendance>(query, transaction: trans).ToList();
                        foreach (var item in result)
                        {
                            var query1 = string.Empty; var attendance = new List<AttendanceModel>(); var EmployeeCodeById = string.Empty;
                            query1 = $@"select * from Attendances where UserId='{item.UserId}'and Source={(int)AttendanceSource.Wifi} and convert(date,AttendanceDateFrom)='{item.AttendanceMaxTime.ToString("yyyy/MM/dd")}'";
                            attendance = conn.Query<AttendanceModel>(query1, transaction: trans).ToList();
                            var updateQuery = string.Empty;
                            if (attendance.Count > 0)
                            {
                                var checkinattendance = attendance.FirstOrDefault(x => x.Direction == (int)AttendanceDirection.Checkin);
                                if (checkinattendance != null)
                                {
                                    if (item.AttendanceMinTime < checkinattendance.AttendanceDateFrom)
                                    {
                                        //checkinattendance.AttendanceDateFrom = item.AttendanceMinTime;
                                        //checkinattendance.AttendanceDateTo = item.AttendanceMinTime;
                                        updateQuery = $@"Update Attendances Set AttendanceDateFrom='{item.AttendanceMinTime.ToString("yyyy/MM/dd HH:mm:ss")}',AttendanceDateTo='{item.AttendanceMinTime.ToString("yyyy/MM/dd HH:mm:ss")}' Where Id={checkinattendance.Id}";
                                        conn.Execute(updateQuery, transaction: trans);
                                    }
                                }
                                var checkoutattendance = attendance.FirstOrDefault(x => x.Direction == (int)AttendanceDirection.CheckOut);
                                if (checkoutattendance != null)
                                {
                                    if (item.AttendanceMaxTime > checkoutattendance.AttendanceDateTo)
                                    {
                                        updateQuery = string.Empty;
                                        //checkoutattendance.AttendanceDateFrom = item.AttendanceMaxTime;
                                        //checkoutattendance.AttendanceDateTo = item.AttendanceMaxTime;
                                        updateQuery = $@"Update Attendances Set AttendanceDateFrom='{item.AttendanceMaxTime.ToString("yyyy/MM/dd HH:mm:ss")}',AttendanceDateTo='{item.AttendanceMaxTime.ToString("yyyy/MM/dd HH:mm:ss")}' Where Id={checkoutattendance.Id}";
                                        conn.Execute(updateQuery, transaction: trans);
                                    }
                                }
                            }
                            else
                            {
                                EmployeeCodeById = conn.Query<string>($@"select EmpCode from Employees where UserId='{item.UserId}'", transaction: trans).FirstOrDefault();
                                AddAttendance(item, item.UserId, EmployeeCodeById, conn, trans);

                            }
                        }
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        string root = Environment.CurrentDirectory + @"\Error";
                        if (!Directory.Exists(root))
                        {
                            Directory.CreateDirectory(root);
                        }


                        string[] lines = new string[] { e.Message.ToString() };

                        string path = root + "\\AttendanceFromWifiError.txt";

                        File.AppendAllLines(path, lines);
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        private static void AddAttendance(DailyAttendance item, string userId, string EmployeeCodeById, IDbConnection conn, IDbTransaction trans)
        {

            var insertQuery = string.Empty;
            insertQuery = $@"insert into Attendances (AttendanceDateFrom,AttendanceDateFromBS,AttendanceDateTo,AttendanceDateToBS,RequestDate,RequestDateBS,EmpCode,UserId,LogTime,Remarks,
            Direction,Status,Source,RecommendStatus,RecommendRemarks,Recommend2Status,Recommend2Remarks,LoginRequestIP,LoginMACAddress,MapLocation,CreatedDate,CreatedBy)
            Select '{item.AttendanceMinTime.ToString("yyyy/MM/dd HH:mm:ss")}','','{item.AttendanceMinTime.ToString("yyyy/MM/dd HH:mm:ss")}','','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}','','{EmployeeCodeById}','{userId}','{DateTime.Now.Date.TimeOfDay}','',
            {(int)AttendanceDirection.Checkin},{(int)ApprovalStatus.Approved},{(int)AttendanceSource.Wifi},{(int)ApprovalStatus.Approved},'',{(int)ApprovalStatus.None},'',
            '{item.MacAddress}','{item.MacAddress}','','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}','{userId}'
            union all
            Select '{item.AttendanceMaxTime.ToString("yyyy/MM/dd HH:mm:ss")}','','{item.AttendanceMaxTime.ToString("yyyy/MM/dd HH:mm:ss")}','','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}','','{EmployeeCodeById}','{userId}','{DateTime.Now.Date.TimeOfDay}','',
            {(int)AttendanceDirection.CheckOut},{(int)ApprovalStatus.Approved},{(int)AttendanceSource.Wifi},{(int)ApprovalStatus.Approved},'',{(int)ApprovalStatus.None},'',
            '{item.MacAddress}','{item.MacAddress}','','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}','{userId}'";
            conn.Execute(insertQuery, transaction: trans);
        }
    }
}
