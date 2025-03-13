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
    $('#waitMessage').css('display', 'block')
    let formData = new FormData($("#uploadForm")[0]);
    $.ajax({
        url: "/MultipleUploadHandle",
        method: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            alert("Файлы успешно загружены и разобраны");
            $('#waitMessage').css('display', 'none')
            console.log(response)
        },
        error: function (response) {
            alert("Загрузка файлов провалилась: " + response.responseJSON.description)
            $('#waitMessage').css('display', 'none')
            console.log(response)
        }
    })
})