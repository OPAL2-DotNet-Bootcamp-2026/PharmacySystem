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
