window.addEventListener('DOMContentLoaded', () => {
    let selectedType = "Redstone"; 
    const buttons = document.querySelectorAll('#toolbar button');
    const grid = document.getElementById('grid');
    const rows = 100;
    const cols = 100;

    const TICK_MS = 100;
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
            grid.appendChild(cell);
        }
    }

    fetchInitialState();
    setInterval(fetchInitialState(), TICK_MS);
    

    

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
        }
        catch (err) {
            console.error(err);
        }
        
    });

    // Remove object
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

    // Update grid helper
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

    
});



async function fetchInitialState() {
    const res = await fetch('/api/simulation/state');
    
    if (res.ok) {
        alert(JSON.stringify(await res.json()));
        updateGrid(await res.json());
    }
}