document.addEventListener(
    "DOMContentLoaded",
    async () => {


        // =====================================
        // CHECK LOGIN
        // =====================================

        if (!Auth.isLoggedIn()) {

            window.location.href =
                "login.html";

            return;
        }



        // =====================================
        // GET USER ROLE
        // =====================================

        const role =
            Auth.role() || "";

        const roleKey =
            role.toLowerCase();



        // Make sure correct role view is used

        if (
            window.location.hash !==
            "#" + roleKey
        ) {

            window.location.hash =
                roleKey;
        }



        // =====================================
        // GET MAIN CONTENT
        // =====================================

        const mainContent =
            document.querySelector(
                ".main-content"
            );


        const footer =
            document.querySelector(
                ".small-footer"
            );



        // =====================================
        // REMOVE OLD HARDCODED STOCK CARDS
        // =====================================

        const oldCards =
            document.querySelectorAll(
                ".stock-card"
            );


        oldCards.forEach(
            card => {

                card.remove();

            }
        );



        // =====================================
        // CREATE DATABASE CONTAINER
        // =====================================

        const container =
            document.createElement(
                "div"
            );


        container.id =
            "pharmacyStockContainer";


        container.innerHTML = `

            <section class="stock-card">

                <p>
                    Loading pharmacy stock...
                </p>

            </section>

        `;



        mainContent.insertBefore(
            container,
            footer
        );



        // =====================================
        // LOAD PHARMACIES FROM DATABASE
        // =====================================

        try {


            const pharmacies =
                await Api.get(
                    "/Pharmacy"
                );


            console.log(
                "Pharmacies:",
                pharmacies
            );


            container.innerHTML =
                "";



            // =================================
            // NO PHARMACIES
            // =================================

            if (
                !pharmacies ||
                pharmacies.length === 0
            ) {

                container.innerHTML = `

                    <section class="stock-card">

                        <p>
                            No pharmacies found.
                        </p>

                    </section>

                `;

                return;
            }



            // =================================
            // LOOP THROUGH PHARMACIES
            // =================================

            for (
                const pharmacy
                of pharmacies
            ) {


                // Pharmacy ID from database

                const pharmacyID =
                    pharmacy.pharmacyID;



                // =================================
                // GET STOCK FOR THIS PHARMACY
                // =================================

                let stocks = [];


                try {

                    stocks =
                        await Api.get(

                            "/PharmacyStock/by-pharmacy/"
                            + pharmacyID

                        );

                }

                catch (error) {

                    console.error(
                        "Stock error for pharmacy:",
                        pharmacyID,
                        error
                    );


                    stocks = [];

                }



                // =================================
                // CREATE PHARMACY CARD
                // =================================

                const section =
                    document.createElement(
                        "section"
                    );


                section.className =
                    "stock-card";



                section.innerHTML = `


                    <div
                        class="stock-card-header"
                    >


                        <div>


                            <h2>

                                ${
                                    pharmacy.pharmacyName
                                    ||
                                    "Pharmacy"
                                }

                            </h2>


                            <p>

                                ${
                                    pharmacy.location
                                    ||
                                    ""
                                }

                            </p>


                        </div>



                        <span
                            class="stock-summary"
                        >

                            ${
                                stocks.length
                            }

                            medicine(s)

                        </span>


                    </div>



                    <div
                        class="table-responsive"
                    >


                        <table
                            class="table stock-table"
                        >


                            <thead>


                                <tr>


                                    <th>
                                        MEDICINE
                                    </th>


                                    <th>
                                        CATEGORY
                                    </th>


                                    <th
                                        class="text-end"
                                    >
                                        QUANTITY
                                    </th>


                                    <th>
                                        EXPIRY
                                    </th>


                                    <th
                                        class="text-end"
                                    >
                                        LEVEL
                                    </th>


                                </tr>


                            </thead>



                            <tbody>

                                ${
                                    createStockRows(
                                        stocks
                                    )
                                }

                            </tbody>


                        </table>


                    </div>


                `;



                container.appendChild(
                    section
                );

            }

        }


        catch (error) {


            console.error(
                "Failed to load pharmacies:",
                error
            );


            container.innerHTML = `

                <section class="stock-card">

                    <p>
                        Failed to load pharmacy stock.
                    </p>

                </section>

            `;

        }



        // =====================================
        // CREATE TABLE ROWS
        // =====================================

        function createStockRows(
            stocks
        ) {


            if (
                !stocks ||
                stocks.length === 0
            ) {

                return `

                    <tr>

                        <td
                            colspan="5"
                            class="text-center"
                        >

                            No stock available.

                        </td>

                    </tr>

                `;

            }



            return stocks
                .map(
                    stock => {


                        // Quantity

                        const quantity =
                            Number(
                                stock.quantity
                                || 0
                            );



                        // Stock level

                        let level =
                            "In Stock";


                        if (
                            quantity === 0
                        ) {

                            level =
                                "Out of Stock";

                        }

                        else if (
                            quantity < 10
                        ) {

                            level =
                                "Low";

                        }



                        // Expiry date

                        let expiry =
                            "-";


                        if (
                            stock.expiryDate
                        ) {

                            expiry =
                                new Date(
                                    stock.expiryDate
                                )
                                .toLocaleDateString();

                        }



                        return `


                            <tr>


                                <td>

                                    ${
                                        stock.medicineName
                                        ||
                                        "-"
                                    }

                                </td>


                                <td>

                                    ${
                                        stock.categoryName
                                        ||
                                        "-"
                                    }

                                </td>


                                <td
                                    class="text-end"
                                >

                                    ${quantity}

                                </td>


                                <td>

                                    ${expiry}

                                </td>


                                <td
                                    class="text-end"
                                >

                                    ${level}

                                </td>


                            </tr>


                        `;

                    }
                )
                .join("");

        }


    }
);