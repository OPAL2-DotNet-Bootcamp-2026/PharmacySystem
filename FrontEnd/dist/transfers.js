"use strict";
// ==========================================
// TYPES
// ==========================================
// ==========================================
// HTML ELEMENTS
// ==========================================
const pharmacistOrder = document.querySelector("#pharmacistOrder");
const transferForm = document.querySelector("#transferForm");
const transferMedicinesBody = document.querySelector("#transferMedicinesBody");
const transferTable = document.querySelector("#transferTableBody");
const transferCount = document.querySelector("#transferCount");
const roadSummary = document.querySelector("#roadSummary");
const roadContent = document.querySelector("#roadContent");
const incomingTransferCount = document.querySelector("#incomingTransferCount");
const incomingTransfers = document.querySelector("#incomingTransfers");
// ==========================================
// STORE APPROVED ORDERS
// ==========================================
let approvedOrders = [];
// ==========================================
// HELPERS
// ==========================================
function isApiObject(value) {
    return (typeof value === "object" &&
        value !== null);
}
function getValue(item, camelCase, pascalCase, fallback = null) {
    if (!isApiObject(item)) {
        return fallback;
    }
    return (item[camelCase] ??
        item[pascalCase] ??
        fallback);
}
// ==========================================
// LOAD APPROVED ORDERS
// ==========================================
async function loadApprovedOrders() {
    if (!pharmacistOrder) {
        return;
    }
    try {
        const orders = await Api.get("/PharmacistOrder");
        console.log("PHARMACIST ORDERS:", orders);
        const orderList = Array.isArray(orders)
            ? orders.filter(isApiObject)
            : [];
        // ONLY APPROVED ORDERS
        approvedOrders =
            orderList.filter((order) => {
                const status = getValue(order, "status", "Status", "");
                return (String(status)
                    .toLowerCase()
                    === "approved");
            });
        pharmacistOrder.innerHTML = `
            <option
                value=""
                selected
                disabled>
                Select approved order
            </option>
        `;
        if (approvedOrders.length === 0) {
            pharmacistOrder.innerHTML += `
                <option
                    value=""
                    disabled>
                    No approved orders found
                </option>
            `;
            return;
        }
        approvedOrders.forEach((order) => {
            const orderId = Number(getValue(order, "pharmacistOrderId", "PharmacistOrderId", 0));
            const pharmacyName = getValue(order, "pharmacyName", "PharmacyName", "");
            pharmacistOrder.innerHTML += `
                    <option value="${orderId}">
                        Order #${orderId}
                        ${pharmacyName
                ? ` - ${String(pharmacyName)}`
                : ""}
                    </option>
                `;
        });
    }
    catch (error) {
        console.error("Failed to load approved orders:", error);
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
    if (!pharmacistOrder) {
        return;
    }
    const selectedId = Number(pharmacistOrder.value);
    const selectedOrder = approvedOrders.find((order) => {
        const orderId = Number(getValue(order, "pharmacistOrderId", "PharmacistOrderId", 0));
        return (orderId === selectedId);
    });
    if (!selectedOrder) {
        return;
    }
    renderOrderMedicines(selectedOrder);
}
// ==========================================
// RENDER ORDER MEDICINES
// ==========================================
function renderOrderMedicines(order) {
    if (!transferMedicinesBody) {
        return;
    }
    transferMedicinesBody.innerHTML =
        "";
    const rawDetails = getValue(order, "orderDetails", "OrderDetails", null)
        ??
            getValue(order, "pharmacistOrderDetails", "PharmacistOrderDetails", []);
    const details = Array.isArray(rawDetails)
        ? rawDetails.filter(isApiObject)
        : [];
    if (details.length === 0) {
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
    details.forEach((detail) => {
        const medicineName = getValue(detail, "medicineName", "MedicineName", null);
        const medicineID = Number(getValue(detail, "medicineID", "MedicineID", getValue(detail, "medicineId", "MedicineId", 0)));
        const quantity = Number(getValue(detail, "quantity", "Quantity", 0));
        transferMedicinesBody.innerHTML += `
                <tr>

                    <td>
                        <strong>
                            ${medicineName
            ??
                `Medicine #${medicineID}`}
                        </strong>
                    </td>

                    <td>-</td>

                    <td class="text-end">
                        ${quantity}
                    </td>

                </tr>
            `;
    });
}
// ==========================================
// CREATE TRANSFER
// ==========================================
async function createTransfer(event) {
    event.preventDefault();
    if (!pharmacistOrder ||
        !transferForm ||
        !transferMedicinesBody) {
        return;
    }
    const pharmacistOrderId = Number(pharmacistOrder.value);
    if (!pharmacistOrderId) {
        alert("Please select an approved order.");
        return;
    }
    const selectedOrder = approvedOrders.find((order) => {
        const orderId = Number(getValue(order, "pharmacistOrderId", "PharmacistOrderId", 0));
        return (orderId ===
            pharmacistOrderId);
    });
    if (!selectedOrder) {
        alert("Approved order not found.");
        return;
    }
    // ======================================
    // PHARMACY ID
    // ======================================
    const pharmacyID = Number(getValue(selectedOrder, "pharmacyID", "PharmacyID", getValue(selectedOrder, "pharmacyId", "PharmacyId", 0)));
    if (!pharmacyID) {
        console.log("SELECTED ORDER:", selectedOrder);
        alert("Pharmacy ID was not found in this order.");
        return;
    }
    // ======================================
    // ORDER DETAILS
    // ======================================
    const rawDetails = getValue(selectedOrder, "orderDetails", "OrderDetails", null)
        ??
            getValue(selectedOrder, "pharmacistOrderDetails", "PharmacistOrderDetails", []);
    const details = Array.isArray(rawDetails)
        ? rawDetails.filter(isApiObject)
        : [];
    if (details.length === 0) {
        alert("This order has no medicines.");
        return;
    }
    // ======================================
    // TRANSFER DETAILS
    // ======================================
    const transferDetails = details.map((detail) => {
        const medicineID = Number(getValue(detail, "medicineID", "MedicineID", getValue(detail, "medicineId", "MedicineId", 0)));
        const quantity = Number(getValue(detail, "quantity", "Quantity", 0));
        return {
            medicineID,
            quantity
        };
    });
    const invalidDetail = transferDetails.find((detail) => detail.medicineID <= 0
        ||
            detail.quantity <= 0);
    if (invalidDetail) {
        console.log("INVALID DETAILS:", transferDetails);
        alert("One medicine has invalid data.");
        return;
    }
    // ======================================
    // MAIN WAREHOUSE
    // ======================================
    const MAIN_WAREHOUSE_ID = 1;
    const newTransfer = {
        warehouseID: MAIN_WAREHOUSE_ID,
        pharmacyID,
        pharmacistOrderId,
        transferDetails
    };
    console.log("TRANSFER TO CREATE:", newTransfer);
    try {
        const result = await Api.post("/Transfer", newTransfer);
        console.log("TRANSFER CREATED:", result);
        alert("Transfer created successfully.");
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
        console.error("Failed to create transfer:", error);
        const message = error instanceof Error
            ? error.message
            : "Failed to create transfer.";
        alert(message);
    }
}
// ==========================================
// LOAD TRANSFERS
// ==========================================
async function loadTransfers() {
    try {
        const transfers = await Api.get("/Transfer");
        console.log("TRANSFERS:", transfers);
        const transferList = Array.isArray(transfers)
            ? transfers.filter(isApiObject)
            : [];
        renderTransfers(transferList);
        renderRoadTransfers(transferList);
        renderIncomingTransfers(transferList);
    }
    catch (error) {
        console.error("Failed to load transfers:", error);
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
        if (incomingTransferCount) {
            incomingTransferCount.textContent =
                "Could not load transfers.";
        }
    }
}
// ==========================================
// RENDER ADMIN / MANAGER TRANSFERS
// ==========================================
function renderTransfers(transfers) {
    if (!transferTable) {
        return;
    }
    transferTable.innerHTML =
        "";
    if (transferCount) {
        transferCount.textContent =
            `${transfers.length} transfers`;
    }
    transfers.forEach((transfer) => {
        // ======================================
        // TRANSFER ID
        // ======================================
        const transferId = Number(getValue(transfer, "transferId", "TransferId", getValue(transfer, "transferID", "TransferID", 0)));
        // ======================================
        // WAREHOUSE ID
        // ======================================
        const warehouseID = Number(getValue(transfer, "warehouseID", "WarehouseID", getValue(transfer, "warehouseId", "WarehouseId", 0)));
        // ======================================
        // PHARMACY ID
        // ======================================
        const pharmacyID = Number(getValue(transfer, "pharmacyID", "PharmacyID", getValue(transfer, "pharmacyId", "PharmacyId", 0)));
        // ======================================
        // WAREHOUSE NAME
        // ======================================
        const warehouseName = getValue(transfer, "location", "Location", getValue(transfer, "warehouseName", "WarehouseName", `Warehouse #${warehouseID}`));
        // ======================================
        // PHARMACY NAME
        // ======================================
        const pharmacyName = getValue(transfer, "pharmacyName", "PharmacyName", `Pharmacy #${pharmacyID}`);
        // ======================================
        // STATUS
        // ======================================
        const status = getValue(transfer, "status", "Status", "-");
        // ======================================
        // TRANSFER DATE
        // ======================================
        const transferDate = getValue(transfer, "transferDate", "TransferDate", null);
        // ======================================
        // RECEIVE DATE
        // ======================================
        const receiveDate = getValue(transfer, "receiveDate", "ReceiveDate", null);
        // ======================================
        // TRANSFER DETAILS
        // ======================================
        const rawTransferDetails = getValue(transfer, "transferDetails", "TransferDetails", []);
        const transferDetails = Array.isArray(rawTransferDetails)
            ? rawTransferDetails
                .filter(isApiObject)
            : [];
        // ======================================
        // MEDICINE NAMES
        // ======================================
        const medicineNames = transferDetails.map((detail) => {
            const name = getValue(detail, "medicineName", "MedicineName", null);
            const medicineID = getValue(detail, "medicineID", "MedicineID", "");
            return (name != null
                ? String(name)
                : `Medicine #${String(medicineID)}`);
        });
        const contents = medicineNames.length > 0
            ? medicineNames.join(", ")
            : "-";
        const cleanStatus = String(status)
            .toLowerCase();
        let actionHtml = "";
        // ======================================
        // PENDING
        // ADMIN / MANAGER SHIPS TRANSFER
        // ======================================
        if (cleanStatus === "pending") {
            actionHtml = `
                    <button
                        class="confirm-btn"
                        onclick="shipTransfer(${transferId})">
                        Mark as Shipped
                    </button>
                `;
        }
        // ======================================
        // SHIPPED
        // WAIT FOR PHARMACIST
        // ======================================
        else if (cleanStatus === "shipped") {
            actionHtml = `
                    <span
                        class="received-date">
                        On the road
                    </span>
                `;
        }
        // ======================================
        // RECEIVED / CANCELLED
        // ======================================
        else {
            actionHtml = `
                    <span
                        class="received-date">

                        ${receiveDate
                ? `Received ${formatDate(receiveDate)}`
                : String(status)}

                    </span>
                `;
        }
        // ======================================
        // ADD ROW
        // ======================================
        transferTable.innerHTML += `
                <tr data-id="${transferId}">

                    <td>
                        <strong>
                            #${transferId}
                        </strong>
                    </td>


                    <td>
                        ${String(warehouseName)}
                    </td>


                    <td>
                        ${String(pharmacyName)}
                    </td>


                    <td>
                        ${contents}
                    </td>


                    <td class="date-text">
                        ${formatDate(transferDate)}
                    </td>


                    <td>

                        <span
                            class="status ${cleanStatus.replace(" ", "-")}">

                            <span
                                class="status-dot">
                            </span>

                            ${String(status)
            .toUpperCase()}

                        </span>

                    </td>


                    <td class="text-end">
                        ${actionHtml}
                    </td>

                </tr>
            `;
    });
    if (transfers.length === 0) {
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
// MARK TRANSFER AS SHIPPED
// ==========================================
async function shipTransfer(id) {
    try {
        await Api.put(`/Transfer/${id}`, {
            status: "Shipped"
        });
        alert("Transfer marked as shipped.");
        await loadTransfers();
    }
    catch (error) {
        console.error("Failed to ship transfer:", error);
        const message = error instanceof Error
            ? error.message
            : "Failed to ship transfer.";
        alert(message);
    }
}
// ==========================================
// MAKE shipTransfer AVAILABLE TO HTML
// ==========================================
window.shipTransfer =
    shipTransfer;
// ==========================================
// ON THE ROAD
// ONLY SHIPPED TRANSFERS
// ==========================================
function renderRoadTransfers(transfers) {
    if (!roadContent ||
        !roadSummary) {
        return;
    }
    const roadTransfers = transfers.filter((transfer) => {
        const status = getValue(transfer, "status", "Status", "");
        const cleanStatus = String(status)
            .toLowerCase();
        return (cleanStatus ===
            "shipped");
    });
    roadSummary.textContent =
        `${roadTransfers.length} shipment(s) on the road`;
    roadContent.innerHTML =
        "";
    if (roadTransfers.length === 0) {
        roadContent.innerHTML = `
            <p class="text-muted mb-0">
                No transfers are currently on the road.
            </p>
        `;
        return;
    }
    // Show first shipped transfer
    const transfer = roadTransfers[0];
    const transferId = Number(getValue(transfer, "transferId", "TransferId", getValue(transfer, "transferID", "TransferID", 0)));
    const pharmacyID = Number(getValue(transfer, "pharmacyID", "PharmacyID", getValue(transfer, "pharmacyId", "PharmacyId", 0)));
    const warehouseName = getValue(transfer, "location", "Location", getValue(transfer, "warehouseName", "WarehouseName", "Main Warehouse — Rusayl"));
    const pharmacyName = getValue(transfer, "pharmacyName", "PharmacyName", `Pharmacy #${pharmacyID}`);
    const rawTransferDetails = getValue(transfer, "transferDetails", "TransferDetails", []);
    const transferDetails = Array.isArray(rawTransferDetails)
        ? rawTransferDetails
            .filter(isApiObject)
        : [];
    const totalQuantity = transferDetails.reduce((total, detail) => {
        const quantity = Number(getValue(detail, "quantity", "Quantity", 0));
        return (total + quantity);
    }, 0);
    roadContent.innerHTML = `
        <div class="road-place">

            <span>
                LEFT
            </span>

            <strong>
                ${String(warehouseName)}
            </strong>

        </div>


        <div class="road">

            <div class="road-line">
            </div>


            <div class="truck">

                <i class="bi bi-truck">
                </i>

            </div>

        </div>


        <div
            class="road-place
                   road-place-right">

            <span>
                HEADING TO
            </span>

            <strong>
                ${String(pharmacyName)}
            </strong>

            <small>

                #${transferId}

                ${totalQuantity > 0
        ? ` · ${totalQuantity} units`
        : ""}

            </small>

        </div>
    `;
}
// ==========================================
// INCOMING TRANSFERS
// PHARMACIST ONLY SEES SHIPPED
// ==========================================
function renderIncomingTransfers(transfers) {
    if (!incomingTransfers) {
        return;
    }
    const incoming = transfers.filter((transfer) => {
        const status = getValue(transfer, "status", "Status", "");
        const cleanStatus = String(status)
            .toLowerCase();
        return (cleanStatus ===
            "shipped");
    });
    if (incomingTransferCount) {
        incomingTransferCount.textContent =
            `${incoming.length} incoming transfer(s)`;
    }
    incomingTransfers.innerHTML =
        "";
    incoming.forEach((transfer) => {
        const transferId = Number(getValue(transfer, "transferId", "TransferId", getValue(transfer, "transferID", "TransferID", 0)));
        const warehouseName = getValue(transfer, "location", "Location", getValue(transfer, "warehouseName", "WarehouseName", "Main Warehouse — Rusayl"));
        const rawTransferDetails = getValue(transfer, "transferDetails", "TransferDetails", []);
        const transferDetails = Array.isArray(rawTransferDetails)
            ? rawTransferDetails
                .filter(isApiObject)
            : [];
        const medicineNames = transferDetails.map((detail) => {
            const medicineName = getValue(detail, "medicineName", "MedicineName", null);
            const medicineID = getValue(detail, "medicineID", "MedicineID", "");
            return (medicineName != null
                ? String(medicineName)
                : `Medicine #${String(medicineID)}`);
        });
        incomingTransfers.innerHTML += `
                <div
                    class="incoming-transfer-row">


                    <div>

                        <span
                            class="small-label">
                            TRANSFER
                        </span>

                        <strong>
                            #${transferId}
                        </strong>

                    </div>


                    <div>

                        <span
                            class="small-label">
                            FROM
                        </span>

                        <strong>
                            ${String(warehouseName)}
                        </strong>

                    </div>


                    <div>

                        <span
                            class="small-label">
                            CONTENTS
                        </span>

                        <strong>

                            ${medicineNames.length > 0
            ? medicineNames.join(", ")
            : "-"}

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
    });
    if (incoming.length === 0) {
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
async function receiveTransfer(id) {
    try {
        await Api.put(`/Transfer/${id}/confirm-receive`, {});
        alert("Transfer received successfully.");
        await loadTransfers();
    }
    catch (error) {
        console.error("Failed to receive transfer:", error);
        const message = error instanceof Error
            ? error.message
            : "Failed to receive transfer.";
        alert(message);
    }
}
// ==========================================
// MAKE receiveTransfer AVAILABLE TO HTML
// ==========================================
window.receiveTransfer =
    receiveTransfer;
// ==========================================
// FORMAT DATE
// ==========================================
function formatDate(dateValue) {
    if (!dateValue) {
        return "-";
    }
    const date = new Date(String(dateValue));
    if (Number.isNaN(date.getTime())) {
        return String(dateValue);
    }
    return date.toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric"
    });
}
// ==========================================
// EVENTS
// ==========================================
pharmacistOrder
    ?.addEventListener("change", handleOrderChange);
transferForm
    ?.addEventListener("submit", (event) => {
    void createTransfer(event);
});
// ==========================================
// FIRST LOAD
// ==========================================
void loadApprovedOrders();
void loadTransfers();
