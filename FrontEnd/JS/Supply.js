document.addEventListener("DOMContentLoaded", function () {

    // ==========================================
    // API ENDPOINTS
    // Change these if your controller routes differ
    // ==========================================

    const ENDPOINTS = {

        suppliers: "/Supplier/GetAll",

        warehouses: "/Warehouse/GetAll",

        medicines: "/Medicine/GetAll",

        supplies: "/Supply/GetAll",

        createSupply: "/Supply/Create"

    };


    // ==========================================
    // LOGIN AND ROLE
    // ==========================================

    if (!Auth.isLoggedIn()) {

        window.location.href = "login.html";

        return;

    }


    const role = Auth.role();

    const canCreateSupply =
        role === "Admin" ||
        role === "Manager";


    // ==========================================
    // GET HTML ELEMENTS
    // ==========================================

    const supplyForm =
        document.getElementById("supplyForm");

    const supplierSelect =
        document.getElementById("supplier");

    const warehouseSelect =
        document.getElementById("warehouse");

    const medicineSelect =
        document.getElementById("medicine");

    const batchNumberInput =
        document.getElementById("batchNumber");

    const quantityInput =
        document.getElementById("quantity");

    const expiryDateInput =
        document.getElementById("expiryDate");

    const unitCostInput =
        document.getElementById("unitCost");

    const saveSupplyBtn =
        document.getElementById("saveSupplyBtn");

    const supplyHistoryBody =
        document.getElementById("supplyHistoryBody");

    const successMessage =
        document.getElementById("supplySuccessMessage");

    const successText =
        document.getElementById("supplySuccessText");


    if (!supplyForm) {

        return;

    }


    // Only Admin and Manager can create supplies

    if (!canCreateSupply) {

        supplyForm.style.display = "none";

        return;

    }


    // ==========================================
    // HELPER FUNCTIONS
    // ==========================================

    function firstValue(item, keys, fallback = "") {

        for (const key of keys) {

            if (
                item?.[key] !== undefined &&
                item[key] !== null
            ) {

                return item[key];

            }

        }

        return fallback;

    }


    function escapeHTML(value) {

        const div = document.createElement("div");

        div.textContent = value ?? "";

        return div.innerHTML;

    }


    function unwrapList(data) {

        if (Array.isArray(data)) {

            return data;

        }


        if (Array.isArray(data?.items)) {

            return data.items;

        }


        if (Array.isArray(data?.data)) {

            return data.data;

        }


        if (Array.isArray(data?.result)) {

            return data.result;

        }


        return [];

    }


    // ==========================================
    // ADD DATA TO SELECT
    // ==========================================

    function fillSelect(
        select,
        items,
        idKeys,
        nameKeys,
        placeholder
    ) {

        select.innerHTML = `

            <option
                value=""
                selected
                disabled>

                ${placeholder}

            </option>

        `;


        unwrapList(items).forEach(function (item) {

            const id =
                firstValue(item, idKeys);

            const name =
                firstValue(item, nameKeys);


            if (id === "" || name === "") {

                return;

            }


            const option =
                document.createElement("option");


            option.value = id;

            option.textContent = name;


            select.appendChild(option);

        });

    }


    // ==========================================
    // LOAD FORM DATA
    // ==========================================

    async function loadFormOptions() {

        try {

            const [
                suppliers,
                warehouses,
                medicines
            ] = await Promise.all([

                Api.get(ENDPOINTS.suppliers),

                Api.get(ENDPOINTS.warehouses),

                Api.get(ENDPOINTS.medicines)

            ]);


            fillSelect(

                supplierSelect,

                suppliers,

                [
                    "supplierID",
                    "SupplierID",
                    "supplierId",
                    "id"
                ],

                [
                    "fullName",
                    "FullName",
                    "supplierName",
                    "SupplierName",
                    "name"
                ],

                "Select supplier"

            );


            fillSelect(

                warehouseSelect,

                warehouses,

                [
                    "warehouseID",
                    "WarehouseID",
                    "warehouseId",
                    "id"
                ],

                [
                    "warehouseName",
                    "WarehouseName",
                    "name"
                ],

                "Select warehouse"

            );


            fillSelect(

                medicineSelect,

                medicines,

                [
                    "medicineID",
                    "MedicineID",
                    "medicineId",
                    "id"
                ],

                [
                    "medicineName",
                    "MedicineName",
                    "name"
                ],

                "Select medicine"

            );

        } catch (error) {

            console.error(error);


            alert(

                error.message ||
                "Could not load supply form data."

            );

        }

    }


    // ==========================================
    // TABLE MESSAGE
    // ==========================================

    function showTableMessage(message) {

        supplyHistoryBody.innerHTML = `

            <tr>

                <td
                    colspan="7"
                    class="supply-table-message">

                    ${escapeHTML(message)}

                </td>

            </tr>

        `;

    }


    // ==========================================
    // NORMALISE SUPPLY DATA
    // ==========================================

    function normaliseSupply(item) {

        return {

            date:
                firstValue(
                    item,
                    [
                        "createdAt",
                        "CreatedAt",
                        "supplyDate",
                        "SupplyDate",
                        "date"
                    ]
                ),

            supplier:
                firstValue(
                    item,
                    [
                        "supplierName",
                        "SupplierName",
                        "supplierFullName",
                        "SupplierFullName"
                    ],
                    "—"
                ),

            medicine:
                firstValue(
                    item,
                    [
                        "medicineName",
                        "MedicineName"
                    ],
                    "—"
                ),

            batchNumber:
                firstValue(
                    item,
                    [
                        "batchNumber",
                        "BatchNumber"
                    ],
                    "—"
                ),

            quantity:
                Number(
                    firstValue(
                        item,
                        [
                            "quantity",
                            "Quantity"
                        ],
                        0
                    )
                ),

            unitCost:
                Number(
                    firstValue(
                        item,
                        [
                            "unitCost",
                            "UnitCost"
                        ],
                        0
                    )
                ),

            warehouse:
                firstValue(
                    item,
                    [
                        "warehouseName",
                        "WarehouseName"
                    ],
                    "—"
                )

        };

    }


    // ==========================================
    // FORMAT DATE
    // ==========================================

    function formatDate(value) {

        if (!value) {

            return "—";

        }


        const date = new Date(value);


        if (Number.isNaN(date.getTime())) {

            return value;

        }


        return new Intl.DateTimeFormat(

            "en-GB",

            {
                day: "2-digit",
                month: "short",
                year: "numeric"
            }

        ).format(date);

    }


    // ==========================================
    // DISPLAY SUPPLIES
    // ==========================================

    function displaySupplies(data) {

        const supplies =
            unwrapList(data)
                .map(normaliseSupply);


        if (supplies.length === 0) {

            showTableMessage(
                "No supplies found."
            );

            return;

        }


        supplyHistoryBody.innerHTML = "";


        supplies.forEach(function (supply) {

            const row =
                document.createElement("tr");


            row.innerHTML = `

                <td>

                    ${escapeHTML(
                        formatDate(supply.date)
                    )}

                </td>


                <td>

                    ${escapeHTML(
                        supply.supplier
                    )}

                </td>


                <td>

                    ${escapeHTML(
                        supply.medicine
                    )}

                </td>


                <td>

                    ${escapeHTML(
                        supply.batchNumber
                    )}

                </td>


                <td>

                    <strong>

                        ${escapeHTML(
                            supply.quantity
                        )}

                    </strong>

                </td>


                <td>

                    OMR ${escapeHTML(
                        supply.unitCost.toFixed(3)
                    )}

                </td>


                <td>

                    ${escapeHTML(
                        supply.warehouse
                    )}

                </td>

            `;


            supplyHistoryBody.appendChild(row);

        });

    }


    // ==========================================
    // LOAD SUPPLY HISTORY
    // GET /api/Supply/GetAll
    // ==========================================

    async function loadSupplies() {

        showTableMessage(
            "Loading supplies..."
        );


        try {

            const data =
                await Api.get(
                    ENDPOINTS.supplies
                );


            displaySupplies(data);

        } catch (error) {

            console.error(error);


            showTableMessage(

                error.message ||
                "Could not load supply history."

            );

        }

    }


    // ==========================================
    // SUCCESS MESSAGE
    // ==========================================

    let successTimer;


    function showSuccessMessage(
        medicineName
    ) {

        successText.textContent =

            `${medicineName} was added to warehouse stock successfully.`;


        successMessage.classList.add(
            "show"
        );


        clearTimeout(successTimer);


        successTimer = setTimeout(

            function () {

                successMessage
                    .classList
                    .remove("show");

            },

            3500

        );

    }


    // ==========================================
    // SAVE SUPPLY
    // POST /api/Supply/Create
    // ==========================================

    supplyForm.addEventListener(

        "submit",

        async function (event) {

            event.preventDefault();


            if (!supplyForm.checkValidity()) {

                supplyForm.reportValidity();

                return;

            }


            const quantity =
                Number(quantityInput.value);


            const unitCost =
                Number(unitCostInput.value);


            const expiryDate =
                new Date(
                    `${expiryDateInput.value}T00:00:00`
                );


            if (quantity <= 0) {

                alert(
                    "Quantity must be greater than zero."
                );

                return;

            }


            if (unitCost < 0) {

                alert(
                    "Unit cost cannot be negative."
                );

                return;

            }


            if (
                Number.isNaN(
                    expiryDate.getTime()
                ) ||
                expiryDate <= new Date()
            ) {

                alert(
                    "Expiry date must be in the future."
                );

                return;

            }


            /*
                These properties must match
                your CreateSupplyDTO in C#
            */

            const body = {

                supplierID:
                    Number(
                        supplierSelect.value
                    ),

                warehouseID:
                    Number(
                        warehouseSelect.value
                    ),

                medicineID:
                    Number(
                        medicineSelect.value
                    ),

                batchNumber:
                    batchNumberInput
                        .value
                        .trim(),

                quantity:
                    quantity,

                expiryDate:
                    expiryDateInput.value,

                unitCost:
                    unitCost

            };


            const selectedMedicine =

                medicineSelect.options[
                    medicineSelect.selectedIndex
                ].text;


            const originalButtonHTML =
                saveSupplyBtn.innerHTML;


            saveSupplyBtn.disabled = true;


            saveSupplyBtn.innerHTML = `

                <span
                    class="spinner-border spinner-border-sm me-2">
                </span>

                Saving...

            `;


            try {

                await Api.post(

                    ENDPOINTS.createSupply,

                    body

                );


                // Clear the form

                supplyForm.reset();


                // Reload supply history

                await loadSupplies();


                // Show success message

                showSuccessMessage(
                    selectedMedicine
                );

            } catch (error) {

                console.error(error);


                alert(

                    error.message ||
                    "Could not save the supply."

                );

            } finally {

                saveSupplyBtn.disabled = false;

                saveSupplyBtn.innerHTML =
                    originalButtonHTML;

            }

        }

    );


    // ==========================================
    // PAGE LOAD
    // ==========================================

    Promise.all([

        loadFormOptions(),

        loadSupplies()

    ]);

});