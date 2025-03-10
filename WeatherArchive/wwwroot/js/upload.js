$('#filesInput').on('change', function () {
    let files = this.files;
    for (let i = 0; i < files.length; i++) {
        let file = files[i];
        debugger
        if (!file.name.endsWith('.xlsx') && !file.name.endsWith('.xls')) {
            this.value = "";
            $('#errorMessage').css('display', 'block');
            return
        } else {
            $('#errorMessage').css('display', 'none');
        }
    }
})

$("#uploadButton").on('click', function (event) {
    event.preventDefault();
    let files = $('#filesInput')[0].files;
    if (files.length === 0) {
        $('#errorMessage').css('display', 'block');
        return
    }
    let formData = new FormData($("#uploadForm")[0]);
    $.ajax({
        url: "/MultipleUploadHandle",
        method: "POST",
        data: formData,
        contentType: false,
        processData: false,
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