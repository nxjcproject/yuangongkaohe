﻿using SqlServerDataAdapter;
using StaffAssessment.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StaffAssessment.Service.StaffSignIn
{
    public class StaffSignInModifyService
    {
        public static DataTable GetStaffInfoTable(string mOrganizationID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT [StaffInfoID]+' '+[Name] as text
                              ,[StaffInfoID] as id
                              ,[StaffInfoItemId]
	                          ,[Name]
                              ,[OrganizationID]
                              ,[WorkingTeamName]
                              ,[WorkingSectionID]      
                              ,[Sex]
                              ,[PhoneNumber]
                              ,[Enabled]
                          FROM [dbo].[system_StaffInfo]
                          where Enabled=1
                          and [OrganizationID] like @mOrganizationID+'%'
                          order by [StaffInfoID]";
            SqlParameter para = new SqlParameter("@mOrganizationID", mOrganizationID);
            DataTable dt = factory.Query(mySql, para);
            return dt;
        }
        public static DataTable GetHistoryStaffSignInTable(string mOrganizationId, string mStaffId, string mStartTime, string mEndTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = "";
            DataTable dt = new DataTable();
            if (mStaffId != "0")
            {
                mySql = @"SELECT A.[RecordId]
                          ,A.[vDate]
                          ,A.[StaffID]
                          ,C.[Name] as StaffName
                          ,A.[OrganizationID]
                          ,A.[WorkingSectionID]
	                      ,B.[WorkingSectionName]
                          ,D.[Shifts]
                          ,A.[Creator]
                          ,A.[CreateTime]
                          ,A.[Remark] 
                  FROM [dbo].[shift_staffSignInRecord] A,[dbo].[system_WorkingSection] B,
                   [dbo].[system_StaffInfo] C,[dbo].[system_WorkingSectionShiftDescription] D
                    where A.[WorkingSectionID]=B.[WorkingSectionItemID]
                    and A.[StaffID]=C.[StaffInfoItemId]
                    and A.[Shifts]=D.[ShiftDescriptionID]
                    and A.OrganizationID like @mOrganizationId+'%'
                    and A.[StaffID]=@mStaffId   
                    and convert(datetime,[vDate])>=convert(datetime,@mStartTime)
                    and convert(datetime,[vDate])<=convert(datetime,@mEndTime)                 
                    order by convert(datetime,[vDate])";
                SqlParameter[] parameter = { 
                                           new SqlParameter("@mOrganizationId", mOrganizationId),
                                           new SqlParameter("@mStaffId", mStaffId), 
                                            new SqlParameter("@mStartTime", mStartTime),
                                             new SqlParameter("@mEndTime", mEndTime)
                                       };
                dt = factory.Query(mySql, parameter);
            }
            else
            {
                mySql = @"SELECT A.[RecordId]
                          ,A.[vDate]
                          ,A.[StaffID]
                          ,C.[Name] as StaffName
                          ,A.[OrganizationID]
                          ,A.[WorkingSectionID]
	                      ,B.[WorkingSectionName]
                          ,D.[Shifts]
                          ,A.[Creator]
                          ,A.[CreateTime]
                          ,A.[Remark] 
                  FROM [dbo].[shift_staffSignInRecord] A,[dbo].[system_WorkingSection] B,
                   [dbo].[system_StaffInfo] C,[dbo].[system_WorkingSectionShiftDescription] D
                    where A.[WorkingSectionID]=B.[WorkingSectionItemID]
                    and A.[StaffID]=C.[StaffInfoItemId]
                    and A.[Shifts]=D.[ShiftDescriptionID]
                    and A.OrganizationID like @mOrganizationId+'%'
                    and convert(datetime,[vDate])>=convert(datetime,@mStartTime)
                    and convert(datetime,[vDate])<=convert(datetime,@mEndTime)                 
                    order by convert(datetime,[vDate])";
                SqlParameter[] parameter = { 
                                               new SqlParameter("@mOrganizationId", mOrganizationId),   
                                               new SqlParameter("@mStartTime", mStartTime),
                                               new SqlParameter("@mEndTime", mEndTime)     
                                           };
                dt = factory.Query(mySql, parameter);
            }
            return dt;
        }
    }
}
