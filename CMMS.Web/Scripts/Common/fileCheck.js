 $('#UploadFiles').change(
        function () {
            var fileExtension = ['tiff', 'tif'];
            if ($.inArray($(this).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
        alert("Only '.tiff or .tif' format is allowed. Maximum size limit is 5 MB");
    this.value = ''; // Clean field
                return false;
            }
        });
    $('#UploadFiles').bind('change', function () {
        var files = $('#UploadFiles')[0].files;
        var totalSize = 0;
        for (var i = 0; i < files.length; i++) {
        // calculate total size of all files        
        totalSize += files[i].size;
    }
        var sizeInMB = totalSize / 1024 / 1024;

        if (sizeInMB <= 5) {

    }
    else {

        alert('This file size is: ' + sizeInMB.toFixed(2) + 'MB, File exceeds maximum size limit i.e. 5 MB');
    this.value = ''; // Clean field
            return false;
        }
});