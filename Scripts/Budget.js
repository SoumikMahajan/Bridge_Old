
$(function () {

    $('#btn_AddBdgtSctn').click(function () {

        if ($('#txt_BudgetDesc').val().trim() == '') {
            errorfunc('txt_BudgetDesc', 'You must enter a description for the budget section before continuing');
            return;
        }
        if ($('#txt_BudgetSecNo').val().trim() == '') {
            //errorfunc('txt_BudgetSecNo', 'You must enter a section number that has not already been used.');
            errorfunc('txt_BudgetSecNo', 'You must enter a section number before adding a budget section.');
            return;
        }


        var PrjtDdl = $('#PrjtDdl').val();
        var txt_BudgetDesc = $('#txt_BudgetDesc').val();
        var txt_BudgetSecNo = $('#txt_BudgetSecNo').val();

    });
});