// ==========================================
// TYPES
// ==========================================

interface PharmacyInfo{
    pharmacyID:number;
    pharmacyName?:string;
    location?:string;
}

interface PharmacyStock{
    medicineName?:string;
    categoryName?:string;
    quantity?:number;
    expiryDate?:string;
}

// ==========================================
// PAGE LOAD
// ==========================================

document.addEventListener("DOMContentLoaded",async():Promise<void>=>{

    // CHECK LOGIN
    if(!Auth.isLoggedIn()){
        window.location.href="login.html";
        return;
    }

    // GET USER ROLE
    const role:string=Auth.role()||"";
    const roleKey:string=role.toLowerCase();

    if(window.location.hash!=="#"+roleKey){
        window.location.hash=roleKey;
    }

    // GET MAIN CONTENT
    const mainContent=document.querySelector<HTMLElement>(".main-content");
    const footerContainer=document.getElementById("footer-container");

    if(!mainContent||!footerContainer){
        return;
    }

    // REMOVE OLD HARDCODED STOCK CARDS
    const oldCards=document.querySelectorAll<HTMLElement>(".stock-card");

    oldCards.forEach((card:HTMLElement):void=>{
        card.remove();
    });

    // CREATE DATABASE CONTAINER
    const container=document.createElement("div");
    container.id="pharmacyStockContainer";

    container.innerHTML=`
        <section class="stock-card">
            <p>Loading pharmacy stock...</p>
        </section>
    `;

    mainContent.insertBefore(container,footerContainer);

    // LOAD PHARMACIES
    try{

        const pharmacies=
            await Api.get<PharmacyInfo[]>("/Pharmacy");

        console.log("Pharmacies:",pharmacies);

        container.innerHTML="";

        if(!pharmacies||pharmacies.length===0){

            container.innerHTML=`
                <section class="stock-card">
                    <p>No pharmacies found.</p>
                </section>
            `;

            return;
        }

        // LOOP THROUGH PHARMACIES
        for(const pharmacy of pharmacies){

            const pharmacyID:number=
                pharmacy.pharmacyID;

            let stocks:PharmacyStock[]=[];

            try{

                stocks=
                    await Api.get<PharmacyStock[]>(
                        "/PharmacyStock/by-pharmacy/"+pharmacyID
                    );

            }catch(error:unknown){

                console.error(
                    "Stock error for pharmacy:",
                    pharmacyID,
                    error
                );

                stocks=[];
            }

            // CREATE PHARMACY CARD
            const section=
                document.createElement("section");

            section.className="stock-card";

            section.innerHTML=`
                <div class="stock-card-header">

                    <div>
                        <h2>${pharmacy.pharmacyName||"Pharmacy"}</h2>
                        <p>${pharmacy.location||""}</p>
                    </div>

                    <span class="stock-summary">
                        ${stocks.length} medicine(s)
                    </span>

                </div>

                <div class="table-responsive">

                    <table class="table stock-table">

                        <thead>
                            <tr>
                                <th>MEDICINE</th>
                                <th>CATEGORY</th>
                                <th class="text-end">QUANTITY</th>
                                <th>EXPIRY</th>
                                <th class="text-end">LEVEL</th>
                            </tr>
                        </thead>

                        <tbody>
                            ${createStockRows(stocks)}
                        </tbody>

                    </table>

                </div>
            `;

            container.appendChild(section);
        }

    }catch(error:unknown){

        console.error(
            "Failed to load pharmacies:",
            error
        );

        container.innerHTML=`
            <section class="stock-card">
                <p>Failed to load pharmacy stock.</p>
            </section>
        `;
    }

    // ==========================================
    // CREATE TABLE ROWS
    // ==========================================

    function createStockRows(
        stocks:PharmacyStock[]
    ):string{

        if(!stocks||stocks.length===0){

            return`
                <tr>
                    <td colspan="5" class="text-center">
                        No stock available.
                    </td>
                </tr>
            `;
        }

        return stocks
            .map((stock:PharmacyStock):string=>{

                const quantity:number=
                    Number(stock.quantity||0);

                let level:string="In Stock";

                if(quantity===0){
                    level="Out of Stock";
                }
                else if(quantity<10){
                    level="Low";
                }

                let expiry:string="-";

                if(stock.expiryDate){
                    expiry=
                        new Date(
                            stock.expiryDate
                        ).toLocaleDateString();
                }

                return`
                    <tr>
                        <td>${stock.medicineName||"-"}</td>
                        <td>${stock.categoryName||"-"}</td>
                        <td class="text-end">${quantity}</td>
                        <td>${expiry}</td>
                        <td class="text-end">${level}</td>
                    </tr>
                `;
            })
            .join("");
    }

});