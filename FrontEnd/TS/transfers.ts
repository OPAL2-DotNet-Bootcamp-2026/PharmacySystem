// ==========================================
// TYPES
// ==========================================

type ApiObject = Record<string, unknown>;

interface TransferDetailRequest {
    medicineID: number;
    quantity: number;
}

interface CreateTransferRequest {
    warehouseID: number;
    pharmacyID: number;
    pharmacistOrderId: number;
    transferDetails: TransferDetailRequest[];
}

// ==========================================
// HTML ELEMENTS
// ==========================================

const pharmacistOrder = document.querySelector<HTMLSelectElement>("#pharmacistOrder");
const transferForm = document.querySelector<HTMLFormElement>("#transferForm");
const transferMedicinesBody = document.querySelector<HTMLTableSectionElement>("#transferMedicinesBody");
const transferTable = document.querySelector<HTMLTableSectionElement>("#transferTableBody");
const transferCount = document.querySelector<HTMLElement>("#transferCount");
const roadSummary = document.querySelector<HTMLElement>("#roadSummary");
const roadContent = document.querySelector<HTMLElement>("#roadContent");
const incomingTransferCount = document.querySelector<HTMLElement>("#incomingTransferCount");
const incomingTransfers = document.querySelector<HTMLElement>("#incomingTransfers");

let approvedOrders: ApiObject[] = [];

// ==========================================
// HELPERS
// ==========================================

function isApiObject(value: unknown): value is ApiObject {
    return typeof value === "object" && value !== null;
}

function getValue(
    item: unknown,
    camelCase: string,
    pascalCase: string,
    fallback: unknown = null
): unknown {
    if (!isApiObject(item)) return fallback;
    return item[camelCase] ?? item[pascalCase] ?? fallback;
}

// ==========================================
// LOAD APPROVED ORDERS
// ==========================================

async function loadApprovedOrders(): Promise<void> {
    if (!pharmacistOrder) return;

    try {
        const orders: unknown = await Api.get("/PharmacistOrder");
        console.log("PHARMACIST ORDERS:", orders);

        const orderList: ApiObject[] = Array.isArray(orders)
            ? orders.filter(isApiObject)
            : [];

        approvedOrders = orderList.filter((order: ApiObject) => {
            const status = getValue(order, "status", "Status", "");
            return String(status).toLowerCase() === "approved";
        });

        pharmacistOrder.innerHTML = `
            <option value="" selected disabled>Select approved order</option>
        `;

        if (approvedOrders.length === 0) {
            pharmacistOrder.innerHTML += `
                <option value="" disabled>No approved orders found</option>
            `;
            return;
        }

        approvedOrders.forEach((order: ApiObject) => {
            const orderId = Number(
                getValue(order, "pharmacistOrderId", "PharmacistOrderId", 0)
            );

            const pharmacyName = getValue(
                order,
                "pharmacyName",
                "PharmacyName",
                ""
            );

            pharmacistOrder.innerHTML += `
                <option value="${orderId}">
                    Order #${orderId}${pharmacyName ? ` - ${String(pharmacyName)}` : ""}
                </option>
            `;
        });

    } catch (error: unknown) {
        console.error("Failed to load approved orders:", error);

        pharmacistOrder.innerHTML = `
            <option value="" selected disabled>
                Could not load approved orders
            </option>
        `;
    }
}

// ==========================================
// ORDER CHANGE
// ==========================================

function handleOrderChange(): void {
    if (!pharmacistOrder) return;

    const selectedId = Number(pharmacistOrder.value);

    const selectedOrder = approvedOrders.find((order: ApiObject) => {
        const orderId = Number(
            getValue(order, "pharmacistOrderId", "PharmacistOrderId", 0)
        );

        return orderId === selectedId;
    });

    if (!selectedOrder) return;

    renderOrderMedicines(selectedOrder);
}

// ==========================================
// RENDER ORDER MEDICINES
// ==========================================

function renderOrderMedicines(order: ApiObject): void {
    if (!transferMedicinesBody) return;

    transferMedicinesBody.innerHTML = "";

    const rawDetails =
        getValue(order, "orderDetails", "OrderDetails", null) ??
        getValue(order, "pharmacistOrderDetails", "PharmacistOrderDetails", []);

    const details: ApiObject[] = Array.isArray(rawDetails)
        ? rawDetails.filter(isApiObject)
        : [];

    if (details.length === 0) {
        transferMedicinesBody.innerHTML = `
            <tr>
                <td colspan="3" class="text-center text-muted">
                    No medicines found.
                </td>
            </tr>
        `;
        return;
    }

    details.forEach((detail: ApiObject) => {
        const medicineName = getValue(
            detail,
            "medicineName",
            "MedicineName",
            null
        );

        const medicineID = Number(
            getValue(
                detail,
                "medicineID",
                "MedicineID",
                getValue(detail, "medicineId", "MedicineId", 0)
            )
        );

        const quantity = Number(
            getValue(detail, "quantity", "Quantity", 0)
        );

        transferMedicinesBody.innerHTML += `
            <tr>
                <td>
                    <strong>${medicineName ?? `Medicine #${medicineID}`}</strong>
                </td>
                <td>-</td>
                <td class="text-end">${quantity}</td>
            </tr>
        `;
    });
}

// ==========================================
// CREATE TRANSFER
// ==========================================

async function createTransfer(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    if (!pharmacistOrder || !transferForm || !transferMedicinesBody) return;

    const pharmacistOrderId = Number(pharmacistOrder.value);

    if (!pharmacistOrderId) {
        alert("Please select an approved order.");
        return;
    }

    const selectedOrder = approvedOrders.find((order: ApiObject) => {
        const orderId = Number(
            getValue(order, "pharmacistOrderId", "PharmacistOrderId", 0)
        );

        return orderId === pharmacistOrderId;
    });

    if (!selectedOrder) {
        alert("Approved order not found.");
        return;
    }

    const pharmacyID = Number(
        getValue(
            selectedOrder,
            "pharmacyID",
            "PharmacyID",
            getValue(selectedOrder, "pharmacyId", "PharmacyId", 0)
        )
    );

    if (!pharmacyID) {
        console.log("SELECTED ORDER:", selectedOrder);
        alert("Pharmacy ID was not found in this order.");
        return;
    }

    const rawDetails =
        getValue(selectedOrder, "orderDetails", "OrderDetails", null) ??
        getValue(
            selectedOrder,
            "pharmacistOrderDetails",
            "PharmacistOrderDetails",
            []
        );

    const details: ApiObject[] = Array.isArray(rawDetails)
        ? rawDetails.filter(isApiObject)
        : [];

    if (details.length === 0) {
        alert("This order has no medicines.");
        return;
    }

    const transferDetails: TransferDetailRequest[] = details.map(
        (detail: ApiObject): TransferDetailRequest => {
            const medicineID = Number(
                getValue(
                    detail,
                    "medicineID",
                    "MedicineID",
                    getValue(detail, "medicineId", "MedicineId", 0)
                )
            );

            const quantity = Number(
                getValue(detail, "quantity", "Quantity", 0)
            );

            return { medicineID, quantity };
        }
    );

    const invalidDetail = transferDetails.find(
        (detail: TransferDetailRequest) =>
            detail.medicineID <= 0 || detail.quantity <= 0
    );

    if (invalidDetail) {
        console.log("INVALID DETAILS:", transferDetails);
        alert("One medicine has invalid data.");
        return;
    }

    const MAIN_WAREHOUSE_ID: number = 1;

    const newTransfer: CreateTransferRequest = {
        warehouseID: MAIN_WAREHOUSE_ID,
        pharmacyID,
        pharmacistOrderId,
        transferDetails
    };

    console.log("TRANSFER TO CREATE:", newTransfer);

    try {
        const result: unknown = await Api.post("/Transfer", newTransfer);

        console.log("TRANSFER CREATED:", result);
        alert("Transfer created successfully.");

        transferForm.reset();

        transferMedicinesBody.innerHTML = `
            <tr>
                <td colspan="3" class="text-center text-muted">
                    Select an approved order.
                </td>
            </tr>
        `;

        await loadApprovedOrders();
        await loadTransfers();

    } catch (error: unknown) {
        console.error("Failed to create transfer:", error);

        const message =
            error instanceof Error
                ? error.message
                : "Failed to create transfer.";

        alert(message);
    }
}

// ==========================================
// LOAD TRANSFERS
// ==========================================

async function loadTransfers(): Promise<void> {
    try {
        const transfers: unknown = await Api.get("/Transfer");

        console.log("TRANSFERS:", transfers);

        const transferList: ApiObject[] = Array.isArray(transfers)
            ? transfers.filter(isApiObject)
            : [];

        renderTransfers(transferList);
        renderRoadTransfers(transferList);
        renderIncomingTransfers(transferList);

    } catch (error: unknown) {
        console.error("Failed to load transfers:", error);

        if (transferTable) {
            transferTable.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center text-danger">
                        Failed to load transfers.
                    </td>
                </tr>
            `;
        }

        if (roadSummary) {
            roadSummary.textContent = "Could not load transfers.";
        }
    }
}

// ==========================================
// RENDER ALL TRANSFERS
// ==========================================

function renderTransfers(transfers: ApiObject[]): void {
    if (!transferTable) return;

    transferTable.innerHTML = "";

    if (transferCount) {
        transferCount.textContent = `${transfers.length} transfers`;
    }

    transfers.forEach((transfer: ApiObject) => {
        const transferId = Number(
            getValue(
                transfer,
                "transferId",
                "TransferId",
                getValue(transfer, "transferID", "TransferID", 0)
            )
        );

        const warehouseID = Number(
            getValue(
                transfer,
                "warehouseID",
                "WarehouseID",
                getValue(transfer, "warehouseId", "WarehouseId", 0)
            )
        );

        const pharmacyID = Number(
            getValue(
                transfer,
                "pharmacyID",
                "PharmacyID",
                getValue(transfer, "pharmacyId", "PharmacyId", 0)
            )
        );

        const warehouseName = getValue(
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

        const pharmacyName = getValue(
            transfer,
            "pharmacyName",
            "PharmacyName",
            `Pharmacy #${pharmacyID}`
        );

        const status = getValue(
            transfer,
            "status",
            "Status",
            "-"
        );

        const transferDate = getValue(
            transfer,
            "transferDate",
            "TransferDate",
            null
        );

        const receiveDate = getValue(
            transfer,
            "receiveDate",
            "ReceiveDate",
            null
        );

        const rawTransferDetails = getValue(
            transfer,
            "transferDetails",
            "TransferDetails",
            []
        );

        const transferDetails: ApiObject[] = Array.isArray(rawTransferDetails)
            ? rawTransferDetails.filter(isApiObject)
            : [];

        const medicineNames: string[] = transferDetails.map(
            (detail: ApiObject): string => {
                const name = getValue(
                    detail,
                    "medicineName",
                    "MedicineName",
                    null
                );

                const medicineID = getValue(
                    detail,
                    "medicineID",
                    "MedicineID",
                    ""
                );

                return name != null
                    ? String(name)
                    : `Medicine #${String(medicineID)}`;
            }
        );

        const contents =
            medicineNames.length > 0
                ? medicineNames.join(", ")
                : "-";

        let actionHtml = "";

        const cleanStatus = String(status).toLowerCase();

        if (
            cleanStatus === "shipped" ||
            cleanStatus === "in transit"
        ) {
            actionHtml = `
                <button
                    class="confirm-btn"
                    onclick="receiveTransfer(${transferId})">
                    Confirm receive
                </button>
            `;
        } else {
            actionHtml = `
                <span class="received-date">
                    ${
                        receiveDate
                            ? `Received ${formatDate(receiveDate)}`
                            : String(status)
                    }
                </span>
            `;
        }

        transferTable.innerHTML += `
            <tr data-id="${transferId}">
                <td><strong>#${transferId}</strong></td>
                <td>${String(warehouseName)}</td>
                <td>${String(pharmacyName)}</td>
                <td>${contents}</td>
                <td class="date-text">${formatDate(transferDate)}</td>

                <td>
                    <span class="status ${cleanStatus.replace(" ", "-")}">
                        <span class="status-dot"></span>
                        ${String(status).toUpperCase()}
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
                <td colspan="7" class="text-center text-muted">
                    No transfers found.
                </td>
            </tr>
        `;
    }
}

// ==========================================
// ON THE ROAD
// ==========================================

function renderRoadTransfers(transfers: ApiObject[]): void {
    if (!roadContent || !roadSummary) return;

    const roadTransfers = transfers.filter(
        (transfer: ApiObject): boolean => {
            const status = getValue(
                transfer,
                "status",
                "Status",
                ""
            );

            const cleanStatus = String(status).toLowerCase();

            return (
                cleanStatus === "pending" ||
                cleanStatus === "shipped" ||
                cleanStatus === "in transit"
            );
        }
    );

    roadSummary.textContent =
        `${roadTransfers.length} shipment(s) on the road`;

    roadContent.innerHTML = "";

    if (roadTransfers.length === 0) {
        roadContent.innerHTML = `
            <p class="text-muted mb-0">
                No transfers are currently on the road.
            </p>
        `;
        return;
    }

    const transfer = roadTransfers[0];

    const transferId = Number(
        getValue(
            transfer,
            "transferId",
            "TransferId",
            getValue(transfer, "transferID", "TransferID", 0)
        )
    );

    const pharmacyID = Number(
        getValue(
            transfer,
            "pharmacyID",
            "PharmacyID",
            getValue(transfer, "pharmacyId", "PharmacyId", 0)
        )
    );

    const warehouseName = getValue(
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

    const pharmacyName = getValue(
        transfer,
        "pharmacyName",
        "PharmacyName",
        `Pharmacy #${pharmacyID}`
    );

    const rawTransferDetails = getValue(
        transfer,
        "transferDetails",
        "TransferDetails",
        []
    );

    const transferDetails: ApiObject[] = Array.isArray(rawTransferDetails)
        ? rawTransferDetails.filter(isApiObject)
        : [];

    const totalQuantity = transferDetails.reduce(
        (total: number, detail: ApiObject): number => {
            const quantity = Number(
                getValue(detail, "quantity", "Quantity", 0)
            );

            return total + quantity;
        },
        0
    );

    roadContent.innerHTML = `
        <div class="road-place">
            <span>LEFT</span>
            <strong>${String(warehouseName)}</strong>
        </div>

        <div class="road">
            <div class="road-line"></div>

            <div class="truck">
                <i class="bi bi-truck"></i>
            </div>
        </div>

        <div class="road-place road-place-right">
            <span>HEADING TO</span>
            <strong>${String(pharmacyName)}</strong>

            <small>
                #${transferId}
                ${totalQuantity > 0 ? ` · ${totalQuantity} units` : ""}
            </small>
        </div>
    `;
}

// ==========================================
// INCOMING TRANSFERS
// ==========================================

function renderIncomingTransfers(transfers: ApiObject[]): void {
    if (!incomingTransfers) return;

    const incoming = transfers.filter(
        (transfer: ApiObject): boolean => {
            const status = getValue(
                transfer,
                "status",
                "Status",
                ""
            );

            const cleanStatus = String(status).toLowerCase();

            return (
                cleanStatus === "pending" ||
                cleanStatus === "shipped" ||
                cleanStatus === "in transit"
            );
        }
    );

    if (incomingTransferCount) {
        incomingTransferCount.textContent =
            `${incoming.length} incoming transfer(s)`;
    }

    incomingTransfers.innerHTML = "";

    incoming.forEach((transfer: ApiObject) => {
        const transferId = Number(
            getValue(
                transfer,
                "transferId",
                "TransferId",
                getValue(transfer, "transferID", "TransferID", 0)
            )
        );

        const warehouseName = getValue(
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

        const rawTransferDetails = getValue(
            transfer,
            "transferDetails",
            "TransferDetails",
            []
        );

        const transferDetails: ApiObject[] = Array.isArray(rawTransferDetails)
            ? rawTransferDetails.filter(isApiObject)
            : [];

        const medicineNames = transferDetails.map(
            (detail: ApiObject): string => {
                const medicineName = getValue(
                    detail,
                    "medicineName",
                    "MedicineName",
                    null
                );

                const medicineID = getValue(
                    detail,
                    "medicineID",
                    "MedicineID",
                    ""
                );

                return medicineName != null
                    ? String(medicineName)
                    : `Medicine #${String(medicineID)}`;
            }
        );

        incomingTransfers.innerHTML += `
            <div class="incoming-transfer-row">

                <div>
                    <span class="small-label">TRANSFER</span>
                    <strong>#${transferId}</strong>
                </div>

                <div>
                    <span class="small-label">FROM</span>
                    <strong>${String(warehouseName)}</strong>
                </div>

                <div>
                    <span class="small-label">CONTENTS</span>
                    <strong>
                        ${
                            medicineNames.length > 0
                                ? medicineNames.join(", ")
                                : "-"
                        }
                    </strong>
                </div>

                <div>
                    <span class="status shipped">
                        <span class="status-dot"></span>
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

async function receiveTransfer(id: number): Promise<void> {
    try {
        await Api.put(
            `/Transfer/${id}/confirm-receive`,
            {}
        );

        alert("Transfer received successfully.");

        await loadTransfers();

    } catch (error: unknown) {
        console.error("Failed to receive transfer:", error);

        const message =
            error instanceof Error
                ? error.message
                : "Failed to receive transfer.";

        alert(message);
    }
}

// Make receiveTransfer available for onclick
(
    window as Window &
        typeof globalThis & {
            receiveTransfer: (id: number) => Promise<void>;
        }
).receiveTransfer = receiveTransfer;

// ==========================================
// FORMAT DATE
// ==========================================

function formatDate(dateValue: unknown): string {
    if (!dateValue) return "-";

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

pharmacistOrder?.addEventListener(
    "change",
    handleOrderChange
);

transferForm?.addEventListener(
    "submit",
    (event: SubmitEvent) => {
        void createTransfer(event);
    }
);

// ==========================================
// FIRST LOAD
// ==========================================

void loadApprovedOrders();
void loadTransfers();