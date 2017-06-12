using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaffAssessment
{
    public class Model_CaculateItems
    {
        private string _Type;
        private string _ValueType;
        private string _OrganizaitonId;
        private string _ExtendInfo;
        private List<Model_CaculateItemDetail> _CaculateItemDetail;
        public Model_CaculateItems()
        {
            _Type = "";
            _ValueType = "";
            _OrganizaitonId = "";
            _ExtendInfo = "";
            _CaculateItemDetail = new List<Model_CaculateItemDetail>();
        }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        public string ValueType
        {
            get
            {
                return _ValueType;
            }
            set
            {
                _ValueType = value;
            }
        }
        public string OrganizaitonId
        {
            get
            {
                return _OrganizaitonId;
            }
            set
            {
                _OrganizaitonId = value;
            }
        }
        public string ExtendInfo
        {
            get
            {
                return _ExtendInfo;
            }
            set
            {
                _ExtendInfo = value;
            }
        }
        public List<Model_CaculateItemDetail> CaculateItemDetail
        {
            get
            {
                return _CaculateItemDetail;
            }
            set
            {
                _CaculateItemDetail = value;
            }
        }
    }
    public class Model_CaculateItemDetail
    {
        private string _Id;
        private string _AssessmentId;
        private string _ObjectId;
        private decimal _WeightedValue;
        private decimal _BestValue;
        private decimal _WorstValue;
        private decimal _StandardValue;
        private decimal _StandardScore;
        private decimal _ScoreFactor;
        private decimal _MaxScore;
        private decimal _MinScore;
        private decimal _CaculateValue;
        private decimal _CaculateScore;
        public Model_CaculateItemDetail()
        {
            _Id = "";
            _AssessmentId = "";
            _WeightedValue = 0.0m;
            _BestValue = 0.0m;
            _WorstValue = 0.0m;
            _StandardValue = 0.0m;
            _StandardScore = 0.0m;
            _ScoreFactor = 0.0m;
            _MaxScore = 0.0m;
            _MinScore = 0.0m;
            _CaculateValue = 0.0m;
            _CaculateScore = 0.0m;
        }
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string AssessmentId
        {
            get
            {
                return _AssessmentId;
            }
            set
            {
                _AssessmentId = value;
            }
        }
       
        public string ObjectId
        {
            get
            {
                return _ObjectId;
            }
            set
            {
                _ObjectId = value;
            }
        }
      
        public decimal WeightedValue
        {
            get
            {
                return _WeightedValue;
            }
            set
            {
                _WeightedValue = value;
            }
        }
        public decimal BestValue
        {
            get
            {
                return _BestValue;
            }
            set
            {
                _BestValue = value;
            }
        }
        public decimal WorstValue
        {
            get
            {
                return _WorstValue;
            }
            set
            {
                _WorstValue = value;
            }
        }
        public decimal StandardValue
        {
            get
            {
                return _StandardValue;
            }
            set
            {
                _StandardValue = value;
            }
        }
        public decimal StandardScore
        {
            get
            {
                return _StandardScore;
            }
            set
            {
                _StandardScore = value;
            }
        }
        public decimal ScoreFactor
        {
            get
            {
                return _ScoreFactor;
            }
            set
            {
                _ScoreFactor = value;
            }
        }
        public decimal MaxScore
        {
            get
            {
                return _MaxScore;
            }
            set
            {
                _MaxScore = value;
            }
        }
        public decimal MinScore
        {
            get
            {
                return _MinScore;
            }
            set
            {
                _MinScore = value;
            }
        }
        public decimal CaculateValue
        {
            get
            {
                return _CaculateValue;
            }
            set
            {
                _CaculateValue = value;
            }
        }
        public decimal CaculateScore
        {
            get
            {
                return _CaculateScore;
            }
            set
            {
                _CaculateScore = value;
            }
        }
    }
}
