//For Common
var count = 0;
$(document).ready(function () {
    var date = new Date();
    var currentMonth = date.getMonth();
    var currentDate = date.getDate();
    var currentYear = date.getFullYear();
    var sort_order = new Array();

    //bindSection();
    $("#section_panel").hide();


    BindCategory();

    //Sort1();
    //Sort2();
    //Sort3();
    //Sort4();
    //Sort5();
    Sort7();
    Sort8();

    bindSectionAll();
    bindVendorAll();


    $("#txt_Project").multiselect(
        {
            header: "Select Project(s)!",
            close: function () {
                var url = window.location.href
                BindCategoryForBudgetReport(url);

                BindCategoryForStatusReport(url);

                BindCategoryForPurchaseReport(url);
            },
            click: function (e) {
                if ($(this).multiselect("widget").find("input:checked").length > 3) {
                    e.preventDefault();
                    notify("Select maximum three projects!")
                    return false;
                }
            }

        }).multiselectfilter();
    var width = '100%';
    $('.ui-multiselect').css('width', width);
    //$('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-state-default').css('color', '#C31001');
    $('.ui-multiselect').css('height', '34px');
    $('.ui-multiselect').css('margin-top', '5px');

    $('#txt_FromDate').bind('copy paste cut', function (e) {
        e.preventDefault();
    });

    $('#txt_ToDate').bind('copy paste cut', function (e) {
        e.preventDefault();
    });






});


function IsNumber(e) {
    var keyCode = e.which ? e.which : e.keyCode
    if ((keyCode > 47 && keyCode < 58) || (keyCode == 8) || (keyCode == 46)) {
        return true;
    }
    else {
        alert('Use Only Numeric value');
        return false;
    }
}
function bindSection(project_id) {


    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/getAllProjectSectionByProjectID';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {

        url1 = 'Reports/getAllProjectSectionByProjectID';
    }
    $.ajax({
        type: "POST",
        url: url1,
        async: false,
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        data: "{ 'projectid': '" + project_id + "'}",
        success: function (data) {
            var sections = JSON.parse(data);
            var total_section = sections.data.length;


            if (count > 0) {
                //$("#txt_Section").multiselect('destroy');

                count++;

            }
            else if (count == 0) {
                count++;
            }

            var html = '';
            for (i = 0; i < total_section; i++) {

                var option = '<option value="' + sections.data[i].id + '" data-id="' + sections.data[i].id + '">' + sections.data[i].Sectionnumber + '-' + sections.data[i].sectionname + '</option>';
                html += option;


            }
            $("#txt_Section").html(html);

            $("#txt_Section").multiselect(
                {

                    click: function (e) {
                        //if ($(this).multiselect("widget").find("input:checked").length > 3) {
                        //    e.preventDefault();
                        //    notify("Select maximum three sections!")
                        //    return false;
                        //}
                    }
                }
                ).multiselectfilter();
            $("#txt_Section").multiselect("refresh");
            var width = '100%';
            $('.ui-multiselect').css('width', width);
            //$('.ui-multiselect-menu').css('width', '39.5%');
            $('.ui-multiselect-menu').css('width', '39.5%');
            $('.ui-state-default').css('color', '#C31001');
            $('.ui-multiselect').css('height', '34px');
            $('.ui-multiselect').css('margin-top', '5px');


        },
        error: function () {
            notify('Something went wrong.');
        }
    });

}

function bindVendor(project_id) {
    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/getAllVendorByProjectID';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {

        url1 = 'Reports/getAllVendorByProjectID';
    }
    $.ajax({
        type: "POST",
        url: url1,
        async: false,
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        data: "{ 'projectid': '" + project_id + "'}",
        success: function (data) {
            var vendors = JSON.parse(data);
            var total_vendor = vendors.data.length;
            if (count > 1) {
                //$("#txt_Vendor").multiselect('destroy')

            }
            else if (count == 1) {
                count++;
            }

            var html = '';
            for (i = 0; i < total_vendor; i++) {
                var option = '<option value="' + vendors.data[i].id + '" data-id="' + vendors.data[i].id + '">' + vendors.data[i].vendorname + '( ' + vendors.data[i].parentvendor + ' )' + '</option>';
                html += option;

            }
            $("#txt_Vendor").html(html);
            $("#txt_Vendor").multiselect(
                {
                    //header: "Select Vendors!",
                    click: function (e) {
                        //if ($(this).multiselect("widget").find("input:checked").length > 3) {
                        //    e.preventDefault();
                        //    notify("Select maximum three vendors!")
                        //    return false;
                        //}
                    }
                }).multiselectfilter();

            $("#txt_Vendor").multiselect("refresh");
            var width = '100%';
            $('.ui-multiselect').css('width', width);
            //$('.ui-multiselect-menu').css('width', '39.5%');
            $('.ui-multiselect-menu').css('width', '39.5%');
            $('.ui-state-default').css('color', '#C31001');
            $('.ui-multiselect').css('height', '34px');
            $('.ui-multiselect').css('margin-top', '5px');
        },
        error: function () {
            notify('Something went wrong.');
        }
    });

}

function bindVendorAll() {
    var url1 = '';
    var project_id = $("#txt_Project").val();

    if (project_id != null) {
        if (LOCALORSERVERNEW == 'LOCAL') {
            url1 = '/Reports/getAllVendorByProjectID';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {

            url1 = 'Reports/getAllVendorByProjectID';
        }
        $.ajax({
            type: "POST",
            url: url1,
            async: false,
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            data: "{ 'projectid': '" + project_id + "'}",
            success: function (data) {
                var vendors = JSON.parse(data);
                var total_vendor = vendors.data.length;


                if (count > 1) {
                    //$("#txt_Vendor1").multiselect('destroy')

                }


                var html = '';
                for (i = 0; i < total_vendor; i++) {
                    var option = '<option value="' + vendors.data[i].id + '" data-id="' + vendors.data[i].id + '">' + vendors.data[i].vendorname + '( ' + vendors.data[i].parentvendor + ' )' + '</option>';
                    html += option;

                }
                $("#txt_Vendor1").html(html);
                $("#txt_Vendor1").multiselect(
                    {
                        header: "Select Vendors!",
                        click: function (e) {
                            //if ($(this).multiselect("widget").find("input:checked").length > 3) {
                            //    e.preventDefault();
                            //    notify("Select maximum three vendors!")
                            //    return false;
                            //}
                        },
                        customheader:"ALL"
                    }).multiselectfilter();
                var width = '100%';
                $('.ui-multiselect').css('width', width);
                //$('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-state-default').css('color', '#C31001');
                $('.ui-multiselect').css('height', '34px');
                $('.ui-multiselect').css('margin-top', '5px');
            },
            error: function () {
                notify('Something went wrong.');
            }
        });
    }
    else {
        if (LOCALORSERVERNEW == 'LOCAL') {
            url1 = '/Reports/getAllVendors';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {

            url1 = 'Reports/getAllVendors';
        }
        $.ajax({
            type: "POST",
            url: url1,
            async: false,
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var vendors = JSON.parse(data);
                var total_vendor = vendors.data.length;


                if (count > 1) {
                    //$("#txt_Vendor1").multiselect('destroy')

                }


                var html = '';
                for (i = 0; i < total_vendor; i++) {
                    var option = '<option value="' + vendors.data[i].id + '" data-id="' + vendors.data[i].id + '">' + vendors.data[i].vendorname + '( ' + vendors.data[i].parentvendor + ' )' + '</option>';
                    html += option;

                }
                $("#txt_Vendor1").html(html);
                $("#txt_Vendor1").multiselect(
                    {
                        
                        click: function (e) {
                            //if ($(this).multiselect("widget").find("input:checked").length > 3) {
                            //    e.preventDefault();
                            //    notify("Select maximum three vendors!")
                            //    return false;
                            //}
                        },
                        customheader: "ALL",
                        header:"Select Vendor(s)"
                    }).multiselectfilter();
                var width = '100%';
                $('.ui-multiselect').css('width', width);
                //$('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-state-default').css('color', '#C31001');
                $('.ui-multiselect').css('height', '34px');
                $('.ui-multiselect').css('margin-top', '5px');
            },
            error: function () {
                notify('Something went wrong.');
            }
        });
    }
}


function bindSectionAll() {
    var url1 = '';
    var project_id = $("#txt_Project").val();

    if (project_id != null) {
        if (LOCALORSERVERNEW == 'LOCAL') {
            url1 = '/Reports/getAllProjectSectionByProjectID';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {

            url1 = 'Reports/getAllProjectSectionByProjectID';
        }

        $.ajax({
            type: "POST",
            url: url1,
            async: false,
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            data: "{ 'projectid': '" + project_id + "'}",
            success: function (data) {
                var sections = JSON.parse(data);
                var total_section = sections.data.length;


                if (count > 1) {
                    //$("#txt_Section1").multiselect('destroy');

                }


                var html = '';
                for (i = 0; i < total_section; i++) {

                    var option = '<option value="' + sections.data[i].id + '" data-id="' + sections.data[i].id + '">' + sections.data[i].Sectionnumber + '-' + sections.data[i].sectionname + '</option>';
                    html += option;


                }
                $("#txt_Section1").html(html);

                $("#txt_Section1").multiselect(
                    {
                        
                        customheader: "ALL",
                        click: function (e) {
                            //if ($(this).multiselect("widget").find("input:checked").length > 3) {
                            //    e.preventDefault();
                            //    notify("Select maximum three sections!")
                            //    return false;
                            //}
                        },
                        header: "Select Section(s)"
                    }
                    ).multiselectfilter();
                var width = '100%';
                $('.ui-multiselect').css('width', width);
                //$('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-state-default').css('color', '#C31001');
                $('.ui-multiselect').css('height', '34px');
                $('.ui-multiselect').css('margin-top', '5px');
            },
            error: function () {
                notify('Something went wrong.');
            }
        });
    }
    else {
        if (LOCALORSERVERNEW == 'LOCAL') {
            url1 = '/Reports/getAllProjectSections';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {

            url1 = 'Reports/getAllProjectSections';
        }
        $.ajax({
            type: "POST",
            url: url1,
            async: false,
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var sections = JSON.parse(data);
                var total_section = sections.data.length;

                if (count > 1) {
                    //$("#txt_Section1").multiselect('destroy')

                }
                else if (count == 3) {
                    count++;
                }

                var html = '';
                for (i = 0; i < total_section; i++) {

                    var option = '<option value="' + sections.data[i].id + '" data-id="' + sections.data[i].id + '">' + sections.data[i].Sectionnumber + '-' + sections.data[i].sectionname + '</option>';
                    html += option;


                }
                $("#txt_Section1").html(html);

                $("#txt_Section1").multiselect(
                    {
                        customheader:"ALL",
                        click: function (e) {
                            //if ($(this).multiselect("widget").find("input:checked").length > 3) {
                            //    e.preventDefault();
                            //    notify("Select maximum three sections!")
                            //    return false;
                            //}
                        },
                        header:"Select Section(s)"
                    }
                    ).multiselectfilter();
                var width = '100%';
                $('.ui-multiselect').css('width', width);
                //$('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-state-default').css('color', '#C31001');
                $('.ui-multiselect').css('height', '34px');
                $('.ui-multiselect').css('margin-top', '5px');
            },
            error: function () {
                notify('Something went wrong.');
            }
        });
    }


}

function deleteorder(obj, orderid, pos) {
    // $(obj).parent().remove();

    var isconfirmed = confirm("Do you really want to delete?");
    if (!isconfirmed) {
        return;
    }

    if (window.location.href.indexOf("/Reports/StatusReport") > -1) {



        if ($("#txtStatusReportCategory").val() != "5" && $("#txtStatusReportCategory").val() != "6") {
            $(".sort_div:eq(" + pos + ")").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == orderid && $(this).attr("data-id") != typeof (undefined)) {
                    $(this).remove();
                    $("#DDl_Sort1").find("option").each(function (index, item) {

                        if ($(this).attr("data-id") == orderid) {
                            $(this).removeAttr('selected');

                        }

                    });
                    $("#DDl_Sort1").multiselect("destroy");
                    Sort1(-1);
                }
            });
        }

        else if ($("#txtStatusReportCategory").val() == "5") {

            $(".sort_div:eq(4)").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == orderid && $(this).attr("data-id") != typeof (undefined)) {
                    $(this).remove();
                    $("#DDl_Sort5").find("option").each(function (index, item) {

                        if ($(this).attr("data-id") == orderid) {
                            $(this).removeAttr('selected');

                        }

                    });
                    $("#DDl_Sort5").multiselect("destroy");
                    Sort5(-1);
                }
            });

        }

        else if ($("#txtStatusReportCategory").val() == "6") {


            $(".sort_div:eq(5)").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == orderid && $(this).attr("data-id") != typeof (undefined)) {
                    $(this).remove();
                    $("#DDl_Sort2").find("option").each(function (index, item) {

                        if ($(this).attr("data-id") == orderid) {
                            $(this).removeAttr('selected');

                        }

                    });
                    $("#DDl_Sort2").multiselect("destroy");
                    Sort2(-1);
                }
            });

        }
    }

    if (window.location.href.indexOf("/Reports/BudgetReport") > -1) {
        Sort3();
        Sort4();
    }

    if (window.location.href.indexOf("/Reports/PurchaseReport") > -1) {
       
        Sort4ForPurchaseReport();
    }

    if (window.location.href.indexOf("/Reports/PrintedChequeList") > -1) {
        $(".sort_div").find("span").each(function (index, item) {
            if ($(this).attr("data-id") == orderid && $(this).attr("data-id") != typeof (undefined)) {
                $(this).remove();
                $("#DDl_Sort8").find("option").each(function (index, item) {

                    if ($(this).attr("data-id") == orderid) {
                        $(this).removeAttr('selected');

                    }

                });
                $("#DDl_Sort8").multiselect("refresh");
                var width = '100%';
                $('.ui-multiselect').css('width', width);
                //$('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-multiselect-menu').css('width', '39.5%');
                $('.ui-state-default').css('color', '#C31001');
                $('.ui-multiselect').css('height', '34px');
                $('.ui-multiselect').css('margin-top', '5px');

            }
        });
       
    }

    //$(".sort_div:eq(0)").find("span").each(function (index, item) {

    //    if ($(this).attr("data-id") == orderid) {
    //        $(this).remove();

    //        //.removeAttr("selected");

    //        //$("#DDl_Sort1").multiselect("destroy");
    //        $("#DDl_Sort1").find("option").each(function (index, item) {

    //            if ($(this).attr("data-id") == orderid) {
    //                $(this).removeAttr('selected');

    //            }

    //        });
    //        Sort1(0);

    //        return;

    //        $("#DDl_Sort2").multiselect("destroy");
    //        $("#DDl_Sort2").find("option").each(function (index, item) {

    //            if ($(this).attr("data-id") == orderid) {
    //                $(this).removeAttr('selected');

    //            }

    //        });
    //        Sort2();

    //        $("#DDl_Sort3").multiselect("destroy");
    //        $("#DDl_Sort3").find("option").each(function (index, item) {

    //            if ($(this).attr("data-id") == orderid) {
    //                $(this).removeAttr('selected');

    //            }

    //        });
    //        Sort3();

    //        $("#DDl_Sort4").multiselect("destroy");
    //        $("#DDl_Sort4").find("option").each(function (index, item) {

    //            if ($(this).attr("data-id") == orderid) {
    //                $(this).removeAttr('selected');

    //            }

    //        });
    //        Sort4();

    //        $("#DDl_Sort5").multiselect("destroy");
    //        $("#DDl_Sort5").find("option").each(function (index, item) {

    //            if ($(this).attr("data-id") == orderid) {
    //                $(this).removeAttr('selected');

    //            }

    //        });
    //        Sort5();

    //        $("#DDl_Sort7").multiselect("destroy");
    //        $("#DDl_Sort7").find("option").each(function (index, item) {

    //            if ($(this).attr("data-id") == orderid) {
    //                $(this).removeAttr('selected');

    //            }

    //        });
    //        Sort7();

    //        $("#DDl_Sort8").multiselect("destroy");
    //        $("#DDl_Sort8").find("option").each(function (index, item) {

    //            if ($(this).attr("data-id") == orderid) {
    //                $(this).removeAttr('selected');

    //            }

    //        });
    //        Sort8();



    //    }

    //});






}

function errorfunc(obj, message) {

    $('#' + obj).val($('#' + obj).val().trim());
    $('#' + obj).attr('placeholder', message);

    $('#' + obj).effect("shake", { times: 2 }, 700);

    //window.setInterval(function () { $('#' + obj).attr('placeholder', ''); }, 5000);


    $('#' + obj).focus();

}




// At Risk By Item Start

function AtRiskByItemReportGenerationByFilter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var AllVendorID = $('#txt_Vendor').val();
    var OrderPattern = "";
    var Status = $("#ddl_ItemStatus").val();




    var From_Date;
    var To_date;

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if ($("#txt_FromDate").val() != '')
        From_Date = ($("#txt_FromDate").val());
    else
        From_Date = null;
    if ($("#txt_ToDate").val() != '')
        To_date = ($("#txt_ToDate").val());
    else
        To_date = null;

    if (AllSectionID == null) {
        AllSectionID = "0";
    }

    if (AllVendorID == null) {
        AllVendorID = "0";
    }

    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += 'PPLI.POLineItemID' + ",";
            if (orderIndex == "2")
                OrderPattern += 'PPO.PurchaseOrderId' + ",";
            if (orderIndex == "3")
                OrderPattern += 'PPO.ArrivalDate' + ",";

        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/AtRiskByItemReportGenerationByFiltter';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/AtRiskByItemReportGenerationByFiltter';
    }
    url1 += "?AllProjectID=" + AllProjectID;
    url1 += "&AllSectionID=" + AllSectionID;
    url1 += "&AllVendorID=" + AllVendorID;
    url1 += "&Status=" + Status;
    url1 += "&StartDate=" + From_Date;
    url1 += "&EndDate=" + To_date;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}

// At Risk By Item End


// Status By Vendor Start

function ExpeditNoteReportGenerationFilter() {
    var AllProjectID = $('#txt_Project').val();
    var OrderPattern = "";
    var Status = $("#ddl_ItemStatus").val();

    var From_Date;
    var To_date;

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if ($("#txt_FromDate").val() != '')
        From_Date = ($("#txt_FromDate").val());
    else
        From_Date = null;
    if ($("#txt_ToDate").val() != '')
        To_date = ($("#txt_ToDate").val());
    else
        To_date = null;



    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
            if (orderIndex == "2")
                OrderPattern += '2' + ",";
            if (orderIndex == "3")
                OrderPattern += '3' + ",";

        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/StatusByVendorGenerationFIlter';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/StatusByVendorGenerationFIlter';
    }
    url1 += "?AllProjectID=" + AllProjectID;
    url1 += "&Status=" + Status;
    url1 += "&StartDate=" + From_Date;
    url1 += "&EndDate=" + To_date;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }

           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}
// Status By Vendor End


////Budget Report By Item Start
//function BudgetReportByItemReportGenerationFilter() {
//    $("#Btn_Report").html('<i class="fa fa-spinner"></i>');
//    var AllProjectID = $('#txt_Project').val();
//    var AllSectionID = $('#txt_Section').val();
//    var OrderPattern = "";

//    if (AllProjectID == null) {
//        notify('Please Select Project');
//        return;
//    }

//    if (AllSectionID == null) {
//        AllSectionID = "0";
//    }


//    //$(".sort_div").find("span").each(function (index, item) {

//    //    var orderIndex = $(this).attr("data-id");

//    //    if (typeof (orderIndex) != 'undefined') {

//    //        if (orderIndex == "1")
//    //            OrderPattern += 'Data1.Vendor' + ",";
//    //        if (orderIndex == "2")
//    //            OrderPattern += 'Data1.PurchaseOrderNumber' + ",";
//    //        if (orderIndex == "3")
//    //            OrderPattern += 'Data1.POLineItemID' + ",";

//    //    }

//    //});

//    if (OrderPattern == "") {
//        OrderPattern = "0";
//    }
//    else {

//        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
//    }

//    var url1 = '';
//    if (LOCALORSERVERNEW == 'LOCAL') {
//        url1 = '/Reports/BudgetReportByItemReportGeneration';
//    }
//    else if (LOCALORSERVERNEW == 'SERVER') {
//        url1 = 'Reports/BudgetReportByItemReportGeneration';
//    }
//    url1 += "?ProjectID=" + AllProjectID;
//    url1 += "&SectionID=" + AllSectionID;
//    url1 += "&order=" + OrderPattern;


//    var totdata = '';
//    $.get(url1)
//       .success(function (response) {
//           var html = "";
//           var url2 = '';
//           if (response == '0') {
//               notify('No records found!!!');
//               return;
//           }
//           if (LOCALORSERVERNEW == 'LOCAL') {
//               totdata = response;
//           }
//           else if (LOCALORSERVERNEW == 'SERVER') {
//               totdata = "POMS/" + response;
//           }


//           setTimeout(function () {
//               window.open(totdata, '_blank');
//               $("#Btn_Report").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
//           }, 1000);
//       });
//}
////Budget Report By Item End


////Budget Report Summery Start
//function BudgetReportSummaryReportGenerationFilter() {
//    $(".btnReport:last").html('<i class="fa fa-spinner"></i>');
//    var AllProjectID = $('#txt_Project').val();
//    var AllSectionID = $('#txt_Section').val();
//    var OrderPattern = "";

//    if (AllProjectID == null) {
//        notify('Please Select Project');
//        return;
//    }

//    if (AllSectionID == null) {
//        AllSectionID = "0";
//    }


//    $(".sort_div").find("span").each(function (index, item) {

//        var orderIndex = $(this).attr("data-id");

//        if (typeof (orderIndex) != 'undefined') {

//            if (orderIndex == "1")
//                OrderPattern += '1';


//        }

//    });

//    if (OrderPattern == "") {
//        OrderPattern = "0";
//    }


//    var url1 = '';
//    if (LOCALORSERVERNEW == 'LOCAL') {
//        url1 = '/Reports/BudgetReportSummaryReportGeneration';
//    }
//    else if (LOCALORSERVERNEW == 'SERVER') {
//        url1 = 'Reports/BudgetReportSummaryReportGeneration';
//    }
//    url1 += "?ProjectID=" + AllProjectID;
//    url1 += "&SectionID=" + AllSectionID;
//    url1 += "&order=" + OrderPattern;


//    var totdata = '';
//    $.get(url1)
//       .success(function (response) {
//           var html = "";
//           var url2 = '';
//           if (response == '0') {
//               notify('No records found!!!');
//               return;
//           }
//           if (LOCALORSERVERNEW == 'LOCAL') {
//               totdata = response;
//           }
//           else if (LOCALORSERVERNEW == 'SERVER') {
//               totdata = "POMS/" + response;
//           }


//           setTimeout(function () {
//               window.open(totdata, '_blank');
//               $(".btnReport:last").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
//           }, 1000);
//       });
//}
////Budget Report Summery End

//Commited Cost Report Summery Start



//Budget Report By Item Start1
function BudgetReportByItemReportGenerationFilter(isSSRSCall) {
    $("#Btn_Report").html('<i class="fa fa-spinner"></i>');
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var OrderPattern = "";

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if (AllSectionID == null) {
        AllSectionID = "0";
    }


    //$(".sort_div").find("span").each(function (index, item) {

    //    var orderIndex = $(this).attr("data-id");

    //    if (typeof (orderIndex) != 'undefined') {

    //        if (orderIndex == "1")
    //            OrderPattern += 'Data1.Vendor' + ",";
    //        if (orderIndex == "2")
    //            OrderPattern += 'Data1.PurchaseOrderNumber' + ",";
    //        if (orderIndex == "3")
    //            OrderPattern += 'Data1.POLineItemID' + ",";

    //    }

    //});

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/BudgetReportByItemReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/BudgetReportByItemReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&order=" + OrderPattern;
    url1 += "&isSSRSCall=" + isSSRSCall;


    var totdata = '';
    $.get(url1)
        .success(function (response) {
            var html = "";
            var url2 = '';
            if (response == "isSSRSCall") {
                window.open("../../SSRSRptViewer/ReportTemplate.aspx");
                if (isSSRSCall == "SSRS_PDF") {
                    //$("#Btn_Report").removeClass("fa fa-spinner");
                    //$("#Btn_Report").addClass("fa fa-file-pdf-o");
                    $("#Btn_Report").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
                }
                //  else {
                //    $("#ssrsbtn").removeClass("fa fa-spin fa-spinner");
                //    $("#ssrsbtn").addClass("fa fa-file-pdf-o");
                //}

                return;
            }
            if (response == '0') {
                notify('No records found!!!');
                //if (op == 1) {
                //    $("#pdfBtn").removeClass("fa fa-spin fa-spinner");
                //    $("#pdfBtn").addClass("fa fa-file-pdf-o");
                //}
                //else {
                //    $("#excelBtn").removeClass("fa fa-spin fa-spinner");
                //    $("#excelBtn").addClass("fa fa-file-excel-o");
                //}
                return;
            }
            if (LOCALORSERVERNEW == 'LOCAL') {
                totdata = response;
            }
            else if (LOCALORSERVERNEW == 'SERVER') {
                totdata = "POMS/" + response;
            }
            setTimeout(function () {
                window.open(urlchanger(totdata), '_blank', 'toolbar=0,location=0,menubar=0');
                // if (op == 1) {
                //    $("#Btn_Report").removeClass("fa fa-spin fa-spinner");
                //$("#Btn_Report").addClass("fa fa-file-pdf-o");

                $("#Btn_Report").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
                //}
                //else {
                //    $("#excelBtn").removeClass("fa fa-spin fa-spinner");
                //    $("#excelBtn").addClass("fa fa-file-excel-o");
                //}
            }, 1000);
        });
    //.success(function (response) {
    //    var html = "";
    //    var url2 = '';
    //    if (response == '0') {
    //        notify('No records found!!!');
    //        return;
    //    }
    //    if (LOCALORSERVERNEW == 'LOCAL') {
    //        totdata = response;
    //    }
    //    else if (LOCALORSERVERNEW == 'SERVER') {
    //        totdata = "POMS/" + response;
    //    }


    //    setTimeout(function () {
    //        window.open(totdata, '_blank');
    //        $("#Btn_Report").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
    //    }, 1000);
    //});
}
//Budget Report By Item End0


//Budget Report Summery Start0
function BudgetReportSummaryReportGenerationFilter(isSSRSCall) {
    $(".btnReport:last").html('<i class="fa fa-spinner"></i>');
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var OrderPattern = "";

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if (AllSectionID == null) {
        AllSectionID = "0";
    }


    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1';


        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }


    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/BudgetReportSummaryReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/BudgetReportSummaryReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&order=" + OrderPattern;
    url1 += "&isSSRSCall=" + isSSRSCall;


    var totdata = '';
    $.get(url1)
        .success(function (response) {
            var html = "";
            var url2 = '';
            if (response == "isSSRSCall") {
                window.open("../../SSRSRptViewer/ReportTemplate.aspx");
                if (isSSRSCall == "SSRS_PDF") {
                    $("#Btn_Report").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
                }


                return;
            }
            if (response == '0') {
                notify('No records found!!!');
                return;
            }
            if (LOCALORSERVERNEW == 'LOCAL') {
                totdata = response;
            }
            else if (LOCALORSERVERNEW == 'SERVER') {
                totdata = "POMS/" + response;
            }
            setTimeout(function () {
                window.open(urlchanger(totdata), '_blank', 'toolbar=0,location=0,menubar=0');
                $(".btnReport:last").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
            }, 1000);
        });
    //.success(function (response) {
    //    var html = "";
    //    var url2 = '';
    //    if (response == '0') {
    //        notify('No records found!!!');
    //        return;
    //    }
    //    if (LOCALORSERVERNEW == 'LOCAL') {
    //        totdata = response;
    //    }
    //    else if (LOCALORSERVERNEW == 'SERVER') {
    //        totdata = "POMS/" + response;
    //    }


    //    setTimeout(function () {
    //        window.open(totdata, '_blank');
    //        $(".btnReport:last").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
    //    }, 1000);
    //});
}
//Budget Report Summery End7




function CommittedCostReportGenerationFilter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var OrderPattern = "";

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if (AllSectionID == null) {
        AllSectionID = "0";
    }


    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/CommittedCostReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/CommittedCostReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}
//Committed Cost Report Summery End



//Commited Cost Report Details Start
function CommittedCostReportDetailReportGenerationFilter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var OrderPattern = "";

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if (AllSectionID == null) {
        AllSectionID = "0";
    }


    //$(".sort_div").find("span").each(function (index, item) {

    //    var orderIndex = $(this).attr("data-id");

    //    if (typeof (orderIndex) != 'undefined') {

    //        if (orderIndex == "1")
    //            OrderPattern += 'Data1.Vendor' + ",";
    //        if (orderIndex == "2")
    //            OrderPattern += 'Data1.PurchaseOrderNumber' + ",";
    //        if (orderIndex == "3")
    //            OrderPattern += 'Data1.POLineItemID' + ",";

    //    }

    //});

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/CommittedCostReportDetailReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/CommittedCostReportDetailReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}
//Committed Cost Report Details End


//Expadite Report By Item Start
function ExpediteReportByItemReportGenerationFilter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var OrderPattern = "";

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if (AllSectionID == null) {
        AllSectionID = "0";
    }


    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
            if (orderIndex == "2")
                OrderPattern += '2' + ",";
            if (orderIndex == "3")
                OrderPattern += '3' + ",";

        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/ExpediteReportByItemReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/ExpediteReportByItemReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}
//Expadite Report By Item  End

//Expadite Report By Status Start
function ExpediteReportByStatusReportGenerationFIlter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var OrderPattern = "";

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if (AllSectionID == null) {
        AllSectionID = "0";
    }


    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
            if (orderIndex == "2")
                OrderPattern += '2' + ",";
            if (orderIndex == "3")
                OrderPattern += '3' + ",";

        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/ExpediteReportByStatusReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/ExpediteReportByStatusReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}
//Expadite Report By Item  End


//Receiving Report By Section Start
function ReceivingReportSectionReportGenerationFilter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var AllVendorID = $('#txt_Vendor').val();
    var OrderPattern = "";

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if (AllSectionID == null) {
        AllSectionID = "0";
    }

    if (AllVendorID == null) {
        AllVendorID = "0";
    }


    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
            if (orderIndex == "2")
                OrderPattern += '2' + ",";
            if (orderIndex == "3")
                OrderPattern += '3' + ",";

        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/ReceivingReportSectionReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/ReceivingReportSectionReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&VendorID=" + AllVendorID;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}
//Receiving Report By Section End


//Purchase Commitment Report Start
function PurchaseCommitmentReportGenerationFilter() {
    $("#Btn_Report").html('<i class="fa fa-spinner"></i>');
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var OrderPattern = "";
    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }
    if (AllSectionID == null) {
        AllSectionID = "0";
    }
    //$(".sort_div").find("span").each(function (index, item) {
    //    var orderIndex = $(this).attr("data-id");
    //    if (typeof (orderIndex) != 'undefined') {
    //        if (orderIndex == "1")
    //            OrderPattern += 'Data1.Vendor' + ",";
    //        if (orderIndex == "2")
    //            OrderPattern += 'Data1.PurchaseOrderNumber' + ",";
    //        if (orderIndex == "3")
    //            OrderPattern += 'Data1.POLineItemID' + ",";
    //    }
    //});
    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {
        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }
    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/PurchaseCommitmentReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/PurchaseCommitmentReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&order=" + OrderPattern;
    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }
           setTimeout(function () {
               window.open(totdata, '_blank');
               $("#Btn_Report").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
           }, 1000);
       });
}
//Purchase Commitment Report  End


// Current Paid Report Start
function CurrentPaidReportReportGenerationFilter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var OrderPattern = "";

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if (AllSectionID == null) {
        AllSectionID = "0";
    }
    if ($("#txt_FromDate").val() != '')
        From_Date = ($("#txt_FromDate").val());
    else
        From_Date = null;
    if ($("#txt_ToDate").val() != '')
        To_date = ($("#txt_ToDate").val());
    else
        To_date = null;

    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
            if (orderIndex == "2")
                OrderPattern += '2' + ",";


        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/CurrentPaidReportReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/CurrentPaidReportReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&order=" + OrderPattern;
    url1 += "&StartDate=" + From_Date;
    url1 += "&EndDate=" + To_date;

    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}
//Current Paid Report End

// Project To Date Report Start

function ProjectToDateReportReportGenerationFilter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var AllVendorID = $('#txt_Vendor').val();
    var OrderPattern = "";

    var From_Date;
    var To_date;

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }

    if ($("#txt_FromDate").val() != '')
        From_Date = ($("#txt_FromDate").val());
    else
        From_Date = null;
    if ($("#txt_ToDate").val() != '')
        To_date = ($("#txt_ToDate").val());
    else
        To_date = null;

    if (AllSectionID == null) {
        AllSectionID = "0";
    }

    if (AllVendorID == null) {
        AllVendorID = "0";
    }

    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
            if (orderIndex == "2")
                OrderPattern += '2' + ",";
        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/ProjectToDateReportReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/ProjectToDateReportReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&VendorID=" + AllVendorID;
    url1 += "&StartDate=" + From_Date;
    url1 += "&EndDate=" + To_date;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}

// Project To Date Report End

//Print Cheque List Start

function PrintedChequeListReportGenerationFilter() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section1').val();
    var AllVendorID = $('#txt_Vendor1').val();
    var OrderPattern = "";
    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
            if (orderIndex == "2")
                OrderPattern += '2' + ",";

        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
        notify("Please Select Sorting Option!");
        return;
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var From_Date;
    var To_date;

    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }
    $("#Btn_Report").html('<i class="fa fa-spinner"></i>');
    if ($("#txt_FromDate").val() != '')
        From_Date = ($("#txt_FromDate").val());
    else
        From_Date = null;
    if ($("#txt_ToDate").val() != '')
        To_date = ($("#txt_ToDate").val());
    else
        To_date = null;

    if (AllSectionID == null) {
        AllSectionID = "0";
    }

    if (AllVendorID == null) {
        AllVendorID = "0";
    }

   
    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/PrintedChequeListReportGeneration';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/PrintedChequeListReportGeneration';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&VendorID=" + AllVendorID;
    url1 += "&StartDate=" + From_Date;
    url1 += "&EndDate=" + To_date;
    url1 += "&order=" + OrderPattern;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';
           if (response == '0') {
               notify('No records found!!!');
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
               $("#Btn_Report").html('<i class="fa fa-file-pdf-o"></i> Export to PDF ');
           }, 1000);
       });
}
//Print Cheque List End


// Expedite Note Start

function ExpeditNoteReportGenerationFilter1() {
    var AllProjectID = $('#txt_Project').val();
    var AllSectionID = $('#txt_Section').val();
    var AllVendorID = $('#txt_Vendor').val();
    var OrderPattern = "";
    var Status = $("#ddl_ItemStatus").val();
    var NoteStatus = $("#ddl_NoteStatus").val();
    var From_Date;
    var To_date;
    if (AllProjectID == null) {
        notify('Please Select Project');
        return;
    }
    if ($("#txt_FromDate").val() != '')
        From_Date = ($("#txt_FromDate").val());
    else
        From_Date = null;
    if ($("#txt_ToDate").val() != '')
        To_date = ($("#txt_ToDate").val());
    else
        To_date = null;

    if (AllSectionID == null) {
        AllSectionID = "0";
    }

    if (AllVendorID == null) {
        AllVendorID = "0";
    }

    $(".sort_div").find("span").each(function (index, item) {

        var orderIndex = $(this).attr("data-id");

        if (typeof (orderIndex) != 'undefined') {

            if (orderIndex == "1")
                OrderPattern += '1' + ",";
            if (orderIndex == "2")
                OrderPattern += '2' + ",";
            if (orderIndex == "3")
                OrderPattern += '3' + ",";

        }

    });

    if (OrderPattern == "") {
        OrderPattern = "0";
    }
    else {

        OrderPattern = OrderPattern.substr(0, (OrderPattern.length - 1));
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Reports/ExpeditNoteReportGenerationByVendor';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Reports/ExpeditNoteReportGenerationByVendor';
    }
    url1 += "?ProjectID=" + AllProjectID;
    url1 += "&SectionID=" + AllSectionID;
    url1 += "&VendorID=" + AllVendorID;
    url1 += "&Status=" + Status;
    url1 += "&StartDate=" + From_Date;
    url1 += "&EndDate=" + To_date;
    url1 += "&order=" + OrderPattern;
    url1 += "&NoteStatus=" + NoteStatus;


    var totdata = '';
    $.get(url1)
       .success(function (response) {
           var html = "";
           var url2 = '';

           if (response == "0") {
               notify("No Record Found!!!");
               return;
           }
           if (LOCALORSERVERNEW == 'LOCAL') {
               totdata = response;
           }
           else if (LOCALORSERVERNEW == 'SERVER') {
               totdata = "POMS/" + response;
           }


           setTimeout(function () {
               window.open(totdata, '_blank');
           }, 1000);
       });
}

// Expedite Note Start End


////////////////////////////////////////////////////Sorting Start/////////////////////////////////////////////////////

function Sort1(pos) {



    $("#DDl_Sort1").multiselect({
        close: function () {



        },


        checkAll: function () {

            

            $(".sort_div:eq(" + pos + ")").find("span").each(function (index, item) {
                $(this).remove();
            });

            if ($(this).val() != null) {
                var sortValue = $(this).val().toString().split(",");
                for (var i = 0; i < sortValue.length; i++) {
                    var html = '';
                    var text = '';
                    if (sortValue[i] == "1")
                        text += 'Item';
                    else if (sortValue[i] == "2")
                        text += "PO"
                    else if (sortValue[i] == "3")
                        text += "Arrival Date"
                    html += "<span class='remove' data-id='" + sortValue[i] + "' ><a onclick='deleteorder(this,\"" + sortValue[i] + "\"," + pos + ")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                    $(".sort_div:eq(" + pos + ")").append(html);

                }
            }
        },

        uncheckAll: function () {
            $(".sort_div").find("span").each(function (index, item) {
                $(this).remove();
            });

        },


        click: function (event, ui) {
            var html = '';
            var text = '';
            var exists = false;
            $(".sort_div").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == ui.value && !ui.checked) {
                    $(this).remove();
                    exists = true;
                }
                else if ($(this).attr("data-id") == ui.value && ui.checked) {
                    alert("Order already exists!!");
                    ui.checked = false;
                    exists = true;
                }

            });
            var pos1;
            if (window.location.href.indexOf("Reports/StatusReport") > -1)
            {
                pos1 = parseInt($("#txtStatusReportCategory").val()) - 1;
            }

            
            if (exists == false) {
                if (ui.value == "1")
                    text += 'Item';
                else if (ui.value == "2")
                    text += "PO"
                else if (ui.value == "3")
                    text += "Arrival Date"
                html += "<span class='remove' data-id='" + ui.value + "' ><a onclick='deleteorder(this,\"" + ui.value + "\"," + pos1 + ")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                $(".sort_div").append(html);

            }
        }

    });
    $("#DDl_Sort1").multiselect('refresh');
    if (pos != -1)
        $("#DDl_Sort1").multiselect('uncheckAll');
    var width = '100%';
    $('.ui-multiselect').css('width', width);
    //$('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-state-default').css('color', '#C31001');
    $('.ui-multiselect').css('height', '34px');
    $('.ui-multiselect').css('margin-top', '5px');




}

function Sort2(pos) {
    $("#DDl_Sort2").multiselect({
        close: function () {

        },

        checkAll: function () {



            $(".sort_div:eq(" + pos + ")").find("span").each(function (index, item) {
                $(this).remove();
            });

            if ($(this).val() != null) {
                var sortValue = $(this).val().toString().split(",");
                for (var i = 0; i < sortValue.length; i++) {
                    var html = '';
                    var text = '';
                    if (sortValue[i] == "1")
                        text += 'Vendor';
                    else if (sortValue[i] == "2")
                        text += "PO"
                    else if (sortValue[i] == "3")
                        text += "Item"
                    html += "<span class='remove' data-id='" + sortValue[i] + "' ><a onclick='deleteorder(this,\"" + sortValue[i] + "\"," + pos + ")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                    $(".sort_div:eq(" + pos + ")").append(html);

                }
            }
        },

        uncheckAll: function () {
            $(".sort_div").find("span").each(function (index, item) {
                $(this).remove();
            });

        },
        click: function (event, ui) {
            var html = '';
            var text = '';
            var exists = false;
            $(".sort_div").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == ui.value && !ui.checked) {
                    $(this).remove();
                    exists = true;
                }
                else if ($(this).attr("data-id") == ui.value && ui.checked) {
                    alert("Order already exists!!");
                    ui.checked = false;
                    exists = true;
                }

            });
            if (exists == false) {
                if (ui.value == "1")
                    text += 'Vendor';
                else if (ui.value == "2")
                    text += "PO"
                else if (ui.value == "3")
                    text += "Item"
                html += "<span class='remove' data-id='" + ui.value + "' ><a onclick='deleteorder(this,\"" + ui.value + "\")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                $(".sort_div").append(html);

            }
        }

    });

    $("#DDl_Sort2").multiselect('refresh');
    if (pos != -1)
        $("#DDl_Sort1").multiselect('uncheckAll');
    var width = '100%';
    $('.ui-multiselect').css('width', width);
    //$('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-state-default').css('color', '#C31001');
    $('.ui-multiselect').css('height', '34px');
    $('.ui-multiselect').css('margin-top', '5px');
}

function Sort3() {

    $(".sort_div").find("span").each(function (index, item) {
        $(this).remove();
    });
    $("#DDl_Sort3").multiselect({
        close: function () {

        },

        header: false,
        click: function (event, ui) {
            var html = '';
            var text = '';
            var exists = false;
            $(".sort_div:eq(0)").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == ui.value && !ui.checked) {
                    $(this).remove();
                    exists = true;
                }
                else if ($(this).attr("data-id") == ui.value && ui.checked) {
                    alert("Order already exists!!");
                    ui.checked = false;
                    exists = true;
                }

            });
            if (exists == false) {
                if (ui.value == "1")
                    text += 'Item';

                html += "<span class='remove' data-id='" + ui.value + "' ><a onclick='deleteorder(this,\"" + ui.value + "\")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                $(".sort_div").append(html);

            }
        }

    });

    $("#DDl_Sort3").multiselect('uncheckAll');
    var width = '100%';
    $('.ui-multiselect').css('width', width);
    //$('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-state-default').css('color', '#C31001');
    $('.ui-multiselect').css('height', '34px');
    $('.ui-multiselect').css('margin-top', '5px');
}

function Sort4() {
    $(".sort_div").find("span").each(function (index, item) {
        $(this).remove();
    });

    $("#DDl_Sort4").multiselect({
        close: function () {

        },

        header: false,
        click: function (event, ui) {
            var html = '';
            var text = '';
            var exists = false;
            $(".sort_div:eq(1)").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == ui.value && !ui.checked) {
                    $(this).remove();
                    exists = true;
                }
                else if ($(this).attr("data-id") == ui.value && ui.checked) {
                    alert("Order already exists!!");
                    ui.checked = false;
                    exists = true;
                }

            });
            if (exists == false) {
                if (ui.value == "1")
                    text += 'Section';

                html += "<span class='remove' data-id='" + ui.value + "' ><a onclick='deleteorder(this,\"" + ui.value + "\")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                $(".sort_div").append(html);

            }
        }

    });

    $("#DDl_Sort4").multiselect('uncheckAll');
    var width = '100%';
    $('.ui-multiselect').css('width', width);
    //$('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-state-default').css('color', '#C31001');
    $('.ui-multiselect').css('height', '34px');
    $('.ui-multiselect').css('margin-top', '5px');
}


function Sort4ForPurchaseReport() {
    $(".sort_div").find("span").each(function (index, item) {
        $(this).remove();
    });

    $("#DDl_Sort4").multiselect({
        close: function () {

        },

        header: false,
        click: function (event, ui) {
            var html = '';
            var text = '';
            var exists = false;
            $(".sort_div:eq(0)").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == ui.value && !ui.checked) {
                    $(this).remove();
                    exists = true;
                }
                else if ($(this).attr("data-id") == ui.value && ui.checked) {
                    alert("Order already exists!!");
                    ui.checked = false;
                    exists = true;
                }

            });
            if (exists == false) {
                if (ui.value == "1")
                    text += 'Section';

                html += "<span class='remove' data-id='" + ui.value + "' ><a onclick='deleteorder(this,\"" + ui.value + "\")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                $(".sort_div").append(html);

            }
        }

    });

    $("#DDl_Sort4").multiselect('uncheckAll');
    var width = '100%';
    $('.ui-multiselect').css('width', width);
    //$('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-state-default').css('color', '#C31001');
    $('.ui-multiselect').css('height', '34px');
    $('.ui-multiselect').css('margin-top', '5px');
}
function Sort5(pos) {

    $("#DDl_Sort5").multiselect({
        close: function () {

        },

        checkAll: function () {

            $(".sort_div:eq(" + pos + ")").find("span").each(function (index, item) {
                $(this).remove();
            });

            if ($(this).val() != null) {
                var sortValue = $(this).val().toString().split(",");
                for (var i = 0; i < sortValue.length; i++) {
                    var html = '';
                    var text = '';
                    if (sortValue[i] == "1")
                        text += 'Item';
                    else if (sortValue[i] == "2")
                        text += "PO"
                    else if (sortValue[i] == "3")
                        text += "Arrival Date"
                    html += "<span class='remove' data-id='" + sortValue[i] + "' ><a onclick='deleteorder(this,\"" + sortValue[i] + "\")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                    $(".sort_div").append(html);

                }
            }
        },

        uncheckAll: function () {
            $(".sort_div").find("span").each(function (index, item) {
                $(this).remove();
            });

        },
        click: function (event, ui) {
            var html = '';
            var text = '';
            var exists = false;
            $(".sort_div:eq(" + pos + ")").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == ui.value && !ui.checked) {
                    $(this).remove();
                    exists = true;
                }
                else if ($(this).attr("data-id") == ui.value && ui.checked) {
                    alert("Order already exists!!");
                    ui.checked = false;
                    exists = true;
                }

            });
            if (exists == false) {
                if (ui.value == "1")
                    text += 'Item';
                else if (ui.value == "2")
                    text += "Vendor"
                else if (ui.value == "3")
                    text += "PO"

                html += "<span class='remove' data-id='" + ui.value + "' ><a onclick='deleteorder(this,\"" + ui.value + "\")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                $(".sort_div").append(html);

            }
        }

    });

    $("#DDl_Sort5").multiselect('refresh');
    if (pos != -1)
        $("#DDl_Sort5").multiselect('uncheckAll');
    var width = '100%';
    $('.ui-multiselect').css('width', width);
    //$('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-multiselect-menu').css('width', '39.5%');
    $('.ui-state-default').css('color', '#C31001');
    $('.ui-multiselect').css('height', '34px');
    $('.ui-multiselect').css('margin-top', '5px');
}

function Sort7() {

    $("#DDl_Sort7").multiselect({
        close: function () {

        },


        click: function (event, ui) {
            var html = '';
            var text = '';
            var exists = false;
            $(".sort_div").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == ui.value && !ui.checked) {
                    $(this).remove();
                    exists = true;
                }
                else if ($(this).attr("data-id") == ui.value && ui.checked) {
                    alert("Order already exists!!");
                    ui.checked = false;
                    exists = true;
                }

            });
            if (exists == false) {
                if (ui.value == "1")
                    text += "PO"
                else if (ui.value == "2")
                    text += "Vendor"

                html += "<span class='remove' data-id='" + ui.value + "' ><a onclick='deleteorder(this,\"" + ui.value + "\")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                $(".sort_div").append(html);

            }
        }

    });
}

function Sort8() {
    $("#DDl_Sort8").multiselect({
        close: function () {

        },
        click: function (event, ui) {
            var html = '';
            var text = '';
            var exists = false;
            $(".sort_div").find("span").each(function (index, item) {
                $(this).remove();
            });
            $(".sort_div").find("span").each(function (index, item) {
                if ($(this).attr("data-id") == ui.value && !ui.checked) {
                    $(this).remove();
                    exists = true;
                }
                else if ($(this).attr("data-id") == ui.value && ui.checked) {
                    alert("Order already exists!!");
                    ui.checked = false;
                    exists = true;
                }

            });
            if (exists == false) {
                if (ui.value == "1")
                    text += "Payment Number"
                if (ui.value == "2")
                    text += "Payment Date"
                html += "<span class='remove' data-id='" + ui.value + "' ><a onclick='deleteorder(this,\"" + ui.value + "\")' title='Remove this ordering'>x</a><span>" + text + "</span></span>";
                $(".sort_div").append(html);

            }
        }
        ,multiple:false

    });
}
////////////////////////////////////////////////////Sorting End////////////////////////////////////////////////


///////////////////////////////////////////Subham Da 16_12_2015 Start////////////////////////////////////////////////

function ShowProjectsAtLabel(obj) {

    var Projects = $(obj).val();
    var totalProjects = '';

    $('#LabelForSection').text('');
    $('#LabelForVendor').text('');
    if ($(obj).find(":selected").text() == '') {
        $('#LabelForProject').text('');
    }
    else {

        for (var i = 0; i < Projects.length; i++) {
            if (totalProjects == '') {
                totalProjects = $(obj).find("option[value='" + Projects[i] + "']").text();
            }
            else {
                totalProjects += ' , ' + $(obj).find("option[value='" + Projects[i] + "']").text();
            }
        }

        $('#LabelForProject').text('Selected Project(s) : ' + totalProjects + "");

        
    }
}
function ShowSectionsAtLabel(obj) {
    var Sections = $(obj).val();

    var totalSections = '';
    if (Sections == '' || Sections == null || Sections == undefined) {

        $('#LabelForSection').text('');
    }
    else {

        for (var i = 0; i < Sections.length; i++) {
            if (totalSections == '') {
                totalSections = $(obj).find("option[value='" + Sections[i] + "']").text();
            }
            else {
                totalSections += ' , ' + $(obj).find("option[value='" + Sections[i] + "']").text();
            }
        }
        $('#LabelForSection').text('Selected Section(s) : ' + totalSections);

    }
}
function ShowVendorsAtLabel(obj) {
    var Vendors = $(obj).val();

    var totalVendors = '';
    if (Vendors == '' || Vendors == null || Vendors == undefined) {

        $('#LabelForVendor').text('');
    }
    else {

        for (var i = 0; i < Vendors.length; i++) {
            if (totalVendors == '') {
                totalVendors = $(obj).find("option[value='" + Vendors[i] + "']").text();
            }
            else {
                totalVendors += ' , ' + $(obj).find("option[value='" + Vendors[i] + "']").text();
            }
        }
        $('#LabelForVendor').text('Selected Vendor(s) : ' + totalVendors);

    }
}


//////////////////////////////////////////Subham Da 16_12_2015 End//////////////////////////////////////////////////




///////////////turya 05/02/2016/////////////////////////////////////////

function close_project_dropdown(project_id) {

    if (project_id != null) {
        //$("#section_panel").show();
        bindSection(project_id);
        bindVendor(project_id);
        bindSectionAll();
        bindVendorAll();

    }
    else {
        bindSectionAll();
        bindVendorAll();
        count = 0;
        //$("#section_panel").hide();


    }
}



/////////////////////////Report Category/////////////////////////////

function BindCategory() {


    $("#txtBudgetReportCategory").multiselect({
        close: function () {

        },
        header: 'Select Category',
        multiple: false
    });

    $("#txtBudgetReportCategory option:selected").removeAttr("selected");



    $("#txtBudgetReportCategory").on("change", function () {
        
        $('#LabelForSection').text('');
        $('#LabelForVendor').text('');
        $(".sec").removeAttr("id");
        $(".lbl").removeAttr("id");
        $(".sort_div").find("span").each(function (index, item) {
            $(this).remove();
        });
        //id = "LabelForSection"

        if ($(this).val() == '1') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".sec").first().attr("id", "txt_Section");
                $(".lbl").first().attr("id", "LabelForSection");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                $("#ByItem").show();
                $("#summery").hide();

                Sort3();

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
                $("#ByItem").hide();
                $("#summery").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }

        else if ($(this).val() == '2') {
            $("#section_panel").show();
            $("#ByItem").hide();
            $("#summery").show();

            var project_id = $('#txt_Project').val();
            if (project_id != null) {
                $(".sec").last().attr("id", "txt_Section");
                $(".lbl").last().attr("id", "LabelForSection");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                Sort4();
            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
            }
        }

        else {
            $("#section_panel").hide();
            $("#ByItem").hide();
            $("#summery").hide();
        }
    });


    $("#txtStatusReportCategory").multiselect({
        close: function () {

        },
        header: 'Select Category',
        multiple: false
    });

    $("#txtStatusReportCategory option:selected").removeAttr("selected");

    $("#txtStatusReportCategory").on("change", function () {
        $('#LabelForSection').text('');
        $('#LabelForVendor').text('');

        $(".sec").removeAttr("id");
        $(".lbl").removeAttr("id");
        $(".vnd").removeAttr("id");
        $(".report-short1").removeAttr("id");
        $(".itemStatus").removeAttr("id");
        $(".fromDate").removeAttr("id");
        $(".toDate").removeAttr("id");

        $(".sort_div").find("span").each(function (index, item) {
            $(this).remove();
        });

        //id = "LabelForSection"

        if ($(this).val() == '1') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".sec").first().attr("id", "txt_Section");
                $(".vnd").first().attr("id", "txt_Vendor");
                $(".lbl").first().attr("id", "LabelForSection");
                $(".lbl:eq(1)").attr("id", "LabelForVendor");
                $(".itemStatus:eq(0)").attr("id", "ddl_ItemStatus");
                $(".report-short1:eq(0)").attr("id", "DDl_Sort1");
                $(".fromDate:eq(0)").attr("id", "txt_FromDate");
                $(".toDate:eq(0)").attr("id", "txt_ToDate");

                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                $("#AtRiskByItem").show();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").hide();
                Sort1(0);
                AllDatePicker();
            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }

        else if ($(this).val() == '2') {
            $("#section_panel").show();
            $("#AtRiskByItem").hide();
            $("#ExpediteByItem").show();
            $("#ExpediteByStatus").hide();
            $("#ExpediteByNote").hide();
            $("#ReceivingReportBuSection").hide();
            $("#StatusByVendor").hide();
            var project_id = $('#txt_Project').val();
            if (project_id != null) {
                $(".sec:eq(1)").attr("id", "txt_Section");
                $(".lbl:eq(2)").last().attr("id", "LabelForSection");
                $(".report-short1:eq(1)").attr("id", "DDl_Sort1");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                Sort1(1);

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
            }
        }
        else if ($(this).val() == '3') {
            $("#section_panel").show();
            $("#AtRiskByItem").hide();
            $("#ExpediteByItem").hide();
            $("#ExpediteByStatus").show();
            $("#ExpediteByNote").hide();
            $("#ReceivingReportBuSection").hide();
            $("#StatusByVendor").hide();
            var project_id = $('#txt_Project').val();
            if (project_id != null) {
                $(".sec:eq(2)").attr("id", "txt_Section");
                $(".lbl:eq(3)").last().attr("id", "LabelForSection");
                $(".report-short1:eq(2)").attr("id", "DDl_Sort1");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                Sort1(2);

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
            }
        }

        else if ($(this).val() == '4') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".sec:eq(3)").attr("id", "txt_Section");
                $(".vnd:eq(1)").attr("id", "txt_Vendor");
                $(".lbl:eq(4)").attr("id", "LabelForSection");
                $(".lbl:eq(5)").attr("id", "LabelForVendor");
                $(".itemStatus:eq(1)").attr("id", "ddl_ItemStatus");
                $(".report-short1:eq(3)").attr("id", "DDl_Sort1");
                $(".fromDate:eq(1)").attr("id", "txt_FromDate");
                $(".toDate:eq(1)").attr("id", "txt_ToDate");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").show();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").hide();
                Sort1(3);
                AllDatePicker();

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
            }
        }


        else if ($(this).val() == '5') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".sec:eq(4)").attr("id", "txt_Section");
                $(".vnd:eq(2)").attr("id", "txt_Vendor");
                $(".lbl:eq(6)").attr("id", "LabelForSection");
                $(".lbl:eq(7)").attr("id", "LabelForVendor");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").show();
                $("#StatusByVendor").hide();
                Sort5(4);


            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }
        else if ($(this).val() == '6') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".fromDate:eq(2)").attr("id", "txt_FromDate");
                $(".toDate:eq(2)").attr("id", "txt_ToDate");
                $(".itemStatus:eq(2)").attr("id", "ddl_ItemStatus");
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").show();
                Sort2(5);
                AllDatePicker();

            }
            else {

                count = 0;
                $("#section_panel").hide();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }
        else {
            $("#section_panel").hide();
            $("#AtRiskByItem").hide();
            $("#ExpediteByItem").hide();
            $("#ExpediteByStatus").hide();
            $("#ReceivingReportBuSection").hide();
            $("#StatusByVendor").hide();
        }
    });

    $("#txtPurchaseCommitmentReportCategory").multiselect({
        close: function () {

        },
        header: 'Select Category',
        multiple: false
    });

    $("#txtPurchaseCommitmentReportCategory option:selected").removeAttr("selected");


    $("#txtPurchaseCommitmentReportCategory").on("change", function () {
        $(".sec").removeAttr("id");
        $(".lbl").removeAttr("id");
        $('#LabelForSection').text('');
        $('#LabelForVendor').text('');
        $(".sort_div").find("span").each(function (index, item) {
            $(this).remove();
        });
        //id = "LabelForSection"

        if ($(this).val() == '1') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                //$(".sec").first().attr("id", "txt_Section");
                //$(".lbl").first().attr("id", "LabelForSection");
                //bindSection(project_id);
                //bindVendor(project_id);
                //bindSectionAll();
                //bindVendorAll();
                //$("#ByItem").show();
                //$("#summery").hide();
                $("#section_panel").hide();
                $("#summery").hide();
                Sort3();
                return;

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
                $("#ByItem").hide();
                $("#summery").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }

        else if ($(this).val() == '2') {
            $("#section_panel").show();
            //$("#ByItem").hide();
            $("#CommitedCostSummery").show();

            var project_id = $('#txt_Project').val();
            if (project_id != null) {
                $(".sec:eq(0)").attr("id", "txt_Section");
                $(".lbl:eq(0)").attr("id", "LabelForSection");
                bindSection(project_id);
                Sort4ForPurchaseReport();
            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
                $("#CommitedCostSummery").hide();
            }
        }

        else {
            $("#section_panel").hide();
            $("CommitedCostSummery").hide();
            //$("#summery").hide();
        }
    });
}


function BindCategoryForBudgetReport(url) {
    if (url.indexOf("/Reports/BudgetReport") > -1) {
       
        $('#LabelForSection').text('');
        $('#LabelForVendor').text('');
        $(".sec").removeAttr("id");
        $(".lbl").removeAttr("id");
        $(".sort_div").find("span").each(function (index, item) {
            $(this).remove();
        });
        //id = "LabelForSection"

        if ($("#txtBudgetReportCategory").val() == '1') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".sec").first().attr("id", "txt_Section");
                $(".lbl").first().attr("id", "LabelForSection");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                $("#ByItem").show();
                $("#summery").hide();

                Sort3();

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
                $("#ByItem").hide();
                $("#summery").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }

        else if ($("#txtBudgetReportCategory").val() == '2') {
            $("#section_panel").show();
            $("#ByItem").hide();
            $("#summery").show();

            var project_id = $('#txt_Project').val();
            if (project_id != null) {
                $(".sec").last().attr("id", "txt_Section");
                $(".lbl").last().attr("id", "LabelForSection");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                Sort4();
            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
            }
        }

        else {
            $("#section_panel").hide();
            $("#ByItem").hide();
            $("#summery").hide();
        }

    }
}

function BindCategoryForPurchaseReport(url) {
    if (url.indexOf("/Reports/PurchaseReport") > -1) {
        
            $(".sec").removeAttr("id");
            $(".lbl").removeAttr("id");
            $('#LabelForSection').text('');
            $('#LabelForVendor').text('');
            $(".sort_div").find("span").each(function (index, item) {
                $(this).remove();
            });
            //id = "LabelForSection"

            if ($("#txtPurchaseCommitmentReportCategory").val() == '1') {
                $("#section_panel").show();
                var project_id = $('#txt_Project').val();
                //notify(project_id);
                if (project_id != null) {
                    //$(".sec").first().attr("id", "txt_Section");
                    //$(".lbl").first().attr("id", "LabelForSection");
                    //bindSection(project_id);
                    //bindVendor(project_id);
                    //bindSectionAll();
                    //bindVendorAll();
                    //$("#ByItem").show();
                    //$("#summery").hide();
                    $("#section_panel").hide();
                    $("#summery").hide();
                    //Sort3();
                return;

                }
                else {
                    bindSectionAll();
                    bindVendorAll();
                    count = 0;
                    $("#section_panel").hide();
                    $("#ByItem").hide();
                    $("#summery").hide();
                    notify("Please Select Projet!!!");
                }


                return;

            }

            else if ($("#txtPurchaseCommitmentReportCategory").val() == '2') {
                $("#section_panel").show();
                //$("#ByItem").hide();
                $("#CommitedCostSummery").show();

                var project_id = $('#txt_Project').val();
                if (project_id != null) {
                    $(".sec:eq(0)").attr("id", "txt_Section");
                    $(".lbl:eq(0)").attr("id", "LabelForSection");
                    bindSection(project_id);
                    Sort4ForPurchaseReport();
                }
                else {
                    bindSectionAll();
                    bindVendorAll();
                    count = 0;
                    $("#section_panel").hide();
                    $("#CommitedCostSummery").hide();
                }
            }

            else {
                $("#section_panel").hide();
                $("CommitedCostSummery").hide();
                //$("#summery").hide();
            }
        

    }
}


function BindCategoryForStatusReport(url) {

    if (url.indexOf("/Reports/StatusReport") > -1) {


        $('#LabelForSection').text('');
        $('#LabelForVendor').text('');

        $(".sec").removeAttr("id");
        $(".lbl").removeAttr("id");
        $(".vnd").removeAttr("id");
        $(".report-short1").removeAttr("id");
        $(".itemStatus").removeAttr("id");
        $(".fromDate").removeAttr("id");
        $(".toDate").removeAttr("id");

        $(".sort_div").find("span").each(function (index, item) {
            $(this).remove();
        });

        //id = "LabelForSection"

        if ($("#txtStatusReportCategory").val() == '1') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".sec").first().attr("id", "txt_Section");
                $(".vnd").first().attr("id", "txt_Vendor");
                $(".lbl").first().attr("id", "LabelForSection");
                $(".lbl:eq(1)").attr("id", "LabelForVendor");
                $(".itemStatus:eq(0)").attr("id", "ddl_ItemStatus");
                $(".report-short1:eq(0)").attr("id", "DDl_Sort1");
                $(".fromDate:eq(0)").attr("id", "txt_FromDate");
                $(".toDate:eq(0)").attr("id", "txt_ToDate");

                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                $("#AtRiskByItem").show();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").hide();
                Sort1(0);
                AllDatePicker();
            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }

        else if ($("#txtStatusReportCategory").val() == '2') {
            $("#section_panel").show();
            $("#AtRiskByItem").hide();
            $("#ExpediteByItem").show();
            $("#ExpediteByStatus").hide();
            $("#ExpediteByNote").hide();
            $("#ReceivingReportBuSection").hide();
            $("#StatusByVendor").hide();
            var project_id = $('#txt_Project').val();
            if (project_id != null) {
                $(".sec:eq(1)").attr("id", "txt_Section");
                $(".lbl:eq(2)").last().attr("id", "LabelForSection");
                $(".report-short1:eq(1)").attr("id", "DDl_Sort1");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                Sort1(1);

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
            }
        }
        else if ($("#txtStatusReportCategory").val() == '3') {
            $("#section_panel").show();
            $("#AtRiskByItem").hide();
            $("#ExpediteByItem").hide();
            $("#ExpediteByStatus").show();
            $("#ExpediteByNote").hide();
            $("#ReceivingReportBuSection").hide();
            $("#StatusByVendor").hide();
            var project_id = $('#txt_Project').val();
            if (project_id != null) {
                $(".sec:eq(2)").attr("id", "txt_Section");
                $(".lbl:eq(3)").last().attr("id", "LabelForSection");
                $(".report-short1:eq(2)").attr("id", "DDl_Sort1");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                Sort1(2);

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
            }
        }

        else if ($("#txtStatusReportCategory").val() == '4') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".sec:eq(3)").attr("id", "txt_Section");
                $(".vnd:eq(1)").attr("id", "txt_Vendor");
                $(".lbl:eq(4)").attr("id", "LabelForSection");
                $(".lbl:eq(5)").attr("id", "LabelForVendor");
                $(".itemStatus:eq(1)").attr("id", "ddl_ItemStatus");
                $(".report-short1:eq(3)").attr("id", "DDl_Sort1");
                $(".fromDate:eq(1)").attr("id", "txt_FromDate");
                $(".toDate:eq(1)").attr("id", "txt_ToDate");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").show();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").hide();
                Sort1(3);
                AllDatePicker();

            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
            }
        }


        else if ($("#txtStatusReportCategory").val() == '5') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".sec:eq(4)").attr("id", "txt_Section");
                $(".vnd:eq(2)").attr("id", "txt_Vendor");
                $(".lbl:eq(6)").attr("id", "LabelForSection");
                $(".lbl:eq(7)").attr("id", "LabelForVendor");
                bindSection(project_id);
                bindVendor(project_id);
                bindSectionAll();
                bindVendorAll();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").show();
                $("#StatusByVendor").hide();
                Sort5(4);


            }
            else {
                bindSectionAll();
                bindVendorAll();
                count = 0;
                $("#section_panel").hide();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }
        else if ($("#txtStatusReportCategory").val() == '6') {
            $("#section_panel").show();
            var project_id = $('#txt_Project').val();
            //notify(project_id);
            if (project_id != null) {
                $(".fromDate:eq(2)").attr("id", "txt_FromDate");
                $(".toDate:eq(2)").attr("id", "txt_ToDate");
                $(".itemStatus:eq(2)").attr("id", "ddl_ItemStatus");
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").show();
                Sort2(5);
                AllDatePicker();

            }
            else {

                count = 0;
                $("#section_panel").hide();
                $("#AtRiskByItem").hide();
                $("#ExpediteByItem").hide();
                $("#ExpediteByStatus").hide();
                $("#ExpediteByNote").hide();
                $("#ReceivingReportBuSection").hide();
                $("#StatusByVendor").hide();
                notify("Please Select Projet!!!");
            }


            return;

        }
        else {
            $("#section_panel").hide();
            $("#AtRiskByItem").hide();
            $("#ExpediteByItem").hide();
            $("#ExpediteByStatus").hide();
            $("#ReceivingReportBuSection").hide();
            $("#StatusByVendor").hide();
        }


    }
}


function AllDatePicker() {
    $('#txt_FromDate').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',

    });

    $('#txt_ToDate').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',

    });

}