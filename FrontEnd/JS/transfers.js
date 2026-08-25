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
// HELPER
// Supports camelCase and PascalCase
// ==========================================

function getValue(
    item,
    camelCase,
    pascalCase,
    fallback = null
) {

    return (
        item?.[camelCase]
        ??
        item?.[pascalCase]
        ??
        fallback
    );

}



// ==========================================
// LOAD APPROVED PHARMACIST ORDERS
// GET /api/PharmacistOrder
// ==========================================

async function loadApprovedOrders() {

    try {

        const orders =
            await Api.get(
                "/PharmacistOrder"
            );


        console.log(
            "PHARMACIST ORDERS:",
            orders
        );


        const orderList =
            Array.isArray(orders)
                ? orders
                : [];


        approvedOrders =
            orderList.filter(
                function (order) {

                    const status =
                        getValue(
                            order,
                            "status",
                            "Status",
                            ""
                        );


                    return (
                        String(status)
                            .toLowerCase()
                        ===
                        "approved"
                    );

                }
            );


        pharmacistOrder.innerHTML = `

            <option
                value=""
                selected
                disabled>

                Select approved order

            </option>

        `;


        if (
            approvedOrders.length === 0
        ) {

            pharmacistOrder.innerHTML += `

                <option
                    value=""
                    disabled>

                    No approved orders found

                </option>

            `;

            return;

        }


        approvedOrders.forEach(
            function (order) {

                const orderId =
                    Number(

                        getValue(
                            order,
                            "pharmacistOrderId",
                            "PharmacistOrderId",
                            0
                        )

                    );


                const pharmacyName =
                    getValue(
                        order,
                        "pharmacyName",
                        "PharmacyName",
                        ""
                    );


                pharmacistOrder.innerHTML += `

                    <option
                        value="${orderId}">

                        Order #${orderId}
                        ${
                            pharmacyName
                                ? ` - ${pharmacyName}`
                                : ""
                        }

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


        pharmacistOrder.innerHTML = `

            <option
                value=""
                selected
                disabled>

                Could not load approved orders

            </option>

        `;

    }

}



// ==========================================
// ORDER CHANGE
// ==========================================

function handleOrderChange() {

    const selectedId =
        Number(
            pharmacistOrder.value
        );


    const selectedOrder =
        approvedOrders.find(
            function (order) {

                const orderId =
                    Number(

                        getValue(
                            order,
                            "pharmacistOrderId",
                            "PharmacistOrderId",
                            0
                        )

                    );


                return (
                    orderId ===
                    selectedId
                );

            }
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

        getValue(
            order,
            "orderDetails",
            "OrderDetails",
            null
        )

        ??

        getValue(
            order,
            "pharmacistOrderDetails",
            "PharmacistOrderDetails",
            []
        );


    if (
        !Array.isArray(details)
        ||
        details.length === 0
    ) {

        transferMedicinesBody.innerHTML = `

            <tr>

                <td
                    colspan="3"
                    class="text-center text-muted">

                    No medicines found.

                </td>

            </tr>

        `;

        return;

    }


    details.forEach(
        function (detail) {

            const medicineName =
                getValue(
                    detail,
                    "medicineName",
                    "MedicineName",
                    null
                );


            const medicineID =
                Number(

                    getValue(
                        detail,
                        "medicineID",
                        "MedicineID",

                        getValue(
                            detail,
                            "medicineId",
                            "MedicineId",
                            0
                        )
                    )

                );


            const quantity =
                Number(

                    getValue(
                        detail,
                        "quantity",
                        "Quantity",
                        0
                    )

                );


            transferMedicinesBody.innerHTML += `

                <tr>

                    <td>

                        <strong>

                            ${
                                medicineName
                                ??
                                `Medicine #${medicineID}`
                            }

                        </strong>

                    </td>


                    <td>

                        -

                    </td>


                    <td class="text-end">

                        ${quantity}

                    </td>

                </tr>

            `;

        }
    );

}



// ==========================================
// CREATE TRANSFER
// POST /api/Transfer
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


    const selectedOrder =
        approvedOrders.find(
            function (order) {

                const orderId =
                    Number(

                        getValue(
                            order,
                            "pharmacistOrderId",
                            "PharmacistOrderId",
                            0
                        )

                    );


                return (
                    orderId ===
                    pharmacistOrderId
                );

            }
        );


    if (!selectedOrder) {

        alert(
            "Approved order not found."
        );

        return;

    }



    // ==========================================
    // PHARMACY ID
    // ==========================================

    const pharmacyID =
        Number(

            getValue(
                selectedOrder,
                "pharmacyID",
                "PharmacyID",

                getValue(
                    selectedOrder,
                    "pharmacyId",
                    "PharmacyId",
                    0
                )
            )

        );


    if (!pharmacyID) {

        console.log(
            "SELECTED ORDER:",
            selectedOrder
        );

        alert(
            "Pharmacy ID was not found in this order."
        );

        return;

    }



    // ==========================================
    // ORDER DETAILS
    // ==========================================

    const details =

        getValue(
            selectedOrder,
            "orderDetails",
            "OrderDetails",
            null
        )

        ??

        getValue(
            selectedOrder,
            "pharmacistOrderDetails",
            "PharmacistOrderDetails",
            []
        );


    if (
        !Array.isArray(details)
        ||
        details.length === 0
    ) {

        alert(
            "This order has no medicines."
        );

        return;

    }



    // ==========================================
    // TRANSFER DETAILS
    // ==========================================

    const transferDetails =
        details.map(
            function (detail) {

                const medicineID =
                    Number(

                        getValue(
                            detail,
                            "medicineID",
                            "MedicineID",

                            getValue(
                                detail,
                                "medicineId",
                                "MedicineId",
                                0
                            )
                        )

                    );


                const quantity =
                    Number(

                        getValue(
                            detail,
                            "quantity",
                            "Quantity",
                            0
                        )

                    );


                return {

                    medicineID:
                        medicineID,

                    quantity:
                        quantity

                };

            }
        );



    // ==========================================
    // VALIDATE DETAILS
    // ==========================================

    const invalidDetail =
        transferDetails.find(
            function (detail) {

                return (
                    detail.medicineID <= 0
                    ||
                    detail.quantity <= 0
                );

            }
        );


    if (invalidDetail) {

        console.log(
            "INVALID DETAILS:",
            transferDetails
        );

        alert(
            "One medicine has invalid data."
        );

        return;

    }



    // ==========================================
    // ONE MAIN WAREHOUSE
    // ==========================================

    const MAIN_WAREHOUSE_ID = 1;



    // ==========================================
    // CREATE BODY
    // ==========================================

    const newTransfer = {

        warehouseID:
            MAIN_WAREHOUSE_ID,

        pharmacyID:
            pharmacyID,

        pharmacistOrderId:
            pharmacistOrderId,

        transferDetails:
            transferDetails

    };


    console.log(
        "TRANSFER TO CREATE:",
        newTransfer
    );


    try {

        const result =
            await Api.post(
                "/Transfer",
                newTransfer
            );


        console.log(
            "TRANSFER CREATED:",
            result
        );


        alert(
            "Transfer created successfully."
        );


        transferForm.reset();


        transferMedicinesBody.innerHTML = `

            <tr>

                <td
                    colspan="3"
                    class="text-center text-muted">

                    Select an approved order.

                </td>

            </tr>

        `;


        await loadApprovedOrders();

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
// GET /api/Transfer
// ==========================================

async function loadTransfers() {

    try {

        const transfers =
            await Api.get(
                "/Transfer"
            );


        console.log(
            "TRANSFERS:",
            transfers
        );


        const transferList =
            Array.isArray(transfers)
                ? transfers
                : [];


        renderTransfers(
            transferList
        );


        renderRoadTransfers(
            transferList
        );


        renderIncomingTransfers(
            transferList
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
                        class="text-center text-danger">

                        Failed to load transfers.

                    </td>

                </tr>

            `;

        }


        if (roadSummary) {

            roadSummary.textContent =
                "Could not load transfers.";

        }

    }

}



// ==========================================
// RENDER ALL TRANSFERS
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
        function (transfer) {

            const transferId =
                Number(

                    getValue(
                        transfer,
                        "transferId",
                        "TransferId",

                        getValue(
                            transfer,
                            "transferID",
                            "TransferID",
                            0
                        )
                    )

                );


            const warehouseID =
                Number(

                    getValue(
                        transfer,
                        "warehouseID",
                        "WarehouseID",

                        getValue(
                            transfer,
                            "warehouseId",
                            "WarehouseId",
                            0
                        )
                    )

                );


            const pharmacyID =
                Number(

                    getValue(
                        transfer,
                        "pharmacyID",
                        "PharmacyID",

                        getValue(
                            transfer,
                            "pharmacyId",
                            "PharmacyId",
                            0
                        )
                    )

                );


            const warehouseName =
                getValue(
                    transfer,
                    "location",
                    "Location",

                    getValue(
                        transfer,
                        "warehouseName",
                        "WarehouseName",
                        `Warehouse #${warehouseID}`
                    )
                );


            const pharmacyName =
                getValue(
                    transfer,
                    "pharmacyName",
                    "PharmacyName",
                    `Pharmacy #${pharmacyID}`
                );


            const status =
                getValue(
                    transfer,
                    "status",
                    "Status",
                    "-"
                );


            const transferDate =
                getValue(
                    transfer,
                    "transferDate",
                    "TransferDate",
                    null
                );


            const receiveDate =
                getValue(
                    transfer,
                    "receiveDate",
                    "ReceiveDate",
                    null
                );


            const transferDetails =
                getValue(
                    transfer,
                    "transferDetails",
                    "TransferDetails",
                    []
                );


            const medicineNames =
                Array.isArray(
                    transferDetails
                )
                    ?
                    transferDetails.map(
                        function (detail) {

                            return (

                                getValue(
                                    detail,
                                    "medicineName",
                                    "MedicineName",
                                    null
                                )

                                ??

                                `Medicine #${
                                    getValue(
                                        detail,
                                        "medicineID",
                                        "MedicineID",
                                        ""
                                    )
                                }`

                            );

                        }
                    )
                    :
                    [];


            const contents =
                medicineNames.length > 0
                    ?
                    medicineNames.join(
                        ", "
                    )
                    :
                    "-";


            let actionHtml =
                "";


            const cleanStatus =
                String(status)
                    .toLowerCase();


            if (
                cleanStatus === "shipped"
                ||
                cleanStatus === "in transit"
            ) {

                actionHtml = `

                    <button
                        class="confirm-btn"
                        onclick="receiveTransfer(${transferId})">

                        Confirm receive

                    </button>

                `;

            }
            else {

                actionHtml = `

                    <span class="received-date">

                        ${
                            receiveDate
                                ?
                                `Received ${
                                    formatDate(
                                        receiveDate
                                    )
                                }`
                                :
                                status
                        }

                    </span>

                `;

            }


            transferTable.innerHTML += `

                <tr
                    data-id="${transferId}">


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
                                transferDate
                            )
                        }

                    </td>


                    <td>

                        <span
                            class="status ${cleanStatus.replace(" ", "-")}">

                            <span
                                class="status-dot">
                            </span>

                            ${String(status).toUpperCase()}

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
                    class="text-center text-muted">

                    No transfers found.

                </td>

            </tr>

        `;

    }

}



// ==========================================
// ON THE ROAD
// Pending / Shipped / In Transit
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


    const roadTransfers =
        transfers.filter(
            function (transfer) {

                const status =
                    getValue(
                        transfer,
                        "status",
                        "Status",
                        ""
                    );


                const cleanStatus =
                    String(status)
                        .toLowerCase();


                return (
                    cleanStatus === "pending"
                    ||
                    cleanStatus === "shipped"
                    ||
                    cleanStatus === "in transit"
                );

            }
        );


    roadSummary.textContent =
        `${roadTransfers.length} shipment(s) on the road`;


    roadContent.innerHTML =
        "";


    if (
        roadTransfers.length === 0
    ) {

        roadContent.innerHTML = `

            <p class="text-muted mb-0">

                No transfers are currently on the road.

            </p>

        `;

        return;

    }


    const transfer =
        roadTransfers[0];


    const transferId =
        Number(

            getValue(
                transfer,
                "transferId",
                "TransferId",

                getValue(
                    transfer,
                    "transferID",
                    "TransferID",
                    0
                )
            )

        );


    const pharmacyID =
        Number(

            getValue(
                transfer,
                "pharmacyID",
                "PharmacyID",

                getValue(
                    transfer,
                    "pharmacyId",
                    "PharmacyId",
                    0
                )
            )

        );


    const warehouseName =
        getValue(
            transfer,
            "location",
            "Location",

            getValue(
                transfer,
                "warehouseName",
                "WarehouseName",
                "Main Warehouse — Rusayl"
            )
        );


    const pharmacyName =
        getValue(
            transfer,
            "pharmacyName",
            "PharmacyName",
            `Pharmacy #${pharmacyID}`
        );


    const transferDetails =
        getValue(
            transfer,
            "transferDetails",
            "TransferDetails",
            []
        );


    let totalQuantity =
        0;


    if (
        Array.isArray(
            transferDetails
        )
    ) {

        totalQuantity =
            transferDetails.reduce(
                function (
                    total,
                    detail
                ) {

                    const quantity =
                        Number(

                            getValue(
                                detail,
                                "quantity",
                                "Quantity",
                                0
                            )

                        );


                    return (
                        total +
                        quantity
                    );

                },

                0

            );

    }


    roadContent.innerHTML = `

        <!-- FROM -->

        <div class="road-place">

            <span>
                LEFT
            </span>

            <strong>
                ${warehouseName}
            </strong>

        </div>


        <!-- ROAD -->

        <div class="road">

            <div class="road-line">
            </div>


            <div class="truck">

                <i class="bi bi-truck"></i>

            </div>

        </div>


        <!-- TO -->

        <div
            class="road-place
                   road-place-right">

            <span>
                HEADING TO
            </span>

            <strong>
                ${pharmacyName}
            </strong>

            <small>

                #${transferId}

                ${
                    totalQuantity > 0
                        ?
                        ` · ${totalQuantity} units`
                        :
                        ""
                }

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
            function (transfer) {

                const status =
                    getValue(
                        transfer,
                        "status",
                        "Status",
                        ""
                    );


                const cleanStatus =
                    String(status)
                        .toLowerCase();


                return (
                    cleanStatus === "pending"
                    ||
                    cleanStatus === "shipped"
                    ||
                    cleanStatus === "in transit"
                );

            }
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
        function (transfer) {

            const transferId =
                Number(

                    getValue(
                        transfer,
                        "transferId",
                        "TransferId",

                        getValue(
                            transfer,
                            "transferID",
                            "TransferID",
                            0
                        )
                    )

                );


            const warehouseName =
                getValue(
                    transfer,
                    "location",
                    "Location",

                    getValue(
                        transfer,
                        "warehouseName",
                        "WarehouseName",
                        "Main Warehouse — Rusayl"
                    )
                );


            const transferDetails =
                getValue(
                    transfer,
                    "transferDetails",
                    "TransferDetails",
                    []
                );


            const medicineNames =
                Array.isArray(
                    transferDetails
                )
                    ?
                    transferDetails.map(
                        function (detail) {

                            return (

                                getValue(
                                    detail,
                                    "medicineName",
                                    "MedicineName",
                                    null
                                )

                                ??

                                `Medicine #${
                                    getValue(
                                        detail,
                                        "medicineID",
                                        "MedicineID",
                                        ""
                                    )
                                }`

                            );

                        }
                    )
                    :
                    [];


            incomingTransfers.innerHTML += `

                <div
                    class="incoming-transfer-row">


                    <div>

                        <span class="small-label">
                            TRANSFER
                        </span>

                        <strong>
                            #${transferId}
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
                                    medicineNames.join(
                                        ", "
                                    )
                                    :
                                    "-"
                            }

                        </strong>

                    </div>


                    <div>

                        <span
                            class="status shipped">

                            <span
                                class="status-dot">
                            </span>

                            ON THE ROAD

                        </span>

                    </div>


                    <div>

                        <button
                            class="confirm-btn"
                            onclick="receiveTransfer(${transferId})">

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
// PUT /api/Transfer/{id}/confirm-receive
// ==========================================

async function receiveTransfer(
    id
) {

    try {

        await Api.put(

            `/Transfer/${id}/confirm-receive`,

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
// MAKE RECEIVE FUNCTION AVAILABLE
// ==========================================

window.receiveTransfer =
    receiveTransfer;



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

if (pharmacistOrder) {

    pharmacistOrder
        .addEventListener(

            "change",

            handleOrderChange

        );

}


if (transferForm) {

    transferForm
        .addEventListener(

            "submit",

            createTransfer

        );

}



// ==========================================
// FIRST PAGE LOAD
// ==========================================

loadApprovedOrders();

loadTransfers();