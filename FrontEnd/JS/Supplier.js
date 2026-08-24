// ==========================================
// SUPPLIERS PAGE
// ==========================================


// ==========================================
// GET HTML ELEMENTS
// ==========================================

const addSupplierBtn =
    document.getElementById("addSupplierBtn");

const supplierModal =
    document.getElementById("supplierModal");

const closeModalBtn =
    document.getElementById("closeModalBtn");

const cancelSupplierBtn =
    document.getElementById("cancelSupplierBtn");

const supplierForm =
    document.getElementById("supplierForm");

const supplierTableBody =
    document.getElementById("supplierTableBody");

const supplierCount =
    document.getElementById("supplierCount");

const modalTitle =
    document.getElementById("modalTitle");

const modalDescription =
    document.getElementById("modalDescription");

const saveSupplierBtn =
    document.getElementById("saveSupplierBtn");

const successMessage =
    document.getElementById("successMessage");

const successTitle =
    document.getElementById("successTitle");

const successText =
    document.getElementById("successText");



// ==========================================
// DEFAULT SUPPLIERS
// ==========================================

const defaultSuppliers = [

    {
        id: 1,
        name: "Gulf Pharma Distribution",
        email: "orders@gulfpharma.com",
        phone: "+96824561200",
        location: "Rusayl, Muscat",
        status: "Active"
    },


    {
        id: 2,
        name: "Muscat Medical Supplies",
        email: "sales@muscatmed.com",
        phone: "+96824778101",
        location: "Ghala, Muscat",
        status: "Active"
    },


    {
        id: 3,
        name: "Al Rawahi Pharma",
        email: "info@alrawahi.com",
        phone: "+96826443311",
        location: "Nizwa, Dakhiliyah",
        status: "Active"
    },


    {
        id: 4,
        name: "Oman Drug House",
        email: "contact@odh.com",
        phone: "+96824900744",
        location: "Sohar, Batinah",
        status: "Active"
    },


    {
        id: 5,
        name: "Delta Med Trading",
        email: "hello@deltamed.com",
        phone: "+96823336890",
        location: "Salalah, Dhofar",
        status: "Inactive"
    }

];



// ==========================================
// LOAD SUPPLIERS FROM LOCAL STORAGE
// ==========================================

let suppliers =
    JSON.parse(
        localStorage.getItem("pharmacySuppliers")
    );



// First time opening the page

if (!suppliers) {

    suppliers = defaultSuppliers;

    saveToLocalStorage();

}



// ==========================================
// EDIT MODE
// ==========================================

let editingSupplierId = null;



// ==========================================
// SAVE TO LOCAL STORAGE
// ==========================================

function saveToLocalStorage() {

    localStorage.setItem(
        "pharmacySuppliers",
        JSON.stringify(suppliers)
    );

}



// ==========================================
// DISPLAY SUPPLIERS
// ==========================================

function displaySuppliers() {

    supplierTableBody.innerHTML = "";


    // Update record count

    supplierCount.textContent =
        `${suppliers.length} records`;


    // No suppliers

    if (suppliers.length === 0) {

        supplierTableBody.innerHTML = `

            <tr>

                <td
                    colspan="5"
                    class="no-suppliers">

                    No suppliers found.

                </td>

            </tr>

        `;

        return;

    }



    // Create rows

    suppliers.forEach(function (supplier) {


        const row =
            document.createElement("tr");


        const statusClass =
            supplier.status === "Active"
                ? "active"
                : "inactive";


        row.innerHTML = `

            <!-- Supplier -->

            <td>

                <span class="supplier-name">

                    ${escapeHTML(supplier.name)}

                </span>


                <span class="supplier-email">

                    ${escapeHTML(supplier.email)}

                </span>

            </td>


            <!-- Phone -->

            <td>

                ${escapeHTML(supplier.phone)}

            </td>


            <!-- Location -->

            <td>

                ${escapeHTML(supplier.location)}

            </td>


            <!-- Status -->

            <td>

                <span class="status ${statusClass}">

                    <span class="status-dot"></span>

                    ${supplier.status.toUpperCase()}

                </span>

            </td>


            <!-- Actions -->

            <td>

                <div class="supplier-actions">


                    <button
                        type="button"
                        class="btn-edit-supplier"
                        data-id="${supplier.id}">

                        Edit

                    </button>


                    <button
                        type="button"
                        class="btn-delete-supplier"
                        data-id="${supplier.id}">

                        Delete

                    </button>


                </div>

            </td>

        `;


        supplierTableBody.appendChild(row);

    });

}



// ==========================================
// OPEN ADD SUPPLIER MODAL
// ==========================================

addSupplierBtn.addEventListener(
    "click",
    function () {

        editingSupplierId = null;


        modalTitle.textContent =
            "Add supplier";


        modalDescription.textContent =
            "Enter supplier information.";


        saveSupplierBtn.textContent =
            "Add supplier";


        supplierForm.reset();


        document.getElementById(
            "supplierStatus"
        ).value = "Active";


        supplierModal.classList.add("show");


        setTimeout(function () {

            document
                .getElementById("supplierName")
                .focus();

        }, 100);

    }
);



// ==========================================
// CLOSE MODAL
// ==========================================

function closeSupplierModal() {

    supplierModal.classList.remove("show");

    supplierForm.reset();

    editingSupplierId = null;

}



// Cancel button

cancelSupplierBtn.addEventListener(
    "click",
    closeSupplierModal
);



// X button

closeModalBtn.addEventListener(
    "click",
    closeSupplierModal
);



// Click outside modal

supplierModal.addEventListener(
    "click",
    function (event) {

        if (event.target === supplierModal) {

            closeSupplierModal();

        }

    }
);



// Escape keyboard

document.addEventListener(
    "keydown",
    function (event) {

        if (
            event.key === "Escape" &&
            supplierModal.classList.contains("show")
        ) {

            closeSupplierModal();

        }

    }
);



// ==========================================
// FORM SUBMIT
// ADD OR UPDATE
// ==========================================

supplierForm.addEventListener(
    "submit",
    function (event) {

        event.preventDefault();



        // Get form values

        const name =
            document
                .getElementById("supplierName")
                .value
                .trim();


        const phone =
            document
                .getElementById("supplierPhone")
                .value
                .trim();


        const email =
            document
                .getElementById("supplierEmail")
                .value
                .trim();


        const location =
            document
                .getElementById("supplierLocation")
                .value
                .trim();


        const status =
            document
                .getElementById("supplierStatus")
                .value;



        // Simple validation

        if (
            !name ||
            !phone ||
            !email ||
            !location
        ) {

            alert(
                "Please fill in all supplier information."
            );

            return;

        }



        // ==================================
        // EDIT EXISTING SUPPLIER
        // ==================================

        if (editingSupplierId !== null) {


            const supplier =
                suppliers.find(
                    function (item) {

                        return (
                            item.id ===
                            editingSupplierId
                        );

                    }
                );


            if (!supplier) {

                return;

            }


            supplier.name = name;

            supplier.phone = phone;

            supplier.email = email;

            supplier.location = location;

            supplier.status = status;


            saveToLocalStorage();

            displaySuppliers();

            closeSupplierModal();


            showSuccessMessage(
                "Supplier updated",
                `${name} was updated successfully.`
            );


            return;

        }



        // ==================================
        // ADD NEW SUPPLIER
        // ==================================


        const newSupplier = {

            id: Date.now(),

            name: name,

            phone: phone,

            email: email,

            location: location,

            status: status

        };


        // Add to array

        suppliers.push(newSupplier);


        // Save

        saveToLocalStorage();


        // Update table

        displaySuppliers();


        // Close modal

        closeSupplierModal();


        // Show success

        showSuccessMessage(
            "Supplier added",
            `${name} was added successfully.`
        );

    }
);



// ==========================================
// TABLE BUTTONS
// EDIT + DELETE
// ==========================================

supplierTableBody.addEventListener(
    "click",
    function (event) {


        // ==================================
        // EDIT
        // ==================================

        if (
            event.target.classList.contains(
                "btn-edit-supplier"
            )
        ) {


            const id =
                Number(
                    event.target.dataset.id
                );


            editSupplier(id);

        }



        // ==================================
        // DELETE
        // ==================================

        if (
            event.target.classList.contains(
                "btn-delete-supplier"
            )
        ) {


            const id =
                Number(
                    event.target.dataset.id
                );


            deleteSupplier(id);

        }

    }
);



// ==========================================
// EDIT SUPPLIER
// ==========================================

function editSupplier(id) {


    const supplier =
        suppliers.find(
            function (item) {

                return item.id === id;

            }
        );


    if (!supplier) {

        return;

    }



    editingSupplierId = id;



    // Change modal title

    modalTitle.textContent =
        "Edit supplier";


    modalDescription.textContent =
        "Update supplier information.";


    saveSupplierBtn.textContent =
        "Save changes";



    // Fill form

    document.getElementById(
        "supplierName"
    ).value = supplier.name;


    document.getElementById(
        "supplierPhone"
    ).value = supplier.phone;


    document.getElementById(
        "supplierEmail"
    ).value = supplier.email;


    document.getElementById(
        "supplierLocation"
    ).value = supplier.location;


    document.getElementById(
        "supplierStatus"
    ).value = supplier.status;



    // Open modal

    supplierModal.classList.add("show");

}



// ==========================================
// DELETE SUPPLIER
// ==========================================

function deleteSupplier(id) {


    const supplier =
        suppliers.find(
            function (item) {

                return item.id === id;

            }
        );


    if (!supplier) {

        return;

    }



    const confirmed =
        confirm(
            `Are you sure you want to delete ${supplier.name}?`
        );


    if (!confirmed) {

        return;

    }



    // Remove supplier

    suppliers =
        suppliers.filter(
            function (item) {

                return item.id !== id;

            }
        );


    // Save new list

    saveToLocalStorage();


    // Refresh table

    displaySuppliers();


    // Success

    showSuccessMessage(
        "Supplier deleted",
        `${supplier.name} was deleted successfully.`
    );

}



// ==========================================
// SUCCESS MESSAGE
// ==========================================

let successTimer;


function showSuccessMessage(
    title,
    message
) {


    // Change text

    successTitle.textContent =
        title;


    successText.textContent =
        message;



    // Show

    successMessage.classList.add(
        "show"
    );



    // Reset previous timer

    clearTimeout(successTimer);



    // Hide after 3 seconds

    successTimer =
        setTimeout(
            function () {

                successMessage
                    .classList
                    .remove("show");

            },
            3000
        );

}



// ==========================================
// ESCAPE HTML
// Prevent user input from becoming HTML
// ==========================================

function escapeHTML(value) {

    const div =
        document.createElement("div");


    div.textContent =
        value ?? "";


    return div.innerHTML;

}



// ==========================================
// FIRST PAGE LOAD
// ==========================================

displaySuppliers();