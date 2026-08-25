document.addEventListener(
    "DOMContentLoaded",
    function () {


        // ==========================================
        // SETTINGS
        // ==========================================

        const MAIN_WAREHOUSE_ID = 1;

        const LOW_STOCK_LIMIT = 20;



        // ==========================================
        // LOGIN CHECK
        // ==========================================

        if (!Auth.isLoggedIn()) {

            window.location.href =
                "login.html";

            return;

        }



        // ==========================================
        // HTML ELEMENTS
        // ==========================================

        const stockBody =
            document.getElementById(
                "warehouseStockBody"
            );


        const totalElement =
            document.getElementById(
                "warehouseTotal"
            );


        const linesElement =
            document.getElementById(
                "warehouseLines"
            );



        // ==========================================
        // ESCAPE HTML
        // Protect values before adding to HTML
        // ==========================================

        function escapeHTML(value) {

            const div =
                document.createElement("div");


            div.textContent =
                value ?? "";


            return div.innerHTML;

        }



        // ==========================================
        // GET VALUE
        // Supports camelCase and PascalCase
        // ==========================================

        function getValue(
            item,
            camelCase,
            pascalCase,
            fallback = ""
        ) {

            return (

                item?.[camelCase] ??

                item?.[pascalCase] ??

                fallback

            );

        }



        // ==========================================
        // NORMALISE STOCK
        // ==========================================

        function normaliseStock(item) {

            return {

                medicineName:
                    getValue(
                        item,
                        "medicineName",
                        "MedicineName",
                        "Unknown medicine"
                    ),


                categoryName:
                    getValue(
                        item,
                        "categoryName",
                        "CategoryName",
                        "—"
                    ),


                quantity:
                    Number(
                        getValue(
                            item,
                            "quantity",
                            "Quantity",
                            0
                        )
                    ),


                expiryDate:
                    getValue(
                        item,
                        "expiryDate",
                        "ExpiryDate",
                        null
                    )

            };

        }



        // ==========================================
        // FORMAT EXPIRY DATE
        // ==========================================

        function formatDate(value) {

            if (!value) {

                return "—";

            }


            const date = new Date(

                `${String(value)
                    .slice(0, 10)}T00:00:00`

            );


            if (
                Number.isNaN(
                    date.getTime()
                )
            ) {

                return String(value);

            }


            return new Intl.DateTimeFormat(

                "en-GB",

                {
                    day: "2-digit",

                    month: "short",

                    year: "numeric"
                }

            ).format(date);

        }



        // ==========================================
        // SHOW TABLE MESSAGE
        // ==========================================

        function showMessage(message) {

            stockBody.innerHTML = `

                <tr>

                    <td
                        colspan="5"
                        class="warehouse-message">

                        ${escapeHTML(message)}

                    </td>

                </tr>

            `;

        }



        // ==========================================
        // DISPLAY WAREHOUSE STOCK
        // ==========================================

        function displayStock(data) {


            const stocks =

                (
                    Array.isArray(data)
                        ? data
                        : []
                )

                .map(normaliseStock);



            // Calculate all quantities

            const totalUnits =

                stocks.reduce(

                    function (
                        total,
                        stock
                    ) {

                        return (
                            total +
                            stock.quantity
                        );

                    },

                    0

                );



            // Get the largest quantity
            // Used for progress bars

            const maximumQuantity =

                Math.max(

                    ...stocks.map(

                        function (stock) {

                            return stock.quantity;

                        }

                    ),

                    1

                );



            // Update total units

            totalElement.textContent =

                totalUnits.toLocaleString();



            // Update medicine count

            linesElement.textContent =

                `${stocks.length} ${
                    stocks.length === 1
                        ? "medicine"
                        : "medicines"
                }`;



            // No stock

            if (stocks.length === 0) {

                showMessage(

                    "No medicines are currently stored in the main warehouse."

                );

                return;

            }



            // Clear table

            stockBody.innerHTML = "";



            // Display each medicine

            stocks.forEach(

                function (stock) {


                    // Low when quantity is
                    // 20 or less

                    const isLow =

                        stock.quantity <=

                        LOW_STOCK_LIMIT;



                    const level =

                        isLow
                            ? "Low"
                            : "Healthy";



                    const levelClass =

                        isLow
                            ? "low"
                            : "healthy";



                    const fillClass =

                        isLow
                            ? "low-fill"
                            : "healthy-fill";



                    // Progress bar percentage

                    const barWidth = Math.max(

                        (
                            stock.quantity /
                            maximumQuantity
                        ) * 100,

                        4

                    );



                    const row =

                        document.createElement(
                            "tr"
                        );



                    row.innerHTML = `

                        <!-- Medicine -->

                        <td class="medicine-name">

                            ${escapeHTML(
                                stock.medicineName
                            )}

                        </td>


                        <!-- Category -->

                        <td class="medicine-category">

                            ${escapeHTML(
                                stock.categoryName
                            )}

                        </td>


                        <!-- Quantity -->

                        <td>

                            <div class="quantity-cell">

                                <strong>

                                    ${escapeHTML(
                                        stock.quantity
                                    )}

                                </strong>


                                <span
                                    class="quantity-track">

                                    <span
                                        class="quantity-fill ${fillClass}"
                                        style="width: ${barWidth}%">
                                    </span>

                                </span>

                            </div>

                        </td>


                        <!-- Expiry -->

                        <td class="expiry-date">

                            ${escapeHTML(

                                formatDate(
                                    stock.expiryDate
                                )

                            )}

                        </td>


                        <!-- Level -->

                        <td class="text-end">

                            <span
                                class="stock-level ${levelClass}">

                                <span class="level-dot">
                                </span>

                                ${level}

                            </span>

                        </td>

                    `;



                    stockBody.appendChild(row);

                }

            );

        }



        // ==========================================
        // LOAD STOCK FROM BACKEND
        //
        // GET:
        // /api/WarehouseStock/GetByWarehouse/1
        // ==========================================

        async function loadWarehouseStock() {


            showMessage(

                "Loading warehouse stock..."

            );


            try {


                const data = await Api.get(

                    `/WarehouseStock/GetByWarehouse/${MAIN_WAREHOUSE_ID}`

                );


                displayStock(data);


            } catch (error) {


                console.error(

                    "Warehouse stock error:",

                    error

                );


                totalElement.textContent =
                    "0";


                linesElement.textContent =
                    "0 medicines";


                showMessage(

                    error.message ||

                    "Could not load warehouse stock."

                );

            }

        }



        // ==========================================
        // PAGE LOAD
        // ==========================================

        loadWarehouseStock();

    }
);