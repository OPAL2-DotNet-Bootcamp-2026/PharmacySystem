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