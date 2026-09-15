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
});