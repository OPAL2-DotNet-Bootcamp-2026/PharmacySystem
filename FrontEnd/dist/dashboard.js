"use strict";
document.addEventListener("DOMContentLoaded", function () {
    // LOGIN GUARD
    if (!Auth.isLoggedIn()) {
        window.location.href = "login.html";
        return;
    }
    const role = Auth.role() || "";
    const roleKey = role.toLowerCase();
    const isAdmin = role === "Admin";
    const isManager = role === "Manager";
    const isPharmacist = role === "Pharmacist";
    if (window.location.hash !== "#" + roleKey) {
        window.location.hash = roleKey;
    }
    const canSeeSupplies = isAdmin || isManager;
    // GET VALUE
    function val(obj, ...names) {
        if (!obj) {
            return undefined;
        }
        for (const name of names) {
            if (obj[name] !== undefined) {
                return obj[name];
            }
            const lower = name.charAt(0).toLowerCase() + name.slice(1);
            if (obj[lower] !== undefined) {
                return obj[lower];
            }
        }
        return undefined;
    }
    // SAFE GET
    async function safeGet(endpoint, fallback) {
        try {
            const data = await Api.get(endpoint);
            return data ?? fallback;
        }
        catch (error) {
            console.warn("Dashboard could not load " + endpoint, error.message);
            return fallback;
        }
    }
    // NUMBER
    function num(value) {
        const n = Number(value);
        return Number.isFinite(n)
            ? n
            : 0;
    }
    // SUM
    function sum(list, getter) {
        return list.reduce(function (total, item) {
            return total + num(getter(item));
        }, 0);
    }
    // ESCAPE HTML
    function escapeHTML(value) {
        const div = document.createElement("div");
        div.textContent = String(value ?? "");
        return div.innerHTML;
    }
    // FORMAT DATE
    function formatDate(value) {
        if (!value) {
            return "—";
        }
        const date = new Date(value);
        if (isNaN(date.getTime())) {
            return "—";
        }
        return date.toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric"
        });
    }
    function formatShortDate(value) {
        if (!value) {
            return "";
        }
        const date = new Date(value);
        if (isNaN(date.getTime())) {
            return "";
        }
        return date.toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "short"
        });
    }
    // STAT CARD
    function statCard(titleText) {
        const titles = document.querySelectorAll(".dashboard-stat-title");
        for (const title of titles) {
            if ((title.textContent || "")
                .trim()
                .toUpperCase()
                ===
                    titleText.toUpperCase()) {
                return title.closest(".dashboard-stat");
            }
        }
        return null;
    }
    function setStat(titleText, value, info) {
        const card = statCard(titleText);
        if (!card) {
            return;
        }
        const heading = card.querySelector("h2");
        if (heading) {
            heading.textContent = String(value);
        }
        const infoEl = card.querySelector(".dashboard-stat-info");
        if (infoEl
            &&
                info !== undefined) {
            infoEl.textContent = info;
        }
    }
    // PIPELINE NODE
    function setNode(index, value, unit) {
        const nodes = document.querySelectorAll(".pipe .node");
        const node = nodes[index];
        if (!node) {
            return;
        }
        const valueEl = node.querySelector(".node__val");
        if (valueEl) {
            valueEl.textContent = String(value);
        }
        const unitEl = node.querySelector(".node__unit");
        if (unitEl
            &&
                unit !== undefined) {
            unitEl.textContent = unit;
        }
    }
    // ==========================================
    // LIVE CLOCK
    function updateClock() {
        const label = document.querySelector(".pipeline__top span");
        if (!label) {
            return;
        }
        const now = new Date();
        const day = now.toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric"
        }).toUpperCase();
        const time = now.toLocaleTimeString("en-GB", {
            hour12: false
        });
        label.innerHTML = `<i class="livedot"></i> LIVE · ${day} · ${time}`;
    }
    updateClock();
    setInterval(updateClock, 1000);
    // ==========================================
    // MAIN LOAD
    async function loadDashboard() {
        const [suppliers, warehouses, pharmacies, orders, transfers, medicines] = await Promise.all([
            safeGet("/Supplier/GetAll", []),
            safeGet("/Warehouse/GetAll", []),
            safeGet("/Pharmacy", []),
            safeGet("/PharmacistOrder", []),
            safeGet("/Transfer", []),
            safeGet("/Medicine/GetAll", [])
        ]);
        // ==========================================
        // SUPPLIES
        const supplies = canSeeSupplies
            ? await safeGet("/Supply", [])
            : [];
        // ==========================================
        // WAREHOUSE STOCK
        const warehouseStockLists = await Promise.all(warehouses.map(function (w) {
            const id = val(w, "WarehouseID", "warehouseID");
            return safeGet(`/WarehouseStock/GetByWarehouse/${id}`, []);
        }));
        const warehouseStock = warehouseStockLists.flat();
        // ==========================================
        // PHARMACY STOCK
        const pharmacyStockLists = await Promise.all(pharmacies.map(function (p) {
            const id = val(p, "PharmacyID", "pharmacyID");
            return safeGet(`/PharmacyStock/by-pharmacy/${id}`, []);
        }));
        const pharmacyStock = pharmacyStockLists.flat();
        // ==========================================
        // TOTALS
        const activeSuppliers = suppliers.filter(function (s) {
            return val(s, "IsActive") === true;
        }).length;
        const inactiveSuppliers = suppliers.length - activeSuppliers;
        const warehouseUnits = sum(warehouseStock, function (s) {
            return val(s, "Quantity");
        });
        const pharmacyUnits = sum(pharmacyStock, function (s) {
            return val(s, "Quantity");
        });
        // ==========================================
        // TRANSFERS
        const movingTransfers = transfers.filter(function (t) {
            const status = String(val(t, "Status") || "").toLowerCase();
            return (status !== "received"
                &&
                    status !== "cancelled");
        });
        const roadUnits = sum(movingTransfers, function (t) {
            const details = val(t, "TransferDetails") || [];
            return sum(details, function (d) {
                return val(d, "Quantity");
            });
        });
        // ==========================================
        // PENDING ORDERS
        const pendingOrders = orders.filter(function (o) {
            return (String(val(o, "Status") || "").toLowerCase()
                ===
                    "pending");
        });
        // ==========================================
        // PIPELINE
        setNode(0, activeSuppliers, "active");
        setNode(1, warehouseUnits, "units");
        setNode(2, pendingOrders.length, "waiting");
        setNode(3, movingTransfers.length, "moving");
        setNode(4, pharmacyUnits, "units");
        // ==========================================
        // LOW STOCK
        const WAREHOUSE_MINIMUM = 50;
        const PHARMACY_MINIMUM = 15;
        let lowWarehouse = [];
        if (canSeeSupplies) {
            const lists = await Promise.all(warehouses.map(async function (w) {
                const id = val(w, "WarehouseID");
                const rows = await safeGet(`/WarehouseStock/GetLowStock/${id}?minimumQuantity=${WAREHOUSE_MINIMUM}`, []);
                return rows.map(function (r) {
                    return {
                        medicine: val(r, "MedicineName"),
                        place: val(w, "Location"),
                        quantity: num(val(r, "Quantity"))
                    };
                });
            }));
            lowWarehouse =
                lists.flat();
        }
        // ==========================================
        // PHARMACY NAME
        const pharmacyNameById = {};
        pharmacies.forEach(function (p) {
            pharmacyNameById[val(p, "PharmacyID")] =
                val(p, "PharmacyName");
        });
        const lowPharmacy = pharmacyStock
            .filter(function (s) {
            return (num(val(s, "Quantity"))
                <
                    PHARMACY_MINIMUM);
        })
            .map(function (s) {
            return {
                medicine: val(s, "MedicineName"),
                place: pharmacyNameById[val(s, "PharmacyID")]
                    ||
                        "Pharmacy",
                quantity: num(val(s, "Quantity"))
            };
        });
        const lowStockRows = lowWarehouse
            .concat(lowPharmacy)
            .sort(function (a, b) {
            return (a.quantity
                -
                    b.quantity);
        });
        // ==========================================
        // STAT CARDS
        setStat("SUPPLIERS", suppliers.length, `${inactiveSuppliers} inactive`);
        if (canSeeSupplies) {
            const lastSupply = supplies
                .slice()
                .sort(function (a, b) {
                return (new Date(val(b, "SupplyDate")).getTime()
                    -
                        new Date(val(a, "SupplyDate")).getTime());
            })[0];
            setStat("SUPPLIES RECEIVED", supplies.length, lastSupply
                ? "Last: " +
                    formatDate(val(lastSupply, "SupplyDate"))
                : "No supplies yet");
        }
        setStat("PENDING ORDERS", pendingOrders.length, `${orders.length} orders in total`);
        setStat("LOW STOCK LINES", lowStockRows.length, `${lowWarehouse.length} warehouse · ${lowPharmacy.length} pharmacy`);
        // ==========================================
        // DONUT
        const totalUnits = warehouseUnits
            +
                roadUnits
            +
                pharmacyUnits;
        const donut = document.querySelector(".donut-chart");
        if (donut) {
            const centre = donut.querySelector("strong");
            if (centre) {
                centre.textContent =
                    String(totalUnits);
            }
            if (totalUnits > 0) {
                const warehousePct = (warehouseUnits
                    /
                        totalUnits) * 100;
                const roadPct = warehousePct
                    +
                        (roadUnits
                            /
                                totalUnits) * 100;
                donut.style.background =
                    "conic-gradient("
                        +
                            `var(--green) 0 ${warehousePct}%,`
                        +
                            `var(--amber) ${warehousePct}% ${roadPct}%,`
                        +
                            `var(--mint) ${roadPct}% 100%)`;
            }
        }
        const legendValues = document.querySelectorAll(".stock-legend div strong");
        if (legendValues.length >= 3) {
            legendValues[0].textContent =
                String(warehouseUnits);
            legendValues[1].textContent =
                String(roadUnits);
            legendValues[2].textContent =
                String(pharmacyUnits);
        }
        // ==========================================
        // MEDICINES
        const categoryByName = {};
        medicines.forEach(function (m) {
            categoryByName[val(m, "MedicineName")] =
                val(m, "CategoryName");
        });
        const heldByMedicine = {};
        warehouseStock
            .concat(pharmacyStock)
            .forEach(function (s) {
            const name = val(s, "MedicineName")
                ||
                    "Unknown";
            heldByMedicine[name] =
                (heldByMedicine[name]
                    ||
                        0)
                    +
                        num(val(s, "Quantity"));
        });
        const topMedicines = Object.keys(heldByMedicine)
            .map(function (name) {
            return {
                name: name,
                category: categoryByName[name]
                    ||
                        "—",
                quantity: heldByMedicine[name]
            };
        })
            .sort(function (a, b) {
            return (b.quantity
                -
                    a.quantity);
        })
            .slice(0, 5);
        // ==========================================
        // MEDICINE BARS
        const barsBox = document.querySelector(".medicine-bars");
        if (barsBox) {
            if (topMedicines.length === 0) {
                barsBox.innerHTML =
                    `<p class="dashboard-stat-info">
                        No stock recorded yet.
                    </p>`;
            }
            else {
                const biggest = topMedicines[0].quantity
                    ||
                        1;
                barsBox.innerHTML =
                    topMedicines
                        .map(function (m) {
                        const width = Math.round((m.quantity
                            /
                                biggest) * 100);
                        return `

                                    <div class="medicine-bar">

                                        <div class="medicine-bar-top">

                                            <span>
                                                ${escapeHTML(m.name)}
                                            </span>

                                            <span>
                                                ${escapeHTML(m.category)}
                                            </span>

                                            <strong>
                                                ${m.quantity}
                                            </strong>

                                        </div>

                                        <div class="bar-track">

                                            <div
                                                class="bar-fill"
                                                style="width:${width}%;">
                                            </div>

                                        </div>

                                    </div>
                                `;
                    })
                        .join("");
            }
        }
        // ==========================================
        // ACTIVITY
        const activity = [];
        transfers.forEach(function (t) {
            activity.push({
                date: new Date(val(t, "TransferDate")),
                dot: String(val(t, "Status") || "").toLowerCase()
                    ===
                        "received"
                    ? "green-dot"
                    : "amber-dot",
                text: `Transfer #${val(t, "TransferId")} `
                    +
                        `${val(t, "Location") || "warehouse"} → `
                    +
                        `${val(t, "PharmacyName") || "pharmacy"} `
                    +
                        `(${val(t, "Status")})`
            });
        });
        orders.forEach(function (o) {
            activity.push({
                date: new Date(val(o, "OrderDate")),
                dot: String(val(o, "Status") || "").toLowerCase()
                    ===
                        "pending"
                    ? "amber-dot"
                    : "green-dot",
                text: `Order #${val(o, "PharmacistOrderId")} `
                    +
                        `by ${val(o, "FullName") || "pharmacist"} `
                    +
                        `— ${val(o, "Status")}`
            });
        });
        supplies.forEach(function (s) {
            activity.push({
                date: new Date(val(s, "SupplyDate")),
                dot: "green-dot",
                text: `Supply ${val(s, "BatchNumber")} added `
                    +
                        `${val(s, "Quantity")} units of `
                    +
                        `${val(s, "MedicineName")}`
            });
        });
        // ==========================================
        // ACTIVITY LIST
        const activityBox = document.querySelector(".activity-list");
        if (activityBox) {
            const recent = activity
                .filter(function (a) {
                return (!isNaN(a.date.getTime()));
            })
                .sort(function (a, b) {
                return (b.date.getTime()
                    -
                        a.date.getTime());
            })
                .slice(0, 6);
            activityBox.innerHTML =
                recent.length === 0
                    ? `<p class="dashboard-stat-info">
                        Nothing has happened yet.
                    </p>`
                    : recent
                        .map(function (a) {
                        return `

                                    <div class="activity-item">

                                        <span
                                            class="activity-dot ${a.dot}">
                                        </span>

                                        <span>
                                            ${escapeHTML(a.text)}
                                        </span>

                                        <small>
                                            ${formatShortDate(a.date)}
                                        </small>

                                    </div>
                                `;
                    })
                        .join("");
        }
        // ==========================================
        // LOW STOCK TABLE
        const lowTableBody = document.querySelector(".low-stock-table tbody");
        if (lowTableBody) {
            lowTableBody.innerHTML =
                lowStockRows.length === 0
                    ? `<tr>
                        <td colspan="3">
                            Nothing is running low.
                        </td>
                    </tr>`
                    : lowStockRows
                        .map(function (row) {
                        return `

                                    <tr>

                                        <td>
                                            <strong>
                                                ${escapeHTML(row.medicine)}
                                            </strong>
                                        </td>

                                        <td>
                                            ${escapeHTML(row.place)}
                                        </td>

                                        <td class="text-end low-number">
                                            ${row.quantity}
                                        </td>

                                    </tr>
                                `;
                    })
                        .join("");
        }
        // ==========================================
        // PHARMACIST CARDS
        if (isPharmacist) {
            const username = (localStorage.getItem("username")
                ||
                    "").toLowerCase();
            const myOrders = orders.filter(function (o) {
                return (String(val(o, "FullName")
                    ||
                        "").toLowerCase()
                    ===
                        username);
            });
            const ownOrders = myOrders.length > 0
                ? myOrders
                : orders;
            const myPending = ownOrders.filter(function (o) {
                return (String(val(o, "Status")
                    ||
                        "").toLowerCase()
                    ===
                        "pending");
            });
            setStat("MY PENDING ORDERS", myPending.length, "Waiting for approval");
            setStat("INCOMING TRANSFERS", movingTransfers.length, "In transit");
            setStat("MY PHARMACY STOCK", pharmacyUnits, "Units on shelf");
        }
    }
    // ==========================================
    // FIRST LOAD
    // ==========================================
    loadDashboard();
});
