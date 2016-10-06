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
    public class AssessmentVersionDefineService
    {
        public static DataTable GetAssessmentVersionDefine(string mOrganizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT A.[KeyId]
                              ,A.[Name]
                              ,A.[Type]
	                          ,B.[Name] as [OrganizationName]
                              ,A.[OrganizationID]
	                          ,C.[WorkingSectionName]
                              ,A.[WorkingSectionID]
                              ,A.[Remark]
                              ,A.[Creator]
                              ,A.[CreateTime]
                          FROM [dbo].[tz_Assessment] A,[dbo].[system_Organization] B,[dbo].[system_WorkingSection] C
                          where A.[OrganizationID]=B.[OrganizationID]
                          and A.[WorkingSectionID]=C.[WorkingSectionID]
                          and A.[OrganizationID]=@mOrganizationId";
            SqlParameter para = new SqlParameter("@mOrganizationId", mOrganizationId);
            DataTable table = factory.Query(mySql, para);
            return table;          
        }
        public static DataTable GetAssessmentVersionDefine(string mProductionID, string mWorkingSectionID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            DataTable table = new DataTable();
            if (mWorkingSectionID.Equals("0"))
            {
                string mySql = @" SELECT A.[KeyId]
                              ,A.[Name]
                              ,A.[Type]
	                          ,B.[Name] as [OrganizationName]
                              ,A.[OrganizationID]
	                          ,C.[WorkingSectionType] as [WorkingSectionName]
                              ,A.[WorkingSectionID]
                              ,A.[Remark]
                              ,A.[Creator]
                              ,A.[CreateTime]
                          FROM [dbo].[tz_Assessment] A,[dbo].[system_Organization] B,[dbo].[system_WorkingSectionType] C
                          where A.[OrganizationID]=B.[OrganizationID]
                          and A.[WorkingSectionID]=C.[WorkingSectionID]
                          and A.[OrganizationID]=@mProductionID";
                table = factory.Query(mySql, new SqlParameter("@mProductionID", mProductionID));
            }
            else {
                string mySql = @" SELECT A.[KeyId]
                              ,A.[Name]
                              ,A.[Type]
	                          ,B.[Name] as [OrganizationName]
                              ,A.[OrganizationID]
	                          ,C.[WorkingSectionType] as [WorkingSectionName]
                              ,A.[WorkingSectionID]
                              ,A.[Remark]
                              ,A.[Creator]
                              ,A.[CreateTime]
                          FROM [dbo].[tz_Assessment] A,[dbo].[system_Organization] B,[dbo].[system_WorkingSectionType] C
                          where A.[OrganizationID]=B.[OrganizationID]
                          and A.[WorkingSectionID]=C.[WorkingSectionID]
                          and A.[OrganizationID]=@mProductionID
                          and A.[WorkingSectionID]=@mWorkingSectionID";
                SqlParameter[] para = { 
                                        new SqlParameter("@mProductionID", mProductionID) ,
                                        new SqlParameter("@mWorkingSectionID", mWorkingSectionID)
                                       };
                table = factory.Query(mySql, para);
            }
           
           
            return table;
        }
        public static Model_CalculateObjects GetCalculateObjects(string myType, string myValueType, string myOrganizationId)
        {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            return Table_CalculateObjects.GetCalculateObjects(myType, myValueType, myOrganizationId, factory);
        }
        public static int ToAddAssessmentVersion(string mOrganizationId, string mWorkingSectionID, string mName, string mType, string mUserName, string mRemark)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"INSERT INTO [dbo].[tz_Assessment]
                           ([KeyId]
                           ,[Name]
                           ,[Type]
                           ,[OrganizationID]
                           ,[WorkingSectionID]
                           ,[Remark]
                           ,[Creator]
                           ,[CreateTime])
                     VALUES
                           (@mKeyId
                           ,@mName
                           ,@mType
                           ,@mProductionID
                           ,@mWorkingSectionID
                           ,@mRemark
                           ,@mCreator
                           ,@mTime)";
            SqlParameter[] para = { 
                                    new SqlParameter("@mKeyId", System.Guid.NewGuid().ToString()) ,
                                    new SqlParameter("@mProductionID", mOrganizationId) ,
                                    new SqlParameter("@mWorkingSectionID", mWorkingSectionID) ,
                                    new SqlParameter("@mName", mName) ,
                                    new SqlParameter("@mType", mType) ,
                                    new SqlParameter("@mCreator", mUserName) ,
                                    new SqlParameter("@mRemark", mRemark) ,
                                    new SqlParameter("@mTime", DateTime.Now.ToString())
                                     };
            int result = factory.ExecuteSQL(mySql,para);
            return result;
        }
        /// <summary>
        /// 编辑操作
        /// </summary>
        /// <param name="mProductionID"></param>
        /// <param name="mWorkingSectionID"></param>
        /// <param name="mName"></param>
        /// <param name="mType"></param>
        /// <param name="mCreator"></param>
        /// <param name="mRemark"></param>
        /// <param name="mKeyId"></param>
        /// <returns></returns>
        public static int ToEditAssessmentVersion(string mOrganizationId, string mWorkingSectionID, string mName, string mType, string mCreator, string mRemark, string mKeyId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @" UPDATE [dbo].[tz_Assessment]
                               SET [Name] = @mName
                                  ,[Type] = @mType
                                  ,[OrganizationID] = @mOrganizationId
                                  ,[WorkingSectionID] = @mWorkingSectionID
                                  ,[Remark] = @mRemark
                                  ,[Creator] = @mCreator
                                  ,[CreateTime] = @mTime
                             WHERE [KeyId] =@mKeyId";
            SqlParameter[] para = { 
                                    new SqlParameter("@mKeyId", mKeyId) ,
                                    new SqlParameter("@mOrganizationId", mOrganizationId) ,
                                    new SqlParameter("@mWorkingSectionID", mWorkingSectionID) ,
                                    new SqlParameter("@mName", mName) ,
                                    new SqlParameter("@mType", mType) ,
                                    new SqlParameter("@mCreator", mCreator) ,
                                    new SqlParameter("@mRemark", mRemark) ,
                                    new SqlParameter("@mTime", DateTime.Now.ToString())
                                     };
            int result = factory.ExecuteSQL(mySql, para);
            return result;
        }
        public static DataTable GetAssessmentVersionDetailTable(string mKeyId) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT A.[Id]
                              ,A.[AssessmentId]
                              ,A.[AssessmentName]
                              ,A.[ObjectId]
                              ,A.[ObjectName]
	                          ,B.[Name] as [OrganizationName]
                              ,A.[OrganizationID]
                              ,A.[KeyId]
                              ,A.[WeightedValue]
                              ,A.[BestValue]
                              ,A.[WorstValue]
                              ,A.[StandardValue]
                              ,A.[StandardScore]
                              ,A.[ScoreFactor]
                              ,A.[MaxScore]
                              ,A.[MinScore]
                              ,A.[Enabled]
                          FROM [dbo].[assessment_AssessmentDetail] A,[dbo].[system_Organization] B
                          where A.[OrganizationID]=B.[OrganizationID]
                      and A.[KeyId]=@mKeyId";
            SqlParameter para = new SqlParameter("@mKeyId", mKeyId);
            DataTable table = factory.Query(mySql, para);
            return table;          
        
        }
        public static int ToAddAssessmentDetail(string mOrganizationID, string mKeyId, string mAssessmentId,string mAssessmentName, string mObjectId,string mObjectName, string mWeightedValue, string mBestValue, string mWorstValue, string mStandardValue, string mStandardScore, string mScoreFactor, string mMaxScore, string mMinScore, string mEnabled)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"INSERT INTO [dbo].[assessment_AssessmentDetail]
                                   ([Id]
                                   ,[AssessmentId]
                                   ,[AssessmentName]
                                   ,[ObjectId]
                                    ,[ObjectName]
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
                                   ,[Enabled])
                             VALUES
                                   (@mId
                                   ,@mAssessmentId
                                   ,@mAssessmentName
                                   ,@mObjectId
                                   ,@mObjectName
                                   ,@mOrganizationID
                                   ,@mKeyId
                                   ,@mWeightedValue
                                   ,@mBestValue
                                   ,@mWorstValue
                                   ,@mStandardValue
                                   ,@mStandardScore
                                   ,@mScoreFactor
                                   ,@mMaxScore
                                   ,@mMinScore
                                   ,@mEnabled)";
            SqlParameter[] para = { 
                                    new SqlParameter("@mId", System.Guid.NewGuid().ToString()) ,
                                    new SqlParameter("@mAssessmentId", mAssessmentId) ,
                                    new SqlParameter("@mAssessmentName", mAssessmentName) ,
                                    new SqlParameter("@mObjectId", mObjectId) ,
                                    new SqlParameter("@mObjectName", mObjectName) ,
                                    new SqlParameter("@mOrganizationID", mOrganizationID) ,
                                    new SqlParameter("@mKeyId", mKeyId) ,                                   
                                    new SqlParameter("@mWeightedValue", mWeightedValue) ,
                                    new SqlParameter("@mBestValue", mBestValue) ,
                                    new SqlParameter("@mWorstValue", mWorstValue),
                                    new SqlParameter("@mStandardValue", mStandardValue) ,
                                    new SqlParameter("@mStandardScore", mStandardScore) ,
                                    new SqlParameter("@mScoreFactor", mScoreFactor) ,
                                    new SqlParameter("@mMaxScore", mMaxScore) ,
                                    new SqlParameter("@mMinScore", mMinScore),
                                    new SqlParameter("@mEnabled", mEnabled)
                                     };
            int result = factory.ExecuteSQL(mySql, para);
            return result;
        }
        public static int ToEditAssessmentDetail(string mOrganizationID, string mKeyId, string mAssessmentId, string mAssessmentName, string mObjectId, string mObjectName, string mWeightedValue, string mBestValue, string mWorstValue, string mStandardValue, string mStandardScore, string mScoreFactor, string mMaxScore, string mMinScore, string mEnabled, string mId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"UPDATE [dbo].[assessment_AssessmentDetail]
                               SET [AssessmentId] = @mAssessmentId
                                  ,[AssessmentName] = @mAssessmentName
                                  ,[ObjectId] = @mObjectId
                                  ,[ObjectName] =@mObjectName
                                  ,[OrganizationID] =@mOrganizationID
                                  ,[KeyId] = @mKeyId
                                  ,[WeightedValue] = @mWeightedValue
                                  ,[BestValue] = @mBestValue
                                  ,[WorstValue] =@mWorstValue
                                  ,[StandardValue] = @mStandardValue
                                  ,[StandardScore] = @mStandardScore
                                  ,[ScoreFactor] = @mScoreFactor
                                  ,[MaxScore] = @mMaxScore
                                  ,[MinScore] = @mMinScore
                                  ,[Enabled] = @mEnabled
                             WHERE [Id] =@mId";
            SqlParameter[] para = { 
                                    new SqlParameter("@mId", mId) ,
                                    new SqlParameter("@mAssessmentId", mAssessmentId) ,
                                    new SqlParameter("@mAssessmentName", mAssessmentName) ,
                                    new SqlParameter("@mObjectId", mObjectId) ,
                                    new SqlParameter("@mObjectName", mObjectName) ,
                                    new SqlParameter("@mOrganizationID", mOrganizationID) ,
                                    new SqlParameter("@mKeyId", mKeyId) ,                                                                  
                                    new SqlParameter("@mWeightedValue", mWeightedValue) ,
                                    new SqlParameter("@mBestValue", mBestValue) ,
                                    new SqlParameter("@mWorstValue", mWorstValue),

                                    new SqlParameter("@mStandardValue", mStandardValue) ,
                                    new SqlParameter("@mStandardScore", mStandardScore) ,                                                                  
                                    new SqlParameter("@mScoreFactor", mScoreFactor) ,
                                    new SqlParameter("@mMaxScore", mMaxScore) ,
                                    new SqlParameter("@mMinScore", mMinScore),

                                    new SqlParameter("@mEnabled", mEnabled)
                                     };
            int result = factory.ExecuteSQL(mySql, para);
            return result;        
        }
        public static int ToDeleteAssessmentDetail(string mId)
        { 
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
                ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
                string mySql = @"DELETE FROM [dbo].[assessment_AssessmentDetail] 
                                 WHERE [Id] =@mId";
                SqlParameter para =  new SqlParameter("@mId", mId) ;
                int result = factory.ExecuteSQL(mySql, para);
                return result;      
        }
        public static int ToDeleteAssessmentVersion(string mKeyId) {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"
                            delete from  [dbo].[tz_Assessment] where [KeyId]=@mKeyId
                            delete from  [dbo].[assessment_AssessmentDetail] where [KeyId]=@mKeyId";
            SqlParameter para = new SqlParameter("@mKeyId", mKeyId);
            int result = factory.ExecuteSQL(mySql, para);
            return result;           
        }
    }
}
