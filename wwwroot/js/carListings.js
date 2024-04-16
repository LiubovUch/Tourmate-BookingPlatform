
$(document).ready(function () {
    function loadCarListings(data) {
        var html = '';
        var rowOpen = false;
        var columnsPerRow = 3;
        if (data.length > 0) {
            html += '<h3>All Car Rentals Available </h3>';
            html += '<hr/>';
            for (var i = 0; i < data.length; i++) {
                if (i % columnsPerRow === 0) {
                    if (rowOpen) {
                        html += '</div>'; 
                    }
                    html += '<div class="row">'; 
                    rowOpen = true;
                }
                html += '<div class="col-sm-4 mb-4"  >';
                html += '<div class="card" style="width: 100%;">';
                html += '<div class="card-body">';
                html += '<h5 class="card-title">Car Model: ' + data[i].carModel + '</h5>';
                html += '<p class="card-text">Rental Company: ' + data[i].rentalCompany + '</p>';
                html += '</div>';
                html += '<ul class="list-group list-group-flush">';
                html += '<li class="list-group-item"><i class="fa-solid fa-car"></i> Car Type: ' + data[i].carType + '</li>';
                html += '<li class="list-group-item"><i class="fa-solid fa-hourglass-start"></i> Availability Start Date: ' + formatDate(data[i].availabilityStartDate) + '</li>';
                html += '<li class="list-group-item"><i class="fa-solid fa-hourglass-end"></i> Availability End Date: ' + formatDate(data[i].availabilityEndDate) + '</li>';
                html += '<li class="list-group-item"><i class="fa-solid fa-wallet"></i> Price: $' + data[i].price + '</li>';
                html += '</ul>';
                html += `<div class="card-body text-center">
                            <button class="btn btn-success rounded-pill details-button" data-id="${data[i].carRentalId}" style="width:30%;">Details</button>
                        </div>`
                html += '</div>';
                html += '</div>';
                if (i % columnsPerRow === columnsPerRow - 1 || i === data.length - 1) {
                    html += '</div>';
                    rowOpen = false;
                }
            }
        } else {
            html = '<p>No cars found.</p>';
        }

        $('#carRentalSearchResults').html(html);
    }

    function formatDate(dateString) {
        var date = new Date(dateString);
        var year = date.getFullYear();
        var month = ('0' + (date.getMonth() + 1)).slice(-2);
        var day = ('0' + date.getDate()).slice(-2);
        return year + '-' + month + '-' + day;
    }

    function loadBestMatches() {
        $.ajax({
            url: '/BookingManagement/CarRental/GetBestMatches',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                $('#bestMatchesContainer').empty();

                if (data && data.length > 0) {
                    var cardHtml = `<h3>Best Matches Based On Your Preferences!</h3>
                                <hr />
                                <div class="row">`; 
                    for (var i = 0; i < data.length; i++) {
                        var carRental = data[i];

                        cardHtml += `
                        <div class="col-sm-4 mb-4" style="margin-top: 10px;">
                            <div class="card" style="width: 100%;">
                                <div class="card-body">
                                    <h5 class="card-title">Car Model: ${carRental.carModel}</h5>
                                    <p class="card-text">Rental Company: ${carRental.rentalCompany}</p>
                                </div>
                                <ul class="list-group list-group-flush">
                                    <li class="list-group-item"><i class="fa-solid fa-car"></i> Car Type: ${carRental.carType}</li>
                                    <li class="list-group-item"><i class="fa-solid fa-hourglass-start"></i> Availability Start Date: ${formatDate(carRental.availabilityStartDate)}</li>
                                    <li class="list-group-item"><i class="fa-solid fa-hourglass-end"></i> Availability End Date: ${formatDate(carRental.availabilityEndDate)}</li>
                                    <li class="list-group-item"><i class="fa-solid fa-wallet"></i> Price: $${carRental.price}</li>
                                </ul>
                                <div class="card-body text-center">
                                    <button class="btn btn-success rounded-pill details-button" data-id="${carRental.carRentalId}" style="width:30%;">Details</button>
                                </div>
                            </div>
                        </div>`;
                    }
                    cardHtml += `</div>`;
                    $('#bestMatchesContainer').append(cardHtml);
                } else {
                    $('#bestMatchesContainer').html('<p>No Car Rentals available.</p>');
                }
            },
            error: function () {
                console.error('Error loading best matches.');
            }
        });
    }
    $(document).on('click', '.details-button', function () {
        var carRentalId = $(this).data('id');
        window.location.href = '/BookingManagement/CarRental/' + carRentalId;
    });
    function loadInitialListings() {
        $.ajax({
            url: '/BookingManagement/CarRental/GetCarRentals',
            method: 'GET',
            success: function (data) {
                loadCarListings(data);
            },
            error: function (error) {
                console.error('Error fetching car rentals:', error);
            }
        });
    }
    $('#carRentalSearchForm').submit(function (event) {
        event.preventDefault();
        var formData = $(this).serialize();

        $.ajax({
            url: $(this).attr('action'),
            type: $(this).attr('method'),
            data: formData,
            dataType: 'json',
            beforeSend: function () {
                $('#carRentalSearchResults').html('<div class="spinner-border text-primary" role="status"><span class="sr-only">Loading...</span></div>');
            },
            success: function (data) {
                loadCarListings(data);
                if ($('#carModel').val() === '' && $('#rentalCompany').val() === '' && $('#sortOrder').val() === '' && $('#carType').val() === '') {
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
