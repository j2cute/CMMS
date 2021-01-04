$(document).ready(function () {
    $("#btn2N").click(function () {
        var PersonnelID = $("#Personnel_tbl_PersonnelID").val();
        $.ajax({
            cache: 'false',
            type: "Post",
            //  contentType: "application/json; charset=utf-8",
            data: { 'id': PersonnelID },
            url: '@Url.Action("EditVictim", "CriminalRecordModule")',
            dataType: 'json',  // add this line
            "success": function (data) {
                if (data !== null) {
                    //var Mydata = data;
                    $("#VictimName").val(data.Personnel_tbl_Validation.Rep_Name);
                    $("#VictimFatherName").val(data.Personnel_tbl_Validation.Rep_FatherName);


                }
            }
        });
    });
});

