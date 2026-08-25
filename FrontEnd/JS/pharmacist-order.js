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
         

    


})