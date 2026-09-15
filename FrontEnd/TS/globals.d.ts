type UserRole = "Admin" | "Manager" | "Pharmacist";

interface AuthSession {
  token: string;
  username: string;
  role: UserRole;
}