document.addEventListener("DOMContentLoaded", async () => {


    // ==========================================
    // CHECK LOGIN
    // ==========================================

    if (!Auth.isLoggedIn()) {

        window.location.href = "login.html";

        return;
    }

    // ==========================================
    // ELEMENTS
    // ==========================================

    const pharmacySelect =
        document.getElementById("pharmacyId");

    const pharmacistSelect =
        document.getElementById("pharmacistId");

    const medicineSelect =
        document.getElementById("medicineId");

     const quantityInput =
        document.getElementById("quantity");

    const addMedicineButton =
        document.querySelector(".btn-add-medicine");

    const submitButton =
        document.querySelector(".btn-submit-order");

    const orderLines =
        document.getElementById("orderLines");

    const estimatedTotal =
        document.getElementById("estimatedTotal");

    
    // ==========================================
    // VARIABLES
    // ==========================================

    let medicines = [];

    let orderDetails = [];

    let currentPharmacist = null;

    // ==========================================
    // GET USER ID FROM TOKEN
    // ==========================================

    function getUserIdFromToken() {

        const token = Auth.token();


        if (!token) {

            return null;
        }
        try {

            const payloadPart =
                token.split(".")[1];


            const payload =
                JSON.parse(
                    atob(
                        payloadPart
                            .replace(/-/g, "+")
                            .replace(/_/g, "/")
                    )
                );

                return (
                payload[
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                ]

                ||

                payload.nameid

                ||

                payload.sub

                ||

                null
            );

        }
        catch (error) {

            console.error(
                "Could not read token:",
                error
            );

            return null;
        }
    }

    // ==========================================
    // LOAD PHARMACIES
    // ==========================================

    async function loadPharmacies() {

        try {

            const pharmacies =
                await Api.get(
                    "/Pharmacy"
                );


            pharmacySelect.innerHTML = `

                <option value="">
                    Select pharmacy
                </option>

            `;

            pharmacies.forEach(pharmacy => {

                if (!pharmacy.isActive) {

                    return;
                }


                pharmacySelect.innerHTML += `

                    <option
                        value="${pharmacy.pharmacyID}"
                    >
                        ${pharmacy.pharmacyName}
                    </option>

                `;

            });

        }

        catch (error) {

            console.error(
                "Failed to load pharmacies:",
                error
            );

        }

    }

     // ==========================================
    // LOAD MEDICINES
    // ==========================================

    async function loadMedicines() {

        try {

            medicines =
                await Api.get(
                    "/Medicine/GetAvailable"
                );

                medicineSelect.innerHTML = `

                <option value="">
                    Select medicine
                </option>

            `;

            medicines.forEach(medicine => {

                medicineSelect.innerHTML += `

                    <option
                        value="${medicine.medicineID}"
                    >
                        ${medicine.medicineName}
                    </option>

                `;

            });

        }

        catch (error) {

            console.error(
                "Failed to load medicines:",
                error
            );

        }

    }

     // ==========================================
    // FIND LOGGED-IN PHARMACIST
    // ==========================================

    async function loadCurrentPharmacist() {

        if (Auth.role() !== "Pharmacist") {

            return;
        }
         try {

            const userId =
                Number(
                    getUserIdFromToken()
                );


            const pharmacists =
                await Api.get(
                    "/Pharmacist"
                );

                currentPharmacist =
                pharmacists.find(
                    pharmacist =>
                        pharmacist.userID === userId
                );

                 if (!currentPharmacist) {

                throw new Error(
                    "Pharmacist profile was not found."
                );
            }

            // Select pharmacist's pharmacy automatically

            pharmacySelect.value =
                currentPharmacist.pharmacyID;


            // Pharmacist cannot choose another pharmacy

            pharmacySelect.disabled = true;

        }

        catch (error) {

            console.error(
                "Failed to find pharmacist:",
                error
            );

            alert(error.message);

        }

    }

     // ==========================================
    // LOAD PHARMACISTS BY PHARMACY
    // ADMIN ONLY
    // ==========================================

    async function loadPharmacistsByPharmacy() {

        if (Auth.role() !== "Admin") {

            return;
        }

        const pharmacyId =
            Number(
                pharmacySelect.value
            );

             pharmacistSelect.innerHTML = `

            <option value="">
                Select pharmacist
            </option>

        `;

         if (!pharmacyId) {

            return;
        }

          try {

            const pharmacists =
                await Api.get(
                    `/Pharmacist/by-pharmacy/${pharmacyId}`
                );

                pharmacists.forEach(pharmacist => {

                if (!pharmacist.isActive) {

                    return;
                }

                pharmacistSelect.innerHTML += `

                    <option
                        value="${pharmacist.pharmacistID}"
                    >
                        ${pharmacist.fullName}
                    </option>

                `;

            });

        }

         catch (error) {

            console.error(
                "Failed to load pharmacists:",
                error
            );

        }

    }

    // ==========================================
    // ADD MEDICINE
    // ==========================================

    function addMedicine() {

        const medicineId =
            Number(
                medicineSelect.value
            );

            const quantity =
            Number(
                quantityInput.value
            );

            // Check medicine

        if (!medicineId) {

            alert(
                "Please select a medicine."
            );

            return;
        }

        // Check quantity

        if (!quantity || quantity < 1) {

            alert(
                "Quantity must be greater than 0."
            );

            return;
        }


        // Find medicine information

        const medicine =
            medicines.find(
                item =>
                    item.medicineID === medicineId
            );

            if (!medicine) {

            alert(
                "Medicine was not found."
            );

            return;
        }

        // Check if medicine already exists in order

        const existingMedicine =
            orderDetails.find(
                item =>
                    item.medicineID === medicineId
            );

            if (existingMedicine) {

            existingMedicine.quantity +=
                quantity;

        }

         else {

            orderDetails.push({

                medicineID:
                    medicineId,

                medicineName:
                    medicine.medicineName,

                unitPrice:
                    medicine.unitPrice,

                quantity:
                    quantity

            });

        }

        // Refresh displayed medicines

        renderOrderDetails();


        // Reset

        medicineSelect.value = "";

        quantityInput.value = 1;

    }

    // ==========================================
    // RENDER ORDER DETAILS
    // ==========================================

    function renderOrderDetails() {

        // No medicines

        if (orderDetails.length === 0) {

            orderLines.innerHTML = `

                <h3>
                    No medicines added yet
                </h3>

                <p>
                    Pick a medicine and a quantity,
                    then add it to the order.
                </p>

            `;

             estimatedTotal.textContent =
                "OMR 0.000";


            return;
        }



        let html = "";

        let total = 0;

        orderDetails.forEach((detail, index) => {

            const subtotal =
                detail.unitPrice *
                detail.quantity;


            total += subtotal;


            html += `

                <div
                    class="d-flex
                           justify-content-between
                           align-items-center
                           border-bottom
                           py-3"
                >
                
                <div>

                        <strong>
                            ${detail.medicineName}
                        </strong>

                        <div class="text-muted">

                            Quantity:
                            ${detail.quantity}

                        </div>

                    </div>

                    <div class="d-flex align-items-center gap-3">

                        <strong>

                            OMR
                            ${subtotal.toFixed(3)}

                        </strong>

                        <button
                            type="button"
                            class="btn btn-sm btn-outline-danger"
                            data-remove-index="${index}"
                        >

                            Remove

                        </button>

                    </div>

                </div>

            `;

        });

         orderLines.innerHTML = html;


        estimatedTotal.textContent =
            `OMR ${total.toFixed(3)}`;

    }

    // ==========================================
    // REMOVE MEDICINE
    // ==========================================

    orderLines.addEventListener(
        "click",
        event => {

            const button =
                event.target.closest(
                    "[data-remove-index]"
                );

                if (!button) {

                return;
            }


            const index =
                Number(
                    button.dataset.removeIndex
                );


            orderDetails.splice(
                index,
                1
            );
            renderOrderDetails();

        }
    );

    // ==========================================
    // CREATE PHARMACIST ORDER
    // ==========================================

    async function createPharmacistOrder() {

        const pharmacyID =
            Number(
                pharmacySelect.value
            );
            let pharmacistID;

            // Logged-in Pharmacist

        if (Auth.role() === "Pharmacist") {

            if (!currentPharmacist) {

                alert(
                    "Pharmacist profile was not found."
                );

                return;
            }

            pharmacistID =
                currentPharmacist.pharmacistID;

        }




         

    


})