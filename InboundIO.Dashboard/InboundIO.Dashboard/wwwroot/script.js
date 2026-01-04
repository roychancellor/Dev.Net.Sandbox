const refreshBtn = document.getElementById('refresh-btn');
const toggleJson = document.getElementById('toggle-json');
const dashboard = document.getElementById('dashboard');

refreshBtn.addEventListener('click', loadHealthStatus);
toggleJson.addEventListener('change', () => {
    document.querySelectorAll('.json-output').forEach(div => {
        div.style.display = toggleJson.checked ? 'block' : 'none';
    });
});

async function loadHealthStatus() {
    dashboard.innerHTML = 'Loading...';

    try {
        const res = await fetch('/api/health-summary');
        const data = await res.json();
        dashboard.innerHTML = '';

        const entries = Object.entries(data);

        // Row 1: 1 spacer + 2 blocks + 1 spacer (centered)
        dashboard.appendChild(createSpacer());
        for (let i = 0; i < 2; i++) {
            const [appName, info] = entries[i];
            const block = createBlock(appName, info);
            dashboard.appendChild(block);
        }
        dashboard.appendChild(createSpacer());

        // Row 2: 4 blocks (full width)
        for (let i = 2; i < 6; i++) {
            const [appName, info] = entries[i];
            const block = createBlock(appName, info);
            dashboard.appendChild(block);
        }

        // Row 3: 1 spacer + 2 blocks + 1 spacer (centered)
        dashboard.appendChild(createSpacer());
        for (let i = 6; i < 8; i++) {
            const [appName, info] = entries[i];
            const block = createBlock(appName, info);
            dashboard.appendChild(block);
        }
        dashboard.appendChild(createSpacer());

        toggleJsonVisibility();
    } catch (err) {
        dashboard.innerHTML = `<p>Error loading health data: ${err.message}</p>`;
    }
}

function createBlock(appName, info) {
    const status = info.status || 'Unknown';
    const json = JSON.stringify(info.raw, null, 2);
    const block = document.createElement('div');
    block.className = `app-block ${status}`;
    block.innerHTML = `
        <h2>${appName}</h2>
        <p>Status: <strong>${status}</strong></p>
        <div class="json-output">${json}</div>
    `;
    return block;
}

function createSpacer() {
    const spacer = document.createElement('div');
    spacer.className = 'spacer';
    return spacer;
}


function toggleJsonVisibility() {
    const show = toggleJson.checked;
    document.querySelectorAll('.json-output').forEach(div => {
        div.style.display = show ? 'block' : 'none';
    });
}
