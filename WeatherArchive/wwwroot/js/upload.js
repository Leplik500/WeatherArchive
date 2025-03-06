$('#filesInput').on('change', function () {
    let files = this.files;
    for (let i = 0; i < files.length; i++) {
        let file = files[i];
        if (!file.name.endsWith('.xlsx') || !file.name.endsWith('.xls')) {
            this.value = "";
            $('#errorMessage').css('display', 'block');
            return
        }
    }
    $.ajax({
        url: "/UploadHandle",
        method: "POST",
        data: $("#uploadForm").serialize(),
        success: function (response) {
            alert("Files uploaded successfully");
            console.log(response)
        },
        error: function (response) {
            alert("Files upload failed: " + response.responseJSON.description)
            console.log(response)
        }
    })
})