type UserRole = "Admin" | "Manager" | "Pharmacist";

interface AuthSession {
  token: string;
  username: string;
  role: UserRole;
}

interface AuthClient {
  save(data: AuthSession): void;
  token(): string | null;
  role(): UserRole | null;
  isLoggedIn(): boolean;
  logout(): void;   
}

interface ApiClient {
  get<TResponse>(endpoint: string): Promise<TResponse>;

  post<TResponse, TBody>(
    endpoint: string,
    body: TBody,
  ): Promise<TResponse>;

  put<TResponse, TBody>(
    endpoint: string,
    body: TBody,
  ): Promise<TResponse>;

  del<TResponse>(endpoint: string): Promise<TResponse>;
}
