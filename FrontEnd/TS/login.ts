(() => {
  interface LoginRequest {
    email: string;
    password: string;
  }

  interface LoginResponse extends AuthSession {}

  function requireElement<T extends Element>(selector: string): T {
    const element = document.querySelector<T>(selector);

    if (!element) {
      throw new Error(`Required login element is missing: ${selector}`);
    }

    return element;
  }

  function errorMessage(error: unknown): string {
    return error instanceof Error
      ? error.message
      : "Unable to sign in. Please try again.";
  }

  function isUserRole(value: string): value is UserRole {
    return value === "Admin" || value === "Manager" || value === "Pharmacist";
  }

  document.addEventListener("DOMContentLoaded", () => {
    const savedRole = Auth.role();

    if (Auth.isLoggedIn() && savedRole) {
      window.location.replace(`dashboard.html#${savedRole.toLowerCase()}`);
      return;
    }

    const form = requireElement<HTMLFormElement>(".loginbox");
    const emailInput = requireElement<HTMLInputElement>("#email");
    const passwordInput = requireElement<HTMLInputElement>("#password");
    const button = requireElement<HTMLButtonElement>(".signin-btn");
    const errorBox = requireElement<HTMLParagraphElement>("#login-error");

    form.addEventListener("submit", async (event: SubmitEvent) => {
      event.preventDefault();
      errorBox.textContent = "";

      if (!form.reportValidity()) {
        return;
      }

      const request: LoginRequest = {
        email: emailInput.value.trim(),
        password: passwordInput.value,
      };

      button.disabled = true;
      form.setAttribute("aria-busy", "true");
      const originalText = button.textContent;
      button.textContent = "Signing in...";

      try {
        const result = await Api.post<LoginResponse, LoginRequest>(
          "/User/login",
          request,
        );

        if (!isUserRole(result.role)) {
          throw new Error("The server returned an unsupported user role.");
        }

        Auth.save(result);
        window.location.href = `dashboard.html#${result.role.toLowerCase()}`;
      } catch (error: unknown) {
        errorBox.textContent = errorMessage(error);
      } finally {
        button.disabled = false;
        form.removeAttribute("aria-busy");
        button.textContent = originalText;
      }
    });
  });
})();
