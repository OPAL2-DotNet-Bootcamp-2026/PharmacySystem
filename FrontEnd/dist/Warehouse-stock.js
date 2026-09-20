"use strict";
document.addEventListener("DOMContentLoaded", () => {
    const MAIN_WAREHOUSE_ID = 1;
    const LOW_STOCK_LIMIT = 20;
    if (!Auth.isLoggedIn()) {
        window.location.href = "login.html";
        return;
    }
    function requiredElement(id) {
        const element = document.getElementById(id);
        if (!element)
            throw new Error(`Missing required element: #${id}`);
        return element;
    }
    const stockBody = requiredElement("warehouseStockBody");
    const totalElement = requiredElement("warehouseTotal");
    const linesElement = requiredElement("warehouseLines");
    function escapeHTML(value) {
        const div = document.createElement("div");
        div.textContent = value === null || value === undefined ? "" : String(value);
        return div.innerHTML;
    }
    function getValue(item, camelCase, pascalCase, fallback = "") {
        return item[camelCase] ?? item[pascalCase] ?? fallback;
    }
    function normaliseStock(item) {
        const rawExpiryDate = getValue(item, "expiryDate", "ExpiryDate", null);
        return {
            medicineName: String(getValue(item, "medicineName", "MedicineName", "Unknown medicine")),
            quantity: Number(getValue(item, "quantity", "Quantity", 0)),
            expiryDate: rawExpiryDate === null || rawExpiryDate === undefined
                ? null
                : String(rawExpiryDate)
        };
    }
    function formatDate(value) {
        if (!value)
            return "—";
        const date = new Date(`${value.slice(0, 10)}T00:00:00`);
        if (Number.isNaN(date.getTime()))
            return value;
        return new Intl.DateTimeFormat("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric"
        }).format(date);
    }
    function showMessage(message) {
        stockBody.innerHTML = `
            <tr>
                <td colspan="4" class="warehouse-message">
                    ${escapeHTML(message)}
                </td>
            </tr>`;
    }
    function displayStock(data) {
        const source = Array.isArray(data)
            ? data
            : [];
        const stocks = source.map(normaliseStock);
        const totalUnits = stocks.reduce((total, stock) => total + stock.quantity, 0);
        const maximumQuantity = Math.max(...stocks.map((stock) => stock.quantity), 1);
        totalElement.textContent = totalUnits.toLocaleString();
        linesElement.textContent = `${stocks.length} ${stocks.length === 1 ? "medicine" : "medicines"}`;
        if (stocks.length === 0) {
            showMessage("No medicines are currently stored in the main warehouse.");
            return;
        }
        stockBody.innerHTML = "";
        stocks.forEach((stock) => {
            const isLow = stock.quantity <= LOW_STOCK_LIMIT;
            const level = isLow ? "Low" : "Healthy";
            const levelClass = isLow ? "low" : "healthy";
            const fillClass = isLow ? "low-fill" : "healthy-fill";
            const barWidth = Math.max((stock.quantity / maximumQuantity) * 100, 4);
            const row = document.createElement("tr");
            row.innerHTML = `
                <td class="medicine-name">${escapeHTML(stock.medicineName)}</td>
                <td>
                    <div class="quantity-cell">
                        <strong>${escapeHTML(stock.quantity)}</strong>
                        <span class="quantity-track">
                            <span class="quantity-fill ${fillClass}" style="width: ${barWidth}%"></span>
                        </span>
                    </div>
                </td>
                <td class="expiry-date">${escapeHTML(formatDate(stock.expiryDate))}</td>
                <td class="text-end">
                    <span class="stock-level ${levelClass}">
                        <span class="level-dot"></span>${level}
                    </span>
                </td>`;
            stockBody.appendChild(row);
        });
    }
    function errorMessage(error, fallback) {
        return error instanceof Error && error.message ? error.message : fallback;
    }
    async function loadWarehouseStock() {
        showMessage("Loading warehouse stock...");
        try {
            const data = await Api.get(`/WarehouseStock/GetByWarehouse/${MAIN_WAREHOUSE_ID}`);
            displayStock(data);
        }
        catch (error) {
            console.error("Warehouse stock error:", error);
            totalElement.textContent = "0";
            linesElement.textContent = "0 medicines";
            showMessage(errorMessage(error, "Could not load warehouse stock."));
        }
    }
    void loadWarehouseStock();
});
