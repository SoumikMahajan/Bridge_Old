$(function () {


    //alert(html);

    BreadCrumbBuild();
});


var GlobalReportsForMenu = new Array();

var currmainmenu = "";
var mm = new Array();
var indx = 0;

var menuflag = false;
var invoicemenuflag = false;
var budgetmenuflag = false;
var projectmenuflag = false;
var vendormenuflag = false;
var purchaseorderflag = false;
var systemadminflag = false;


$(function () {
    debugger;
    //var menuhtml = "<ul class='clearfix'>"
    var menuhtml = "<ul class='nav navbar-nav'>"
    var menulist = new Array();

    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/Menu/MenuBuilder';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'Menu/MenuBuilder';
    }



    $.ajax({
        type: "POST",
        url: url,
        async: false,
        data: { 'userid': '0' },
        success: function (str) {
            //alert(str);
            try {
                menuarraybuild(str);

                menuhtmlbuild();
            }
            catch (Err) {
                // alert(URLString1(''));

                var url56 = '';
                if (LOCALORSERVERNEW == 'LOCAL') {
                    url56 = '/Home/Login';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
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
    ////console.log(str);
    for (var b in str) {



        var a = str[b];

        //console.log("***************" + mainmenu);
        var mainmenu = a.split('^')[0];

        var submenu = a.split('^')[1];
        var menuurl = a.split('^')[2];

        // //console.log(submenu);
        if (mainmenu == 'Reports') {
            if (menuflag == false) {
                indx = 0;
                mm[mainmenu] = new Array();
                if (str1.indexOf('Reports^Budget Report^Reports/BudgetReport') > -1) {
                    mm[mainmenu][indx] = 'Budget Report' + "^" + 'Reports/BudgetReport';
                    indx++;
                }


                if (str1.indexOf('Reports^Purchase Commitment^Reports/PurchaseCommitmentReport') > -1) {
                    mm[mainmenu][indx] = 'Purchase Commitment' + "^" + 'Reports/PurchaseCommitmentReport';
                    indx++;
                }
                if (str1.indexOf('Reports^Committed Cost Report^Reports/CommittedCostNewReport') > -1) {
                    mm[mainmenu][indx] = 'Committed Cost Report' + "^" + 'Reports/CommittedCostNewReport';
                    indx++;
                }



                if (str1.indexOf('Reports^Status Report^Reports/StatusReport') > -1) {
                    mm[mainmenu][indx] = 'Status Report' + "^" + 'Reports/StatusReport';
                    indx++;
                }
                if (str1.indexOf('Reports^Receiving Report Section^Reports/ReceivingReportSection') > -1) {
                    mm[mainmenu][indx] = 'Receiving Report' + "^" + 'Reports/ReceivingReportSection';
                    indx++;
                }



                if (str1.indexOf('Reports^Project Report^Reports/ProjectReport') > -1) {
                    mm[mainmenu][indx] = 'Project Report' + "^" + 'Reports/ProjectReport';
                    indx++;
                }
                if (str1.indexOf('Reports^Printed Cheque List^Reports/PrintedChequeList') > -1) {
                    mm[mainmenu][indx] = 'Payment Report' + "^" + 'Reports/PrintedChequeList';
                }


                if (str1.indexOf('Reports^PO Report^Reports/POReport') > -1) {
                    mm[mainmenu][indx] = 'PO Report' + "^" + 'Reports/POReport';
                    indx++;
                }








                menuflag = true;
            }
        }
        else if (mainmenu == 'Projects') {
            if (projectmenuflag == false) {
                indx = 0;
                mm[mainmenu] = new Array();
                if (str1.indexOf('Projects^Create Project^Project/CreateProject') > -1) {
                    mm[mainmenu][indx] = 'Create Project' + "^" + urlchanger('Project/CreateProject');
                    indx++;
                }

                if (str1.indexOf('Projects^Project List^Project/Projectlist') > -1) {
                    mm[mainmenu][indx] = 'Project List' + "^" + urlchanger('Project/Projectlist');
                }

                projectmenuflag = true;
            }

        }

        else if (mainmenu == 'Vendors') {
            if (vendormenuflag == false) {
                indx = 0;
                mm[mainmenu] = new Array();

                if (str1.indexOf('Vendors^Add Vendors^adduser/vendor') > -1) {
                    mm[mainmenu][indx] = 'Add Vendors' + "^" + urlchanger('adduser/vendor');
                    indx++;
                }
                if (str1.indexOf('Vendors^Contacts^vendor/allcontacts') > -1) {
                    mm[mainmenu][indx] = 'Contacts' + "^" + urlchanger('vendor/allcontacts');
                    mm[mainmenu][indx+1] = 'Vendors' + "^" + urlchanger('vendor/allvendors');

                }

                vendormenuflag = true;
            }

        }

        else if (mainmenu == 'Purchase Orders') {
            if (purchaseorderflag == false) {
                indx = 0;
                mm[mainmenu] = new Array();

                if (str1.indexOf('Purchase Orders^Create Purchase Order^PurchaseOrder/CreatePurchaseOrder') > -1) {
                    mm[mainmenu][indx] = 'Create Purchase Order' + "^" + urlchanger('PurchaseOrder/CreatePurchaseOrder');
                    indx++;
                }
                if (str1.indexOf('Purchase Orders^PO Search^PurchaseOrder/POList') > -1) {
                    mm[mainmenu][indx] = 'Search Purchase order' + "^" + urlchanger('PurchaseOrder/POList');

                }

                purchaseorderflag = true;
            }

        }
        else if (mainmenu == 'Invoice') {

            if (invoicemenuflag == false) {
                indx = 0;
                mm[mainmenu] = new Array();
                if (str1.indexOf('Invoice^Create Invoice^Invoice/InvoiceEntry') > -1) {
                    mm[mainmenu][indx] = 'ENTER INVOICE' + "^" + urlchanger('Invoice/InvoiceEntry');
                    indx++;

                    mm[mainmenu][indx] = 'ENTER CREDIT NOTE' + "^" + urlchanger('Invoice/CreditNoteEntry');
                    indx++;


                }
                if (str1.indexOf('Invoice^Search^Invoice/invoicesearch') > -1) {
                    mm[mainmenu][indx] = 'SEARCH INVOICE/CREDIT NOTE' + "^" + urlchanger('Invoice/invoicesearch');
                    indx++;
                }

                if (str1.indexOf('Invoice^Payment^Invoice/ManualPayment') > -1) {
                    mm[mainmenu][indx] = 'ENTER PAYMENT' + "^" + urlchanger('Invoice/ManualPayment');
                    indx++;
                }
                if (str1.indexOf('Invoice^Search Payment^Invoice/ManualPaymentSearch') > -1) {

                    mm[mainmenu][indx] = 'SEARCH PAYMENT' + "^" + urlchanger('Invoice/ManualPaymentSearch');

                }



                invoicemenuflag = true;
            }

        }
        else if (mainmenu == 'Budgets') {
            if (budgetmenuflag == false) {
                indx = 0;
                mm[mainmenu] = new Array();
                if (str1.indexOf('Budgets^Outline^Budget/BudgetSection') > -1) {
                    mm[mainmenu][indx] = 'SUMMARY' + "^" + urlchanger('Budget/BudgetSection');
                    indx++;
                }

                if (str1.indexOf('Budgets^Line Items^Budget/Lineitems') > -1) {
                    mm[mainmenu][indx] = 'DETAIL' + "^" + urlchanger('Budget/Lineitems');
                    indx++;
                }

                if (str1.indexOf('Budgets^View^Budget/ReadBudget') > -1) {
                    mm[mainmenu][indx] = 'View' + "^" + urlchanger('Budget/ReadBudget');
                }



                budgetmenuflag = true;
            }

        }

        else if (mainmenu == 'System Admin') {
            if (systemadminflag == false) {
                indx = 0;
                mm[mainmenu] = new Array();
                if (str1.indexOf('System Admin^Add Client/Consultant^adduser/ClientandInteriorDesigner') > -1) {
                    mm[mainmenu][indx] = 'Add Client/Consultant' + "^" + urlchanger('adduser/ClientandInteriorDesigner');
                    indx++;
                }

                if (str1.indexOf('System Admin^Add Staff and Administrator^user/adduser') > -1) {
                    mm[mainmenu][indx] = 'Add Staff and Administrator' + "^" + urlchanger('user/adduser');
                    indx++;
                }
                if (str1.indexOf('System Admin^All Users^user/UserList') > -1) {
                    mm[mainmenu][indx] = 'All Users' + "^" + urlchanger('user/UserList');
                    indx++;
                }
                if (str1.indexOf('System Admin^Assign System Level Permission^Home/SystemLevelPermission') > -1) {
                    mm[mainmenu][indx] = 'Assign System Level Permission' + "^" + urlchanger('Home/SystemLevelPermission');
                    indx++;
                }
                if (str1.indexOf('System Admin^System Admin Activity^SystemAdmin/SystemAdminActivity') > -1) {
                    mm[mainmenu][indx] = 'System Admin Activity' + "^" + urlchanger('SystemAdmin/SystemAdminActivity');
                    indx++;
                }
                if (str1.indexOf('System Admin^Bridge Supporter^Home/BridgeSupporter') > -1) {
                    mm[mainmenu][indx] = 'Bridge Supporter' + "^" + urlchanger('Home/BridgeSupporter');
                    //indx++;
                }
                systemadminflag = true;
            }

        }

        else {
            if (currmainmenu != mainmenu) {
                ////console.log(mainmenu + " Built");
                indx = 0;
                mm[mainmenu] = new Array();
                mm[mainmenu][indx] = submenu + "^" + menuurl;
                currmainmenu = mainmenu;


            }
            else {
                ////console.log(currmainmenu + "===" + mainmenu);
                ////console.log(mainmenu + " Adding");
                indx++;
                mm[mainmenu][indx] = submenu + "^" + menuurl;

            }
        }
    }
}

function menuhtmlbuild() {

    var flag = 1;
    var mhtml = "<ul class='nav navbar-nav'>";
    var mhtml1 = "";
    var mhtml2 = "";

    var hurl = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        hurl = '';//'/';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        hurl = '';
    }

    hurl += urlchanger("/Home/Dashboard");
    mhtml += "<li>"
            + "	<a href='" + hurl + "'>" + "HOME" + "</a>"
            + "</li>";

    var expdturl = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        expdturl = '';//'/';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        expdturl = '';
    }

    expdturl += urlchanger("/Expedite/Home");
    var mhtmlexpedite = "<li>"
            + "	<a href='" + expdturl + "'>" + "EXPEDITE" + "</a>"
            + "</li>";


    for (var m in mm) {






        var mobj = mm[m];

        if (mm[m].length == -1) {

            var ele = mm[m][0];

            var tmp1 = ele.split('^')[0].toUpperCase();
            if (tmp1 == "PROJECTS" || tmp1 == "VENDORS" || tmp1 == "BUDGET")// || tmp1 == "PROJECTS" || tmp1 == "PROJECTS")
            {
                mhtml1 += "<li>"
                    + "	<a href='" + ele.split('^')[1] + "'>" + ele.split('^')[0].toUpperCase() + "</a>"  + "</li>";
                flag = 1;
            }
            else if (tmp1 == "REPORTING") {
            }
            else {
                mhtml2 += "<li>"
                    + "	<a href='" + ele.split('^')[1] + "'>" + ele.split('^')[0].toUpperCase() + "</a>"
                    + "</li>";
                flag = 2;
            }

        }
        else {
            var tmp1 = m.toUpperCase();
            if (tmp1 == "PROJECTS" || tmp1 == "VENDORS" || tmp1 == "BUDGETS" || tmp1 == "PURCHASE ORDERS") {
                flag = 1;
            }
            else if (tmp1 == "REPORTING") {
                flag = 0;
            }
            else {
                flag = 2;
            }

            var url = '';
            if (LOCALORSERVERNEW == 'LOCAL') {
                url = '/';
            }
            else if (LOCALORSERVERNEW == 'SERVER') {
                url = '';
            }

            //alert(m);

            var mainmenuurl = '';

            if (m.trim().toLowerCase() == 'purchase orders') {
                if (LOCALORSERVERNEW == 'LOCAL') {
                    mainmenuurl = '/';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    mainmenuurl = '/';
                }
                mainmenuurl += 'PurchaseOrder/POList';
            }
            else if (m.trim().toLowerCase() == 'invoice') {
                if (LOCALORSERVERNEW == 'LOCAL') {
                    mainmenuurl = '/';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    mainmenuurl = '/';
                }
                mainmenuurl += 'Invoice/invoicesearch';
            }
            else if (m.trim().toLowerCase() == 'vendors') {
                if (LOCALORSERVERNEW == 'LOCAL') {
                    mainmenuurl = '/';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    mainmenuurl = '/';
                }
                //mainmenuurl += 'PurchaseOrder/POList';
                mainmenuurl += 'vendor/allcontacts';
            }
            else if (m.trim().toLowerCase() == 'projects') {
                if (LOCALORSERVERNEW == 'LOCAL') {
                    mainmenuurl = '/';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    mainmenuurl = '/';
                }
                //mainmenuurl += 'PurchaseOrder/POList';
                mainmenuurl += 'Project/Projectlist';
            }
            else if (m.trim().toLowerCase() == 'budgets') {
                if (LOCALORSERVERNEW == 'LOCAL') {
                    mainmenuurl = '/';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    mainmenuurl = '/';
                }
                mainmenuurl += 'Budget/Lineitems';
            }
            else {

                mainmenuurl = 'javascript:void(0);';
            }


            if (flag == 1) {
                mhtml1 += "<li  class='dropdown'>"
                       + '<a href="' + mainmenuurl + '" onclick="master_gotourl(this);" data-hrefurl="' + mainmenuurl + '" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">' + m.toUpperCase() + ''
                       + '<span class="caret"></span></a>'
                     //+ "<span class='submenu-button'></span><a href='" "'>"  "</a>"
                     + '<ul class="dropdown-menu">';
            }
            else if (flag == 2) {

                if (m.toUpperCase() != "REPORTS") {
                    mhtml2 += "<li  class='dropdown'>"
                           + '<a href="' + mainmenuurl + '" onclick="master_gotourl(this);" data-hrefurl="' + mainmenuurl + '" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">' + m.toUpperCase() + ''
                           + '<span class="caret"></span></a>'
                         //+ "<span class='submenu-button'></span><a href='" "'>"  "</a>"
                         + '<ul class="dropdown-menu">';
                }
                else {
                    mhtml2 += "<li  class='dropdown' id='reportingmenu'>"
                          + '<a href="' + mainmenuurl + '" onclick="master_gotourl(this);" data-hrefurl="' + mainmenuurl + '" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">' + m.toUpperCase() + ''
                          + '<span class="caret"></span></a>'
                        //+ "<span class='submenu-button'></span><a href='" "'>"  "</a>"
                        + '<ul class="dropdown-menu">';

                }
            }

            for (var i in mm[m]) {

                var ele = mm[m][i];

                if (flag == 1) {
                    mhtml1 += "<li>"
                            + "<a href='" + url + ele.split('^')[1] + "'>" + ele.split('^')[0].toUpperCase() + "</a>"
                            + "</li>";
                }
                else if (flag == 2) {
                    mhtml2 += "<li>"
                          + "<a href='" + url + ele.split('^')[1] + "'>" + ele.split('^')[0].toUpperCase() + "</a>"
                          + "</li>";


                }


            }

            if (flag == 1) {
                mhtml1 += "</ul></li>";
            }
            else if (flag == 2) {
                mhtml2 += "</ul></li>";

            }

        }

        // //console.log(mhtm1l);

    }
    //mhtml += mhtm1l + mhtmlexpedite;// + mhtml2;

    var isallowed = 0;

    var url = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = '/Home/getExpediteMenuAllowed';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = '/Home/getExpediteMenuAllowed';
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        data: {},
        success: function (data) {
            isallowed = data;
        },
        error: function () {
            notify('Something went wrong.');
        }
    });


    if (isallowed != '0') {
        mhtml += mhtml1 + mhtmlexpedite + mhtml2 + "</ul>";
    }
    else {
        mhtml += mhtml1 + mhtml2 + "</ul>";

    }



    $('#navbar').html(mhtml);

    var reporturl = '';

    if (LOCALORSERVERNEW == 'LOCAL') {
        reporturl = '/';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        reporturl = '';
    }
    reporturl += urlchanger("Reports/Report");

    var reporthtml = '<a href="' + reporturl + '">REPORTS</a>'
    GlobalReportsForMenu = [];
    $('#reportingmenu').find('ul').find('li').each(function () {
        GlobalReportsForMenu.push($(this).find('a').text());
    });

    
    $('#reportingmenu').html(reporthtml);
    
    

}

function BreadCrumbBuild() {
    //var html = "<p id='breadcrumb'>" //style='border: none;'
    //            + "<a href='#'>Home</a>"
    //            + "<a href='#'>Grandpa</a>"
    //            + "<a href='#'>Father</a>"
    //            + "<a href='#'>Son</a>"
    //            + "Product"
    //            + "</p>";
    var html = "";


    var hurl = '';
    var murl = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        hurl = '/';
        murl = '/';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        hurl = '';
        murl = '';
    }

    hurl += "/Home/Dashboard";

    //alert(URLString().toLowerCase());
    if (URLString().toLowerCase() == '/home/dashboard') {
        ////console.log('kkk');
        html = "<p id='breadcrumb'>"
                + "<a href='" + "/Home/Dashboard" + "'>Home</a>"
                 + "<a href='" + "#" + "'>PURCHASE ORDERS</a>"
                + "Dashboard"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorder/createpurchaseorder') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>PURCHASE ORDERS</a>"
                + "CREATE PURCHASE ORDER"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorder/polist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>PURCHASE ORDERS</a>"
                + "SEARCH PURCHASE ORDER"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/systemadmin/systemadminactivity') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                //+ "<a href='" + murl + "'>PURCHASE ORDERS</a>"
                + "System Admin"
                + "</p>";
    }


    else if (URLString().toLowerCase() == '/purchaseorderview/polist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrderView/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrderView/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Purchase Order Search"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorder/deletedpolist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Deleted Purchase Order"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorderview/deletedpolist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrderView/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrderView/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Deleted Purchase Order"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorder/viewpohistory') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Purchase Order History"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorderview/viewpohistory') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrderView/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrderView/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Purchase Order History"
                + "</p>";
    }
   

    else if (URLString().toLowerCase() == '/systemadmin/faqadmin') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/systemadmin/systemadminactivity';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'systemadmin/systemadminactivity';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>System Admin</a>"
                + "FAQ Admin"
                + "</p>";
    }


    else if (URLString().toLowerCase() == '/reports/bridgesupporter_report') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Reports/Report';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Reports/Report';
        }
       
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Report</a>"
                + "Bridge Supporter Report"
                + "</p>";
}

    else if (URLString().toLowerCase() == '/purchaseorder/poedit') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Edit Purchase Order"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorderview/poviewnew') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrderView/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrderView/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "View Purchase Order"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorder/additemstopo') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Add Items To Purchase Order"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorder/editpolineitems') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Edit Purchase Order Line Items"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/budget/budgetsection') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Budget/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Budget/';
        }
        murl += 'Lineitems';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>BUDGETS</a>"
                + "SUMMARY"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/budget/lineitems') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Budget/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Budget/';
        }
        murl += 'Lineitems';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>BUDGETS</a>"
                + "DETAIL"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/budgetview/lineitems') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/BudgetView/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'BudgetView/';
        }
        murl += 'Lineitems';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Budgets</a>"
                + "Create/Edit Budget"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/adduser/vendor') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allcontacts';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>VENDORS</a>"
                + "ADD VENDORS"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/purchaseorder/editpolineitemsforcommitted') {

        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/PurchaseOrder/POList';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/POList';
        }
       // murl += 'allcontacts';


        var Pinum = $('#Hdn_ItemNum').val();;
        var ponum = $('#ponumbershow').text();
        var POor = $('#Hdn_PurchaseOrderID').val();
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
            + "<a href='" + murl + "'>Purchase Order</a>"
            + "<a href='/PurchaseOrder/POEdit/" + POor + "'>PO " + ponum + " </a> "
                + " ITEM "+ Pinum 
                + "</p>";
    }



    else if (URLString().toLowerCase() == '/adduser/vendorcontactsfornewparentvendor') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allcontacts';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Vendors</a>"
                + "Add Vendor Contact under Parent Vendor"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendor/allcontacts') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allcontacts';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>VENDORS</a>"
                + "CONTACTS"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendor/allvendors') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allvendors';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>VENDORS</a>"
                + "VENDORS"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendorview/allcontacts') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendorview/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendorview/';
        }
        murl += 'allcontacts';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Vendors</a>"
                + "Vendor Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendor/editvendor') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allcontacts';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                 + "<a href='" + murl + "'>Vendors</a>"
                + "Edit Vendor Information"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendorview/viewvendor') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendorview/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendorview/';
        }
        murl += 'allcontacts';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                 + "<a href='" + murl + "'>Vendors</a>"
                + "Vendor Information"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/adduser/vendorcontacts') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allcontacts';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Vendors</a>"
                + "Add Vendor Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendor/editcontact') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allcontacts';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Vendors</a>"
                + "Edit Vendor Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendorview/viewcontact') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/vendorview/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendorview/';
        }
        murl += 'allcontacts';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Vendors</a>"
                + "Vendor Contact"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/createproject') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>PROJECTS</a>"
                + "CREATE PROJECT"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/projectlist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>PROJECTS</a>"
                + "PROJECT LIST"
                + "</p>";
    }
        //else if (URLString().toLowerCase() == '/projectview/projectlist') {
        //    if (LOCALORSERVERNEW == 'LOCAL') {
        //        murl = '/ProjectView/';
        //    }
        //    else if (LOCALORSERVERNEW == 'SERVER') {
        //        murl = 'ProjectView/';
        //    }
        //    murl += 'Projectlist';
        //    html = "<p id='breadcrumb'>"
        //            + "<a href='" + hurl + "'>Home</a>"
        //            + "<a href='" + murl + "'>Projects</a>"
        //            + "Project List"
        //            + "</p>";
        //}
    else if (URLString().toLowerCase() == '/project/editproject') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "Edit Project"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/projectview/editproject') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/ProjectView/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'ProjectView/';
        }
        murl += 'Projectlist';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "View Project"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/projectteam') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "Project Team"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/projectcontacts') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "Add Project Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/editprojectcontact') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "Edit Project Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/adduser/clientandinteriordesigner') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/user/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'user/';
        }
        murl += 'UserList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>SYSTEM ADMIN</a>"
                + "ADD CLIENT/CONSULTANT"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/report') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
               // + "<a href='" + murl + "'>SYSTEM ADMIN</a>"
                + "Report"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/about/accessiblecustomerserviceplan') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/user/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'user/';
        }
        murl += 'UserList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>ABOUT</a>"
                + "ACCESSIBLE CUSTOMER SERVICE PLAN"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/about/privacypolicy') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/user/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'user/';
        }
        murl += 'UserList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>ABOUT</a>"
                + "PRIVACY POLICY"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/user/adduser') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/user/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'user/';
        }
        murl += 'UserList';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>SYSTEM ADMIN</a>"
                + "ADD STAFF AND ADMINISTRATOR"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/expedite/home') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                //+ "<a href='javascript:void(0);'>EXPEDITE</a>"
                + "EXPEDITE"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/expedite/home_new') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>EXPEDITE</a>"
                + "TRACK STATUS"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/project/viewproject') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "View Project"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/systemadmin/systemadminactivity') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>SYSTEM ADMIN</a>"
                + "SYSTEM ADMIN ACTIVITY"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/home/systemlevelpermission') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>SYSTEM ADMIN</a>"
                + "ASSIGN SYSTEM LEVEL PERMISSION"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/user/userlist') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>SYSTEM ADMIN</a>"
                + "ALL USERS"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/tasklist/taskmanager') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Task</a>"
                + "Task Manager"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/invoice/invoiceentry') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "ENTER INVOICE"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/home/bridgesupporter') {
       
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                //+ "<a href='" + murl + "'>INVOICE</a>"
                + "BRIDGE SUPPORTER CONTROL"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/systemadmin/faqadmin') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/systemadmin/systemadminactivity';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'systemadmin/systemadminactivity';
        }
        
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>System Admin</a>"
                + "FAQ Admin"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/invoice/invoiceentry_tax' || URLString().toLowerCase() == '/invoiceentry_tax') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "ENTER INVOICE"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/invoice/creditentry_tax' || URLString().toLowerCase() == '/creditentry_tax') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "ENTER CREDIT"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/invoice/creditedit_tax' || URLString().toLowerCase() == '/creditedit_tax') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "EDIT CREDIT"
                + "</p>";
    }
    

    
    else if (URLString().toLowerCase() == '/invoice/creditnoteedit') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "EDIT CREDIT NOTE"
                + "</p>";
    }


    else if (URLString().toLowerCase() == '/invoice/creditnoteentry') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "ENTER CREDIT NOTE"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/invoice/invoicesearch') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "SEARCH INVOICE"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/invoice/creditnoteedit') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "EDIT CREDIT NOTE"
                + "</p>";
    }


    else if (URLString().toLocaleLowerCase() == '/invoice/manualpayment') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + " ENTER PAYMENT"
                + "</p>";
    }

    else if (URLString().toLocaleLowerCase() == '/invoice/manualpaymentsearch') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='" + murl + "'>INVOICE</a>"
                + "Search PAYMENT"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/invoice/invoiceedit_tax') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Invoice</a>"
                + "Edit Invoice"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/invoice/invoiceedit') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Invoice</a>"
                + "Edit Invoice"
                + "</p>";
    }
    


    else if (URLString().toLowerCase() == '/home/mysettings') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>My Settings</a>"
                + "Change Settings"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/statusbyvendor') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Status By Vendor"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/receivingreportsection') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Receiving Report Section"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/purchasecommitmentreport') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Purchase Commitment Report"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/projecttodatereport') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Project To Date Report"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/printedchequelist') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Payment Report"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/expeditnote') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Expedite Note"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/expeditereportbystatus') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Expedite Report By Status"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/expeditereportbyitem') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Expedite Report By Item"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/currentpaidreport') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Current Paid Report"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/committedcostreport') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Committed Cost Report"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/committedcostreportdetail') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Committed Cost Report Detail"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/budgetreportsummary') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Budget Report Summary"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/budgetreportbyitem') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "Budget Report By Item"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/atriskbyitem') {

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Reports</a>"
                + "At Risk By Item"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendor/searchpolist') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Search PURCHASE ORDER"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendorview/searchpolist') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "Search Purchase Order"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/vendor/mergevendor') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Vendor</a>"
                + "Search Purchase Order"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/budgetreport') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>REPORTS</a>"
                + "BUDGET REPORT"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/reports/statusreport') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>REPORTS</a>"
                + "STATUS REPORT"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/purchasereport') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>REPORTS</a>"
                + "PURCHASE REPORT"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/budget/readbudget') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>Budget</a>"
                + "View Budget"
                + "</p>";
    }


    else if (URLString().toLowerCase() == '/reports/committedcostnewreport') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>REPORTS</a>"
                + "COMMITTED COST REPORT"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/reports/projectreport') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>REPORTS</a>"
                + "PROJECT REPORT"
                + "</p>";
    }

    else if (URLString().toLowerCase() == '/reports/poreport') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>HOME</a>"
                + "<a href='javascript:void(0);'>REPORTS</a>"
                + "PO REPORT"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/invoice/editcreditnote') {

        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = '/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
            + "<a href='" + hurl + "'>HOME</a>"
            + "<a href='" + murl + "'>INVOICE</a>"
            + "EDIT CREDIT NOTE"
            + "</p>";
    }


    ////console.log(html);

    ////console.log(html.replace(/<a/g, "<li><a").replace(/<\/a>/g, "</a></li>"));
    html = html.replace(/<a/g, "<li><a").replace(/<\/a>/g, "</a></li>");
    html = html.replace("<p id='breadcrumb'>", "<ol class='breadcrumb'>");
    html = html.replace("</p>", "</ol>");

    var h1 = html.split("</ol>")[0];
    var h2 = h1.split("</li>")[h1.split("</li>").length - 1];
    var h3 = h1.split(h2)[0];

    var h3 = "<li>" + h2 + "</li>";
    html = h1.split(h2)[0] + h3 + "</ol>";

    if (URLString().toLowerCase() == '/home/dashboard') {

        html = "<ol class='breadcrumb text-uppercase'><li><a href='/Home/Dashboard'>HOME</a></li><li>TASK LIST</li></ol>";
    }
   

    $('#breadcrumb').after(html);
    $('#breadcrumb').next('ol').addClass('text-uppercase');

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
}

function URLString() {
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
   // console.log("****************" + strQueryString);
    return strQueryString;
}

