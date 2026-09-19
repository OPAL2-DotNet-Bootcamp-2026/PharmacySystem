"use strict";
document.addEventListener("DOMContentLoaded", () => {
    const ENDPOINTS = {
        suppliers: "/Supplier/GetAll",
        medicines: "/Medicine/GetAll",
        supplies: "/Supply",
        createSupply: "/Supply"
    };
    if (!Auth.isLoggedIn()) {
        window.location.href = "login.html";
        return;
    }
    const role = Auth.role();
    const canCreateSupply = role === "Admin" || role === "Manager";
    function requiredElement(id) {
        const element = document.getElementById(id);
        if (!element)
            throw new Error(`Missing required element: #${id}`);
        return element;
    }
    const supplyForm = requiredElement("supplyForm");
    const supplierSelect = requiredElement("supplier");
    const warehouseSelect = requiredElement("warehouse");
    const medicineSelect = requiredElement("medicine");
    const batchNumberInput = requiredElement("batchNumber");
    const quantityInput = requiredElement("quantity");
    const expiryDateInput = requiredElement("expiryDate");
    const unitCostInput = requiredElement("unitCost");
    const saveSupplyBtn = requiredElement("saveSupplyBtn");
    const supplyHistoryBody = requiredElement("supplyHistoryBody");
    const successMessage = requiredElement("supplySuccessMessage");
    const successText = requiredElement("supplySuccessText");
    if (!canCreateSupply) {
        supplyForm.style.display = "none";
        return;
    }
    function firstValue(item, keys, fallback) {
        for (const key of keys) {
            if (item[key] !== undefined && item[key] !== null) {
                return item[key];
            }
        }
        return fallback;
    }
    function textValue(value, fallback = "") {
        return value === undefined || value === null ? fallback : String(value);
    }
    function escapeHTML(value) {
        const div = document.createElement("div");
        div.textContent = textValue(value);
        return div.innerHTML;
    }
    function unwrapList(data) {
        if (Array.isArray(data)) {
            return data.filter(isApiRecord);
        }
        if (isApiRecord(data)) {
            const wrapper = data;
            const possibleLists = [wrapper.items, wrapper.data, wrapper.result];
            for (const list of possibleLists) {
                if (Array.isArray(list)) {
                    return list.filter(isApiRecord);
                }
            }
        }
        return [];
    }
    function isApiRecord(value) {
        return typeof value === "object" && value !== null && !Array.isArray(value);
    }
    function fillSelect(select, items, idKeys, nameKeys, placeholder) {
        select.innerHTML = "";
        const placeholderOption = document.createElement("option");
        placeholderOption.value = "";
        placeholderOption.textContent = placeholder;
        placeholderOption.selected = true;
        placeholderOption.disabled = true;
        select.appendChild(placeholderOption);
        unwrapList(items).forEach((item) => {
            const id = textValue(firstValue(item, idKeys, ""));
            const name = textValue(firstValue(item, nameKeys, ""));
            if (id === "" || name === "")
                return;
            const option = document.createElement("option");
            option.value = id;
            option.textContent = name;
            select.appendChild(option);
        });
    }
    async function loadFormOptions() {
        try {
            const [suppliers, medicines] = await Promise.all([
                Api.get(ENDPOINTS.suppliers),
                Api.get(ENDPOINTS.medicines)
            ]);
            fillSelect(supplierSelect, suppliers, ["supplierID", "SupplierID", "supplierId", "id"], ["fullName", "FullName", "supplierName", "SupplierName", "name"], "Select supplier");
            fillSelect(medicineSelect, medicines, ["medicineID", "MedicineID", "medicineId", "id"], ["medicineName", "MedicineName", "name"], "Select medicine");
        }
        catch (error) {
            console.error(error);
            alert(errorMessage(error, "Could not load supply form data."));
        }
    }
    function errorMessage(error, fallback) {
        return error instanceof Error && error.message ? error.message : fallback;
    }
    function showTableMessage(message) {
        supplyHistoryBody.innerHTML = `
            <tr>
                <td colspan="7" class="supply-table-message">
                    ${escapeHTML(message)}
                </td>
            </tr>`;
    }
    function normaliseSupply(item) {
        const rawDate = firstValue(item, ["supplyDate", "SupplyDate", "createdAt", "CreatedAt", "date"], null);
        return {
            date: rawDate instanceof Date ||
                typeof rawDate === "string" ||
                typeof rawDate === "number" ? rawDate : null,
            supplier: textValue(firstValue(item, ["fullName", "FullName", "supplierName", "SupplierName", "supplierFullName", "SupplierFullName"], "—")),
            medicine: textValue(firstValue(item, ["medicineName", "MedicineName"], "—")),
            batchNumber: textValue(firstValue(item, ["batchNumber", "BatchNumber"], "—")),
            quantity: Number(firstValue(item, ["quantity", "Quantity"], 0)),
            unitCost: Number(firstValue(item, ["unitCost", "UnitCost"], 0)),
            warehouse: textValue(firstValue(item, ["location", "Location", "warehouseName", "WarehouseName"], "Main Warehouse — Rusayl"))
        };
    }
    function formatDate(value) {
        if (value === null || value === "")
            return "—";
        const date = new Date(value);
        if (Number.isNaN(date.getTime()))
            return String(value);
        return new Intl.DateTimeFormat("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric"
        }).format(date);
    }
    function displaySupplies(data) {
        const supplies = unwrapList(data).map(normaliseSupply);
        if (supplies.length === 0) {
            showTableMessage("No supplies found.");
            return;
        }
        supplyHistoryBody.innerHTML = "";
        supplies.forEach((supply) => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${escapeHTML(formatDate(supply.date))}</td>
                <td>${escapeHTML(supply.supplier)}</td>
                <td>${escapeHTML(supply.medicine)}</td>
                <td>${escapeHTML(supply.batchNumber)}</td>
                <td><strong>${escapeHTML(supply.quantity)}</strong></td>
                <td>OMR ${escapeHTML(supply.unitCost.toFixed(3))}</td>
                <td>${escapeHTML(supply.warehouse)}</td>`;
            supplyHistoryBody.appendChild(row);
        });
    }
    async function loadSupplies() {
        showTableMessage("Loading supplies...");
        try {
            const data = await Api.get(ENDPOINTS.supplies);
            console.log("SUPPLIES:", data);
            displaySupplies(data);
        }
        catch (error) {
            console.error(error);
            showTableMessage(errorMessage(error, "Could not load supply history."));
        }
    }
    let successTimer;
    function showSuccessMessage(medicineName) {
        successText.textContent = `${medicineName} was added to warehouse stock successfully.`;
        successMessage.classList.add("show");
        if (successTimer !== undefined)
            clearTimeout(successTimer);
        successTimer = setTimeout(() => {
            successMessage.classList.remove("show");
        }, 3500);
    }
    supplyForm.addEventListener("submit", async (event) => {
        event.preventDefault();
        if (!supplyForm.checkValidity()) {
            supplyForm.reportValidity();
            return;
        }
        const quantity = Number(quantityInput.value);
        const unitCost = Number(unitCostInput.value);
        const expiryDate = new Date(`${expiryDateInput.value}T00:00:00`);
        if (quantity <= 0) {
            alert("Quantity must be greater than zero.");
            return;
        }
        if (unitCost < 0) {
            alert("Unit cost cannot be negative.");
            return;
        }
        if (Number.isNaN(expiryDate.getTime()) || expiryDate <= new Date()) {
            alert("Expiry date must be in the future.");
            return;
        }
        const body = {
            supplierID: Number(supplierSelect.value),
            warehouseID: Number(warehouseSelect.value),
            medicineID: Number(medicineSelect.value),
            batchNumber: batchNumberInput.value.trim(),
            quantity,
            expiryDate: expiryDateInput.value,
            unitCost
        };
        console.log("SUPPLY BODY:", body);
        const selectedMedicine = medicineSelect.options[medicineSelect.selectedIndex]?.text ?? "Medicine";
        const originalButtonHTML = saveSupplyBtn.innerHTML;
        saveSupplyBtn.disabled = true;
        saveSupplyBtn.innerHTML = `
            <span class="spinner-border spinner-border-sm me-2"></span>
            Saving...`;
        try {
            await Api.post(ENDPOINTS.createSupply, body);
            supplyForm.reset();
            warehouseSelect.value = "1";
            await loadSupplies();
            showSuccessMessage(selectedMedicine);
        }
        catch (error) {
            console.error(error);
            alert(errorMessage(error, "Could not save the supply."));
        }
        finally {
            saveSupplyBtn.disabled = false;
            saveSupplyBtn.innerHTML = originalButtonHTML;
        }
    });
    void Promise.all([loadFormOptions(), loadSupplies()]);
});
