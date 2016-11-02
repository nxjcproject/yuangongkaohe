using SqlServerDataAdapter;
using StaffAssessment.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StaffAssessment.Service.StaffSignIn
{
   public class StaffSignInService
    {
       public static DataTable GetWorkingSectionTable(string mOrganizationID, string mWorkingSectionID)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
           string mySql = @"SELECT A.[ShiftDescriptionID]
                                  ,B.[WorkingSectionType] as [WorkingSectionName]
                                  ,A.[WorkingSectionID]
                                  ,A.[Shifts]
                                  ,A.[StartTime]
                                  ,A.[EndTime]
                                  ,A.[Remark]
                              FROM [dbo].[system_WorkingSectionShiftDescription] A,[dbo].[system_WorkingSectionType] B
                              where A.[WorkingSectionID]=B.[WorkingSectionID]
                              and B.[OrganizationID] =@mOrganizationID
                              and A.[WorkingSectionID]=@mWorkingSectionID";
           SqlParameter[] para = {
                                    new SqlParameter("@mOrganizationID", mOrganizationID),
                                    new SqlParameter("@mWorkingSectionID", mWorkingSectionID)
                                 };
           DataTable dt = factory.Query(mySql, para);
           return dt;
       }
       public static DataTable GetStaffInfoTable(string mOrganizationID, string team)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           if (team == "全部"|| team == "")
           {
               string allMySql = @"SELECT [StaffInfoID]+' '+[Name] as text
                              ,[StaffInfoID] as id
                              ,[StaffInfoItemId]
	                          ,[Name]
                              ,[OrganizationID]
                              ,[WorkingTeamName]
                              ,[WorkingSectionID]      
                              ,[Sex]
                              ,[PhoneNumber]
                              ,[StaffType]
                              ,[Enabled]
                          FROM [dbo].[system_StaffInfo]
                          where Enabled=1
                          and [StaffType]!='superior' or [StaffType] is null
                          and [OrganizationID]=@mOrganizationID
                          order by [StaffInfoID]";
               SqlParameter para = new SqlParameter("@mOrganizationID", mOrganizationID);
               DataTable AllDt = dataFactory.Query(allMySql, para);
               return AllDt;
           }
           else
           {
               string mySql = @"SELECT [StaffInfoID]+' '+[Name] as text
                              ,[StaffInfoID] as id
                              ,[StaffInfoItemId]
	                          ,[Name]
                              ,[OrganizationID]
                              ,[WorkingTeamName]
                              ,[WorkingSectionID]      
                              ,[Sex]
                              ,[PhoneNumber]
                              ,[StaffType]
                              ,[Enabled]
                          FROM [dbo].[system_StaffInfo]
                          where Enabled=1
                          and [StaffType]!='superior' or [StaffType] is null
                          and [WorkingTeamName] like @team+'%'
                          and [OrganizationID]=@mOrganizationID
                          order by [StaffInfoID]";
               SqlParameter[] paras = {
                                  new SqlParameter ("@mOrganizationID", mOrganizationID),
                                  new SqlParameter("@team", team)
                                                };
               DataTable dt = dataFactory.Query(mySql, paras);
               return dt;
           }
       }
       public static int Save(string mworkingId, string organizationId, string mvDate, string mShift, string itemId)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"insert into [dbo].[shift_staffSignInRecord] (vDate,StaffID,OrganizationID,WorkingSectionID,Shifts) values
                                          (@mvDate,@itemId,@organizationId,@mworkingId,@mShift)";
           SqlParameter[] parameters = {
                                    new SqlParameter("@mworkingId", mworkingId),
                                    new SqlParameter("@organizationId", organizationId),
                                    new SqlParameter("@mvDate", mvDate), 
                                    new SqlParameter("@itemId", itemId),
                                    new SqlParameter("@mShift", mShift)
                                         };
           int result = dataFactory.ExecuteSQL(mySql, parameters);
           return result;
       }
       public static DataTable GetHistoryStaffSignInTable(string mOrganizationID, string itemID, string mStartTime, string mEndTime)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
             string  mySql = @"SELECT A.[RecordId]
                          ,A.[vDate]
                          ,A.[StaffID]
                          ,C.[Name] as StaffName
                          ,A.[OrganizationID]
                          ,A.[WorkingSectionID]
	                      ,B.[WorkingSectionType] as [WorkingSectionName]
                          ,D.[Shifts]
                          ,D.[StartTime]
                          ,D.[EndTime]
                          ,A.[Creator]
                          ,A.[CreateTime]
                          ,A.[Remark] 
                  FROM [dbo].[shift_staffSignInRecord] A,[dbo].[system_WorkingSectionType] B,
                   [dbo].[system_StaffInfo] C,[dbo].[system_WorkingSectionShiftDescription] D
                    where A.[WorkingSectionID]=B.[WorkingSectionID]
                    and A.[StaffID]=C.[StaffInfoItemId]
                    and D.[ShiftDescriptionID]=A.[Shifts]
                    and A.OrganizationID=@mOrganizationID
                    and A.[StaffID]=@itemID  
                    and convert(datetime,A.[vDate])>=convert(datetime,@mStartTime)
                    and convert(datetime,A.[vDate])<=convert(datetime,@mEndTime)                 
                    order by convert(datetime,[vDate])";
               SqlParameter[] parameter = { 
                                            new SqlParameter("@mOrganizationID", mOrganizationID),
                                            new SqlParameter("@itemID", itemID), 
                                            new SqlParameter("@mStartTime", mStartTime),
                                            new SqlParameter("@mEndTime", mEndTime)
                                       };
             DataTable dt = factory.Query(mySql, parameter);
           return dt;
       }
       public static int InsertSignIn(string mOrganizationID, string mVdate, string mStaffId, string mWorkingSectionID, string mShifts, string mUserName)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
           int mreturn=0;
           //检查一致性
           string selectSQL = @"SELECT * FROM [dbo].[shift_staffSignInRecord]
                                where [vDate]=@mVdate
                                and [StaffID]=@mStaffId
                                and [OrganizationID]=@mOrganizationID
                                and [WorkingSectionID]=@mWorkingSectionID";
           SqlParameter[] para = {
                                 new SqlParameter("@mVdate",mVdate),
                                 new SqlParameter("@mStaffId",mStaffId),
                                 new SqlParameter("@mOrganizationID",mOrganizationID),
                                 new SqlParameter("@mWorkingSectionID",mWorkingSectionID)
                                 };
           DataTable selectTable = factory.Query(selectSQL, para);

           if (selectTable.Rows.Count > 0)
           {
               mreturn = -1;        //已经签到
           }
           else {
               string insertSQL = @"INSERT INTO [dbo].[shift_staffSignInRecord]
                                       ([RecordId]
                                       ,[vDate]
                                       ,[StaffID]
                                       ,[OrganizationID]
                                       ,[WorkingSectionID]
                                       ,[Shifts]
                                       ,[Creator]
                                       ,[CreateTime]
                                       ,[Remark])
                                 VALUES
                                       (@mRecordId
                                       ,@mvDate
                                       ,@mStaffID
                                       ,@mOrganizationID
                                       ,@mWorkingSectionID
                                       ,@mShifts
                                       ,@mCreator
                                       ,@mCreateTime
                                       ,null)";
               SqlParameter[] param = {
                                    new SqlParameter("@mRecordId",System.Guid.NewGuid().ToString()),
                                    new SqlParameter("@mvDate",mVdate),
                                    new SqlParameter("@mStaffID",mStaffId),
                                    new SqlParameter("@mOrganizationID",mOrganizationID),
                                    new SqlParameter("@mWorkingSectionID",mWorkingSectionID),
                                    new SqlParameter("@mShifts",mShifts),
                                    new SqlParameter("@mCreator",mUserName),
                                    new SqlParameter("@mCreateTime",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                                 };
               mreturn = factory.ExecuteSQL(insertSQL,param);                
           }
            return mreturn;
       }
    }
}
