document.getElementById('searchBtn').addEventListener('click', async function () {
    const keyword = document.getElementById('keyword').value;
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

    const searchRequest = {
        keyword: keyword,
        startDate: startDate ? new Date(startDate).toISOString() : null,
        endDate: endDate ? new Date(endDate).toISOString() : null
    };

    const response = await fetch('/api/logs/search', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(searchRequest)
    });

    if (response.ok) {
        const results = await response.json();
        displayResults(results);
    } else {
        alert('Error fetching logs.');
    }
});

async function displayResults(results) {
    const resultsList = document.getElementById('resultsList');
    resultsList.innerHTML = '';

    results.forEach(result => {
        const li = document.createElement('li');
        const timestamp = new Date(result.timestamp).toLocaleString(); // format for readability
        li.textContent = `Time: ${timestamp} | ID: ${result.id} | Message: ${result.message}`;
        li.onclick = () => viewDetails(result.id);  // Refresh details when a new log is clicked
        resultsList.appendChild(li);
    });
}

async function viewDetails(id) {
    // Clear the current details content before updating
    const detailsContent = document.getElementById('logDetailsContent');
    detailsContent.textContent = 'Loading details...'; // Optional: show loading message

    const response = await fetch(`/api/logs/log/${id}`);
    const logDetails = await response.json();

    // After fetching, update the details section
    detailsContent.textContent = JSON.stringify(logDetails, null, 2);
    document.getElementById('logDetails').classList.remove('hidden');
}

document.getElementById('closeDetailsBtn').addEventListener('click', function () {
    document.getElementById('logDetails').classList.add('hidden');
});
