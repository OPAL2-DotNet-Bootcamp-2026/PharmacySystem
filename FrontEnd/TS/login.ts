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
    return value === "Admin" ||
      value === "Manager" ||
      value === "Pharmacist";
  }

  document.addEventListener("DOMContentLoaded", () => {
    const savedRole = Auth.role();

    // CHECK IF ALREADY LOGGED IN
    if(Auth.isLoggedIn()){
        const role=Auth.role();
        
    if(role){
        
        window.location.replace(
            "dashboard.html#"+role.toLowerCase()
        );
    }
    
    return;
    }

    const form =
      document.querySelector(".loginbox") as HTMLFormElement;

    const emailInput =
      document.getElementById("email") as HTMLInputElement;

    const passwordInput =
      document.querySelector("#password") as HTMLInputElement;

    const button =
      document.querySelector(".signin-btn") as HTMLButtonElement;

    const errorBox =
      document.querySelector("#login-error") as HTMLParagraphElement    ;

      form.addEventListener(
      "submit",
      async (event: SubmitEvent) => {
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
          const result = await Api.post<
            LoginResponse,
            LoginRequest
          >("/User/login", request);

          if (!isUserRole(result.role)) {
            throw new Error(
              "The server returned an unsupported user role.",
            );
          }

          Auth.save(result);

          window.location.href =
            `dashboard.html#${result.role.toLowerCase()}`;
        } catch (error: unknown) {
          errorBox.textContent = errorMessage(error);
        } finally {
          button.disabled = false;
          form.removeAttribute("aria-busy");
          button.textContent = originalText;
        }
      },
    );
  });
})();
