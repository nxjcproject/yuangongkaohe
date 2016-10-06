using SqlServerDataAdapter;
using StaffAssessment.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StaffAssessment.Service.StaffAssessment
{
    public class StaffAssessmentService
    {
        public static DataTable GetWorkingSectionByStaffSignIn(string mOrganizationID, string mStartTime, string mEndTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT distinct( convert(int,A.[StaffID])) as id
                                 ,A.[StaffID]+' '+B.[Name] as [text]
                                 ,B.[Name] ,A.[Shifts]
                              FROM [dbo].[shift_staffSignInRecord] A,[dbo].[system_StaffInfo] B
                              where A.[StaffID]=B.[StaffInfoID]
                              and A.[OrganizationID] like @mOrganizationID+'%'
                              and convert(datetime,[vDate])>=convert(datetime,@mStartTime)
                              and convert(datetime,[vDate])<=convert(datetime,@mEndTime)
                            union
                            SELECT convert(int,0) as id, '全部' as [text],'' as [Name],'' as [Shifts]
                              order by convert(int,A.[StaffID]),[Shifts],[Name]";
            SqlParameter[] para = { 
                                      new SqlParameter("@mOrganizationID", mOrganizationID),
                                      new SqlParameter("@mStartTime", mStartTime),
                                      new SqlParameter("@mEndTime", mEndTime)
                                  };
            DataTable dt = factory.Query(mySql, para);
            return dt;
        }

        public static DataTable GetAssessmentVersionTable(string mOrganizationID, string mWorkingSectionID) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT [KeyId]
                                    ,[Name]
                                    ,[Type]
                                    ,[OrganizationID]
                                    ,[WorkingSectionID]
                                FROM [NXJC_DEV].[dbo].[tz_Assessment]
                                where [OrganizationID]=@mOrganizationID
                                and [WorkingSectionID]=@mWorkingSectionID";
            SqlParameter[] para = { new SqlParameter("@mOrganizationID", mOrganizationID) ,
                                  new SqlParameter("@mWorkingSectionID", mWorkingSectionID) 
                                  };
            DataTable dt = factory.Query(mySql, para);
            return dt;          
        }
        public static DataTable GetAssessmentResultTableByDay(string mOrganizationID, string mWorkingSectionID, string mStaffId, string mGroupId, string mStartTime, string mEndTime, string mVersionId, string mStatisticalCycle) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
                string mySql = @"SELECT A.[KeyId]
                                  ,A.[Name]
	                              ,B.[Name] as StaffName
                                  ,A.[StaffID]
                                  ,A.[OrganizationID]
                                  ,A.[WorkingSectionID]
                                  ,A.[StartTime]
                                  ,A.[EndTime]
                                  ,A.[TimeStamp]
                                  ,A.[GroupId]
                                  ,A.[Remark]
                                  ,A.[Creator]
                                  ,A.[CreateTime]
                              FROM  [dbo].[tz_ShiftAssessmentResult] A,[dbo].[system_StaffInfo] B
                              where A.[StaffID]=B.[StaffInfoItemId]
                              and A.[OrganizationID]=@mOrganizationID
                              and A.[WorkingSectionID]=@mWorkingSectionID
                              and A.[GroupId]=@mGroupId
                              {0}
                              and A.[StartTime]>=convert(datetime,@mStartTime)
                              and A.[EndTime]<=convert(datetime,@mEndTime)";
            SqlParameter[] para = { 
                                      new SqlParameter("@mOrganizationID", mOrganizationID),
                                      new SqlParameter("@mWorkingSectionID", mWorkingSectionID),
                                      new SqlParameter("@mGroupId", mGroupId),
                                      new SqlParameter("@mStartTime", mStartTime),
                                      new SqlParameter("@mEndTime", mEndTime)
                                  };
            DataTable dt = new DataTable();
            if (mStaffId.Equals("0"))
            {
                mySql = string.Format(mySql, @"");
                dt = factory.Query(mySql, para);
            }
            else {
                mySql = string.Format(mySql, @" and A.[StaffID]=@mStaffId");
                SqlParameter[] paras = new SqlParameter[6];
                para.CopyTo(paras, 0);
                paras[5] = new SqlParameter("@mStaffId", mStaffId);
                dt = factory.Query(mySql, paras);
            }

            return dt;     
        
        }
        public static DataTable GetAssessmentResultDetailTableByDay(string mOrganizationID, string mWorkingSectionID, string mStaffId, string mGroupId, string mStartTime, string mEndTime, string mVersionId, string mStatisticalCycle) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT A.[Id]
                                  ,D.[Name] as StaffName
                                  ,A.[AssessmentId]
                                  ,C.[AssessmentName]
                                  ,C.[ObjectName]
                                  ,A.[ObjectId]
                                  ,A.[OrganizationID]
                                  ,A.[KeyId]
                                  ,A.[WeightedValue]
                                  ,A.[BestValue]
                                  ,A.[WorstValue]
                                  ,A.[AssessmenScore]
                                  ,A.[WeightedAverageCredit]
                            FROM  [dbo].[assessment_ShiftAssessmentResultDetail] A,[dbo].[tz_ShiftAssessmentResult] B
                              ,[dbo].[assessment_AssessmentDetail] C,[dbo].[system_StaffInfo] D
                            where B.[OrganizationID]=@mOrganizationID
                            and B.[WorkingSectionID]=@mWorkingSectionID
                            and B.[GroupId]=@mGroupId
                             {0}                        
                            and B.[StartTime]>=convert(datetime,@mStartTime)
                            and B.[EndTime]<=convert(datetime,@mEndTime)
                            and A.[KeyId]=B.[KeyId]
                            and C.[AssessmentId]=A.[AssessmentId]
                            and C.[ObjectId]=A.[ObjectId]
                            and C.[OrganizationID] like @mOrganizationID+'%'
                            and C.[KeyId]=@mVersionId
                            and B.[StaffID]=D.[StaffInfoItemId]";
            SqlParameter[] para = { 
                                      new SqlParameter("@mOrganizationID", mOrganizationID),
                                      new SqlParameter("@mWorkingSectionID", mWorkingSectionID),
                                      new SqlParameter("@mGroupId", mGroupId),
                                      new SqlParameter("@mStartTime", mStartTime),
                                      new SqlParameter("@mEndTime", mEndTime),
                                      new SqlParameter("@mVersionId", mVersionId)
                                  };
            DataTable dt = new DataTable();
            if (mStaffId.Equals("0"))
            {
                mySql = string.Format(mySql, @"");
                dt = factory.Query(mySql, para);
            }
            else
            {
                mySql = string.Format(mySql, @" and B.[StaffID]=@mStaffId");
                SqlParameter[] paras = new SqlParameter[7];
                para.CopyTo(paras, 0);
                paras[6] = new SqlParameter("@mStaffId", mStaffId);
                dt = factory.Query(mySql, paras);
            }
            return dt;     
        }
        public static DataTable GetStaffAssessmentTZ(string mProductionID, string mWorkingSectionID, string mStaffId, string mStaffName, string mGroupId, string mGroupName, string mStartTime,string mEndTime, string mVersionId, string mStatisticalCycle, string mCreator) 
        {
            //需加入验证是否存在该考核

            DataTable tztable = tztableStructrue();
         //   DataTable table = tableStructure();
            DateTime time01 = new DateTime();
            DateTime time02 = new DateTime();
            string starTime = "";
            string endTime = "";
            if (mStatisticalCycle.Equals("month"))
            {
                time01 = Convert.ToDateTime(mStartTime + "-01");
                time02 = time01.AddMonths(1).AddMinutes(-1);
                starTime = time01.ToString("yyyy-MM-dd HH:mm:ss");
                endTime = time02.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (mStatisticalCycle.Equals("year"))
            {
                starTime = Convert.ToDateTime(mStartTime + "-01-01 00:00:00").ToString("yyyy-MM-dd HH:mm:ss");
                endTime = Convert.ToDateTime(mStartTime + "-12-31 23:59:59").ToString("yyyy-MM-dd HH:mm:ss");

            }
            else if (mStatisticalCycle.Equals("day"))
            { 
                starTime = Convert.ToDateTime(mStartTime + " 00:00:00").ToString("yyyy-MM-dd HH:mm:ss");
                endTime = Convert.ToDateTime(mStartTime +  "  23:59:59").ToString("yyyy-MM-dd HH:mm:ss");           
            }
            //全部员工
            if (mStaffId.Equals("0"))
            {
                DataTable staffList = commonClass.GetStaffInfoTable(mProductionID, mWorkingSectionID);
                foreach (DataRow dr in staffList.Rows)
                {
                    if (!dr["Name"].ToString().Trim().Equals(""))
                        tztable.Rows.Add(System.Guid.NewGuid().ToString(), mGroupName, dr["id"].ToString().Trim(), mProductionID, mWorkingSectionID, starTime, endTime, DateTime.Now.ToString(), mGroupId,null, mCreator,null, dr["Name"].ToString().Trim());
                }
            }
            else {
                tztable.Rows.Add(System.Guid.NewGuid().ToString(), mGroupName, mStaffId, mProductionID, mWorkingSectionID, starTime, endTime, DateTime.Now.ToString(), mGroupId,null, mCreator,null, mStaffName);                  
            }
            return tztable;                
        }
        //public static DataTable GetStaffAssessmentCalculateResult(string mProductionID, string mWorkingSectionID, string mStaffId, string mStaffName, string mGroupName, string mGroupId, string mStartTime, string mVersionId, string mStatisticalCycle) 
        //{
        //    DataTable table = new DataTable();

        //    return table;
        //}
        public static DataTable GetStaffAssessmentCalculateResult(DataTable tzTable, string mVersionId, string mStatisticalCycle)
        {

            //mVersionId,        7
            //mStatisticalCycle  8
            // [Id]
            //,[AssessmentId]
            //,[ObjectId]
            //,[OrganizaitonID]
            //,[KeyId]
            //,[WeightedValue]
            //,[BestValue]
            //,[WorstValue]
            //,[AssessmenScore]
            //,[WeightedAverageCredit]

            //tzTable字段
            //table.Columns.Add("KeyId", typeof(string));
            //table.Columns.Add("StaffID", typeof(string));
            //table.Columns.Add("mStaffName", typeof(string));
            //table.Columns.Add("OrganizationID", typeof(string));
            //table.Columns.Add("WorkingSectionID", typeof(string));
            //table.Columns.Add("StartTime", typeof(string));
            //table.Columns.Add("EndTime", typeof(string));
            //table.Columns.Add("TimeStamp", typeof(string));
            //table.Columns.Add("GroupId", typeof(string));    
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            DataTable table = new DataTable();
            string mKeyId = "";
            string mProductionID="";
            string mWorkingSectionID = "";
            string mStaffId = "";
            string starTime = "";
            string endTime = "";



            DataTable myAssessmentItemsTable = GetStaffAssessmentItems(mVersionId);
            DataTable m_AssessmentTable = new DataTable();
            //DataTable m_AssessmentTable = myAssessmentItemsTable.Clone();
            for (int i = 0; i < tzTable.Rows.Count; i++)
            {
                //DataTable myWorkingTimeTable, DataTable myAssessmentItemsTable
                //   1.获取员工工作时间段表    2 .考核项表
                mKeyId = tzTable.Rows[i]["KeyId"].ToString();
                mProductionID = tzTable.Rows[i]["OrganizationID"].ToString();
                mWorkingSectionID = tzTable.Rows[i]["WorkingSectionID"].ToString();
                mStaffId = tzTable.Rows[i]["StaffID"].ToString();
                starTime = tzTable.Rows[i]["StartTime"].ToString();
                endTime = tzTable.Rows[i]["EndTime"].ToString();
                DataTable myWorkingTimeTable = GetStaffSignInTimeList( mProductionID,mWorkingSectionID,mStaffId,starTime,endTime );

               // DataTable mAssessmentTable = Test.GetStaffAssessment(myWorkingTimeTable, myAssessmentItemsTable);
                DataTable mAssessmentTable = Function_AssessmentCaculate.GetStaffAssessment(myWorkingTimeTable, myAssessmentItemsTable,factory);
                //for (int j = 0; j < mAssessmentTable.Rows.Count;j++ )
                //{
                foreach(DataRow dr in mAssessmentTable.Rows){
                   dr["KeyId"]=mKeyId;
                   dr["id"] = System.Guid.NewGuid().ToString();
                }
                m_AssessmentTable.Merge(mAssessmentTable);
            }
            m_AssessmentTable.Columns.Add("AssessmentName", typeof(string));
            DataTable AssessmentCatalogueTable = Table_AssessmentCatalogue.GetAssessmentCatalogue();

            for (int i = 0; i < m_AssessmentTable.Rows.Count;i++ )
            {
                for (int j = 0; j < AssessmentCatalogueTable.Rows.Count;j++ )
                {

                    if (m_AssessmentTable.Rows[i]["AssessmentId"].ToString().Trim().Equals(AssessmentCatalogueTable.Rows[j]["AssessmentId"].ToString().Trim()))
                    {
                        m_AssessmentTable.Rows[i]["AssessmentName"] = AssessmentCatalogueTable.Rows[j]["Name"];              
                    }
                }
            }
            return m_AssessmentTable; 
        }
        private static DataTable GetAssessmentCatalogue()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT  [AssessmentId]
                                  ,[Name]
                                  ,[Type]
                                  ,[ValueType]
                                  ,[Enabled]
                                  ,[Creator]
                                  ,[CreateTime]
                                  ,[Remark]
                              FROM [dbo].[assessment_AssessmentCatalogue]
                              where [Enabled]=1";

            DataTable AssessmentCatalogueTable = factory.Query(mySql);
            return AssessmentCatalogueTable;           
        }
        private static DataTable tztableStructrue() 
        {                  
            DataTable table = new DataTable();
                      // [KeyId]
                      //,[Name]
                      //,[StaffID]
                      //,[OrganizationID]
                      //,[WorkingSectionID]
                      //,[StartTime]
                      //,[EndTime]
                      //,[TimeStamp]
                      //,[GroupId]
                      //,[Remark]
                      //,[Creator]
                      //,[CreateTime]
            table.Columns.Add("KeyId", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("StaffID", typeof(string));
            table.Columns.Add("OrganizationID", typeof(string));
            table.Columns.Add("WorkingSectionID", typeof(string));
            table.Columns.Add("StartTime", typeof(string));
            table.Columns.Add("EndTime", typeof(string));
            table.Columns.Add("TimeStamp", typeof(string));
            table.Columns.Add("GroupId", typeof(string));
            table.Columns.Add("Remark", typeof(string));
            table.Columns.Add("Creator", typeof(string));
            table.Columns.Add("CreateTime", typeof(string));
            table.Columns.Add("StaffName", typeof(string));
            return table;           
        }
        private static DataTable tableStructure() 
        {
            DataTable table = new DataTable();
                      // [Id]
                      //,[AssessmentId]
                      //,[ObjectId]
                      //,[OrganizationID]
                      //,[KeyId]
                      //,[WeightedValue]
                      //,[BestValue]
                      //,[WorstValue]
                      //,[AssessmenScore]
                      //,[WeightedAverageCredit]
                      //mStaffName
            table.Columns.Add("Id", typeof(string));
            table.Columns.Add("AssessmentId", typeof(string));
            table.Columns.Add("ObjectId", typeof(string));
            table.Columns.Add("OrganizationID", typeof(string));
            table.Columns.Add("KeyId", typeof(string));
            table.Columns.Add("WeightedValue", typeof(string));
            table.Columns.Add("BestValue", typeof(string));
            table.Columns.Add("WorstValue", typeof(string));
            table.Columns.Add("AssessmenScore", typeof(string));
            table.Columns.Add("WeightedAverageCredit", typeof(string));
            table.Columns.Add("StaffName", typeof(string));

            return table;     
        }
        /// <summary>
        /// 获取某个员工的签到时间
        /// </summary>
        /// <returns></returns>
        private static DataTable GetStaffSignInTimeList(string mProductionID,string mWorkingSectionID,string mStaffId,string starTime,string endTime ) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT convert(datetime,A.vDate+' '+B.[StartTime]) as StartTime
	                              ,convert(datetime,A.vDate+' '+B.[EndTime]) as EndTime
                              FROM [dbo].[shift_staffSignInRecord] A,
                                   [dbo].[system_WorkingSectionShiftDescription] B
                              where A.[WorkingSectionID]=B.[WorkingSectionID]
                              and A.[Shifts]=B.[Shifts]
                              and A.[OrganizationID]=@mProductionID
                              and A.[WorkingSectionID]=@mWorkingSectionID
                              and A.[StaffID]=@mStaffId
                              and convert(datetime,A.[vDate])>=convert(datetime,@starTime)
                              and convert(datetime,A.[vDate])<=convert(datetime,@endTime)";
            SqlParameter[] para = { 
                                      new SqlParameter("@mProductionID", mProductionID) ,
                                      new SqlParameter("@mWorkingSectionID", mWorkingSectionID) ,
                                      new SqlParameter("@mStaffId", mStaffId) ,
                                      new SqlParameter("@starTime", starTime) ,
                                      new SqlParameter("@endTime", endTime) 
                                  };
            DataTable mSignInTime = factory.Query(mySql, para);
            return mSignInTime;            
        }
        /// <summary>
        /// 获取岗位考核项
        /// </summary>
        /// <param name="mKeyId"></param>
        /// <returns></returns>
        public static DataTable GetStaffAssessmentItems(string mKeyId) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT [Id]
                                  ,[AssessmentId]
                                  ,[ObjectId]
                                  ,[OrganizationID]
                                  ,[KeyId]
                                  ,[WeightedValue]
                                  ,[BestValue]
                                  ,[WorstValue]
                                  ,[StandardValue]
                                  ,[StandardScore]
                                  ,[ScoreFactor]
                                  ,[MaxScore]
                                  ,[MinScore]
                                  ,[Enabled]
                          FROM [dbo].[assessment_AssessmentDetail]
                              where Enabled=1
                              and KeyId=@mKeyId
                            group by [AssessmentId],[ObjectId]
                              ,[OrganizationID]
                              ,[KeyId],[Id] ,[WeightedValue]
                              ,[BestValue]
                              ,[WorstValue]
                              ,[StandardValue]
                              ,[StandardScore]
                              ,[ScoreFactor]
                              ,[MaxScore]
                              ,[MinScore]
                              ,[Enabled]";
            SqlParameter para = new SqlParameter("@mKeyId", mKeyId);
            DataTable mAssessmentItems = factory.Query(mySql, para);
            return mAssessmentItems;   
        }

        public static int InsertStaffAssessmentResult(DataTable tzTable ,DataTable detailTable) 
        {
            //插入这两个表
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);           
            //////
                 //验证是否已经插入？？只验证tz表的插入即可！
                //已经插入 result=-1;
            /////
            int result = factory.Save("tz_ShiftAssessmentResult", tzTable) + factory.Save("assessment_ShiftAssessmentResultDetail", detailTable);         
            result = result > 0 ? 1 : 0;

            return result;
        }
        public static DataTable tzTableStructure()
        {
            DataTable table = new DataTable();
            table.Columns.Add("KeyId", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("StaffID", typeof(string));
            table.Columns.Add("OrganizationID", typeof(string));
            table.Columns.Add("WorkingSectionID", typeof(string));
            table.Columns.Add("StartTime", typeof(string));
            table.Columns.Add("EndTime", typeof(string));
            table.Columns.Add("TimeStamp", typeof(string));
            table.Columns.Add("GroupId", typeof(string));
            table.Columns.Add("Remark", typeof(string));
            table.Columns.Add("Creator", typeof(string));
            table.Columns.Add("CreateTime", typeof(string));
            return table;
        }
        public static DataTable detailTableStructure()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(string));
            table.Columns.Add("AssessmentId", typeof(string));
            table.Columns.Add("ObjectId", typeof(string));
            table.Columns.Add("OrganizationID", typeof(string));
            table.Columns.Add("KeyId", typeof(string));
            table.Columns.Add("WeightedValue", typeof(string));
            table.Columns.Add("BestValue", typeof(string));
            table.Columns.Add("WorstValue", typeof(string));
            table.Columns.Add("AssessmenScore", typeof(string));
            table.Columns.Add("WeightedAverageCredit", typeof(string));
            return table;
        }
    }
}
