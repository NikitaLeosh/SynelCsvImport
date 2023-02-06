function hide_table_elements(table_id, visible) {
    // Shows only the first `visible` table elements
    table_elements = document.getElementById(table_id).children[1].children

    for (const element of table_elements) {
        if (visible == 0) {
            element.style.display = 'none'
        }
        else {
            element.style.display = 'table-row'
            visible -= 1
        }
    }
}

// Use below solution for <td> without `value` attribute
const getCellValue = (tr, idx) => tr.children[idx].innerText.replace('$', '') || tr.children[idx].textContent.replace('$', '');
/*const getCellValue = (tr, idx) => tr.children[idx].getAttribute('value')*/

const comparer = (idx, asc) => (a, b) =>
    ((v1, v2) => v1 !== '' && v2 !== '' && !isNaN(v1) && !isNaN(v2) ? v1 - v2 : v1.toString().localeCompare(v2))
        (getCellValue(asc ? a : b, idx), getCellValue(asc ? b : a, idx))

const reload_table = () => hide_table_elements('Employees', document.getElementById('form-select-employees').value)

window.addEventListener('load', function () {
    reload_table()

    // Show per page
    document.getElementById('form-select-employees').addEventListener('change', function () {
        counter = this.value
        hide_table_elements('Employees', this.value)
    });

    // Search in table
    document.getElementById('search-in-table').addEventListener('input', function () {
        let td, tdValue, filter, rows, searchCol;
        filter = this.value.toLowerCase();
        rows = document.getElementsByTagName('tr');
        searchCol = document.getElementById("searchBy").value;
        console.log('Check check!');
        if (filter == '')
            return reload_table()

        for (const row of rows) {
            td = row.getElementsByTagName('td')[searchCol];
            if (td) {
                tdValue = td.textValue || td.innerText;
                if (tdValue.toLowerCase().indexOf(filter) > -1) {
                    row.style.display = 'table-row'
                } else {
                    row.style.display = 'none'
                }
            }
        }
    });

    // Sort table
    document.querySelectorAll('th').forEach(th => th.addEventListener('click', (() => {
        const table = document.getElementById('Employees').children[1]

        Array.from(table.querySelectorAll('tr:nth-child(n)'))
            .sort(comparer(Array.from(th.parentNode.children).indexOf(th), this.asc = !this.asc))
            .forEach(tr => table.appendChild(tr));

        reload_table()
    })));
});