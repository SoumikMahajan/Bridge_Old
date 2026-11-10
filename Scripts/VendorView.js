//var LOCALORSERVER = "LOCAL";//"SERVER"; //

var usdata;
var us = new Array();

var uscontactdata;
var uscontact = new Array();


var GlobalQS = '';
function BaseUrlReturn(obj) {
    //debugger
    if (obj == 'BU') {
        if (window.location.hostname == 'localhost') {
            return window.location.protocol + '//' + window.location.host + window.location.pathname.substring(0, window.location.pathname.lastIndexOf('/'));
        }
        else {
            return window.location.protocol + '//' + window.location.host;
        }
    }
    else if (obj == 'QS') {

        return window.location.search.substring(1, window.loca1221tion.search.length);
    }
}

function QueryString() {
    //debugger
    var strQueryString = BaseUrlReturn('QS');
    //alert(strQueryString);
}


function clicknew() {

    $(document).ready(function () {

        var url1 = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url1 = 'http://www.thebridgebytcg.com/VendorView/getAllVendorList';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url1 = 'VendorView/getAllVendorList';
        }

        var totdata;


        $.get(url1)
            .success(function (response) {

                totdata = JSON.parse(response);
                
                usdata = totdata.data;
                

                for (i = 0; i < usdata.length ; i++) {
                    us[i] = new Array();
                    us[i][0] = usdata[i].Vendor_ID;
                    us[i][1] = usdata[i].ParentVendorName;
                    us[i][2] = usdata[i].VendorName;
                    us[i][3] = usdata[i].Country;
                    us[i][4] = usdata[i].State;
                    us[i][5] = usdata[i].City;
                    us[i][6] = usdata[i].Address1;
                    us[i][7] = usdata[i].ContactTitle;
                    us[i][8] = usdata[i].IsActive;
                    us[i][9] = usdata[i].Contact_ID;
                }

                $('#example').dataTable({
                    "processing": true,
                    "data": us
                    , "columns": [
                        { "title": "Vendor_ID" },
                        { "title": "VendorName" },
                        { "title": "Contact" },
                        { "title": "Country" },
                        { "title": "State" },
                        { "title": "City" },
                         { "title": "Address1" },
                        {
                            "title": "ContactTitle"
                        },
                        {
                            "title": "IsActive", "class": "displaynone"
                        },
                        {
                            "title": "Activation"
                        }
                    ]
                    ,
                    "initComplete": function () {
                        RowHtmlRender();
                    },
                    "drawCallback": function (settings) {
                        RowHtmlRender();
                    },
                    "fnDrawCallback": function( oSettings ) {
                        RowHtmlRender();
                    },
                    "aoColumnDefs": [
                        { 'bSortable': false, 'aTargets': [0, 9] }
                    ]

                });

                var html = "";



                html = "<tr>"
                      + "<th class='Resetth' rowspan='1' colspan='1' align='center'>"
                      + "<a><input type='button' value='Reset' onclick='ResetSearch();' style='margin-bottom:6px;' /></a>"
                      + "</th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='padding: 2px 2%;' data-indx='1' type='text' placeholder='Search VendorName'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='padding: 2px 2%;' data-indx='2' type='text' placeholder='Search Contact'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='padding: 2px 2%;' data-indx='3' type='text' placeholder='Search Country'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='padding: 2px 2%;' data-indx='4' type='text' placeholder='Search State'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='padding: 2px 2%;' data-indx='5' type='text' placeholder='Search City'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='padding: 2px 2%;' data-indx='6' type='text' placeholder='Search Address1'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='padding: 2px 2%;' data-indx='7' type='text' placeholder='Search ContactTitle'></th>"
                      + "<th rowspan='1' colspan='1'>"

                        + "<select data-indx='8' class='srch'>"
                        + "<option value=''>All</option>"
                        + "<option value='1' selected='selected'>Active</option>"
                        + "<option value='0'>Deactive</option>"
                        + "</select>"

                        + "</th>"

                      + "<th></th>"
                      + "</tr>";

                $('#example thead').append(html);
                

                var table = $('#example').DataTable();

                /*-----------------------------Added By Subhabrata Mondal 6_11_2015------------------------------*/

                //$('#example').find("thead th select option").each(function () {
                //    if ($(this).val() == "1")
                //        $(this).attr("selected", 0);
                //});

                table.column(8).search(1).draw();

                /*-----------------------------Added By Subhabrata Mondal 6_11_2015------------------------------*/



                $('.srch').on('keyup change', function () {
                    var indx = $(this).attr('data-indx');
                    var txt = this.value;
                    table
                           .column(indx)
                           .search(txt)
                           .draw();
                    RowHtmlRendernew();
                });
            });
    });
}


$(document).ready(function () {
    clicknew();

    $('#example_paginate').live('click', function () {
        // alert(1);
        //clicknew();
    });

    $('.paginate_button').live('click', function () {
        RowHtmlRendernew();
    });

});

//$(function () {
//    $('select[name="example_length"]').live('change', function () {
//        alert(2);
//        RowHtmlRendernew();
//    });
//});

function RowHtmlRender() {

    $('#example thead tr').find("th:eq(0)").html('Action').removeClass('sorting_asc');


    $('#example tbody tr').find("td:eq(9)").each(function () {

        var yy = $(this).prev("td").html();
        var zz = $(this).html();

        if ($(this).find('input[type="button"]').attr('data-id') == undefined) {
            zz = $(this).html();
        }
        else {

            zz = $(this).find('input[type="button"]').attr('data-id');
        }

        if ($(this).find('p').attr('data-id') == undefined) {

            zz = $(this).html();
        }
        else {
            zz = $(this).find('p').attr('data-id');
        }

       

        if (yy == 1) {
            //  $(this).html('<input type="button" value="DeActivate" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'0\');" style="margin-bottom:6px;" data-id="' + zz + '" />');

            //var html = '<p class="fa fa-check-circle re_activate1"  btn_activate' + zz + ' id="IF' + zz + '" style="display:block;cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'0\');" data-id="' + zz + '" ></p>';
            
            var html = '<p class="fa fa-check-circle re_activate1"  btn_activate' + zz + ' id="IF' + zz + '" style="display:block;font-size:20px;" data-id="' + zz + '" ></p>';
            $(this).html(html);

           
        }
        else if (yy == 0) {

            //$(this).html('<input type="button" value="Activate" class="re_activate_new" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'1\');" style="margin-bottom:6px;" data-id="' + zz + '" />');
            //$(this).html('<p class="fa fa-times-circle de_activate1 btn_activate 1"   id="IIF' + zz + '" style="cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'1\');" data-id="' + zz + '"></p> ');
            
            $(this).html('<p class="fa fa-times-circle de_activate1 btn_activate 1"   id="IIF' + zz + '" style="font-size:20px;"  data-id="' + zz + '"></p> ');
        }

    });


    $('#example tbody tr').find("td:eq(0)").each(function () {
        var zz = $(this).html();

       

        if ($(this).find('input[type="button"]').attr('data-id') == undefined) {
            zz = $(this).html();
        }
        else {

            zz = $(this).find('input[type="button"]').attr('data-id');
        }



        $(this).html('<a href="ViewVendor/' + zz + '"><input type="button" value="Select" style="margin-bottom:6px;" data-id="' + zz + '" /></a>');
      

    });

}

function RowHtmlRendernew() {

    $('#example thead tr').find("th:eq(0)").html('Action').removeClass('sorting_asc');


    $('#example tbody tr').find("td:eq(9)").each(function () {

        var yy = $(this).prev("td").html();
        var zz = '';

        if ($(this).find('input[type="button"]').attr('data-id') == undefined) {
            zz = $(this).html();
        }
        else {

            zz = $(this).find('input[type="button"]').attr('data-id');
        }

        if ($(this).find('p').attr('data-id') == undefined) {

            zz = $(this).html();
        }
        else {
            zz = $(this).find('p').attr('data-id');
        }


        

        if (yy == 1) {
            //  $(this).html('<input type="button" value="DeActivate" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'0\');" style="margin-bottom:6px;" data-id="' + zz + '" />');

            //var html = '<p class="fa fa-check-circle re_activate1"  btn_activate' + zz + ' id="IF' + zz + '" style="display:block;cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'0\');" data-id="' + zz + '" ></p>';

            var html = '<p class="fa fa-check-circle re_activate1"  btn_activate' + zz + ' id="IF' + zz + '" style="display:block;font-size:20px;" data-id="' + zz + '" ></p>';
            $(this).html(html);
        }
        else if (yy == 0) {

            //$(this).html('<input type="button" value="Activate" class="re_activate_new" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'1\');" style="margin-bottom:6px;" data-id="' + zz + '" />');
            //$(this).html('<p class="fa fa-times-circle de_activate1 btn_activate 1"   id="IIF' + zz + '" style="cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'1\');" data-id="' + zz + '"></p> ');
            $(this).html('<p class="fa fa-times-circle de_activate1 btn_activate 1"   id="IIF' + zz + '" style="font-size:20px;"  data-id="' + zz + '"></p> ');
        }

    });


    $('#example tbody tr').find("td:eq(0)").each(function () {
        var zz = '';// $(this).find('input[type="button"]').attr('data-id');
        if ($(this).find('input[type="button"]').attr('data-id') == undefined) {
            zz = $(this).html();
        }
        else {

            zz = $(this).find('input[type="button"]').attr('data-id');
        }

        $(this).html('<a href="ViewVendor/' + zz + '"><input type="button" value="Select" style="margin-bottom:6px;" data-id="' + zz + '" /></a>');

    });

    $('.Resetth').html("<a><input type='button' value='Reset' onclick='ResetSearch();' style='margin-bottom:6px;' /></a>");

}

function EditVendorContact(id) {
    var url = "adduser/VendorContacts/" + id;

}

function ResetSearch() {
    var table = $('#example').DataTable();

    $('.srch').each(function () {

        this.value = '';
        var indx = $(this).attr('data-indx');
        var txt = this.value;

        table
               .column(indx)
               .search(txt)
               .draw();


    });

}


function GetCtryID(obj) {

    var Cnrty = $(obj).find('option:selected').attr("name").split("_")[1];
    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = 'http://www.thebridgebytcg.com/adduser/getAllStateByCntryID';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'adduser/getAllStateByCntryID';
    }

    $.ajax({
        type: "POST",
        url: url,
        data: { 'CountryID': Cnrty },
        success: GetSuccessDetail,
        error: failed
    });
}
function GetStateID(obj) {
    var State = $(obj).find('option:selected').val();

    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = 'http://www.thebridgebytcg.com/adduser/GetAllCityByStateID';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'adduser/GetAllCityByStateID';
    }
    $.ajax({
        type: "POST",
        url: url,
        data: { 'StateID': State },
        success: GetSuccessDetailState,
        error: failed
    });
}
function GetSuccessDetailState(data) {
    //alert(data);
    //$('#Ddl_City').html(data);
    //$('#txt_City').html(data);
}

function GetSuccessDetail(data) {
    $('#Ddl_State').html(data);
}

function failed(data) {
    alert('Failed');
}


$(function () {
    $('#btn_EdtPrntVndr').click(function () {

        var ischecked = $('#chk_bridgesupporter').is(':checked') ? 1 : 0;

        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = 'http://www.thebridgebytcg.com/VendorView/SetBridgeSupport';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'VendorView/SetBridgeSupport';
        }

        var mode = 'SETBRIDGESUPPORTER';
        var vendorid = $('#Vendorid').val();

        $.ajax({
            type: "POST",
            async: false,
            url: url,
            data: { 'mode': mode, 'vendorid': vendorid, 'ischecked': ischecked },
            success: function (data) {

            },
            error: function () {
                alert('Something going wrong');
            }
        });



        if ($('#txt_Name').val().trim() == '') {
            //alert('You must enter a Vendor Name before continue');
            errorfunc('txt_Name', 'You must enter a Vendor Name before continue');
            return false;
        }

        var Name = $('#txt_Name').val().trim();

        if ($('#txt_LegalName').val().trim() == '') {
            //alert('You must enter Legal Name before continue');
            errorfunc('txt_LegalName', 'You must enter Legal Name before continue');
            return false;
        }

        var LegalName = $('#txt_LegalName').val().trim();

        //if ($('#txt_ShrtName').val().trim() == '') {
        //    errorfunc('txt_ShrtName', 'You must enter a Short Name before continue');
        //    return false;
        //}

        var ShortName = '';//$('#txt_ShrtName').val().trim();


        //if ($('#txt_Addr').val().trim() == '') {
        //    errorfunc('txt_Addr', 'You must enter Address before continue');
        //    return false;
        //}

        var Address = $('#txt_Addr').val().trim();



        //if ($('#Ddl_Country').find('option:selected').attr("name").split("_")[1] == 0) {
        //    alert('You must select Country before continue');
        //    $('#Ddl_Country').focus();
        //    return false;
        //}

        var Ctry = $('#Ddl_Country').find('option:selected').attr("name").split("_")[1];

        //if ($('#Ddl_State').find('option:selected').attr("name").split("_")[1] == 0) {
        //    alert('You must select State before continue');
        //    $('#Ddl_State').focus();
        //    return false;
        //}

        var State = $('#Ddl_State').find('option:selected').attr("name").split("_")[1];


        ////if ($('#txt_City').find('option:selected').attr("name").split("_")[1] == 0) {
        ////    alert('You must enter City before continue');
        ////    $('#txt_City').focus();
        ////    return false;
        ////}
        //if ($('#txt_City').val().trim() == '') {
        //    errorfunc('txt_City', 'You must enter City before continue');
        //    return false;
        //}

        var City = $('#txt_City').val().trim();



        //if ($('#txt_ZipCode').val().trim() == '') {
        //    errorfunc('txt_ZipCode', 'You must enter ZipCode before continue');
        //    return false;
        //}

        var ZipCode = $('#txt_ZipCode').val().trim();

        //if ($('#txt_Email').val().trim() == '') {
        //    errorfunc('txt_Email', 'You must enter Email address before continue');
        //    return false;
        //}

        var Email = $('#txt_Email').val().trim();

        if (Email != '') {
            var reg = /^[^@\s]+@[^@\s]+\.[^@\s]{2,}$/;
            var eaddress = Email;
            if (Email != '' && reg.test(eaddress) == false) {
                $('#txt_Email').val('');
                errorfunc('txt_Email', 'You must enter an email address in the correct email format.');
                return false;
            }
        }

        //if ($('#txt_WebSiteUrl').val().trim() == '') {
        //    errorfunc('txt_WebSiteUrl', 'You must enter WebSite Url before continue');
        //    return false;
        //}

        var WebSiteUrl = $('#txt_WebSiteUrl').val().trim();

        //if ($('#txt_Phone').val().trim() == '') {
        //    errorfunc('txt_Phone', 'You must enter Phone before continue');
        //    return false;
        //}

        var isok = isValidUSPhoneFormat($('#txt_Phone').val());

        //if (!isok) {
        //    errorfunc('txt_Phone', 'You must enter Phone before continue');
        //    return false;
        //}

        var Phone = $('#txt_Phone').val().trim();
        var Extension = $('#txt_Extension').val().trim();

        //if ($('#txt_Fax').val().trim() == '') {
        //    errorfunc('txt_Fax', 'You must enter Fax before continue');
        //    return false;
        //}

        var Fax = $('#txt_Fax').val().trim();

        if ($('#Ddl_Currency').find('option:selected').attr("name").split("_")[1] == 0) {
            alert('You must select Currency before continue');
            $('#Ddl_Currency').val('');
            $('#Ddl_Currency').focus();
            return false;
        }

        var curr = $('#Ddl_Currency').find('option:selected').attr("name").split("_")[1];

        //if ($('#Ddl_Clftion').val() == null) {
        //    alert('You must select Classification before continue');
        //    $('#Ddl_Clftion').val('');
        //    $('#Ddl_Clftion').focus();
        //    return false;
        //}

        //var clfs = $('#Ddl_Clftion').find('option:selected').attr("name").split("_")[1];

        var clfs = $('#Ddl_Clftion').val();

        if (clfs != null && clfs != 'null' && clfs != undefined && clfs != 'undefined') {
            clfs = clfs.toString().replace(/,/g, "^");
        }
        else {
            clfs = '';
        }

        //if ($('#txt_YtdPchs').val().trim() == '') {            
        //    errorfunc('txt_YtdPchs', 'You must enter YTD Purchase amount before continue');
        //    return false;
        //}

        var YTD = '';//$('#txt_YtdPchs').val().trim();



        var Isselected = '0';

        if ($('#txt_Actv').is(":checked") == true) {
            Isselected = '1';
        }
        else {
            Isselected = '0';
        }

        var Note = $('#txt_VndrNotes').val();

        var ParentVendorID = $('#Vendorid').val();

        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = 'http://www.thebridgebytcg.com/VendorView/UpdateVendorDetail';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'VendorView/UpdateVendorDetail';
        }

        var Phone2 = $('#txt_Phone2').val().trim();

        $.ajax({
            type: "POST",
            url: url,
            data: { 'ParentVendor_ID': ParentVendorID, 'Name': Name, 'LegalName': LegalName, 'ShortName': ShortName, 'Address': Address, 'City': City, 'Country': Ctry, 'State': State, 'ZipCode': ZipCode, 'EmailID': Email, 'WebSiteUrl': WebSiteUrl, 'Phone': Phone, 'Extension': Extension, 'Fax': Fax, 'Currency': curr, 'Classification': clfs, 'YTD': YTD, 'IsActive': Isselected, 'Phone2': Phone2, 'Note': Note },
            success: function (data) {
                if (data != '') {
                    alert('Vendor Updated');
                    window.location.reload();
                }
                else {
                    alert('Problem to create Vendor');
                }
            },
            error: function () {
                alert('Something going wrong');
            }
        });






    });
});



function RowHtmlRenderfrContcts() {

    $('#ContactsUnderParentVndr thead tr').find("th:eq(0)").html('Action').removeClass('sorting_asc');



    $('#ContactsUnderParentVndr tbody tr').find("td:eq(0)").each(function () {
        var zz = $(this).html();
        //var yy = $(this).parent().find("td:eq(0)").html();

        if ($(this).find('a').attr('data-id') == undefined) {
            zz = $(this).html();
        }
        else {

            zz = $(this).find('a').attr('data-id');
        }

        
        var viewUrl = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            
            viewUrl = '../ViewContact/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
           
            viewUrl = 'VendorView/ViewContact/';
        }

        
        $(this).html('<a href="' + viewUrl + zz + '"><input type="button" style="margin-bottom:6px;" class="buttonedit" /></a>');

    });

    // $('#ContactsUnderParentVndr thead tr').find("th:eq(0)").html("<a><input type='button' value='Reset' onclick='ResetSearch();' style='margin-bottom:6px;' /></a>");

}

function ResetContactSearch() {
    var table = $('#ContactsUnderParentVndr').DataTable();

    $('.srch').each(function () {

        this.value = '';
        var indx = $(this).attr('data-indx');
        var txt = this.value;

        table
               .column(indx)
               .search(txt)
               .draw();


    });

    $('#ContactsUnderParentVndr thead tr').find("th:eq(0)").removeClass('sorting_asc');

}


function ActivateDeactivateContact(obj, ContactID, val) {

    var url = "";

    if (LOCALORSERVERNEW == 'LOCAL') {
        url = 'ActivateDeactivateContact';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'VendorView/ActivateDeactivateContact/';
    }



    $.ajax({
        type: "POST",
        url: url,
        async: false,
        //dataType: "text",
        //contentType: "application/json; charset=utf-8",
        data: { 'VendorContactID': ContactID, 'Isactive': val },
        success: function (data) {
            if (data == 'FAILED') {
                alert('You can not delete the lasrt contact for a vendor.');
            }
            else {
                if (val == 1) {
                    alert('Successfully activated the vendor');
                    $(obj).attr("onclick", "ActivateDeactivateContact(this,\'" + ContactID + "\','0');");
                    $(obj).removeClass("re_activate_new");
                    $(obj).val('Deactivate');
                    $(obj).parent().parent().find("td:eq(8)").html('1');
                }
                else {

                    alert('Successfully deactivated the vendor');
                    $(obj).attr("onclick", "ActivateDeactivateContact(this,\'" + ContactID + "\','1');");
                    $(obj).addClass("re_activate_new");
                    $(obj).val('Activate');
                    $(obj).parent().parent().find("td:eq(8)").html('0');
                }

            }

        },
        error: function () {
            alert('Something went wrong.');
        }
    });


}

$(function () {



    $('select[name="example_length"]').live('change', function () {
        RowHtmlRendernew();

    });


});


function isValidUSPhoneFormat(elementValue) {
    var phoneNumberPattern = /^[(]{0,1}[0-9]{3}[)]{0,1}[-\s.]{0,1}[0-9]{3}[-\s.]{0,1}[0-9]{4}$/;
    if (phoneNumberPattern.test(elementValue) == false) {
        var phoneNumberPattern = /^(\()?\d{3}(\))?(.|\s)?\d{3}(.|\s)\d{4}$/;
        return phoneNumberPattern.test(elementValue);
    }
    return phoneNumberPattern.test(elementValue);
}
function errorfunc(obj, message) {
    $('#' + obj).val($('#' + obj).val().trim());
    $('#' + obj).attr('placeholder', message);
    $('#' + obj).effect("shake", { times: 2 }, 700);
    //window.setInterval(function () { $('#' + obj).attr('placeholder', ''); }, 5000);
    $('#' + obj).focus();
}
