// ==========================================
// PHARMACY STOCK PAGE
// ==========================================


// ==========================================
// API URL
// ==========================================

const API_URL =
    "https://localhost:7000";



// ==========================================
// HTML ELEMENTS
// ==========================================

const pharmacyIdInput =
    document.querySelector(
        "#pharmacyIdInput"
    );


const loadPharmacyButton =
    document.querySelector(
        "#loadPharmacyButton"
    );


const stockSearchInput =
    document.querySelector(
        "#stockSearchInput"
    );


const stockLevelFilter =
    document.querySelector(
        "#stockLevelFilter"
    );


const stockTableBody =
    document.querySelector(
        "#stockTableBody"
    );


const stockSummary =
    document.querySelector(
        "#stockSummary"
    );


const stockCount =
    document.querySelector(
        "#stockCount"
    );


const totalQuantity =
    document.querySelector(
        "#totalQuantity"
    );


const lowStockCount =
    document.querySelector(
        "#lowStockCount"
    );



// ==========================================
// STORE CURRENT STOCK
// ==========================================

let currentStocks = [];



// ==========================================
// GET TOKEN
// ==========================================

function getToken() {

    return localStorage.getItem(
        "token"
    );
}



// ==========================================
// AUTH HEADERS
// ==========================================

function getAuthHeaders() {

    const token =
        getToken();


    return {

        "Authorization":
            `Bearer ${token}`

    };
}



// ==========================================
// LOAD PHARMACY STOCK
// ==========================================

async function loadPharmacyStock() {


    const pharmacyId =
        Number(
            pharmacyIdInput.value
        );


    if (!pharmacyId) {

        alert(
            "Please enter a pharmacy ID."
        );

        return;
    }


    try {


        // ==================================
        // FETCH STOCK FROM BACKEND
        // ==================================

        const response =
            await fetch(

                `${API_URL}/api/PharmacyStock/by-pharmacy/${pharmacyId}`,

                {

                    headers:
                        getAuthHeaders()

                }

            );


        // ==================================
        // CHECK RESPONSE
        // ==================================

        if (!response.ok) {

            throw new Error(
                `Request failed: ${response.status}`
            );
        }



        // ==================================
        // JSON → JAVASCRIPT ARRAY
        // ==================================

        const stocks =
            await response.json();



        console.log(
            "Pharmacy Stock:",
            stocks
        );



        // Keep a copy in JavaScript

        currentStocks =
            stocks;



        // Display stock

        renderPharmacyStock(
            stocks
        );



        // Update summary cards

        renderStockSummary(
            stocks
        );


        if (stockSummary) {

            stockSummary.textContent =
                `${stocks.length} medicine(s) in Pharmacy #${pharmacyId}`;

        }


    }

    catch (error) {


        console.error(
            "Failed to load pharmacy stock:",
            error
        );


        currentStocks = [];


        renderPharmacyStock(
            []
        );


        renderStockSummary(
            []
        );


        if (stockSummary) {

            stockSummary.textContent =
                "Failed to load pharmacy stock.";

        }


    }

}



// ==========================================
// RENDER PHARMACY STOCK
// ==========================================

function renderPharmacyStock(
    stocks
) {


    if (!stockTableBody) {

        return;

    }


    // Clear previous rows

    stockTableBody.innerHTML =
        "";



    // ======================================
    // LOOP THROUGH STOCK
    // ======================================

    stocks.forEach(
        stock => {


            // Medicine ID

            const medicineId =
                stock.medicineId
                ??
                stock.MedicineID
                ??
                "-";



            // Medicine Name

            const medicineName =
                stock.medicineName
                ??
                `Medicine #${medicineId}`;



            // Quantity

            const quantity =
                stock.quantity
                ??
                0;



            // Expiry Date

            const expiryDate =
                stock.expiryDate
                ??
                "-";



            // Stock level

            const stockLevel =
                getStockLevel(
                    quantity
                );



            // Badge

            const badgeClass =
                getStockBadgeClass(
                    stockLevel
                );



            // ==================================
            // ADD TABLE ROW
            // ==================================

            stockTableBody.innerHTML += `

                <tr
                    data-medicine-id="${medicineId}"
                >


                    <td>

                        <strong>

                            ${medicineName}

                        </strong>

                    </td>


                    <td>

                        ${medicineId}

                    </td>


                    <td class="text-end">

                        ${quantity}

                    </td>


                    <td>

                        ${formatDate(
                            expiryDate
                        )}

                    </td>


                    <td>

                        <span
                            class="badge ${badgeClass}"
                        >

                            ${stockLevel}

                        </span>

                    </td>


                </tr>

            `;


        }

    );



    // ======================================
    // IF NO STOCK
    // ======================================

    if (
        stocks.length === 0
    ) {


        stockTableBody.innerHTML = `

            <tr>

                <td
                    colspan="5"
                    class="text-center text-muted py-4"
                >

                    No pharmacy stock found.

                </td>

            </tr>

        `;

    }

}



// ==========================================
// STOCK LEVEL
// ==========================================

function getStockLevel(
    quantity
) {


    if (
        quantity === 0
    ) {

        return "Out";

    }


    if (
        quantity <= 10
    ) {

        return "Low";

    }


    return "Healthy";

}



// ==========================================
// STOCK BADGE CLASS
// ==========================================

function getStockBadgeClass(
    stockLevel
) {


    if (
        stockLevel === "Out"
    ) {

        return "text-bg-danger";

    }


    if (
        stockLevel === "Low"
    ) {

        return "text-bg-warning";

    }


    return "text-bg-success";

}



// ==========================================
// FORMAT DATE
// ==========================================

function formatDate(
    dateValue
) {


    if (
        !dateValue
        ||
        dateValue === "-"
    ) {

        return "-";

    }


    const date =
        new Date(
            dateValue
        );


    if (
        Number.isNaN(
            date.getTime()
        )
    ) {

        return dateValue;

    }


    return date.toLocaleDateString(

        "en-GB",

        {

            day:
                "2-digit",

            month:
                "short",

            year:
                "numeric"

        }

    );

}



// ==========================================
// SUMMARY NUMBERS
// ==========================================

function renderStockSummary(
    stocks
) {


    // Number of medicines

    const medicineCount =
        stocks.length;



    // Total quantity

    const quantityTotal =
        stocks.reduce(

            (
                total,
                stock
            ) =>

                total
                +
                (
                    stock.quantity
                    ??
                    0
                ),

            0

        );



    // Low stock count

    const lowCount =
        stocks.filter(

            stock => {

                const quantity =
                    stock.quantity
                    ??
                    0;


                return (
                    quantity > 0
                    &&
                    quantity <= 10
                );

            }

        ).length;



    // ==================================
    // DISPLAY SUMMARY
    // ==================================

    if (stockCount) {

        stockCount.textContent =
            medicineCount;

    }


    if (totalQuantity) {

        totalQuantity.textContent =
            quantityTotal;

    }


    if (lowStockCount) {

        lowStockCount.textContent =
            lowCount;

    }

}



// ==========================================
// FILTER PHARMACY STOCK
// ==========================================

function filterPharmacyStock() {


    const searchText =
        stockSearchInput
            .value
            .trim()
            .toLowerCase();



    const selectedLevel =
        stockLevelFilter
            .value;



    // ======================================
    // FILTER CURRENT STOCK ARRAY
    // ======================================

    const filteredStocks =
        currentStocks.filter(

            stock => {


                // Medicine ID

                const medicineId =
                    String(
                        stock.medicineId
                        ??
                        stock.MedicineID
                        ??
                        ""
                    );



                // Medicine Name

                const medicineName =
                    (
                        stock.medicineName
                        ??
                        ""
                    )
                    .toLowerCase();



                // Quantity

                const quantity =
                    stock.quantity
                    ??
                    0;



                // Stock Level

                const stockLevel =
                    getStockLevel(
                        quantity
                    );



                // ==================================
                // SEARCH MATCH
                // ==================================

                const matchesSearch =

                    medicineName.includes(
                        searchText
                    )

                    ||

                    medicineId.includes(
                        searchText
                    );



                // ==================================
                // STOCK LEVEL MATCH
                // ==================================

                const matchesLevel =

                    selectedLevel ===
                        "All"

                    ||

                    stockLevel ===
                        selectedLevel;



                // Both must match

                return (

                    matchesSearch
                    &&
                    matchesLevel

                );

            }

        );



    // Display filtered rows

    renderPharmacyStock(
        filteredStocks
    );

}



// ==========================================
// SHOW LOW STOCK ONLY
// ==========================================

function showLowStock() {


    const lowStocks =
        currentStocks.filter(

            stock => {


                const quantity =
                    stock.quantity
                    ??
                    0;


                return (

                    quantity > 0
                    &&
                    quantity <= 10

                );

            }

        );


    renderPharmacyStock(
        lowStocks
    );

}



// ==========================================
// EVENTS
// ==========================================


// Load stock button

if (
    loadPharmacyButton
) {

    loadPharmacyButton.addEventListener(

        "click",

        loadPharmacyStock

    );

}



// Search while typing

if (
    stockSearchInput
) {

    stockSearchInput.addEventListener(

        "input",

        filterPharmacyStock

    );

}



// Stock level dropdown

if (
    stockLevelFilter
) {

    stockLevelFilter.addEventListener(

        "change",

        filterPharmacyStock

    );

}



// Press Enter inside Pharmacy ID

if (
    pharmacyIdInput
) {

    pharmacyIdInput.addEventListener(

        "keydown",

        event => {


            if (
                event.key === "Enter"
            ) {

                loadPharmacyStock();

            }

        }

    );

}