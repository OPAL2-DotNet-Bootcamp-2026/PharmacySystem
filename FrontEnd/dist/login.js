"use strict";
(() => {
    function requireElement(selector) {
        const element = document.querySelector(selector);
        if (!element) {
            throw new Error(`Required login element is missing: ${selector}`);
        }
        return element;
    }
    function errorMessage(error) {
        return error instanceof Error
            ? error.message
            : "Unable to sign in. Please try again.";
    }
    function isUserRole(value) {
        return value === "Admin" ||
            value === "Manager" ||
            value === "Pharmacist";
    }
    document.addEventListener("DOMContentLoaded", () => {
        const savedRole = Auth.role();
        // CHECK IF ALREADY LOGGED IN
        if (Auth.isLoggedIn()) {
            const role = Auth.role();
            if (role) {
                window.location.replace("dashboard.html#" + role.toLowerCase());
            }
            return;
        }
        const form = document.querySelector(".loginbox");
        const emailInput = document.getElementById("email");
        const passwordInput = document.querySelector("#password");
        const button = document.querySelector(".signin-btn");
        // CREATE ERROR MESSAGE
        let errorBox = document.getElementById("login-error");
        if (!errorBox) {
            errorBox = document.createElement("p");
            errorBox.id = "login-error";
            errorBox.style.cssText =
                "color:#d33;" +
                    "margin:8px 0;" +
                    "min-height:20px;" +
                    "font-size:14px;";
            button.insertAdjacentElement("beforebegin", errorBox);
        }
        form.addEventListener("submit", async (event) => {
            event.preventDefault();
            errorBox.textContent = "";
            const email = emailInput.value.trim();
            const password = passwordInput.value;
            if (!email || !password) {
                errorBox.textContent =
                    "Please enter email and password.";
                return;
            }
            button.disabled = true;
            form.setAttribute("aria-busy", "true");
            const originalText = button.textContent;
            button.textContent = "Signing in...";
            try {
                const result = await Api.post("/User/login", {
                    email: email,
                    password: password
                });
                Auth.save({
                    token: result.token,
                    username: result.username,
                    role: result.role
                });
                window.location.href =
                    "dashboard.html#" +
                        result.role.toLowerCase();
            }
            catch (error) {
                if (error instanceof Error) {
                    errorBox.textContent =
                        error.message;
                }
            }
            finally {
                button.disabled = false;
                button.textContent =
                    originalText;
            }
        });
    });
})();
