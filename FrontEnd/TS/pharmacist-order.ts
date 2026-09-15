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