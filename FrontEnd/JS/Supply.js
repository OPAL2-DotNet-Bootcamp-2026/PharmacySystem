document.addEventListener(
    "DOMContentLoaded",
    function () {


        // ==========================================
        // API ENDPOINTS
        // ==========================================

        const ENDPOINTS = {

            suppliers:
                "/Supplier/GetAll",

            medicines:
                "/Medicine/GetAll",

            supplies:
                "/Supply",

            createSupply:
                "/Supply"

        };



        // ==========================================
        // LOGIN CHECK
        // ==========================================

        if (!Auth.isLoggedIn()) {

            window.location.href =
                "login.html";

            return;

        }



        // ==========================================
        // ROLE
        // ==========================================

        const role =
            Auth.role();


        const canCreateSupply =

            role === "Admin" ||
            role === "Manager";



        // ==========================================
        // HTML ELEMENTS
        // ==========================================

        const supplyForm =
            document.getElementById(
                "supplyForm"
            );


        const supplierSelect =
            document.getElementById(
                "supplier"
            );


        const warehouseSelect =
            document.getElementById(
                "warehouse"
            );


        const medicineSelect =
            document.getElementById(
                "medicine"
            );


        const batchNumberInput =
            document.getElementById(
                "batchNumber"
            );


        const quantityInput =
            document.getElementById(
                "quantity"
            );


        const expiryDateInput =
            document.getElementById(
                "expiryDate"
            );


        const unitCostInput =
            document.getElementById(
                "unitCost"
            );


        const saveSupplyBtn =
            document.getElementById(
                "saveSupplyBtn"
            );


        const supplyHistoryBody =
            document.getElementById(
                "supplyHistoryBody"
            );


        const successMessage =
            document.getElementById(
                "supplySuccessMessage"
            );


        const successText =
            document.getElementById(
                "supplySuccessText"
            );



        if (!supplyForm) {

            return;

        }



        // ==========================================
        // ONLY ADMIN / MANAGER
        // ==========================================

        if (!canCreateSupply) {

            supplyForm.style.display =
                "none";

            return;

        }



        // ==========================================
        // HELPER: GET FIRST VALUE
        // ==========================================

        function firstValue(
            item,
            keys,
            fallback = ""
        ) {

            for (const key of keys) {

                if (
                    item?.[key] !== undefined &&
                    item?.[key] !== null
                ) {

                    return item[key];

                }

            }


            return fallback;

        }



        // ==========================================
        // ESCAPE HTML
        // ==========================================

        function escapeHTML(value) {

            const div =
                document.createElement(
                    "div"
                );


            div.textContent =
                value ?? "";


            return div.innerHTML;

        }



        // ==========================================
        // UNWRAP LIST
        // ==========================================

        function unwrapList(data) {

            if (Array.isArray(data)) {

                return data;

            }


            if (
                Array.isArray(
                    data?.items
                )
            ) {

                return data.items;

            }


            if (
                Array.isArray(
                    data?.data
                )
            ) {

                return data.data;

            }


            if (
                Array.isArray(
                    data?.result
                )
            ) {

                return data.result;

            }


            return [];

        }



        // ==========================================
        // FILL SELECT
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


            unwrapList(items)
                .forEach(
                    function (item) {


                        const id =
                            firstValue(
                                item,
                                idKeys
                            );


                        const name =
                            firstValue(
                                item,
                                nameKeys
                            );


                        if (
                            id === "" ||
                            name === ""
                        ) {

                            return;

                        }


                        const option =
                            document.createElement(
                                "option"
                            );


                        option.value =
                            id;


                        option.textContent =
                            name;


                        select.appendChild(
                            option
                        );

                    }
                );

        }



        // ==========================================
        // LOAD SUPPLIER + MEDICINE
        // ==========================================

        async function loadFormOptions() {

            try {


                const [
                    suppliers,
                    medicines
                ] =
                    await Promise.all([

                        Api.get(
                            ENDPOINTS.suppliers
                        ),

                        Api.get(
                            ENDPOINTS.medicines
                        )

                    ]);



                // Supplier dropdown

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



                // Medicine dropdown

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


            }
            catch (error) {


                console.error(
                    error
                );


                alert(

                    error.message ||

                    "Could not load supply form data."

                );

            }

        }



        // ==========================================
        // TABLE MESSAGE
        // ==========================================

        function showTableMessage(
            message
        ) {

            supplyHistoryBody.innerHTML = `

                <tr>

                    <td
                        colspan="7"
                        class="supply-table-message">

                        ${escapeHTML(
                            message
                        )}

                    </td>

                </tr>

            `;

        }



        // ==========================================
        // NORMALISE SUPPLY
        // ==========================================

        function normaliseSupply(
            item
        ) {

            return {


                // Date

                date:
                    firstValue(
                        item,
                        [
                            "supplyDate",
                            "SupplyDate",
                            "createdAt",
                            "CreatedAt",
                            "date"
                        ],
                        null
                    ),



                // ==================================
                // SUPPLIER NAME
                //
                // Backend returns:
                // FullName
                // ==================================

                supplier:
                    firstValue(
                        item,
                        [
                            "fullName",
                            "FullName",
                            "supplierName",
                            "SupplierName",
                            "supplierFullName",
                            "SupplierFullName"
                        ],
                        "—"
                    ),



                // ==================================
                // MEDICINE NAME
                // ==================================

                medicine:
                    firstValue(
                        item,
                        [
                            "medicineName",
                            "MedicineName"
                        ],
                        "—"
                    ),



                // Batch number

                batchNumber:
                    firstValue(
                        item,
                        [
                            "batchNumber",
                            "BatchNumber"
                        ],
                        "—"
                    ),



                // Quantity

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



                // Unit cost

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



                // ==================================
                // WAREHOUSE NAME
                //
                // Backend returns:
                // Location
                // ==================================

                warehouse:
                    firstValue(
                        item,
                        [
                            "location",
                            "Location",
                            "warehouseName",
                            "WarehouseName"
                        ],
                        "Main Warehouse — Rusayl"
                    )

            };

        }



        // ==========================================
        // FORMAT DATE
        // ==========================================

        function formatDate(
            value
        ) {

            if (!value) {

                return "—";

            }


            const date =
                new Date(
                    value
                );


            if (
                Number.isNaN(
                    date.getTime()
                )
            ) {

                return value;

            }


            return new Intl
                .DateTimeFormat(

                    "en-GB",

                    {

                        day:
                            "2-digit",

                        month:
                            "short",

                        year:
                            "numeric"

                    }

                )
                .format(
                    date
                );

        }



        // ==========================================
        // DISPLAY SUPPLIES
        // ==========================================

        function displaySupplies(
            data
        ) {

            const supplies =

                unwrapList(data)
                    .map(
                        normaliseSupply
                    );


            if (
                supplies.length === 0
            ) {

                showTableMessage(

                    "No supplies found."

                );

                return;

            }


            supplyHistoryBody.innerHTML =
                "";



            supplies.forEach(

                function (supply) {


                    const row =

                        document
                            .createElement(
                                "tr"
                            );


                    row.innerHTML = `


                        <!-- DATE -->

                        <td>

                            ${escapeHTML(
                                formatDate(
                                    supply.date
                                )
                            )}

                        </td>



                        <!-- SUPPLIER -->

                        <td>

                            ${escapeHTML(
                                supply.supplier
                            )}

                        </td>



                        <!-- MEDICINE -->

                        <td>

                            ${escapeHTML(
                                supply.medicine
                            )}

                        </td>



                        <!-- BATCH -->

                        <td>

                            ${escapeHTML(
                                supply.batchNumber
                            )}

                        </td>



                        <!-- QUANTITY -->

                        <td>

                            <strong>

                                ${escapeHTML(
                                    supply.quantity
                                )}

                            </strong>

                        </td>



                        <!-- UNIT COST -->

                        <td>

                            OMR ${escapeHTML(

                                supply.unitCost
                                    .toFixed(3)

                            )}

                        </td>



                        <!-- WAREHOUSE -->

                        <td>

                            ${escapeHTML(
                                supply.warehouse
                            )}

                        </td>

                    `;


                    supplyHistoryBody
                        .appendChild(
                            row
                        );

                }

            );

        }



        // ==========================================
        // LOAD SUPPLY HISTORY
        //
        // GET /api/Supply
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


                console.log(
                    "SUPPLIES:",
                    data
                );


                displaySupplies(
                    data
                );


            }
            catch (error) {


                console.error(
                    error
                );


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



            successMessage
                .classList
                .add(
                    "show"
                );


            clearTimeout(
                successTimer
            );


            successTimer =
                setTimeout(

                    function () {

                        successMessage
                            .classList
                            .remove(
                                "show"
                            );

                    },

                    3500

                );

        }



        // ==========================================
        // SAVE SUPPLY
        //
        // POST /api/Supply
        // ==========================================

        supplyForm
            .addEventListener(

                "submit",

                async function (
                    event
                ) {


                    event
                        .preventDefault();



                    // ==================================
                    // HTML VALIDATION
                    // ==================================

                    if (
                        !supplyForm
                            .checkValidity()
                    ) {

                        supplyForm
                            .reportValidity();

                        return;

                    }



                    // ==================================
                    // VALUES
                    // ==================================

                    const quantity =

                        Number(
                            quantityInput
                                .value
                        );


                    const unitCost =

                        Number(
                            unitCostInput
                                .value
                        );


                    const expiryDate =

                        new Date(

                            `${expiryDateInput.value}T00:00:00`

                        );



                    // ==================================
                    // QUANTITY VALIDATION
                    // ==================================

                    if (
                        quantity <= 0
                    ) {

                        alert(

                            "Quantity must be greater than zero."

                        );

                        return;

                    }



                    // ==================================
                    // UNIT COST VALIDATION
                    // ==================================

                    if (
                        unitCost < 0
                    ) {

                        alert(

                            "Unit cost cannot be negative."

                        );

                        return;

                    }



                    // ==================================
                    // EXPIRY VALIDATION
                    // ==================================

                    if (

                        Number.isNaN(
                            expiryDate
                                .getTime()
                        )

                        ||

                        expiryDate <=
                        new Date()

                    ) {

                        alert(

                            "Expiry date must be in the future."

                        );

                        return;

                    }



                    // ==================================
                    // CREATE BODY
                    //
                    // Matches CreateSupplyDto
                    // ==================================

                    const body = {


                        supplierID:

                            Number(
                                supplierSelect
                                    .value
                            ),


                        warehouseID:

                            Number(
                                warehouseSelect
                                    .value
                            ),


                        medicineID:

                            Number(
                                medicineSelect
                                    .value
                            ),


                        batchNumber:

                            batchNumberInput
                                .value
                                .trim(),


                        quantity:

                            quantity,


                        expiryDate:

                            expiryDateInput
                                .value,


                        unitCost:

                            unitCost

                    };



                    console.log(
                        "SUPPLY BODY:",
                        body
                    );



                    const selectedMedicine =

                        medicineSelect
                            .options[
                                medicineSelect
                                    .selectedIndex
                            ]
                            .text;



                    const originalButtonHTML =

                        saveSupplyBtn
                            .innerHTML;



                    saveSupplyBtn
                        .disabled =
                        true;



                    saveSupplyBtn
                        .innerHTML = `

                            <span
                                class="spinner-border spinner-border-sm me-2">
                            </span>

                            Saving...

                        `;



                    try {


                        // Save into database

                        await Api.post(

                            ENDPOINTS
                                .createSupply,

                            body

                        );



                        // Clear fields

                        supplyForm
                            .reset();



                        /*
                         * reset() clears the hidden
                         * warehouse value too only if
                         * dynamically changed.
                         *
                         * In your HTML it is value="1",
                         * so ensure it remains 1.
                         */

                        warehouseSelect
                            .value =
                            "1";



                        // Reload history

                        await loadSupplies();



                        // Success message

                        showSuccessMessage(

                            selectedMedicine

                        );


                    }
                    catch (error) {


                        console.error(
                            error
                        );


                        alert(

                            error.message ||

                            "Could not save the supply."

                        );


                    }
                    finally {


                        saveSupplyBtn
                            .disabled =
                            false;


                        saveSupplyBtn
                            .innerHTML =
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

    }
);