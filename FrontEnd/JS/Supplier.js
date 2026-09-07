


document.addEventListener("DOMContentLoaded", function () {


    // LOGIN 

    if (!Auth.isLoggedIn()) {

        window.location.href = "login.html";

        return;

    }


    const role = Auth.role();

    const canEdit =role === "Admin" || role === "Manager";
        

    const canDelete = role === "Admin";
       



    // GET HTML ELEMENTS

    const addSupplierBtn =document.getElementById("addSupplierBtn");

    const supplierModal =document.getElementById("supplierModal");

    const closeModalBtn =document.getElementById("closeModalBtn");

    const cancelSupplierBtn =document.getElementById("cancelSupplierBtn");

    const supplierForm =document.getElementById("supplierForm");

    const supplierTableBody =document.getElementById("supplierTableBody");

    const supplierCount =document.getElementById("supplierCount");

    const modalTitle =document.getElementById("modalTitle");   

    const modalDescription = document.getElementById("modalDescription");

    const saveSupplierBtn =document.getElementById("saveSupplierBtn");

    const successMessage =document.getElementById("successMessage"); 

    const successTitle =document.getElementById("successTitle");
  
    const successText =document.getElementById("successText");
        





    let suppliers = [];

    let editingSupplierId = null;



    // Hide "Add supplier" for roles that cannot create

    if (!canEdit) {addSupplierBtn.style.display = "none";}

        



    function normalise(item) {

        return {

            id:
                item.supplierID ??
                item.SupplierID,

            name:
                item.fullName ??
                item.FullName ??
                "",

            phone:
                item.phone ??
                item.Phone ??
                "",

            email:
                item.email ??
                item.Email ??
                "",

            location:
                item.location ??
                item.Location ??
                "",

            isActive:
                (item.isActive ??
                    item.IsActive) === true

        };

    }



    // ==========================================
    // LOAD SUPPLIERS FROM THE DATABASE
    // GET /api/Supplier/GetAll
    // ==========================================

    async function loadSuppliers() {

        showTableMessage("Loading suppliers...");


        try {

            const data = await Api.get("/Supplier/GetAll");     


            suppliers =(data || []).map(normalise);
            
            displaySuppliers();

        } catch (error) {

            console.error(error);


            showTableMessage( error.message ||"Could not load suppliers.");

            supplierCount.textContent = "0 records";

        }

    }



    // HELPER
    // Show one message row inside the table

    function showTableMessage(text) {

        supplierTableBody.innerHTML = `

            <tr>

                <td
                    colspan="5"
                    class="no-suppliers">

                    ${escapeHTML(text)}

                </td>

            </tr>

        `;

    }



    // DISPLAY SUPPLIERS

    function displaySuppliers() {

        supplierTableBody.innerHTML = "";


        supplierCount.textContent =`${suppliers.length} records`;
            
        if (suppliers.length === 0) {

            showTableMessage("No suppliers found.");

            return;

        }



        suppliers.forEach(function (supplier) {


            const row = document.createElement("tr");

            const statusClass =
                supplier.isActive
                    ? "active"
                    : "inactive";


            const statusLabel =
                supplier.isActive
                    ? "ACTIVE"
                    : "INACTIVE";



            // Buttons depend on the user's role

            let actionsHTML = "";


            if (canEdit) {

                actionsHTML += `

                    <button
                        type="button"
                        class="btn-edit-supplier"
                        data-id="${supplier.id}">

                        Edit

                    </button>

                `;

            }


            if (canDelete) {

                actionsHTML += `

                    <button
                        type="button"
                        class="btn-delete-supplier"
                        data-id="${supplier.id}">

                        Delete

                    </button>

                `;

            }



            row.innerHTML = `

                <!-- Supplier -->

                <td>

                    <span class="supplier-name">

                        ${escapeHTML(supplier.name)}

                    </span>


                    <span class="supplier-email">

                        ${escapeHTML(supplier.email)}

                    </span>

                </td>


                <!-- Phone -->

                <td>

                    ${escapeHTML(supplier.phone)}

                </td>


                <!-- Location -->

                <td>

                    ${escapeHTML(supplier.location)}

                </td>


                <!-- Status -->

                <td>

                    <span class="status ${statusClass}">

                        <span class="status-dot"></span>

                        ${statusLabel}

                    </span>

                </td>


                <!-- Actions -->

                <td>

                    <div class="supplier-actions">

                        ${actionsHTML}

                    </div>

                </td>

            `;


            supplierTableBody.appendChild(row);

        });

    }



    // OPEN ADD SUPPLIER MODAL

    addSupplierBtn.addEventListener(
        "click",
        function () {

            editingSupplierId = null;


            modalTitle.textContent = "Add supplier";
            

            modalDescription.textContent ="Enter supplier information.";        

            saveSupplierBtn.textContent ="Add supplier";
                
            supplierForm.reset();

            document.getElementById("supplierStatus" ).value = "Active";
                       
            supplierModal.classList.add("show");


            setTimeout(function () {

                document .getElementById("supplierName").focus();         

            }, 100);

        }
    );



    // CLOSE MODAL

    function closeSupplierModal() {

        supplierModal.classList.remove("show");

        supplierForm.reset();

        editingSupplierId = null;

    }



    cancelSupplierBtn.addEventListener("click",closeSupplierModal);
             


    closeModalBtn.addEventListener("click",closeSupplierModal);
        

    // Click outside the modal

    supplierModal.addEventListener(
        "click",
        function (event) {

            if (event.target === supplierModal) {

                closeSupplierModal();

            }

        }
    );



    // Escape key

    document.addEventListener(
        "keydown",
        function (event) {

            if (
                event.key === "Escape" &&
                supplierModal.classList.contains("show")
            ) {

                closeSupplierModal();

            }

        }
    );



    // ==========================================
    // FORM SUBMIT
    // Create (POST) or Update (PUT)
    // ==========================================

    supplierForm.addEventListener(
        "submit",
        async function (event) {

            event.preventDefault();



            const name =document.getElementById("supplierName").value.trim();
                

            const phone =document.getElementById("supplierPhone").value.trim();
                             

            const email =document.getElementById("supplierEmail").value.trim();
          
                    

            const location =document.getElementById("supplierLocation").value.trim();
                
                

            const status =document.getElementById("supplierStatus").value;
                
                    
            
            // Required fields

            if (
                !name ||
                !phone ||
                !email ||
                !location
            ) {

                alert("Please fill in all supplier information." );                 

                return;

            }



            // The backend requires an email ending in .com

            if (!/.+@.+\.com$/.test(email)) {

                alert("Email must contain @ and end with .com" );
                    
                return;

            }



            // Phone column is only 13 characters

            if (phone.length > 13) {

                alert("Phone number cannot be longer than 13 characters." );
                    

                return;

            }



            // Property names must match the C# DTO

            const body = {

                fullName: name,

                phone: phone,

                email: email,

                location: location,

                isActive: status === "Active"

            };



            // Block double clicks

            const originalButtonText =saveSupplierBtn.textContent;
                

            saveSupplierBtn.disabled = true;

            saveSupplierBtn.textContent = "Saving...";



            try {


                // ==================================
                // UPDATE
                // PUT /api/Supplier/Update/5
                // ==================================

                if (editingSupplierId !== null) {


                    await Api.put(`/Supplier/Update/${editingSupplierId}`,body);
                        
                                      
                    closeSupplierModal();

                    await loadSuppliers();


                    showSuccessMessage("Supplier updated",`${name} was updated successfully.`);
                                             

                    return;

                }



                // ==================================
                // CREATE
                // POST /api/Supplier/Create
                // ==================================

                await Api.post("/Supplier/Create",body);
                    
                

                closeSupplierModal();

                await loadSuppliers();


                showSuccessMessage("Supplier added", `${name} was added successfully.`);
                    
                            

            } catch (error) {

                console.error(error);


                // Shows the real reason
                // "Supplier email already exists"

                alert(error.message ||"Could not save the supplier.");                            

            } finally {

                saveSupplierBtn.disabled = false;

                saveSupplierBtn.textContent =originalButtonText;
                    

            }

        }
    );



    // ==========================================
    // TABLE BUTTONS
    // EDIT + DELETE
    // ==========================================

    supplierTableBody.addEventListener(
        "click",
        function (event) {


            if (event.target.classList.contains("btn-edit-supplier") )
               
                
            {

                editSupplier( Number(event.target.dataset.id));     
                
            }



            if (event.target.classList.contains("btn-delete-supplier") ) 
                     
           {

                deleteSupplier(
                    Number(event.target.dataset.id)
                );

            }

        }
    );



    // ==========================================
    // EDIT SUPPLIER
    // Fills the form from the loaded list
    // ==========================================

    function editSupplier(id) {


        const supplier =
            suppliers.find(
                function (item) {

                    return item.id === id;

                }
            );


        if (!supplier) {

            return;

        }



        editingSupplierId = id;



        modalTitle.textContent = "Edit supplier";
           


        modalDescription.textContent ="Update supplier information.";     

        saveSupplierBtn.textContent ="Save changes";

            


        document.getElementById(
            "supplierName"
        ).value = supplier.name;


        document.getElementById(
            "supplierPhone"
        ).value = supplier.phone;


        document.getElementById(
            "supplierEmail"
        ).value = supplier.email;


        document.getElementById(
            "supplierLocation"
        ).value = supplier.location;


        document.getElementById(
            "supplierStatus"
        ).value =
            supplier.isActive
                ? "Active"
                : "Inactive";



        supplierModal.classList.add("show");

    }



    // ==========================================
    // DELETE SUPPLIER
    // DELETE /api/Supplier/Delete/5
    // ==========================================

    async function deleteSupplier(id) {


        const supplier =
            suppliers.find(
                function (item) {

                    return item.id === id;

                }
            );


        if (!supplier) {

            return;

        }

        const confirmed =confirm(`Are you sure you want to delete ${supplier.name}?`);
            

        if (!confirmed) {

            return;

        }


//delete supplier from the database( soft delete )
        try {


            await Api.del(`/Supplier/Delete/${id}` );
                
            await loadSuppliers();

            showSuccessMessage(
                "Supplier deleted",
                `${supplier.name} was deleted successfully.`
            );


        } catch (error) {

            console.error(error);


            alert(error.message ||"Could not delete the supplier."  );
                

        }

    }



    // ==========================================
    // SUCCESS MESSAGE
    // ==========================================

    let successTimer;


    function showSuccessMessage(title, message) {


        successTitle.textContent = title;

        successText.textContent = message;


        successMessage.classList.add("show");


        clearTimeout(successTimer);


        successTimer =
            setTimeout(
                function () {

                    successMessage
                        .classList
                        .remove("show");

                },
                3000
            );

    }



    // ESCAPE HTML
    // Prevent user input from becoming HTML

    function escapeHTML(value) {

        const div = document.createElement("div");

        div.textContent = value ?? "";
        return div.innerHTML;

    }



    //  PAGE LOAD
    // 

    loadSuppliers();


});