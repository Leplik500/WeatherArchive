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
    $('#waitMessage').css('display', 'block');
    let formData = new FormData($("#uploadForm")[0]);
    $.ajax({
        url: "/MultipleUploadHandle",
        method: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            alert("Все файлы успешно загружены и разобраны");
            $('#waitMessage').css('display', 'none');
            console.log(response);
        },
        error: function (response) {
            let json = response.responseJSON;
            let message;
            if (json.status === 'PartialSuccess') {
                message = "Некоторые файлы загружены с ошибками:\n" + json.description.join('\n');
            } else {
                message = "Все файлы не загружены. Ошибки:\n" + json.description.join('\n');
            }
            alert(message);
            $('#waitMessage').css('display', 'none');
            console.log(response);
        }
    })
})