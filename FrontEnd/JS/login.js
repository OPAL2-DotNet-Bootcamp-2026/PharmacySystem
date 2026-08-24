document.addEventListener("DOMContentLoaded", () => {

    // If user is already logged in
    if (Auth.isLoggedIn()) {
        window.location.replace(
            "dashboard.html#" + Auth.role().toLowerCase()
        );
        return;
    }


})