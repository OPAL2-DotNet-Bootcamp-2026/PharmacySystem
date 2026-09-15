(() => {
  type OrderActionStatus = "Approved" | "Cancelled";

  interface Pharmacy {
    pharmacyID: number;
    pharmacyName: string;
    isActive: boolean;
  }

   interface Medicine {
    medicineID: number;
    medicineName: string;
    unitPrice: number;
  }

  interface Pharmacist {
    pharmacistID: number;
    userID: number;
    pharmacyID: number;
    fullName: string;
    isActive: boolean;
  }

  interface OrderLine {
    medicineID: number;
    medicineName: string;
    unitPrice: number;
    quantity: number;
  }

  interface PharmacistOrderDetail {
    medicineID: number;
    medicineName: string;
    quantity: number;
  }

  interface PharmacistOrder {
    pharmacistOrderId: number;
    pharmacistID: number;
    fullName: string;
    pharmacyID: number;
    pharmacyName: string;
    orderDate: string;
    totalCost: number;
    status: string;
    orderDetails: PharmacistOrderDetail[];
  }

  interface CreateOrderDetailRequest {
    medicineID: number;
    quantity: number;
  }

   interface CreateOrderRequest {
    pharmacistID: number;
    pharmacyID: number;
    orderDetails: CreateOrderDetailRequest[];
  }

   interface CreateOrderResponse {
    pharmacistOrderId: number;
    message: string;
  }

   interface UpdateOrderStatusRequest {
    status: OrderActionStatus;
  }

  interface JwtPayload {
    [claim: string]: unknown;
    nameid?: unknown;
    sub?: unknown;
  }

  const userIdClaim =
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

  function requireElement<T extends Element>(
    selector: string,
  ): T {
    const element = document.querySelector<T>(selector);

    if (!element) {
      throw new Error(
        `Required order-page element is missing: ${selector}`,
      );
    }

    return element;
  }

  function errorMessage(error: unknown): string {
    return error instanceof Error
      ? error.message
      : "Something went wrong. Please try again.";
  }

  function isUserRole(
    value: string | null,
  ): value is UserRole {
    return value === "Admin" ||
      value === "Manager" ||
      value === "Pharmacist";
  }

  function getUserIdFromToken(): number | null {
    const token = Auth.token();

    if (!token) {
      return null;
    }

    try {
      const payloadPart = token.split(".")[1];

      if (!payloadPart) {
        return null;
      }

      const base64 = payloadPart
        .replace(/-/g, "+")
        .replace(/_/g, "/");

      const paddedBase64 = base64.padEnd(
        Math.ceil(base64.length / 4) * 4,
        "=",
      );

      const parsed: unknown =
        JSON.parse(atob(paddedBase64));

      if (!parsed || typeof parsed !== "object") {
        return null;
      }

      const payload = parsed as JwtPayload;

      const rawUserId =
        payload[userIdClaim] ??
        payload.nameid ??
        payload.sub;

      const userId = Number(rawUserId);

      return Number.isInteger(userId) && userId > 0
        ? userId
        : null;
    } catch (error: unknown) {
      console.error(
        "Could not read the login token:",
        error,
      );
        return null;
    }
  }

  function setRoleHash(role: UserRole): void {
    const expectedHash = `#${role.toLowerCase()}`;

    if (window.location.hash !== expectedHash) {
      window.history.replaceState(
        null,
        "",
        `${window.location.pathname}` +
          `${window.location.search}` +
          `${expectedHash}`,
      );
    }
}

function formatMoney(value: number): string {
    return `OMR ${Number(value).toFixed(3)}`;
  }

  function formatDate(value: string): string {
    const date = new Date(value);

    return Number.isNaN(date.getTime())
      ? "-"
      : date.toLocaleDateString("en-GB");
  }

  function appendTextCell(
    row: HTMLTableRowElement,
    text: string,
    className?: string,
  ): HTMLTableCellElement {
    const cell = row.insertCell();
    cell.textContent = text;

    if (className) {
      cell.className = className;
    }

    return cell;
  }

  function appendMedicineCell(
    row: HTMLTableRowElement,
    details: PharmacistOrderDetail[],
  ): void {
    const cell = row.insertCell();

    if (details.length === 0) {
      cell.textContent = "-";
      return;
    }

    details.forEach((detail) => {
      const line = document.createElement("div");

      line.textContent =
        `${detail.medicineName} ×${detail.quantity}`;

      cell.append(line);
    });
  }

   function setTableMessage(
    tableBody: HTMLTableSectionElement,
    colspan: number,
    message: string,
  ): void {
    const row = tableBody.insertRow();
    const cell = row.insertCell();

    cell.colSpan = colspan;
    cell.className = "text-center py-4";
    cell.textContent = message;
  }

  async function initialise(): Promise<void> {
    if (!Auth.isLoggedIn()) {
      window.location.replace("login.html");
      return;
    }

    const roleValue = Auth.role();

    if (!isUserRole(roleValue)) {
      window.location.replace("login.html");
      return;
    }






