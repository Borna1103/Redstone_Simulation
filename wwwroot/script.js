window.addEventListener('DOMContentLoaded', () => {
    let selectedType = "Redstone"; 
    const buttons = document.querySelectorAll('#toolbar button');
    const viewport = document.getElementById('viewport')
    const grid = document.getElementById('grid');

    let scale = 1;
    let translateX = 0;
    let translateY = 0;
    const MIN_SCALE = 0.4;
    const MAX_SCALE = 2.5;

    const GRID_SIZE = 100;      
    const CELL_SIZE = 32;       
    const GRID_PX = GRID_SIZE * CELL_SIZE;

    let isPanning = false;
    let lastX = 0;
    let lastY = 0;

    const rows = 100;
    const cols = 100;

    const TICK_MS = 100;

    function applyTransform() {
        grid.style.transform =
            `translate(${translateX}px, ${translateY}px) scale(${scale})`;
    }

    function clampPan() {
        const viewW = viewport.clientWidth;
        const viewH = viewport.clientHeight;

        const scaledW = GRID_PX * scale;
        const scaledH = GRID_PX * scale;

        translateX = Math.min(0, Math.max(viewW - scaledW, translateX));
        translateY = Math.min(0, Math.max(viewH - scaledH, translateY));
    }

    viewport.addEventListener('mousedown', e => {
        if (e.button !== 1) return; // middle mouse only
        e.preventDefault();

        isPanning = true;
        lastX = e.clientX;
        lastY = e.clientY;
        viewport.classList.add('dragging');
    });

    window.addEventListener('mouseup', () => {
        isPanning = false;
        viewport.classList.remove('dragging');
    });

    window.addEventListener('mousemove', e => {
        if (!isPanning) return;

        translateX += e.clientX - lastX;
        translateY += e.clientY - lastY;

        lastX = e.clientX;
        lastY = e.clientY;

        clampPan();
        applyTransform();
    });

    

    viewport.addEventListener('wheel', e => {
        e.preventDefault();

        const zoomFactor = e.deltaY < 0 ? 1.1 : 0.9;
        const newScale = Math.min(MAX_SCALE, Math.max(MIN_SCALE, scale * zoomFactor));

        const rect = viewport.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;
        const mouseY = e.clientY - rect.top;

        const worldX = (mouseX - translateX) / scale;
        const worldY = (mouseY - translateY) / scale;

        scale = newScale;

        translateX = mouseX - worldX * scale;
        translateY = mouseY - worldY * scale;

        clampPan();
        applyTransform();
    }, { passive: false });

    // Toolbar button click handling
    buttons.forEach(button => {
        button.addEventListener('click', () => {
            buttons.forEach(b => b.classList.remove('selected'));
            button.classList.add('selected');
            selectedType = button.dataset.type;
        });
    });

    

    // Generate grid
    for (let r = 0; r < rows; r++) {
        for (let c = 0; c < cols; c++) {
            const cell = document.createElement('div');
            cell.className = 'cell';
            cell.dataset.x = r;
            cell.dataset.y = c;
            cell.dataset.hasObj = 'false';
            cell.toggleAttached = false
            grid.appendChild(cell);
        }
    }

    fetchInitialState();
    setInterval(fetchInitialState, TICK_MS);

    async function fetchInitialState() {
        const res = await fetch('/api/simulation/tick', {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            }
        }
        );
        
        if (res.ok) {
            updateGrid(await res.json());
        }
    }
    // Clear grid
    document.getElementById('clear-button').addEventListener('click', async () => {
        const response = await fetch('/api/simulation/clear', {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (!response.ok) return;

        updateGrid(await response.json());
    });

    // Place object
    grid.addEventListener('click', async (e) => {
        try {
            const cell = e.target.closest('.cell');
            if (!cell) return;
            if (cell.dataset.hasObj === 'true') return;
            const x = Number(cell.dataset.x);
            const y = Number(cell.dataset.y);

            const response = await fetch('/api/simulation/place', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ x, y, type: selectedType })
            });

            if (!response.ok) return;
            
            updateGrid(await response.json());
            AddToggleEvent();
        }
        catch (err) {
            console.error(err);
        }
        
    });

    grid.addEventListener('contextmenu', async (e) => {
        e.preventDefault();
        const cell = e.target.closest('.cell');
        if (!cell) return;
        if (cell.dataset.hasObj === 'false') return;
        const x = Number(cell.dataset.x);
        const y = Number(cell.dataset.y);

        const response = await fetch('/api/simulation/remove', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ x, y })
        });

        if (!response.ok) return;

        updateGrid(await response.json());
    });

    function updateGrid(cells) {
        cells.forEach(c => {
            const cell = document.querySelector(
                `.cell[data-x='${c.x}'][data-y='${c.y}']`
            );
            if (!cell) return;
            
            if (c.type != null) {
                cell.dataset.hasObj = 'true';
                cell.className = `cell ${c.type.toLowerCase()} ${c.shape.toLowerCase()} ${c.orientation == null ? '' : c.orientation.toLowerCase()}`;
                cell.style.setProperty('--power', c.strength );
            }
            else {
                cell.dataset.hasObj = 'false';
                cell.className = 'cell';
                cell.style.removeProperty('--power');
            }
            
        });
    }

    function AddToggleEvent() {
        const cells = document.querySelectorAll('.cell');
        cells.forEach(cell => {
            const type = cell.classList.contains('repeater') || 
                        cell.classList.contains('comparator') || 
                        cell.classList.contains('lever');
            if (!type) return;

            if (cell.dataset.toggleAttached === 'true') return;
            cell.dataset.toggleAttached = 'true';

            cell.addEventListener('click', async () => {
                const x = Number(cell.dataset.x);
                const y = Number(cell.dataset.y);

                try {
                    const response = await fetch('/api/simulation/toggle', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ x, y })
                    });

                    if (!response.ok) return;

                    updateGrid(await response.json());
                } catch (err) {
                    console.error('Toggle failed:', err);
                }
            });
        });
    }

});



