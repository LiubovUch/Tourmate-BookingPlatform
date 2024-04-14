function performSearch() {
    $.ajax({
        url: '/Hotel/Search', // Update URL according to your routing
        method: 'GET',
        data: $('#searchForm').serialize(), // Serialize form data for sending to the server
        success: function (data) {
            // Update the search results section with the returned data
            $('.row').html(data);
        },
        error: function (xhr, status, error) {
            console.error("Error performing search:", error);
        }
    });
}
