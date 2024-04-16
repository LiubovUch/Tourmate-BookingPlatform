$(document).ready(function () {
    function loadFlightListings(data) {
        var html = '';
        var rowOpen = false;
        var columnsPerRow = 3;
        if (data.length > 0) {
            html += '<h3>All Flights Available </h3>';
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
                html += '<h5 class="card-title">Airline: ' + data[i].airline + '</h5>';
                html += '<p class="card-text"> From ' + data[i].departureLocation + ' <i class="fa-solid fa-route"></i> to ' + data[i].arrivalLocation + '</p>';
                html += '</div>';
                html += '<ul class="list-group list-group-flush">';
                html += '<li class="list-group-item"><i class="fa-solid fa-plane-departure"></i> Departure Time: ' + formatDate(data[i].departureTime) + '</li>';
                html += '<li class="list-group-item"><i class="fa-solid fa-plane-arrival"></i> Arrival Time: ' + formatDate(data[i].arrivalTime) + '</li>';
                html += '<li class="list-group-item"><i class="fa-solid fa-wallet"></i> Price: $' + data[i].price + '</li>';
                html += '</ul>';
                html += '<div class="card-body text-center">';
                html += `<button class="btn btn-success rounded-pill details-button" data-id="${data[i].flightId}" style="width:30%;">Details</button>`;
                html += '</div>';
                html += '</div>'; 
                html += '</div>';
                if (i % columnsPerRow === columnsPerRow - 1 || i === data.length - 1) {
                    html += '</div>';
                    rowOpen = false;
                }
            }
        } else {
            html = '<p>No flights found.</p>';
        }
        $('#flightSearchResults').html(html);
    }
    $(document).on('click', '.details-button', function () {
        var flightId = $(this).data('id');
        window.location.href = '/BookingManagement/Flight/' + flightId;
    });
    function formatDate(dateString) {
        var date = new Date(dateString);
        var year = date.getFullYear();
        var month = ('0' + (date.getMonth() + 1)).slice(-2);
        var day = ('0' + date.getDate()).slice(-2);
        var hours = ('0' + date.getHours()).slice(-2);
        var minutes = ('0' + date.getMinutes()).slice(-2);
        return year + '-' + month + '-' + day + ' ' + hours + ':' + minutes;
    }

    function loadInitialListings() {
        $.ajax({
            url: '/BookingManagement/Flight/GetFlights',
            method: 'GET',
            success: function (data) {
                loadFlightListings(data);
            },
            error: function (error) {
                console.error('Error fetching flights:', error);
            }
        });
    }

    $('#flightSearchForm').submit(function (event) {
        event.preventDefault(); 
        var formData = $(this).serialize();

        $.ajax({
            url: $(this).attr('action'),
            type: $(this).attr('method'),
            data: formData,
            dataType: 'json',
            beforeSend: function () {
                $('#flightSearchResults').html('<div class="spinner-border text-primary" role="status"><span class="sr-only">Loading...</span></div>');
            },
            success: function (data) {
                loadFlightListings(data);
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    });

    loadInitialListings();
});
