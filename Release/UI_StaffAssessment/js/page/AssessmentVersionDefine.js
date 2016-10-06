//用于查询
var mOrganizationId = "";
var myOrganizationId = "";                           
var IsAdd = true;
//用于添加和编辑
var eName = "";
var eType = "";
               
var eWorkingSectionID = "";
var eRemark = "";
var eCreator = "";
//
var eKeyId = "";
var meditContrastId = "";
////用于考核详细表的增加和编辑
var eAssessmentObjectId = "";
var eAssessmentName = "";
var eObjectId = "";
var eObjectName = "";
var eProcessLine = "";
var eWeightedValue = "";
var eBestValue = "";
var eWorstValue = "";
var eStandardValue = "";
var eStandardScore = "";
var eScoreFactor = "";
var eMaxScore = "";
var eMinScore = "";
var eEnabled = "";

var IsAddDetail = true;
var eId = "";
$(document).ready(function () {
    LoadMainDataGrid("first");
    LoadMainDataGridDetail("first");
    LoadAssessmentCatalogue();
});
function onOrganisationTreeClick(node) {

    mOrganizationId = node.OrganizationId;
    var mLevel = mOrganizationId.split('_');
    if (mLevel.length != 4) {
        $.messager.alert('提示','请选择分厂级别！');

    } else {
        $('#organizationName').textbox('setText', node.text);
        LoadWorkingSection(mOrganizationId);
        //   LoadEditWorkingSection(mOrganizationId);
        LoadProcessLine(mOrganizationId);
    }
}
var myType ="";
var myValueType ="";
function LoadAssessmentCatalogue() {
    $.ajax({
        type: "POST",
        url: "AssessmentVersionDefine.aspx/GetAssessmentCatalogue",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            var cobdata = myData.rows;
            cobdata.sort();
            $('#eAssessmentObject').combobox({
                panelHeight: '200',
                valueField: 'AssessmentId',
                textField: 'Name',
                data: cobdata,
                onSelect: function (record) {
                    myType = record.Type;
                    myValueType = record.ValueType;
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '加载失败！');
        }
    });
}
function LoadWorkingSection(mValue) {
    $.ajax({
        type: "POST",
        url: "AssessmentVersionDefine.aspx/GetWorkingSection",
        data: " {mOrganizationId:'" + mValue + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            var comboboxData = new Array();
            comboboxData[0] = { "WorkingSectionID": "0", "WorkingSectionName": "全部" };
            for (i = 1; i < myData.rows.length + 1; i++) {
                comboboxData[i] = myData.rows[i - 1];
            }
            $('#workingSection').combobox({
                valueField: 'WorkingSectionID',
                textField: 'WorkingSectionName',
                panelHeight: 'auto',
                data: comboboxData
            });
            $('#workingSection').combobox('select','0');
            $('#eWorkingSection').combobox({
                valueField: 'WorkingSectionID',
                textField: 'WorkingSectionName',
                panelHeight: 'auto',
                data: myData.rows
            });
        },
        error: function () {
            $.messager.alert('失败', '加载失败！');
        }
    });
}
function Query() {
    if (mOrganizationId == "" ) {
        $.messager.alert('提示', '请选择组织机构！');
    } else {
        mWorkingSectionID = $('#workingSection').combobox('getValue');
        var mUrl = "AssessmentVersionDefine.aspx/GetAssessmentVersionDefine";
        var mData = " {mOrganizationId:'" + mOrganizationId + "',mWorkingSectionID:'" + mWorkingSectionID + "'}";
        $.ajax({
                type: "POST",
                url: mUrl,
                data: mData,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = jQuery.parseJSON(msg.d);
                    if (myData.total == 0) {
                        LoadMainDataGrid("last", []);
                        $.messager.alert('提示', '未查询到考核版本！');
                    } else {
                        LoadMainDataGrid("last", myData);
                    }
                },
                error: function () {
                    LoadMainDataGrid("last", []);
                    $.messager.alert('提示', '加载失败！');
                }
            });
    }
}
function LoadMainDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').datagrid({
            columns: [[
                    { field: 'Name', title: '考核项名称', width: 120 },
                    //{ field: 'OrganizationName', title: '产线', width: 100 },
                    { field: 'WorkingSectionName', title: '岗位', width: 100, align: 'center' },
                    { field: 'Type', title: '考核类型', width: 60, align: 'center' },
                    { field: 'Creator', title: '编辑人', width: 65, align: 'center' },
                    { field: 'CreateTime', title: '编辑时间', width: 120 },
                    { field: 'Remark', title: '备注', width: 120 },
                    {
                        field: 'edit', title: '编辑', width: 100, formatter: function (value, row, index) {
                            var str = "";
                            str = '<a href="#" onclick="editFun(true,\'' + row.KeyId + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面" onclick="editFun(true,\'' + row.KeyId + '\')"/>编辑</a>';
                            str = str + '<a href="#" onclick="deleteFun(\'' + row.KeyId + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"  onclick="deleteFun(\'' + row.KeyId + '\')"/>删除</a>';
                            //str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面" onclick="DeletePageFun(\'' + row.id + '\');"/>删除';
                            return str;
                        }
                    },
                    {
                        field: 'reserch', title: '查询考核项', width: 100,align:'center', formatter: function (value, row, index) {
                            return '<a href="#" onclick="reserchFun(\'first\',\'' + row.KeyId + '\')"><img class="iconImg" src = "/lib/ealib/themes/icons/search.png" title="查看" onclick="reserchFun(\'first\',\'' + row.KeyId  + '\')"/>查看</a>';
                        }
                    }

            ]],
            fit: true,
            toolbar: "#toorBar",
            idField: 'KeyId',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
            //,
            //onClickRow: function (index, row) {
            //    reserchFun("first", row.KeyId, row.Name);
            //}
        });
    }
    else {
        $('#grid_Main').datagrid('loadData', myData);
    }
}
function LoadMainDataGridDetail(type, myData) {
    if (type == "first") {
        $('#grid_MainDetail').datagrid({
            columns: [[
                    { field: 'AssessmentName', title: '考核项', width: 100 },
                    { field: 'ObjectName', title: '考核元素', width: 120 },                  
                    { field: 'OrganizationName', title: '产线', width: 80 },
                    { field: 'WeightedValue', title: '权重', width: 60, align: 'center' },
                    { field: 'BestValue', title: '最好值', width: 60, align: 'center' },
                    { field: 'WorstValue', title: '最差值', width: 60, align: 'center' },
                    { field: 'StandardValue', title: '标准指标', width: 60, align: 'center' },
                    { field: 'StandardScore', title: '标准分', width: 60, align: 'center' },
                    { field: 'ScoreFactor', title: '得分因子', width: 60, align: 'center' },
                    { field: 'MaxScore', title: '最大得分', width: 60, align: 'center' },
                    { field: 'MinScore', title: '最小得分', width: 60, align: 'center' },
                    {
                        field: 'Enabled', title: '启用标志', width: 80, align: 'center', formatter: function (value, row, index) {
                            if (value=='True') {return value = "是";} else if (value =='False') { return value = "否"; }
                        }
                    },
                    {
                        field: 'edit', title: '编辑', width: 100, align: 'center', formatter: function (value, row, index) {
                            var str = "";
                            str = '<a href="#" onclick="editDetailFun(true,\'' + row.Id + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面" onclick="editDetailFun(true,\'' + row.Id + '\')"/>编辑</a>';
                            str = str + '<a href="#" onclick="deleteDetailFun(\'' + row.Id + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"  onclick="deleteDetailFun(\'' + row.Id + '\')"/>删除</a>';
                            //str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面" onclick="DeletePageFun(\'' + row.id + '\');"/>删除';
                            return str;
                        }
                    }

            ]],
            fit: true,
            toolbar: "#toorBarDetail",
            idField: 'Id',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
    else {
        $('#grid_MainDetail').datagrid('loadData', myData);
    }
}
function save()
{
    eWorkingSectionID = $('#eWorkingSection').combobox('getValue');
    eName = $('#eAssessmentName').textbox('getText');
    eType = $('#eAssessmentype').textbox('getText');
    eRemark = $('#eRemark').textbox('getText');
    if (mOrganizationId == "") {
        $.messager.alert('提示', '请选择产线！');
    } else {
        if (eWorkingSectionID==""||eName == "" || eType == "") {
            $.messager.alert('提示', '请填写未填项！');
        }
        else {
            var mUrl = "";
            var mData = "";
            if (IsAdd) {
                mUrl = "AssessmentVersionDefine.aspx/AddAssessmentVersion";
                mData = " {mOrganizationId:'" + mOrganizationId + "',mWorkingSectionID:'" + eWorkingSectionID + "',mName:'" + eName + "',mType:'" + eType  + "',mRemark:'" + eRemark + "'}";
            } else {
                mUrl = "AssessmentVersionDefine.aspx/EditAssessmentVersion";
                mData = " {mOrganizationId:'" + mOrganizationId + "',mWorkingSectionID:'" + eWorkingSectionID + "',mName:'" + eName + "',mType:'" + eType + "',mRemark:'" + eRemark + "',mKeyId:'" + eKeyId + "'}";
            }
            $.ajax({
                type: "POST",
                url: mUrl,
                data:mData,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    if (myData >= 1) {
                        if (IsAdd) {
                            $.messager.alert('提示', '添加成功！');
                        } else {
                            $.messager.alert('提示', '编辑成功！');
                        }                   
                    } else {
                        $.messager.alert('提示', '操作失败！');
                    }
                    $('#AddandEditor').window('close');
                    refreshFun();
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                }
            });
        }
    }
}
function addFun() {
    editFun(false);
}
function refreshFun() {
    Query();
}
function editFun(IsEdit, editContrastId) {
      // A.[KeyId]
      //,A.[Name]
      //,A.[Type]
	  //,B.[Name] as [OrganizationName]
      //,A.[OrganizationID]
	  //,C.[WorkingSectionName]
      //,A.[WorkingSectionID]
      //,A.[Remark]
      //,A.[Creator]
    //,A.[CreateTime]   

    if (IsEdit) {
        IsAdd = false;  //编辑
        $('#grid_Main').datagrid('selectRecord', editContrastId);
        var data = $('#grid_Main').datagrid('getSelected');
        // 1.页面显示  2 定义参数

        eKeyId = editContrastId;
        $('#eWorkingSection').combobox('setValue', data.WorkingSectionID);
        $('#eAssessmentName').textbox('setText', data.Name);
        eName = data.Name;
        $('#eAssessmentype').textbox('setText', data.Type);
        eType = data.Type;
        $('#eRemark').textbox('setText', data.Remark);
        eRemark = data.Remark;

        //var eName = "";
        //var eType = "";
        //var eProductionID = "";
        //var eWorkingSectionID = "";
        //var eRemark = "";
        //var eCreator = "";

    }
    else {
        IsAdd = true; //添加
        //初始化     1.页面显示的初始化 2 参数的初始化
        $('#eWorkingSection').combobox('clear');
        $('#eAssessmentName').textbox('setText', "");
        $('#eAssessmentype').textbox('setText', "");  
        $('#eRemark').textbox('setText', "");
        eName = "";
        eType = "";
        eRemark = "";
        if (mOrganizationId == "") {
            $.messager.alert('提示', '请选择组织机构！');
        }
    }
    $('#AddandEditor').window('open');
}
function deleteFun(editContrastId) {
    if (editContrastId != "") {
        $.messager.confirm('提示', '确定要删除该考核版本及该版本下的所有考核项？', function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: "AssessmentVersionDefine.aspx/DeleteAssessmentVersion",
                    data: "{mKeyId:'" + editContrastId + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        var myData = msg.d;
                        if (myData >= 1) {
                            $.messager.alert('提示', '删除成功！');
                        } else {
                            $.messager.alert('提示', '删除失败！');
                        }
                        refreshFun();
                    },
                    error: function () {
                        $.messager.alert('提示', '操作失败！');
                    }
                });
            }
        });
    }else {
        $.messager.alert('提示', '请选择考核项！');
    }     
}
function reserchFun(type, editContrastId) {
    meditContrastId = editContrastId;
    if (type == "first") {
        $('#grid_Main').datagrid('selectRecord', editContrastId);
        var data = $('#grid_Main').datagrid('getSelected');
        $('#assessmentName').textbox('setText', data.Name);
        myOrganizationId = data.OrganizationID;
    }
    $.ajax({
        type: "POST",
        url: "AssessmentVersionDefine.aspx/GetAssessmentVersionDetail",
        data: " {mKeyId:'" + editContrastId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                LoadMainDataGridDetail("last", []);
                $.messager.alert('提示', '该考核版本下没有考核项目！');
            } else {
                LoadMainDataGridDetail("last", myData);
            }
        },
        error: function () {
            $.messager.alert('提示', '加载失败！');
        }
    });
}
var myOrganizationId = "";
function LoadProcessLine(mValue) {
    $.ajax({
        type: "POST",
        url: "AssessmentVersionDefine.aspx/GetProcessLine",
        data: " {mOrganizationId:'" + mValue + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#eProcessLine').combobox({
                valueField: 'OrganizationID',
                textField: 'Name',
                panelHeight: 'auto',
                data: myData.rows,
                onSelect: function (record) {
                    myOrganizationId = record.OrganizationID;
                    ///加载变量
                     LoadAssessmentObjects(myOrganizationId);
                }
            });
        },
        error: function () {
            $.messager.alert('失败', '产线加载失败！');
        }
    });
}
function LoadAssessmentObjects(organizationId)
{  
    $.ajax({
        type: "POST",
        url: "AssessmentVersionDefine.aspx/GetAssessmentObjects",
        data: "{myType:'" + myType + "',myValueType:'" + myValueType + "',myOrganizationId:'" + organizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#eAssessment').combotree({
                //valueField: 'id',
                valueField: 'variableId',
                textField: 'text',
                //valueField: 'OrganizationID',
                //textField: 'Name',
                panelHeight: 'auto',
                data: myData
            });
           // $('#eAssessment').combotree('tree').tree("collapseAll");
        },
        error: function () {
            $.messager.alert('提示', '加载失败！');
        }
    });
}
function addDetailFun() {
    editDetailFun(false);
}
function refreshDetailFun() {
     var myKeyId= meditContrastId;
     reserchFun("last",myKeyId);
}
function deleteDetailFun(editContrastId) {
    //删除考核项
    if (editContrastId != "") {
        $.messager.confirm('提示', '确定要删除该考核项？', function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: "AssessmentVersionDefine.aspx/DeleteAssessmentDetail",
                    data: "{mId:'" + editContrastId + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        var myData = msg.d;
                        if (myData >= 1) {
                            $.messager.alert('提示', '删除成功！');
                        } else {
                            $.messager.alert('提示', '删除失败！');
                        }
                        refreshDetailFun();
                    },
                    error: function () {
                        $.messager.alert('提示', '操作失败！');
                    }
                });
            }
        }
    )}
}
function editDetailFun(IsEdit, editContrastId) {
    if (IsEdit) {
        IsAddDetail = false;  //编辑
        $('#grid_MainDetail').datagrid('selectRecord', editContrastId);
        var data = $('#grid_MainDetail').datagrid('getSelected');
        //
        eId = data.Id;
        // $('#eAssessmentObject').combobox('setValue', data.AssessmentId);
        $('#eAssessmentObject').combobox('select', data.AssessmentId);
        $('#eProcessLine').combobox('setText', data.OrganizationName); 
       
        eAssessmentObjectId = data.AssessmentId;
        eAssessmentName = data.AssessmentName;
        eObjectId = data.ObjectId;
        eObjectName = data.ObjectName;
        $('#eAssessment').val(data.ObjectName);
        eProcessLine = data.OrganizationID;
        LoadAssessmentObjects(eProcessLine);
    
          
        $('#eWeightedValue').numberbox('setValue', data.WeightedValue);
        eWeightedValue = data.WeightedValue;
        $('#eBestValue').numberbox('setValue', data.BestValue);
        eBestValue = data.BestValue;
        $('#eWorstValue').numberbox('setValue', data.WorstValue);
        eWorstValue = data.WorstValue;
        $('#eStandardValue').numberbox('setValue', data.StandardValue);
        eStandardValue = data.WorstValue;
        $('#eStandardScore').numberbox('setValue', data.StandardScore);
        eStandardScore = data.WorstValue;
        $('#eScoreFactor').numberbox('setValue', data.ScoreFactor);
        eScoreFactor = data.WorstValue;
        $('#eMaxScore').numberbox('setValue', data.MaxScore);
        eMaxScore = data.WorstValue;
        $('#eMinScore').numberbox('setValue', data.MinScore);
        eMinScore = data.WorstValue;
        $('#eEnabled').combobox('setValue', data.Enabled);
        eEnabled = data.Enabled;
    } else {
        IsAddDetail = true;
        //初始化
        $('#eAssessmentObject').combobox('clear');
        eAssessmentObjectId = "";
        eAssessmentName = "";
        $('#eAssessment').combotree('setText',"");
        eObjectId = "";
        eObjectName = "";
        $('#eProcessLine').combobox('setText', "");
        eProcessLine = "";
        $('#eWeightedValue').numberbox('setValue', "");
        eWeightedValue = "";
        $('#eBestValue').numberbox('setValue',"");
        eBestValue = "";
        $('#eWorstValue').numberbox('setValue',"");
        eWorstValue = "";

        $('#eStandardValue').numberbox('setValue',"");
        eStandardValue = "";
        $('#eStandardScore').numberbox('setValue', "100");
        eStandardScore = "100";
        $('#eScoreFactor').numberbox('setValue', "");
        eScoreFactor = "";
        $('#eMaxScore').numberbox('setValue', "200");
        eMaxScore = "200";
        $('#eMinScore').numberbox('setValue',"0");
        eMinScore = "0";

        $('#eEnabled').combobox('setValue','True');
        eEnabled = 'True';
        if (meditContrastId == "") {
            $.messager.alert('提示', '请选择考核项！');
        }
    }
    $('#AddandEditorDetail').window('open');
}
function saveDetail() {

   // myOrganizationId   meditContrastId
    //eAssessmentObject  combobox
    //获取参数
    eAssessmentObjectId = $('#eAssessmentObject').combobox('getValue');
    eAssessmentName = $('#eAssessmentObject').combobox('getText');
    //eAssessment   textbox
    var t = $('#eAssessment').combotree('tree').tree('getSelected');
    
    eObjectId = t.VariableId;
    eObjectName = t.text;
    //参数
    eWeightedValue = $('#eWeightedValue').numberbox('getValue');
    eBestValue = $('#eBestValue').numberbox('getValue');
    eWorstValue = $('#eWorstValue').numberbox('getValue');
    eStandardValue = $('#eStandardValue').numberbox('getValue');
    eStandardScore = $('#eStandardScore').numberbox('getValue');
    eScoreFactor = $('#eScoreFactor').numberbox('getValue');
    eMaxScore = $('#eMaxScore').numberbox('getValue');
    eMinScore = $('#eMinScore').numberbox('getValue');
    eEnabled = $('#eEnabled').combobox('getValue');

    if (meditContrastId == "") {
        $.messager.alert('提示', '请选择考核项！');
    } else {
        if (eAssessmentObjectId == "" || eObjectId == "" || eWeightedValue == "" || eBestValue == "" || eWorstValue == "" || eEnabled == "") {
            $.messager.alert('提示', '请选择未填项！');
        }
        var mUrl = "";
        var mData = "";
        if (IsAddDetail) {
            mUrl = "AssessmentVersionDefine.aspx/AddAssessmentDetail";
            mData = "{mOrganizationID:'" + myOrganizationId+ "',mKeyId:'" + meditContrastId+ "',mAssessmentId:'" + eAssessmentObjectId+ "',mAssessmentName:'" + eAssessmentName
                + "',mObjectId:'" + eObjectId
                + "',mObjectName:'" + eObjectName
                + "',mWeightedValue:'" + eWeightedValue
                + "',mBestValue:'" + eBestValue
                + "',mWorstValue:'" + eWorstValue
                + "',mStandardValue:'" + eStandardValue
                + "',mStandardScore:'" + eStandardScore
                + "',mScoreFactor:'" + eScoreFactor
                + "',mMaxScore:'" + eMaxScore
                + "',mMinScore:'" + eMinScore
                + "',mEnabled:'" + eEnabled + "'}";
        } else {
            mUrl = "AssessmentVersionDefine.aspx/UptateGetAssessmentDetail";
            mData = "{mOrganizationID:'" + myOrganizationId + "',mKeyId:'" + meditContrastId + "',mAssessmentId:'" + eAssessmentObjectId + "',mAssessmentName:'" + eAssessmentName + "',mObjectId:'" + eObjectId + "',mObjectName:'" + eObjectName
                + "',mWeightedValue:'" + eWeightedValue
                + "',mBestValue:'" + eBestValue
                + "',mWorstValue:'" + eWorstValue
                + "',mStandardValue:'" + eStandardValue
                + "',mStandardScore:'" + eStandardScore
                + "',mScoreFactor:'" + eScoreFactor
                + "',mMaxScore:'" + eMaxScore
                + "',mMinScore:'" + eMinScore
                + "',mEnabled:'" + eEnabled
                + "',mId:'" + eId + "'}";
        }
        $.ajax({
            type: "POST",
            url: mUrl,
            data: mData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var myData = msg.d;
                if (myData >= 1) {
                    if (IsAddDetail) {
                        $.messager.alert('提示', '添加成功！');
                    } else {
                        $.messager.alert('提示', '编辑成功！');
                    }
                } else {
                    $.messager.alert('提示', '操作失败！');
                }
                $('#AddandEditorDetail').window('close');
                refreshDetailFun();
            },
            error: function () {
                $.messager.alert('提示', '操作失败！');
            }
        });
    }
 
}

