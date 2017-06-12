var mOrganizationId = "";
var mWorkingSectionID = "";
var mProductionName = "";
var mProductionID = "";
var mStaffId = "";
var mStaffName = "";
var mStartTime = "";
var mEndTime = "";
var mGroupId = "";
var mGroupName = "";
var mStatisticalCycle = "";
var mAssessmentCycle = "";
var mVersionId = "";
$(document).ready(function () {
    InitialDate("month");
    LoadAssessmentGroupGrid();
    LoadMainDataGrid("first");
    LoadresultDetailDataGrid("first");// grid_resultDetail
});
function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    mOrganizationId = node.OrganizationId;
    LoadWorkingSectionList(mOrganizationId);
}
var mCoefficient = "";
var mWorkingSectionItemID = "";
function LoadWorkingSectionList(mValue) {
    $.ajax({
        type: "POST",
        url: "StaffAssessment.aspx/GetWorkingSection",
        data: " {mOrganizationID:'" + mValue + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#workingSection').combobox({
                valueField: 'WorkingSectionItemID',
                textField: 'WorkingSectionName',
                panelHeight: '300',
                data: myData.rows,
                onSelect: function (record) {
                    mWorkingSectionItemID = record.WorkingSectionItemID;
                    mProductionID = record.OrganizationID;
                    mCoefficient = record.AssessmentCoefficient;
                    LoadStaffInfo(mWorkingSectionItemID);     //员工信息
                    LoadAssessmentVersion(mWorkingSectionItemID);
                }
            });
        },       
        error: function () {
            $.messager.alert('失败', '加载失败！');
        }
    });
}
//根据岗位加载员工列表
function LoadStaffInfo(mWorkingSectionItemId) {
    $.ajax({
        type: "POST",
        url: "StaffAssessment.aspx/GetStaffInfo",
        data: " {mProductionId:'" + mOrganizationId + "',mWorkingSectionItemID:'" + mWorkingSectionItemId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#Staff').combobox({
                data: myData.rows,
                valueField: 'id',
                textField: 'text',
                panelHeight: 'auto',
                onSelect: function (record) {
                    mStaffId = record.id;
                    mStaffName = record.Name;
                }
            });
            $('#Staff').combobox('select', 0);
        },
        error: function () {
            $.messager.alert('失败', '加载失败！');
        }
    });
}
//加载考核分组
function LoadAssessmentGroupGrid() {
    $.ajax({
        type: "POST",
        url: "StaffAssessment.aspx/GetAssessmentGroupGrid",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#AssessmentGroup').combogrid({
                panelWidth: '180px',
                panelHeight: 'auto',
                idField: 'GroupId',
                textField: 'Name',
                columns: [[
                    { field: 'GroupId', title: '', width: 40, hidden: true },
                    { field: 'Name', title: '考核分组', width: 100 },
                    {
                        field: 'StatisticalCycle', title: '考核周期', align: 'center', width: 75, formatter: function (value, row, index) {
                            if (value == "day") {
                                return value = "日";
                            } else if (value == "month") {
                                return value = "月";
                            } else if (value == "year") {
                                return value = "年";
                            }
                        }
                    }
                ]],
                data: myData,
                onSelect: function (index, row) {
                    mGroupId = row.GroupId;
                    mGroupName = row.Name;
                    mStatisticalCycle = row.StatisticalCycle;
                    InitialDate(mStatisticalCycle);
                    if (mStatisticalCycle == "day") {
                        mAssessmentCycle = "日";
                    } else if (mStatisticalCycle == "month") {
                        mAssessmentCycle = "月";
                    } else if (mStatisticalCycle == "year") {
                        mAssessmentCycle = "年";
                    }
                    $('#AssessmentCycle').textbox('setText', mAssessmentCycle);
                }
            });
        },
        error: function () {
            // $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    });
}
//加载版本
function LoadAssessmentVersion(workingSectionItemId){
    $.ajax({
        type: "POST",
        url: "StaffAssessment.aspx/GetAssessmentVersion",
        data: " {mOrganizationID:'" + mOrganizationId + "',mWorkingSectionItemID:'" + workingSectionItemId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#AssessmentVersion').combobox({
                data: myData.rows,
                valueField: 'KeyId',
                textField: 'Name',
                panelHeight: 'auto',
                onSelect: function (record) {
                     mVersionId = record.KeyId;
                }
            });
            $('#Staff').combobox('select', 0);
        },
        error: function () {
           // $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    });

}
var resultData;
var saveData = new Array();
function Calculate() {
    if (mStatisticalCycle != "") {
        if (mStatisticalCycle == 'month') {
            mStartTime = $('#date_smonth').combobox('getValue');
            mEndTime = "";
        } else if (mStatisticalCycle == 'year') {
            mStartTime = $('#date_year').combobox('getValue');
            mEndTime = "";
        } else if (mStatisticalCycle == 'day') {
            mStartTime = $('#date_sday').datebox('getValue');
            mEndTime = $('#date_eday').datebox('getValue');
        }
        if (mStartTime == "") {
            $.messager.alert('提示', '请选择考核日期！');
        } else { 
            $.ajax({
                type: "POST",
                url: "StaffAssessment.aspx/CalculateStaffAssessment",  //(mProductionID, mWorkingSectionID, mStaffId,mStaffName, mGroupId,mGroupName, mStartTime, mVersionId, mStatisticalCycle)
                data: "{mProductionID:'" + mOrganizationId + "',mWorkingSectionID:'" + mWorkingSectionItemID + "',mCoefficient:'" + mCoefficient + "',mStaffId:'" + mStaffId + "',mStaffName:'" + mStaffName + "',mGroupId:'" + mGroupId + "',mGroupName:'" + mGroupName + "',mStartTime:'" + mStartTime + "',mEndTime:'" + mEndTime + "',mVersionId:'" + mVersionId + "',mStatisticalCycle:'" + mStatisticalCycle + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    var mData = myData.split("&");
                    saveData = mData;
                    var tzData = jQuery.parseJSON(mData[0]);
                    resultData = jQuery.parseJSON(mData[1]);

                    if (myData.total == 0) {
                        LoadMainDataGrid("last", []);
                        $.messager.alert('提示', '0条考核数据！');
                    } else {
                        LoadMainDataGrid("last", tzData);
                    }
                },
                error: function () {
                    $.messager.alert('失败', '加载失败！');
                }
            });
        }
    } else {
        $.messager.alert('提示', '请选择考核周期！');
    }
}
function Query() {
    var mUrl = "";
    var mData = "";
    if (mStatisticalCycle == "day") {
        mStartTime = $('#date_sday').datebox('getValue');
        mEndTime = $('#date_eday').datebox('getValue')+' 23:59:59';
    }
    else if (mStatisticalCycle == "month") {
        var myMonth=$('#date_smonth').combobox('getValue');
        mStartTime = myMonth+'-01';
        mEndTime = myMonth + '-' + getDaysInMonth(myMonth)+' 23:59:59';
    } else if (mStatisticalCycle == "year") {
        mStartTime = $('#date_year').datebox('getValue') + '-01-01 00:00:00';
        mEndTime = $('#date_year').datebox('getValue')+'-12-31 23:59:59';
    }
    mUrl = "StaffAssessment.aspx/GetAssessmentResult";
    mData = " {mProductionID:'" + mOrganizationId + "',mWorkingSectionID:'" + mWorkingSectionItemID + "',mStaffId:'" + mStaffId + "',mGroupId:'" + mGroupId + "',mGroupName:'" + mGroupName + "',mStartTime:'" + mStartTime + "',mEndTime:'" + mEndTime + "',mVersionId:'" + mVersionId + "',mStatisticalCycle:'" + mStatisticalCycle + "'}";
    $.ajax({
        type: "POST",
        url: mUrl,
        data: mData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //var myData = jQuery.parseJSON(msg.d);
            //LoadMainDataGrid("last", myData);
            var myData = msg.d;  
            var mData = myData.split("&");
            var tzData = jQuery.parseJSON(mData[0]);
            resultData = jQuery.parseJSON(mData[1]);

            if (tzData.total == 0) {
                LoadMainDataGrid("last", []);
                LoadresultDetailDataGrid("last", []);
                $.messager.alert('提示', '未查询到考查记录，请进行考核计算！');
            } else {
                LoadresultDetailDataGrid("last", []);
                LoadMainDataGrid("last", tzData);
            }        
        },
        error: function () {
            $.messager.alert('失败', '加载失败！');
        }
    });
}
function Save() {
    var saveTz = JSON.stringify(jQuery.parseJSON(saveData[0]).rows);
    saveTz = saveTz.replace('[','').replace(']','');
    var saveDetail = JSON.stringify(jQuery.parseJSON(saveData[1]).rows);
    saveDetail = saveDetail.replace('[', '').replace(']', '');
    //var saveTz = jQuery.parseJSON(saveData[0]).rows;
    //var saveDetail = jQuery.parseJSON(saveData[1]).rows;
    if (saveTz.total == 0) {
        $.messager.alert("提示","没有可保存结构！");
    } else {
        $.ajax({
            type: "POST",
            url: "StaffAssessment.aspx/SaveStaffAssessmentResult",  //(mProductionID, mWorkingSectionID, mStaffId,mStaffName, mGroupId,mGroupName, mStartTime, mVersionId, mStatisticalCycle)
            data: "{TzJson:'" + saveTz + "',DetailJson:'" + saveDetail + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var result = msg.d;
                if (result=="-1") {
                    $.messager.alert('提示', '不需要重复保存！');              
                } else if (result == "0") {
                    $.messager.alert('提示', '保存失败！');
                } else if (result == "1") {
                    $.messager.alert('提示', '保存成功！');
                }        
            },
            error: function () {
                $.messager.alert('失败', '保存失败！');
            }
        });
    }
}
function Clear() {
    LoadMainDataGrid("last", []);
    LoadresultDetailDataGrid("last", []);
}
function AssessmentResultdetail(myKeyId,myStaffName) {
    var mStaffName = myStaffName;
    var m_Id = "";
    var m_AssessmentId = "";
    var m_AssessmentName = "";
    var m_ObjectId = "";
    var m_ObjectName = "";
    var m_OrganizationID = "";
    var m_KeyId = "";
    var m_WeightedValue = "";
    var m_BestValue = "";
    var m_WorstValue = "";
    var m_AssessmenScore = "";
    var m_WeightedAverageCredit = "";
    var m_OrganizationName = "";
    var myDetail=[];
    for (var i = 0; i < resultData.rows.length; i++) {
        var test = resultData.rows[i]["KeyId"].toLowerCase();
        if (resultData.rows[i]["KeyId"].toLowerCase() == myKeyId.toLowerCase()) {
            m_Id = resultData.rows[i]["Id"];
            m_AssessmentId = resultData.rows[i]["AssessmentId"];
            m_AssessmentName = resultData.rows[i]["AssessmentName"];
            m_ObjectId = resultData.rows[i]["ObjectId"];
            m_ObjectName = resultData.rows[i]["ObjectName"];
            m_OrganizationID = resultData.rows[i]["OrganizationID"];
            m_KeyId = resultData.rows[i]["KeyId"];
            m_WeightedValue = resultData.rows[i]["WeightedValue"];
            m_BestValue = resultData.rows[i]["BestValue"];
            m_WorstValue = resultData.rows[i]["WorstValue"];
            m_AssessmenScore = resultData.rows[i]["AssessmenScore"];
            m_WeightedAverageCredit = resultData.rows[i]["WeightedAverageCredit"];
            m_OrganizationName = resultData.rows[i]["OrganizationName"]
            var myRow = { "StaffName": mStaffName, "Id": m_Id, "AssessmentId": m_AssessmentId, "AssessmentName": m_AssessmentName, "ObjectId": m_ObjectId, "ObjectName": m_ObjectName, "OrganizationID": m_OrganizationID, "KeyId": m_KeyId, "WeightedValue": m_WeightedValue, "BestValue": m_BestValue, "WorstValue": m_WorstValue, "AssessmenScore": m_AssessmenScore, "WeightedAverageCredit": m_WeightedAverageCredit, "OrganizationName": m_OrganizationName };
            myDetail.push(myRow);
        }
    }
    if (0 == myDetail.length) {
        LoadresultDetailDataGrid("last", []);
        $.messager.alert('提示', '未查询到数据！');
    } else {
        LoadresultDetailDataGrid('last', myDetail);
    }
    //myDetail

}
function LoadMainDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').datagrid({
            columns: [[
                    { field: 'StaffName', title: '员工', width: 60 },
                    { field: 'StartTime', title: '考核开始时间', width: 120 },
                    { field: 'EndTime', title: '考核结束时间', width: 120 },
                    {
                        field: 'edit', title: '查看', width: 60, formatter: function (value, row, index) {
                            return  '<a href="#" onclick="AssessmentResultdetail(\'' + row.KeyId + '\',\''+row.StaffName+'\')"><img class="iconImg" src = "/lib/ealib/themes/icons/search.png" title="查看" onclick="AssessmentResultdetail(\'' + row.KeyId + '\',\''+row.StaffName+ '\')"/>查看</a>';                           
                        }
                    }
            ]],
            fit: true,
           // toolbar: "#toorBar",
            idField: 'KeyId',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
    else {
        $('#grid_Main').datagrid('loadData', myData);
    }
}
function LoadresultDetailDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_resultDetail').datagrid({
            frozenColumns: [[
                { field: 'StaffName', title: '员工', width: 60 }
            ]],
            columns: [[
                    { field: 'OrganizationName', title: '产线', width: 100 },
                    { field: 'ObjectName', title: '考核元素', width: 130 },
                    { field: 'AssessmentName', title: '考核项', width: 100 },                   
                    { field: 'WeightedValue', title: '权重', width: 80 },
                    //{ field: 'BestValue', title: '最好值', width: 80 },
                    //{ field: 'WorstValue', title: '最差值', width: 80 },
                    { field: 'AssessmenScore', title: '考核分', width: 80 },
                    { field: 'WeightedAverageCredit', title: '加权分', width: 80 }
            ]],
            fit: true,
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
    else {
        $('#grid_resultDetail').datagrid('loadData', myData);
    }
}


///初始化时间
function InitialDate(type) {
    var nowDate = new Date();
    var beforeDate = new Date();
    if ("year" == type) {
        $(".myear").show();
        $(".mmonth").hide();
        $(".mday").hide();
        var lastYear = nowDate.getFullYear() - 1;
        //myear = $("#date_year").val(lastYear);
        var mData = [];
        for (var i = 0; i < 3;i++){
            lastYear = lastYear - i;
            mdata = {
                type: lastYear,
                value: lastYear
            };
            mData.push(mdata);
        }
        $("#date_year").combobox({
            valueField: 'value',
            textField: 'type',
            data: mData,
            panelHeight: 'auto',
            onSelect: function (node) {
                mValue = node.value;
                InitialDate(mValue)
            }
        });
        $('#date_year').combobox('setValue', nowDate.getFullYear() - 1);
    }
    else if ("month" == type) {
        $(".myear").hide();
        $(".mmonth").show();
        $(".mday").hide();
        //emonth = $("#date_emonth").val(endDate);
        var mData = [];
        for (var i = nowDate.getMonth() ; i >= 1 ; i--) {
            if (i < 10) {
                i = "0" + i;
            }
            var monthDate = nowDate.getFullYear() + '-' + i;
            mdata = {
                type: monthDate,
                value: monthDate
            };
            mData.push(mdata);        
        }
        $("#date_smonth").combobox({
            valueField: 'value',
            textField: 'type',
            data:mData,
            panelHeight: 'auto',
            onSelect: function (node) {
                mValue = node.value;
                InitialDate(mValue)
            }
        });
        var endDate = nowDate.getFullYear() + '-' + (nowDate.getMonth());
        if (nowDate.getMonth() < 10) {
            var endDate = nowDate.getFullYear() + '-0' + (nowDate.getMonth());
        }
        $('#date_smonth').combobox('setValue', endDate);
    }
    else if ("day" == type) {
        $(".myear").hide();
        $(".mmonth").hide();
        $(".mday").show();
        beforeDate.setDate(nowDate.getDate());
        var startDate = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + (beforeDate.getDate());
        var endDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
        sday = $('#date_sday').datebox('setValue', startDate);
        eday = $('#date_eday').datebox('setValue', endDate);
    }
}
//根据月份获取这个月的天数
function getDaysInMonth(myDate) {
    var yearMonth = myDate.split('-');
    year = yearMonth[0];
    month=yearMonth[1];
    month = parseInt(month, 10); //parseInt(number,type)这个函数后面如果不跟第2个参数来表示进制的话，默认是10进制。 
    var temp = new Date(year, month, 0);
    return temp.getDate();
}