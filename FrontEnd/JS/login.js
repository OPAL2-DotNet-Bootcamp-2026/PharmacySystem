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
        return value === "Admin" || value === "Manager" || value === "Pharmacist";
    }
    document.addEventListener("DOMContentLoaded", () => {
        const savedRole = Auth.role();
        if (Auth.isLoggedIn() && savedRole) {
            window.location.replace(`dashboard.html#${savedRole.toLowerCase()}`);
            return;
        }
        const form = requireElement(".loginbox");
        const emailInput = requireElement("#email");
        const passwordInput = requireElement("#password");
        const button = requireElement(".signin-btn");
        const errorBox = requireElement("#login-error");
        form.addEventListener("submit", async (event) => {
            event.preventDefault();
            errorBox.textContent = "";
            if (!form.reportValidity()) {
                return;
            }
            const request = {
                email: emailInput.value.trim(),
                password: passwordInput.value,
            };
            button.disabled = true;
            form.setAttribute("aria-busy", "true");
            const originalText = button.textContent;
            button.textContent = "Signing in...";
            try {
                const result = await Api.post("/User/login", request);
                if (!isUserRole(result.role)) {
                    throw new Error("The server returned an unsupported user role.");
                }
                Auth.save(result);
                window.location.href = `dashboard.html#${result.role.toLowerCase()}`;
            }
            catch (error) {
                errorBox.textContent = errorMessage(error);
            }
            finally {
                button.disabled = false;
                form.removeAttribute("aria-busy");
                button.textContent = originalText;
            }
        });
    });
})();
//# sourceMappingURL=login.js.map