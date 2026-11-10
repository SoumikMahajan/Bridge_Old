$(function () {
   

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
    if (LOCALORSERVERNEW == 'LOCAL') {
        url = 'http://www.thebridgebytcg.com/Menu/MenuBuilder';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url = 'Menu/MenuBuilder';
    }



    $.ajax({
        type: "POST",
        url: url,
        data: { 'userid': '0' },
        success: function (str) {
            //alert(str);
            try
            {
                menuarraybuild(str);

                menuhtmlbuild();
            }
            catch(Err)
            {
                // alert(URLString1(''));

                var url56 = '';
                if (LOCALORSERVERNEW == 'LOCAL') {
                    url56 = 'http://www.thebridgebytcg.com/Home/Login';
                }
                else if (LOCALORSERVERNEW == 'SERVER') {
                    url56 = 'Home/Login';
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
   // //console.log(str);
    for (var b in str) {

        

        var a = str[b];

       
        var mainmenu = a.split('^')[0];

        var submenu = a.split('^')[1];
        var menuurl = a.split('^')[2];

       // //console.log(submenu);

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
                  + "<a href='"+ mainmenuurl + "'>" + m + "</a>"
                  + "<ul class='sub-menu'>";

            for (var i in mm[m]) {

                var ele = mm[m][i];
                mhtml += "<li>"
                        + "<a href='" + url + ele.split('^')[1] + "'>" + ele.split('^')[0] + "</a>"
                        + "</li>";

            }



            mhtml += "</ul></li>";
        }

        ////console.log(mhtml);

    }


    mhtml += "</ul>";
    ////console.log(mhtml);
    
    $('#menu').html(mhtml);

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
        hurl = 'http://www.thebridgebytcg.com/';
        murl = 'http://www.thebridgebytcg.com/';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        hurl = '';
        murl = '';
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
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Create Purchase Order"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorder/polist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/PurchaseOrder/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrder/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Purchase Order List"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorderview/polist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/PurchaseOrderView/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'PurchaseOrderView/';
        }
        murl += 'POList';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Purchase Order</a>"
                + "Purchase Order List"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/purchaseorder/deletedpolist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/PurchaseOrder/';
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
            murl = 'http://www.thebridgebytcg.com/PurchaseOrderView/';
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
            murl = 'http://www.thebridgebytcg.com/PurchaseOrder/';
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
            murl = 'http://www.thebridgebytcg.com/PurchaseOrderView/';
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

    else if (URLString().toLowerCase() == '/purchaseorder/poedit') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/PurchaseOrder/';
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
            murl = 'http://www.thebridgebytcg.com/PurchaseOrderView/';
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
            murl = 'http://www.thebridgebytcg.com/PurchaseOrder/';
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
            murl = 'http://www.thebridgebytcg.com/PurchaseOrder/';
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
            murl = 'http://www.thebridgebytcg.com/Budget/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Budget/';
        }
        murl += 'Lineitems';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Budgets</a>"
                + "Budget Section"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/budget/lineitems') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/Budget/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Budget/';
        }
        murl += 'Lineitems';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Budgets</a>"
                + "Create/Edit Budget"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/budgetview/lineitems') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/BudgetView/';
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
            murl = 'http://www.thebridgebytcg.com/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allcontacts';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Vendors</a>"
                + "Add Parent Vendor"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/adduser/vendorcontactsfornewparentvendor') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/vendor/';
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
            murl = 'http://www.thebridgebytcg.com/vendor/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'vendor/';
        }
        murl += 'allcontacts';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Vendors</a>"
                + "Vendor Contacts"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendorview/allcontacts') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/vendorview/';
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
            murl = 'http://www.thebridgebytcg.com/vendor/';
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
            murl = 'http://www.thebridgebytcg.com/vendorview/';
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
            murl = 'http://www.thebridgebytcg.com/vendor/';
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
            murl = 'http://www.thebridgebytcg.com/vendor/';
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
            murl = 'http://www.thebridgebytcg.com/vendorview/';
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
            murl = 'http://www.thebridgebytcg.com/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "Create Project"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/projectlist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/Project/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Project/';
        }
        murl += 'Projectlist';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "Project Lists"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/projectview/projectlist') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/ProjectView/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'ProjectView/';
        }
        murl += 'Projectlist';
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Projects</a>"
                + "Project Lists"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/editproject') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/Project/';
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
            murl = 'http://www.thebridgebytcg.com/ProjectView/';
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
            murl = 'http://www.thebridgebytcg.com/Project/';
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
            murl = 'http://www.thebridgebytcg.com/Project/';
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
            murl = 'http://www.thebridgebytcg.com/Project/';
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
            murl = 'http://www.thebridgebytcg.com/user/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'user/';
        }
        murl += 'UserList';


        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Users</a>"
                + "Add Client or Interior Designer"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/user/adduser') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/user/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'user/';
        }
        murl += 'UserList';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Users</a>"
                + "Add PC Staff and Administrator"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/expedite/home') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Expedite</a>"
                + "Track Status"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/project/viewproject') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/Project/';
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
                + "<a href='javascript:void(0);'>User</a>"
                + "All Users"
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
            murl = 'http://www.thebridgebytcg.com/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Invoice</a>"
                + "Create Invoice"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/invoice/invoicesearch') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/Invoice/';
        }
        else if (LOCALORSERVERNEW == 'SERVER') {
            murl = 'Invoice/';
        }
        murl += 'invoicesearch';

        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='" + murl + "'>Invoice</a>"
                + "Search Invoice"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/invoice/invoiceedit') {
        if (LOCALORSERVERNEW == 'LOCAL') {
            murl = 'http://www.thebridgebytcg.com/Invoice/';
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
                + "Printed Cheque List"
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
                + "<a href='" + hurl + "'>Home</a>"
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
                + "PO Search"
                + "</p>";
    }
    else if (URLString().toLowerCase() == '/vendorview/searchpolist') {
        html = "<p id='breadcrumb'>"
                + "<a href='" + hurl + "'>Home</a>"
                + "<a href='javascript:void(0);'>Purchase Order</a>"
                + "PO Search"
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
        return strQueryString.replace('/aspdemo/poms','');
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
    
    return strQueryString;    
}

