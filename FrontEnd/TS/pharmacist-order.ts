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

    const role = roleValue;
    setRoleHash(role);

    const pharmacySelect =
      requireElement<HTMLSelectElement>("#pharmacyId");

    const medicineSelect =
      requireElement<HTMLSelectElement>("#medicineId");

    const quantityInput =
      requireElement<HTMLInputElement>("#quantity");

    const addMedicineButton =
      requireElement<HTMLButtonElement>(
        ".btn-add-medicine",
      );

      const submitButton =
      requireElement<HTMLButtonElement>(
        ".btn-submit-order",
      );

      const orderLines =
      requireElement<HTMLDivElement>("#orderLines");

    const estimatedTotal =
      requireElement<HTMLElement>("#estimatedTotal");

    const allOrdersTableBody =
      requireElement<HTMLTableSectionElement>(
        "#allOrdersTableBody",
      );

      const myOrdersTableBody =
      requireElement<HTMLTableSectionElement>(
        "#myOrdersTableBody",
      );

      let medicines: Medicine[] = [];
    let orderDetails: OrderLine[] = [];

    let currentPharmacist: Pharmacist | null = null;

    async function loadPharmacies(): Promise<void> {
      const pharmacies =
        await Api.get<Pharmacy[]>("/Pharmacy");

      const placeholder =
        new Option("Select pharmacy", "");

      const options = pharmacies
        .filter((pharmacy) => pharmacy.isActive)
        .map(
          (pharmacy) =>
            new Option(
              pharmacy.pharmacyName,
              String(pharmacy.pharmacyID),
            ),
        );

      pharmacySelect.replaceChildren(
        placeholder,
        ...options,
      );
    }

    async function loadMedicines(): Promise<void> {
      medicines =
        await Api.get<Medicine[]>(
          "/Medicine/GetAvailable",
        );

      const placeholder =
        new Option("Select medicine", "");

      const options = medicines.map(
        (medicine) =>
          new Option(
            medicine.medicineName,
            String(medicine.medicineID),
          ),
      );

      medicineSelect.replaceChildren(
        placeholder,
        ...options,
      );
    }

    async function loadCurrentPharmacist():
      Promise<void> {
      const userId = getUserIdFromToken();

      if (!userId) {
        throw new Error(
          "Your user ID could not be read " +
            "from the login token.",
        );
      }

      const pharmacists =
        await Api.get<Pharmacist[]>("/Pharmacist");

      currentPharmacist =
        pharmacists.find(
          (pharmacist) =>
            pharmacist.userID === userId &&
            pharmacist.isActive,
        ) ?? null;

      if (!currentPharmacist) {
        throw new Error(
          "Your active pharmacist profile was not found.",
        );
      }

       pharmacySelect.value =
        String(currentPharmacist.pharmacyID);

      pharmacySelect.disabled = true;
    }

    function renderOrderDetails(): void {
      orderLines.replaceChildren();

      orderLines.classList.toggle(
        "order-lines-empty",
        orderDetails.length === 0,
      );

       if (orderDetails.length === 0) {
        const heading =
          document.createElement("h3");

        heading.textContent =
          "No medicines added yet";

        const message =
          document.createElement("p");

        message.textContent =
          "Pick a medicine and a quantity, " +
          "then add it to the order.";

        orderLines.append(heading, message);
        estimatedTotal.textContent = "OMR 0.000";
        return;
      }

      const fragment =
        document.createDocumentFragment();

      let total = 0;

      orderDetails.forEach((detail, index) => {
        const subtotal =
          detail.unitPrice * detail.quantity;

        total += subtotal;

        const line =
          document.createElement("div");

        line.className =
          "d-flex justify-content-between " +
          "align-items-center border-bottom py-3";

          const information =
          document.createElement("div");

        const name =
          document.createElement("strong");

        name.textContent = detail.medicineName;

        const quantity =
          document.createElement("div");

        quantity.className = "text-muted";
        quantity.textContent =
          `Quantity: ${detail.quantity}`;

        information.append(name, quantity);

         const controls =
          document.createElement("div");

        controls.className =
          "d-flex align-items-center gap-3";

        const price =
          document.createElement("strong");

        price.textContent = formatMoney(subtotal);

        const removeButton =
          document.createElement("button");

        removeButton.type = "button";

        removeButton.className =
          "btn btn-sm btn-outline-danger";

        removeButton.dataset.removeIndex =
          String(index);

        removeButton.textContent = "Remove";

        removeButton.setAttribute(
          "aria-label",
          `Remove ${detail.medicineName} ` +
            "from the order",
        );

        controls.append(price, removeButton);
        line.append(information, controls);
        fragment.append(line);
      });

      orderLines.append(fragment);

      estimatedTotal.textContent =
        formatMoney(total);
    }

    function addMedicine(): void {
      const medicineID =
        Number(medicineSelect.value);

      const quantity =
        Number(quantityInput.value);

      if (
        !Number.isInteger(medicineID) ||
        medicineID < 1
      ) {
        alert("Please select a medicine.");
        return;
      }

      if (
        !Number.isInteger(quantity) ||
        quantity < 1
      ) {
        alert(
          "Quantity must be a whole number " +
            "greater than 0.",
        );
        return;
      }

      const medicine = medicines.find(
        (item) => item.medicineID === medicineID,
      );

      if (!medicine) {
        alert("Medicine was not found.");
        return;
      }

      const existingMedicine =
        orderDetails.find(
          (item) =>
            item.medicineID === medicineID,
        );

        if (existingMedicine) {
        existingMedicine.quantity += quantity;
      } else {
        orderDetails.push({
          medicineID,
          medicineName: medicine.medicineName,
          unitPrice: Number(medicine.unitPrice),
          quantity,
        });
      }

      renderOrderDetails();

      medicineSelect.value = "";
      quantityInput.value = "1";
    }

    function renderAllOrders(
      orders: PharmacistOrder[],
    ): void {
      allOrdersTableBody.replaceChildren();

      if (orders.length === 0) {
        setTableMessage(
          allOrdersTableBody,
          7,
          "No orders found.",
        );

        return;
      }

      const fragment =
        document.createDocumentFragment();

      orders.forEach((order) => {
        const row =
          document.createElement("tr");

        appendTextCell(
          row,
          `#${order.pharmacistOrderId}`,
        );

        const pharmacyCell = row.insertCell();

        const pharmacyName =
          document.createElement("strong");

        pharmacyName.textContent =
          order.pharmacyName;

        const pharmacistName =
          document.createElement("small");

        pharmacistName.textContent =
          order.fullName;

        pharmacyCell.append(
          pharmacyName,
          document.createElement("br"),
          pharmacistName,
        );

        appendMedicineCell(
          row,
          order.orderDetails ?? [],
        );

        appendTextCell(
          row,
          formatDate(order.orderDate),
        );

        appendTextCell(
          row,
          formatMoney(order.totalCost),
          "text-end",
        );

        appendTextCell(row, order.status);

        const actionsCell = row.insertCell();
        actionsCell.className = "text-end";

        if (order.status === "Pending") {
          const approveButton =
            document.createElement("button");

          approveButton.type = "button";

          approveButton.className =
            "btn btn-sm btn-success me-1";

          approveButton.dataset.approveId =
            String(order.pharmacistOrderId);

          approveButton.textContent = "Approve";

          const rejectButton =
            document.createElement("button");

          rejectButton.type = "button";

          rejectButton.className =
            "btn btn-sm btn-danger";

          rejectButton.dataset.rejectId =
            String(order.pharmacistOrderId);

          rejectButton.textContent = "Reject";

          actionsCell.append(
            approveButton,
            rejectButton,
          );
        } else {
          actionsCell.textContent = "-";
        }

        fragment.append(row);
      });

      allOrdersTableBody.append(fragment);
    }

    function renderMyOrders(
      orders: PharmacistOrder[],
    ): void {
      myOrdersTableBody.replaceChildren();

      if (orders.length === 0) {
        setTableMessage(
          myOrdersTableBody,
          6,
          "No orders found.",
        );

        return;
      }

      const fragment =
        document.createDocumentFragment();

      orders.forEach((order) => {
        const row =
          document.createElement("tr");

        appendTextCell(
          row,
          `#${order.pharmacistOrderId}`,
        );

        appendTextCell(row, order.pharmacyName);

         appendMedicineCell(
          row,
          order.orderDetails ?? [],
        );

        appendTextCell(
          row,
          formatDate(order.orderDate),
        );

        appendTextCell(
          row,
          formatMoney(order.totalCost),
          "text-end",
        );

        appendTextCell(row, order.status);

        fragment.append(row);
      });

      myOrdersTableBody.append(fragment);
    } 

    function updateOrderCounts(
      orders: PharmacistOrder[],
    ): void {
      const pendingCount = orders.filter(
        (order) => order.status === "Pending",
      ).length;

      document
        .querySelectorAll<HTMLElement>(
          "[data-order-count]",
        )
        .forEach((badge) => {
          badge.textContent =
            String(pendingCount);
        });
    }

    async function loadOrders(): Promise<void> {
      try {
        const orders =
          await Api.get<PharmacistOrder[]>(
            "/PharmacistOrder",
          );

        updateOrderCounts(orders);

        if (
          role === "Admin" ||
          role === "Manager"
        ) {
          renderAllOrders(orders);
          return;
        }

        const pharmacist = currentPharmacist;

        if (pharmacist) {
          renderMyOrders(
            orders.filter(
              (order) =>
                order.pharmacistID ===
                pharmacist.pharmacistID,
            ),
          );
        }
      } catch (error: unknown) {
        console.error(
          "Failed to load orders:",
          error,
        );

        const target =
          role === "Pharmacist"
            ? myOrdersTableBody
            : allOrdersTableBody;

        const colspan =
          role === "Pharmacist" ? 6 : 7;

        target.replaceChildren();

        setTableMessage(
          target,
          colspan,
          errorMessage(error),
        );
      }
    }

    async function createPharmacistOrder():
      Promise<void> {
      if (
        role !== "Pharmacist" ||
        !currentPharmacist
      ) {
        alert(
          "An active pharmacist profile " +
            "is required to create an order.",
        );

        return;
      }

      if (orderDetails.length === 0) {
        alert(
          "Please add at least one medicine.",
        );

        return;
      }

      const order: CreateOrderRequest = {
        pharmacistID:
          currentPharmacist.pharmacistID,

        pharmacyID:
          currentPharmacist.pharmacyID,

        orderDetails: orderDetails.map(
          ({ medicineID, quantity }) => ({
            medicineID,
            quantity,
          }),
        ),
      };

      submitButton.disabled = true;
      submitButton.textContent = "Submitting...";

      try {
        await Api.post<
          CreateOrderResponse,
          CreateOrderRequest
        >("/PharmacistOrder", order);

        alert(
          "Pharmacist order created successfully.",
        );

        orderDetails = [];
        renderOrderDetails();

        await loadOrders();
      } catch (error: unknown) {
        console.error(
          "Failed to create order:",
          error,
        );

        alert(errorMessage(error));
      } finally {
        submitButton.disabled = false;
        submitButton.textContent =
          "Submit order";
      }
    }

     async function updateOrderStatus(
      orderID: number,
      status: OrderActionStatus,
    ): Promise<void> {
      if (
        role !== "Admin" &&
        role !== "Manager"
      ) {
        return;
      }

      const request: UpdateOrderStatusRequest = {
        status,
      };

      try {
        await Api.put<
          string,
          UpdateOrderStatusRequest
        >(
          `/PharmacistOrder/${orderID}/status`,
          request,
        );

        alert(
          `Order ${status.toLowerCase()} successfully.`,
        );

        await loadOrders();
      } catch (error: unknown) {
        console.error(
          "Failed to update order:",
          error,
        );

        alert(errorMessage(error));
      }
    }

     if (role === "Pharmacist") {
      addMedicineButton.addEventListener(
        "click",
        addMedicine,
      );

      submitButton.addEventListener(
        "click",
        () => {
          void createPharmacistOrder();
        },
      );

      orderLines.addEventListener(
        "click",
        (event: MouseEvent) => {
          if (!(event.target instanceof Element)) {
            return;
          }

          const button =
            event.target.closest<HTMLButtonElement>(
              "[data-remove-index]",
            );

            if (!button) {
            return;
          }

          const index =
            Number(button.dataset.removeIndex);

          if (
            Number.isInteger(index) &&
            index >= 0 &&
            index < orderDetails.length
          ) {
            orderDetails.splice(index, 1);
            renderOrderDetails();
          }
        },
      );
    }

    if (
      role === "Admin" ||
      role === "Manager"
    ) {
      allOrdersTableBody.addEventListener(
        "click",
        (event: MouseEvent) => {
          if (!(event.target instanceof Element)) {
            return;
          }

          const approveButton =
            event.target.closest<HTMLButtonElement>(
              "[data-approve-id]",
            );

          const rejectButton =
            event.target.closest<HTMLButtonElement>(
              "[data-reject-id]",
            );

          const button =
            approveButton ?? rejectButton;

            if (!button) {
            return;
          }

          const orderID = Number(
            approveButton?.dataset.approveId ??
              rejectButton?.dataset.rejectId,
          );

          if (
            !Number.isInteger(orderID) ||
            orderID < 1
          ) {
            return;
          }

          button.disabled = true;

          void updateOrderStatus(
            orderID,
            approveButton
              ? "Approved"
              : "Cancelled",
          );
        },
      );
    }

    try {
      if (role === "Pharmacist") {
        await Promise.all([
          loadPharmacies(),
          loadMedicines(),
        ]);

        await loadCurrentPharmacist();
        renderOrderDetails();
      }

       await loadOrders();
    } catch (error: unknown) {
      console.error(
        "Failed to initialise the order page:",
        error,
      );

      alert(errorMessage(error));
    }
  }











      






