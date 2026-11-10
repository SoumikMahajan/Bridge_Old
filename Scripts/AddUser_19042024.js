


function GetCtryID(obj) {
    
    var Cnrty = $(obj).find('option:selected').attr("name").split("_")[1];

    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/adduser/getAllStateByCntryID';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = '/adduser/getAllStateByCntryID';
    }
    
    $.ajax({
        type: "POST",
        url: url,
        async:false,
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
        url = '/adduser/getAllStateByCntryID';
    }

    $.ajax({
        type: "POST",
        url: url,
        async: false,
        data: { 'CountryID': Cnrty },
        success: function (data) {
            $('#RDdl_State').html(data);
        },
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
        url = '/adduser/GetAllCityByStateID';
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
    //$('#Ddl_City').html(data);
    //$('#txt_City').html(data);
}

function GetSuccessDetail(data) {
    $('#Ddl_State').html(data);
}

function failed(data) {
    alert('Failed');
}

function CheckAvailEmail(obj) {
    if ($(obj).val() != '') {
       
        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = '/adduser/CheckEmailAvail';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = '/adduser/CheckEmailAvail';
        }

        $.ajax({
            type: "POST",
            url: url,
            data: { 'EmailID': $(obj).val() },
            success: GetEmailSuccessDetail,
            error: failed
        });
    }
    else {
        $('#txt_Email').focus();
    }
}

function GetEmailSuccessDetail(data) {
    if (data != '') {
        alert('EmailID is used');
        $('#txt_Email').val('');
        $('#txt_Email').focus();
    }
    else {

    }
}

$(document).ready(function () {
    $('.user_panel_container select').change(function () {
       // $(this).next('.showHide').show(0);
    });
});

function errorfunc(obj, message) {

    $('#' + obj).val($('#' + obj).val().trim());
    $('#' + obj).attr('placeholder', message);

    $('#' + obj).effect("shake", { times: 2 }, 700);

    //window.setInterval(function () { $('#' + obj).attr('placeholder', ''); }, 5000);


    $('#' + obj).focus();

}

$(function () {
    $('#btn_CrtUsr').click(function () {
        var LName = $('#txt_LglName').val();
        if (LName.trim() == "") {
            errorfunc('txt_LglName', 'Please enter Legal name');
            return;
        }

        var SName = $('#txt_SName').val();
        if (SName.trim() == "") {
            errorfunc('txt_ShrtName', 'Please enter Short name');
            return;
        }

        var Address1 = $('#txt_Add1').val();
        if (Address1.trim() == "") {
            errorfunc('txt_Add1', 'You must enter an Address1 before continuing');
            return;
        }
        var Address2 = $('#txt_Add2').val();
        if (Address2.trim() == "") {
            errorfunc('txt_Add2', 'You must enter an Address2 before continuing');
            return;
        }
        if ($('#Ddl_Country').val() == '') {
            alert('You must enter Address1 before continue');
            $('#txt_Add1').focus();
            return false;
        }

        if ($('#Ddl_Country').find('option:selected').attr("name").split("_")[1] == 0) {
            alert('You must select Country before continue');
            return false;
        }

        if ($('#Ddl_State').find('option:selected').attr("name").split("_")[1] == 0) {
            alert('You must select a State/Province before continuing');
            return false;
        }
        
        var Email = $('#txt_Email').val();
        if (Email.trim() == "") {
            errorfunc('txt_Email', 'You must enter an Email Address before continuing');
            return;
        }
        if (Email != '') {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
            var eaddress = Email;
            if (Email != '' && reg.test(eaddress) == false) {
                $('#txt_Email').val('');
                errorfunc('txt_Email', 'You must enter an email address in the correct email format.');
                return false;
            }
        }
        //var Password = $('#txt_Password').val();
        //if (Password.trim() == "") {
        //    errorfunc('txt_Password', 'Please enter Password');
        //    return;
        //}
        //var decimal = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$/;
        //var val = Password;
        //if (val.length > 7 && val.length < 16) {
        //    if (val.match(decimal)) {
        //    }
        //    else {
        //        $('#txt_Password').val('');
        //        errorfunc('txt_Password', 'Password Must contains atleast one lower case,one upper case and one digit');
        //        return false;
        //    }
        //}
        //else {
        //    $('#txt_Password').val('');
        //    errorfunc('txt_Password', 'Password must be between 8 to 15 character');
        //    return false;
        //}
        //var ConfrmPswrd = $('#txt_ConfrmPswrd').val();
        //if (Password.trim() != ConfrmPswrd.trim()) {
        //    $('#txt_Password').val('');
        //    $('#txt_ConfrmPswrd').val('');
        //    errorfunc('txt_Password', 'Please enter Password');
        //    return;
        //}
        
        

       


        
        
        if ($('#Ddl_UsrTyp').val() == 0) {
            alert("Please select a UserType");

            $('#Ddl_UsrTyp').focus();
            return;
        }

    });

    //var keylist = "abcdefghijklmnopqrstuvwxyz123456789"
    //var temp = ''

    //function generatepass(plength) {
    //    temp = ''
    //    for (i = 0; i < plength; i++)
    //        temp += keylist.charAt(Math.floor(Math.random() * keylist.length))
    //    return temp
    //}

    function generatepassword() {
        var plength = 8;
        var keylist = 'abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
        var temp = ''
        var isupperchar = false;
        var isnumber = false;
        var islowerchar = false;
        temp = ''
        while (isupperchar == false || isnumber == false || islowerchar == false) {
            temp = '';
            for (i = 0; i < plength; i++) {
                var tmp = keylist.charAt(Math.floor(Math.random() * keylist.length));
                var ccode = tmp.charCodeAt(0);
                if ((ccode > 64 && ccode < 91)) {
                    isupperchar = true;
                }
                else if ((ccode > 96 && ccode < 123)) {
                    islowerchar = true;
                }
                else if (ccode > 47 && ccode < 58) {
                    isnumber = true;
                }
                temp += tmp;
            }
        }
        // alert('isupperchar: ' + isupperchar + ' islowerchar: ' + islowerchar + ' isnumber: ' + isnumber);
        return temp
    }

    $('#btn_CrtClientOrID').click(function () {       
            var FName = $('#txt_FName').val();
            if (FName.trim() == "") {
                errorfunc('txt_FName', 'Please enter First name');
                return;
            }
            var LName = $('#txt_LName').val();
            if (LName.trim() == "") {
                errorfunc('txt_LName', 'Please enter Last name');
                return;
            }
            var UName = $('#txt_UName').val();
            //if (UName.trim() == "") {
            //    errorfunc('txt_UName', 'Please enter User name');
            //    return;
            //}
            var Password = $('#txt_Password').val();

            var chkpassword = Password;

            if (chkpassword.trim() == "") {

                Password = generatepassword();
               //errorfunc('txt_Password', 'Please enter Password');
               //return;
            }
            var decimal = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$/;
            var val = Password;
            if (val.length > 7 && val.length < 16) {
                if (val.match(decimal)) {
                }
                else {
                    $('#txt_Password').val('');
                    errorfunc('txt_Password', 'Password Must contains atleast one lower case,one upper case and one digit');
                    return false;
                }
            }
            else {
                $('#txt_Password').val('');
                errorfunc('txt_Password', 'Password must be between 8 to 15 character');
                return false;
            }
            var ConfrmPswrd = $('#txt_ConfrmPswrd').val();

            if (chkpassword.trim() == "") {
                ConfrmPswrd = Password;
            }

            if (Password.trim() != ConfrmPswrd.trim()) {
                $('#txt_Password').val('');
                $('#txt_ConfrmPswrd').val('');
                errorfunc('txt_Password', 'Password Mismatched');
                return;
            }
            var Email = $('#txt_Email').val();
            if (Email.trim() == "") {
                errorfunc('txt_Email', 'Please enter Email');
                return;
            }
            if (Email != '') {
                var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
                var eaddress = Email;
                if (Email != '' && reg.test(eaddress) == false) {
                    $('#txt_Email').val('');
                    errorfunc('txt_Email', 'Please enter valid Email');
                    return false;
                }
            }


            var Address = $('#txt_Address').val();
            //if (Address.trim() == "") {
            //    errorfunc('txt_Address', 'Please enter Address');
            //    return;
            //}

            //var ZipCode = $('#txt_ZipCode').val();
            //if (ZipCode.trim() == "") {
            //    errorfunc('txt_ZipCode', 'Please enter zip code');
            //    return;
            //}
            
            var Phone = $('#txt_Phone').val();
            //if (Phone.trim() == "") {
            //    errorfunc('txt_Phone', 'Please enter Phone Number');
            //    return;
            //}
            var isok = isValidUSPhoneFormat($('#txt_Phone').val());

            //if (!isok) {
            //    errorfunc('txt_Phone', 'You must enter Phone before continue');
            //    return false;
            //}


            if($('#Ddl_UsrTyp').val() == 0)
            {
                alert("Please select a UserType");

                $('#Ddl_UsrTyp').focus();
                return;
            }
            var scope = angular.element($("#angulardiv")).scope().AddClientorID();
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


function GetHdnVal(obj) {

}


$(function () {
    $('#btn_CrtVendorContact').click(function () {



    var txt_ContactFname = $('#txt_ContactFname').val();
    if (txt_ContactFname.trim() == '') {
        errorfunc('txt_ContactFname', 'Please enter Contact Firstname');
        return;
    }

    var txt_ContactLname = $('#txt_ContactLname').val();
    //if (txt_ContactLname.trim() == '') {
    //    errorfunc('txt_ContactLname', 'Please enter Contact Lastname');
    //    return;
    //}

    //var txt_Add1 = $('#txt_Add1').val();
    //if (txt_Add1.trim() == '') {
    //    errorfunc('txt_Add1', 'Please enter address');
    //    return;
    //}

    //var Ddl_Country = $('#Ddl_Country').val();
    //if (Ddl_Country.trim() == '0') {
    //    alert('Please select country');
    //    $('#Ddl_Country').focus();
    //    return;
    //}

    //var Ddl_State = $('#Ddl_State').val();
    //if (Ddl_State.trim() == '0') {
    //    alert('Please select state');
    //    $('#Ddl_State').focus();
    //    return;
    //}




    //var Ddl_City = $('#Ddl_City').val();
    //if (Ddl_City.trim() == '') {
    //    alert('Please Enter city');
    //    $('#Ddl_City').focus();
    //    return;
    //}

    var txt_ZipCode = $('#txt_ZipCode').val();
    ////if (txt_ZipCode.trim() == '') {
    ////    errorfunc('txt_ZipCode', 'Please enter zipcode');
    ////    return;
    ////}

    var txt_Phone = $('#txt_Phone').val();
    //if (txt_Phone.trim() == '') {
    //    errorfunc('txt_Phone', 'Please enter phone number');
    //    return;
    //}
    var isok = isValidUSPhoneFormat($('#txt_Phone').val());

    //if (!isok) {

    //    errorfunc('txt_Phone', 'You must enter Phone before continue');
    //    return false;

    //}


    var txt_Mobile = $('#txt_Mobile').val();

    var txt_Fax = $('#txt_Fax').val();
    ////if (txt_Fax.trim() == '') {
    ////    errorfunc('txt_Fax', 'Please enter fax number');
    ////    return;
    ////}

    var txt_ContactTitle = $('#txt_ContactTitle').val();
    //if (txt_ContactTitle.trim() == '') {
    //    errorfunc('txt_ContactTitle', 'Please enter contact title');
    //    return;
    //}



    var Email = $('#txt_Email').val();


    if ($('#Chk_IsSystemUser').is(":checked") == true) {
        if (Email.trim() == "") {
            errorfunc('txt_Email', 'You must enter an Email Address before continuing');
            return;
        }
    }

    
    if (Email != '') {
        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
        var eaddress = Email;
        if (Email != '' && reg.test(eaddress) == false) {
            $('#txt_Email').val('');
            errorfunc('txt_Email', 'You must enter an email address in the correct email format.');
            return false;
        }
    }



    var txt_Website = $('#txt_Website').val();
    ////if (txt_Website.trim() == '') {
    ////    errorfunc('txt_Website', 'Please enter website');
    ////    return;
    ////}

    ////var Ddl_IsHiddn = $('#Ddl_IsHiddn').val();
    ////if (Ddl_IsHiddn.trim() == '0') {
    ////    alert('Please select Is hidden field');
    ////    return;
    ////}


    ////var Ddl_IsSystemUser = $('#Ddl_IsSystemUser').val();
    ////if (Ddl_IsSystemUser.trim() == 'select') {
    ////    alert('Please select Is System user field');
    ////    return;
    ////}

    var scope = angular.element($("#angulardiv")).scope().AddVendorContact();


});
});


$(function () {
    $('#btn_CrtPrntVndr').click(function () {
        if ($('#txt_LegalName').hasClass('errorClass') || $('#Ddl_Currency').hasClass('errorClass')) {
            notify("This vendor already exists");
            return;
        }

        if ($('#txt_LegalName').val().trim() == '') {
            //alert('You must enter Legal Name before continue');
            errorfunc('txt_LegalName', 'You must enter Legal Name before continue');
            return false;
        }



        if ($('#txt_Name').val().trim() == '') {
            //alert('You must enter a Vendor Name before continue');
            errorfunc('txt_Name', 'You must enter a Vendor Name before continue');
            return false;
        }

        var Name = $('#txt_Name').val().trim();



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

        var Ctry = $('#Ddl_Country').find('option:selected').attr("name").split("_")[1];
        var State = $('#Ddl_State').find('option:selected').attr("name").split("_")[1];

        var City = $('#txt_City').val().trim();
        ////var City = $('#txt_City').find('option:selected').attr("name").split("_")[1];



        ////if ($('#txt_ZipCode').val().trim() == '') {           
        ////    errorfunc('txt_ZipCode', 'You must enter ZipCode before continue');
        ////    return false;
        ////}

        var ZipCode = $('#txt_ZipCode').val().trim();

        //if ($('#txt_Email').val().trim() == '') {
        //    errorfunc('txt_Email', 'You must enter Email address before continue');
        //    return false;
        //}

        var Email = $('#txt_Email').val().trim();

        //if (Email.trim() == "") {
        //    errorfunc('txt_Email', 'Please enter Email');
        //    return;
        //}

        if (Email != '') {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
            var eaddress = Email;
            if (Email != '' && reg.test(eaddress) == false) {
                $('#txt_Email').val('');
                errorfunc('txt_Email', 'Please enter valid Email');
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

        var Phone = $('#txt_Phone').val().trim();

        var Extension = $('#txt_Extension').val().trim();

        var isok = isValidUSPhoneFormat($('#txt_Phone').val());

        //if (!isok) {

        //    errorfunc('txt_Phone', 'You must enter Phone before continue');
        //    return false;

        //}

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

        var VndrNotes = $('#txt_VndrNotes').val();

        var Isselected = '0';

        Isselected = '1';

        //if ($('#txt_Actv').is(":checked") == true) {
        //    Isselected = '1';
        //}
        //else {
        //    Isselected = '0';
        //}


        var Minority = "";
        var Woman = "";

        if ($("#Minority_check").is(':checked')) {
            Minority = "1";
        }
        else {
            Minority = "0";

        }

        if ($("#Woman_check").is(':checked')) {
            Woman = "1";
        }
        else {
            Woman = "0";

        }

        var url = '';
        if (LOCALORSERVERNEW == 'LOCAL') {
            url = '/adduser/SaveUserDetail';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            url = '/adduser/SaveUserDetail';
        }

        var RAddress = $('#Rtxt_Addr').val().trim();
        var RStatus = $('#Remit_chk').is(':checked') ? '1' : '0';
        var RCity = $('#Rtxt_City').val().trim();
        var RZipCode = $('#Rtxt_ZipCode').val().trim();
        var RState = $('#RDdl_State').find('option:selected').attr("name").split("_")[1];
        var RCtry = $('#RDdl_Country').find('option:selected').attr("name").split("_")[1];
        var txt_Terms = $('#txt_Terms').val();

        if (RStatus == "0") {
            RAddress = Address;
            RCity = City;
            RZipCode = ZipCode;
            RState = State;
            RCtry = Ctry;

        }


        $.ajax({
            type: "POST",
            url: url,
            data: {
                'Name': Name, 'LegalName': LegalName, 'ShortName': ShortName, 'Address': Address, 'City': City, 'Country': Ctry, 'State': State, 'ZipCode': ZipCode,
                'EmailID': Email, 'WebSiteUrl': WebSiteUrl, 'Phone': Phone, 'Extension': Extension, 'Fax': Fax, 'Currency': curr, 'Classification': clfs, 'YTD': YTD,
                'IsActive': Isselected, 'VndrNotes': VndrNotes,
                'RCountry': RCtry, 'RState': RState, 'RZipCode': RZipCode, 'RAddress': RAddress, 'RCity': RCity, 'RStatus': RStatus, 'terms': txt_Terms, 'Minority': Minority, 'Woman': Woman
            },
            success: GetEmailSuccessDetail1,
            error: failedEmail
        });


        function GetEmailSuccessDetail1(data) {
            if (data == 'SUCCESS') {
                //------------------------------ Added By Subhabrata Mondal 7_11_2015---------------------------------------//
                Storefiles(); // to store vendor files in temporarily
                //------------------------------ Added By Subhabrata Mondal 7_11_2015---------------------------------------//

                irrrrr = 0;

                notify('Please add contact of the vendor. If contact is not added the vendor will not be saved.', 'green');

                if (LOCALORSERVERNEW == 'LOCAL') {
                    url = '/adduser/VendorContactsForNewParentVendor';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    url = '/adduser/VendorContactsForNewParentVendor';
                }

                var name = $('#txt_Name').val().trim();

                window.setTimeout(function () {
                    window.location = url + '?Vname=' + name;
                }, 5000);


            }
            else if (data == 'INVALID^EMAIL') {
                $('#txt_Email').val('');
                errorfunc('txt_Email', 'EmailID already exist.You must use another Email address before continue');
                return false;

            }
            else if (data == 'INVALID^ParentVendorName') {
                $('#txt_Name').val('');
                errorfunc('txt_Name', 'Common Name in use. Please try another !!');
                return false;
            }
            else {
                alert('Problem to create Vendor');
            }
        }

        function failedEmail() {
            alert('Something going wrong');
        }






        

    });

    
});

//---------------------------------Added By Subhabrata Mondal 7_11_2015--------------------------------------//

function Storefiles() {

    var formData = new FormData();
    for (var key in filedata) {
        formData.append(key, filedata[key]);
    }

    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = '/adduser/StoreVendorFiles';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = '/adduser/StoreVendorFiles';
    }
    $.ajax({
        type: "POST",
        url: url1,
        data: formData,
        async: false,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            // alert('success!!');
        },
        error: function (error) {
            //alert("errror");
        }
    });
}

//---------------------------------End By Subhabrata Mondal 7_11_2015--------------------------------------//
