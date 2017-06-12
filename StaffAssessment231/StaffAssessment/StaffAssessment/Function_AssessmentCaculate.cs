using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;
namespace StaffAssessment
{
    public class Function_AssessmentCaculate
    {
        private const string Clinker = "clinker";
        private const string Cement = "cementmill";
        public static DataTable GetStaffAssessment(DataTable myWorkingTimeTable, DataTable myAssessmentItemsTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            DataTable m_AssessmentItemsTable = myAssessmentItemsTable.Copy();
            m_AssessmentItemsTable.Columns.Add("AssessmenScore", typeof(decimal));
            m_AssessmentItemsTable.Columns.Add("WeightedAverageCredit", typeof(decimal));
            Dictionary<string, Model_CaculateItems> m_CaculateItems = GetCaculateItems(m_AssessmentItemsTable);      //相同
            if (myWorkingTimeTable != null)
            {
                CaculateItemValue(ref m_CaculateItems, myWorkingTimeTable, myDataFactory);
                foreach (Model_CaculateItems myCaculateItem in m_CaculateItems.Values)
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        string m_StaffAssessmentId = myCaculateItem.CaculateItemDetail[i].Id;
                        DataRow[] m_ResultDataRowTemp = m_AssessmentItemsTable.Select(string.Format("Id = '{0}'", m_StaffAssessmentId));
                        for (int j = 0; j < m_ResultDataRowTemp.Length; j++)
                        {
                            m_ResultDataRowTemp[j]["AssessmenScore"] = myCaculateItem.CaculateItemDetail[i].CaculateValue;
                            m_ResultDataRowTemp[j]["WeightedAverageCredit"] = myCaculateItem.CaculateItemDetail[i].CaculateScore;
                        }
                    }
                }
            }


            // m_AssessmentItemsTable.Rows[i]["WeightedAverageCredit"] = 20.00m;
            return m_AssessmentItemsTable;
        }
        private static Dictionary<string, Model_CaculateItems> GetCaculateItems(DataTable myAssessmentItemsTable)
        {
            Dictionary<string, Model_CaculateItems> m_CaculateItems = new Dictionary<string, Model_CaculateItems>();
            if (myAssessmentItemsTable != null)
            {
                for (int i = 0; i < myAssessmentItemsTable.Rows.Count; i++)
                {
                    DataRow[] m_AssessmentCaculateRows = Table_AssessmentCatalogue.GetAssessmentCatalogue().Select(string.Format("AssessmentId = '{0}'", myAssessmentItemsTable.Rows[i]["AssessmentId"].ToString()));
                    if (m_AssessmentCaculateRows.Length > 0)
                    {
                        string m_KeyIdTemp = myAssessmentItemsTable.Rows[i]["OrganizationID"].ToString() + m_AssessmentCaculateRows[0]["Type"].ToString() + m_AssessmentCaculateRows[0]["ValueType"].ToString();
                        if (!m_CaculateItems.ContainsKey(m_KeyIdTemp))
                        {
                            Model_CaculateItems m_CaculateItemTemp = new Model_CaculateItems();
                            m_CaculateItemTemp.OrganizaitonId = myAssessmentItemsTable.Rows[i]["OrganizationID"].ToString();
                            m_CaculateItemTemp.Type = m_AssessmentCaculateRows[0]["Type"].ToString();
                            m_CaculateItemTemp.ValueType = m_AssessmentCaculateRows[0]["ValueType"].ToString();
                            Model_CaculateItemDetail m_CaculateItemDetailTemp = new Model_CaculateItemDetail();
                            m_CaculateItemDetailTemp.Id = myAssessmentItemsTable.Rows[i]["Id"].ToString();
                            m_CaculateItemDetailTemp.AssessmentId = myAssessmentItemsTable.Rows[i]["AssessmentId"].ToString();
                            m_CaculateItemDetailTemp.ObjectId = myAssessmentItemsTable.Rows[i]["ObjectId"].ToString();
                            m_CaculateItemDetailTemp.WeightedValue = (decimal)myAssessmentItemsTable.Rows[i]["WeightedValue"];
                            m_CaculateItemDetailTemp.BestValue = (decimal)myAssessmentItemsTable.Rows[i]["BestValue"];
                            m_CaculateItemDetailTemp.WorstValue = (decimal)myAssessmentItemsTable.Rows[i]["WorstValue"];
                            m_CaculateItemDetailTemp.StandardValue = (decimal)myAssessmentItemsTable.Rows[i]["StandardValue"];
                            m_CaculateItemDetailTemp.StandardScore = (decimal)myAssessmentItemsTable.Rows[i]["StandardScore"];
                            m_CaculateItemDetailTemp.ScoreFactor = (decimal)myAssessmentItemsTable.Rows[i]["ScoreFactor"];
                            m_CaculateItemDetailTemp.MaxScore = (decimal)myAssessmentItemsTable.Rows[i]["MaxScore"];
                            m_CaculateItemDetailTemp.MinScore = (decimal)myAssessmentItemsTable.Rows[i]["MinScore"];

                            m_CaculateItemTemp.CaculateItemDetail.Add(m_CaculateItemDetailTemp);
                            m_CaculateItems.Add(m_KeyIdTemp, m_CaculateItemTemp);
                        }
                        else
                        {
                            Model_CaculateItemDetail m_CaculateItemDetailTemp = new Model_CaculateItemDetail();
                            m_CaculateItemDetailTemp.Id = myAssessmentItemsTable.Rows[i]["Id"].ToString();
                            m_CaculateItemDetailTemp.AssessmentId = myAssessmentItemsTable.Rows[i]["AssessmentId"].ToString();
                            m_CaculateItemDetailTemp.ObjectId = myAssessmentItemsTable.Rows[i]["ObjectId"].ToString();
                            m_CaculateItemDetailTemp.WeightedValue = (decimal)myAssessmentItemsTable.Rows[i]["WeightedValue"];
                            m_CaculateItemDetailTemp.BestValue = (decimal)myAssessmentItemsTable.Rows[i]["BestValue"];
                            m_CaculateItemDetailTemp.WorstValue = (decimal)myAssessmentItemsTable.Rows[i]["WorstValue"];
                            m_CaculateItemDetailTemp.StandardValue = (decimal)myAssessmentItemsTable.Rows[i]["StandardValue"];
                            m_CaculateItemDetailTemp.StandardScore = (decimal)myAssessmentItemsTable.Rows[i]["StandardScore"];
                            m_CaculateItemDetailTemp.ScoreFactor = (decimal)myAssessmentItemsTable.Rows[i]["ScoreFactor"];
                            m_CaculateItemDetailTemp.MaxScore = (decimal)myAssessmentItemsTable.Rows[i]["MaxScore"];
                            m_CaculateItemDetailTemp.MinScore = (decimal)myAssessmentItemsTable.Rows[i]["MinScore"];

                            m_CaculateItems[m_KeyIdTemp].CaculateItemDetail.Add(m_CaculateItemDetailTemp);
                        }
                    }


                }
            }
            return m_CaculateItems;
        }
        public static void CaculateItemValue(ref Dictionary<string, Model_CaculateItems> m_CaculateItems, DataTable myWorkingTimeTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            DataTable m_DataBaseTable = GetFactoryDataBase(myDataFactory);
            DataTable m_OrganizationLevelCodeTable = GetOrganizationLevelCode(myDataFactory);
            foreach (Model_CaculateItems myCaculateItem in m_CaculateItems.Values)
            {
                Model_CaculateItems m_CaculateItemTemp = myCaculateItem;
                if (myCaculateItem.Type == "MaterialWeight" && myCaculateItem.ValueType == "MaterialWeight")  //计算产量
                {
                    GetMaterialWeightValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, myDataFactory);
                }
                else if (myCaculateItem.Type == "Energy" && (myCaculateItem.ValueType == "ElectricityQuantity"
                    || myCaculateItem.ValueType == "ElectricityConsumption" || myCaculateItem.ValueType == "CoalConsumption"))  //计算电量
                {
                    GetNormalElectricityValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, myDataFactory);
                }
                else if (myCaculateItem.Type == "Comprehensive" && (myCaculateItem.ValueType == "ComprehensivePowerConsumption"

                    || myCaculateItem.ValueType == "ComprehensiveCoalConsumption" || myCaculateItem.ValueType == "ComprehensiveEnergyConsumption"
                    || myCaculateItem.ValueType == "ComprehensivePowerConsumptionComparable" || myCaculateItem.ValueType == "ComprehensiveCoalConsumptionComparable"
                    || myCaculateItem.ValueType == "ComprehensiveEnergyConsumptionComparable"))  //计算综合电耗
                {
                    GetComprehensiveConsumptionValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, m_OrganizationLevelCodeTable, myDataFactory);
                }
                else if (myCaculateItem.Type == "Energy" && (myCaculateItem.ValueType == "ElectricityConsumptionAlarmCount"
                    || myCaculateItem.ValueType == "CoalConsumptionAlarmCount" || myCaculateItem.ValueType == "PowAlarmCount")) //计算电耗报警
                {
                    GetAlarmCountValue(ref m_CaculateItemTemp, myWorkingTimeTable, myDataFactory);
                }
                else if (myCaculateItem.Type == "Production" && (myCaculateItem.ValueType == "ElectricalDowntimeCount"
                    || myCaculateItem.ValueType == "MechanicalDowntimeCount" || myCaculateItem.ValueType == "ProcessDowntimeCount"))  //计算故障停机次数
                {
                    GetMachineHaltCountValue(ref m_CaculateItemTemp, myWorkingTimeTable, myDataFactory);
                }
                else if (myCaculateItem.Type == "Production" && (myCaculateItem.ValueType == "ElectricalDowntimeTime"
                    || myCaculateItem.ValueType == "MechanicalDowntimeTime" || myCaculateItem.ValueType == "ProcessDowntimeTime"))  //计算故障停机次数
                {
                    GetMachineHaltTimeValue(ref m_CaculateItemTemp, myWorkingTimeTable, myDataFactory);
                }
                else if (myCaculateItem.Type == "Production" && (myCaculateItem.ValueType == "RunningRate"
                    || myCaculateItem.ValueType == "FaultRate" || myCaculateItem.ValueType == "OutputPerHour"))  //计算运转率,运转时间,台时产量
                {
                    GetMachineRunIndicatorsValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, myDataFactory);
                }
                else if (myCaculateItem.Type == "DCSTag")         //提取DCS记录
                {
                    if (myCaculateItem.ValueType == "DCSTagAvg")
                    {
                        GetDCSTagsValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, "avg", myDataFactory);
                    }
                    else if (myCaculateItem.ValueType == "DCSTagSum")
                    {
                        GetDCSTagsValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, "sum", myDataFactory);
                    }
                    else if (myCaculateItem.ValueType == "DCSTagCount")
                    {
                        GetDCSTagsValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, "count", myDataFactory);
                    }
                }
                else if (myCaculateItem.Type == "CurrentTag")
                {
                    if (myCaculateItem.ValueType == "CurrentTagAvg")
                    {
                        GetCurrentTagsValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, "avg", myDataFactory);
                    }
                    else if (myCaculateItem.ValueType == "CurrentTagSum")
                    {
                        GetCurrentTagsValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, "sum", myDataFactory);
                    }
                    else if (myCaculateItem.ValueType == "CurrentTagCount")
                    {
                        GetCurrentTagsValue(ref m_CaculateItemTemp, myWorkingTimeTable, m_DataBaseTable, "count", myDataFactory);
                    }
                }
                for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)      //返回值赋值给参数变量
                {
                    myCaculateItem.CaculateItemDetail[i].CaculateValue = m_CaculateItemTemp.CaculateItemDetail[i].CaculateValue;
                    if (m_CaculateItemTemp.CaculateItemDetail[i].StandardValue != 0)
                    {
                        myCaculateItem.CaculateItemDetail[i].CaculateScore = m_CaculateItemTemp.CaculateItemDetail[i].WeightedValue + m_CaculateItemTemp.CaculateItemDetail[i].ScoreFactor * (m_CaculateItemTemp.CaculateItemDetail[i].CaculateValue - m_CaculateItemTemp.CaculateItemDetail[i].StandardValue) / m_CaculateItemTemp.CaculateItemDetail[i].StandardValue;
                    }
                    else
                    {
                        myCaculateItem.CaculateItemDetail[i].CaculateScore = myCaculateItem.CaculateItemDetail[i].CaculateValue;
                    }
                }
            }
        }
        private static DataTable GetFactoryDataBase(SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT A.OrganizationID
                                  ,A.DatabaseID
                                  ,A.Name
	                              ,rtrim(B.MeterDatabase) as MeterDatabase
                                  ,rtrim(B.DCSProcessDatabase) as DCSProcessDatabase
                              FROM system_Organization A, system_Database B
                              where A.Enabled = 1
                              and A.DatabaseID = B.DatabaseID";
            try
            {
                DataTable m_FcatoryDataBase = myDataFactory.Query(m_Sql);
                return m_FcatoryDataBase;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetOrganizationLevelCode(SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT A.OrganizationID
                                  ,A.LevelCode
                                  ,A.Type
                                  ,A.LevelType
                                  ,A.Name
                              FROM system_Organization A
                              where A.Enabled = 1";
            try
            {
                DataTable m_FcatoryDataBase = myDataFactory.Query(m_Sql);
                return m_FcatoryDataBase;
            }
            catch
            {
                return null;
            }
        }
        private static void GetMaterialWeightValue(ref Model_CaculateItems myCaculateItem, DataTable myWorkingTimeTable, DataTable myDataBaseTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            if (myWorkingTimeTable.Rows.Count > 0)
            {
                string m_WorkingTimeStringTemplate = "(vDate >= '{0}' and vDate <= '{1}')";
                string m_WorkingTimeString = "";
                for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                string m_MaterialVariableIdString = GetMaterialVariableId(myCaculateItem, myDataFactory);
                if (m_WorkingTimeString != "" && m_MaterialVariableIdString != "")
                {
                    DataRow[] m_FactoryDataBaseName = myDataBaseTable.Select(string.Format("OrganizationID = '{0}'", myCaculateItem.OrganizaitonId));
                    if (m_FactoryDataBaseName.Length > 0)
                    {
                        DataTable m_MaterialValueTable = GetMaterialValue(m_WorkingTimeString, m_MaterialVariableIdString, m_FactoryDataBaseName[0]["MeterDatabase"].ToString(), myDataFactory);
                        if (m_MaterialValueTable != null && m_MaterialValueTable.Rows.Count == 1)
                        {
                            for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                            {
                                decimal m_MaterialValueTemp = m_MaterialValueTable.Rows[0][myCaculateItem.CaculateItemDetail[i].ObjectId] != DBNull.Value ? decimal.Parse(m_MaterialValueTable.Rows[0][myCaculateItem.CaculateItemDetail[i].ObjectId].ToString()) : 0.0m;
                                myCaculateItem.CaculateItemDetail[i].CaculateValue = m_MaterialValueTemp;
                            }
                        }
                    }
                }
            }
        }
        private static void GetNormalElectricityValue(ref Model_CaculateItems myCaculateItem, DataTable myWorkingTimeTable, DataTable myDataBaseTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_WorkingTimeStringTemplate = "(vDate >= '{0}' and vDate <= '{1}')";
            string m_WorkingTimeString = "";
            for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            string m_VariableIdString = "";
            for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
            {
                if (i == 0)
                {
                    m_VariableIdString = "'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
                else
                {
                    m_VariableIdString = m_VariableIdString + ",'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
            }
            DataRow[] m_FactoryDataBaseName = myDataBaseTable.Select(string.Format("OrganizationID = '{0}'", myCaculateItem.OrganizaitonId));

            if (m_WorkingTimeString != "" && m_VariableIdString != "" && m_FactoryDataBaseName.Length > 0)
            {
                DataTable m_ResultValueTable = GetElectricityValue(myCaculateItem.OrganizaitonId, m_WorkingTimeString, m_VariableIdString, m_FactoryDataBaseName[0]["MeterDatabase"].ToString(), myDataFactory);
                if (m_ResultValueTable != null && m_ResultValueTable.Rows.Count > 0)
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultRow = m_ResultValueTable.Select(string.Format("OrganizationID = '{0}' and VariableID = '{1}'", myCaculateItem.OrganizaitonId, myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultRow.Length > 0)
                        {
                            if (myCaculateItem.ValueType == "ElectricityQuantity")
                            {
                                decimal m_ElectricityValueTemp = m_ResultRow[0]["FormulaValue"] != DBNull.Value ? decimal.Parse(m_ResultRow[0]["FormulaValue"].ToString()) : 0.0m;
                                myCaculateItem.CaculateItemDetail[i].CaculateValue = m_ElectricityValueTemp;
                            }
                            else if (myCaculateItem.ValueType == "ElectricityConsumption")
                            {
                                decimal m_ElectricityValueTemp = m_ResultRow[0]["FormulaValue"] != DBNull.Value ? decimal.Parse(m_ResultRow[0]["FormulaValue"].ToString()) : 0.0m;
                                decimal m_MaterialWeightTemp = m_ResultRow[0]["DenominatorValue"] != DBNull.Value ? decimal.Parse(m_ResultRow[0]["DenominatorValue"].ToString()) : 0.0m;

                                myCaculateItem.CaculateItemDetail[i].CaculateValue = m_MaterialWeightTemp > 0 ? m_ElectricityValueTemp / m_MaterialWeightTemp : 0.0m;
                            }
                            else if (myCaculateItem.ValueType == "CoalConsumption")
                            {
                                decimal m_MaterialWeightTemp = m_ResultRow[0]["DenominatorValue"] != DBNull.Value ? decimal.Parse(m_ResultRow[0]["DenominatorValue"].ToString()) : 0.0m;
                                decimal m_CoalWeightTemp = m_ResultRow[0]["CoalDustConsumption"] != DBNull.Value ? decimal.Parse(m_ResultRow[0]["CoalDustConsumption"].ToString()) : 0.0m;

                                myCaculateItem.CaculateItemDetail[i].CaculateValue = m_MaterialWeightTemp > 0 ? 1000 * m_CoalWeightTemp / m_MaterialWeightTemp : 0.0m;   //煤粉单位转换,由吨变为千克
                            }
                        }
                    }
                }
            }
        }
        private static void GetComprehensiveConsumptionValue(ref Model_CaculateItems myCaculateItem, DataTable myWorkingTimeTable, DataTable myDataBaseTable, DataTable myOrganizationLevelCodeTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_WorkingTimeStringTemplate = "(vDate >= '{0}' and vDate <= '{1}')";
            string m_WorkingTimeString = "";
            for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }

            DataRow[] m_FactoryDataBaseName = myDataBaseTable.Select(string.Format("OrganizationID = '{0}'", myCaculateItem.OrganizaitonId));
            DataRow[] m_OrganizationLevelCode = myOrganizationLevelCodeTable.Select(string.Format("OrganizationID = '{0}'", myCaculateItem.OrganizaitonId));
            if (m_WorkingTimeString != "" && m_FactoryDataBaseName.Length > 0 && m_OrganizationLevelCode.Length > 0)
            {
                string m_DataBaseName = m_FactoryDataBaseName[0]["MeterDatabase"].ToString();
                string m_LevelCode = m_OrganizationLevelCode[0]["LevelCode"].ToString();
                AutoSetParameters.AutoGetEnergyConsumptionByBasicData_V1 m_AutoGetEnergyConsumptionByBasicData_V1 = new AutoSetParameters.AutoGetEnergyConsumptionByBasicData_V1(myDataFactory);
                for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                {
                    if (myCaculateItem.ValueType == "ComprehensivePowerConsumption")
                    {
                        decimal m_ElectricityValueTemp = 0.0m;
                        if (myCaculateItem.CaculateItemDetail[i].ObjectId == Clinker)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetClinkerPowerConsumptionWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }
                        else if (myCaculateItem.CaculateItemDetail[i].ObjectId == Cement)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetCementPowerConsumptionWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }

                        myCaculateItem.CaculateItemDetail[i].CaculateValue = m_ElectricityValueTemp;
                    }
                    else if (myCaculateItem.ValueType == "ComprehensiveCoalConsumption")
                    {
                        decimal m_ElectricityValueTemp = 0.0m;
                        if (myCaculateItem.CaculateItemDetail[i].ObjectId == Clinker)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetClinkerCoalConsumptionWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }
                        else if (myCaculateItem.CaculateItemDetail[i].ObjectId == Cement)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetCementCoalConsumptionWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }

                        myCaculateItem.CaculateItemDetail[i].CaculateValue = m_ElectricityValueTemp;
                    }
                    else if (myCaculateItem.ValueType == "ComprehensiveEnergyConsumption")
                    {
                        decimal m_ElectricityValueTemp = 0.0m;
                        if (myCaculateItem.CaculateItemDetail[i].ObjectId == Clinker)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetClinkerEnergyConsumptionWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }
                        else if (myCaculateItem.CaculateItemDetail[i].ObjectId == Cement)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetCementEnergyConsumptionWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }

                        myCaculateItem.CaculateItemDetail[i].CaculateValue = m_ElectricityValueTemp;
                    }
                    else if (myCaculateItem.ValueType == "ComprehensivePowerConsumptionComparable")
                    {
                        decimal m_ElectricityValueTemp = 0.0m;
                        if (myCaculateItem.CaculateItemDetail[i].ObjectId == Clinker)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetClinkerPowerConsumptionComparableWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }
                        else if (myCaculateItem.CaculateItemDetail[i].ObjectId == Cement)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetCementPowerConsumptionComparableWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }

                        myCaculateItem.CaculateItemDetail[i].CaculateValue = m_ElectricityValueTemp;
                    }
                    else if (myCaculateItem.ValueType == "ComprehensiveCoalConsumptionComparable")
                    {
                        decimal m_ElectricityValueTemp = 0.0m;
                        if (myCaculateItem.CaculateItemDetail[i].ObjectId == Clinker)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetClinkerCoalConsumptionComparableWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }
                        else if (myCaculateItem.CaculateItemDetail[i].ObjectId == Cement)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetCementCoalConsumptionComparableWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }

                        myCaculateItem.CaculateItemDetail[i].CaculateValue = m_ElectricityValueTemp;
                    }
                    else if (myCaculateItem.ValueType == "ComprehensiveEnergyConsumptionComparable")
                    {
                        decimal m_ElectricityValueTemp = 0.0m;
                        if (myCaculateItem.CaculateItemDetail[i].ObjectId == Clinker)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetClinkerEnergyConsumptionComparableWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }
                        else if (myCaculateItem.CaculateItemDetail[i].ObjectId == Cement)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_Value = m_AutoGetEnergyConsumptionByBasicData_V1.GetCementEnergyConsumptionComparableWithFormula(m_DataBaseName, "day", myWorkingTimeTable, m_LevelCode);
                            m_ElectricityValueTemp = m_Value.CaculateValue;
                        }

                        myCaculateItem.CaculateItemDetail[i].CaculateValue = m_ElectricityValueTemp;
                    }
                }
            }
        }
        private static void GetAlarmCountValue(ref Model_CaculateItems myCaculateItem, DataTable myWorkingTimeTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_WorkingTimeStringTemplate = "(A.StartTime >= '{0}' and A.StartTime <= '{1}')";
            string m_WorkingTimeString = "";
            for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            string m_VariableIdString = "";
            for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
            {
                if (i == 0)
                {
                    m_VariableIdString = "'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
                else
                {
                    m_VariableIdString = m_VariableIdString + ",'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
            }
            DataTable m_AlarmCountValueTable = GetAlarmCount(m_WorkingTimeString, m_VariableIdString, myCaculateItem.OrganizaitonId, myDataFactory);
            if (m_AlarmCountValueTable != null)
            {
                if (myCaculateItem.Type == "Energy" && myCaculateItem.ValueType == "ElectricityConsumptionAlarmCount")  //电耗报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_AlarmCountValueTable.Select(string.Format("VariableId = '{0}' and EnergyConsumptionType = '电耗超标'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
                else if (myCaculateItem.Type == "Energy" && myCaculateItem.ValueType == "CoalConsumptionAlarmCount")  //煤耗报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_AlarmCountValueTable.Select(string.Format("VariableId = '{0}' and EnergyConsumptionType = '煤耗超标'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
                else if (myCaculateItem.Type == "Energy" && myCaculateItem.ValueType == "PowAlarmCount")  //功率报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_AlarmCountValueTable.Select(string.Format("VariableId = '{0}' and EnergyConsumptionType = '功率超标'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
            }
        }
        private static void GetMachineHaltCountValue(ref Model_CaculateItems myCaculateItem, DataTable myWorkingTimeTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_WorkingTimeStringTemplate = "(A.HaltTime >= '{0}' and A.HaltTime <= '{1}')";
            string m_WorkingTimeString = "";
            for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            string m_VariableIdString = "";
            for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
            {
                if (i == 0)
                {
                    m_VariableIdString = "'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
                else
                {
                    m_VariableIdString = m_VariableIdString + ",'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
            }
            DataTable m_MachinHaltCountValueTable = GetMachinHaltCount(m_WorkingTimeString, m_VariableIdString, myCaculateItem.OrganizaitonId, myDataFactory);
            if (m_MachinHaltCountValueTable != null)
            {
                if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "ElectricalDowntimeCount")  //电气故障报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_MachinHaltCountValueTable.Select(string.Format("VariableId = '{0}' and ReasonStatisticsTypeId = 'ElectricalDowntime'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
                else if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "MechanicalDowntimeCount")  //机械故障报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_MachinHaltCountValueTable.Select(string.Format("VariableId = '{0}' and ReasonStatisticsTypeId = 'MechanicalDowntime'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
                else if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "ProcessDowntimeCount")  //工艺故障报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_MachinHaltCountValueTable.Select(string.Format("VariableId = '{0}' and ReasonStatisticsTypeId = 'ProcessDowntime'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
            }
        }
        private static void GetMachineHaltTimeValue(ref Model_CaculateItems myCaculateItem, DataTable myWorkingTimeTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_WorkingTimeStringTemplate = "(A.HaltTime >= '{0}' and A.HaltTime <= '{1}')";
            string m_WorkingTimeString = "";
            for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            string m_VariableIdString = "";
            for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
            {
                if (i == 0)
                {
                    m_VariableIdString = "'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
                else
                {
                    m_VariableIdString = m_VariableIdString + ",'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
            }
            DataTable m_MachinHaltTimeValueTable = GetMachinHaltTime(m_WorkingTimeString, m_VariableIdString, myCaculateItem.OrganizaitonId, myDataFactory);
            if (m_MachinHaltTimeValueTable != null)
            {
                if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "ElectricalDowntimeTime")  //电气故障报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_MachinHaltTimeValueTable.Select(string.Format("VariableId = '{0}' and ReasonStatisticsTypeId = 'ElectricalDowntime'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
                else if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "MechanicalDowntimeTime")  //机械故障报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_MachinHaltTimeValueTable.Select(string.Format("VariableId = '{0}' and ReasonStatisticsTypeId = 'MechanicalDowntime'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
                else if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "ProcessDowntimeTime")  //工艺故障报警
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_ResultDataRowTemp = m_MachinHaltTimeValueTable.Select(string.Format("VariableId = '{0}' and ReasonStatisticsTypeId = 'ProcessDowntime'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_ResultDataRowTemp.Length > 0)
                        {
                            myCaculateItem.CaculateItemDetail[i].CaculateValue = decimal.Parse(m_ResultDataRowTemp[0]["Value"].ToString());
                        }
                    }
                }
            }
        }
        private static void GetMachineRunIndicatorsValue(ref Model_CaculateItems myCaculateItem, DataTable myWorkingTimeTable, DataTable myDataBaseTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {

            string m_VariableIdString = "";
            for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
            {
                if (i == 0)
                {
                    m_VariableIdString = "'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
                else
                {
                    m_VariableIdString = m_VariableIdString + ",'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
            }
            string m_WorkingTimeStringTemplate = "(vDate >= '{0}' and vDate <= '{1}')";
            string m_WorkingTimeString = "";
            for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            DataRow[] m_FactoryDataBaseName = myDataBaseTable.Select(string.Format("OrganizationID = '{0}'", myCaculateItem.OrganizaitonId));
            string m_DataBaseName = m_FactoryDataBaseName[0]["MeterDatabase"].ToString();

            DataTable m_MachineHaltTimeTable = GetMachineHaltTimeForRunIndicator(myWorkingTimeTable, m_VariableIdString, myCaculateItem.OrganizaitonId, myDataFactory);
            if (m_MachineHaltTimeTable != null)
            {
                if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "RunningRate")  //运转率
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_MachineHaltRowsTemp = m_MachineHaltTimeTable.Select(string.Format("VariableId = '{0}'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_MachineHaltRowsTemp.Length > 0)
                        {
                            decimal m_CalendarTime = GetSumTime(myWorkingTimeTable, "StartTime", "EndTime", "");
                            if (m_CalendarTime > 0)
                            {
                                DataTable m_MachineHaltTableTemp = m_MachineHaltRowsTemp.CopyToDataTable();
                                decimal m_RepairTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId = 'RepairTime'");
                                decimal m_NormalStopTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId = 'NormalStopTime'");
                                decimal m_DownTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId in ('ElectricalDowntime','MechanicalDowntime','ProcessDowntime') or ReasonId is null or ReasonId = ''");

                                myCaculateItem.CaculateItemDetail[i].CaculateValue = (m_CalendarTime - m_RepairTime - m_NormalStopTime - m_DownTime) / m_CalendarTime;
                            }
                        }
                    }
                }
                else if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "FaultRate")  //故障率
                {
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_MachineHaltRowsTemp = m_MachineHaltTimeTable.Select(string.Format("VariableId = '{0}'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_MachineHaltRowsTemp.Length > 0)
                        {
                            decimal m_CalendarTime = GetSumTime(myWorkingTimeTable, "StartTime", "EndTime", "");
                            if (m_CalendarTime > 0)
                            {
                                DataTable m_MachineHaltTableTemp = m_MachineHaltRowsTemp.CopyToDataTable();
                                decimal m_RepairTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId = 'RepairTime'");
                                decimal m_NormalStopTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId = 'NormalStopTime'");
                                decimal m_DownTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId in ('ElectricalDowntime','MechanicalDowntime','ProcessDowntime') or ReasonId is null or ReasonId = ''");
                                decimal m_EnvironmentTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId = 'EnvironmentTime'");
                                decimal m_Denominator = m_CalendarTime - m_RepairTime - m_NormalStopTime - m_EnvironmentTime;
                                if (m_Denominator > 0)
                                {
                                    myCaculateItem.CaculateItemDetail[i].CaculateValue = m_DownTime / m_Denominator;
                                }
                            }
                        }
                    }
                }
                else if (myCaculateItem.Type == "Production" && myCaculateItem.ValueType == "OutputPerHour")  //台时产量
                {
                    DataTable m_MaterialWeight = GetMaterialWeightForRunIndicator(m_WorkingTimeString, m_VariableIdString, m_DataBaseName, myDataFactory);
                    for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
                    {
                        DataRow[] m_MachineHaltRowsTemp = m_MachineHaltTimeTable.Select(string.Format("VariableId = '{0}'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        DataRow[] m_MachineOutputTemp = m_MaterialWeight.Select(string.Format("VariableId = '{0}'", myCaculateItem.CaculateItemDetail[i].ObjectId));
                        if (m_MachineHaltRowsTemp.Length > 0 && m_MachineOutputTemp.Length > 0)
                        {
                            decimal m_CalendarTime = GetSumTime(myWorkingTimeTable, "StartTime", "EndTime", "");
                            if (m_CalendarTime > 0)
                            {
                                DataTable m_MachineHaltTableTemp = m_MachineHaltRowsTemp.CopyToDataTable();
                                decimal m_RepairTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId = 'RepairTime'");
                                decimal m_NormalStopTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId = 'NormalStopTime'");
                                decimal m_DownTime = GetIntersectionSumTime(m_MachineHaltTableTemp, myWorkingTimeTable, "HaltTime", "RecoverTime", "StartTime", "EndTime", "ReasonStatisticsTypeId in ('ElectricalDowntime','MechanicalDowntime','ProcessDowntime') or ReasonId is null or ReasonId = ''");
                                decimal m_RunTime = m_CalendarTime - m_RepairTime - m_NormalStopTime - m_DownTime;
                                if (m_RunTime > 0)
                                {
                                    decimal m_MachineOutputValueTemp = m_MachineOutputTemp[0]["Value"] != DBNull.Value ? decimal.Parse(m_MachineOutputTemp[0]["Value"].ToString()) : 0.0m;
                                    myCaculateItem.CaculateItemDetail[i].CaculateValue = m_MachineOutputValueTemp / m_RunTime;
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void GetDCSTagsValue(ref Model_CaculateItems myCaculateItemTemp, DataTable myWorkingTimeTable, DataTable myDataBaseTable, string myCaculateType, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_OrganizationID = myCaculateItemTemp.OrganizaitonId;
            DataRow[] m_FactoryDataBaseIDRow = myDataBaseTable.Select(string.Format("OrganizationID = '{0}'", m_OrganizationID));
            List<string> m_FactoryDataBaseID = new List<string>();
            for (int i = 0; i < m_FactoryDataBaseIDRow.Length; i++)
            {
                m_FactoryDataBaseID.Add(m_FactoryDataBaseIDRow[i]["DCSProcessDatabase"].ToString());
            }
            if (m_FactoryDataBaseID.Count > 0)
            {
                List<string> m_TagIdList = new List<string>();
                for (int i = 0; i < myCaculateItemTemp.CaculateItemDetail.Count; i++)
                {
                    m_TagIdList.Add(myCaculateItemTemp.CaculateItemDetail[i].ObjectId);
                }

                DataTable m_ContrastTable = GetTagDataContrast(m_TagIdList.ToArray(), m_FactoryDataBaseID.ToArray(), myDataFactory);
                DataTable m_TagDataValueTable = GetTagDataValue(m_ContrastTable, myWorkingTimeTable, myCaculateType, myDataFactory);
                if (m_TagDataValueTable != null && m_TagDataValueTable.Rows.Count > 0)
                {
                    for (int i = 0; i < myCaculateItemTemp.CaculateItemDetail.Count; i++)
                    {
                        string m_ColumnName = myCaculateItemTemp.CaculateItemDetail[i].ObjectId;
                        if (m_TagDataValueTable.Columns.Contains(m_ColumnName))
                        {
                            try
                            {
                                myCaculateItemTemp.CaculateItemDetail[i].CaculateValue = m_TagDataValueTable.Rows[0][m_ColumnName] != DBNull.Value ? (decimal)m_TagDataValueTable.Rows[0][m_ColumnName] : 0.00m;
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

        }

        private static void GetCurrentTagsValue(ref Model_CaculateItems myCaculateItemTemp, DataTable myWorkingTimeTable, DataTable myDataBaseTable, string myCaculateType, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_OrganizationID = myCaculateItemTemp.OrganizaitonId;
            DataRow[] m_FactoryDataBaseIDRow = myDataBaseTable.Select(string.Format("OrganizationID = '{0}'", m_OrganizationID));
            List<string> m_FactoryDataBaseID = new List<string>();
            for (int i = 0; i < m_FactoryDataBaseIDRow.Length; i++)
            {
                m_FactoryDataBaseID.Add(m_FactoryDataBaseIDRow[i]["MeterDatabase"].ToString());
            }
            if (m_FactoryDataBaseID.Count > 0)
            {
                List<string> m_TagIdList = new List<string>();
                for (int i = 0; i < myCaculateItemTemp.CaculateItemDetail.Count; i++)
                {
                    m_TagIdList.Add(myCaculateItemTemp.CaculateItemDetail[i].ObjectId);
                }

                DataTable m_CurrentDataValueTable = GetCurrentDataValue(m_FactoryDataBaseID.ToArray(), m_TagIdList.ToArray(), myWorkingTimeTable, myCaculateType, myDataFactory);
                if (m_CurrentDataValueTable != null && m_CurrentDataValueTable.Rows.Count > 0)
                {
                    for (int i = 0; i < myCaculateItemTemp.CaculateItemDetail.Count; i++)
                    {
                        string m_ColumnName = myCaculateItemTemp.CaculateItemDetail[i].ObjectId;
                        if (m_CurrentDataValueTable.Columns.Contains(m_ColumnName))
                        {
                            try
                            {
                                myCaculateItemTemp.CaculateItemDetail[i].CaculateValue = m_CurrentDataValueTable.Rows[0][m_ColumnName] != DBNull.Value ? (decimal)m_CurrentDataValueTable.Rows[0][m_ColumnName] : 0.00m;
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }
        ///////////////////////////////////////////
        /// <summary>
        /// ///////////////////////////////////////
        /// </summary>
        /// <param name="myCaculateItem"></param>
        /// <param name="myDataFactory"></param>
        /// <returns></returns>
        private static string GetMaterialVariableId(Model_CaculateItems myCaculateItem, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_ReturnString = "";
            string m_Sql = @"SELECT A.OrganizationID
                                ,B.VariableId
                                ,B.Name
                                ,B.Type
                                ,B.TagTableName
                                ,B.Formula
                            FROM tz_Material A, material_MaterialDetail B
                                where A.Enable = 1
                                and A.State= 0
                                and A.OrganizationID = '{0}'
                                and A.KeyId = B.KeyID
                                and B.Visible = 1
                                and B.VariableId in ({1})
                                order by B.VariableId";

            string m_VariableId = "";
            for (int i = 0; i < myCaculateItem.CaculateItemDetail.Count; i++)
            {
                if (i == 0)
                {
                    m_VariableId = "'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
                else
                {
                    m_VariableId = m_VariableId + ",'" + myCaculateItem.CaculateItemDetail[i].ObjectId + "'";
                }
            }
            if (m_VariableId != "")
            {
                m_Sql = string.Format(m_Sql, myCaculateItem.OrganizaitonId, m_VariableId);
                try
                {
                    DataTable m_MaterialVariableIdTable = myDataFactory.Query(m_Sql);
                    if (m_MaterialVariableIdTable != null)
                    {
                        Dictionary<string, string> m_MaterialVariableIdTempe = new Dictionary<string, string>();
                        for (int i = 0; i < m_MaterialVariableIdTable.Rows.Count; i++)
                        {
                            if (!m_MaterialVariableIdTempe.ContainsKey(m_MaterialVariableIdTable.Rows[i]["VariableId"].ToString()))
                            {
                                m_MaterialVariableIdTempe.Add(m_MaterialVariableIdTable.Rows[i]["VariableId"].ToString(), m_MaterialVariableIdTable.Rows[i]["Formula"].ToString());
                            }
                            else
                            {
                                m_MaterialVariableIdTempe[m_MaterialVariableIdTable.Rows[i]["VariableId"].ToString()] = m_MaterialVariableIdTempe[m_MaterialVariableIdTable.Rows[i]["VariableId"].ToString()] + " + " + m_MaterialVariableIdTable.Rows[i]["Formula"].ToString();
                            }
                        }
                        foreach (string myVariableItems in m_MaterialVariableIdTempe.Keys)
                        {
                            if (m_ReturnString == "")
                            {
                                m_ReturnString = "sum(" + m_MaterialVariableIdTempe[myVariableItems] + ") as " + myVariableItems;
                            }
                            else
                            {
                                m_ReturnString = m_ReturnString + ", sum(" + m_MaterialVariableIdTempe[myVariableItems] + ") as " + myVariableItems;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else
            {
            }
            return m_ReturnString;
        }
        private static DataTable GetMaterialValue(string myWorkingTimeString, string myMaterialVariableIdString, string myDataBaseName, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"Select {0} from {1}.dbo.HistoryDCSIncrement where {2}";
            m_Sql = string.Format(m_Sql, myMaterialVariableIdString, myDataBaseName, myWorkingTimeString);
            try
            {
                DataTable m_MaterialValueTable = myDataFactory.Query(m_Sql);
                return m_MaterialValueTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetElectricityValue(string myOrganizationId, string myWorkingTimeString, string myVariableIdString, string myDataBaseName, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT OrganizationID
                                  ,VariableID
                                  ,sum(FormulaValue) as FormulaValue
                                  ,sum(DenominatorValue) as DenominatorValue
                                  ,sum(CoalDustConsumption) as CoalDustConsumption
                              FROM {1}.dbo.HistoryFormulaValue
                              where OrganizationID = '{0}'
                              and ({2})
                              and VariableID in ({3})
                              group by OrganizationID, VariableID
                              union all
                              SELECT OrganizationID
                                  ,VariableID
                                  ,sum(FormulaValue) as FormulaValue
                                  ,sum(DenominatorValue) as DenominatorValue
                                  ,sum(CoalDustConsumption) as CoalDustConsumption
                              FROM {1}.dbo.HistoryMainMachineFormulaValue
                              where OrganizationID = '{0}'
                              and ({2})
                              and VariableID in ({3})
                              group by OrganizationID, VariableID";
            m_Sql = string.Format(m_Sql, myOrganizationId, myDataBaseName, myWorkingTimeString, myVariableIdString);
            try
            {
                DataTable m_ElectricityValueTable = myDataFactory.Query(m_Sql);
                return m_ElectricityValueTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetAlarmCount(string myWorkingTimeString, string myVariableIdString, string myOrganizationId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT A.VariableID as VariableId
                                ,A.EnergyConsumptionType
	                            ,count(0) as Value
                            FROM shift_EnergyConsumptionAlarmLog A, system_Organization B, system_Organization C
                            where A.VariableID in ({1})
                            and A.OrganizationID = B.OrganizationID
                            and B.LevelCode like C.LevelCode + '%'
                            and C.OrganizationID = '{0}'
                            and ({2})
                            group by A.VariableID, A.EnergyConsumptionType";
            m_Sql = string.Format(m_Sql, myOrganizationId, myVariableIdString, myWorkingTimeString);
            try
            {
                DataTable m_AlarmCountTable = myDataFactory.Query(m_Sql);
                return m_AlarmCountTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetMachinHaltCount(string myWorkingTimeString, string myVariableIdString, string myOrganizaitonId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT A.EquipmentID as VariableId
                                   ,B.ReasonStatisticsTypeId
	                               ,count(0) as Value
                              FROM shift_MachineHaltLog A, system_MachineHaltReason B, system_Organization C, system_Organization D
                              where A.EquipmentID in ({0})
                              and ({1})
                              and A.ReasonID = B.MachineHaltReasonID
                              and A.OrganizationID = C.OrganizationID
                              and D.OrganizationID = '{2}'
                              and C.LevelCode like D.LevelCode + '%'
                              group by A.EquipmentID, B.ReasonStatisticsTypeId ";
            m_Sql = string.Format(m_Sql, myVariableIdString, myWorkingTimeString, myOrganizaitonId);
            try
            {
                DataTable m_MachinHaltCountTable = myDataFactory.Query(m_Sql);
                return m_MachinHaltCountTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetMachinHaltTime(string myWorkingTimeString, string myVariableIdString, string myOrganizaitonId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT A.EquipmentID as VariableId
                                   ,B.ReasonStatisticsTypeId
	                               ,sum( case when A.RecoverTime > A.HaltTime then convert(decimal(18,4), DATEDIFF (second, A.HaltTime, A.RecoverTime))/3600 else 0.0 end) as Value 
                              FROM shift_MachineHaltLog A, system_MachineHaltReason B, system_Organization C, system_Organization D
                              where A.EquipmentID in ({0})
                              and ({1})
                              and A.ReasonID = B.MachineHaltReasonID
                              and A.OrganizationID = C.OrganizationID
                              and D.OrganizationID = '{2}'
                              and C.LevelCode like D.LevelCode + '%'
                              group by A.EquipmentID, B.ReasonStatisticsTypeId ";
            m_Sql = string.Format(m_Sql, myVariableIdString, myWorkingTimeString, myOrganizaitonId);
            try
            {
                DataTable m_MachinHaltCountTable = myDataFactory.Query(m_Sql);
                return m_MachinHaltCountTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetMachineHaltTimeForRunIndicator(DataTable myWorkingTimeTable, string myVariableIdString, string myOrganizaitonId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_StartTime = ((DateTime)myWorkingTimeTable.Compute("Min(StartTime)", null)).ToString("yyyy-MM-dd HH:mm:ss");
            string m_EndTime = ((DateTime)myWorkingTimeTable.Compute("Max(StartTime)", null)).ToString("yyyy-MM-dd HH:mm:ss");
            string m_Sql = @"Select C.MachineHaltLogID
                                        ,C.OrganizationID
                                        ,(case when C.HaltTime is null then convert(datetime,'{0}') else C.HaltTime end) as HaltTime
                                        ,(case when C.RecoverTime is null then convert(datetime,'{1}') else C.RecoverTime end) as RecoverTime
                                        ,C.EquipmentID as VariableId
                                        ,C.ReasonID as ReasonId
										,F.ReasonStatisticsTypeId 
										from shift_MachineHaltLog C
										   left join system_MachineHaltReason F on C.ReasonID = F.MachineHaltReasonID
										where ((C.HaltTime >= '{0}' and C.HaltTime <= '{1}')
										   or (C.RecoverTime >= '{0}' and C.RecoverTime <= '{1}')
                                           or (C.HaltTime < '{0}' and C.RecoverTime > '{1}')
                                           or (C.HaltTime <= '{1}' and C.RecoverTime is null)
										   or (C.RecoverTime >= '{0}' and C.HaltTime is null))
                                        and C.OrganizationID = '{2}'
                                        and C.EquipmentId in ({3})
                                        order by C.HaltTime";
            m_Sql = string.Format(m_Sql, m_StartTime, m_EndTime, myOrganizaitonId, myVariableIdString);
            try
            {
                DataTable m_MachineHaltTimeTable = myDataFactory.Query(m_Sql);
                return m_MachineHaltTimeTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetMaterialWeightForRunIndicator(string myWorkingTimeString, string myVariableIdString, string myDataBaseName, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT EquipmentId as EquipmentId
                                  ,OutputFormula as Formula
	                              ,ROW_NUMBER() OVER (ORDER BY EquipmentId) AS VariableId
                              FROM equipment_EquipmentDetail
                              where EquipmentId in ({0})
                              order by EquipmentId";
            m_Sql = string.Format(m_Sql, myVariableIdString);
            try
            {
                DataTable m_MaterialIdTable = myDataFactory.Query(m_Sql);
                string m_SelectColumnTemp = "";
                if (m_MaterialIdTable != null)
                {
                    Dictionary<string, string> m_MaterialIdTableTemp = new Dictionary<string, string>();
                    for (int i = 0; i < m_MaterialIdTable.Rows.Count; i++)
                    {
                        if (!m_MaterialIdTableTemp.ContainsKey(m_MaterialIdTable.Rows[i]["VariableId"].ToString()))
                        {
                            m_MaterialIdTableTemp.Add("m" + m_MaterialIdTable.Rows[i]["VariableId"].ToString(), m_MaterialIdTable.Rows[i]["Formula"].ToString());
                        }
                        else
                        {
                            m_MaterialIdTableTemp["m" + m_MaterialIdTable.Rows[i]["VariableId"].ToString()] = m_MaterialIdTableTemp["m" + m_MaterialIdTable.Rows[i]["VariableId"].ToString()] + " + " + m_MaterialIdTable.Rows[i]["Formula"].ToString();
                        }
                    }
                    foreach (string myVariableItems in m_MaterialIdTableTemp.Keys)
                    {
                        if (m_SelectColumnTemp == "")
                        {
                            m_SelectColumnTemp = "sum(" + m_MaterialIdTableTemp[myVariableItems] + ") as " + myVariableItems;
                        }
                        else
                        {
                            m_SelectColumnTemp = m_SelectColumnTemp + ", sum(" + m_MaterialIdTableTemp[myVariableItems] + ") as " + myVariableItems;
                        }
                    }
                    DataTable m_EquipmentOutputValueTable = GetEquipmentOutputValue(myWorkingTimeString, m_SelectColumnTemp, myDataBaseName, m_MaterialIdTable, myDataFactory);
                    return m_EquipmentOutputValueTable;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetEquipmentOutputValue(string myWorkingTimeString, string mySelectColumnString, string myDataBaseName, DataTable myMaterialIdTable, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"Select {0} from {1}.dbo.HistoryDCSIncrement where {2}";
            DataTable m_EquipmentOutputTable = new DataTable();
            m_EquipmentOutputTable.Columns.Add("VariableId", typeof(string));
            m_EquipmentOutputTable.Columns.Add("Value", typeof(decimal));
            m_Sql = string.Format(m_Sql, mySelectColumnString, myDataBaseName, myWorkingTimeString);
            try
            {
                DataTable m_MaterialValueTable = myDataFactory.Query(m_Sql);
                if (m_MaterialValueTable != null)
                {
                    for (int i = 0; i < myMaterialIdTable.Rows.Count; i++)
                    {
                        m_EquipmentOutputTable.Rows.Add(myMaterialIdTable.Rows[i]["EquipmentId"].ToString(), (decimal)m_MaterialValueTable.Rows[0]["m" + myMaterialIdTable.Rows[i]["VariableId"].ToString()]);
                    }
                }
                return m_EquipmentOutputTable;
            }
            catch
            {
                return null;
            }
        }
        private static decimal GetSumTime(DataTable myWorkingTimeTable, string myStartTimeColumnName, string myEndtimeColumnName, string myConditionString)
        {
            decimal m_SumTime = 0.0m;
            if (myConditionString != "")
            {
                DataRow[] m_WorkingTimeRowsTemp = myWorkingTimeTable.Select(myConditionString);
                for (int i = 0; i < m_WorkingTimeRowsTemp.Length; i++)
                {
                    DateTime m_StartTime = (DateTime)m_WorkingTimeRowsTemp[i][myStartTimeColumnName];
                    DateTime m_EndTime = (DateTime)m_WorkingTimeRowsTemp[i][myEndtimeColumnName];
                    m_SumTime = m_SumTime + (decimal)m_EndTime.Subtract(m_StartTime).TotalSeconds;
                }
            }
            else
            {
                for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
                {
                    DateTime m_StartTime = (DateTime)myWorkingTimeTable.Rows[i][myStartTimeColumnName];
                    DateTime m_EndTime = (DateTime)myWorkingTimeTable.Rows[i][myEndtimeColumnName];
                    m_SumTime = m_SumTime + (decimal)m_EndTime.Subtract(m_StartTime).TotalSeconds;
                }
            }
            return m_SumTime / 3600;
        }
        private static decimal GetIntersectionSumTime(DataTable mySourceDateTimeTable, DataTable myTargetDateTimeTable, string mySourceStartTimeColumnName, string mySourceEndtimeColumnName, string myTargetStartTimeColumnName, string myTargetEndtimeColumnName, string mySourceConditionString)
        {
            decimal m_SumTime = 0.0m;
            DataView TargetTableDataView = myTargetDateTimeTable.DefaultView;    //对目标表排序
            TargetTableDataView.Sort = myTargetStartTimeColumnName + " asc";
            DataTable m_TargetDateTimeTable = TargetTableDataView.ToTable();
            if (mySourceConditionString != "")
            {
                DataRow[] m_SourceTimeRowsTemp = mySourceDateTimeTable.Select(mySourceConditionString);
                for (int i = 0; i < m_SourceTimeRowsTemp.Length; i++)
                {
                    DateTime m_SourceStartTime = (DateTime)m_SourceTimeRowsTemp[i][mySourceStartTimeColumnName];
                    DateTime m_SourceEndTime = (DateTime)m_SourceTimeRowsTemp[i][mySourceEndtimeColumnName];
                    for (int j = 0; j < m_TargetDateTimeTable.Rows.Count; j++)
                    {
                        DateTime m_TargetStartTime = (DateTime)m_TargetDateTimeTable.Rows[j][myTargetStartTimeColumnName];
                        DateTime m_TargetEndTime = (DateTime)m_TargetDateTimeTable.Rows[j][myTargetEndtimeColumnName];
                        DateTime m_StartTime;
                        DateTime m_EndTime;
                        if (m_SourceStartTime < m_TargetStartTime)
                        {
                            m_StartTime = m_TargetStartTime;
                        }
                        else
                        {
                            m_StartTime = m_SourceStartTime;
                        }
                        if (m_SourceEndTime < m_TargetEndTime)
                        {
                            m_EndTime = m_SourceEndTime;
                        }
                        else
                        {
                            m_EndTime = m_TargetEndTime;
                        }
                        decimal m_DiffSeconds = (decimal)m_EndTime.Subtract(m_StartTime).TotalSeconds;
                        if (m_DiffSeconds > 0)
                        {
                            m_SumTime = m_SumTime + m_DiffSeconds;
                        }
                    }
                }
            }
            return m_SumTime / 3600;
        }

        /// <summary>
        /// ///////////////获得DCS标签值///////////////////
        /// </summary>
        /// <param name="myTagIdArray">标签ID列表</param>
        /// <returns></returns>
        private static DataTable GetTagDataContrast(string[] myTagIdArray, string[] myTagDataBase, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_TagsContrastSql = "";
            string m_Tags = "";
            for (int i = 0; i < myTagIdArray.Length; i++)
            {
                if (i == 0)
                {
                    m_Tags = "'" + myTagIdArray[i] + "'";
                }
                else
                {
                    m_Tags = m_Tags + ",'" + myTagIdArray[i] + "'";
                }
            }
            for (int i = 0; i < myTagDataBase.Length; i++)
            {
                if (i == 0)
                {
                    m_TagsContrastSql = string.Format(@"SELECT distinct rtrim(A.Item) as Id
                            ,rtrim(A.TableName) as TableName
                            ,rtrim(A.FieldName) as FieldName
                            ,A.DataType
                            ,A.IsCumulant
                            ,'{0}' as DataBaseName
                        FROM {0}.dbo.DCSContrast A
                        where A.DataTypeStandard <> 'bit' and A.Item in ({1}) ", myTagDataBase[i], m_Tags);
                }
                else
                {
                    m_TagsContrastSql = m_TagsContrastSql + "union all" + string.Format(@"SELECT distinct rtrim(A.Item) as Id
                            ,rtrim(A.TableName) as TableName
                            ,rtrim(A.FieldName) as FieldName
                            ,A.DataType
                            ,A.IsCumulant
                            ,'{0}' as DataBaseName
                        FROM {0}.dbo.DCSContrast A
                        where A.DataTypeStandard <> 'bit' and A.Item in ({1}) ", myTagDataBase[i], m_Tags);
                }
            }
            try
            {
                DataTable m_ContrastTable = myDataFactory.Query(m_TagsContrastSql);
                return m_ContrastTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetTagDataValue(DataTable myContrastTable, DataTable myWorkingTimeTable, string CaculateType, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            if (myContrastTable != null)
            {
                string m_SqlSelect = "";
                string m_Condistion = "";
                Dictionary<string, string[]> m_DataTableName = new Dictionary<string, string[]>();
                for (int i = 0; i < myContrastTable.Rows.Count; i++)
                {
                    string m_Id = myContrastTable.Rows[i]["Id"].ToString();
                    string m_DataBaseName = myContrastTable.Rows[i]["DataBaseName"].ToString();
                    string m_TableName = "History_" + myContrastTable.Rows[i]["TableName"].ToString();
                    string m_FieldName = myContrastTable.Rows[i]["FieldName"].ToString();
                    if (!m_DataTableName.ContainsKey(m_DataBaseName + m_TableName))
                    {
                        m_DataTableName.Add(m_DataBaseName + m_TableName, new string[] { m_DataBaseName, m_TableName });
                    }
                    if (i == 0)
                    {
                        m_SqlSelect = string.Format("Select {3}({0}.{1}) as {2}", m_TableName, m_FieldName, m_Id, CaculateType);
                    }
                    else
                    {
                        m_SqlSelect = m_SqlSelect + ", " + string.Format("{3}({0}.{1}) as {2}", m_TableName, m_FieldName, m_Id, CaculateType);
                    }
                }

                string m_LastDataTableValue = "";

                foreach (string[] m_DataTableValue in m_DataTableName.Values)
                {
                    if (m_LastDataTableValue == "")
                    {
                        m_SqlSelect = m_SqlSelect + string.Format(" from {0}.dbo.{1}", m_DataTableValue[0], m_DataTableValue[1]);

                        string m_WorkingTimeStringTemplate = "(" + m_DataTableValue[1] + ".vDate >= '{0}' and " + m_DataTableValue[1] + ".vDate <= '{1}')";
                        string m_WorkingTimeString = "";
                        if (myWorkingTimeTable != null)
                        {
                            for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
                            {
                                if (i == 0)
                                {
                                    m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }
                        }

                        m_Condistion = string.Format(" where {0}", m_WorkingTimeString);
                    }
                    else
                    {
                        m_SqlSelect = m_SqlSelect + string.Format(", {0}.dbo.{1}", m_DataTableValue[0], m_DataTableValue[1]);
                        m_Condistion = m_Condistion + string.Format(" and {0}.vDate = {1}.vDate", m_LastDataTableValue, m_DataTableValue[1]);
                    }
                    m_LastDataTableValue = m_DataTableValue[1];
                }
                m_SqlSelect = m_SqlSelect + m_Condistion;
                try
                {
                    DataTable mTagValue = myDataFactory.Query(m_SqlSelect);
                    return mTagValue;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        private static DataTable GetCurrentDataValue(string[] myFactoryDataBaseID, string[] myTagIdList, DataTable myWorkingTimeTable, string CaculateType, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            if (myTagIdList != null && myFactoryDataBaseID != null && myTagIdList.Length > 0 && myFactoryDataBaseID.Length > 0)
            {
                string m_TableName = "History_Current";
                string m_SqlSelect = "";
                string m_Condistion = "";
                string m_LastDataTableValue = "";
                for (int i = 0; i < myFactoryDataBaseID.Length; i++)
                {
                    string m_DataBaseName = myFactoryDataBaseID[i];
                    for (int j = 0; j < myTagIdList.Length; j++)
                    {
                        string m_Id = myTagIdList[j];
                        string m_FieldName = myTagIdList[j];
                        if (m_SqlSelect == "")
                        {
                            m_SqlSelect = string.Format("Select {3}({0}.{1}) as {2}", m_TableName, m_FieldName, m_Id, CaculateType);
                        }
                        else
                        {
                            m_SqlSelect = m_SqlSelect + ", " + string.Format("{3}({0}.{1}) as {2}", m_TableName, m_FieldName, m_Id, CaculateType);
                        }
                    }
                }

                for (int j = 0; j < myFactoryDataBaseID.Length; j++)
                {
                    string m_DataBaseName = myFactoryDataBaseID[j];
                    if (m_LastDataTableValue == "")
                    {
                        m_SqlSelect = m_SqlSelect + string.Format(" from {0}.dbo.{1}", m_DataBaseName, m_TableName);

                        string m_WorkingTimeStringTemplate = "(" + m_TableName + ".vDate >= '{0}' and " + m_TableName + ".vDate <= '{1}')";
                        string m_WorkingTimeString = "";
                        if (myWorkingTimeTable != null)
                        {
                            for (int i = 0; i < myWorkingTimeTable.Rows.Count; i++)
                            {
                                if (i == 0)
                                {
                                    m_WorkingTimeString = string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    m_WorkingTimeString = m_WorkingTimeString + " or " + string.Format(m_WorkingTimeStringTemplate, ((DateTime)myWorkingTimeTable.Rows[i]["StartTime"]).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)myWorkingTimeTable.Rows[i]["EndTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }
                        }

                        m_Condistion = string.Format(" where {0}", m_WorkingTimeString);
                    }
                    else
                    {
                        m_SqlSelect = m_SqlSelect + string.Format(", {0}.dbo.{1}", m_DataBaseName, m_TableName);
                        m_Condistion = m_Condistion + string.Format(" and {0}.vDate = {1}.vDate", m_LastDataTableValue, m_TableName);
                    }
                }
                
                m_SqlSelect = m_SqlSelect + m_Condistion;
                try
                {
                    DataTable mTagValue = myDataFactory.Query(m_SqlSelect);
                    return mTagValue;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
