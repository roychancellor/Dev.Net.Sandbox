document.getElementById('searchBtn').addEventListener('click', async function () {
    const logDetails = document.getElementById('logDetails');
    logDetails.classList.add('hidden'); // Hide log details

    const keyword = document.getElementById('keyword').value;
    const field = document.getElementById('field').value;
    const timeRange = document.getElementById('timeRange').value;
    let startDate = null;
    let endDate = null;

    // Calculate date range based on the selected time range option
    if (timeRange === 'custom') {
        startDate = document.getElementById('startDate').value;
        endDate = document.getElementById('endDate').value;
    } else {
        const now = new Date();
        switch (timeRange) {
            case 'last15':
                startDate = new Date(now.getTime() - 15 * 60000);
                break;
            case 'last30':
                startDate = new Date(now.getTime() - 30 * 60000);
                break;
            case 'last1Hour':
                startDate = new Date(now.getTime() - 1 * 3600000);
                break;
            case 'last3Hours':
                startDate = new Date(now.getTime() - 3 * 3600000);
                break;
            case 'last6Hours':
                startDate = new Date(now.getTime() - 6 * 3600000);
                break;
            case 'last12Hours':
                startDate = new Date(now.getTime() - 12 * 3600000);
                break;
            case 'last24Hours':
                startDate = new Date(now.getTime() - 24 * 3600000);
                break;
            case 'last2Days':
                startDate = new Date(now.getTime() - 48 * 3600000);
                break;
            case 'last7Days':
                startDate = new Date(now.getTime() - 168 * 3600000);
                break;
            default: // "allTime"
                startDate = null;
                break;
        }
    }

    const searchRequest = {
        keyword: keyword,
        field: field, // include the selected field
        value: keyword, // match the keyword against the field, or all fields if field is empty
        //startDate: startDate ? new Date(startDate).toISOString() : null,
        //endDate: endDate ? new Date(endDate).toISOString() : null
        startDate: startDate ? formatLocalDateTime(new Date(startDate)) : null,
        endDate: endDate ? formatLocalDateTime(new Date(endDate)) : null

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
    const detailsContent = document.getElementById('logDetailsContent');
    detailsContent.textContent = 'Loading details...'; // Optional loading message

    const response = await fetch(`/api/logs/log/${id}`);
    const logDetails = await response.json();

    detailsContent.textContent = JSON.stringify(logDetails, null, 2);
    document.getElementById('logDetails').classList.remove('hidden');
}

document.getElementById('closeDetailsBtn').addEventListener('click', function () {
    document.getElementById('logDetails').classList.add('hidden');
});

document.getElementById('copyDetailsBtn').addEventListener('click', function () {
    const detailsContent = document.getElementById('logDetailsContent');
    const text = detailsContent.textContent || detailsContent.innerText;

    // Try to copy the text to clipboard
    navigator.clipboard.writeText(text).then(function () {
        showToast('Log details copied to clipboard!');
    }).catch(function (err) {
        console.error('Error copying text to clipboard', err);
        alert('Failed to copy details.');
    });
});

function showToast(message) {
    const toast = document.createElement('div');
    toast.classList.add('toast');
    toast.textContent = message;

    // Append toast to the body
    document.body.appendChild(toast);

    // Show the toast
    toast.classList.add('show');

    // Hide the toast after 2 seconds
    setTimeout(() => {
        toast.classList.remove('show');
        document.body.removeChild(toast); // Remove toast after it disappears
    }, 2000);
}

// Show/hide the custom date input fields based on the time range selection
document.getElementById('timeRange').addEventListener('change', function () {
    const customFields = document.getElementById('customDateFields');
    if (this.value === 'custom') {
        customFields.classList.remove('hidden');
    } else {
        customFields.classList.add('hidden');
    }
});

function formatLocalDateTime(date) {
    const pad = (n) => n.toString().padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
}

