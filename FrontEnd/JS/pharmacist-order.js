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



         

    


})