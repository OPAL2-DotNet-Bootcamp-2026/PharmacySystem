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


})