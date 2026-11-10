$(document).ready(function (e) {
    $('.security-title').click(function () {

       // $('.security-detail').css('display', 'none');
        // $(this).next('.security-detail').toggle();

        if ($(this).next('.security-detail').css('display') != 'block') {
            $('.security-detail').css('display', 'none');
            $(this).next('.security-detail').css('display', 'block');
        }
        else {
            $(this).next('.security-detail').css('display', 'none');
        }
    });




    //$("input[type=checkbox]").switchButton({
    //    on_label: 'yes',
    //    off_label: 'no'
    //});


});