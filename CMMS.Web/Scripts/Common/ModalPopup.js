﻿$(function () {
 
    $.ajaxSetup({ cache: false });
    $("a[data-modal]").on("click", function (e) {
    
        $('#myModalContent').load(this.href, function (response,status,xhr) {

            if (status == "error") {
                alert("You are not authorized to perform this operation.");
            } else {

                $('#myModal').modal({

                    keyboard: true
                }, 'show');
            }
        });
        return false;
    });
});


//function bindForm(dialog) {
//    $('form', dialog).submit(function () {
//        $('#progress').show();
//        $.ajax({
//            url: this.action,
//            type: this.method,
//            data: $(this).serialize(),
//            success: function (result) {
//                if (result.success) {
//                    $('#myModal').modal('hide');
//                    $('#progress').hide();
//                    location.reload();
//                } else {
//                    $('#progress').hide();
//                    $('#myModalContent').hide();
//                    bindForm();
//                }
//            }
//        });
//        return false;
//    });
//}
