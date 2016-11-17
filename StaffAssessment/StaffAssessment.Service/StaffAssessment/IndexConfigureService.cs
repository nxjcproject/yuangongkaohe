using SqlServerDataAdapter;
using StaffAssessment.Infrastructure.Configuration;
using EasyUIJsonParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
//using Newtonsoft.Json;
using System.Text.RegularExpressions;


namespace StaffAssessment.Service.StaffAssessment
{
    public class IndexConfigureService
    {
        public static Model_CalculateObjects GetCalculateObjects(string myType, string myValueType, string myOrganizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            return Table_CalculateObjects.GetCalculateObjects(myType, myValueType, myOrganizationId, factory);            
        }
        public static Model_StandardIndexObjects GetIndexDataTable(string myOrganizationId, string myAssessmentId, string myType, string ValueType)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            return Table_StandardIndexObjects.GetStandardIndexObjects(myOrganizationId, myAssessmentId, myType, ValueType,factory);
        }
//        public static void SaveIndexId(string json, string assessmentId, string myName)
//        {

//            //string[] detailJsons = json.JsonPickArray("StandardIndex");
//            //string  mId = json.JsonPick("StandardIndex");
//            string connectionString = ConnectionStringFactory.NXJCConnectionString;
//            //string[] shuju = json.Split('[');
//            string[] shuju = Regex.Split(json, "children", RegexOptions.IgnoreCase);

//            using (TransactionScope tsCope = new TransactionScope())
//            {
//                using (SqlConnection connection = new SqlConnection(connectionString))
//                {
//                    SqlCommand command = connection.CreateCommand();

//                    command.CommandText = @"insert into [assessment_StandardIndex]
//                            (OrganizationID,AssessmentId,ObjectId,StandardIndex,Editor,EditTime) values (@OrganizationID,@assessmentId,@VariableId,@StandardIndex,@myName,@time)";

//                    connection.Open();

//                    foreach (string detail in shuju)
//                    {
//                        if (detail.JsonPick("StandardIndex").Length != 0)
//                        {
//                            string OrganizationID = detail.JsonPick("OrganizationID");
//                            string VariableId = detail.JsonPick("VariableId");
//                            string StandardIndex = detail.JsonPick("StandardIndex");
//                            command.Parameters.Clear();
//                            command.Parameters.Add(new SqlParameter("OrganizationID", OrganizationID));
//                            command.Parameters.Add(new SqlParameter("assessmentId", assessmentId));
//                            command.Parameters.Add(new SqlParameter("VariableId", VariableId));
//                            command.Parameters.Add(new SqlParameter("StandardIndex", StandardIndex));
//                            command.Parameters.Add(new SqlParameter("myName", myName));
//                            command.Parameters.Add(new SqlParameter("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
//                            command.ExecuteNonQuery();
//                        };
//                    }
//                }

//                tsCope.Complete();
//            }
//        }
        public static void SaveIndexId(string json, string assessmentId, string myName)
        {

            //string[] detailJsons = json.JsonPickArray("StandardIndex");
            //string  mId = json.JsonPick("StandardIndex");
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            //string[] shuju = json.Split('[');
            string[] shuju = Regex.Split(json, "children", RegexOptions.IgnoreCase);

            using (TransactionScope tsCope = new TransactionScope())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    foreach (string detail in shuju)
                    {
                        string mOrganizationID = detail.JsonPick("OrganizationID");
                        string mVariableId = detail.JsonPick("VariableId");
                        command.CommandText = @"delete from [assessment_StandardIndex]
                                           where [OrganizationID]=@mOrganizationID
                                                  and [AssessmentId]=@assessmentId
                                                  and [ObjectId]=@mVariableId";
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("mOrganizationID", mOrganizationID));
                        command.Parameters.Add(new SqlParameter("assessmentId", assessmentId));
                        command.Parameters.Add(new SqlParameter("mVariableId", mVariableId));
                        command.ExecuteNonQuery();
                    }
                    //connection.Open();
                    foreach (string detail in shuju)
                    {
                        if (detail.JsonPick("StandardIndex").Length == 0 && detail.JsonPick("OrganizationID").Length !=0)
                        {
                         command.CommandText = @"insert into [assessment_StandardIndex]
                            (OrganizationID,AssessmentId,ObjectId,Editor,EditTime) values (@OrganizationID,@assessmentId,@VariableId,@myName,@time)";
                          
                            string OrganizationID = detail.JsonPick("OrganizationID");
                            string VariableId = detail.JsonPick("VariableId");
                            string StandardIndex = detail.JsonPick("StandardIndex");
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("OrganizationID", OrganizationID));
                            command.Parameters.Add(new SqlParameter("assessmentId", assessmentId));
                            command.Parameters.Add(new SqlParameter("VariableId", VariableId));
                            command.Parameters.Add(new SqlParameter("StandardIndex", StandardIndex));
                            command.Parameters.Add(new SqlParameter("myName", myName));
                            command.Parameters.Add(new SqlParameter("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                            command.ExecuteNonQuery();
                        };
                        if (detail.JsonPick("StandardIndex").Length != 0 && detail.JsonPick("OrganizationID").Length != 0)
                        {
                            command.CommandText = @"insert into [assessment_StandardIndex]
                            (OrganizationID,AssessmentId,ObjectId,StandardIndex,Editor,EditTime) values (@OrganizationID,@assessmentId,@VariableId,@StandardIndex,@myName,@time)"; ;
                            //connection.Open();
                            string OrganizationID = detail.JsonPick("OrganizationID");
                            string VariableId = detail.JsonPick("VariableId");
                            string StandardIndex = detail.JsonPick("StandardIndex");
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("OrganizationID", OrganizationID));
                            command.Parameters.Add(new SqlParameter("assessmentId", assessmentId));
                            command.Parameters.Add(new SqlParameter("VariableId", VariableId));
                            command.Parameters.Add(new SqlParameter("StandardIndex", StandardIndex));
                            command.Parameters.Add(new SqlParameter("myName", myName));
                            command.Parameters.Add(new SqlParameter("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                            command.ExecuteNonQuery();
                        }
                    }
                }

                tsCope.Complete();
            }
        }
    }
}
