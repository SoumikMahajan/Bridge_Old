
replaceAll = function (str, search, replacement) {
    var target = str;


    return target.split(search).join(replacement);
};







(function ($) {
    $.fn.datepickercustom = function (options) {

        return this.each(function () {
            options.dateFormat = 'yy.mm.dd';

            $(this).datepicker(options);
            $(this).removeAttr('onkeypress');
            $(this).removeAttr('onkeyup');

            $(this).attr('onkeyup', 'testkeyup(event,this);');
            $(this).attr('onblur', 'makevaliddate(event,this);');
        });
    };
})(jQuery);
function makevaliddate(e, obj) {
    var d = $(obj).val();
    if (d.trim() == "") {
        return;
    }
    var isok = false;
    if (d.split('.').length == 3) {
        var year = d.split('.')[0];
        var month = parseInt(d.split('.')[1]) - 1;
        var day = d.split('.')[2];
        isok = isValidDate(year, month, day);
    }
    if (!isok) {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();
        if (parseInt(dd) < 10) { dd = "0" + dd; }
        if (parseInt(mm) < 10) { mm = "0" + mm; }


        $(obj).val(yyyy + '.' + mm + '.' + dd);
    }
    else {
        var today = new Date(d);

        var dd = today.getDate();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();
        if (parseInt(dd) < 10) { dd = "0" + dd; }
        if (parseInt(mm) < 10) { mm = "0" + mm; }


        $(obj).val(yyyy + '.' + mm + '.' + dd);

    }


    $(obj).removeClass('errorClass');
}
function testkeyup(e, obj) {
    var d = $(obj).val();
    var isok = false;
    if (d.split('.').length == 3) {
        var year = d.split('.')[0];
        var month = parseInt(d.split('.')[1]) - 1;
        var day = d.split('.')[2];
        isok = isValidDate(year, month, day);
    }
    if (!isok) { $(obj).addClass('errorClass'); }
    else { $(obj).removeClass('errorClass'); }
}
function isValidDate(year, month, day) {
    source_date = new Date(year, month, day);
    if (year != source_date.getFullYear()) {

        return false;
    }
    if (month != source_date.getMonth()) {

        return false;
    }
    if (day != source_date.getDate()) {

        return false;
    }
    return true;
}


alert = function (str) {
    notify(str);
    return false;
}


var LOCALORSERVERNEW = "LOCAL"; //  "SERVER"; //                       

var hostpath = window.location.host;

var urlsuffix = "http://www.ets-dev.com/aspdemo/POMS";



//var urlsuffix = hostpath + "/aspdemo/POMS";

var regGlobal = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;


if (LOCALORSERVERNEW == "SERVER") {

    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) // If Internet Explorer, return version number
    {

        //console.log("MAMA1");
        document.write("<base href='../../' />");

    }
    else  // If another browser, return 0
    {
        //console.log("MAMA2");

        document.write("<base href='https://www.thebridgebytcg.com/' />");
    }




}


// document.write("<base href='http://www.ets-dev.com/aspdemo/poms/' />");
//document.write("<base href='/' />");



var list = new Array();


list.push("purchaseorder/createpurchaseorder".toLowerCase());
list.push("purchaseorder/poedit".toLowerCase());
list.push("purchaseorder/additemstopo".toLowerCase());
list.push("purchaseorder/editpolineitems".toLowerCase());
list.push("adduser/vendor".toLowerCase());
list.push("vendor/editvendor".toLowerCase());
list.push("adduser/vendorcontactsfornewparentvendor".toLowerCase());
list.push("vendor/editcontact".toLowerCase());
list.push("project/createproject".toLowerCase());
list.push("project/editproject".toLowerCase());
//list.push("project/projectteam".toLowerCase());
list.push("project/projectcontacts".toLowerCase());
list.push("project/editprojectcontact".toLowerCase());
list.push("adduser/clientandinteriordesigner".toLowerCase());
list.push("user/adduser".toLowerCase());
list.push("tasklist/taskmanager".toLowerCase());
list.push("invoice/invoiceentry".toLowerCase());
list.push("home/mysettings".toLowerCase());
list.push("Invoice/ManualPayment".toLowerCase());


var page = window.location.pathname.toLowerCase();
var irrrrr = 0;

for (var t = 0; t < list.length ; t++) {

    irrrrr = page.indexOf(list[t]);

    if (irrrrr > 0) {
        t = list.length + 1;
    }
}


var unsavedchanges = false;

$(document).on('keyup', 'input', function () {
    unsavedchanges = true;
    window.onbeforeunload = unsaved;
});


$(document).on('keyup', 'textarea', function () {
    unsavedchanges = true;
    window.onbeforeunload = unsaved;


});

$(document).on('change', 'select', function () {
    unsavedchanges = true;
    window.onbeforeunload = unsaved;
});




// window.onbeforeunload = saved;




function unsaved() {


    // i = 0;

    if (irrrrr > 0) {

        return 'You have unsaved changes!';
    }

}

function saved() {
}



$(function () {
    checkCookie();
});

function checkCookie() {
    var cookieEnabled = (navigator.cookieEnabled) ? true : false;
    if (!cookieEnabled) {
        alert("You have disabled cookies.This site needs cookies enabled.Please go to browser settings and enable cookie.");
    }
}


function errorfunc(obj, message) {

    $('#' + obj).val($('#' + obj).val().trim());
    $('#' + obj).attr('placeholder', message);

    $('#' + obj).effect("shake", { times: 2 }, 700);

    //window.setInterval(function () { $('#' + obj).attr('placeholder', ''); }, 5000);


    $('#' + obj).focus();

}
$(function () {
    $(".resetbtn").click(function () { window.location.reload(); });
});

var tmout;

function notifyprev(text) {

    ////console.log('111');
    clearTimeout(tmout);

    $('.notification').remove();

    var html = "<div class='container alert alert-danger notification col-sm-12' role='alert' style='z-index:99999; cursor: pointer;' onclick='removenotification();'>"

               // + "<span style='float:right;margin-right:-10px;margin-top:-12px;' onclick='removenotification();'>" + "<i class='fa fa-times-circle'></i>" + "</span>"
                + "<span style='width:90%;float:left;'>"
                + "<i class='fa fa-exclamation mrg5R'></i>"
                + text
                + "</span>"
                + "</div>";

    //   html = '<div class="alert alert-danger notification" role="alert">'
    // + '<strong>Oh snap!</strong> Change a few things up and try submitting again.'
    //+ '</div>';
    $('body').append(html);

    tmout = window.setTimeout(function () {

        // $('.notification').hide("drop", 1000);
        $('.notification').fadeOut(3000);

    }, 5000);

}

//text :: The message
//mode  :: color of the background green/blue/yellow/red

function notify(text, mode, time) {

    ////console.log('1');
    var sign = "<i class='fa fa-exclamation-triangle mrg5R'></i>";

    if (mode == undefined) {
        mode = "alert-danger";
    }
    else if (mode.trim().toUpperCase() == "GREEN") {
        mode = "alert-success";
        sign = "<i class='fa fa-check mrg5R'></i>";

    }
    else if (mode.trim().toUpperCase() == "BLUE") {
        mode = "alert-info";
        sign = "<i class='fa fa-check mrg5R'></i>";

    }
    else if (mode.trim().toUpperCase() == "YELLOW") {
        mode = "alert-warning";
        sign = "<i class='fa fa-check mrg5R'></i>";


    }
    else if (mode.trim().toUpperCase() == "RED") {
        mode = "alert-danger";
        sign = "<i class='fa fa-exclamation-triangle mrg5R'></i>";

    }
    else if (mode.trim().toUpperCase() == "YELLOW-LOAD") {
        mode = "alert-warning";
        sign = "<i class='fa fa-spinner fa-spin mrg5R'></i>";

    }
    else {
        mode = "alert-danger";
        sign = "<i class='fa fa-exclamation-triangle mrg5R'></i>";

    }

    clearTimeout(tmout);

    $('.notification').remove();

    var html = "<div class='container alert " + mode + "  notification col-sm-12' role='alert' style='z-index:99999; cursor: pointer;' onclick='removenotification();'>"
                + "<span style='width:90%;float:left;'>"
                + sign
                + text
                + "</span>"
                + "</div>";
    time = time || 5000;
    $('body').append(html);

    tmout = window.setTimeout(function () {
        $('.notification').fadeOut(3000);

    }, time);

}


function removenotification() {

    $('.notification').fadeOut(1000);
}

function checkphone_global(obj) {
    var str = $(obj).val();
    ////console.log(str);

    var patt = /^[+]?[0-9]{0,3}[-]?\(?([0-9]{3,5})\)?([ .-]?)([0-9]{3})\2([0-9]{4,5})$/g;
    var res = patt.test(str);

    //console.log(res + "   >>Validated");
    if (!res && str.trim() != '') {
        // e.preventDefault();
        $(obj).css('background', 'rgb(238, 241, 234)');
        $(obj).css('color', 'red');
    }
    else {
        $(obj).css('background', 'none');
        $(obj).css('color', '#555');

    }
}

$(function () {





    $('.phonenumber').bind('blur', function (e) {
        var obj = e.target;


        var str = $(obj).val();

        var patt = /^[+]?[0-9]{0,3}[-]?\(?([0-9]{3,5})\)?([ .-]?)([0-9]{3})\2([0-9]{4,5})$/g;
        var res = patt.test(str);
        if (!res) {
            // e.preventDefault();
            $(obj).css('background', 'rgb(238, 241, 234)');
            $(obj).css('color', 'red');
        }
        else {
            $(obj).css('background', 'none');
            $(obj).css('color', '#555');

        }


    });

    $('.numberonly,.floatonly').bind("paste", function (e) {
        setTimeout(function () {
            var obj = e.target;
            if (isNaN($(obj).val())) {
                $(obj).val('');
                notify("Please paste valid number");
                return;
            }
            else if ($(obj).hasClass('numberonly')) {
                var qty = parseInt($(obj).val());
                $(obj).val(qty);
                return;
            }
        }, 0);
    });

    $(document).on('keydown', '.floatonly', function (eve) {

        //console.log(eve.which)
        if ($(this).val().length == '1' && (eve.which == 8 || eve.which == 46)) {
            eve.preventDefault();
            $(this).val(0);
        }
    });

    $(document).on('keypress', '.floatonly', function (eve) {



        if ((eve.which != 46 || $(this).val().indexOf('.') != -1) && (eve.which < 48 || eve.which > 57) || (eve.which == 46 && $(this).caret().start == 0)) {
            eve.preventDefault();
        }
        $(document).on('keyup', '.floatonly', function (eve) {
            if ($(this).val().indexOf('.') == 0) {
                $(this).val($(this).val().substring(1));
            }
        });
    });
    $(document).on('keypress', ".numberonly", function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            // notify("Digits only allowed");
            return false;
        }
    });

    $(document).on('keyup', ".postalcode", function (e) {
        $(this).val($(this).val().toUpperCase());
    });



});

$(document).on('keypress', "input:text", function (e) {
    ////console.log('****');
    var maxVal = $(this).attr("data-maxValCheck");

    if (typeof maxVal !== typeof undefined && maxVal !== false) {
        var curval = $(this).val().toString() + String.fromCharCode(e.keyCode);
        if (parseFloat(curval) > parseFloat(maxVal)) {
            e.preventDefault();
        }
    }


});






function setProjectPreferenceBS(obj) {






    var ProjectID = $(obj).find('option:selected').attr('data-id');
    var ProjectID = ProjectID == null ? "" : ProjectID.toString();







    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/Project/setProjectSelectionPreference';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'Project/setProjectSelectionPreference';
    }




    $.ajax({
        url: url,
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        data: "{ 'projectIDs' : '" + ProjectID + "'}",
        failed: function (error) {

        },
        success: function (response) {

        }
    });




}



function setProjectPreference(obj) {






    var ProjectID = $(obj).val();
    var ProjectID = ProjectID == null ? "" : ProjectID.toString();

    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/Project/setProjectSelectionPreference';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'Project/setProjectSelectionPreference';
    }




    $.ajax({
        url: url,
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        data: "{ 'projectIDs' : '" + ProjectID + "'}",
        failed: function (error) {

        },
        success: function (response) {

        }
    });




}






function getProjectPreference() {

    var res = "";
    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/Project/getProjectSelectionPreference';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'Project/getProjectSelectionPreference';
    }




    $.ajax({
        url: url,
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        data: "{}",
        failed: function (error) {

        },
        success: function (response) {
            res = response;
            if (res.split(',').length > 0) {
                res = res.split(',')[0];
            }


        }
    });


    return res;

}


function getProjectPreferencemulti() {

    var res = "";
    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/Project/getProjectSelectionPreference';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'Project/getProjectSelectionPreference';
    }




    $.ajax({
        url: url,
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        data: "{}",
        failed: function (error) {

        },
        success: function (response) {
            res = response;



        }
    });


    return res;

}



//        $(document).ready(function () {
//            alert(1);
//            ck = getCookie("menuhide");
//            ////console.log(ck);

//            if (ck == "true") {

//                $(".up-menu-hide").addClass('displaynone');
//                $('.desktoponly').addClass('desktoponlyhidden');

//                $('.desktoponly').removeClass('desktoponly');
//                $('.desktoponlyhidden').css('display', 'none');


//                $('.up-menu-show').removeClass('displaynone');

//            }
//            else {
//                $(this).addClass('displaynone');
//            }

//            $('.up-menu-hide').click(function () {

//                setCookie("menuhide", "true", 7);

//                $(this).addClass('displaynone');
//                $('.desktoponly').addClass('desktoponlyhidden');

//                $('.desktoponly').removeClass('desktoponly');
//                $('.desktoponlyhidden').css('display', 'none');


//                $('.up-menu-show').removeClass('displaynone');
//            });

//            $('.up-menu-show').click(function () {


//                delete_cookie("menuhide");


//                $(this).addClass('displaynone');

//                $('.desktoponlyhidden').addClass('desktoponly');

//                $('.desktoponlyhidden').removeClass('desktoponlyhidden');
//                //$('.desktoponlyhidden').css('display', 'none');


//                $('.up-menu-hide').removeClass('displaynone');
//            });



//        });


//function setCookie(cname, cvalue, exdays) {
//    var d = new Date();
//    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
//    var expires = "expires=" + d.toUTCString();
//    document.cookie = cname + "=" + cvalue + "; " + expires;
//}

//function delete_cookie(name) {
//    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
//}


//function getCookie(cname) {
//    var name = cname + "=";
//    var ca = document.cookie.split(';');
//    for (var i = 0; i < ca.length; i++) {
//        var c = ca[i];
//        while (c.charAt(0) == ' ') c = c.substring(1);
//        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
//    }
//    return "";
//}

function AddtableTdTextAlign(tableName, colposition, mode, headerEnable) {
    var colpos = colposition.toString().split('^');
    if (headerEnable == false) {
        for (var i = 0; i < colpos.length; i++) {

            $(tableName + " tbody tr").each(function () {
                var tablecell = $(this).find("td:eq(" + parseInt(colpos[i]) + ")");
                $(tablecell).prop("align", mode);

            });
        }

    }

}

function AddtableThTextAlign(tableName, colposition, mode, headerEnable) {
    var colpos = colposition.toString().split('^');
    if (headerEnable == true) {
        for (var i = 0; i < colpos.length; i++) {
            $(tableName + " thead tr").each(function () {
                var tablecell = $(this).find("th:eq(" + parseInt(colpos[i]) + ")");
                $(tablecell).prop("align", mode);

            });
        }
    }
}


$(document).on('keyup', '.emailvalidator', function () {

    var Email = $(this).val();
    var rdd = $(this);
    $(rdd).parent().find('.emailexists').remove();
    $(this).removeClass('errorClass');

    if (Email != '') {
        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.]{2,4})+\.([A-Za-z]{2,4})$/;
        var eaddress = Email;
        if (Email != '' && reg.test(eaddress) == false) {

            $(this).addClass('errorClass');

            return false;
        }
        else {

            var url = '';
            if (LOCALORSERVERNEW == 'LOCAL') {
                url = '/Home/checkemailonkeyup';
            }
            else if (LOCALORSERVERNEW == 'SERVER') {
                url = 'Home/checkemailonkeyup';
            }


            $.ajax({
                url: url,
                async: false,
                type: "POST",
                dataType: "text",
                contentType: "application/json; charset=utf-8",
                data: "{ 'email' : '" + Email + "'}",
                failed: function (error) {

                },
                success: function (response) {

                    if (response != "0") {
                        $(rdd).addClass('errorClass');
                        if ($(rdd).parent().find('.emailexists').length == 0) {
                            $(rdd).prev('label').after('<span class="errorClass emailexists"> (Email Exists) </span>');
                        }
                    }
                    else {
                        $(rdd).parent().find('.emailexists').remove();
                    }
                }
            });
        }
    }
});

$(document).on('click', '.uncheckallbtn', function () {

    var classselected = $(this).attr('data-uncheckclass');

    $('.' + classselected).prop('checked', false);

});


$(document).on('click', '.checkallbtn', function () {

    var classselected = $(this).attr('data-checkclass');

    $('.' + classselected).prop('checked', true);

});















function PreventTwoDecimalPlace(obj, decimalOffset) {

    $(obj).keypress(function (e) {
        var character = String.fromCharCode(e.keyCode)
        var newValue = this.value + character;



        if (newValue.indexOf(".") > -1) {
            if (newValue.split('.')[1].length > decimalOffset) {
                e.preventDefault();
                return false;
            }
        }



    });


}





$(window).scroll(function () {



    if ($('#scrollabletoppos').offset() != undefined) {


        var window_top = $(window).scrollTop();
        var div_top = $('#scrollabletoppos').offset().top || 0;
        if (window_top > div_top) {
            $('#scrollabletopposadd').addClass('topdivfixed');
        }
        else {
            $('#scrollabletopposadd').removeClass('topdivfixed');
        }

    }

    else if ($('#navbar2').offset() != undefined) {

        var window_top = $(window).scrollTop();
        var div_top = $('#navbar2').offset().top || 0;



        if (window_top > 145) {
            ////console.log(1);
            $('#navbar2').addClass('topdivfixed2');
            $('#navbar2').css('width', '110%');

        }
        else {

            $('#navbar2').removeClass('topdivfixed2');
            $('#navbar2').css('width', '100%');

        }
    }



});


$(document).ready(function () {

    var currlocation = window.location.href;



    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/Home/SetUserURL';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'Home/SetUserURL';
    }




    $.ajax({
        url: url,
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        data: "{ 'URL' : '" + currlocation + "'}",
        failed: function (error) {

        },
        success: function (response) {

        }
    });




});


$(document).on('keyup', '.alltoupper', function (e) {
    $(this).val($(this).val().toUpperCase());
});


(function (replace) {                         // Cache the original method
    String.prototype.replace = function () {  // Redefine the method
        // Extra function logic here
        var one_plus_one = 3;
        // Now, call the original method
        ////console.log('HHHIIIIII' + this);
        return replace.apply(this, arguments);
    };
})(String.prototype.replace);


function _multidrop_downupper(e, obj) {
    var key = e.which;
    if (key == 40) {
        var o2 = $(obj).parent().parent().next('ul');
        var i = 0;
        var j = true;
        $(o2).find('li').each(function () {
            var dis = $(this).css('display');
            if (dis != 'none' && j) {
                $(this).find('label').trigger('mouseover');
                j = false;
            }
        });


        //for (var i = 0; i < $('.ui-multiselect-checkboxes li').length; i++) {
        //    var dis = $(this).css('display');
        //    if (dis != 'none') {
        //        $(this).find('label').trigger('mouseover');
        //        i = $('.ui-multiselect-checkboxes li').length;
        //    }

        //}

        // $(obj).focus();
    }
    //console.log(key + "LLLLUUPPOO");
}



$(document).on('change', '.datecontrol', function () {
    var from = $(this).attr('data-from');
    var to = $(this).attr('data-to');
    from = $('#' + from);
    to = $('#' + to);
    var mode = $(this).find('option:selected').attr('data-val');


    var startdate;
    var enddate;

    switch (mode) {
        case 'All':
            startdate = moment().subtract(365, "days").format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;
        case 'last365':
            startdate = moment().subtract(365, "days").format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;
        case 'today':
            startdate = moment().format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;
        case 'custom':
            startdate = moment().format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;
        case 'yesterday':
            startdate = moment().subtract(1, "days").format("YYYY.MM.DD");
            enddate = moment().subtract(1, "days").format("YYYY.MM.DD");
            break;
        case 'thisweek':
            startdate = moment().startOf('week').format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;

        case 'thismonth':
            startdate = moment().startOf('month').format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;
        case 'thisquarter':
            startdate = moment().startOf('quarter').format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;

        case 'thisyear':
            startdate = moment().startOf('year').format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;






        case 'lastweek':
            var tdate = moment().subtract(1, 'week');
            startdate = tdate.startOf('week').format("YYYY.MM.DD");
            enddate = tdate.endOf('week').format("YYYY.MM.DD");
            break;

        case 'lastmonth':
            var tdate = moment().subtract(1, 'month');
            startdate = tdate.startOf('month').format("YYYY.MM.DD");
            enddate = tdate.endOf('month').format("YYYY.MM.DD");
            break;
        case 'lastquarter':
            var tdate = moment().subtract(1, 'quarter');
            startdate = tdate.startOf('quarter').format("YYYY.MM.DD");
            enddate = tdate.endOf('quarter').format("YYYY.MM.DD");
            break;

        case 'lastyear':
            var tdate = moment().subtract(1, 'year');
            startdate = tdate.startOf('year').format("YYYY.MM.DD");
            enddate = tdate.endOf('year').format("YYYY.MM.DD");
            break;


        default:
            startdate = moment().format("YYYY.MM.DD");
            enddate = moment().format("YYYY.MM.DD");
            break;
    }

    if (mode != "custom") {
        $(from).datepicker("option", "disabled", true);
        $(to).datepicker("option", "disabled", true);
    }
    else {
        $(from).datepicker("option", "disabled", false);
        $(to).datepicker("option", "disabled", false);
    }

    $(from).datepicker('setDate', startdate);
    $(to).datepicker('setDate', enddate);
});


function urlchanger(str) {
    if (LOCALORSERVERNEW == "SERVER") {
        if (str.indexOf("thebridgebytcg") < 0) {
            str = ("thebridgebytcg.com/" + str).replace("//", "/");
            return ("http://" + str);
        }
        else {
            return str;
        }
    }
    else {
        return str;
    }
}


var styles = [
    'background: linear-gradient(#204d74, #7792a8)'
    , 'border: 1px solid #3E0E02'
    , 'color: white'
    , 'display: block'
    , 'text-shadow: 0 1px 0 rgba(0, 0, 0, 0.3)'
    , 'box-shadow: 0 1px 0 rgba(255, 255, 255, 0.4) inset, 0 5px 3px -5px rgba(0, 0, 0, 0.5), 0 -13px 5px -10px rgba(255, 255, 255, 0.4) inset'
    , 'line-height: 40px'
    , 'text-align: center'
    , 'font-weight: bold'
    , 'font-size: 20px'
].join(';');

console.log('%c Thank you for using The Bridge By Tcg.', styles);
console.log('%c This console is used by developers for development purpose, please avoid writing codes here to avoid malfunction of Website.', styles);



