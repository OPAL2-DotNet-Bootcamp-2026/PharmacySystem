type ApiRecord = Record<string, unknown>;

interface ApiListWrapper {
    items?: unknown;
    data?: unknown;
    result?: unknown;
}

interface NormalisedSupply {
    date: string | number | Date | null;
    supplier: string;
    medicine: string;
    batchNumber: string;
    quantity: number;
    unitCost: number;
    warehouse: string;
}

interface CreateSupplyDto {
    supplierID: number;
    warehouseID: number;
    medicineID: number;
    batchNumber: string;
    quantity: number;
    expiryDate: string;
    unitCost: number;
}

document.addEventListener("DOMContentLoaded", (): void => {
    const ENDPOINTS = {
        suppliers: "/Supplier/GetAll",
        medicines: "/Medicine/GetAll",
        supplies: "/Supply",
        createSupply: "/Supply"
    } as const;

    if (!Auth.isLoggedIn()) {
        window.location.href = "login.html";
        return;
    }

    const role: string | null = Auth.role();
    const canCreateSupply: boolean = role === "Admin" || role === "Manager";

    function requiredElement<T extends HTMLElement>(id: string): T {
        const element = document.getElementById(id);
        if (!element) throw new Error(`Missing required element: #${id}`);
        return element as T;
    }

    const supplyForm = requiredElement<HTMLFormElement>("supplyForm");
    const supplierSelect = requiredElement<HTMLSelectElement>("supplier");
    const warehouseSelect = requiredElement<HTMLSelectElement>("warehouse");
    const medicineSelect = requiredElement<HTMLSelectElement>("medicine");
    const batchNumberInput = requiredElement<HTMLInputElement>("batchNumber");
    const quantityInput = requiredElement<HTMLInputElement>("quantity");
    const expiryDateInput = requiredElement<HTMLInputElement>("expiryDate");
    const unitCostInput = requiredElement<HTMLInputElement>("unitCost");
    const saveSupplyBtn = requiredElement<HTMLButtonElement>("saveSupplyBtn");
    const supplyHistoryBody = requiredElement<HTMLTableSectionElement>("supplyHistoryBody");
    const successMessage = requiredElement<HTMLElement>("supplySuccessMessage");
    const successText = requiredElement<HTMLElement>("supplySuccessText");

    if (!canCreateSupply) {
        supplyForm.style.display = "none";
        return;
    }

    function firstValue<T>(
        item: ApiRecord,
        keys: readonly string[],
        fallback: T
    ): unknown | T {
        for (const key of keys) {
            if (item[key] !== undefined && item[key] !== null) {
                return item[key];
            }
        }
        return fallback;
    }

    function textValue(value: unknown, fallback = ""): string {
        return value === undefined || value === null ? fallback : String(value);
    }

    function escapeHTML(value: unknown): string {
        const div: HTMLDivElement = document.createElement("div");
        div.textContent = textValue(value);
        return div.innerHTML;
    }

    function unwrapList(data: unknown): ApiRecord[] {
        if (Array.isArray(data)) {
            return data.filter(isApiRecord);
        }

        if (isApiRecord(data)) {
            const wrapper: ApiListWrapper = data;
            const possibleLists: unknown[] = [wrapper.items, wrapper.data, wrapper.result];

            for (const list of possibleLists) {
                if (Array.isArray(list)) {
                    return list.filter(isApiRecord);
                }
            }
        }

        return [];
    }

    function isApiRecord(value: unknown): value is ApiRecord {
        return typeof value === "object" && value !== null && !Array.isArray(value);
    }

    function fillSelect(
        select: HTMLSelectElement,
        items: unknown,
        idKeys: readonly string[],
        nameKeys: readonly string[],
        placeholder: string
    ): void {
        select.innerHTML = "";

        const placeholderOption: HTMLOptionElement = document.createElement("option");
        placeholderOption.value = "";
        placeholderOption.textContent = placeholder;
        placeholderOption.selected = true;
        placeholderOption.disabled = true;
        select.appendChild(placeholderOption);

        unwrapList(items).forEach((item: ApiRecord): void => {
            const id: string = textValue(firstValue(item, idKeys, ""));
            const name: string = textValue(firstValue(item, nameKeys, ""));

            if (id === "" || name === "") return;

            const option: HTMLOptionElement = document.createElement("option");
            option.value = id;
            option.textContent = name;
            select.appendChild(option);
        });
    }

    async function loadFormOptions(): Promise<void> {
        try {
            const [suppliers, medicines]: [unknown, unknown] = await Promise.all([
                Api.get(ENDPOINTS.suppliers),
                Api.get(ENDPOINTS.medicines)
            ]);

            fillSelect(
                supplierSelect,
                suppliers,
                ["supplierID", "SupplierID", "supplierId", "id"],
                ["fullName", "FullName", "supplierName", "SupplierName", "name"],
                "Select supplier"
            );

            fillSelect(
                medicineSelect,
                medicines,
                ["medicineID", "MedicineID", "medicineId", "id"],
                ["medicineName", "MedicineName", "name"],
                "Select medicine"
            );
        } catch (error: unknown) {
            console.error(error);
            alert(errorMessage(error, "Could not load supply form data."));
        }
    }

    function errorMessage(error: unknown, fallback: string): string {
        return error instanceof Error && error.message ? error.message : fallback;
    }

    function showTableMessage(message: string): void {
        supplyHistoryBody.innerHTML = `
            <tr>
                <td colspan="7" class="supply-table-message">
                    ${escapeHTML(message)}
                </td>
            </tr>`;
    }

    function normaliseSupply(item: ApiRecord): NormalisedSupply {
        const rawDate: unknown = firstValue(
            item,
            ["supplyDate", "SupplyDate", "createdAt", "CreatedAt", "date"],
            null
        );

        return {
            date: rawDate instanceof Date ||
                typeof rawDate === "string" ||
                typeof rawDate === "number" ? rawDate : null,
            supplier: textValue(firstValue(
                item,
                ["fullName", "FullName", "supplierName", "SupplierName", "supplierFullName", "SupplierFullName"],
                "—"
            )),
            medicine: textValue(firstValue(item, ["medicineName", "MedicineName"], "—")),
            batchNumber: textValue(firstValue(item, ["batchNumber", "BatchNumber"], "—")),
            quantity: Number(firstValue(item, ["quantity", "Quantity"], 0)),
            unitCost: Number(firstValue(item, ["unitCost", "UnitCost"], 0)),
            warehouse: textValue(firstValue(
                item,
                ["location", "Location", "warehouseName", "WarehouseName"],
                "Main Warehouse — Rusayl"
            ))
        };
    }

    function formatDate(value: string | number | Date | null): string {
        if (value === null || value === "") return "—";

        const date: Date = new Date(value);
        if (Number.isNaN(date.getTime())) return String(value);

        return new Intl.DateTimeFormat("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric"
        }).format(date);
    }

    function displaySupplies(data: unknown): void {
        const supplies: NormalisedSupply[] = unwrapList(data).map(normaliseSupply);

        if (supplies.length === 0) {
            showTableMessage("No supplies found.");
            return;
        }

        supplyHistoryBody.innerHTML = "";

        supplies.forEach((supply: NormalisedSupply): void => {
            const row: HTMLTableRowElement = document.createElement("tr");
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

    async function loadSupplies(): Promise<void> {
        showTableMessage("Loading supplies...");

        try {
            const data: unknown = await Api.get(ENDPOINTS.supplies);
            console.log("SUPPLIES:", data);
            displaySupplies(data);
        } catch (error: unknown) {
            console.error(error);
            showTableMessage(errorMessage(error, "Could not load supply history."));
        }
    }

    let successTimer: ReturnType<typeof setTimeout> | undefined;

    function showSuccessMessage(medicineName: string): void {
        successText.textContent = `${medicineName} was added to warehouse stock successfully.`;
        successMessage.classList.add("show");

        if (successTimer !== undefined) clearTimeout(successTimer);

        successTimer = setTimeout((): void => {
            successMessage.classList.remove("show");
        }, 3500);
    }

    supplyForm.addEventListener("submit", async (event: SubmitEvent): Promise<void> => {
        event.preventDefault();

        if (!supplyForm.checkValidity()) {
            supplyForm.reportValidity();
            return;
        }

        const quantity: number = Number(quantityInput.value);
        const unitCost: number = Number(unitCostInput.value);
        const expiryDate: Date = new Date(`${expiryDateInput.value}T00:00:00`);

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

        const body: CreateSupplyDto = {
            supplierID: Number(supplierSelect.value),
            warehouseID: Number(warehouseSelect.value),
            medicineID: Number(medicineSelect.value),
            batchNumber: batchNumberInput.value.trim(),
            quantity,
            expiryDate: expiryDateInput.value,
            unitCost
        };

        console.log("SUPPLY BODY:", body);

        const selectedMedicine: string =
            medicineSelect.options[medicineSelect.selectedIndex]?.text ?? "Medicine";
        const originalButtonHTML: string = saveSupplyBtn.innerHTML;

        saveSupplyBtn.disabled = true;
        saveSupplyBtn.innerHTML = `
            <span class="spinner-border spinner-border-sm me-2"></span>
            Saving...`;

        try {
            await Api.post<unknown, CreateSupplyDto>(ENDPOINTS.createSupply, body);
            supplyForm.reset();
            warehouseSelect.value = "1";
            await loadSupplies();
            showSuccessMessage(selectedMedicine);
        } catch (error: unknown) {
            console.error(error);
            alert(errorMessage(error, "Could not save the supply."));
        } finally {
            saveSupplyBtn.disabled = false;
            saveSupplyBtn.innerHTML = originalButtonHTML;
        }
    });

    void Promise.all([loadFormOptions(), loadSupplies()]);
});
