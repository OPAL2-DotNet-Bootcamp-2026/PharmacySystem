interface SupplierApiItem {
    supplierID?: number;
    SupplierID?: number;
    fullName?: string;
    FullName?: string;
    phone?: string;
    Phone?: string;
    email?: string;
    Email?: string;
    location?: string;
    Location?: string;
    isActive?: boolean;
    IsActive?: boolean;
}

interface Supplier {
    id: number;
    name: string;
    phone: string;
    email: string;
    location: string;
    isActive: boolean;
}

interface SaveSupplierDto {
    fullName: string;
    phone: string;
    email: string;
    location: string;
    isActive: boolean;
}

document.addEventListener("DOMContentLoaded", (): void => {
    if (!Auth.isLoggedIn()) {
        window.location.href = "login.html";
        return;
    }

    const role: UserRole | null = Auth.role();
    const canEdit: boolean = role === "Admin" || role === "Manager";
    const canDelete: boolean = role === "Admin";

    function requiredElement<T extends HTMLElement>(id: string): T {
        const element = document.getElementById(id);
        if (!element) throw new Error(`Missing required element: #${id}`);
        return element as T;
    }

    const addSupplierBtn = requiredElement<HTMLButtonElement>("addSupplierBtn");
    const supplierModal = requiredElement<HTMLElement>("supplierModal");
    const closeModalBtn = requiredElement<HTMLButtonElement>("closeModalBtn");
    const cancelSupplierBtn = requiredElement<HTMLButtonElement>("cancelSupplierBtn");
    const supplierForm = requiredElement<HTMLFormElement>("supplierForm");
    const supplierTableBody = requiredElement<HTMLTableSectionElement>("supplierTableBody");
    const supplierCount = requiredElement<HTMLElement>("supplierCount");
    const modalTitle = requiredElement<HTMLElement>("modalTitle");
    const modalDescription = requiredElement<HTMLElement>("modalDescription");
    const saveSupplierBtn = requiredElement<HTMLButtonElement>("saveSupplierBtn");
    const successMessage = requiredElement<HTMLElement>("successMessage");
    const successTitle = requiredElement<HTMLElement>("successTitle");
    const successText = requiredElement<HTMLElement>("successText");

    const supplierNameInput = requiredElement<HTMLInputElement>("supplierName");
    const supplierPhoneInput = requiredElement<HTMLInputElement>("supplierPhone");
    const supplierEmailInput = requiredElement<HTMLInputElement>("supplierEmail");
    const supplierLocationInput = requiredElement<HTMLInputElement>("supplierLocation");
    const supplierStatusSelect = requiredElement<HTMLSelectElement>("supplierStatus");

    let suppliers: Supplier[] = [];
    let editingSupplierId: number | null = null;
    let successTimer: ReturnType<typeof setTimeout> | undefined;

    if (!canEdit) addSupplierBtn.style.display = "none";

    function normalise(item: SupplierApiItem): Supplier {
        return {
            id: item.supplierID ?? item.SupplierID ?? 0,
            name: item.fullName ?? item.FullName ?? "",
            phone: item.phone ?? item.Phone ?? "",
            email: item.email ?? item.Email ?? "",
            location: item.location ?? item.Location ?? "",
            isActive: (item.isActive ?? item.IsActive) === true
        };
    }

    function errorMessage(error: unknown, fallback: string): string {
        return error instanceof Error && error.message ? error.message : fallback;
    }

    async function loadSuppliers(): Promise<void> {
        showTableMessage("Loading suppliers...");
        try {
            const data = await Api.get<SupplierApiItem[]>("/Supplier/GetAll");
            suppliers = (data ?? []).map(normalise);
            displaySuppliers();
        } catch (error: unknown) {
            console.error(error);
            showTableMessage(errorMessage(error, "Could not load suppliers."));
            supplierCount.textContent = "0 records";
        }
    }

    function showTableMessage(text: string): void {
        supplierTableBody.innerHTML = `
            <tr>
                <td colspan="5" class="no-suppliers">${escapeHTML(text)}</td>
            </tr>`;
    }

    function displaySuppliers(): void {
        supplierTableBody.innerHTML = "";
        supplierCount.textContent = `${suppliers.length} records`;

        if (suppliers.length === 0) {
            showTableMessage("No suppliers found.");
            return;
        }

        suppliers.forEach((supplier: Supplier): void => {
            const row = document.createElement("tr");
            const statusClass = supplier.isActive ? "active" : "inactive";
            const statusLabel = supplier.isActive ? "ACTIVE" : "INACTIVE";
            let actionsHTML = "";

            if (canEdit) {
                actionsHTML += `<button type="button" class="btn-edit-supplier" data-id="${supplier.id}">Edit</button>`;
            }
            if (canDelete) {
                actionsHTML += `<button type="button" class="btn-delete-supplier" data-id="${supplier.id}">Delete</button>`;
            }

            row.innerHTML = `
                <td>
                    <span class="supplier-name">${escapeHTML(supplier.name)}</span>
                    <span class="supplier-email">${escapeHTML(supplier.email)}</span>
                </td>
                <td>${escapeHTML(supplier.phone)}</td>
                <td>${escapeHTML(supplier.location)}</td>
                <td>
                    <span class="status ${statusClass}">
                        <span class="status-dot"></span>${statusLabel}
                    </span>
                </td>
                <td><div class="supplier-actions">${actionsHTML}</div></td>`;

            supplierTableBody.appendChild(row);
        });
    }

    addSupplierBtn.addEventListener("click", (): void => {
        editingSupplierId = null;
        modalTitle.textContent = "Add supplier";
        modalDescription.textContent = "Enter supplier information.";
        saveSupplierBtn.textContent = "Add supplier";
        supplierForm.reset();
        supplierStatusSelect.value = "Active";
        supplierModal.classList.add("show");
        setTimeout((): void => supplierNameInput.focus(), 100);
    });

    function closeSupplierModal(): void {
        supplierModal.classList.remove("show");
        supplierForm.reset();
        editingSupplierId = null;
    }

    cancelSupplierBtn.addEventListener("click", closeSupplierModal);
    closeModalBtn.addEventListener("click", closeSupplierModal);

    supplierModal.addEventListener("click", (event: MouseEvent): void => {
        if (event.target === supplierModal) closeSupplierModal();
    });

    document.addEventListener("keydown", (event: KeyboardEvent): void => {
        if (event.key === "Escape" && supplierModal.classList.contains("show")) {
            closeSupplierModal();
        }
    });

    supplierForm.addEventListener("submit", async (event: SubmitEvent): Promise<void> => {
        event.preventDefault();

        const name = supplierNameInput.value.trim();
        const phone = supplierPhoneInput.value.trim();
        const email = supplierEmailInput.value.trim();
        const location = supplierLocationInput.value.trim();
        const status = supplierStatusSelect.value;

        if (!name || !phone || !email || !location) {
            alert("Please fill in all supplier information.");
            return;
        }
        if (!/.+@.+\.com$/.test(email)) {
            alert("Email must contain @ and end with .com");
            return;
        }
        if (phone.length > 13) {
            alert("Phone number cannot be longer than 13 characters.");
            return;
        }

        const body: SaveSupplierDto = {
            fullName: name,
            phone,
            email,
            location,
            isActive: status === "Active"
        };

        const originalButtonText = saveSupplierBtn.textContent;
        saveSupplierBtn.disabled = true;
        saveSupplierBtn.textContent = "Saving...";

        try {
            if (editingSupplierId !== null) {
                await Api.put<unknown, SaveSupplierDto>(`/Supplier/Update/${editingSupplierId}`, body);
                closeSupplierModal();
                await loadSuppliers();
                showSuccessMessage("Supplier updated", `${name} was updated successfully.`);
                return;
            }

            await Api.post<unknown, SaveSupplierDto>("/Supplier/Create", body);
            closeSupplierModal();
            await loadSuppliers();
            showSuccessMessage("Supplier added", `${name} was added successfully.`);
        } catch (error: unknown) {
            console.error(error);
            alert(errorMessage(error, "Could not save the supplier."));
        } finally {
            saveSupplierBtn.disabled = false;
            saveSupplierBtn.textContent = originalButtonText;
        }
    });

    supplierTableBody.addEventListener("click", (event: MouseEvent): void => {
        const target = event.target;
        if (!(target instanceof HTMLElement)) return;

        const id = Number(target.dataset.id);
        if (target.classList.contains("btn-edit-supplier")) editSupplier(id);
        if (target.classList.contains("btn-delete-supplier")) void deleteSupplier(id);
    });

    function editSupplier(id: number): void {
        const supplier = suppliers.find((item: Supplier): boolean => item.id === id);
        if (!supplier) return;

        editingSupplierId = id;
        modalTitle.textContent = "Edit supplier";
        modalDescription.textContent = "Update supplier information.";
        saveSupplierBtn.textContent = "Save changes";
        supplierNameInput.value = supplier.name;
        supplierPhoneInput.value = supplier.phone;
        supplierEmailInput.value = supplier.email;
        supplierLocationInput.value = supplier.location;
        supplierStatusSelect.value = supplier.isActive ? "Active" : "Inactive";
        supplierModal.classList.add("show");
    }

    async function deleteSupplier(id: number): Promise<void> {
        const supplier = suppliers.find((item: Supplier): boolean => item.id === id);
        if (!supplier) return;

        if (!confirm(`Are you sure you want to delete ${supplier.name}?`)) return;

        try {
            await Api.del<unknown>(`/Supplier/Delete/${id}`);
            await loadSuppliers();
            showSuccessMessage("Supplier deleted", `${supplier.name} was deleted successfully.`);
        } catch (error: unknown) {
            console.error(error);
            alert(errorMessage(error, "Could not delete the supplier."));
        }
    }

    function showSuccessMessage(title: string, message: string): void {
        successTitle.textContent = title;
        successText.textContent = message;
        successMessage.classList.add("show");

        if (successTimer !== undefined) clearTimeout(successTimer);
        successTimer = setTimeout((): void => successMessage.classList.remove("show"), 3000);
    }

    function escapeHTML(value: unknown): string {
        const div = document.createElement("div");
        div.textContent = value === null || value === undefined ? "" : String(value);
        return div.innerHTML;
    }

    void loadSuppliers();
});
