$(document).ready(function () {
    // Function to load hotel listings
    function loadHotelListings(data) {
        var html = '';
        var rowOpen = false;
        var columnsPerRow = 3;
        if (data.length > 0) {
            html += '<h3>All Hotels Available </h3>';
            html += '<hr/>';
            for (var i = 0; i < data.length; i++) {
                if (i % columnsPerRow === 0) {
                    if (rowOpen) {
                        html += '</div>'; // Close the previous row
                    }
                    html += '<div class="row">'; // Open a new row
                    rowOpen = true;
                }
                html += '<div class="col-sm-4 mb-4"  >';
                html += '<div class="card" style="width: 100%;">';
                html += '<img class="card-img-top" src="' + data[i].pictureUrl + '" alt="Hotel Image" style="width: 100%; height: 200px; object-fit: cover; object-position: center center;">';
                html += '<div class="card-body">';
                html += '<h5 class="card-title">Hotel Name: ' + data[i].name + '</h5>';
                html += '<p class="card-text">Location: ' + data[i].location + '</p>';
                html += '</div>';
                html += '<ul class="list-group list-group-flush">';
                html += '<li class="list-group-item" style="height:60px"><i class="fa-solid fa-check"></i> Amenities: ' + data[i].amenities + '</li>';
                html += '<li class="list-group-item"><i class="fa-solid fa-wallet"></i> Price per night: $' + data[i].price + '</li>';
                html += '</ul>';
                html += '<div class="card-body text-center">';
                html += `<button class="btn btn-success rounded-pill details-button" data-id="${data[i].hotelId}" style="width:30%;">Details</button>`;
                html += '</div>';
                html += '</div>'; // Close the card

                html += '</div>';
                if (i % columnsPerRow === columnsPerRow - 1 || i === data.length - 1) {
                    html += '</div>';
                    rowOpen = false;
                }
            }
        } else {
            html = '<p>No hotels found.</p>';
        }

        $('#hotelSearchResults').html(html);
    }

    function loadBestMatches() {
        $.ajax({
            url: '/BookingManagement/Hotel/GetBestMatches',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                // Clear previous results before appending new ones
                $('#bestMatchesContainer').empty();

                if (data && data.length > 0) {
                    var cardHtml = `<h3>Best Matches Based On Your Preferences!</h3>
                                        <hr />
                                        <div class="row">`; // Open a row container
                    for (var i = 0; i < data.length; i++) {
                        var hotel = data[i];

                        cardHtml += `
                                <div class="col-sm-4 mb-4" style="margin-top: 10px;">
                                    <div class="card" style="width: 100%;">
                                        <img class="card-img-top" src="${hotel.pictureUrl}" alt="Hotel Image" style="width: 100%; height: 200px; object-fit: cover; object-position: center center;">
                                        <div class="card-body">
                                            <h5 class="card-title">Hotel Name: ${hotel.name}</h5>
                                            <p class="card-text">Location: ${hotel.location}</p>
                                        </div>
                                        <ul class="list-group list-group-flush">
                                            <li class="list-group-item" style="height:60px"><i class="fa-solid fa-check"></i> Amenities: ${hotel.amenities}</li>
                                            <li class="list-group-item"><i class="fa-solid fa-wallet"></i> Price per night: $${hotel.price}</li>
                                        </ul>
                                        <div class="card-body text-center">
                                            <button class="btn btn-success rounded-pill details-button" data-id="${hotel.hotelId}" style="width:30%;">Details</button>
                                        </div>
                                    </div>
                                </div>`;
                    }
                    cardHtml += `</div>`; // Close the row container
                    $('#bestMatchesContainer').append(cardHtml);
                } else {
                    $('#bestMatchesContainer').html('<p>No hotels available.</p>');
                }
            },
            error: function () {
                console.error('Error loading best matches.');
            }
        });
    }
    $(document).on('click', '.details-button', function () {
        var hotelId = $(this).data('id');
        // Redirect to details page with the selected car rental ID
        window.location.href = '/BookingManagement/Hotel/' + hotelId;
    });
    function loadInitialListings() {
        $.ajax({
            url: '/BookingManagement/Hotel/GetHotels',
            method: 'GET',
            success: function (data) {
                loadHotelListings(data);
            },
            error: function (error) {
                console.error('Error fetching hotels:', error);
            }
        });
    }

    $('#hotelSearchForm').submit(function (event) {
        event.preventDefault();
        var formData = $(this).serialize();

        // Make AJAX request
        $.ajax({
            url: $(this).attr('action'),
            type: $(this).attr('method'),
            data: formData,
            dataType: 'json',
            beforeSend: function () {
                $('#hotelSearchResults').html('<div class="spinner-border text-primary" role="status"><span class="sr-only">Loading...</span></div>');
            },
            success: function (data) {
                loadHotelListings(data);
                if ($('#name').val() === '' && $('#location').val() === '' && $('#sortOrder').val() === '') {
                    loadBestMatches();
                } else {
                    $('#bestMatchesContainer').empty();
                }
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    });

    loadInitialListings();
    loadBestMatches();
});
