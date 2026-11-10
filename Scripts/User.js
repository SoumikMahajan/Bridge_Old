var LOCALORSERVER = "SERVER"; //  "LOCAL";//         
var usdata;
var us = new Array();



var GlobalQS = '';



//$(function () {
//    var url = '';
//    if (LOCALORSERVER == 'LOCAL') {
//        url = '/Scripts/MenuJS.js';
//    }
//    else if (LOCALORSERVER == 'SERVER') {
//        url = 'Scripts/MenuJS.js';
//    }
//    $.getScript(url, function () {
//        // Use anything defined in the loaded script...
//    });
//});







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

        return window.location.search.substring(1, window.location.search.length);
    }
}

function QueryString() {
    //debugger
    var strQueryString = BaseUrlReturn('QS');
    //alert(strQueryString);
}




$(document).ready(function () {
    
   

    $('.chkScrty').change(function () {
       
        var id = $(this).attr('id').split('_')[1];
        var Isselected = '0';

        if ($(this).is(":checked") == true) {
            Isselected = '1';
        }
        else {
            Isselected = '0';
        }

        var url1 = '/Home/UpdateUserSecurity';
        //url1 = 'http://www.ets-dev.com/aspdemo/POMS/Home/UpdateUserSecurity';
        //url1 = 'UpdateUserSecurity';
        

        if (LOCALORSERVER == 'LOCAL')
        {
            url1 = '/Home/UpdateUserSecurity';
        }
        else if (LOCALORSERVER == 'SERVER') {
            url1 = 'Home/UpdateUserSecurity';
        }

        $.ajax({
            type: "POST",
            url: url1,
            data: { 'id': id, 'Isselected': Isselected },
            success: function (comment) {
                //alert(comment);
            }
        });
    });



    $('#btn_UserLogin').click(function () {

        if ($('#txt_UserName').val() == '') {
            notify('Please enter your E-Mail');
            $('#txt_UserName').focus();
        }
        else if ($('#txt_UserPassword').val() == '') {
            notify('Please enter your password');
            $('#txt_UserPassword').focus();
        }
        else {

            var url1 = '/Home/LoginValidate';
            var url2 = '/Home/Dashboard';

            if (LOCALORSERVER === 'LOCAL') {

                url1 = '/Home/LoginValidate';
                url2 = '/Home/Dashboard';
            }
            else if (LOCALORSERVER === 'SERVER') {
                //var gg = window.location.pathname.toLowerCase().indexOf("/");
                
                //if (gg >= 5000) {
                //    url1 = 'LoginValidate';
                //    url2 = 'Dashboard';
                //}
                //else {

                //    url1 = 'Home/LoginValidate';
                //    url2 = 'Home/Dashboard';

                //    var gg1 = window.location.pathname.toLowerCase().indexOf("poms/home/");
                //    if (gg1 >= 0) {
                //        url1 = 'POMS/Home/LoginValidate';
                //        url2 = 'POMS/Home/Dashboard';
                //    }
                //}                
               
            }
            

            $.ajax({
                type: "POST",
                url: url1,
               
                data: { 'UserName': $('#txt_UserName').val(), 'UserPassword': $('#txt_UserPassword').val() },
                success: function (comment) {
                    var a = comment.split('^')[0] === 'SUCCESS';
                    console.log(comment);
                    if (comment.split('^')[0] === 'SUCCESS') {

                        var IsPasswordReset = comment.split('^')[1];
                        if (IsPasswordReset === "0") {
                            $('.login').hide();
                            $('.resetpassworddiv').show();
                            return;
                        }

                        if ($('#chk_Rem').is(':checked')) {
                            setCookie("username", $('#txt_UserName').val(), 365);
                            setCookie("PassWord", $('#txt_UserPassword').val(), 365);
                        }
                        else {
                            setCookie("username", '', -2);
                            setCookie("PassWord", '', -2);
                        }
                        irrrrr = 0;

                        if (comment.split('^')[2].trim() == "") {
                            window.location.href = url2;
                        }
                        else {
                            window.location.href = comment.split('^')[2].trim();
                        }
                    }
                    else {
                        notify('Your user name or password is incorrect. Please check your information and try again.');
                        $('#txt_UserPassword').val('');
                        $('#txt_UserPassword'). focus();
                    }
                }
            });
        }


        
    });



    $('#txt_UserPassword').keypress(function (event) {

        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            if ($('#txt_UserPassword').val() == '') {
                alert('Please enter your password');
            }
            else if ($('#txt_UserName').val() == '') {
                alert('Please enter your user name');
            }
            else {
                $('#btn_UserLogin').click();
            }
        }

    });


});

function chkScrtyChange(obj) {
    var id = $(obj).attr('id').split('_')[0];

    //alert(id);
}

 $(document).ready(function () {        
        checkCookie();
       
        if ($('#txt_UserName').val() == '')
        {
            $('#txt_UserName').focus();
        }
        else if ($('#txt_UserPassword').val() == '')
        {
            $('#txt_UserPassword').focus();
        }

    });

function setCookie(cname, cvalue, exdays) {      
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();       
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) != -1) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function checkCookie() {

    var vl = $('#userprevlogin').val();
    //console.log(vl + "NNNN");
    var user = getCookie("username");
    var pass = getCookie("PassWord");

    if (user != "") {
        $('#txt_UserName').val(user);
        $('#txt_UserPassword').val(pass);
        $('#chk_Rem').prop('checked', true);
        $('#btn_UserLogin').focus();
        //if (vl != "" && vl != null) {
        //    setTimeout(function () {
        //        $('#btn_UserLogin').click();
        //    }, 1000);

        //}

    } else {                 
        setCookie("username", user, 2);
        setCookie("PassWord", user, 2);           
    }
  
}  

function AddUser()
{
    var obj = new Object();
    obj.FirstName = "Turya";
    obj.LastName = "Ban";
}

/********************************************************************************************
			10-11-2014
********************************************************************************************/

$(document).ready(function () {
    $('.col-sm-12 select').change(function () {
        $('.showHide').show(0);
    });
});
$(document).ready(function () {
    $('.forgotTop').click(function () {
        $('.login').slideUp(200);
        $('.forgotBot').slideDown(200);
    });
    $('.cancelBtn').click(function () {
        $('.forgotBot').slideUp(200);
        $('.login').slideDown(200);
    });
});

/********************************************************************************************
			End
********************************************************************************************/
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

$(document).ready(function () {
    $('#btn_CrtUsr').click(function () {

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
      
        var Email = $('#txt_Email').val();
        if (Email.trim() == "") {
            errorfunc('txt_Email', 'Please enter Email');
            return;
        }
        if (Email != '') {
            var reg =  /^[^@\s]+@[^@\s]+\.[^@\s]{2,}$/;
            var eaddress = Email;
            if (Email != '' && reg.test(eaddress) == false) {
                $('#txt_Email').val('');
                errorfunc('txt_Email', 'Please enter valid Email');
                return false;
            }
        }
        if($('#Ddl_UsrTyp').val() == 0)
        {
            alert("Please select a UserType");

            $('#Ddl_UsrTyp').focus();
            return;
        }
        var scope = angular.element($("#angulardiv")).scope().AddUser();
    });
});





        function ForgtPassWrd() {
        
            if (document.getElementById('email').value == '') {
                //document.getElementById("email").style.borderColor = 'red';
                alert('Please enter your e-mail address.');
                document.getElementById('email').focus();
                return false;
            }
            if (document.getElementById('email').value != '') {
                var reg = /^[^@\s]+@[^@\s]+\.[^@\s]{2,}$/;
                var eaddress = document.getElementById("email").value;
                if (reg.test(eaddress) == false) {
                    document.getElementById("email").focus();
                    //document.getElementById("email").style.borderColor = 'red';
                    alert('This is not a valid e-mail address. Please try again.');
                    return false;
                }
                else {
                    //document.getElementById("email").style.borderColor = '';
                }
            }


            var EmailFrgtPass = document.getElementById('email').value;
            var url1 = '/Home/ForgtPassWrd';
            //url1 = 'http://www.ets-dev.com/aspdemo/POMS/Home/ForgtPassWrd';
            //url1 = 'POMS/Home/ForgtPassWrd';


            if (LOCALORSERVER == 'LOCAL') {
                url1 = '/Home/ForgtPassWrd';
            }
            else if (LOCALORSERVER == 'SERVER') {
                ////url1 = 'POMS/Home/ForgtPassWrd';
                //url1 = 'ForgtPassWrd';


                var gg = window.location.pathname.toLowerCase().indexOf("home/");


                if (gg >= 0) {

                    url1 = 'ForgtPassWrd';
                    url1 = '/Home/ForgtPassWrd';
                }
                else {

                    url1 = '/Home/ForgtPassWrd';


                    var gg1 = window.location.pathname.toLowerCase().indexOf("poms/home/");
                    if (gg1 >= 0) {
                        url1 = 'POMS/Home/ForgtPassWrd';

                    }
                }
            }




            $.ajax({
                type: "POST",
                url: url1,
                data: "{EmailFrgtPass:'" + EmailFrgtPass + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "text",
                success: SuccessEmailFrgtPass,
                error: failedEmailFrgtPass
            });

        }


        function changepassword() {

            var newpassword = document.getElementById('password1').value;
            if (document.getElementById('password1').value == '') {
                //document.getElementById("email").style.borderColor = 'red';
                alert('Please enter Password.');
                document.getElementById('password1').focus();
                return false;
            }

            var decimal = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$/;
            var val = newpassword;
            if (val.length > 7 && val.length < 16) {
                if (val.match(decimal)) {
                }
                else {
                    $('#password1').val('');
                    errorfunc('password1', 'Password must contain at least one lower case, one upper case and one digit');
                    return false;
                }
            }
            else {
                $('#password1').val('');
                errorfunc('password1', 'Password must be between 8 to 15 character');
                return false;
            }


            if (document.getElementById('password1').value != document.getElementById('password2').value) {
                alert('Password does not match.');
                document.getElementById('password1').value = '';
                document.getElementById('password2').value = '';
                document.getElementById('password1').focus();
                return false;
            }


           
            var url1 = '/Home/UpdatePassword1';
            //url1 = 'http://www.ets-dev.com/aspdemo/POMS/Home/ForgtPassWrd';
            //url1 = 'POMS/Home/ForgtPassWrd';


            if (LOCALORSERVER == 'LOCAL') {
                url1 = '/Home/UpdatePassword1';
            }
            else if (LOCALORSERVER == 'SERVER') {
                ////url1 = 'POMS/Home/ForgtPassWrd';
                //url1 = 'ForgtPassWrd';


                var gg = window.location.pathname.toLowerCase().indexOf("home/");


                if (gg >= 0) {

                    url1 = '/Home/UpdatePassword1';

                }
                else {

                    url1 = '/Home/UpdatePassword1';


                    //var gg1 = window.location.pathname.toLowerCase().indexOf("poms/home/");
                    //if (gg1 >= 0) {
                    //    url1 = 'POMS/Home/UpdatePassword1';

                    //}
                }
            }


            var newpassword = $('#password1').val();
            var retypenewpassword = $('#password1').val();
            var oldpassword = $('#txt_UserPassword').val();

            $.ajax({
                type: "POST",
                url: url1,
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                data: "{ \"UserID\" : \"" + "" + "\", \"oldpassword\" : \"" + oldpassword + "\", \"newpassword\" : \"" + newpassword + "\", \"retypenewpassword\" : \"" + retypenewpassword + "\"}",
                async: false,
                success: function (data) {
                    irrrrr = 0;
                    if (data == 'SUCCESS') {

                        alert('Password successfully changed!');
                        if (LOCALORSERVER == 'SERVER') {
                            window.location.href = "";
                        }
                        else {
                            window.location.reload();
                        }
                    }
                    else {
                        notify(data);
                    }
                },
                error: function () { alert('Something went wrong'); }
            });



        }

        function SuccessEmailFrgtPass(data) {
            
            if (data == "Failed") {
                alert('This is not a valid e-mail address. Please try again.');
            }
            else {
                notify('An e-mail has been sent to you.  Please check your e-mail to reset your account password.','green');
            }
            $('#email').val('');
            

        }
function failedEmailFrgtPass(data) {
    //if (data.d != '') {
    //    //Notificationa('Some Error Occured' + '!', 'ERR');
    //}
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}


$(document).ready(function () {


    //var url = '';
    //if (LOCALORSERVER == 'LOCAL') {
    //    url = '/Scripts/Menu1.js';
    //}
    //else if (LOCALORSERVER == 'SERVER') {
    //    url = 'Scripts/Menu1.js';
    //}
    //$.getScript(url, function () {
    //    // Use anything defined in the loaded script...
    //});



    $('#btn_ResetPassword').click(function () {

        

        if ($('#txt_NewPassword').val() == '') {
            alert('Please enter your password');
        }
        

        if (CheckPassword() == false) {
            return;
        }


        if ($('#txt_NewPassword').val() != $('#txt_NewConfirmPassword').val()) {

            alert('The passwords that you  entered do not match. Please ensure that they are the same.');
            return;
        }

        

        var url1 = '/Home/ResetPasswordConfirm';
        var url2 = '/';


        //url1 = 'http://www.ets-dev.com/aspdemo/POMS/Home/ResetPasswordConfirm';
        //url2 = 'http://www.ets-dev.com/aspdemo/POMS/';
        //url1 = 'POMS/Home/ResetPasswordConfirm';
        //url2 = 'POMS/';




        if (LOCALORSERVER == 'LOCAL') {
            url1 = '/Home/ResetPasswordConfirm';
            url2 = '/';
        }
        else if (LOCALORSERVER == 'SERVER') {   
            url1 = '/Home/ResetPasswordConfirm';
            url2 = '..';
            url2 = '/';
            
            var gg1 = window.location.pathname.toLowerCase().indexOf("poms/home/");
            if (gg1 >= 0) {
                url2 = '../../POMS';
            }
            else {
                url2 = '../../';
            }           
        }

        console.log(url1);
        //return;

        var token = getParameterByName('token');


        var Password = $('#txt_NewPassword').val();

        $.ajax({
            type: "POST",
            url: url1,
            data: { 'token': token, 'Password': Password },
            success: function (comment) {
                alert('Password reset successfully');
                irrrrr = 0;
                window.location.href = url2;
            }
        });

    });
});

    
function CheckPassword() {
    var decimal = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$/;

    var val = $('#txt_NewPassword').val();

    if (val.length > 7 && val.length < 16) {
        if (val.match(decimal)) {            
            return true;
        }
        else {
            alert('Password Must contains atleast one lower case,one upper case and one digit');
            return false;
        }
    }
    else {
        alert('Password must be between 8 to 15 character');
        return false;
    }
}



function clicK() {

    var url1 = '/User/getAllUserList';

    if (LOCALORSERVER == 'LOCAL') {
        url1 = '/User/getAllUserList';
    }
    else if (LOCALORSERVER == 'SERVER') {
        url1 = 'getAllUserList';
    }


    $(document).ready(function () {

        try{

            var totdata;


            $.post(url1)
                .success(function (response) {

                    totdata = JSON.parse(response);
                   
                    usdata = totdata.data;
                    

                   


               

                    for (i = 0; i < usdata.length ; i++) {

                        us[i] = new Array();

                        us[i][0] = "<i class='fa fa-hand-o-right'></i>";
                        //us[i][1] = "<input class='de_activate1 btn_activate' type='button' value='Deactivate'><input class='re_activate1 btn_activate' type='button' value='Activate'>";
                        us[i][1] = '<i class="fa fa-times-circle de_activate1 btn_activate" style="font-size: 20px;"></i><i class="fa fa-check-circle re_activate1 btn_activate" style="font-size:20px;"></i>';
                        us[i][2] = usdata[i].FldID;
                        us[i][3] = usdata[i].Firstname;
                        us[i][4] = usdata[i].Lastname;
                        us[i][5] = "<a href='mailto:"+usdata[i].Email+"'>"+usdata[i].Email+"</a>";
                        us[i][6] = usdata[i].Usertype;
                        if (usdata[i].Isbuyer == "1")

                            us[i][7] = "Yes";
                        else
                            us[i][7] = "No";

                        us[i][8] = usdata[i].Isactive;
                    }

                

                    $('#example').dataTable({
                        "processing": true,
                        "data": us
                        , "columns": [
                            { "title": "Action", "class": "MddleAlgnment" },
                            { "title": "Activation", "class": "toggle ForIcons" },
                            { "title": "User ID" },
                            { "title": "First Name" },
                            { "title": "Last Name" },
                            { "title": "E-mail" },
                            { "title": "User Type" },
                            {
                                "title": "Buyer"
                            },
                            { "title": "Isactive", "class": "displaynone" },
                            {
                                "data": null,
                                "class": "toggle displaynone",
                                "defaultContent": "<input class='de_activate1 btn_activate' type='button' value='Deactivate'><input class='re_activate1 btn_activate' type='button' value='Activate'>"
                            }
                        ]
                        ,
                        "initComplete": function () {
                            $('#example thead tr').find("th:eq(0)").removeClass('sorting_asc');

                            RowHtmlRender();
                        },
                        "aoColumnDefs": [
                            { 'bSortable': false, 'aTargets': [0,1,6] }
                        ]
                        , "fnDrawCallback": function (oSettings) {
                            RowHtmlRender();
                        }
                        //,"columnDefs": [{ "targets": [0,1,6], "orderable": false }]

                    });









                    //$('#example tbody').on('click', '.btn_activate', function () {
                    //    // $(this).parent().parent().find("td:eq(2)").html('<input type="checkbox">');
                    //    var data = $(this).parent().parent().find("td:eq(2)").html();


                    //});

                    $('#example tbody').on('click', '.btn_activate', function () {
                        // $(this).parent().parent().find("td:eq(2)").html('<input type="checkbox">');
                        var data = $(this).parent().parent().find("td:eq(4)").html();


                    });

                    var html = "";


                    var html = "<tr>"
                        + "<th rowspan='1' colspan='1'>"
                        + "<input type='button' value='Reset' id='resetsrch' class='reset_activate'>"
                        + "</th>"
                        + "<th rowspan='1'  colspan='1'>"
                        + "<select style='width:100%;padding: 2px;' data-indx='8' class='srch' id='cksrch7'>"
                        + "<option value=''>All</option>"
                        + "<option value='1'>Active</option>"
                        + "<option value='0'>Deactive</option>"
                        + "</select>"
                        + "</th>"
                        + "<th rowspan='1' colspan='1'>"
                        + "<input type='text' class='srch' id='cksrch1' data-indx='2' style='width:100%;padding: 2px; font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                        + "<th rowspan='1' colspan='1'>"
                        + "<input type='text' data-indx='3' id='cksrch2' class='srch' style='width:100%;padding: 2px; font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                        + "<th rowspan='1' colspan='1'>"
                        + "<input type='text' data-indx='4' id='cksrch3' class='srch' style='width:100%;padding: 2px; font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                        + "<th rowspan='1' colspan='1'>"
                        + "<input type='text' data-indx='5' id='cksrch4' class='srch' style='width:100%;padding: 2px; font-family:Arial, FontAwesome' placeholder=' &#xF002;'></th>"
                        + "<th rowspan='1' colspan='1'>"
                        + "<select data-indx='6' id='cksrch5' class='srch'>"
                        + "<option value=''>All</option>"
                        + "<option value='Administrator'>Administrator</option>"
                        + "<option value='Buyer'>Buyer</option>"
                        + "<option value='Client/Operator'>Client/Operator</option>"
                        + "<option value='Consultant'>Consultant</option>"
                        + "<option value='Accountant'>Accountant</option>"
                        + "</select>"                        
                        + "<th rowspan='1' colspan='1'>"
                        + "<select data-indx='7' id='cksrch6' class='srch'>"
                        + "<option value=''>All</option>"
                        + "<option value='yes'>Yes</option>"
                        + "<option value='no'>No</option>"
                        + "</select>"
                        + "</th>"
                        + "<th rowspan='1' colspan='1' class='displaynone'>"
                        + "<input type='button' value='Reset' id='resetsrch' class='reset_activate'>"
                        + "</th>"
                        + "</tr>";


                  
                    
                  








                    $('#example thead').append(html);

                    var table = $('#example').DataTable();

                    $('.srch').each(function () {

                        var cval = 'ck_' + $(this).attr('id');

                        if (getCookie(cval) != "") {
                            $(this).val(getCookie(cval));
                            var indx = $(this).attr('data-indx');
                            var txt = this.value;

                            var cval = 'ck_' + $(this).attr('id');
                            


                            if (indx == 6) {


                                if (txt != '') {
                                    txt = '^' + txt + '$';
                                }
                                table
                                       .column(indx)
                                       .search(txt, true, false)
                                       .draw();
                                RowHtmlRender();
                            }
                            else {
                                table
                                       .column(indx)
                                       .search(txt)
                                       .draw();
                                RowHtmlRender();
                            }

                            
                        }

                    });


                    // DataTable
                   

                   
                    $('.srch').on('keyup change', function () {
                       
                        var indx = $(this).attr('data-indx');
                        var txt = this.value;

                        var cval = 'ck_' + $(this).attr('id');
                        delete_cookie(cval);
                        //setCookie(cval, txt, 7)

                        
                        if (indx == 6) {

                            
                            if (txt != '') {
                                txt = '^' + txt + '$';
                            }
                            table
                                   .column(indx)
                                   .search(txt, true, false)
                                   .draw();
                            RowHtmlRender();
                        }
                        else {
                            table
                                   .column(indx)
                                   .search(txt)
                                   .draw();
                            RowHtmlRender();
                        }
                    });

                    $('#resetsrch').click(function () {

                     

                           
                        $('.srch').val('');

                        $('.srch').each(function () {

                            var cval = 'ck_' + $(this).attr('id');
                            delete_cookie(cval);
                            var indx = $(this).attr('data-indx');

                            table
                                        .column(indx)
                                        .search(this.value)
                                        .draw();
                            RowHtmlRender();

                        });


                    });

                    /*-----------------------------Added By Subhabrata Mondal 6_11_2015------------------------------*/

                   
                   // table.column(8).search(1).draw();

                    /*-----------------------------Added By Subhabrata Mondal 6_11_2015------------------------------*/

                    // Apply the search
               
                });
        }
        catch (err) {
        }

        });
    

}







$(document).ready(function () {
   
    clicK();

    $('.paginate_button').live('click', function () {
        RowHtmlRender();
    });


   

});

function RowHtmlRender() {
 
    ////$('#example tbody tr').find("td:eq(1)").each(function () {
    ////    var zz = $(this).html();
    ////    var yy = $(this).parent().find("td:eq(0)").html();
        
    ////    $(this).html('<a href="javascript:void(0);" onclick="EditPOPUP(\'' + yy + '\')" >' + zz + '</a>');
        
    ////});    

    //$('#example tbody tr').find("td:eq(3)").each(function () {
    //    var zz = $(this).html();
    //    var yy = $(this).parent().find("td:eq(2)").html();

    //    $(this).html('<a href="javascript:void(0);" onclick="EditPOPUP(\'' + yy + '\')" >' + zz + '</a>');

    //});
    $('#example tbody tr').find("td:eq(0)").each(function () {
        var zz = $(this).html();
        var yy = $(this).parent().find("td:eq(2)").html();

        $(this).html('<i class="fa fa-hand-o-right" style="cursor:pointer;" onclick="EditPOPUP(\'' + yy + '\')"></i>');
        //'<a href="javascript:void(0);" onclick="EditPOPUP(\'' + yy + '\')" >' + zz + '</a>');

    });

    //$('#example tbody tr').find("td:eq(7)").each(function () {
    //    var zz1 = $(this).html();
    //    var yy1 = $(this).parent().find("td:eq(0)").html();
    //    var xx = $(this).parent().find("td:eq(6)").html();
    //    if (xx == 1) {
    //        $(this).html('<input class="de_activate1 btn_activate ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');
    //    }
    //    else {
    //        $(this).html('<input class="de_activate1 btn_activate displaynone ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input style="display:block" class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');

    //    }
    //});


    $('#example tbody tr').find("td:eq(9)").each(function () {
        var zz1 = $(this).html();
        var yy1 = $(this).parent().find("td:eq(2)").html();
        var xx = $(this).parent().find("td:eq(8)").html();
        if (xx == 1) {
            $(this).html('<input class="de_activate1 displaynone btn_activate ' + xx + '" type="button" value="Deactivate" id="reactivate_' + yy1 + '"><input class="re_activate1 btn_activate" type="button" value="Activate" id="deactivate_' + yy1 + '">');
        }
        else {
            $(this).html('<input class="de_activate1 btn_activate  ' + xx + '" type="button" value="Deactivate" id="reactivate_' + yy1 + '"><input style="display:block" class="re_activate1 btn_activate displaynone" type="button" value="Activate" id="deactivate_' + yy1 + '">');

        }
    });
    $('#example tbody tr').find("td:eq(1)").each(function () {
        var zz1 = $(this).html();
        var yy1 = $(this).parent().find("td:eq(2)").html();
        var xx = $(this).parent().find("td:eq(8)").html();
        //if (xx == 1) {
        //    $(this).html('<input class="de_activate1 btn_activate ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');
        //}
        //else {
        //    $(this).html('<input class="de_activate1 btn_activate displaynone ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input style="display:block" class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');

        //}

        if (xx == 1) {
            $(this).html('<i class="fa fa-times-circle de_activate1 displaynone btn_activate ' + xx + '" id="reactivate_' + yy1 + '" style="cursor:pointer;font-size:20px;"></i><i class="fa fa-check-circle re_activate1 btn_activate" id="deactivate_' + yy1 + '" style="cursor:pointer;font-size:20px;"></i>');
        }
        else {
            $(this).html('<i class="fa fa-times-circle de_activate1 btn_activate  ' + xx + '" id="reactivate_' + yy1 + '" style="cursor:pointer;font-size:20px;"></i><i class="fa fa-check-circle re_activate1 btn_activate displaynone" id="deactivate_' + yy1 + '" style="display:block;cursor:pointer;font-size:20px;"></i>');
        }

        //////////if (xx == 1) {
        //////////    $(this).html('Activate');
        //////////}
        //////////else {
        //////////    $(this).html('Deactivate');

        //////////}
    });


    //$('.toggle .de_activate1').on('click', function () {
    //    $(this).hide();
    //    $(this).parent().parent().find("td:eq(6)").html('0');
       
    //    $(this).next('.re_activate1').show();
    //    var deactivate = $(this).attr('id');
    //    var deactivemode = deactivate.split('_')[0];
    //    var deactiveUID = deactivate.split('_')[1];
    //    activatedeactivateUser(deactivemode, deactiveUID);
    //});


    $('.toggle .de_activate1').on('click', function () {

       

        $(this).hide();
        $(this).parent().parent().find("td:eq(8)").html('0');

        $(this).next('.re_activate1').show();
        var deactivate = $(this).attr('id');
        var deactivemode = deactivate.split('_')[0];
        var deactiveUID = deactivate.split('_')[1];
        var rowPos = 0;
        var columnPos = 8;
        for (var i = 0; i < us.length; i++) {
            var fid = us[i][2];
            if (fid == deactiveUID) {
                var table = $('#example').DataTable();
                table.cell(rowPos, columnPos).data('0').draw();
            }
            rowPos++;
        }
        activatedeactivateUser(deactivemode, deactiveUID);
    });


    //$('.toggle .re_activate1').on('click', function () {
    //    $(this).hide();
    //    $(this).parent().parent().find("td:eq(6)").html('1');
    //    $(this).prev('.de_activate1').show();
    //    var reactivate = $(this).attr('id');
    //    var reactivemode = reactivate.split('_')[0];
    //    var reactiveUID = reactivate.split('_')[1];
    //    activatedeactivateUser(reactivemode, reactiveUID);
    //});

    $('.toggle .re_activate1').on('click', function () {
        $(this).addClass('displaynone');
        $(this).parent().parent().find("td:eq(8)").html('1');
        $(this).prev('.de_activate1').removeClass('displaynone');
      
        var reactivate = $(this).attr('id');
        var reactivemode = reactivate.split('_')[0];
        var reactiveUID = reactivate.split('_')[1];
        var rowPos = 0;
        var columnPos = 8;
        for (var i = 0; i < us.length; i++) {
            var fid = us[i][2];
            if (fid == reactiveUID) {
                var table = $('#example').DataTable();
                table.cell(rowPos, columnPos).data('1').draw();
            }
            rowPos++;
        }
        activatedeactivateUser(reactivemode, reactiveUID);
    });
  
}




function RowHtmlRenderNew() {

    ////$('#example tbody tr').find("td:eq(1)").each(function () {
    ////    var zz = $(this).html();
    ////    var yy = $(this).parent().find("td:eq(0)").html();

    ////    $(this).html('<a href="javascript:void(0);" onclick="EditPOPUP(\'' + yy + '\')" >' + zz + '</a>');

    ////});

    //$('#example tbody tr').find("td:eq(3)").each(function () {
    //    var zz = $(this).html();
    //    var yy = $(this).parent().find("td:eq(2)").html();

    //    $(this).html('<a href="javascript:void(0);" onclick="EditPOPUP(\'' + yy + '\')" >' + zz + '</a>');

    //});

    $('#example tbody tr').find("td:eq(0)").each(function () {
        var zz = $(this).html();
        var yy = $(this).parent().find("td:eq(2)").html();

        
        $(this).html('<i class="fa fa-hand-o-right" style="cursor:pointer;" onclick="EditPOPUP(\'' + yy + '\')"></i>');
        //$(this).html('<a href="javascript:void(0);" onclick="EditPOPUP(\'' + yy + '\')" >' + zz + '</a>');

    });


    

    //$('#example tbody tr').find("td:eq(7)").each(function () {
    //    var zz1 = $(this).html();
    //    var yy1 = $(this).parent().find("td:eq(0)").html();
    //    var xx = $(this).parent().find("td:eq(6)").html();
    //    if (xx == 1) {
    //        $(this).html('<input class="de_activate1 btn_activate ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');
    //    }
    //    else {
    //        $(this).html('<input class="de_activate1 btn_activate displaynone ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input style="display:block" class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');

    //    }
    //});

    $('#example tbody tr').find("td:eq(9)").each(function () {
        var zz1 = $(this).html();
        var yy1 = $(this).parent().find("td:eq(2)").html();
        var xx = $(this).parent().find("td:eq(8)").html();
        if (xx == 1) {
            $(this).html('<input class="de_activate1 btn_activate ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');
        }
        else {
            $(this).html('<input class="de_activate1 btn_activate displaynone ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input style="display:block" class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');

        }
    });

    $('#example tbody tr').find("td:eq(1)").each(function () {
        var zz1 = $(this).html();
        var yy1 = $(this).parent().find("td:eq(2)").html();
        var xx = $(this).parent().find("td:eq(8)").html();
        //if (xx == 1) {
        //    $(this).html('<input class="de_activate1 btn_activate ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');
        //}
        //else {
        //    $(this).html('<input class="de_activate1 btn_activate displaynone ' + xx + '" type="button" value="Deactivate" id="deactivate_' + yy1 + '"><input style="display:block" class="re_activate1 btn_activate" type="button" value="Activate" id="reactivate_' + yy1 + '">');

        //}

        if (xx == 1) {
            $(this).html('<i class="fa fa-times-circle de_activate1 btn_activate ' + xx + '" id="deactivate_' + yy1 + '" style="cursor:pointer;font-size:20px;"></i><i class="fa fa-check-circle re_activate1 btn_activate displaynone" id="reactivate_' + yy1 + '" style="cursor:pointer;font-size:20px;"></i>');
        }
        else {
            $(this).html('<i class="fa fa-times-circle de_activate1 btn_activate displaynone ' + xx + '" id="deactivate_' + yy1 + '" style="cursor:pointer;font-size:20px;"></i><i style="display:block" class="fa fa-check-circle re_activate1 btn_activate" id="reactivate_' + yy1 + '" style="cursor:pointer;font-size:20px;"></i>');

        }
    });
}


function activatedeactivateUser(mode, UID) {
    
    var url2 = '';

    if (LOCALORSERVER == 'LOCAL') {
        url2 = '/User/activatedeactivateUser';
    }
    else if (LOCALORSERVER == 'SERVER') {
        // url2 = 'activatedeactivateUser';
        url2 = '/User/activatedeactivateUser';
    }


    if (mode == 'reactivate') {
        //alert('r' + UID);

        $.ajax({
            type: "POST",
            url: url2,
            data: { 'mode': 'REACTIVE', 'UID': UID },
            success: function (comment) {
                notify('User is Activated', 'green');
                window.location.reload();
                
            }
        });
        
    }
    else if (mode == 'deactivate') {
        // alert('d' + UID);

        $.ajax({
            type: "POST",
            url: url2,
            data: { 'mode': 'DEACTIVE', 'UID': UID },
            success: function (comment) {
                alert('User is Deactivated');
                window.location.reload();
            }
        });
    }

}

 
function errorfunc(obj, message) {

    $('#' + obj).val($('#' + obj).val().trim());
    $('#' + obj).attr('placeholder', message);

    $('#' + obj).effect("shake", { times: 2 }, 700);

    //window.setInterval(function () { $('#' + obj).attr('placeholder', ''); }, 5000);


    $('#' + obj).focus();

}


function enabledisablepasswrdfld(obj) {   
    if ($(obj).is(":checked") == false) {
        $('#txt_Password').val('');
        $('#txt_ConfrmPswrd').val('');
        

        $('#txt_Password').attr('disabled', true);
        $('#txt_ConfrmPswrd').attr('disabled', true);
        
    }
    else {

        $('#txt_Password').val('');
        $('#txt_ConfrmPswrd').val('');
        


        $('#txt_Password').removeAttr('disabled');
        $('#txt_ConfrmPswrd').removeAttr('disabled');
        
    }
}


$(document).ready(function () {

    $('#btn_EdtUsr').click(function () {

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

        if ($('#chk_ChngPasswrd').is(':checked')) {

            var Password = $('#txt_Password').val();
            if (Password.trim() == "") {
                errorfunc('txt_Password', 'Please enter Password');
                return;
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
            if (Password.trim() != ConfrmPswrd.trim()) {
                $('#txt_Password').val('');
                $('#txt_ConfrmPswrd').val('');
                errorfunc('txt_Password', 'Password Mismatched !!');
                return;
            }
        }





        var Email = $('#txt_Email').val();
        if (Email.trim() == "") {
            errorfunc('txt_Email', 'Please enter Email');
            return;
        }
        if (Email != '') {
            var reg = /^[^@\s]+@[^@\s]+\.[^@\s]{2,}$/;
            var eaddress = Email;
            if (Email != '' && reg.test(eaddress) == false) {
                $('#txt_Email').val('');
                errorfunc('txt_Email', 'Please enter valid Email');
                return false;
            }
        }

        if (!$('#UsrAddress').css('display') == 'none') {
            var Address = $('#txt_Address').val();
            if (Address.trim() == "") {
                errorfunc('txt_Address', 'Please enter Address');
                return;
            }

            var Phone = $('#txt_Phone').val();
            if (Phone.trim() == "") {
                errorfunc('txt_Phone', 'Please enter Phone Number');
                return;
            }
            var isok = isValidUSPhoneFormat($('#txt_Phone').val());

            if (!isok) {
                errorfunc('txt_Phone', 'You must enter Phone before continue');
                return false;
            }

        }

        var scope = angular.element($("#angulardiv")).scope().EditUser();


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

$(document).ready(function () {
    try {
        $('#txt_Address').autosize();
    }
    catch (err) {
        
    }
   

});

$(function () {
    $('select[name="example_length"]').live('change', function () {
       // alert(2);
        RowHtmlRender();
    });
});

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
        success: function (data) {
            $('#Ddl_State').html(data);
        },
        error: function () {
            alert('Error');
        }
    });
}


function GetCtryIDByID(obj,stateid) {

    var Cnrty = obj;

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
        success: function (data) { 
            $('#Ddl_State').html(data);

            
                $('#Ddl_State').val(stateid);
           
        },
        error: function () {
            alert('Error');
        }
    });
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}

function delete_cookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function enableUserTypeEdit(obj) {
    if (confirm("Are you sure you want to change the User Type?")) {
        $('#Ddl_UserType').removeAttr('disabled');
        $(obj).remove();
    }
    else
        return;
}