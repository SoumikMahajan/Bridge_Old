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
            url1 = '/Vendor/getAllVendorList';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url1 = 'Vendor/getAllVendorList';
        }
        var totdata;

        $.get(url1)
            .success(function (response) {
                totdata = JSON.parse(response);
                //usdata = totdata.data;
                usdata = totdata;
                
               

                for (i = 0; i < usdata.length ; i++) {
                    us[i] = new Array();
                    var url1 = '';
                    if (LOCALORSERVERNEW == 'LOCAL') {
                        url1 = '/Vendor/EditVendor/';
                    }
                    else if (LOCALORSERVERNEW == 'SERVER') {
                        url1 = urlchanger('Vendor/EditVendor/');
                    }
                    url1 += usdata[i].Vendor_ID;
                    var htm = "";

                    if (usdata[i].IsSystemUser == "1") {
                        htm += '<i title="Send LogIn info" style="cursor:pointer;color:#7EA6C7;" onclick="openmailpopup(this);" data-email="' + usdata[i].Email + '" class="fa fa-share mrg5R"></i>';
                    }

                    htm += '<a data-vid="' + usdata[i].Vendor_ID + '" href="' + url1 + '"><i class="fa fa-pencil-square-o"></i></a>';

                    var url111 = '';
                    if (LOCALORSERVERNEW == 'LOCAL') {
                        url111 = '/Vendor/EditContact/';
                    }
                    else if (LOCALORSERVERNEW == 'SERVER') {
                        url111 = '/Vendor/EditContact/';
                    }

                    url111 += usdata[i].VendorName.split('^')[1];

                    var htm2 = "<a style='width: 315px;text-overflow: ellipsis;overflow: hidden;whitespace: no-wrap;white-space: nowrap;' title='" + usdata[i].ParentVendorName
                        + "' href='" + url1 + "'>" + usdata[i].ParentVendorName + "</a>";

                    us[i][0] = htm;
                    us[i][2] = usdata[i].IsHidden == '0' ? 'N' : 'Y';
                    us[i][3] = "<p class='text-uppercase' style='width: 250px;text-overflow: ellipsis;overflow: hidden;whitespace: no-wrap;white-space: nowrap;' title='" + usdata[i].ParentVendorName + "'>" + usdata[i].ParentVendorName + "</p>";
                    us[i][4] = "<p style='width: 200px;text-overflow: ellipsis;overflow: hidden;whitespace: no-wrap;white-space: nowrap;' title='" + usdata[i].Classifications + "'>" + usdata[i].Classifications + "</p>";
                    us[i][5] = "<a target='_blank' href='" + url111 + "'>" + usdata[i].VendorName.split('^')[0] + "</a>";
                    us[i][9] = usdata[i].Fax;
                    us[i][11] = usdata[i].Country;
                    us[i][8] = '<a href="telto:' + usdata[i].Phone + '">' + usdata[i].Phone + '</a>';
                    us[i][7] = usdata[i].City;
                    us[i][10] = '<a href="mailto:' + usdata[i].Email + '">' + usdata[i].Email + '</a>';
                    us[i][6] = usdata[i].ContactTitle;
                    us[i][12] = usdata[i].IsActive;

                    var zz = usdata[i].Contact_ID;
                    var html = "";
                    var yy = usdata[i].IsActive;

                    if (yy == 1) {
                        html = '<p class="fa fa-check-circle re_activate1"  btn_activate' + zz + ' id="IF' + zz + '" style="display:block;cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'0\');" data-id="' + zz + '" ></p>';
                    }
                    else if (yy == 0) {
                        html = '<p class="fa fa-times-circle de_activate1 btn_activate 1"   id="IIF' + zz + '" style="cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'1\');" data-id="' + zz + '"></p> ';
                    }

                    if (usdata[i].IsSystemUser == "1") {
                        us[i][1] = html;
                    }
                    else {
                        us[i][1] = "";
                    }
                    us[i][13] = usdata[i].IsActive + '' + usdata[i].IsSystemUser;//usdata[i].IsSystemUser;
                    us[i][14] = usdata[i].IsActive;
                }

                
                $('#example').dataTable().fnDestroy();
                $('#example').dataTable({
                    "processing": true,
                    fixedHeader: true,
                    "data": us,
                    "columns": [
                        { "title": "Action", "class": "leftposition" },
                        { "title": "Active" },
                        { "title": "Hidden" },
                        { "title": "Vendor Name" },
                        { "title": "Classifications" },
                        { "title": "Contact" },
                        { "title": "Title" },
                        { "title": "City" },
                        { "title": "Phone" },
                        { "title": "Fax", "class": "displaynone" },
                        { "title": "Email" },
                        { "title": "Country", "class": "displaynone" },
                        { "title": "IsActive", "class": "displaynone" },
                        { "title": "Activation", "class": "displaynone" },
                        { "title": "Activation", "class": "displaynone" }
                    ],
                    //"pageLength": overridedpagelength,
                    "initComplete": function () {
        
                    },
                    //"drawCallback": function (settings) {
                    //    RowHtmlRender();
                    //},
                    //"aoColumnDefs": [
                    //    { 'bSortable': false, 'aTargets': [0, 1] }
                    //]
                    //, "order": [[2, "asc"], [3, "asc"]]

                });

                var html = "";
                html = "<tr>"
                      + "<th class='Resetth' rowspan='1' colspan='1'>"
                      + "<a><input type='button' value='Reset' onclick='ResetSearch();' style='margin-bottom:6px;' /></a>"
                      + "<th rowspan='1' colspan='1'>"
                      + "<select data-indx='13' class='srch'>"
                      + "<option value='' selected='selected'>All</option>"
                      + "<option value='1' >Active</option>"
                      + "<option value='0'>Inactive</option>"
                      //+ "<option value='2'>Active System User</option>"
                      + "<option value='3'>Inactive System User</option>"
                      + "</select>"
                      + "</th>"
                      + "<th>"
                      + "<select data-indx='2' class='srch'>"
                      + "<option value=''>All</option>"
                      + "<option value='Y'>Yes</option>"
                      + "<option value='N' selected='selected'>No</option>"
                      + "</select>"
                      + "</th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='width:100%;padding: 2px 2%; font-family:Arial, FontAwesome' placeholder=' &#xF002;' data-indx='3' type='text'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='width:100%;padding: 2px 2%; font-family:Arial, FontAwesome' placeholder=' &#xF002;' data-indx='4' type='text'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='width:100%;padding: 2px 2%; font-family:Arial, FontAwesome' placeholder=' &#xF002;' data-indx='5' type='text'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='width:100%;padding: 2px 2%; font-family:Arial, FontAwesome' placeholder=' &#xF002;' data-indx='6' type='text'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='width:100%;padding: 2px 2%; font-family:Arial, FontAwesome' placeholder=' &#xF002;' data-indx='7' type='text'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='width:100%;padding: 2px 2%; font-family:Arial, FontAwesome' placeholder=' &#xF002;' data-indx='8' type='text'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch' style='width:100%;padding: 2px 2%; font-family:Arial, FontAwesome' placeholder=' &#xF002;' data-indx='10' type='text'></th>"
                      + "</tr>";

                $('#example thead').append(html);

                $('.srch').on('keyup change', function () {
                    
                    
                    var table = $('#example').DataTable();
                    
                    var indx = $(this).attr('data-indx');
                    var txt = this.value;
                    console.log("main txt: " + txt);
                    if (indx == "13") {

                        table.column('13').search("").draw();
                        table.column('14').search("").draw();

                        if (txt == '0' || txt == '1') {
                            table.column('14').search(txt).draw();
                            console.log(indx + "txt: " + txt);
                            console.log(table);
                        }
                        else if (txt == '2' || txt == '3') {
                            if (txt == '2')
                                txt = '11';
                            if (txt == '3')
                                txt = '01';
                            table.column('13').search(txt).draw();
                            console.log("S " + indx + "txt: " + txt);
                            console.log(table);
                        }
                        else {
                            table.column('14').search("").draw();
                            console.log(indx + "txt: " + txt);
                            console.log(table);
                        }
                    }
                    else {
                        table.column(indx).search(txt).draw();
                    }
                });

                $('#example').show();

                $('#example2').dataTable({
                    "processing": true,
                    "data": us,
                    "columns": [
                       { "title": "Action", "class": "centerposition" },
                       { "title": "VendorName", "class": "displaynone" },
                       { "title": "Contact" },
                       { "title": "Email" },
                       { "title": "Country", "class": "displaynone" },
                       { "title": "State/Prov", "class": "displaynone" },
                       { "title": "City", "class": "displaynone" },
                       { "title": "Address1", "class": "displaynone" },
                       {
                           "title": "ContactTitle", "class": "displaynone"
                       },
                       {
                           "title": "IsActive", "class": "displaynone"
                       },
                       {
                           "title": "Activation", "class": "displaynone"
                       },
                        {
                            "title": "issystem", "class": "displaynone"
                        },
                        {
                            "title": "issystem", "class": "displaynone"
                        }
                    ]
                   ,
                    "initComplete": function () {





                    },
                    "drawCallback": function (settings) {

                    },
                    "aoColumnDefs": [
                        { 'bSortable': false, 'aTargets': [0, 9] }
                    ]

                });

                var html2 = "";



                html2 = "<tr>"
                      + "<th class='Resetth' rowspan='1' colspan='1'>"
                      + "<a><input type='checkbox' class='mrg5R' onclick='checkuncheckallmail(this);' /><input type='button' value='Reset' onclick='ResetSearch2();' style='margin-bottom:6px;' /></a>"
                      + "</th>"
                      + "<th rowspan='1' colspan='1'><input class='srch2' style='padding: 2px 2%;' data-indx='2' type='text' placeholder='Search Contact'></th>"
                      + "<th rowspan='1' colspan='1'><input class='srch2' style='padding: 2px 2%;' data-indx='3' type='text' placeholder='Search Email'></th>"
                      + "</tr>";

                $('#example2 thead').append(html2);

                var table = $('#example2').DataTable();

                table.column(11).search(1).draw();
                table.column(12).search(1).draw();

                /*-----------------------------Added By Subhabrata Mondal 6_11_2015------------------------------*/

                //$('#example').find("thead th select option").each(function () {
                //    if ($(this).val() == "1")
                //        $(this).attr("selected", 0);
                //});

                $('#example2 tbody tr').each(function () {

                    var vid = $(this).find('td:eq(0)').find('a').attr('data-vid');
                    $(this).find('td:eq(0)').html("<input data-email='" + $(this).find('td:eq(3)').html() + "' type='checkbox' data-id='" + vid + "' class='mailcheck'/>");

                });

                /*-----------------------------Added By Subhabrata Mondal 6_11_2015------------------------------*/



                $('.srch2').on('keyup change', function () {
                    var indx = $(this).attr('data-indx');
                    var txt = this.value;
                    table
                           .column(indx)
                           .search(txt)
                           .draw();

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

function RowHtmlRender() {

    return;
    $('#example thead tr').find("th:eq(0)").html('Action').removeClass('sorting_asc');


    $('#example tbody tr').find("td:eq(9)").each(function () {

        var yy = $(this).prev("td").html();
        var zz = $(this).html();

        if (yy == 1) {
            var html = '<p class="fa fa-check-circle re_activate1"  btn_activate' + zz + ' id="IF' + zz + '" style="display:block;cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'0\');" data-id="' + zz + '" ></p>';

            $(this).html(html);
        }
        else if (yy == 0) {
            $(this).html('<p class="fa fa-times-circle de_activate1 btn_activate 1"   id="IIF' + zz + '" style="cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'1\');" data-id="' + zz + '"></p> ');
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



        $(this).html('<a href="Vendor/EditVendor/' + zz + '"><i onclick="sectionclicked(this);" class="fa fa-pencil-square-o"></i><input type="button" style="display:none;" value="Edit" style="margin-bottom:6px;" data-id="' + zz + '" /></a>');

    });

}

function RowHtmlRendernew() {

    return;
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
            //$(this).html('<input type="button" value="DeActivate" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'0\');" style="margin-bottom:6px;" data-id="' + zz + '" />');

            var html = '<p class="fa fa-check-circle re_activate1"  btn_activate' + zz + ' id="IF' + zz + '" style="display:block;cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'0\');" data-id="' + zz + '" ></p>';

            $(this).html(html);
        }
        else if (yy == 0) {
            //$(this).html('<input type="button" value="Activate" class="re_activate_new" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'1\');" style="margin-bottom:6px;" data-id="' + zz + '" />');

            $(this).html('<p class="fa fa-times-circle de_activate1 btn_activate 1"   id="IIF' + zz + '" style="cursor:pointer;font-size:20px;" onclick="ActivateDeactivateContact(this,\'' + zz + '\',\'1\');" data-id="' + zz + '"></p> ');
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

        $(this).html('<a href="Vendor/EditVendor/' + zz + '"><input type="button" value="Edit" style="margin-bottom:6px;" data-id="' + zz + '" /></a>');

    });

    $('.Resetth').html("<a><input type='button' value='Reset' onclick='ResetSearch();' style='margin-bottom:6px;' /></a>");

}

function EditVendorContact(id) {
    var url = "adduser/VendorContacts/" + id;

}

function ResetSearch2() {
    var table = $('#example2').DataTable();

    $('.srch2').each(function () {

        this.value = '';
        var indx = $(this).attr('data-indx');
        var txt = this.value;

        table
               .column(indx)
               .search(txt)
               .draw();


    });

}

function ResetSearch() {
    var table = $('#example').DataTable();
    table.search('').columns().search('').draw();

    $('.srch').each(function () {
        this.value = '';
    });

}

function GetCtryID(obj) {

    var Cnrty = $(obj).find('option:selected').attr("name").split("_")[1];
    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/adduser/getAllStateByCntryID';
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

function GetCtryID2(obj) {

    var Cnrty = $(obj).find('option:selected').attr("name").split("_")[1];
    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/adduser/getAllStateByCntryID';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'adduser/getAllStateByCntryID';
    }

    $.ajax({
        type: "POST",
        url: url,
        data: { 'CountryID': Cnrty },
        success: function (data) { $('#RDdl_State').html(data); },
        error: failed
    });
}

function GetStateID(obj) {
    var State = $(obj).find('option:selected').val();

    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/adduser/GetAllCityByStateID';
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
            url = '/Vendor/SetBridgeSupport';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'Vendor/SetBridgeSupport';
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

        if ($('#txt_LegalName').val().trim() == '') {
            //alert('You must enter Legal Name before continue');
            errorfunc('txt_LegalName', 'You must enter Legal Name before continue');
            return false;
        }

        var LegalName = $('#txt_LegalName').val().trim();


        if ($('#txt_Name').val().trim() == '') {
            //alert('You must enter a Vendor Name before continue');
            errorfunc('txt_Name', 'You must enter a Vendor Name before continue');
            return false;
        }

        var Name = $('#txt_Name').val().trim();



        //if ($('#txt_ShrtName').val().trim() == '') {
        //    errorfunc('txt_ShrtName', 'You must enter a Short Name before continue');
        //    return false;
        //}

        var ShortName = '';//$('#txt_ShrtName').val().trim();


        //if ($('#txt_Addr').val().trim() == '') {
        //    errorfunc('txt_Addr', 'You must enter Address before continue');
        //    return false;
        //}





        //if ($('#Ddl_Country').find('option:selected').attr("name").split("_")[1] == 0) {
        //    alert('You must select Country before continue');
        //    $('#Ddl_Country').focus();
        //    return false;
        //}

        var Ctry = $('#Ddl_Country').find('option:selected').attr("name").split("_")[1];
        var RCtry = $('#RDdl_Country').find('option:selected').attr("name").split("_")[1];

        //if ($('#Ddl_State').find('option:selected').attr("name").split("_")[1] == 0) {
        //    alert('You must select State before continue');
        //    $('#Ddl_State').focus();
        //    return false;
        //}

        var sname = $('#Ddl_State').find('option:selected').attr("name");
        var rsname = $('#RDdl_State').find('option:selected').attr("name");

        var State = sname == undefined ? '0' : sname.split("_")[1];
        var RState = rsname == undefined ? '0' : rsname.split("_")[1];


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
        var RCity = $('#Rtxt_City').val().trim();



        //if ($('#txt_ZipCode').val().trim() == '') {
        //    errorfunc('txt_ZipCode', 'You must enter ZipCode before continue');
        //    return false;
        //}

        var ZipCode = $('#txt_ZipCode').val().trim();
        var RZipCode = $('#Rtxt_ZipCode').val().trim();


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

        var WebSiteUrl = $('#txt_WebSiteUrl').val().trim();



        var isok = isValidUSPhoneFormat($('#txt_Phone').val());


        var Phone = $('#txt_Phone').val().trim();
        var Extension = $('#txt_Extension').val().trim();


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

        var termsid = $('#txt_Terms').val();

        var Minority = "";
        var Woman = "";

        if ($("#Minority_checkEdit").is(':checked')) {
            Minority = "1";
        }
        else {
            Minority = "0";

        }

        if ($("#Woman_checkEdit").is(':checked')) {
            Woman = "1";
        }
        else {
            Woman = "0";

        }

        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = '/Vendor/UpdateVendorDetail';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'Vendor/UpdateVendorDetail';
        }

        var Phone2 = $('#txt_Phone2').val().trim();

        var Address = $('#txt_Addr').val().trim();
        var RAddress = $('#Rtxt_Addr').val().trim();
        var RStatus = $('#Remit_chk').is(':checked') ? '1' : '0';

        //if (Address.trim() == "") {
        //    notify("You must add address before continue");
        //    $('#txt_Addr').focus();
        //    return;
        //}

        $.ajax({
            type: "POST",
            url: url,
            data: {
                'ParentVendor_ID': ParentVendorID, 'Name': Name, 'LegalName': LegalName, 'ShortName': ShortName, 'Address': Address, 'City': City,
                'Country': Ctry, 'State': State, 'ZipCode': ZipCode, 'EmailID': Email, 'WebSiteUrl': WebSiteUrl, 'Phone': Phone, 'Extension': Extension, 'Fax': Fax,
                'Currency': curr, 'Classification': clfs, 'YTD': YTD, 'IsActive': Isselected, 'Phone2': Phone2, 'Note': Note, 'termsid': termsid,
                'RCountry': RCtry, 'RState': RState, 'RZipCode': RZipCode, 'RAddress': RAddress, 'RCity': RCity, 'RStatus': RStatus, 'Minority': Minority, 'Woman': Woman
            },
            success: function (data) {
                if (data !== undefined && data === 'INVALID^CURRERR') {
                    irrrrr = 0;
                    notify('Vendor currency cannot be changed after invoices have been entered');
                }
                else if (data !== '' && data === 'SUCCESS') {
                    irrrrr = 0;
                    notify('Vendor Updated', 'green');

                    setTimeout(function () {
                        window.location.reload();
                    }, 1000);
                }
                else {
                    notify('Problem to update Vendor', "RED");
                }
                //if (data != '') {
                //    irrrrr = 0;
                //    notify('Vendor Updated', 'green');
                //    window.location.reload();
                //}
                //else {
                //    notify('Problem to update Vendor', "RED");
                //}
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

        var deleteUrl = '';
        var editUrl = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            deleteUrl = '../DeleteVendorContact/';
            editUrl = '../EditContact/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            deleteUrl = 'Vendor/DeleteVendorContact/';
            editUrl = 'Vendor/EditContact/';
        }

        $(this).html('<a class="delrow" data-id="' + zz + '" onclick="event.preventDefault();" href="' + deleteUrl + zz + '"><i title="Delete Contact" class="fa fa-trash-o fa-fw" style="font-size: 1.3em;color:black;cursor:pointer;margin-left:5px;"></i></a><a href="' + editUrl + zz + '"><i title="Edit Contact" class="fa fa-pencil-square-o" style="font-size: 1.3em;color:black;cursor:pointer;margin-left:5px;"></a>');

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
        $('#ContactsUnderParentVndr tbody tr').each(function (index, item) {
            $(this).show();

        });



    });

    $('#ContactsUnderParentVndr thead tr').find("th:eq(0)").removeClass('sorting_asc');

}

function ActivateDeactivateContact(obj, ContactID, val) {

    var url = "";

    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/Vendor/ActivateDeactivateContact';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'Vendor/ActivateDeactivateContact/';
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
                    notify('Successfully activated the vendor', "green");
                    window.location.reload();

                    $(obj).attr("onclick", "ActivateDeactivateContact(this,\'" + ContactID + "\','0');");
                    $(obj).removeClass("re_activate_new");

                    $(obj).addClass("re_activate1");
                    $(obj).removeClass("de_activate1");


                    $(obj).parent().parent().find("td:eq(8)").html('1');

                    var table = $('#example').DataTable();

                    table.draw();


                }
                else {

                    notify('Successfully deactivated the vendor', "green");
                    window.location.reload();

                    $(obj).attr("onclick", "ActivateDeactivateContact(this,\'" + ContactID + "\','1');");
                    $(obj).removeClass("re_activate1");
                    $(obj).addClass("de_activate1");
                    $(obj).val('Activate');
                    $(obj).parent().parent().find("td:eq(8)").html('0');

                    var table = $('#example').DataTable();

                    table.draw();
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
