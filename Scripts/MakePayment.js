/// <reference path="angular.min.js" />


var makePaymentApp = angular.module("makePayment", []);

makePaymentApp.controller("makePayment", function ($scope,$http) {

    var paymentmode = [];
    var bank = [];
    $http({
        url: "/Invoice/GetAllPaymentMode",
        method: "GET"
       
    }).then(function successCallback(data) {

        //var selectedIndex = 0;
        for(var i=0;i<data.data.length;i++)
        {
            
            //if (data.data[i]["IsDefault"] === true)
            //    selectedIndex = i;
            paymentmode.push({

                ID : data.data[i]["PaymentMode_ID"],
                Val: data.data[i]["PaymentModeName"],
                Set: data.data[i]["isDefault"]

            });
            
        }

        $scope.options = paymentmode;
        $scope.selectedOption = $scope.options[0];
        
        
    }, function errorCallback(data) {
        
    });

    bank.push({

        ID: -1,
        Val: "Select Bank Account",

    });
    $http({
        url: "/Invoice/GetAllBankAccount",
        method: "GET"

    }).then(function successCallback(data) {

        //var selectedIndex = 0;
        for (var i = 0; i < data.data.length; i++) {

            //if (data.data[i]["IsDefault"] === true)
            //    selectedIndex = i;
            bank.push({

                ID: data.data[i]["BankAccount_ID"],
                Val: data.data[i]["BankAccountName"],

            });

        }

        $scope.option1s = bank;
        $scope.selectedOption1 = $scope.option1s[0];


    }, function errorCallback(data) {

    });

});