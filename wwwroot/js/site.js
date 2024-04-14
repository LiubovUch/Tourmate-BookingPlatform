@model YourViewModel

<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script>
    $(document).ready(function() {
        // AJAX for search results
        $('#searchForm').submit(function(event) {
            event.preventDefault(); // Prevent default form submission

            // Show loader/spinner
            $('#searchResults').html('<div class="loader"></div>');

            // Send AJAX request
            $.ajax({
                url: '@Url.Action("SearchResults", "Search")', // URL to your search results endpoint
                type: 'GET',
                data: $(this).serialize(), // Serialize form data
                success: function(response) {
                    $('#searchResults').html(response); // Update search results section
                },
                error: function(xhr, status, error) {
                    console.error(xhr.responseText); // Log error
                }
            });
        });

        // AJAX for form submission
        $('#bookingForm').submit(function(event) {
            event.preventDefault(); // Prevent default form submission

            // Send AJAX request
            $.ajax({
                url: '@Url.Action("ConfirmBooking", "Booking")', // URL to your booking confirmation endpoint
                type: 'POST',
                data: $(this).serialize(), // Serialize form data
                success: function(response) {
                    $('#bookingSection').html(response); // Update booking section
                },
                error: function(xhr, status, error) {
                    console.error(xhr.responseText); // Log error
                }
            });
        });
    });
</script>
