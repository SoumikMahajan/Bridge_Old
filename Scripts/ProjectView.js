

$(function () {

    var date = new Date();
    var currentMonth = date.getMonth();
    var currentDate = date.getDate();
    var currentYear = date.getFullYear();

    $('#txt_InstallDate').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',
        minDate: new Date(currentYear, currentMonth, currentDate)
    });

    $('#txt_CompletionDate').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',
        minDate: new Date(currentYear, currentMonth, currentDate)
    });

    $('#txt_WareHouseReqDate').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',
        minDate: new Date(currentYear, currentMonth, currentDate)
    });

    $('#txt_OpeningDate').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',
        minDate: new Date(currentYear, currentMonth, currentDate)
    });

});

function GetCtryIDForProject(obj) {

    var StateBindID = $(obj).attr("name");

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
        success: function (data) {
            if (data != null) {
                $('#' + StateBindID).html(data);
            }
            else {

            }
        }
    });

}

///////////////////////////////////////////////-----------Project Creation and Updation Realated Operation----------/////////////////////////////////////////////

$(function () {

    $('#btn_CreateProject').click(function () {

        var mode = 'SaveProjectDetail';
        var altMsg = 'Project has been created';

        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = 'http://www.thebridgebytcg.com/ProjectView/SaveProjectDetail';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'ProjectView/SaveProjectDetail';
        }

        ProjectCreationUpdationOptCommon(mode, altMsg, url);

    });

    $('#btn_UpdateProject').click(function () {




        var mode = 'UpdateProjectDetail';
        var altMsg = 'Project has been Updated';

        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = 'http://www.thebridgebytcg.com/ProjectView/UpdateProjectDetail';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'ProjectView/UpdateProjectDetail';
        }

        ProjectCreationUpdationOptCommon(mode, altMsg, url);

    });

    function ProjectCreationUpdationOptCommon(mode, altMsg, url) {
        var ProjectName = $('#txt_ProjectName').val().trim();
        if (ProjectName == "") {
            errorfunc('txt_ProjectName', 'You must enter a Project Name before saving the Project');
            return;
        }


        var LegalName = $('#txt_LegalName').val().trim();
        //if (LegalName == "") {
        //    errorfunc('txt_LegalName', 'You must enter a Legal Name before saving the Project');
        //    return;
        //}

        var InvAddress = $('#txt_InvAddr').val().trim();
        //if (InvAddress == "") {
        //    $('#txt_InvAddr').attr("style", "height:30px");
        //    errorfunc('txt_InvAddr', 'You must enter an Invoice Address before saving the Project');
        //    return;
        //}

        var InvCountry = $('#Ddl_InvCountry').val().trim();
        //if (InvCountry == "0") {
        //    alert('You must select an Invoice Country before saving the Project');
        //    $('#Ddl_InvCountry').focus();
        //    return;
        //}

        var InvState = $('#Ddl_InvState').val().trim();
        //if (InvState == "0") {
        //    alert('You must select an Invoice State before saving the Project');
        //    $('#Ddl_InvState').focus();
        //    return;
        //}

        var InvCity = $('#txt_InvCity').val().trim();
        //if (InvCity == "") {
        //    errorfunc('txt_InvCity', 'You must enter an Invoice City before saving the Project');
        //    return;
        //}


        var InvZipCode = $('#txt_InvZipCode').val().trim();
        //if (InvZipCode == "") {
        //    errorfunc('txt_InvZipCode', 'Please enter Postal code before saving the Project');
        //    return;
        //}

        var NoUserAppvBdgt = $('#ddl_NoUserAppvBdgt').val().trim();

        var NoUserAppvPO = $('#ddl_NoUserAppvPO').val().trim();

        var ClientID = $('#txt_Client').val().trim();
        //if (ClientID == "0") {
        //    alert('Please select client before saving the Project');
        //    $('#txt_Client').focus();
        //    return;
        //}

        var ManagerID = $('#txt_Manager').val().trim();
        //if (ManagerID == "0") {
        //    alert('Please select Manager before saving the Project');
        //    $('#txt_Manager').focus();          
        //    return;
        //}

        var DevInfo = $('#txt_DevInfo').val().trim();
        ////if (DevInfo == "") {
        ////    errorfunc('txt_DevInfo', 'Please enter Legal name');
        ////    return;
        ////}

        var HotelAddr = $('#txt_HotelAddr').val().trim();
        ////if (HotelAddr == "") {
        ////     alert('Please select client before saving the Project');
        ////    return;
        ////}

        var HotelCountry = $('#Ddl_HotelCountry').val().trim();
        //if (HotelCountry == "0") {
        //    alert('Please select Hotel Country before saving the Project');
        //    $('#Ddl_HotelCountry').focus();
        //    return;
        //}

        var HotelState = $('#Ddl_HotelState').val().trim();
        //if (HotelState == "0") {
        //    alert('Please select Hotel State before saving the Project');
        //    $('#Ddl_HotelState').focus();
        //    return;
        //}

        var HotelCity = $('#txt_HotelCity').val().trim();
        ////if (HotelCity == "") {
        ////    errorfunc('txt_HotelCity', 'Please enter Hotel City name');
        ////    return;
        ////}

        var HotelZipCode = $('#txt_HotelZipCode').val().trim();
        ////if (HotelZipCode == "") {
        ////    errorfunc('txt_HotelZipCode', 'Please enter Hotel PostalCode name');
        ////    return;
        ////}


        var ProjectScope = $('#txt_ProjectScope').val().trim();
        ////if (ProjectScope == "") {
        ////    errorfunc('txt_ProjectScope', 'Please enter Legal name');
        ////    return;
        ////}

        var OwnerName = $('#txt_OwnerName').val().trim();
        ////if (OwnerName == "") {
        ////    errorfunc('txt_OwnerName', 'Please enter Legal name');
        ////    return;
        ////}

        var OwnerAddr = $('#txt_OwnerAddr').val().trim();
        ////if (OwnerAddr == "") {
        ////    errorfunc('txt_OwnerAddr', 'Please enter Legal name');
        ////    return;
        ////}

        var OwnerCountry = $('#Ddl_OwnerCountry').val().trim();
        //if (OwnerCountry == "0") {
        //    alert('Please select owner Country');
        //    return;
        //}

        var OwnerState = $('#Ddl_OwnerState').val().trim();
        //if (OwnerState == "0") {
        //    alert('Please select Owner State');
        //    $('#Ddl_OwnerState').focus();
        //    return;
        //}

        var OwnerCity = $('#txt_OwnerCity').val().trim();
        ////if (OwnerCity == "") {
        ////    errorfunc('txt_OwnerCity', 'Please enter Legal name');
        ////    return;
        ////}

        var OwnerZipCode = $('#txt_OwnerZipCode').val().trim();
        ////if (OwnerZipCode == "") {
        ////    errorfunc('txt_OwnerZipCode', 'Please enter Legal name');
        ////    return;
        ////}

        var OwnerContact = $('#txt_OwnerContact').val().trim();
        ////if (OwnerContact == "") {
        ////    errorfunc('txt_OwnerContact', 'Please enter Legal name');
        ////    return;
        ////}

        var OwnerNote = $('#txt_OwnerNote').val().trim();
        ////if (OwnerNote == "") {
        ////    $('#txt_OwnerNote').attr("style","height:30px");
        ////    errorfunc('txt_OwnerNote', 'Please enter Legal name');
        ////    return;
        ////}

        var OwnerFax = $('#txt_OwnerFax').val().trim();
        ////if (OwnerFax == "") {
        ////    errorfunc('txt_OwnerFax', 'Please enter Legal name');
        ////    return;
        ////}

        var OwnerEmail = $('#txt_OwnerEmail').val().trim();

        //if (OwnerEmail == "") {
        //    errorfunc('txt_OwnerEmail', 'You must enter an Owner Mail address before continuing');
        //    return;
        //}

        if (OwnerEmail != '') {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
            var eaddress = OwnerEmail;
            if (OwnerEmail != '' && reg.test(eaddress) == false) {
                $('#txt_OwnerEmail').val('');
                errorfunc('txt_OwnerEmail', 'You must enter a valid email format.');
                return false;
            }
        }

        var OwnerMobile = $('#txt_OwnerMobile').val().trim();
        ////if (OwnerMobile == "") {
        ////    errorfunc('txt_OwnerMobile', 'Please enter Legal name');
        ////    return;
        ////}

        var AccountingContact = $('#txt_AccountingContact').val().trim();
        ////if (AccountingContact == "") {
        ////    errorfunc('txt_AccountingContact', 'Please enter Legal name');
        ////    return;
        ////}

        var AccountingPhone = $('#txt_AccountingPhone').val().trim();
        //if (AccountingPhone == "") {
        //    errorfunc('txt_AccountingPhone', 'Please enter Phone number');
        //    return;
        //}

        var AccountingFax = $('#txt_AccountingFax').val().trim();
        ////if (AccountingFax == "") {
        ////    errorfunc('txt_AccountingFax', 'Please enter Legal name');
        ////    return;
        ////}

        var AccountingEmail = $('#txt_AccountingEmail').val().trim();

        //if (AccountingEmail == "") {
        //    errorfunc('txt_AccountingEmail', 'You must enter an Accounting Mail address before continuing');
        //    return;
        //}

        if (AccountingEmail != '') {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
            var eaddress = AccountingEmail;
            if (AccountingEmail != '' && reg.test(eaddress) == false) {
                $('#txt_AccountingEmail').val('');
                errorfunc('txt_AccountingEmail', 'You must enter a valid email format.');
                return false;
            }
        }


        ////if (AccountingEmail == OwnerEmail) {
        ////    alert('Accounting mail address and Owner mail address cannot be same');
        ////    $('#txt_AccountingEmail').focus();
        ////    return false;
        ////}


        var FederalID = $('#txt_FederalID').val().trim();
        ////if (FederalID == "") {
        ////    errorfunc('txt_FederalID', 'Please enter Legal name');
        ////    return;
        ////}

        var GST = $('#txt_GST').val().trim();
        if (GST == "") {
            GST = 0;
        }
        var GSTTYPE = $('#txt_GSTLblShw').val().trim();
        if (GSTTYPE == '') {
            GSTTYPE = 'GST/HST';
        }
        GST = GST + '^' + GSTTYPE;
        ////if (GST == "") {
        ////    errorfunc('txt_GST', 'Please enter Legal name');
        ////    return;
        ////}

        var PST = $('#txt_PST').val().trim();
        if (PST == "") {
            PST = 0;
        }
        var PSTTYPE = $('#txt_PSTLblShw').val().trim();
        if (PSTTYPE == '') {
            PSTTYPE = 'PST';
        }
        PST = PST + '^' + PSTTYPE;
        ////if (PST == "") {
        ////    errorfunc('txt_PST', 'Please enter Legal name');
        ////    return;
        ////}

        var StateTax = $('#txt_StateTax').val().trim();
        if (StateTax == "") {
            StateTax = 0;
        }
        var StateTaxTYPE = $('#txt_StateTxLblShw').val().trim();
        if (StateTaxTYPE == '') {
            StateTaxTYPE = 'State Tax';
        }
        StateTax = StateTax + '^' + StateTaxTYPE;
        //if (StateTax == "") {
        //    errorfunc('txt_StateTax', 'Please enter Legal name');
        //    return;
        //}

        var CurrencyCode = $('#ddl_CurrencyCode').val().trim();
        ////if (CurrencyCode == "0") {
        ////    alert('Please Select Currency code');
        ////    $('#ddl_CurrencyCode').focus();
        ////    return;
        ////}

        var InstallDate = $('#txt_InstallDate').val().trim();
        ////if (InstallDate == "") {
        ////    errorfunc('txt_InstallDate', 'Please enter Legal name');
        ////    return;
        ////}

        var CompletionDate = $('#txt_CompletionDate').val().trim();
        ////if (CompletionDate == "") {
        ////    errorfunc('txt_CompletionDate', 'Please enter Legal name');
        ////    return;
        ////}

        var WareHouseReqDate = $('#txt_WareHouseReqDate').val().trim();
        ////if (WareHouseReqDate == "") {
        ////    errorfunc('txt_WareHouseReqDate', 'Please enter Legal name');
        ////    return;
        ////}

        var OpeningDate = $('#txt_OpeningDate').val().trim();
        ////if (OpeningDate == "") {
        ////    errorfunc('txt_OpeningDate', 'Please enter Legal name');
        ////    return;
        ////}

        var ProjectCmt = $('#txt_ProjectCmt').val().trim();
        ////if (ProjectCmt == "") {
        ////    $('#txt_ProjectCmt').attr("style", "height:30px");
        ////    errorfunc('txt_ProjectCmt', 'Please enter Legal name');
        ////    return;
        ////}

        var ProjectContacts = "";

        var OnwnerRequirement = $('#txt_OwnerReqmnt').val().trim();

        ////var ProjectContacts = $('#ddl_ProjectContacts').val().trim();
        ////if (ProjectContacts == "0") {
        ////    alert('Please select Project Contacts');
        ////    $('#ddl_ProjectContacts').focus();
        ////    return;
        ////}

        var cnt = 0;
        var InstallFle = "";
        $('.getFile').each(function () {
            cnt = 1;
            InstallFle += $(this).attr("name") + "�";
        });

        ////if (InstallFle != "")
        ////{
        ////    InstallFle = InstallFle.substring(0, (InstallFle.length - 1));
        ////    alert(InstallFle);
        ////}

        ////if (cnt == 0)
        ////{
        ////    alert('You have not uploaded any file');
        ////    return false;
        ////}
        var ProjectID = $('#ProjectID').val();

        var PrchsOrdrCmmnts = $('#txt_PrchsOrdrCmmnts').val();

        $.ajax({
            type: "POST",
            url: url,
            data: {
                'ProjectName': ProjectName, 'LegalName': LegalName, 'InvAddress': InvAddress, 'InvCountry': InvCountry, 'InvState': InvState, 'InvCity': InvCity,
                'InvZipCode': InvZipCode, 'NoUserAppvBdgt': NoUserAppvBdgt, 'NoUserAppvPO': NoUserAppvPO, 'ClientID': ClientID, 'ManagerID': ManagerID, 'DevInfo': DevInfo,
                'HotelAddr': HotelAddr, 'HotelCountry': HotelCountry, 'HotelState': HotelState, 'HotelCity': HotelCity, 'HotelZipCode': HotelZipCode, 'ProjectScope': ProjectScope,
                'OwnerName': OwnerName, 'OwnerAddr': OwnerAddr, 'OwnerCountry': OwnerCountry, 'OwnerState': OwnerState, 'OwnerCity': OwnerCity, 'OwnerZipCode': OwnerZipCode,
                'OwnerContact': OwnerContact, 'OwnerNote': OwnerNote, 'OwnerFax': OwnerFax, 'OwnerEmail': OwnerEmail, 'OwnerMobile': OwnerMobile, 'AccountingContact': AccountingContact,
                'AccountingPhone': AccountingPhone, 'AccountingFax': AccountingFax, 'AccountingEmail': AccountingEmail, 'FederalID': FederalID, 'GST': GST, 'PST': PST, 'StateTax': StateTax,
                'CurrencyCode': CurrencyCode, 'InstallDate': InstallDate, 'CompletionDate': CompletionDate, 'WareHouseReqDate': WareHouseReqDate, 'OpeningDate': OpeningDate,
                'ProjectCmt': ProjectCmt, 'OnwnerRequirement': OnwnerRequirement, 'InstallationFile': InstallFle, 'mode': mode, 'ProjectID': ProjectID, 'PrchsOrdrCmmnts': PrchsOrdrCmmnts
            },
            success: function (data) {
                if (data != null) {

                    if (data.split('^')[0] == 'Success') {
                        if (data.split('^')[1] != 0) {
                            uploadfiles(data.split('^')[1], 'TC');
                            uploadfiles(data.split('^')[1], 'PF');
                            uploadfiles(data.split('^')[1], 'ISF');

                            //------------------------Added By Subhabrata Mondal 9_11_2015------------------------
                            saveVendorCODetails(mode, data.split('^')[1]);
                            //------------------------End By Subhabrata Mondal 9_11_2015------------------------//
                        }
                        //alert(altMsg);
                        var projectid = parseInt(data.split('^')[1]) - 1000;

                        var url = '';
                        if (LOCALORSERVERNEW == 'LOCAL') {
                            url = 'http://www.thebridgebytcg.com/ProjectView/projectteam/';

                        }
                        else if (LOCALORSERVERNEW == 'SERVER') {
                            url = 'ProjectView/projectteam/';

                        }

                        url += projectid;
                        notify(altMsg);
                        if (mode != 'UpdateProjectDetail') {
                            window.location.href = url;
                        }

                    }
                    else if (data.split('^')[0] == 'Fail') {
                        notify('error');
                    }
                    else if (data.split('^')[0] == '') {
                        notify('Something going Wrong');
                    }
                }

            }
        });

    }

});

////////////////////////////////////////////////////////---------------------End ----------------------------///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////--- Project Contact Related Operation ----------////////////////////////////////////////////////////////////////

$(function () {

    $('#btn_CreatePrjtContact').click(function () {

        var mode = 'SaveProjectCntDetail';
        var alertMsg = 'Project Contact is created';
        var confMsg = 'Do you want to create Project Contact?';
        var pid = $('#Hdn_ProjectID').val();

        var url = '';
        var url1 = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = 'http://www.thebridgebytcg.com/ProjectView/SaveProjectContactDetail';
            url1 = 'http://www.thebridgebytcg.com/ProjectView/editproject/' + pid;
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'ProjectView/SaveProjectContactDetail';
            url1 = 'ProjectView/editproject/' + pid;
        }

        ProjectContactOperation(mode, alertMsg, url, url1, confMsg);

    });

    $('#btn_EditPrjtContact').click(function () {

        $('#loader_EditProjectContact').show();
        $('#btn_EditPrjtContact').hide();

        var mode = 'UpdateProjectContactDtl';
        var alertMsg = 'Project Contact is Updated';
        var confMsg = 'Do you want to update Project Contact';

        var pid = $('#ProjectID').val();

        var url = '';
        var url1 = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = 'http://www.thebridgebytcg.com/ProjectView/UpdateProjectContactDetail';
            url1 = 'http://www.thebridgebytcg.com/ProjectView/editproject/' + pid;
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'ProjectView/UpdateProjectContactDetail';
            url1 = '/ProjectView/editproject/' + pid;
        }

        ProjectContactOperation(mode, alertMsg, url, url1, confMsg);

        $('#loader_EditProjectContact').hide();
        $('#btn_EditPrjtContact').show();
    });

    function ProjectContactOperation(mode, alertMsg, url, url1, confMsg) {

        var ProjectContactType = $('#Ddl_ProjectContactType').val().trim();
        if (ProjectContactType == "0") {
            notify('You must select a Contact Type before saving the Contact');
            $('#Ddl_ProjectContactType').focus();
            return;
        }

        var ProjectContactName = $('#txt_ProjectContactName').val().trim();
        if (ProjectContactName == "") {
            errorfunc('txt_ProjectContactName', 'You must enter a Contact Name before saving the Contact');
            return;
        }

        var ProjectAddress = $('#txt_ProjectAddress').val().trim();
        //if (ProjectAddress == "") {
        //    errorfunc('txt_ProjectAddress', 'You must enter an Address before saving the Project');
        //    return;
        //}

        var Country = $('#ddlCrntry').val().trim();
        //if (Country == "0") {
        //    alert('You must select Country before saving the Project');
        //    $('#ddlCrntry').focus();
        //    return;
        //}

        var ProjectState = $('#Ddl_ProjectState').val().trim();
        //if (ProjectState == "0") {
        //    alert('You must select State before saving the Project');
        //    $('#Ddl_ProjectState').focus();
        //    return;
        //}

        var ProjectCity = $('#txt_ProjectCity').val().trim();
        //if (ProjectCity == "") {
        //    errorfunc('txt_ProjectCity', 'You must enter City before saving the Contact');
        //    return;
        //}

        var ProjectTelephone = $('#txt_ProjectTelephone').val().trim();

        var Extension = $('#txt_Extension').val().trim();
        ////if (ProjectTelephone == "") {
        ////    errorfunc('txt_ProjectTelephone', 'You must enter a Contact Name before saving the Contact');
        ////    return;
        ////}

        var ProjectMobile = $('#txt_ProjectMobile').val().trim();
        ////if (ProjectContactName == "") {
        ////    errorfunc('txt_ProjectContactName', 'You must enter a Contact Name before saving the Contact');
        ////    return;
        ////}

        var ProjectFax = $('#txt_ProjectFax').val().trim();
        ////if (ProjectContactName == "") {
        ////    errorfunc('txt_ProjectContactName', 'You must enter a Contact Name before saving the Contact');
        ////    return;
        ////}

        var Email = $('#txt_Email').val();
        //if (Email.trim() == "") {
        //    errorfunc('txt_Email', 'You must enter an Email Address before continuing');
        //    return;
        //}
        if (Email != '') {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
            var eaddress = Email;
            if (Email != '' && reg.test(eaddress) == false) {
                $('#txt_Email').val('');
                errorfunc('txt_Email', 'You must enter an email address in the correct email format.');
                return false;
            }
        }


        var ProjectID = $('#Hdn_ProjectID').val();

        var confirmPjtCntSave = confirm(confMsg);

        if (confirmPjtCntSave) {

            $.ajax({
                type: "POST",
                url: url,
                data: {
                    'ContactTypeID': ProjectContactType, 'ContactName': ProjectContactName, 'PAddress': ProjectAddress, 'PCountry': Country, 'PState': ProjectState,
                    'PCity': ProjectCity, 'PTelephone': ProjectTelephone, 'Extension': Extension, 'PMobile': ProjectMobile, 'PFax': ProjectFax, 'Email': Email, 'mode': mode, 'ProjectID': ProjectID
                },
                success: function (data) {
                    if (data != null) {
                        if (data == 'Success') {
                            alert(alertMsg);




                            window.location = url1;

                            // window.location.href = url1;
                        }
                        else if (data == 'Fail') {
                            alert('error');
                        }
                        else if (data == '') {
                            alert('Something going wrong');
                        }
                    }

                }
            });
        }

    }

    $('#filedrag').click(function () {
        $('#fileselect').click();
    });

});

//////////////////////////////////////////////------------------End --------------------------------//////////////////////////////////////////////////////////////////


$(document).ready(function () {

    $("#fileselect").change(function () {

    });

});


/////for showing
function clicknew() {

    // $(document).ready(function () {

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = 'http://www.thebridgebytcg.com/ProjectView/getAllProjectList';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        //url1 = urlsuffix + "/" + 'ProjectView/getAllProjectList';
        url1 = 'ProjectView/getAllProjectList';
    }

    // alert(url1);
    var totdata;
    $.get(url1)
        .success(function (response) {
            var project;
            totdata = JSON.parse(response);
           
            var project = new Array();
            for (i = 0; i < totdata.length ; i++) {
                project[i] = new Array();
                project[i][0] = totdata[i].ProjectID;
                project[i][1] = "";
                project[i][2] = totdata[i].ProjectNumber;
                project[i][3] = totdata[i].ProjectName;
                project[i][4] = totdata[i].LegalName;
                project[i][5] = totdata[i].HotelCity;
                project[i][6] = totdata[i].HotelState;
                project[i][7] = totdata[i].HotelCountry;
                project[i][8] = totdata[i].OwnerName;
                project[i][9] = totdata[i].OwnerContact;
                project[i][10] = "<a href='tel:" + totdata[i].OwnerMobile + "'>" + totdata[i].OwnerMobile + "</a>";
                project[i][11] = "<a href='mailto:" + totdata[i].OwnerEmail + "'>" + totdata[i].OwnerEmail + "</a>";
            }
            $('#project').dataTable({
                "processing": true,
                "data": project,
                "columns": [
                    { "title": "ProjectID", "class": "displaynone" },
                    { "title": "Action" },
                    { "title": "ProjectNumber" },
                    { "title": "ProjectName" },
                    { "title": "LegalName" },
                    { "title": "HotelCity" },
                    { "title": "HotelState", "class": "displaynone" },
                    { "title": "HotelCountry" },
                    { "title": "OwnerName" },
                    { "title": "OwnerContact" },
                    { "title": "OwnerMobile" },
                    { "title": "OwnerEmail" }
                ]
                ,
                "initComplete": function () {
                    RowHtmlRender();
                },
                "aoColumnDefs": [
                    { 'bSortable': false, 'aTargets': [0, 1] }
                ]
            });
            var html = "";
            html = "<tr>"
                + "<th><input type='button' value='Reset' onclick='ResetSearch();' style='margin-bottom:6px;' /></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='2' placeholder='Search ProjectNumber'></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='3' placeholder='Search ProjectName'></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='4' placeholder='Search LegalName'></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='5' placeholder='Search HotelCity'></th>"
                //+"<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='6' placeholder='Search HotelState'></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='7' placeholder='Search HotelCountry'></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='8' placeholder='Search OwnerName'></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='9' placeholder='Search OwnerContact'></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='10' placeholder='Search OwnerMobile'></th>"
                + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='11' placeholder='Search OwnerEmail'></th></tr>";
            $('#project thead').append(html);
            var table = $('#project').DataTable();
            $('.srch').on('keyup change', function () {
                var indx = $(this).attr('data-indx');
                var txt = this.value;
                table
                       .column(indx)
                       .search(txt)
                       .draw();
                RowHtmlRender();
            });
        });
    // });
}

function RowHtmlRender() {
    // $('#project thead tr').find("th:eq(1)").html('Action').removeClass('sorting_asc');
    $('#project_filter').css('display', 'none');
    $('#project tbody tr').find("td:eq(1)").each(function () {
        var pid = $(this).prev("td").html();
        ////var zz = $(this).html();
        //$(this).html('<input type="button" value="Edit" onclick="EditProject(this,\'' + pid + '\');" style="margin-bottom:6px;" data-id="' + pid + '" />');
        $(this).html('<input type="button" class="buttonedit btnhover" onmouseover="EditHover(this);" onclick="EditProject(this,\'' + pid + '\');" style="margin-bottom:6px;" data-id="' + pid + '" />');

    });
}

function EditHover(obj) {
    $(obj).focus();
}
//$(document).ready(function(){
//    $(".btnhover").hover(function () {
//        alert(1);
//        $(this).focus();
//    }, function(){

//    });
//});


function ResetSearch() {
    var table = $('#project').DataTable();
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

$(function () {
    $('select[name="project_length"]').live('change', function () {
        RowHtmlRender();
    });
});

function EditProject(obj, pid) {
    //alert(pid);
    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = 'http://www.thebridgebytcg.com/ProjectView/EditProject/' + pid;
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'ProjectView/EditProject/' + pid;
    }
    window.location.href = url1;
}







