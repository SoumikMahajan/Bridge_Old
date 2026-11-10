$(function () {
   
    var url = '';
    var url1 = "~/Home/Login";
    if (LOCALORSERVER == 'LOCAL') {
        //url = 'http://www.thebridgebytcg.com/Logout/Logout';
        url = 'http://thebridgebytcg.com/Logout/Logout';
    }
    else if (LOCALORSERVER == 'SERVER') {
        url = '/Logout/Logout';
    }
    url += '?url=' + url1;

    //    var html = '<a href="' + url + '"><div style="float:right;position:relative;top:-11px;cursor:pointer;">Log Out</div></a><div class="menu-wrap"><nav class="menu"  id="menu"></nav></div>';
    var html = '<a style="float:right;cursor:pointer;margin-top: 60px;font-weight: bolder;font-size: 1.15em;" href="' + url + '">Log Out</a><br style="clear:both;" /><div class="menu-wrap"><nav class="menu"  id="menu"></nav></div><div id="navbar" style="clear:left;" ></div>';

    $('.logo_container_B').after(html);

    //alert(html);

    BreadCrumbBuild();
});




var currmainmenu = "";
var mm = new Array();
var indx = 0;

$(function () {

    var menuhtml = "<ul class='clearfix'>"
    var menulist = new Array();


    var url = '';
    if (LOCALORSERVER == 'LOCAL') {
        url = 'http://thebridgebytcg.com/Menu/MenuBuilder';
    }
    else if (LOCALORSERVER == 'SERVER') {
        url = '/Menu/MenuBuilder';
    }



    $.ajax({
        type: "POST",
        url: url,
        data: { 'userid': '0' },
        success: function (str) {
            //alert(str);
            try {
                alert('&&&');
                menuarraybuild(str);

                menuhtmlbuild();
            }
            catch (Err) {
                var url56 = '';
                if (LOCALORSERVER == 'LOCAL') {
                    url56 = 'http://thebridgebytcg.com/Home/Login';
                }
                else if (LOCALORSERVER == 'SERVER') {
                    url56 = '/Home/Login';
                }

                window.location.href = url56;
            }
        },
        error: function () { notify('Something went wrong'); }
    });
});

function menuarraybuild(str1) {


    var str = new Array();
    str = str1.split(',');
    // console.log(str);
    for (var b in str) {



        var a = str[b];


        var mainmenu = a.split('^')[0];

        var submenu = a.split('^')[1];
        var menuurl = a.split('^')[2];

        // console.log(submenu);

        if (currmainmenu != mainmenu) {
            //console.log(mainmenu + " Built");
            indx = 0;
            mm[mainmenu] = new Array();
            mm[mainmenu][indx] = submenu + "^" + menuurl;
            currmainmenu = mainmenu;


        }
        else {
            //console.log(currmainmenu + "===" + mainmenu);
            //console.log(mainmenu + " Adding");
            indx++;
            mm[mainmenu][indx] = submenu + "^" + menuurl;

        }
    }
}

function menuhtmlbuild() {

   
    var mhtml = "<ul class='clearfix'>";


    var hurl = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        hurl = 'http://www.thebridgebytcg.com/';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        hurl = '';
    }

    hurl += "Home/Dashboard";
    mhtml += "<li>"
            + "	<a href='" + hurl + "'>" + "Home" + "</a>"
            + "</li>";

    var expdturl = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        expdturl = 'http://www.thebridgebytcg.com/';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        expdturl = '';
    }

    expdturl += "Expedite/Home";
    mhtml += "<li>"
            + "	<a href='" + expdturl + "'>" + "Expedite" + "</a>"
            + "</li>";


    for (var m in mm) {



        var mobj = mm[m];

        if (mm[m].length == -1) {

            var ele = mm[m][0];

            mhtml += "<li>"
                + "	<a href='" + ele.split('^')[1] + "'>" + ele.split('^')[0] + "</a>"
                + "</li>";


        }
        else {

            var url = '';
            if (LOCALORSERVERNEW == 'LOCAL') {
                url = 'http://www.thebridgebytcg.com/';
            }
            else if (LOCALORSERVERNEW == 'SERVER') {
                url = '';
            }

            //alert(m);

            var mainmenuurl = '';

            if (m.trim().toLowerCase() == 'purchase orders') {
                if (LOCALORSERVERNEW == 'LOCAL') {
                    mainmenuurl = 'http://www.thebridgebytcg.com/';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    mainmenuurl = '';
                }
                mainmenuurl += 'PurchaseOrder/POList';
            }
            else if (m.trim().toLowerCase() == 'vendors') {
                if (LOCALORSERVERNEW == 'LOCAL') {
                    mainmenuurl = 'http://www.thebridgebytcg.com/';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    mainmenuurl = '';
                }
                //mainmenuurl += 'PurchaseOrder/POList';
                mainmenuurl += 'vendor/allcontacts';
            }
            else if (m.trim().toLowerCase() == 'projects') {
                if (LOCALORSERVERNEW == 'LOCAL') {
                    mainmenuurl = 'http://www.thebridgebytcg.com/';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    mainmenuurl = '';
                }
                //mainmenuurl += 'PurchaseOrder/POList';
                mainmenuurl += 'Project/Projectlist';
            }
            else {

                mainmenuurl = 'javascript:void(0);';
            }

            mhtml += "<li>"
                  + "<a href='" + mainmenuurl + "'>" + m + "</a>"
                  + "<ul class='sub-menu'>";

            for (var i in mm[m]) {

                var ele = mm[m][i];
                mhtml += "<li>"
                        + "<a href='" + url + ele.split('^')[1] + "'>" + ele.split('^')[0] + "</a>"
                        + "</li>";

            }



            mhtml += "</ul></li>";
        }

        //console.log(mhtml);

    }


    mhtml += "</ul>";
    //console.log(mhtml);

    $('#menu').html(mhtml);

}

function BreadCrumbBuild() {
    var html = "<p id='breadcrumb'>" //style='border: none;'
                + "<a href='#'>Home</a>"
                + "<a href='#'>Grandpa</a>"
                + "<a href='#'>Father</a>"
                + "<a href='#'>Son</a>"
                + "Product"
                + "</p>";


    var hurl = '';
    if (LOCALORSERVER== 'LOCAL') {
        hurl = 'http://www.thebridgebytcg.com/';
    }
    else if (LOCALORSERVER == 'SERVER') {
        hurl = '';
    }

    hurl += "Home/Dashboard";
    //alert(URLString().toLowerCase());
    if (URLString().toLowerCase() == '/home/dashboard') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "Dashboard"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorder/createpurchaseorder') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Create Purchase Order"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorder/polist') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Purchase Order List"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorder/deletedpolist') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Deleted Post"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorder/viewpohistory') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Purchase Order History"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorder/poedit') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Edit Purchase Order"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorder/additemstopo') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Add Items To Purchase Order"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorder/editpolineitems') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Edit Purchase Order Line Items"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/budget/budgetsection') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Budgets</a>"
                + "Budget Section"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/budget/lineitems') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Budgets</a>"
                + "Create/Edit Budget"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/adduser/vendor') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Vendors</a>"
                + "Add Parent Vendor"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/adduser/vendorcontactsfornewparentvendor') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Vendors</a>"
                + "Add Vendor Contact under Parent Vendor"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendor/allcontacts') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Vendors</a>"
                + "Vendor Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendor/editvendor') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Vendors</a>"
                + "Edit Vendor Information"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/adduser/vendorcontacts') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Vendors</a>"
                + "Add Vendor Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendor/editcontact') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Vendors</a>"
                + "Edit Vendor Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/createproject') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Projects</a>"
                + "Create Project"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/projectlist') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Projects</a>"
                + "Project Lists"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/editproject') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Projects</a>"
                + "Edit Project"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/projectteam') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Projects</a>"
                + "Project Team"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/projectcontacts') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Projects</a>"
                + "Add Project Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/editprojectcontact') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Projects</a>"
                + "Edit Project Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/adduser/clientandinteriordesigner') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>System Admin</a>"
                + "Add Client or Interior Designer"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/user/adduser') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>System Admin</a>"
                + "Add PC Staff and Administrator"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/expedite/home') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Expedite</a>"
                + "
"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/viewproject') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Project</a>"
                + "View Project"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/systemadmin/systemadminactivity') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>System Admin</a>"
                + "System Admin Activity"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/home/systemlevelpermission') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>System Admin</a>"
                + "System Level permission"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/user/userlist') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>System Admin</a>"
                + "All Users"
                + "</p>";
    }

    //alert(URLString().toLowerCase());

    $('#navbar').after(html);

}

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

function URLString1() {
    //debugger
    var strQueryString = "";
    //alert(window.location.pathname);

    if (window.location.pathname.split('/').length > 3) {
        strQueryString = window.location.pathname.substring(0, window.location.pathname.lastIndexOf('/'));
    }
    else {
        strQueryString = window.location.pathname;
    }


    //alert(LOCALORSERVERNEW + '---------' + strQueryString);
    if (LOCALORSERVERNEW == 'LOCAL') {
        return strQueryString;
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        //alert(strQueryString.replace('/aspdemo/poms', ''));
        return strQueryString.replace('/aspdemo/poms', '');
    }
}function URLString() {
    //debugger
    var strQueryString = "";
    //alert(window.location.pathname);

    var locpathnm = window.location.pathname.toLowerCase().replace('/aspdemo/poms', '');

    if (locpathnm.split('/').length > 3) {
        strQueryString = locpathnm.substring(0, locpathnm.lastIndexOf('/'));
        //alert(locpathnm + '--------' + strQueryString);
    }
    else {
        strQueryString = locpathnm;
    }

    return strQueryString;
}

