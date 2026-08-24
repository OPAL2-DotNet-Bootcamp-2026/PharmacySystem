// ==========================================
// TRANSFERS PAGE
// ==========================================

const API_URL = "https://localhost:7000";

const transferTable =
    document.querySelector("#transferTableBody");

const searchInput =
    document.querySelector("#searchInput");

const statusFilter =
    document.querySelector("#statusFilter");


// ==========================================
// GET ALL TRANSFERS
// ==========================================

async function loadTransfers() {

    try {

        const response =
            await fetch(
                `${API_URL}/api/Transfer`
            );


        if (!response.ok) {

            throw new Error(
                `Error: ${response.status}`
            );
        }


        const transfers =
            await response.json();


        console.log(transfers);

        renderTransfers(transfers);

    }

    catch (error) {

        console.error(
            "Failed to load transfers:",
            error
        );

    }
}


// ==========================================
// RENDER TRANSFERS
// ==========================================

function renderTransfers(transfers) {

    transferTable.innerHTML = "";


    transfers.forEach(transfer => {

        transferTable.innerHTML += `

            <tr>

                <td>
                    ${transfer.transferId}
                </td>

                <td>
                    ${transfer.warehouseId}
                </td>

                <td>
                    ${transfer.pharmacyId}
                </td>

                <td>
                    ${transfer.transferDate}
                </td>

                <td>
                    ${transfer.receiveDate ?? "-"}
                </td>

                <td>
                    ${transfer.status}
                </td>

                <td>

                    ${
                        transfer.status !== "Received"

                        ? `
                            <button
                                class="btn btn-sm btn-success"
                                onclick="receiveTransfer(${transfer.transferId})"
                            >
                                Receive
                            </button>
                          `

                        : "Received"
                    }

                </td>

            </tr>

        `;
    });
}


// ==========================================
// RECEIVE TRANSFER
// ==========================================

async function receiveTransfer(id) {

    try {

        const response =
            await fetch(
                `${API_URL}/api/Transfer/${id}/receive`,
                {
                    method: "PUT"
                }
            );


        if (!response.ok) {

            throw new Error(
                `Error: ${response.status}`
            );
        }


        console.log(
            "Transfer received successfully"
        );


        loadTransfers();

    }

    catch (error) {

        console.error(
            "Failed to receive transfer:",
            error
        );

    }
}


// ==========================================
// FILTER TRANSFERS
// ==========================================

async function filterTransfers() {

    try {

        const response =
            await fetch(
                `${API_URL}/api/Transfer`
            );


        if (!response.ok) {

            throw new Error(
                `Error: ${response.status}`
            );
        }


        const transfers =
            await response.json();


        const searchText =
            searchInput.value
                .trim()
                .toLowerCase();


        const selectedStatus =
            statusFilter.value;


        const filteredTransfers =
            transfers.filter(transfer => {

                const matchesSearch =

                    transfer.status
                        .toLowerCase()
                        .includes(searchText)

                    ||

                    String(
                        transfer.transferId
                    ).includes(searchText);


                const matchesStatus =

                    selectedStatus === "All"

                    ||

                    transfer.status ===
                        selectedStatus;


                return (
                    matchesSearch &&
                    matchesStatus
                );
            });


        renderTransfers(
            filteredTransfers
        );

    }

    catch (error) {

        console.error(
            "Failed to filter transfers:",
            error
        );

    }
}


// ==========================================
// EVENTS
// ==========================================

if (searchInput) {

    searchInput.addEventListener(
        "input",
        filterTransfers
    );
}


if (statusFilter) {

    statusFilter.addEventListener(
        "change",
        filterTransfers
    );
}


// ==========================================
// FIRST PAGE LOAD
// ==========================================

loadTransfers();