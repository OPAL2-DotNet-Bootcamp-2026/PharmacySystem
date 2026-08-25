// ==========================================
// TRANSFERS PAGE
// ==========================================


// ==========================================
// HTML ELEMENTS
// ==========================================

const pharmacistOrder =
    document.querySelector(
        "#pharmacistOrder"
    );


const transferForm =
    document.querySelector(
        "#transferForm"
    );


const transferMedicinesBody =
    document.querySelector(
        "#transferMedicinesBody"
    );


const transferTable =
    document.querySelector(
        "#transferTableBody"
    );


const transferCount =
    document.querySelector(
        "#transferCount"
    );


const roadSummary =
    document.querySelector(
        "#roadSummary"
    );


const roadContent =
    document.querySelector(
        "#roadContent"
    );


const incomingTransferCount =
    document.querySelector(
        "#incomingTransferCount"
    );


const incomingTransfers =
    document.querySelector(
        "#incomingTransfers"
    );



// ==========================================
// STORE APPROVED ORDERS
// ==========================================

let approvedOrders = [];



// ==========================================
// LOAD APPROVED PHARMACIST ORDERS
// ==========================================

async function loadApprovedOrders() {

    try {

        const orders =
            await Api.get(
                "/api/PharmacistOrder"
            );


        approvedOrders =
            orders.filter(
                order =>
                    order.status ===
                    "Approved"
            );


        pharmacistOrder.innerHTML = `

            <option
                value=""
                selected
                disabled
            >
                Select approved order
            </option>

        `;


        approvedOrders.forEach(
            order => {

                pharmacistOrder.innerHTML += `

                    <option
                        value="${order.pharmacistOrderId}"
                    >

                        Order #${order.pharmacistOrderId}

                    </option>

                `;

            }
        );

    }

    catch (error) {

        console.error(
            "Failed to load approved orders:",
            error
        );

    }

}



// ==========================================
// WHEN APPROVED ORDER CHANGES
// ==========================================

function handleOrderChange() {

    const selectedId =
        Number(
            pharmacistOrder.value
        );


    const selectedOrder =
        approvedOrders.find(
            order =>
                order.pharmacistOrderId
                ===
                selectedId
        );


    if (!selectedOrder) {

        return;

    }


    renderOrderMedicines(
        selectedOrder
    );

}



// ==========================================
// RENDER ORDER MEDICINES
// ==========================================

function renderOrderMedicines(
    order
) {

    transferMedicinesBody.innerHTML =
        "";


    const details =
        order.orderDetails
        ??
        order.pharmacistOrderDetails
        ??
        [];


    details.forEach(
        detail => {

            transferMedicinesBody.innerHTML += `

                <tr>


                    <td>

                        <strong>

                            ${
                                detail.medicineName
                                ??
                                `Medicine #${detail.medicineId}`
                            }

                        </strong>

                    </td>


                    <td>

                        ${
                            detail.categoryName
                            ??
                            "-"
                        }

                    </td>


                    <td class="text-end">

                        ${
                            detail.quantity
                            ??
                            0
                        }

                    </td>


                </tr>

            `;

        }
    );


    if (
        details.length === 0
    ) {

        transferMedicinesBody.innerHTML = `

            <tr>

                <td
                    colspan="3"
                    class="text-center text-muted"
                >

                    No medicines found.

                </td>

            </tr>

        `;

    }

}



// ==========================================
// CREATE TRANSFER
// ==========================================

async function createTransfer(
    event
) {

    event.preventDefault();


    const pharmacistOrderId =
        Number(
            pharmacistOrder.value
        );


    if (!pharmacistOrderId) {

        alert(
            "Please select an approved order."
        );

        return;

    }



    // Find selected order from array

    const selectedOrder =
        approvedOrders.find(
            order =>
                order.pharmacistOrderId
                ===
                pharmacistOrderId
        );


    if (!selectedOrder) {

        alert(
            "Approved order not found."
        );

        return;

    }



    // Get medicines/details

    const details =
        selectedOrder.orderDetails
        ??
        selectedOrder.pharmacistOrderDetails
        ??
        [];



    // Convert order details
    // into transfer details

    const transferDetails =
        details.map(
            detail => {

                return {

                    medicineId:
                        detail.medicineId,

                    quantity:
                        detail.quantity

                };

            }
        );



    // ======================================
    // CREATE TRANSFER OBJECT
    // ======================================

    const newTransfer = {

        pharmacistOrderId:
            selectedOrder.pharmacistOrderId,

        pharmacyId:
            selectedOrder.pharmacyId,

        warehouseId:
            selectedOrder.warehouseId,

        transferDetails:
            transferDetails

    };



    console.log(
        "Transfer to create:",
        newTransfer
    );



    try {

        await Api.post(
            "/api/Transfer",
            newTransfer
        );


        alert(
            "Transfer created successfully."
        );



        // Reset form

        transferForm.reset();



        // Reset medicine table

        transferMedicinesBody.innerHTML = `

            <tr>

                <td
                    colspan="3"
                    class="text-center text-muted"
                >

                    Select an approved order.

                </td>

            </tr>

        `;



        // Reload approved orders
        // because created order should not
        // stay available

        await loadApprovedOrders();



        // Reload transfer history

        await loadTransfers();

    }

    catch (error) {

        console.error(
            "Failed to create transfer:",
            error
        );


        alert(
            error.message
            ||
            "Failed to create transfer."
        );

    }

}



// ==========================================
// LOAD ALL TRANSFERS
// ==========================================

async function loadTransfers() {

    try {

        const transfers =
            await Api.get(
                "/api/Transfer"
            );


        console.log(
            "Transfers:",
            transfers
        );


        renderTransfers(
            transfers
        );


        renderRoadTransfers(
            transfers
        );


        renderIncomingTransfers(
            transfers
        );

    }

    catch (error) {

        console.error(
            "Failed to load transfers:",
            error
        );


        if (transferTable) {

            transferTable.innerHTML = `

                <tr>

                    <td
                        colspan="7"
                        class="text-center text-danger"
                    >

                        Failed to load transfers.

                    </td>

                </tr>

            `;

        }

    }

}



// ==========================================
// RENDER TRANSFER HISTORY
// ==========================================

function renderTransfers(
    transfers
) {

    if (!transferTable) {

        return;

    }


    transferTable.innerHTML =
        "";



    if (transferCount) {

        transferCount.textContent =
            `${transfers.length} transfers`;

    }



    transfers.forEach(
        transfer => {


            const transferId =
                transfer.transferId;



            const warehouseName =
                transfer.warehouseName
                ??
                `Warehouse #${transfer.warehouseId}`;



            const pharmacyName =
                transfer.pharmacyName
                ??
                `Pharmacy #${transfer.pharmacyId}`;



            const status =
                transfer.status
                ??
                "-";



            const medicineNames =
                transfer.medicineNames
                ??
                [];



            const contents =
                medicineNames.length > 0

                    ? medicineNames.join(", ")

                    : "-";



            let actionHtml =
                "";



            if (
                status ===
                "Shipped"
            ) {

                actionHtml = `

                    <button
                        class="confirm-btn"
                        onclick="receiveTransfer(${transferId})"
                    >

                        Confirm receive

                    </button>

                `;

            }

            else {

                actionHtml = `

                    <span class="received-date">

                        ${
                            transfer.receiveDate
                            ?
                            `Received ${formatDate(
                                transfer.receiveDate
                            )}`
                            :
                            status
                        }

                    </span>

                `;

            }



            transferTable.innerHTML += `

                <tr
                    data-id="${transferId}"
                >


                    <td>

                        <strong>

                            #${transferId}

                        </strong>

                    </td>


                    <td>

                        ${warehouseName}

                    </td>


                    <td>

                        ${pharmacyName}

                    </td>


                    <td>

                        ${contents}

                    </td>


                    <td class="date-text">

                        ${
                            formatDate(
                                transfer.transferDate
                            )
                        }

                    </td>


                    <td>

                        <span
                            class="status ${status.toLowerCase()}"
                        >

                            <span
                                class="status-dot"
                            >
                            </span>

                            ${
                                status.toUpperCase()
                            }

                        </span>

                    </td>


                    <td class="text-end">

                        ${actionHtml}

                    </td>


                </tr>

            `;

        }
    );



    if (
        transfers.length === 0
    ) {

        transferTable.innerHTML = `

            <tr>

                <td
                    colspan="7"
                    class="text-center text-muted"
                >

                    No transfers found.

                </td>

            </tr>

        `;

    }

}



// ==========================================
// ON THE ROAD
// ==========================================

function renderRoadTransfers(
    transfers
) {

    if (
        !roadContent
        ||
        !roadSummary
    ) {

        return;

    }



    const shippedTransfers =
        transfers.filter(
            transfer =>
                transfer.status ===
                "Shipped"
        );



    roadSummary.textContent =
        `${shippedTransfers.length} shipment(s) on the road`;



    roadContent.innerHTML =
        "";



    if (
        shippedTransfers.length === 0
    ) {

        roadContent.innerHTML = `

            <p class="text-muted mb-0">

                No transfers are currently on the road.

            </p>

        `;

        return;

    }



    const transfer =
        shippedTransfers[0];



    const warehouseName =
        transfer.warehouseName
        ??
        `Warehouse #${transfer.warehouseId}`;



    const pharmacyName =
        transfer.pharmacyName
        ??
        `Pharmacy #${transfer.pharmacyId}`;



    roadContent.innerHTML = `

        <div class="road-place">

            <span>
                LEFT
            </span>

            <strong>

                ${warehouseName}

            </strong>

        </div>



        <div class="road">

            <div class="road-line">
            </div>


            <div class="truck">

                <i class="bi bi-truck"></i>

            </div>

        </div>



        <div
            class="road-place road-place-right"
        >

            <span>
                HEADING TO
            </span>

            <strong>

                ${pharmacyName}

            </strong>

            <small>

                #${transfer.transferId}

            </small>

        </div>

    `;

}



// ==========================================
// PHARMACIST INCOMING TRANSFERS
// ==========================================

function renderIncomingTransfers(
    transfers
) {

    if (!incomingTransfers) {

        return;

    }



    const incoming =
        transfers.filter(
            transfer =>
                transfer.status ===
                "Shipped"
        );



    if (
        incomingTransferCount
    ) {

        incomingTransferCount.textContent =
            `${incoming.length} incoming transfer(s)`;

    }



    incomingTransfers.innerHTML =
        "";



    incoming.forEach(
        transfer => {


            const warehouseName =
                transfer.warehouseName
                ??
                `Warehouse #${transfer.warehouseId}`;



            const medicineNames =
                transfer.medicineNames
                ??
                [];



            incomingTransfers.innerHTML += `

                <div
                    class="incoming-transfer-row"
                >


                    <div>

                        <span class="small-label">

                            TRANSFER

                        </span>

                        <strong>

                            #${transfer.transferId}

                        </strong>

                    </div>


                    <div>

                        <span class="small-label">

                            FROM

                        </span>

                        <strong>

                            ${warehouseName}

                        </strong>

                    </div>


                    <div>

                        <span class="small-label">

                            CONTENTS

                        </span>

                        <strong>

                            ${
                                medicineNames.length > 0

                                ?
                                medicineNames.join(", ")

                                :
                                "-"
                            }

                        </strong>

                    </div>


                    <div>

                        <span
                            class="status shipped"
                        >

                            <span
                                class="status-dot"
                            >
                            </span>

                            SHIPPED

                        </span>

                    </div>


                    <div>

                        <button
                            class="confirm-btn"
                            onclick="receiveTransfer(${transfer.transferId})"
                        >

                            Confirm receive

                        </button>

                    </div>


                </div>

            `;

        }
    );



    if (
        incoming.length === 0
    ) {

        incomingTransfers.innerHTML = `

            <p class="text-muted">

                No incoming transfers.

            </p>

        `;

    }

}



// ==========================================
// RECEIVE TRANSFER
// ==========================================

async function receiveTransfer(
    id
) {

    try {

        await Api.put(
            `/api/Transfer/${id}/receive`,
            {}
        );


        alert(
            "Transfer received successfully."
        );


        await loadTransfers();

    }

    catch (error) {

        console.error(
            "Failed to receive transfer:",
            error
        );


        alert(
            error.message
            ||
            "Failed to receive transfer."
        );

    }

}



// ==========================================
// FORMAT DATE
// ==========================================

function formatDate(
    dateValue
) {

    if (!dateValue) {

        return "-";

    }


    const date =
        new Date(
            dateValue
        );


    if (
        Number.isNaN(
            date.getTime()
        )
    ) {

        return dateValue;

    }


    return date.toLocaleDateString(

        "en-GB",

        {

            day:
                "2-digit",

            month:
                "short",

            year:
                "numeric"

        }

    );

}



// ==========================================
// EVENTS
// ==========================================

if (
    pharmacistOrder
) {

    pharmacistOrder.addEventListener(

        "change",

        handleOrderChange

    );

}



if (
    transferForm
) {

    transferForm.addEventListener(

        "submit",

        createTransfer

    );

}



// ==========================================
// FIRST PAGE LOAD
// ==========================================

loadApprovedOrders();

loadTransfers();