// document.addEventListener('DOMContentLoaded', function() {
//     // Initial view setup or any other initialization
//     showView('about'); // Show about by default or based on some logic
// });

// function showView(viewId) {
//     // Hide all views
//     document.querySelectorAll('.view').forEach(function(view) {
//         view.style.display = 'none';
//     });

//     // Show the selected view
//     document.getElementById(viewId + 'View').style.display = 'block';
// }




document.getElementById('imageUpload').addEventListener('change', function(event) {
    var file = event.target.files[0];

    if (!file) {
        alert('No file selected.');
        return;
    }

    if (file.size > 5242880) { // 5MB in bytes
        alert('The file is too large. Please select a file smaller than 5MB.');
        return;
    }

    var reader = new FileReader();
    reader.onload = function(e) {
        // Display image
        var imagePreviewContainer = document.getElementById('imagePreviewContainer');
        imagePreviewContainer.innerHTML = ''; // Clear the container
        var img = document.createElement('img');
        img.src = e.target.result; // This is already a base64 representation
        img.style.maxWidth = '200px';
        img.style.maxHeight = '200px';
        imagePreviewContainer.appendChild(img);

        // e.target.result contains the Base64 encoded string of the image
        var base64String = e.target.result.split(',')[1]; // Remove the data URL part

        console.log(base64String); // Log the base64 string. You can send this to your server

        // Assuming you have a form field or some way to include this in your AJAX request
        // For demonstration purposes, let's say you have a hidden input field for this purpose
        document.getElementById('your_hidden_input_field_id').value = base64String;
    };

    // Read the file as a data URL to display the image
    reader.readAsDataURL(file);
});

document.getElementById('addSpace').addEventListener('click', function() {
    console.log('Button clicked!'); // This should appear in the browser console when the button is clicked.
    const container = document.getElementById('spacesContainer');
    const newRow = document.createElement('div');
    newRow.classList.add('spaceRow');
    newRow.innerHTML = `
        <input type="text" class="spaceID" name="spaceID[]" placeholder="Space ID">
        <input type="text" class="timeLimit" name="timeLimit[]" placeholder="Time Limit">
    `;
    container.appendChild(newRow);
});

function collectSpacesAndTimeLimits() {
    let spacesDict = {};
    // Assuming each spaceID input is directly followed by its corresponding timeLimit input
    const spaceIDs = document.querySelectorAll('.spaceID');
    const timeLimits = document.querySelectorAll('.timeLimit');

    spaceIDs.forEach((spaceInput, index) => {
        const spaceID = spaceInput.value;
        const timeLimit = timeLimits[index].value; // Assuming the same index
        if (spaceID && timeLimit) {
            spacesDict[spaceID] = parseInt(timeLimit);
        }
    });

    return spacesDict;
}


var acc = document.getElementsByClassName("accordion");
var i;

for (i = 0; i < acc.length; i++) {
acc[i].addEventListener("click", function() {
    this.classList.toggle("active");
    var panel = this.nextElementSibling;
    if (panel.style.maxHeight) {
    panel.style.maxHeight = null;
    } else {
    panel.style.maxHeight = panel.scrollHeight + "px";
    }
});
 }

$(document).ready(function() {
    $('.accordion').click(function() {
        this.classList.toggle("active");
        var formContainer = $(this).next('.form-container');
        if (formContainer.css('display') === "none") {
            formContainer.css('display', 'flex'); // Show the form
        } else {
            formContainer.css('display', 'none'); // Hide the form
        }
    });

    $('#loadRequestsButton').click(function() {
        $.ajax({
            url: 'http://localhost:8080/api/SpaceManager/createSpace',
            type: 'GET',
            dataType: 'json',
            success: function(data) {
                $('#recoveryRequestsList').empty(); 
                data.forEach(function(request) {
                    var listItem = '<li>Name : ' + request.floorPlanName + ', Spaces : ' + request.spaceList;
                    if(request.resolveDate) {
                        listItem += ', Resolve Date: ' + request.resolveDate;
                    }
                  
                    listItem += '</li>';
                    $('#recoveryRequestsList').append(listItem);
                });
            },
            error: function(error) {
                console.error(error);
            }
        });
        $('#accountCreationForm').submit(function(e) {
            e.preventDefault();
            

            var companyFloor = {
                FloorPlanName: $('#floorPlanName').val(),
                FloorPLanImage: base64String,
                FloorSpaces: collectSpacesAndTimeLimits()
                
            };
            $.ajax({
                url: 'http://localhost:8080/api/SpaceManager/postSpace',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(companyFloor),
                success: function(response) {
                    alert('Account created successfully!');
                    console.log(response); // Log response for debugging
                },
                error: function(xhr, status, error) {
                    console.error(xhr.responseText); // Log response text for debugging
                    alert('Error creating account. ' + xhr.responseText);
                }
            });
        });
    });
});