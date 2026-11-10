

$(function () {

    var date = new Date();
    var currentMonth = date.getMonth();
    var currentDate = date.getDate();
    var currentYear = date.getFullYear();

    $('#txt_InstallDate').datepickercustom({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',
        altField: "#txt_CompletionDate,#txt_OpeningDate",
        altFormat: "yy.mm.dd",
        minDate: new Date(currentYear, currentMonth, currentDate)
    });

    $('#txt_CompletionDate').datepickercustom({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',
        altField: "#txt_OpeningDate",
        altFormat: "yy.mm.dd",
        minDate: new Date(currentYear, currentMonth, currentDate)
    });

    $('#txt_WareHouseReqDate').datepickercustom({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'MM dd yy',
        altField: "#txt_InstallDate,#txt_CompletionDate,#txt_OpeningDate",
        altFormat: "yy.mm.dd",
        minDate: new Date(currentYear, currentMonth, currentDate)
    });

    $('#txt_OpeningDate').datepickercustom({
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
        url = '/adduser/getAllStateByCntryID';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'adduser/getAllStateByCntryID';
    }

    $.ajax({
        type: "POST",
        url: url,
        async : false,
        data: { 'CountryID': Cnrty },
        success: function (data) {
            if (data!=null) {               
                $('#'+StateBindID).html(data);
            }
            else {
               
            }
        }
    });

}

///////////////////////////////////////////////-----------Project Creation and Updation Realated Operation----------/////////////////////////////////////////////

$(function () {

    $('#btn_CreateProject').click(function () {


        $(this).hide();


        var mode = 'SaveProjectDetail';
        var altMsg = 'Project has been created';

        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = '/Project/SaveProjectDetail';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'Project/SaveProjectDetail';
        }

        ProjectCreationUpdationOptCommon(mode, altMsg, url);       

    });
    
    $('#btn_UpdateProject').click(function () {
        
      


        var mode = 'UpdateProjectDetail';
        var altMsg = 'Project has been Updated';

        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = '/Project/UpdateProjectDetail';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'Project/UpdateProjectDetail';
        }

        ProjectCreationUpdationOptCommon(mode, altMsg, url);             

    });
    
    function ProjectCreationUpdationOptCommon(mode, altMsg, url)
    {

        

        var filestoexpose = '';

        if (altMsg == 'Project has been Updated') {

            $('.downloadablefile').each(function () {

                if ($(this).is(':checked')) {
                    var filename = $(this).next('a').attr('href');
                    filename = filename.substring(filename.lastIndexOf('/') + 1, filename.length);
                    filestoexpose += filestoexpose == '' ? filename + '|||' + 1 : '^^^' + filename + '|||' + 1;
                }
            else {

                        var filename = $(this).next('a').attr('href');
            filename = filename.substring(filename.lastIndexOf('/') + 1, filename.length);
            filestoexpose += filestoexpose == '' ? filename + '|||' + 0 : '^^^' + filename + '|||' + 0;
        }




            });
        }

        if (filestoexpose != '') {

            var PID = $('#ProjectID').val();

            //console.log(filestoexpose);


            var url1 = '';
            if (LOCALORSERVERNEW == 'LOCAL') {
                url1 = '/Project/ProjectFilesToInclude';
            }
            else if (LOCALORSERVERNEW == 'SERVER') {
                url1 = 'Project/ProjectFilesToInclude';
            }


            $.ajax({
                type: "POST",
                async: false,
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                url: url1,
                data: " { \"PID\" : \"" +  PID + "\",\"FILE\" : \"" + filestoexpose + "\"}",
                success: function (data) {

                    
                }
            });




        }
        

    
        var isBridgeSupporter = $('#bridgesupporter').is(':checked') ? 1 : 0;

        var ProjectName = $('#txt_ProjectName').val().trim();
        if (ProjectName == "") {
            errorfunc('txt_ProjectName', 'You must enter a Project Name before saving the Project');
            $('#btn_CreateProject').show();
            return;
        }


        var LegalName = $('#txt_LegalName').val().trim();
      
        var InvAddress = $('#txt_InvAddr').val().trim();
        
        var InvCountry = $('#Ddl_InvCountry').val().trim();
       
        var InvState = $('#Ddl_InvState').val().trim();
       
        var InvCity = $('#txt_InvCity').val().trim();
       
        var InvZipCode = $('#txt_InvZipCode').val().trim();
               var NoUserAppvBdgt = $('#ddl_NoUserAppvBdgt').val().trim();       

        var NoUserAppvPO = $('#ddl_NoUserAppvPO').val().trim();
       
        var ClientID = '0';//$('#txt_Client').val().trim();
       
        var ManagerID = '0';// $('#txt_Manager').val().trim();
      
        var DevInfo = $('#txt_DevInfo').val().trim();
        var HotelAddr = $('#txt_HotelAddr').val().trim();
       
        var HotelCountry = $('#Ddl_HotelCountry').val().trim();
       
        var HotelState = $('#Ddl_HotelState').val().trim();
      
        var HotelCity = $('#txt_HotelCity').val().trim();
        
        var HotelZipCode = $('#txt_HotelZipCode').val().trim();
       
        var ProjectScope = $('#txt_ProjectScope').val().trim();
      
        var OwnerName = $('#txt_OwnerName').val().trim();
       
        var OwnerAddr = $('#txt_OwnerAddr').val().trim();
      
        var OwnerCountry = $('#Ddl_OwnerCountry').val().trim();
       
        var OwnerState = $('#Ddl_OwnerState').val().trim();
       
        var OwnerCity = $('#txt_OwnerCity').val().trim();
        var OwnerZipCode = $('#txt_OwnerZipCode').val().trim();
        
        var OwnerContact = $('#txt_OwnerContact').val().trim();
       

        var OwnerNote = $('#txt_OwnerNote').val().trim();
       

        var OwnerFax = $('#txt_OwnerFax').val().trim();
        

        var OwnerEmail = $('#txt_OwnerEmail').val().trim();

       

        if (OwnerEmail != '') {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
            var eaddress = OwnerEmail;
            if (OwnerEmail != '' && reg.test(eaddress) == false) {
                $('#txt_OwnerEmail').val('');
                errorfunc('txt_OwnerEmail', 'You must enter a valid email format.');
                $('#btn_CreateProject').show();
                return false;
            }
        }

        var OwnerMobile = $('#txt_OwnerMobile').val().trim();
      

        var AccountingContact = $('#txt_AccountingContact').val().trim();
        
        var AccountingPhone = $('#txt_AccountingPhone').val().trim();
        

        var AccountingFax = $('#txt_AccountingFax').val().trim();
        
        var AccountingEmail = $('#txt_AccountingEmail').val().trim();

       

        if (AccountingEmail != '') {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
            var eaddress = AccountingEmail;
            if (AccountingEmail != '' && reg.test(eaddress) == false) {
                $('#txt_AccountingEmail').val('');
                errorfunc('txt_AccountingEmail', 'You must enter a valid email format.');
                $('#btn_CreateProject').show();
                return false;
            }
        }


        var FederalID = $('#txt_FederalID').val().trim();
     
        var GST = $('#txt_GST').val().trim();
        if(GST == ""){
            GST = 0;
        }
        var GSTTYPE = $('#txt_GSTLblShw').val().trim();
        if (GSTTYPE == '') {
            GSTTYPE = 'NOGSTTYPE';
        }
        GST = GST + '^' + GSTTYPE;
       

        var PST = $('#txt_PST').val().trim();
        if (PST == "") {
            PST = 0;
        }
        var PSTTYPE = $('#txt_PSTLblShw').val().trim();
        if (PSTTYPE == '') {
            PSTTYPE = 'NOPSTTYPE';
        }
        PST = PST + '^' + PSTTYPE;
       

        var StateTax = $('#txt_StateTax').val().trim();
        if (StateTax == "") {
            StateTax = 0;
        }
        var StateTaxTYPE = $('#txt_StateTxLblShw').val().trim();
        if (StateTaxTYPE == '') {
            StateTaxTYPE = 'NOStateTaxTYPE';
        }
        StateTax = StateTax + '^' + StateTaxTYPE;
       

        var CurrencyCode = $('#ddl_CurrencyCode').val().trim();
      

        var InstallDate = $('#txt_InstallDate').val().trim();
      
        var CompletionDate = $('#txt_CompletionDate').val().trim();
       

        var WareHouseReqDate = $('#txt_WareHouseReqDate').val().trim();
       

        var OpeningDate = $('#txt_OpeningDate').val().trim();
       
        var ProjectCmt = $('#txt_ProjectCmt').val().trim();
       

        var ProjectContacts = "";

        var OnwnerRequirement = $('#txt_OwnerReqmnt').val().trim();


        var cnt = 0;
        var InstallFle = "";
        $('.getFile').each(function () {
            cnt = 1;
            InstallFle += $(this).attr("name") + "�";
        });

        var ProjectID = $('#ProjectID').val();

        var PrchsOrdrCmmnts = $('#txt_PrchsOrdrCmmnts').val(); 

        var phoneextension = $('#txt_AccountingPhoneExtension').val();
        var ownerphoneextension = $('#txt_OwnerPhoneExtension').val();
      

        $.ajax({
            type: "POST",
            async : false,
            url: url,
            data: {
                'ProjectName': ProjectName, 'LegalName': LegalName, 'InvAddress': InvAddress, 'InvCountry': InvCountry, 'InvState': InvState, 'InvCity': InvCity,
                'InvZipCode': InvZipCode, 'NoUserAppvBdgt': NoUserAppvBdgt, 'NoUserAppvPO': NoUserAppvPO, 'ClientID': ClientID, 'ManagerID': ManagerID, 'DevInfo': DevInfo,
                'HotelAddr': HotelAddr, 'HotelCountry': HotelCountry, 'HotelState': HotelState, 'HotelCity': HotelCity, 'HotelZipCode': HotelZipCode, 'ProjectScope': ProjectScope,
                'OwnerName': OwnerName, 'OwnerAddr': OwnerAddr, 'OwnerCountry': OwnerCountry, 'OwnerState': OwnerState, 'OwnerCity': OwnerCity, 'OwnerZipCode': OwnerZipCode,
                'OwnerContact': OwnerContact, 'OwnerNote': OwnerNote, 'OwnerFax': OwnerFax, 'OwnerEmail': OwnerEmail, 'OwnerMobile': OwnerMobile, 'AccountingContact': AccountingContact,
                'AccountingPhone': AccountingPhone, 'AccountingFax': AccountingFax, 'AccountingEmail': AccountingEmail, 'FederalID': FederalID, 'GST': GST, 'PST': PST, 'StateTax': StateTax,
                'CurrencyCode': CurrencyCode, 'InstallDate': InstallDate, 'CompletionDate': CompletionDate, 'WareHouseReqDate': WareHouseReqDate, 'OpeningDate': OpeningDate,
                'ProjectCmt': ProjectCmt, 'OnwnerRequirement': OnwnerRequirement, 'InstallationFile': InstallFle,
                'mode': mode, 'ProjectID': ProjectID, 'PrchsOrdrCmmnts': PrchsOrdrCmmnts, 'AccPhoneExtension': phoneextension, 'ownerphoneextension': ownerphoneextension,
                'isBridgeSupporter': isBridgeSupporter
            },
            success: function (data) {
                if (data != null) {

                    notify(altMsg, 'green');


                    if (data.split('^')[0] == 'Success') {
                        if (data.split('^')[1] != 0) {
                            uploadfiles(data.split('^')[1], 'TC');
                            uploadfiles(data.split('^')[1], 'PF');
                            uploadfiles(data.split('^')[1], 'ISF');
                            var pid = data.split('^')[1];
                            //------------------------Added By Subhabrata Mondal 9_11_2015------------------------
                            saveVendorCODetails(mode, data.split('^')[1]);
                            //------------------------End By Subhabrata Mondal 9_11_2015------------------------//
                            if (mode == 'SaveProjectDetail') {
                                confirm('Project has been created with Project Number ' + pid);
                            }


                            if (mode == 'SaveProjectDetail') {
                                var url1 = '';
                                if (LOCALORSERVERNEW == 'LOCAL') {
                                    url1 = '/Project/AssignReportPermission';
                                }
                                else if (LOCALORSERVERNEW == 'SERVER') {
                                    url1 = 'Project/AssignReportPermission';
                                }
                                $.ajax({
                                    type: "POST",
                                    url: url1,
                                    data: { 'ProjectID': pid }
                                });
                            }

                            else if (mode == "UpdateProjectDetail") {

                                saveVendorCODetails();

                                //console.log('triggered');

                                notify(altMsg, 'green');
                                irrrrr = 0;
                                window.setTimeout(function () {
                                    window.location.reload();
                                }, 3000);



                                var url2 = '';
                                if (LOCALORSERVERNEW == 'LOCAL') {
                                    url2 = '/Project/UpdateReportPermission';
                                }
                                else if (LOCALORSERVERNEW == 'SERVER') {
                                    url2 = 'Project/UpdateReportPermission';
                                }

                                var permission = '';
                                if ($("#chkBudgetReport").prop('checked') == true) {
                                    permission += 'on' + "^";
                                }
                                else {
                                    permission += 'off' + "^";
                                }

                                if ($("#chkPurchaseReport").prop('checked') == true) {
                                    permission += 'on' + "^";
                                }
                                else {
                                    permission += 'off' + "^";
                                }

                                if ($("#chkStatusReport").prop('checked') == true) {
                                    permission += 'on' + "^";
                                }
                                else {
                                    permission += 'off' + "^";
                                }
                                if ($("#chkAccountingReport").prop('checked') == true) {
                                    permission += 'on'
                                }
                                else {
                                    permission += 'off'
                                }

                                //permission+=$("#chkBudgetReport").val()+"^";
                                //permission += $("#chkPurchaseReport").val() + "^";
                                //permission += $("#chkStatusReport").val() + "^";
                                //permission += $("#chkAccountingReport").val()
                                $.ajax({
                                    type: "POST",
                                    url: url2,
                                    data: { 'ProjectID': (parseInt(data.split('^')[1]) - 1000), 'AccessPermission': permission },
                                    success: function () {
                                        
                                    }
                                });
                            }


                           

                        }
                        //alert(altMsg);
                        var projectid = parseInt(data.split('^')[1]) - 1000;

                        var url = '';
                        if (LOCALORSERVERNEW == 'LOCAL') {
                            url = '/Project/projectteam/';

                        }
                        else if (LOCALORSERVERNEW == 'SERVER') {
                            url = '/Project/projectteam/';

                        }

                        url += projectid;
                        notify(altMsg,'green');
                        if (mode != 'UpdateProjectDetail') {
                            irrrrr = 0;
                            window.location.href = url;
                        }
                        
                    }
                    else if (data.split('^')[0] == 'Fail') {
                        notify('error');
                    }
                    else if (data.split('^')[0] == '') {
                        notify('Project Updated', 'green');
                        irrrrr = 0;
                        window.setTimeout(function () {
                            window.location.reload();
                        }, 3000);
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
            url = '/Project/SaveProjectContactDetail';
            url1 = '/Project/editproject/' + pid;
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'Project/SaveProjectContactDetail';
            url1 = 'Project/editproject/' + pid;
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
            url = '/Project/UpdateProjectContactDetail';
            url1 = '/Project/editproject/' + pid;
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = 'Project/UpdateProjectContactDetail';
            url1 = '/Project/editproject/' + pid;
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
        var ZipCode = $('#zipcode').val();


        var confirmPjtCntSave = confirm(confMsg);

        if (confirmPjtCntSave) {

            $.ajax({
                type: "POST",
                url: url,
                data: {
                    'ContactTypeID': ProjectContactType, 'ContactName': ProjectContactName, 'PAddress': ProjectAddress, 'PCountry': Country, 'PState': ProjectState,
                    'PCity': ProjectCity, 'PTelephone': ProjectTelephone, 'Extension': Extension, 'PMobile': ProjectMobile, 'PFax': ProjectFax,
                    'Email': Email, 'mode': mode, 'ProjectID': ProjectID, 'ZipCode': ZipCode
                },
                success: function (data) {
                    if (data != null) {
                        if (data == 'Success') {
                            notify(alertMsg,'green');

                            irrrrr = 0;
                               

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
            url1 = '/Project/getAllProjectList';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            //url1 = urlsuffix + "/" + 'Project/getAllProjectList';
            url1 = 'Project/getAllProjectList';
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
                    project[i][12] = totdata[i].Bcount;;
                }
                $('#project').dataTable({
                    "processing": true,
                    "data": project,
                    fixedHeader: true,
                    "columns": [
                        { "title": "ProjectID", "class": "displaynone" },
                        { "title": "Action" },
                        { "title": "Project" },
                        { "title": "Name" },
                        { "title": "Legal Name" },
                        { "title": "Hotel City" },
                        { "title": "Hotel State","class" : "displaynone" },
                        { "title": "Country" },
                        { "title": "Owner Name"    },
                        { "title": "Contact" },
                        { "title": "Mobile"  },
                        { "title": "Email" },
                        { "title": "Bcount", "class": "displaynone" },

                    ]
                    ,
                    "initComplete": function () {

                       RowHtmlRender();
                    },
                    "aoColumnDefs": [
                        { 'bSortable': false, 'aTargets': [0, 1] }
                    ]
                });

               // $("#project").colResizable({ liveDrag: true });


                var html = "";
                html = "<tr>"
                    + "<th><input type='button' value='Reset' onclick='ResetSearch();' style='margin-bottom:6px;' /></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='2' style='width:100%; font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='3' style='width:100%; font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='4' style='width:100%; font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='5' style='width:100%; font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                    //+"<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='6' placeholder='Search HotelState'></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='7' style='width:100%;font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='8' style='width:100%;font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='9' style='width:100%;font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='10' style='width:100%;font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                    + "<th rowspan='1' colspan='1'><input class='srch' type='text' data-indx='11' style='width:100%;font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th></tr>";
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
    $('#project_filter').css('display','none');
    $('#project tbody tr').find("td:eq(1)").each(function () {
        var pid = $(this).prev("td").html();
        var bcount = parseInt($(this).parent().find('td:eq(12)').html());
        ////var zz = $(this).html();
        //$(this).html('<input type="button" value="Edit" onclick="EditProject(this,\'' + pid + '\');" style="margin-bottom:6px;" data-id="' + pid + '" />');
        if (bcount == 0) {
            $(this).html('<i title="Edit Project" class="fa fa-pencil-square-o" style="font-size: 1.4em;cursor:pointer;margin-left:5px;" onclick="EditProject(this,\'' + pid + '\');" data-id="' + pid + '" ></i>'
                + '<i title="Delete Project" class="fa fa-trash-o fa-fw" style="font-size: 1.4em;cursor:pointer;" onclick="DeleteProject(this,\'' + pid + '\');" data-id="' + pid + '"></i>');
        }
        else {
            $(this).html('<i title="Edit Project" class="fa fa-pencil-square-o" style="font-size: 1.4em;cursor:pointer;margin-left:5px;" onclick="EditProject(this,\'' + pid + '\');" data-id="' + pid + '" ></i>');

        }
       
    });
}

function EditHover(obj) {    
    $(obj).focus();
}


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
    $(document).on('change','select[name="project_length"]', function () {
        RowHtmlRender();
    });
});

function EditProject(obj, pid) {
    //alert(pid);

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Project/AssignReportPermission';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'Project/AssignReportPermission';
    }
    $.ajax({
        type: "POST",
        url: url1,
        data: { 'ProjectID': pid }
    });


    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Project/EditProject/' + pid;
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = '/Project/EditProject/' + pid;
    }
    irrrrr = 0;
    window.location.href = url1;
}
function DeleteProject(obj, pid) {
    //alert(pid);

    var chck = confirm("Do you really want to delete the project?");
    if (!chck) {
        return;
    }



    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/Project/DeleteProject';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        //url1 = 'Project/DeleteProject/' + pid;
        url1 = 'Project/DeleteProject';
    }
    //alert(url1);
    $.ajax({
        url: url1,
        data: {'id': pid},        
        datatype: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data == 'SUCCESS')
            {
                alert('Data deleted successfully!!');
                //window.location.reload();
                var rw = $(obj).parent().parent();
                var table = $('#project').DataTable();
                table
                .row(rw)
                .remove()
                .draw();
               RowHtmlRender();
            }
            else {
                notify(data);
            }


        },
        failed: function (data) {

        }
    });


}






