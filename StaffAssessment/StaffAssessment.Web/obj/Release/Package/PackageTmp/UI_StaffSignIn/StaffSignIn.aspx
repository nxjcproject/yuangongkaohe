 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffSignIn.aspx.cs" Inherits="StaffAssessment.Web.UI_StaffSignIn.StaffSignIn" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>员工签到</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script> 
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 

    <script type="text/javascript" src="js/page/StaffSignIn.js" charset="utf-8"></script>
</head>
<body>
 <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width: 230px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <!-- 图表开始 -->
        <div id="toolbar_ReportTemplate" style=" height: 58px; padding-top: 10px">                    
                        <table>
                            <tr>
                                <td >组织机构：</td>
                                <td><input id="productLineName" class="easyui-textbox" style="width: 80px;" readonly="readonly" />
                                    <input id="organizationId" readonly="readonly" style="display: none;" />
                                </td>
                                <td>签到日期：</td>
                                <td>
                                    <input id="mTime1" class="easyui-combobox" data-options="panelHeight:'auto'" style="width: 100px;" />
                                </td> 
                                <td>岗&nbsp &nbsp &nbsp 位：</td> 
	    			            <td>
                                    <input class="easyui-combobox" type="text" id="workingSection" style="width:120px" />                  
	    			            </td>
                                </tr>
                        <tr>
                                <td>班 &nbsp &nbsp &nbsp 组：</td>
	    			            <td>
                                    <select class="easyui-combobox" id="workingTeam"data-options="panelHeight:'auto'" name="workingTeam" style="width:80px">
                                        <option value="全部">全部</option><option value="A">A组</option><option value="B">B组</option>
                                        <option value="C">C组</option><option value="D">D组</option><option value="常白">常白班</option>
                                    </select>
	    			            </td>               
                                <td>员 &nbsp &nbsp &nbsp 工：</td>
                                <td>
                                    <input id="Staff" class="easyui-combobox" data-options="panelHeight:'auto'" style="width: 90px;" />
                                </td>                                                                        
                                <td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                    onclick="Query();">签到列表</a>
                                </td>    
                           </tr>
                       </table>                    
        </div>
        <div id="reportTable" data-options="region:'center', border:true, collapsible:true, split:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
                <div data-options="region:'north',split:true"style="height:240px">
                     <table id="Windows_Report"></table>
                </div>
                <div data-options="region:'center',title:'签到历史查询',split:true">
                        <div id="toorBar" title="" style="height:40px;padding:10px;">
                            <table>
                                <tr>
                                    <td>  
                                        <table>
                                            <tr>
                                                <td>开始时间：</td>
                                                <td>
                                                        <input id="startTime" type="text" class="easyui-datebox" style="width:100px;" required="required"/>
                                                </td>
                                                    <td>结束时间：</td>
                                                    <td>
                                                        <input id="endTime" type="text" class="easyui-datebox" style="width:100px;" required="required"/>
                                                </td>
                     
                                                    <td>员工:</td>
                                                <td >                               
                                                    <input id="DownStaff" class="easyui-combobox" style="width:90px" />               
                                                </td>              
                                                    <td>
                                                    <a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="historyQuery()">查询</a>
                                                </td>
                                            </tr>
                                        </table> 
                                    </td>
                                </tr>  
                            </table>
                      </div>
                      <table id="grid_Main"class="easyui-datagrid"></table>
                </div>
                </div>
        </div>
     </div>
</body>
</html>
