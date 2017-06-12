using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace StaffAssessment
{
    public class Table_AssessmentCatalogue
    {
        public static DataTable GetAssessmentCatalogue()
        {
            DataTable m_AssessmentCatalogueTable = GetAssessmentCatalogueTableConstruct();
            m_AssessmentCatalogueTable.Rows.Add("EnergyMaterialWeight", "产量/消耗量", "MaterialWeight", "MaterialWeight", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ProcessElectricityQuantity", "电量", "Energy", "ElectricityQuantity", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ProcessElectricityConsumption", "工序电耗", "Energy", "ElectricityConsumption", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ProcessCoalConsumption", "工序煤耗", "Energy", "CoalConsumption", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ElectricityConsumpitionAlarmCount", "电耗报警次数", "Energy", "ElectricityConsumptionAlarmCount", "", "");
            m_AssessmentCatalogueTable.Rows.Add("CoalConsumpitionAlarmCount", "煤耗报警次数", "Energy", "CoalConsumptionAlarmCount", "", "");
            m_AssessmentCatalogueTable.Rows.Add("PowerAlarmCount", "功率报警次数", "Energy", "PowAlarmCount", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ComprehensiveElectricityConsumption", "综合电耗", "Comprehensive", "ComprehensivePowerConsumption", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ComprehensiveCoalConsumption", "综合煤耗", "Comprehensive", "ComprehensiveCoalConsumption", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ComprehensiveEnergyConsumption", "综合能耗", "Comprehensive", "ComprehensiveEnergyConsumption", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ComparableElectricityConsumption", "可比综合电耗", "Comprehensive", "ComprehensivePowerConsumptionComparable", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ComparableCoalConsumption", "可比综合煤耗", "Comprehensive", "ComprehensiveCoalConsumptionComparable", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ComparableEnergyConsumption", "可比综合能耗", "Comprehensive", "ComprehensiveEnergyConsumptionComparable", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ElectricalDowntimeCount", "电气故障次数", "Production", "ElectricalDowntimeCount", "", "");
            m_AssessmentCatalogueTable.Rows.Add("MechanicalDowntimeCount", "机械故障次数", "Production", "MechanicalDowntimeCount", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ProcessDowntimeCount", "工艺故障次数", "Production", "ProcessDowntimeCount", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ElectricalDowntimeTime", "电气故障时间", "Production", "ElectricalDowntimeTime", "", "");
            m_AssessmentCatalogueTable.Rows.Add("MechanicalDowntimeTime", "机械故障时间", "Production", "MechanicalDowntimeTime", "", "");
            m_AssessmentCatalogueTable.Rows.Add("ProcessDowntimeTime", "工艺故障时间", "Production", "ProcessDowntimeTime", "", "");
            m_AssessmentCatalogueTable.Rows.Add("RunningRate", "运转率", "Production", "RunningRate", "", "");
            m_AssessmentCatalogueTable.Rows.Add("FaultRate", "故障率", "Production", "FaultRate", "", "");
            m_AssessmentCatalogueTable.Rows.Add("OutputPerHour", "台时产量", "Production", "OutputPerHour", "", "");
            /*
            DataTable m_RunIndicatorsTable = RunIndicators.RunIndicatorsItems.GetRunIndicatorsItemsTable();
            if (m_RunIndicatorsTable != null)
            {
                for (int i = 0; i < m_RunIndicatorsTable.Rows.Count; i++)
                {
                    m_AssessmentCatalogueTable.Rows.Add(m_RunIndicatorsTable.Rows[i]["IndicatorId"].ToString()
                                                        , m_RunIndicatorsTable.Rows[i]["IndicatorName"].ToString()
                                                        , "Production"
                                                        , "RunIndicators"
                                                        , ""
                                                        , "通过指标库计算");
                }
            }*/
            return m_AssessmentCatalogueTable;
        }
        private static DataTable GetAssessmentCatalogueTableConstruct()
        {
            DataTable AssessmentCatalogueTable = new DataTable();
            AssessmentCatalogueTable.Columns.Add("AssessmentId", typeof(string));
            AssessmentCatalogueTable.Columns.Add("Name", typeof(string));
            AssessmentCatalogueTable.Columns.Add("Type", typeof(string));
            AssessmentCatalogueTable.Columns.Add("ValueType", typeof(string));
            AssessmentCatalogueTable.Columns.Add("KeyId", typeof(string));
            AssessmentCatalogueTable.Columns.Add("Remark", typeof(string));
            return AssessmentCatalogueTable;
        }
    }
}
