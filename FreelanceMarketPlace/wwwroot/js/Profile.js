document.addEventListener("DOMContentLoaded", function () {
    console.log("Hello")

    const submitBtn = document.querySelector('.save-photo')
    console.log(submitBtn)
    submitBtn.addEventListener("click", function () {
        var formData = new FormData();

        // Get the file input element
        var fileInput = document.getElementById('ProfilePicture');
        console.log(fileInput.files)
        // Check if a file is selected
        if (fileInput.files.length > 0) {
            // Get the selected file
            var file = fileInput.files[0];

            // Append the file to the FormData object
            formData.append("ProfilePicture", file);
        } else {
            alert("No file selected.");
            return;
        }
        console.log([...formData.entries()]);
        // Send the AJAX request
        $.ajax({
            url: '/User/EditProfilePicture',  
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                alert(response.message);
                $('#exampleModal').modal('hide');
                location.reload()
            },
            error: function (xhr, status, error) {
                console.log("Error uploading file:", error);
                console.log("Status:", status);
                console.log("Response:", xhr.responseText);
            }
        });
    })
   
})