document.addEventListener("DOMContentLoaded", () => {

    // If user is already logged in
    if (Auth.isLoggedIn()) {
        window.location.replace(
            "dashboard.html#" + Auth.role().toLowerCase()
        );
        return;
    }

const form = document.querySelector(".loginbox");
const emailInput = document.getElementById("email");
const passwordInput = document.getElementById("password");
const button = document.querySelector(".signin-btn");
})