document.addEventListener("DOMContentLoaded", async () => {


    // ==========================================
    // CHECK LOGIN
    // ==========================================

    if (!Auth.isLoggedIn()) {

        window.location.href = "login.html";

        return;
    }


})