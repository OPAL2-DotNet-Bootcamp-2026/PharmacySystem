document.addEventListener("DOMContentLoaded", () => {

    // =========================================
    // CHECK LOGIN
    // =========================================

    if (!Auth.isLoggedIn()) {

        window.location.href = "login.html";

        return;
    }


    // =========================================
    // ADMIN ONLY
    // =========================================

    if (Auth.role() !== "Admin") {

        window.location.href =
            "dashboard.html#" +
            Auth.role().toLowerCase();

        return;
    }



    // =========================================
    // FORM ELEMENTS
    // =========================================

    const form = document.getElementById("create-user-form");

    const usernameInput =document.getElementById("username");

    const emailInput =document.getElementById("email");

    const passwordInput =document.getElementById("password");

    const roleInput =document.getElementById("role");
 
    const button =document.querySelector(".btn-create");
        
    const messageBox =document.getElementById("create-user-message");
        



    // =========================================
    // TABLE ELEMENTS
    // =========================================

    const usersTableBody = document.getElementById("users-table-body");
       
    const usersSummary =document.getElementById("users-summary");
        
    const usersMessage =document.getElementById("users-message");
        



    // =========================================
    // LOAD USERS FROM DATABASE
    // =========================================

    async function loadUsers() {

        try {

            usersMessage.style.color ="#5E6F66";      

            usersMessage.textContent ="Loading users...";
                
            // Call Backend
            const users =await Api.get("/User/GetAllUsers" );
                
            console.log("Users:",users);
                
                

            // Clear old table
            usersTableBody.innerHTML = "";


            // Update account count
            usersSummary.textContent =
                users.length +
                (users.length === 1
                    ? " account"
                    : " accounts");



            usersMessage.textContent = "";



            // No users
            if (users.length === 0) {

                usersTableBody.innerHTML = `

                    <tr>

                        <td
                            colspan="5"
                            class="text-center py-4"
                        >
                            No users found.
                        </td>

                    </tr>

                `;

                return;
            }



            // =========================================
            // CREATE TABLE ROWS
            // =========================================

            users.forEach(user => {


                const row = document.createElement("tr");
                   



                // Role color
                let roleClass ="badge-pharmacist";
                    


                if (user.role === "Admin") {

                    roleClass ="badge-admin";
                        

                }

                else if (user.role === "Manager") {

                    roleClass ="badge-manager";
                        

                }



                row.innerHTML = `

                    <td>
                        ${user.username}
                    </td>


                    <td class="muted-cell">
                        ${user.email}
                    </td>


                    <td>

                        <span
                            class="badge-role ${roleClass}"
                        >
                            ${user.role}
                        </span>

                    </td>


                    <td>

                        <span
                            class="badge-status badge-on"
                        >
                            Active
                        </span>

                    </td>


                    <td class="text-end">

                        <button
                            type="button"
                            class="btn-delete"
                            data-user-id="${user.userID}"
                        >
                            Delete
                        </button>

                    </td>

                `;


                usersTableBody.appendChild(row);

            });

        }

        catch (err) {

            console.error("Load users error:", err );          

            usersMessage.style.color = "#d33";
               
            usersMessage.textContent ="Could not load users: " +err.message;

                
                
        }

    }



    // =========================================
    // CREATE USER
    // =========================================

    form.addEventListener(
        "submit",
        async (e) => {

            e.preventDefault();


            messageBox.textContent = "";


            const username =usernameInput.value.trim();
                
            const email =emailInput.value.trim();   

            const password = passwordInput.value;
               
            const role =roleInput.value;
                



            // =================================
            // VALIDATION
            // =================================

            if (
                !username ||
                !email ||
                !password ||
                !role
            ) {

                messageBox.style.color = "#d33";               

                messageBox.textContent = "Please complete all fields.";
                   
                return;
            }



            if (password.length < 8) {

                messageBox.style.color ="#d33";
                    
                messageBox.textContent ="Password must be at least 8 characters.";
 
                return;
            }



            // =================================
            // LOADING BUTTON
            // =================================

            button.disabled = true;

            const originalText =button.innerHTML;           

            button.textContent ="Creating...";


            try {


                // =================================
                // CREATE USER IN DATABASE
                // =================================

                const result =
                    await Api.post("/User/create",
                        
                        {
                            username: username,
                            email: email,
                            password: password,
                            role: role
                        }
                    );



                console.log("Created user:",result );
                    
                    
            

                // =================================
                // SUCCESS
                // =================================

                messageBox.style.color ="#1F8A70";
                    
                messageBox.textContent ="User created successfully.";
                    



                // Clear form
                form.reset();


                await loadUsers();

            }

            catch (err) {

                messageBox.style.color = "#d33";

                messageBox.textContent =err.message;
                    

            }

            finally {

                button.disabled = false;


                button.innerHTML =originalText;
                    

            }

        }
    );



    // =========================================
    // DELETE USER
    // =========================================

    usersTableBody.addEventListener(
        "click",
        async (e) => {


            const deleteButton =
                e.target.closest(
                    ".btn-delete"
                );


            if (!deleteButton) {

                return;

            }



            const userId =deleteButton.dataset.userId;
                

            if (!userId) {

                return;

            }



            const confirmed = confirm("Are you sure you want to deactivate this user?");
               
                    

            if (!confirmed) {

                return;

            }



            deleteButton.disabled = true;


            deleteButton.textContent ="Deleting...";
                



            try {


                await Api.del(`/User/DeleteUser/${userId}`);

                    
              


                usersMessage.style.color = "#1F8A70";
                   


                usersMessage.textContent ="User deactivated successfully.";
                    



                // Reload table
                await loadUsers();

            }

            catch (err) {


                usersMessage.style.color =  "#d33";           

                usersMessage.textContent = err.message;
                
                deleteButton.disabled =false;
                    
                deleteButton.textContent ="Delete";

                    
            }

        }
    );



    // =========================================
    // LOAD USERS WHEN PAGE OPENS
    // =========================================

    loadUsers();

});