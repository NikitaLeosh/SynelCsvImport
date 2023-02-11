var previousMax, currentPage;

function hide_table_elements(table_id, currentPage, displayPerPage) {
    // Shows only the table elements corresponding to the page.
    table_elements = document.getElementById(table_id).children[1].querySelectorAll('tr:nth-child(n)');
    var visibleMax = currentPage * displayPerPage;
    var visibleMin = (currentPage - 1) * displayPerPage;
    table_elements.forEach((element, index) => {
        if (index >= visibleMax || index < visibleMin) {
            element.style.display = 'none'
        }
        else {
            element.style.display = 'table-row'
        }
    });
};

//const getCellValue = (row, index) => $(row).children('td').eq(index).text();
function getCellValue(row, index) {
    console.log(row.children[index], index);
    return row.children[index].innerText || row.children[index].textContent;
}
const comparer = (idx, asc) => (a, b) => ((v1, v2) =>
    v1 !== '' && v2 !== '' && !isNaN(v1) && !isNaN(v2) ? v1 - v2 : v1.toString().localeCompare(v2)
)(getCellValue(asc ? a : b, idx), getCellValue(asc ? b : a, idx));


const reload_table = () => hide_table_elements('Employees', 1, document.getElementById('display-per-page').value)

window.addEventListener('load', function () {
    reload_table();
    currentPage = 1;
    var tableElements = document.getElementById('Employees').children[1].querySelectorAll('tr:nth-child(n)');
    var paginationNumbers = document.getElementById('pagination-numbers');
    //append page numbers to html
    const appendPageNumbers = (current, max) => {
        const pageNumber = document.createElement("div");
        pageNumber.className = "pageNumber"
        pageNumber.innerHTML = `page ${current} of ${max}`;
        paginationNumbers.appendChild(pageNumber);
    };
    //count pages
    const getPaginationNumbers = (current) => {
        paginationNumbers.innerHTML = '';
        var displayPerPage = document.getElementById('display-per-page').value;
        var pageCount = Math.ceil(tableElements.length / displayPerPage);
        appendPageNumbers(current, pageCount);

    };
    getPaginationNumbers(currentPage);

    //next page
    document.getElementById('next-button').addEventListener('click', function () {
        var displayPerPage = document.getElementById('display-per-page').value;
        var pageCount = Math.ceil(tableElements.length / displayPerPage);
        if (currentPage < pageCount) {
            currentPage++;
            hide_table_elements('Employees', currentPage, displayPerPage);
            getPaginationNumbers(currentPage);
        }
    })
    //previous page
    document.getElementById('prev-button').addEventListener('click', function () {
        var displayPerPage = document.getElementById('display-per-page').value;
        if (currentPage > 1) {
            currentPage--;
            hide_table_elements('Employees', currentPage, displayPerPage);
            getPaginationNumbers(currentPage);

        }
    })

    // Show per page
    document.getElementById('display-per-page').addEventListener('change', function () {
        console.log('Check change!');
        counter = this.value
        hide_table_elements('Employees', 1, this.value)
        getPaginationNumbers(currentPage);
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

    //sort table
    document.querySelectorAll('th').forEach(th => th.addEventListener('click', (() => {
        const table = document.getElementById('Employees').children[1];
        Array.from(table.querySelectorAll(`tr`)).
            sort(comparer(Array.from(th.parentNode.children).indexOf(th), this.asc = !this.asc))
            .forEach(tr => table.appendChild(tr));
    })))

});