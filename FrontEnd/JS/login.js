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

// Create error message area
    let errorBox = document.getElementById("login-error");

    if (!errorBox) {
        errorBox = document.createElement("p");

        errorBox.id = "login-error";

        errorBox.style.cssText =
            "color:#d33;" +
            "margin:8px 0;" +
            "min-height:20px;" +
            "font-size:14px;";

        button.insertAdjacentElement(
            "beforebegin",
            errorBox
);
}

form.addEventListener("submit", async (e) => {

        // Stop the form from refreshing the page
        e.preventDefault();

        errorBox.textContent = "";



        const email = emailInput.value.trim();
        const password = passwordInput.value;



        // Check fields
        if (!email || !password) {
            errorBox.textContent =
                "Please enter email and password.";
            return;
        }


})

})