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


})